using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: GetListEntries.java
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Get list entries using the System API: 
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qgygtle.htm">QGYGTLE</a>
	/// 
	/// </summary>
	public class GetListEntries : OpenListProgram<ListEntryFormat, ListFormatListener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private int inputLength_;
	  private sbyte[] requestHandle_;
	  private int recordLength_;
	  private int numberOfRecordsToReturn_;
	  private int startingRecord_;
	  private ListEntryFormat formatter_;
	  private ListFormatListener formatListener_;
	  private ListInformation info_;
	  private sbyte[] tempData_;

	  public GetListEntries()
	  {
	  }

	  public GetListEntries(int lengthOfReceiverVariable, sbyte[] requestHandle, int recordLength, int numberOfRecordsToReturn, int startingRecord, ListEntryFormat formatter, ListFormatListener listener)
	  {
		inputLength_ = lengthOfReceiverVariable < 8 ? 8 : lengthOfReceiverVariable;
		requestHandle_ = requestHandle;
		recordLength_ = recordLength;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		startingRecord_ = startingRecord;
		formatter_ = formatter;
		formatListener_ = listener;
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QGYGTLE";
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
			return 7;
		  }
	  }

	  public virtual void newCall()
	  {
		info_ = null;
	  }

	  public virtual ListEntryFormat Formatter
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


	  public virtual ListFormatListener FormatListener
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


	  public virtual int LengthOfReceiverVariable
	  {
		  get
		  {
			return inputLength_;
		  }
		  set
		  {
			inputLength_ = value < 8 ? 8 : value;
		  }
	  }


	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
		  }
	  }

	  public virtual sbyte[] RequestHandle
	  {
		  get
		  {
			return requestHandle_;
		  }
		  set
		  {
			requestHandle_ = value;
		  }
	  }


	  public virtual int RecordLength
	  {
		  get
		  {
			return recordLength_;
		  }
		  set
		  {
			recordLength_ = value;
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


	  public virtual int StartingRecord
	  {
		  get
		  {
			return startingRecord_;
		  }
		  set
		  {
			startingRecord_ = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterInputLength(final int parmIndex)
	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 1:
		  case 2:
		  case 4:
		  case 5:
		  case 6:
			return 4;
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
		  case 3:
			  return 80;
		  case 6:
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
		  case 3:
			  return Parameter.TYPE_OUTPUT;
		  case 6:
			  return Parameter.TYPE_INPUT_OUTPUT;
		}
		return Parameter.TYPE_INPUT;
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
		  case 2:
			if (requestHandle_.Length == 4)
			{
				return requestHandle_;
			}
			Array.Copy(requestHandle_, 0, tempData, 0, 4);
			return tempData;
		  case 4:
			  Conv.intToByteArray(numberOfRecordsToReturn_, tempData, 0);
			  return tempData;
		  case 5:
			  Conv.intToByteArray(startingRecord_, tempData, 0);
			  return tempData;
		  case 6:
			  return ZERO;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] tempData, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 0:
			if (formatter_ != null)
			{
			  formatter_.format(tempData, maxLength, recordLength_, formatListener_);
			}
			break;
		  case 3:
			if (maxLength < 12)
			{
			  info_ = null;
			}
			else
			{
			  info_ = Util.readOpenListInformationParameter(tempData, maxLength);
			}
			break;
		  default:
			break;
		}
	  }
	}


}