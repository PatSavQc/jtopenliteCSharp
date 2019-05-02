using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsFormatOLJB0100.java
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

	public class OpenListOfJobsFormatOLJB0100 : OpenListOfJobsFormat<OpenListOfJobsFormatOLJB0100Listener>
	{
	  private readonly char[] charBuffer_ = new char[10];

	  public OpenListOfJobsFormatOLJB0100()
	  {
	  }

	  public virtual OpenListOfJobsKeyField[] KeyFields
	  {
		  set
		  {
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "OLJB0100";
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return OpenListOfJobsFormat_Fields.FORMAT_OLJB0100;
		  }
	  }

	  public virtual int MinimumRecordLength
	  {
		  get
		  {
			return 56;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfJobsFormatOLJB0100Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfJobsFormatOLJB0100Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + 54 <= maxLength)
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
		  listener.newJobEntry(jobNameUsed, userNameUsed, jobNumberUsed, internalJobIdentifier, status, jobType, jobSubtype);
		  if (numRead + 2 <= maxLength)
		  {
			numRead += 2;
		  }
		}
	  }
	}

}