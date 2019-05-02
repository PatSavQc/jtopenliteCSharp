using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsFormatOLJB0200.java
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

	public class OpenListOfJobsFormatOLJB0200 : OpenListOfJobsFormat<OpenListOfJobsFormatOLJB0200Listener>
	{
	  private OpenListOfJobsKeyField[] keyFields_;
	  private readonly char[] charBuffer_ = new char[100];

	  public OpenListOfJobsFormatOLJB0200()
	  {
	  }

	  public OpenListOfJobsFormatOLJB0200(OpenListOfJobsKeyField[] keyFields)
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
			return "OLJB0200";
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return OpenListOfJobsFormat_Fields.FORMAT_OLJB0200;
		  }
	  }

	  public virtual int MinimumRecordLength
	  {
		  get
		  {
			return 60;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfJobsFormatOLJB0200Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfJobsFormatOLJB0200Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + recordLength <= maxLength)
		{
		  string jobNameUsed = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string userNameUsed = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string jobNumberUsed = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  sbyte[] internalJobIdentifier = new sbyte[16];
		  Array.Copy(data, numRead, internalJobIdentifier, 0, 16);
		  numRead += 16;
		  string status = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string jobType = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  string jobSubtype = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  numRead += 2;
		  int jobInfoStatus = data[numRead] & 0x00FF;
		  bool infoStatus = jobInfoStatus == 0x40;
		  numRead += 4;
		  listener.newJobEntry(jobNameUsed, userNameUsed, jobNumberUsed, internalJobIdentifier, status, jobType, jobSubtype, infoStatus);
		  int recordOffset = 60;
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
			  Util.readKeyData(data, numRead, key, length, isBinary, listener, charBuffer_);
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