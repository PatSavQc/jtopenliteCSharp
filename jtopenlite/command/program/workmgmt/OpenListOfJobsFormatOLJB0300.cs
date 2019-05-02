using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsFormatOLJB0300.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command.program;

	public class OpenListOfJobsFormatOLJB0300 : OpenListOfJobsFormat<OpenListOfJobsFormatOLJB0300Listener>
	{
	  private OpenListOfJobsKeyField[] keyFields_;
	  private readonly char[] charBuffer_ = new char[100];

	  public OpenListOfJobsFormatOLJB0300()
	  {
	  }

	  public OpenListOfJobsFormatOLJB0300(OpenListOfJobsKeyField[] keyFields)
	  {
		keyFields_ = keyFields;
	  }

	  public virtual OpenListOfJobsKeyField[] KeyFields
	  {
		  get
		  {
			return keyFields_;
		  }
		  set
		  {
			keyFields_ = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return "OLJB0300";
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return OpenListOfJobsFormat_Fields.FORMAT_OLJB0300;
		  }
	  }

	  public virtual int MinimumRecordLength
	  {
		  get
		  {
			return 40;
		  }
	  }

	  private readonly sbyte[] lastJobNameBytes_ = new sbyte[10];
	  private string lastJobName_ = "          ";
	  private readonly sbyte[] lastUserNameBytes_ = new sbyte[10];
	  private string lastUserName_ = "          ";
	  private readonly sbyte[] lastMemoryPoolBytes_ = new sbyte[10];
	  private string lastMemoryPool_ = "          ";
	  private readonly sbyte[] lastCurrentUserProfileBytes_ = new sbyte[10];
	  private string lastCurrentUserProfile_ = "          ";
	  private readonly sbyte[] lastSubsystemBytes_ = new sbyte[20];
	  private string lastSubsystem_ = "                    ";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static boolean matches(final byte[] data, final int offset, final byte[] data2)
	  private static bool matches(sbyte[] data, int offset, sbyte[] data2)
	  {
		for (int i = 0; i < data2.Length; ++i)
		{
		  if (data[offset + i] != data2[i])
		  {
			  return false;
		  }
		}
		return true;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getJobName(final byte[] data, final int numRead)
	  private void getJobName(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastJobNameBytes_))
		{
		  Array.Copy(data, numRead, lastJobNameBytes_, 0, 10);
		  lastJobName_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getUserName(final byte[] data, final int numRead)
	  private void getUserName(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastUserNameBytes_))
		{
		  Array.Copy(data, numRead, lastUserNameBytes_, 0, 10);
		  lastUserName_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getMemoryPool(final byte[] data, final int numRead)
	  private void getMemoryPool(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastMemoryPoolBytes_))
		{
		  Array.Copy(data, numRead, lastMemoryPoolBytes_, 0, 10);
		  lastMemoryPool_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getCurrentUserProfile(final byte[] data, final int numRead)
	  private void getCurrentUserProfile(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastCurrentUserProfileBytes_))
		{
		  Array.Copy(data, numRead, lastCurrentUserProfileBytes_, 0, 10);
		  lastCurrentUserProfile_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getSubsystem(final byte[] data, final int numRead)
	  private void getSubsystem(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastSubsystemBytes_))
		{
		  Array.Copy(data, numRead, lastSubsystemBytes_, 0, 20);
		  lastSubsystem_ = Conv.ebcdicByteArrayToString(data, numRead, 20, charBuffer_);
		}
	  }

	  private readonly HashObject hashObject_ = new HashObject();
	  private readonly Dictionary<HashObject, string> statusCache_ = new Dictionary<HashObject, string>();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private String getActiveJobStatus(final byte[] data, final int numRead)
	  private string getActiveJobStatus(sbyte[] data, int numRead)
	  {
		int num = Conv.byteArrayToInt(data, numRead);
		hashObject_.Hash = num;
		string status = (string)statusCache_[hashObject_];
		if (string.ReferenceEquals(status, null))
		{
		  HashObject obj = new HashObject();
		  obj.Hash = num;
		  status = Conv.ebcdicByteArrayToString(data, numRead, 4, charBuffer_);
		  statusCache_[obj] = status;
		}
		return status;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfJobsFormatOLJB0300Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfJobsFormatOLJB0300Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + 36 <= maxLength)
		{
		  getJobName(data, numRead);
		  string jobNameUsed = lastJobName_;
		  numRead += 10;
		  getUserName(data, numRead);
		  string userNameUsed = lastUserName_;
		  numRead += 10;
		  string jobNumberUsed = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  string activeJobStatus = getActiveJobStatus(data, numRead);
		  numRead += 4;
		  string jobType = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  string jobSubtype = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  // int totalLengthOfDataReturned = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  listener.newJobEntry(jobNameUsed, userNameUsed, jobNumberUsed, activeJobStatus, jobType, jobSubtype);
		  if (numRead + 4 <= maxLength)
		  {
			numRead += 4;
			int recordOffset = 40;
			if (keyFields_ != null)
			{
			  for (int i = 0; i < keyFields_.Length; ++i)
			  {
				int skip = keyFields_[i].Displacement - recordOffset;
				numRead += skip;
				recordOffset += skip;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = keyFields_[i].getKey();
				int key = keyFields_[i].Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = keyFields_[i].getLength();
				int length = keyFields_[i].Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isBinary = keyFields_[i].isBinary();
				bool isBinary = keyFields_[i].Binary;
				if (key == 1306)
				{
				  // Memory pool.
				  getMemoryPool(data, numRead);
				  listener.newKeyData(key, lastMemoryPool_, data, numRead);
				}
				else if (key == 305)
				{
				  // Current user profile.
				  getCurrentUserProfile(data, numRead);
				  listener.newKeyData(key, lastCurrentUserProfile_, data, numRead);
				}
				else if (key == 1906)
				{
				  // Subsystem.
				  getSubsystem(data, numRead);
				  listener.newKeyData(key, lastSubsystem_, data, numRead);
				}
				else
				{
				  Util.readKeyData(data, numRead, key, length, isBinary, listener, charBuffer_);
				}
				numRead += length;
				recordOffset += length;
			  }
			}
			int skip = recordLength - recordOffset;
			if (skip > 0)
			{
				numRead += skip;
			}
		  }
		}
	  }
	}

}