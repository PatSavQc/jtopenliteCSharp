///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveJournalReceiverInformatoin.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.journal
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command.program;

	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/QJORRCVI.htm">QJOURNAL.QjoRtvJrnReceiverInformation</a>
	/// 
	/// </summary>
	public class RetrieveJournalReceiverInformation : CallServiceProgramProcedure, CallServiceProgramParameterFormat
	{
	  public const string FORMAT_RRCV0100 = "RRCV0100";

	  private int inputLength_ = 512;
	  private string receiverName_;
	  private string receiverLibrary_;
	  private string formatName_ = FORMAT_RRCV0100;
	  private RetrieveJournalReceiverInformationListener listener_;

	  private readonly char[] charBuffer_ = new char[30];

	  public RetrieveJournalReceiverInformation() : base("QJOURNAL", "QSYS", "QjoRtvJrnReceiverInformation", RETURN_VALUE_FORMAT_NONE)
	  {
		ParameterFormat = this;
	  }

	  public RetrieveJournalReceiverInformation(string receiverName, string receiverLibrary, RetrieveJournalReceiverInformationListener listener) : this(512, receiverName, receiverLibrary, FORMAT_RRCV0100, listener)
	  {
	  }

	  public RetrieveJournalReceiverInformation(int lengthOfReceiverVariable, string receiverName, string receiverLibrary, string format, RetrieveJournalReceiverInformationListener listener) : this()
	  {
		inputLength_ = (lengthOfReceiverVariable < 13 ? 13 : lengthOfReceiverVariable);
		receiverName_ = receiverName;
		receiverLibrary_ = receiverLibrary;
		formatName_ = format;
		listener_ = listener;
	  }

	  public virtual int ParameterCount
	  {
		  get
		  {
			return 5;
		  }
	  }

	  public virtual int getParameterLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return inputLength_;
		  case 1:
			  return 4;
		  case 2:
			  return 20;
		  case 3:
			  return 8;
		  case 4:
			  return 4;
		}
		return 0;
	  }

	  public virtual int getParameterFormat(int parmIndex)
	  {
		return com.ibm.jtopenlite.command.program.CallServiceProgramParameterFormat_Fields.PARAMETER_FORMAT_BY_REFERENCE;
	  }

	  public virtual void fillInputData(int parmIndex, sbyte[] buffer, int offset)
	  {
		switch (parmIndex)
		{
		  case 0:
			break;
		  case 1:
			Conv.intToByteArray(inputLength_, buffer, offset);
			break;
		  case 2:
			Conv.stringToBlankPadEBCDICByteArray(receiverName_, buffer, offset, 10);
			Conv.stringToBlankPadEBCDICByteArray(receiverLibrary_, buffer, offset + 10, 10);
			break;
		  case 3:
			Conv.stringToEBCDICByteArray37(formatName_, buffer, offset);
			break;
		  case 4:
			Conv.intToByteArray(0, buffer, offset);
			break;
		}
	  }

	  public virtual void setOutputData(int parmIndex, sbyte[] buffer, int offset)
	  {
		if (parmIndex == 0)
		{
		  if (formatName_.Equals(FORMAT_RRCV0100) && listener_ != null)
		  {
			string journalName = Conv.ebcdicByteArrayToString(buffer, offset + 28, 10, charBuffer_);
			string journalLibrary = Conv.ebcdicByteArrayToString(buffer, offset + 38, 10, charBuffer_);
			char status = Conv.ebcdicByteToChar(buffer[offset + 88]);
			string attachedDateAndTime = Conv.ebcdicByteArrayToString(buffer, offset + 95, 13, charBuffer_);
			string detachedDateAndTime = Conv.ebcdicByteArrayToString(buffer, offset + 108, 13, charBuffer_);
			long numberOfJournalEntries = Conv.zonedDecimalToLong(buffer, offset + 372, 20);
			long firstSequenceNumber = Conv.zonedDecimalToLong(buffer, offset + 412, 20);
			long lastSequenceNumber = Conv.zonedDecimalToLong(buffer, offset + 432, 20);
			listener_.newReceiverInfo(receiverName_, receiverLibrary_, journalName, journalLibrary, numberOfJournalEntries, firstSequenceNumber, lastSequenceNumber, status, attachedDateAndTime, detachedDateAndTime);
		  }
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


	  public virtual string ReceiverName
	  {
		  get
		  {
			return receiverName_;
		  }
		  set
		  {
			receiverName_ = value;
		  }
	  }


	  public virtual string ReceiverLibrary
	  {
		  get
		  {
			return receiverLibrary_;
		  }
		  set
		  {
			receiverLibrary_ = value;
		  }
	  }


	  public virtual string FormatName
	  {
		  get
		  {
			return formatName_;
		  }
		  set
		  {
			formatName_ = value;
		  }
	  }


	  public virtual RetrieveJournalReceiverInformationListener Listener
	  {
		  get
		  {
			return listener_;
		  }
		  set
		  {
			listener_ = value;
		  }
	  }

	}



}