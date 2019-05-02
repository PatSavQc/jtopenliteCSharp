///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: .java
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
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.openlist;

	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qgyolspl.htm">QGYOLSPL</a>
	/// This class fully implements the V5R4 specification of QGYOLSPL.
	/// 
	/// </summary>
	public class OpenListOfSpooledFiles : OpenListProgram<OpenListOfSpooledFilesFormat, OpenListOfSpooledFilesFormatListener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private OpenListOfSpooledFilesFormat formatter_;
	  private OpenListOfSpooledFilesFormatListener formatListener_;
	  private int inputLength_;
	  private int numberOfRecordsToReturn_;
	  private SortListener sortListener_;
	  private string jobName_;
	  private string jobUser_;
	  private string jobNumber_;
	  private OpenListOfSpooledFilesFilterListener filterListener_;
	  private ListInformation info_;

	  private sbyte[] tempData_;

	  public OpenListOfSpooledFiles()
	  {
	  }

	  public OpenListOfSpooledFiles(OpenListOfSpooledFilesFormat format, int lengthOfReceiverVariable, int numberOfRecordsToReturn, SortListener sortInformation, OpenListOfSpooledFilesFilterListener filterInformation, string jobName, string jobUser, string jobNumber)
	  {
		formatter_ = format;
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		sortListener_ = sortInformation;
		filterListener_ = filterInformation;
		jobName_ = jobName;
		jobUser_ = jobUser;
		jobNumber_ = jobNumber;
	  }

	  public sbyte[] TempDataBuffer
	  {
		  get
		  {
			int maxSize = 0;
			for (int i = 0; i < NumberOfParameters; ++i)
			{
			  int len = getParameterOutputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			  len = getParameterInputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			}
			if (tempData_ == null || tempData_.Length < maxSize)
			{
			  tempData_ = new sbyte[maxSize];
			}
			return tempData_;
		  }
	  }

	  public virtual OpenListOfSpooledFilesFormat Formatter
	  {
		  get
		  {
			return formatter_;
		  }
		  set
		  {
			formatter_ = value;
		  }
	  }


	  public virtual OpenListOfSpooledFilesFormatListener FormatListener
	  {
		  get
		  {
			return formatListener_;
		  }
		  set
		  {
			formatListener_ = value;
		  }
	  }


	  public virtual string ProgramName
	  {
		  get
		  {
			return "QGYOLSPL";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QGY";
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 10;
		  }
	  }

	  public virtual void newCall()
	  {
		info_ = null;
	  }

	  public virtual string JobName
	  {
		  set
		  {
			jobName_ = value;
		  }
		  get
		  {
			return jobName_;
		  }
	  }


	  public virtual string JobUser
	  {
		  set
		  {
			jobUser_ = value;
		  }
		  get
		  {
			return jobUser_;
		  }
	  }


	  public virtual string JobNumber
	  {
		  set
		  {
			jobNumber_ = value;
		  }
		  get
		  {
			return jobNumber_;
		  }
	  }


	  public virtual int LengthOfReceiverVariable
	  {
		  get
		  {
			return inputLength_;
		  }
		  set
		  {
			inputLength_ = value;
		  }
	  }


	  public virtual int NumberOfRecordsToReturn
	  {
		  get
		  {
			return numberOfRecordsToReturn_;
		  }
		  set
		  {
			numberOfRecordsToReturn_ = value;
		  }
	  }


	  public virtual SortListener SortListener
	  {
		  get
		  {
			return sortListener_;
		  }
		  set
		  {
			sortListener_ = value;
		  }
	  }


	  public virtual OpenListOfSpooledFilesFilterListener FilterListener
	  {
		  get
		  {
			return filterListener_;
		  }
		  set
		  {
			filterListener_ = value;
		  }
	  }


	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterInputLength(final int parmIndex)
	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 0;
		  case 1:
			  return 4;
		  case 2:
			  return 0;
		  case 3:
			  return 4;
		  case 4:
			  return sortListener_ == null ? 4 : 4 + (sortListener_.NumberOfSortKeys * 12);
		  case 5:
			if (filterListener_ != null)
			{
			  if (filterListener_ is OpenListOfSpooledFilesFilterOSPF0200Listener)
			  {
				OpenListOfSpooledFilesFilterOSPF0200Listener listener = (OpenListOfSpooledFilesFilterOSPF0200Listener)filterListener_;
				return 110 + (listener.NumberOfUserNames * 10) + (listener.NumberOfOutputQueues * 20) + (listener.NumberOfStatuses * 10) + (listener.NumberOfPrinterDevices * 10);
			  }
			  else if (filterListener_ is OpenListOfSpooledFilesFilterOSPF0100Listener)
			  {
				OpenListOfSpooledFilesFilterOSPF0100Listener listener = (OpenListOfSpooledFilesFilterOSPF0100Listener)filterListener_;
				return 36 + (listener.NumberOfUserNames * 12) + (listener.NumberOfOutputQueues * 20) + (listener.NumberOfStatuses * 12) + (listener.NumberOfPrinterDevices * 12);
			  }
			}
			return 0; //TODO
			 // return 92;
		  case 6:
			  return 26;
		  case 7:
			  return 8;
		  case 8:
			  return 4;
		  case 9:
			  return 8;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterOutputLength(final int parmIndex)
	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return inputLength_;
		  case 2:
			  return 80;
		  case 8:
			  return 4;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
		  case 2:
			return Parameter.TYPE_OUTPUT;
		  case 8:
			return Parameter.TYPE_INPUT_OUTPUT;
		}
		return Parameter.TYPE_INPUT;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public byte[] getParameterInputData(final int parmIndex)
	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempData = getTempDataBuffer();
		sbyte[] tempData = TempDataBuffer;
		switch (parmIndex)
		{
		  case 1:
			  Conv.intToByteArray(inputLength_, tempData, 0);
			  return tempData;
		  case 3:
			  Conv.intToByteArray(numberOfRecordsToReturn_, tempData, 0);
			  return tempData;
		  case 4:
			if (sortListener_ == null)
			{
			  Conv.intToByteArray(0, tempData, 0);
			  return tempData;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfKeys = sortListener_.getNumberOfSortKeys();
			  int numberOfKeys = sortListener_.NumberOfSortKeys;
			  Conv.intToByteArray(numberOfKeys, tempData, 0);
			  int offset = 4;
			  for (int i = 0; i < numberOfKeys; ++i)
			  {
				Conv.intToByteArray(sortListener_.getSortKeyFieldStartingPosition(i), tempData, offset);
				Conv.intToByteArray(sortListener_.getSortKeyFieldLength(i), tempData, offset + 4);
				Conv.shortToByteArray(sortListener_.getSortKeyFieldDataType(i), tempData, offset + 8);
				tempData[offset + 10] = sortListener_.isAscending(i) ? unchecked((sbyte)0xF1) : unchecked((sbyte)0xF2);
				tempData[offset + 11] = 0;
				offset += 12;
			  }
			}
			return tempData;
		  case 5:
			if (filterListener_ != null && filterListener_ is OpenListOfSpooledFilesFilterOSPF0200Listener)
			{
			  OpenListOfSpooledFilesFilterOSPF0200Listener listener = (OpenListOfSpooledFilesFilterOSPF0200Listener)filterListener_;
			  int offset = 110;
			  Conv.intToByteArray(getParameterInputLength(5), tempData, 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numUsers = listener.getNumberOfUserNames();
			  int numUsers = listener.NumberOfUserNames;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetToUsers = numUsers > 0 ? offset : 0;
			  int offsetToUsers = numUsers > 0 ? offset : 0;
			  Conv.intToByteArray(offsetToUsers, tempData, 4);
			  Conv.intToByteArray(numUsers, tempData, 8);
			  Conv.intToByteArray(10, tempData, 12);
			  offset += numUsers * 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numOutputQueues = listener.getNumberOfOutputQueues();
			  int numOutputQueues = listener.NumberOfOutputQueues;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetToOutputQueues = numOutputQueues > 0 ? offset : 0;
			  int offsetToOutputQueues = numOutputQueues > 0 ? offset : 0;
			  Conv.intToByteArray(offsetToOutputQueues, tempData, 16);
			  Conv.intToByteArray(numOutputQueues, tempData, 20);
			  Conv.intToByteArray(20, tempData, 24);
			  offset += numOutputQueues * 20;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numStatuses = listener.getNumberOfStatuses();
			  int numStatuses = listener.NumberOfStatuses;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetToStatuses = numStatuses > 0 ? offset : 0;
			  int offsetToStatuses = numStatuses > 0 ? offset : 0;
			  Conv.intToByteArray(offsetToStatuses, tempData, 28);
			  Conv.intToByteArray(numStatuses, tempData, 32);
			  Conv.intToByteArray(10, tempData, 36);
			  offset += numStatuses * 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numPrinterDevices = listener.getNumberOfPrinterDevices();
			  int numPrinterDevices = listener.NumberOfPrinterDevices;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetToPrinterDevices = numPrinterDevices > 0 ? offset : 0;
			  int offsetToPrinterDevices = numPrinterDevices > 0 ? offset : 0;
			  Conv.intToByteArray(offsetToPrinterDevices, tempData, 40);
			  Conv.intToByteArray(numPrinterDevices, tempData, 44);
			  Conv.intToByteArray(10, tempData, 48);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String formType = listener.getFormType();
			  string formType = listener.FormType;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(formType, null) ? "*ALL" : formType, tempData, 52, 10);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String userSpecifiedData = listener.getUserSpecifiedData();
			  string userSpecifiedData = listener.UserSpecifiedData;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(userSpecifiedData, null) ? "*ALL" : userSpecifiedData, tempData, 62, 10);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String systemName = listener.getSystemName();
			  string systemName = listener.SystemName;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(systemName, null) ? "*ALL" : systemName, tempData, 72, 8);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String startDate = listener.getStartingSpooledFileCreateDate();
			  string startDate = listener.StartingSpooledFileCreateDate;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(startDate, null) ? "*ALL" : startDate, tempData, 80, 7);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String startTime = listener.getStartingSpooledFileCreateTime();
			  string startTime = listener.StartingSpooledFileCreateTime;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(startTime, null) ? "" : startTime, tempData, 87, 6);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String endDate = listener.getEndingSpooledFileCreateDate();
			  string endDate = listener.EndingSpooledFileCreateDate;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(endDate, null) ? "" : endDate, tempData, 93, 7);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String endTime = listener.getEndingSpooledFileCreateTime();
			  string endTime = listener.EndingSpooledFileCreateTime;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(endTime, null) ? "" : endTime, tempData, 100, 6);
			  Conv.intToByteArray(0, tempData, 106);
			  offset = 110;
			  for (int i = 0; i < numUsers; ++i)
			  {
				string user = listener.getUserName(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(user, null) ? "" : user, tempData, offset, 10);
				offset += 10;
			  }
			  for (int i = 0; i < numOutputQueues; ++i)
			  {
				string name = listener.getOutputQueueName(i);
				string lib = listener.getOutputQueueLibrary(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(name, null) ? "" : name, tempData, offset, 10);
				offset += 10;
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(lib, null) ? "" : lib, tempData, offset, 10);
				offset += 10;
			  }
			  for (int i = 0; i < numStatuses; ++i)
			  {
				string status = listener.getStatus(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(status, null) ? "" : status, tempData, offset, 10);
				offset += 10;
			  }
			  for (int i = 0; i < numPrinterDevices; ++i)
			  {
				string dev = listener.getPrinterDevice(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(dev, null) ? "" : dev, tempData, offset, 10);
				offset += 10;
			  }
			}
			else if (filterListener_ != null)
			{
			  OpenListOfSpooledFilesFilterOSPF0100Listener listener = (OpenListOfSpooledFilesFilterOSPF0100Listener)filterListener_;
			  int offset = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numUsers = listener.getNumberOfUserNames();
			  int numUsers = listener.NumberOfUserNames;
			  Conv.intToByteArray(numUsers, tempData, offset);
			  offset += 4;
			  for (int i = 0; i < numUsers; ++i)
			  {
				string user = listener.getUserName(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(user, null) ? "" : user, tempData, offset, 10);
				offset += 10;
				Conv.shortToByteArray(0, tempData, offset);
				offset += 2;
			  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numOutputQueues = listener.getNumberOfOutputQueues();
			  int numOutputQueues = listener.NumberOfOutputQueues;
			  Conv.intToByteArray(numOutputQueues, tempData, offset);
			  offset += 4;
			  for (int i = 0; i < numOutputQueues; ++i)
			  {
				string name = listener.getOutputQueueName(i);
				string lib = listener.getOutputQueueLibrary(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(name, null) ? "" : name, tempData, offset, 10);
				offset += 10;
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(lib, null) ? "" : lib, tempData, offset, 10);
				offset += 10;
			  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String formType = listener.getFormType();
			  string formType = listener.FormType;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(formType, null) ? "*ALL" : formType, tempData, offset, 10);
			  offset += 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String userSpecifiedData = listener.getUserSpecifiedData();
			  string userSpecifiedData = listener.UserSpecifiedData;
			  Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(userSpecifiedData, null) ? "*ALL" : userSpecifiedData, tempData, offset, 10);
			  offset += 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numStatuses = listener.getNumberOfStatuses();
			  int numStatuses = listener.NumberOfStatuses;
			  Conv.intToByteArray(numStatuses, tempData, offset);
			  offset += 4;
			  for (int i = 0; i < numStatuses; ++i)
			  {
				string status = listener.getStatus(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(status, null) ? "" : status, tempData, offset, 10);
				offset += 10;
				Conv.shortToByteArray(0, tempData, offset);
				offset += 2;
			  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numPrinterDevices = listener.getNumberOfPrinterDevices();
			  int numPrinterDevices = listener.NumberOfPrinterDevices;
			  Conv.intToByteArray(numPrinterDevices, tempData, offset);
			  offset += 4;
			  for (int i = 0; i < numPrinterDevices; ++i)
			  {
				string dev = listener.getPrinterDevice(i);
				Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(dev, null) ? "" : dev, tempData, offset, 10);
				offset += 10;
				Conv.shortToByteArray(0, tempData, offset);
				offset += 2;
			  }
			}
			return tempData;
		  case 6:
			Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(jobName_, null) ? "" : jobName_, tempData, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(jobUser_, null) ? "" : jobUser_, tempData, 10, 10);
			Conv.stringToBlankPadEBCDICByteArray(string.ReferenceEquals(jobNumber_, null) ? "" : jobNumber_, tempData, 20, 6);
			return tempData;
		  case 7:
			Conv.stringToBlankPadEBCDICByteArray(formatter_.Name, tempData, 0, 8);
			return tempData;
		  case 8:
			  return ZERO;
		  case 9:
			if (filterListener_ != null && filterListener_ is OpenListOfSpooledFilesFilterOSPF0200Listener)
			{
			  Conv.stringToBlankPadEBCDICByteArray("OSPF0200", tempData, 0, 8);
			}
			else
			{
			  Conv.stringToBlankPadEBCDICByteArray("OSPF0100", tempData, 0, 8);
			}
			return tempData;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 0:
			if (formatter_ != null)
			{
			  formatter_.format(data, maxLength, maxLength, formatListener_);
			}
			break;
		  case 2:
			if (maxLength < 12)
			{
			  info_ = null;
			}
			else
			{
			  info_ = Util.readOpenListInformationParameter(data, maxLength);
			}
			break;
		  default:
			break;
		}
	  }
	}


}