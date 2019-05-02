using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListUsersImpl.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.ddm;

	internal sealed class ListUsersImpl : DDMReadCallback, UserInfoListener
	{
	  private DDMRecordFormat rf_;

	  private UserInfoListener uiListener_;

	  internal ListUsersImpl()
	  {
	  }

	  public UserInfoListener UserInfoListener
	  {
		  set
		  {
			uiListener_ = value;
		  }
	  }

	  public void totalRecords(long total)
	  {
	  }

	  public void newUserInfo(UserInfo info)
	  {
		users_.Add(info);
	  }

	  private readonly List<UserInfo> users_ = new List<UserInfo>();
	  private bool done_ = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void newRecord(DDMCallbackEvent event, DDMDataBuffer dataBuffer) throws IOException
	  public void newRecord(DDMCallbackEvent @event, DDMDataBuffer dataBuffer)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] data = dataBuffer.getRecordDataBuffer();
		sbyte[] data = dataBuffer.RecordDataBuffer;
		string userName = rf_.getField("UPUPRF").getString(data);
		string userClass = rf_.getField("UPUSCL").getString(data);
		string passwordExpired = rf_.getField("UPPWEX").getString(data);
		long maxStorage = rf_.getField("UPMXST").getLong(data);
		long storageUsed = rf_.getField("UPMXSU").getLong(data);
		string description = rf_.getField("UPTEXT").getString(data);
		string locked = rf_.getField("UPUPLK").getString(data);
		string damaged = rf_.getField("UPUPDM").getString(data);
		string status = rf_.getField("UPSTAT").getString(data);
		long uid = rf_.getField("UPUID").getLong(data);
		long gid = rf_.getField("UPGID").getLong(data);
		UserInfo ui = new UserInfo(userName, userClass, passwordExpired, maxStorage, storageUsed, description, locked, damaged, status, uid, gid);
	//    users_.addElement(ui);
		uiListener_.newUserInfo(ui);
	  }

	  public void recordNotFound(DDMCallbackEvent @event)
	  {
		done_ = true;
	  }

	  public void endOfFile(DDMCallbackEvent @event)
	  {
		done_ = true;
	  }

	  private bool done()
	  {
		return done_;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UserInfo[] getUsers(final DDMConnection ddmConn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public UserInfo[] getUsers(DDMConnection ddmConn)
	  {
		IList<Message> messages = ddmConn.executeReturnMessageList("DSPUSRPRF USRPRF(*ALL) TYPE(*BASIC) OUTPUT(*OUTFILE) OUTFILE(QTEMP/TBALLUSERS)");
		if (messages.Count > 0)
		{
		  if (messages.Count != 1 && !messages[0].ID.Equals("CPF9861")) // Output file created.
		  {
			throw new MessageException("Error retrieving users: ", messages);
		  }
		}

		users_.Clear();
		if (rf_ == null)
		{
		  rf_ = ddmConn.getRecordFormat("QTEMP", "TBALLUSERS");
		  rf_.getField("UPUSCL").CacheStrings = true;
		  rf_.getField("UPPWEX").CacheStrings = true;
		  rf_.getField("UPUPLK").CacheStrings = true;
		  rf_.getField("UPUPDM").CacheStrings = true;
		  rf_.getField("UPSTAT").CacheStrings = true;
		}

		done_ = false;

		DDMFile file = ddmConn.open("QTEMP", "TBALLUSERS", "TBALLUSERS", "QSYDSUPB", DDMFile.READ_ONLY, false, 160, 1);

		IList<DDMFileMemberDescription> desc = ddmConn.getFileMemberDescriptions(file);
		if (desc != null && desc.Count > 0)
		{
		  uiListener_.totalRecords(desc[0].RecordCount);
		}

		while (!done())
		{
		  ddmConn.readNext(file, this);
		}
		ddmConn.close(file);

		UserInfo[] arr = new UserInfo[users_.Count];
		return (UserInfo[])users_.toArray(arr);
	  }
	}

}