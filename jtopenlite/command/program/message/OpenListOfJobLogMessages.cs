using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobLogMessages.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.message
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command.program.openlist;


	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/QGYOLJBL.htm">QGYOLJBL</a>
	/// This class fully implements the V5R4 specification of QGYOLJBL.
	/// 
	/// </summary>
	public class OpenListOfJobLogMessages : OpenListProgram<OpenListOfJobLogMessagesOLJL0100, OpenListOfJobLogMessagesOLJL0100Listener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private int inputLength_;
	  private int numberOfRecordsToReturn_;
	  private OpenListOfJobLogMessagesOLJL0100 formatter_;
	  private OpenListOfJobLogMessagesOLJL0100Listener formatListener_;
	  private ListInformation info_;
	  private OpenListOfJobLogMessagesSelectionListener selectionListener_;

	  private sbyte[] tempData_;

	  public OpenListOfJobLogMessages()
	  {
	  }

	  public OpenListOfJobLogMessages(int lengthOfReceiverVariable, int numberOfRecordsToReturn, OpenListOfJobLogMessagesOLJL0100 format)
	  {
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		formatter_ = format;
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QGYOLJBL";
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

	  public virtual OpenListOfJobLogMessagesSelectionListener SelectionListener
	  {
		  get
		  {
			return selectionListener_;
		  }
		  set
		  {
			selectionListener_ = value;
		  }
	  }


	  public virtual OpenListOfJobLogMessagesOLJL0100 Formatter
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


	  public virtual OpenListOfJobLogMessagesOLJL0100Listener FormatListener
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


	  public virtual void newCall()
	  {
		info_ = null;
	  }

	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
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
			int num = 80;
			if (selectionListener_ != null)
			{
			  string queue = selectionListener_.CallMessageQueueName;
			  num += 4 * selectionListener_.FieldIdentifierCount + (string.ReferenceEquals(queue, null) ? 1 : queue.Length);

			}
			return num;
		  case 5:
			  return 4;
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
		  case 2:
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
		  case 2:
			return Parameter.TYPE_OUTPUT;
		  case 6:
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
		  case 4: // Message selection info.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String listDirection = selectionListener_ == null ? "*NEXT" : selectionListener_.getListDirection();
			string listDirection = selectionListener_ == null ? "*NEXT" : selectionListener_.ListDirection;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qualifiedJobName = selectionListener_ == null ? "*" : selectionListener_.getQualifiedJobName();
			string qualifiedJobName = selectionListener_ == null ? "*" : selectionListener_.QualifiedJobName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] internalJobIdentifier = selectionListener_ == null ? null : selectionListener_.getInternalJobIdentifier();
			sbyte[] internalJobIdentifier = selectionListener_ == null ? null : selectionListener_.InternalJobIdentifier;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startingMessageKey = selectionListener_ == null ? 0 : selectionListener_.getStartingMessageKey();
			int startingMessageKey = selectionListener_ == null ? 0 : selectionListener_.StartingMessageKey;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maximumMessageLength = selectionListener_ == null ? 1024 : selectionListener_.getMaximumMessageLength();
			int maximumMessageLength = selectionListener_ == null ? 1024 : selectionListener_.MaximumMessageLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maximumMessageHelpLength = selectionListener_ == null ? 1024 : selectionListener_.getMaximumMessageHelpLength();
			int maximumMessageHelpLength = selectionListener_ == null ? 1024 : selectionListener_.MaximumMessageHelpLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfFieldsToReturn = selectionListener_ == null ? 1 : selectionListener_.getFieldIdentifierCount();
			int numberOfFieldsToReturn = selectionListener_ == null ? 1 : selectionListener_.FieldIdentifierCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String callMessageQueueName = selectionListener_ == null ? "*" : selectionListener_.getCallMessageQueueName();
			string callMessageQueueName = selectionListener_ == null ? "*" : selectionListener_.CallMessageQueueName;

			int offsetOfIdentifiersOfFieldsToReturn = 80;
			int offsetOfCallMessageQueueName = offsetOfIdentifiersOfFieldsToReturn + (4 * numberOfFieldsToReturn);
			Conv.stringToBlankPadEBCDICByteArray(listDirection, tempData, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(qualifiedJobName, tempData, 10, 26);
			if (internalJobIdentifier == null)
			{
			  for (int i = 36; i < 52; ++i)
			  {
				  tempData[i] = 0x40; // Blanks.
			  }
			}
			else
			{
			  Array.Copy(internalJobIdentifier, 0, tempData, 36, 16);
			}
			Conv.intToByteArray(startingMessageKey, tempData, 52);
			Conv.intToByteArray(maximumMessageLength, tempData, 56);
			Conv.intToByteArray(maximumMessageHelpLength, tempData, 60);
			Conv.intToByteArray(offsetOfIdentifiersOfFieldsToReturn, tempData, 64);
			Conv.intToByteArray(numberOfFieldsToReturn, tempData, 68);
			Conv.intToByteArray(offsetOfCallMessageQueueName, tempData, 72);
			Conv.intToByteArray(callMessageQueueName.Length, tempData, 76);
			int offset = 80;
			for (int i = 0; i < numberOfFieldsToReturn; ++i)
			{
			  Conv.intToByteArray(selectionListener_ == null ? 1001 : selectionListener_.getFieldIdentifier(i), tempData, offset);
			  offset += 4;
			}
			Conv.stringToEBCDICByteArray37(callMessageQueueName, tempData, offset);
			return tempData;
		  case 5:
			  Conv.intToByteArray(getParameterInputLength(4), tempData, 0);
			  return tempData;
		  case 6:
			  return ZERO;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		// int numRead = 0;
		switch (parmIndex)
		{
		  case 0:
			if (formatter_ != null)
			{
			  formatter_.format(data, maxLength, 0, formatListener_);
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