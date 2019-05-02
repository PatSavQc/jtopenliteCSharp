using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFormatOSPL0300.java
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

	public class OpenListOfSpooledFilesFormatOSPL0300 : OpenListOfSpooledFilesFormat<OpenListOfSpooledFilesFormatOSPL0300Listener>
	{
	  private readonly char[] charBuffer_ = new char[10];

	  public OpenListOfSpooledFilesFormatOSPL0300()
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "OSPL0300";
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return OpenListOfSpooledFilesFormat_Fields.FORMAT_OSPL0300;
		  }
	  }

	  private readonly sbyte[] lastJobNameBytes_ = new sbyte[10];
	  private string lastJobName_ = "          ";
	  private readonly sbyte[] lastJobUserBytes_ = new sbyte[10];
	  private string lastJobUser_ = "          ";
	  private readonly sbyte[] lastSpooledFileNameBytes_ = new sbyte[10];
	  private string lastSpooledFileName_ = "          ";
	  private readonly sbyte[] lastDateOpenedBytes_ = new sbyte[7];
	  private string lastDateOpened_ = "       ";
	  private readonly sbyte[] lastJobSystemNameBytes_ = new sbyte[10];
	  private string lastJobSystemName_ = "          ";
	  private readonly sbyte[] lastFormTypeBytes_ = new sbyte[10];
	  private string lastFormType_ = "          ";
	  private readonly sbyte[] lastOutputQueueNameBytes_ = new sbyte[10];
	  private string lastOutputQueueName_ = "          ";
	  private readonly sbyte[] lastOutputQueueLibraryBytes_ = new sbyte[10];
	  private string lastOutputQueueLibrary_ = "          ";

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
//ORIGINAL LINE: private void getJobUser(final byte[] data, final int numRead)
	  private void getJobUser(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastJobUserBytes_))
		{
		  Array.Copy(data, numRead, lastJobUserBytes_, 0, 10);
		  lastJobUser_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getSpooledFileName(final byte[] data, final int numRead)
	  private void getSpooledFileName(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastSpooledFileNameBytes_))
		{
		  Array.Copy(data, numRead, lastSpooledFileNameBytes_, 0, 10);
		  lastSpooledFileName_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getDateOpened(final byte[] data, final int numRead)
	  private void getDateOpened(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastDateOpenedBytes_))
		{
		  Array.Copy(data, numRead, lastDateOpenedBytes_, 0, 7);
		  lastDateOpened_ = Conv.ebcdicByteArrayToString(data, numRead, 7, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getJobSystemName(final byte[] data, final int numRead)
	  private void getJobSystemName(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastJobSystemNameBytes_))
		{
		  Array.Copy(data, numRead, lastJobSystemNameBytes_, 0, 10);
		  lastJobSystemName_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getFormType(final byte[] data, final int numRead)
	  private void getFormType(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastFormTypeBytes_))
		{
		  Array.Copy(data, numRead, lastFormTypeBytes_, 0, 10);
		  lastFormType_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getOutputQueueName(final byte[] data, final int numRead)
	  private void getOutputQueueName(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastOutputQueueNameBytes_))
		{
		  Array.Copy(data, numRead, lastOutputQueueNameBytes_, 0, 10);
		  lastOutputQueueName_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getOutputQueueLibrary(final byte[] data, final int numRead)
	  private void getOutputQueueLibrary(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastOutputQueueLibraryBytes_))
		{
		  Array.Copy(data, numRead, lastOutputQueueLibraryBytes_, 0, 10);
		  lastOutputQueueLibrary_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfSpooledFilesFormatOSPL0300Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfSpooledFilesFormatOSPL0300Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + 136 <= maxLength)
		{
		  //String jobName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getJobName(data, numRead);
		  string jobName = lastJobName_;
		  numRead += 10;
		  //String jobUser = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getJobUser(data, numRead);
		  string jobUser = lastJobUser_;
		  numRead += 10;
		  string jobNumber = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  //String spooledFileName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getSpooledFileName(data, numRead);
		  string spooledFileName = lastSpooledFileName_;
		  numRead += 10;
		  int spooledFileNumber = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int fileStatus = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  //String dateOpened = Conv.ebcdicByteArrayToString(data, numRead, 7, charBuffer_);
		  getDateOpened(data, numRead);
		  string dateOpened = lastDateOpened_;
		  numRead += 7;
		  string timeOpened = Conv.ebcdicByteArrayToString(data, numRead, 6, charBuffer_);
		  numRead += 6;
		  string spooledFileSchedule = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  //String jobSystemName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getJobSystemName(data, numRead);
		  string jobSystemName = lastJobSystemName_;
		  numRead += 10;
		  string userData = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
		  //String formType = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getFormType(data, numRead);
		  string formType = lastFormType_;
		  numRead += 10;
		  //String outputQueueName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getOutputQueueName(data, numRead);
		  string outputQueueName = lastOutputQueueName_;
		  numRead += 10;
		  //String outputQueueLibrary = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getOutputQueueLibrary(data, numRead);
		  string outputQueueLibrary = lastOutputQueueLibrary_;
		  numRead += 10;
		  int asp = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int size = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int multiplier = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  long totalSize = (long)size * (long)multiplier;
		  int totalPages = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  int copiesLeftToPrint = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  string priority = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 1;
		  numRead += 3;
		  int ippJobIdentifier = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  listener.newSpooledFileEntry(jobName, jobUser, jobNumber, spooledFileName, spooledFileNumber, fileStatus, dateOpened, timeOpened, spooledFileSchedule, jobSystemName, userData, formType, outputQueueName, outputQueueLibrary, asp, totalSize, totalPages, copiesLeftToPrint, priority, ippJobIdentifier);
		}
	  }
	}





}