///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListDiskStatuses.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.ddm;

	/// <summary>
	/// Represents the information returned by the WRKDSKSTS command.
	/// 
	/// </summary>
	public class ListDiskStatuses
	{
	  private readonly ListDiskStatusesImpl impl_ = new ListDiskStatusesImpl();

	  public ListDiskStatuses()
	  {
	  }

	  public virtual string ElapsedTime
	  {
		  get
		  {
			return impl_.ElapsedTime;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiskStatus[] getDiskStatuses(final CommandConnection cmdConn, final DDMConnection ddmConn, String workingLibrary) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual DiskStatus[] getDiskStatuses(CommandConnection cmdConn, DDMConnection ddmConn, string workingLibrary)
	  {
		return impl_.getDiskStatuses(cmdConn, ddmConn, workingLibrary,false);
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiskStatus[] getDiskStatuses(final CommandConnection cmdConn, final DDMConnection ddmConn, String workingLibrary, boolean reset) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual DiskStatus[] getDiskStatuses(CommandConnection cmdConn, DDMConnection ddmConn, string workingLibrary, bool reset)
	  {
		return impl_.getDiskStatuses(cmdConn, ddmConn, workingLibrary, reset);
	  }

	}


}