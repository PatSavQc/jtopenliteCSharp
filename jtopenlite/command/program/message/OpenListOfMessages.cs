///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfMessages.java
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
	using com.ibm.jtopenlite.command;

	using com.ibm.jtopenlite.command.program.openlist;


	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/QGYOLMSG.htm">QGYOLMSG</a>
	/// This class fully implements the V5R4 specification of QGYOLMSG.
	/// 
	/// </summary>
	public class OpenListOfMessages : OpenListProgram<OpenListOfMessagesLSTM0100, OpenListOfMessagesLSTM0100Listener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private int inputLength_;
	  private int numberOfRecordsToReturn_;
	  private string sortInformation_;
	  private string userOrQueueIndicator_;
	  private string userOrQueueName_;
	  private string queueLibrary_;
	  private OpenListOfMessagesLSTM0100 formatter_;
	  private OpenListOfMessagesLSTM0100Listener formatListener_;
	  private ListInformation info_;
	  private OpenListOfMessagesSelectionListener selectionListener_;

	  private int numberOfQueuesUsed_;
	  private string messageQueue1_;
	  private string messageQueue2_;

	  private sbyte[] tempData_;

	  public OpenListOfMessages()
	  {
	  }

	  public OpenListOfMessages(int lengthOfReceiverVariable, int numberOfRecordsToReturn, string sortInformation, string userOrQueueIndicator, string userOrQueueName, string queueLibrary, OpenListOfMessagesLSTM0100 format)
	  {
	//    super("QGY", "QGYOLMSG", 10);
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		sortInformation_ = sortInformation;
		userOrQueueIndicator_ = userOrQueueIndicator;
		userOrQueueName_ = userOrQueueName;
		queueLibrary_ = queueLibrary;
		formatter_ = format;
	  }

	  public virtual OpenListOfMessagesLSTM0100Listener FormatListener
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
			return "QGYOLMSG";
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

	  public virtual OpenListOfMessagesSelectionListener SelectionListener
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


	  public virtual OpenListOfMessagesLSTM0100 Formatter
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


	  public virtual void newCall()
	  {
		info_ = null;
		numberOfQueuesUsed_ = 0;
		messageQueue1_ = null;
		messageQueue2_ = null;
	  }

	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
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


	  public virtual string SortInformation
	  {
		  get
		  {
			return sortInformation_;
		  }
		  set
		  {
			sortInformation_ = value;
		  }
	  }


	  public virtual string UserOrQueueIndicator
	  {
		  get
		  {
			return userOrQueueIndicator_;
		  }
		  set
		  {
			userOrQueueIndicator_ = value;
		  }
	  }


	  public virtual string UserOrQueueName
	  {
		  get
		  {
			return userOrQueueName_;
		  }
		  set
		  {
			userOrQueueName_ = value;
		  }
	  }


	  public virtual string QueueLibrary
	  {
		  get
		  {
			return queueLibrary_;
		  }
		  set
		  {
			queueLibrary_ = value;
		  }
	  }


	  public virtual int NumberOfQueuesUsed
	  {
		  get
		  {
			return numberOfQueuesUsed_;
		  }
	  }

	  public virtual string MessageQueue1
	  {
		  get
		  {
			return messageQueue1_;
		  }
	  }

	  public virtual string MessageQueue2
	  {
		  get
		  {
			return messageQueue2_;
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
			  return 1;
		  case 5:
			int num = 44;
			if (selectionListener_ != null)
			{
			  num += 10 * selectionListener_.SelectionCriteriaCount + 4 * selectionListener_.StartingMessageKeyCount + 4 * selectionListener_.FieldIdentifierCount;
			}
			if (num < 62)
			{
				num = 62;
			}
			return num;
		  case 6:
			  return 4;
		  case 7:
			  return 21;
		  case 8:
			  return 0;
		  case 9:
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
		  case 8:
			  return 44;
		  case 9:
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
		  case 8:
			return Parameter.TYPE_OUTPUT;
		  case 9:
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
			  Conv.stringToEBCDICByteArray37(sortInformation_, tempData, 0);
			  return tempData;
		  case 5: // Message selection info.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String listDirection = selectionListener_ == null ? "*NEXT" : selectionListener_.getListDirection();
			string listDirection = selectionListener_ == null ? "*NEXT" : selectionListener_.ListDirection;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int severityCriteria = selectionListener_ == null ? 0 : selectionListener_.getSeverityCriteria();
			int severityCriteria = selectionListener_ == null ? 0 : selectionListener_.SeverityCriteria;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maximumMessageLength = selectionListener_ == null ? 1024 : selectionListener_.getMaximumMessageLength();
			int maximumMessageLength = selectionListener_ == null ? 1024 : selectionListener_.MaximumMessageLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maximumMessageHelpLength = selectionListener_ == null ? 1024 : selectionListener_.getMaximumMessageHelpLength();
			int maximumMessageHelpLength = selectionListener_ == null ? 1024 : selectionListener_.MaximumMessageHelpLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfSelectionCriteria = selectionListener_ == null ? 1 : selectionListener_.getSelectionCriteriaCount();
			int numberOfSelectionCriteria = selectionListener_ == null ? 1 : selectionListener_.SelectionCriteriaCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfStartingMessageKeys = selectionListener_ == null ? 1 : selectionListener_.getStartingMessageKeyCount();
			int numberOfStartingMessageKeys = selectionListener_ == null ? 1 : selectionListener_.StartingMessageKeyCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfFieldsToReturn = selectionListener_ == null ? 1 : selectionListener_.getFieldIdentifierCount();
			int numberOfFieldsToReturn = selectionListener_ == null ? 1 : selectionListener_.FieldIdentifierCount;

			int offsetOfSelectionCriteria = 44;
			int offsetOfStartingMessageKeys = offsetOfSelectionCriteria + (10 * numberOfSelectionCriteria);
			int offsetOfIdentifiersOfFieldsToReturn = offsetOfStartingMessageKeys + (4 * numberOfStartingMessageKeys);
			Conv.stringToBlankPadEBCDICByteArray(listDirection, tempData, 0, 10);
			Conv.intToByteArray(severityCriteria, tempData, 12);
			Conv.intToByteArray(maximumMessageLength, tempData, 16);
			Conv.intToByteArray(maximumMessageHelpLength, tempData, 20);
			Conv.intToByteArray(offsetOfSelectionCriteria, tempData, 24);
			Conv.intToByteArray(numberOfSelectionCriteria, tempData, 28);
			Conv.intToByteArray(offsetOfStartingMessageKeys, tempData, 32);
			Conv.intToByteArray(offsetOfIdentifiersOfFieldsToReturn, tempData, 36);
			Conv.intToByteArray(numberOfFieldsToReturn, tempData, 40);
			int offset = 44;
			for (int i = 0; i < numberOfSelectionCriteria; ++i)
			{
			  Conv.stringToBlankPadEBCDICByteArray(selectionListener_ == null ? "*ALL" : selectionListener_.getSelectionCriteria(i), tempData, offset, 10);
			  offset += 10;
			}
			for (int i = 0; i < numberOfStartingMessageKeys; ++i)
			{
	//          Conv.stringToBlankPadEBCDICByteArray(selectionListener_ == null ? "0000" : selectionListener_.getStartingMessageKey(i), tempData, offset, 4);
			  Conv.intToByteArray(selectionListener_ == null ? 0x0000 : selectionListener_.getStartingMessageKey(i), tempData, offset);
			  offset += 4;
			}
			for (int i = 0; i < numberOfFieldsToReturn; ++i)
			{
			  Conv.intToByteArray(selectionListener_ == null ? 1001 : selectionListener_.getFieldIdentifier(i), tempData, offset);
			  offset += 4;
			}
			return tempData;
		  case 6:
			  Conv.intToByteArray(getParameterInputLength(5), tempData, 0);
			  return tempData;
		  case 7:
			Conv.stringToBlankPadEBCDICByteArray(userOrQueueIndicator_, tempData, 0, 1);
			Conv.stringToBlankPadEBCDICByteArray(userOrQueueName_, tempData, 1, 10);
			Conv.stringToBlankPadEBCDICByteArray(queueLibrary_, tempData, 11, 10);
			return tempData;
		  case 9:
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
		  case 8:
			if (maxLength >= 44)
			{
			  numberOfQueuesUsed_ = Conv.byteArrayToInt(data, 0);
			  messageQueue1_ = Conv.ebcdicByteArrayToString(data, 4, 20);
			  messageQueue2_ = Conv.ebcdicByteArrayToString(data, 24, 20);
			}
			break;
		  default:
			break;
		}
	  }
	}


}