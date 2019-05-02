﻿using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFormatOSPL0200.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{
	using com.ibm.jtopenlite;

	public class OpenListOfSpooledFilesFormatOSPL0200 : OpenListOfSpooledFilesFormat<OpenListOfSpooledFilesFormatOSPL0200Listener>
	{
	  private readonly char[] charBuffer_ = new char[10];

	  public OpenListOfSpooledFilesFormatOSPL0200()
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "OSPL0200";
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return OpenListOfSpooledFilesFormat_Fields.FORMAT_OSPL0200;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfSpooledFilesFormatOSPL0200Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfSpooledFilesFormatOSPL0200Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + 184 <= maxLength)
		{
		  int current = numRead;
		  string spooledFileName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string jobName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string jobUser = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string jobNumber = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  int spooledFileNumber = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int totalPages = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int currentPage = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int copiesLeftToPrint = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  string outputQueueName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string outputQueueLibrary = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string userData = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string status = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string formType = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  string priority = Conv.ebcdicByteArrayToString(data, numRead, 2, charBuffer_);
		  numRead += 2;
		  sbyte[] internalJobIdentifier = new sbyte[16];
		  Array.Copy(data, numRead, internalJobIdentifier, 0, 16);
		  numRead += 16;
		  sbyte[] internalSpooledFileIdentifier = new sbyte[16];
		  Array.Copy(data, numRead, internalSpooledFileIdentifier, 0, 16);
		  numRead += 16;
		  string deviceType = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  numRead += 2;
		  int offsetToExtension = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int lengthOfExtension = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  numRead += 4;
		  string dateOpened = Conv.ebcdicByteArrayToString(data, numRead, 7, charBuffer_);
		  numRead += 7;
		  string timeOpened = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  string printerAssigned = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  string printerName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  numRead += 8;
		  string jobSystemName = null;
		  if (offsetToExtension > 0 && lengthOfExtension > 0)
		  {
			numRead += (numRead - current - offsetToExtension);
			jobSystemName = Conv.ebcdicByteArrayToString(data, numRead, 8, charBuffer_);
		  }
		  listener.newSpooledFileEntry(spooledFileName, jobName, jobUser, jobNumber, spooledFileNumber, totalPages, currentPage, copiesLeftToPrint, outputQueueName, outputQueueLibrary, userData, status, formType, priority, internalJobIdentifier, internalSpooledFileIdentifier, deviceType, dateOpened, timeOpened, printerAssigned, printerName, jobSystemName);
		}
	  }
	}





}