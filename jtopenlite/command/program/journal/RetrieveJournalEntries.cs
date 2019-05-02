///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveJournalEntries.java
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
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/QJORJRNE.htm">
	/// QJOURNAL.QjoRetrieveJournalEntries
	/// </a>
	/// 
	/// </summary>
	public class RetrieveJournalEntries : CallServiceProgramProcedure, CallServiceProgramParameterFormat
	{
	  public const string FORMAT_RJNE0100 = "RJNE0100";
	  public const string FORMAT_RJNE0200 = "RJNE0200";

		/// <summary>
		/// KEY_RANGE_OF_JOURNAL_RECEIVERS corresponds to RCVRNG parameter of RCVJRNE
		/// Command. Type is CHAR(40)
		/// </summary>
		public const int KEY_RANGE_OF_JOURNAL_RECEIVERS = 1;

		/// <summary>
		/// KEY_STARTING_SEQUENCE_NUMBER corresponds to FROMENT parameter of RCVJRNE
		/// Command. Type is CHAR(20)
		/// </summary>
		public const int KEY_STARTING_SEQUENCE_NUMBER = 2;
		/// <summary>
		/// KEY_STARTING_TIME_STAMP corresponds to FROMTIME parameter of RCVJRNE
		/// Command. Type is CHAR(26)
		/// </summary>
		public const int KEY_STARTING_TIME_STAMP = 3;
		/// <summary>
		/// KEY_ENDING_SEQUENCE_NUMBER corresponds to TOENT parameter of RCVJRNE
		/// Command. Type is CHAR(20)
		/// </summary>
		public const int KEY_ENDING_SEQUENCE_NUMBER = 4;
		/// <summary>
		/// KEY_ENDING_TIME_STAMP corresponds to TOTIME parameter of RCVJRNE Command.
		/// Type is CHAR(26)
		/// </summary>
		public const int KEY_ENDING_TIME_STAMP = 5;
		/// <summary>
		/// KEY_NUMBER_OF_ENTRIES corresponds to NBRENT parameter of RCVJRNE Command.
		/// Type is BINARY(4)
		/// </summary>
		public const int KEY_NUMBER_OF_ENTRIES = 6;
		/// <summary>
		/// KEY_JOURNAL_CODES corresponds to JRNCDE parameter of RCVJRNE Command.
		/// Type is CHAR(*)
		/// </summary>
		public const int KEY_JOURNAL_CODES = 7;
		/// <summary>
		/// KEY_JOURNAL_ENTRY_TYPES corresponds to ENTTYP parameter of RCVJRNE
		/// Command. Type is CHAR(*)
		/// </summary>
		public const int KEY_JOURNAL_ENTRY_TYPES = 8;
		/// <summary>
		/// KEY_JOB corresponds to JOB parameter of RCVJRNE Command. Type is CHAR(26)
		/// </summary>
		public const int KEY_JOB = 9;
		/// <summary>
		/// KEY_PROGRAM corresponds to PGM parameter of RCVJRNE Command. Type is
		/// CHAR(10)
		/// </summary>
		public const int KEY_PROGRAM = 10;
		/// <summary>
		/// KEY_USER_PROFILE corresponds to USRPRF parameter of RCVJRNE Command. Type
		/// is CHAR(10)
		/// </summary>
		public const int KEY_USER_PROFILE = 11;
		/// <summary>
		/// KEY_COMMIT_CYCLE_IDENTIFIER corresponds to CMTCYCID parameter of RCVJRNE
		/// Command. Type is CHAR(20)
		/// </summary>
		public const int KEY_COMMIT_CYCLE_IDENTIFIER = 12;
		/// <summary>
		/// KEY_DEPENDENT_ENTRIES corresponds to DEPENT parameter of RCVJRNE Command.
		/// Type is CHAR(10)
		/// </summary>
		public const int KEY_DEPENDENT_ENTRIES = 13;
		/// <summary>
		/// KEY_INCLUDE_ENTRIES corresponds to INCENT parameter of RCVJRNE Command.
		/// Type is CHAR(10)
		/// </summary>
		public const int KEY_INCLUDE_ENTRIES = 14;
		/// <summary>
		/// KEY_NULL_VALUE_INDICATORS_LENGTH corresponds to NULLINDLEN parameter of
		/// RCVJRNE Command. Type is CHAR(10)
		/// </summary>
		public const int KEY_NULL_VALUE_INDICATORS_LENGTH = 15;
		/// <summary>
		/// KEY_FILE corresponds to FILE parameter of RCVJRNE Command. Type is
		/// CHAR(*)
		/// </summary>
		public const int KEY_FILE = 16;
		/// <summary>
		/// KEY_OBJECT corresponds to OBJ parameter of RCVJRNE Command. Type is
		/// CHAR(*)
		/// </summary>
		public const int KEY_OBJECT = 17;
		/// <summary>
		/// KEY_OBJECT_PATH corresponds to OBJPATH parameter of RCVJRNE Command. Type
		/// is CHAR(*)
		/// </summary>
		public const int KEY_OBJECT_PATH = 18;
		/// <summary>
		/// KEY_OBJECT_FILE_IDENTIFIER corresponds to OBJFID parameter of RCVJRNE
		/// Command. Type is CHAR(*)
		/// </summary>
		public const int KEY_OBJECT_FILE_IDENTIFIER = 19;
		/// <summary>
		/// KEY_DIRECTORY_SUBTREE corresponds to SUBTREE parameter of RCVJRNE
		/// Command. Type is CHAR(5)
		/// </summary>
		public const int KEY_DIRECTORY_SUBTREE = 20;
		/// <summary>
		/// KEY_NAME_PATTERN corresponds to PATTERN parameter of RCVJRNE Command.
		/// Type is CHAR(*)
		/// </summary>
		public const int KEY_NAME_PATTERN = 21;
		/// <summary>
		/// KEY_FORMAT_MINIMIZED_DATA corresponds to FMTMINDTA parameter of RCVJRNE
		/// Command. Type is CHAR(10)
		/// </summary>
		public const int KEY_FORMAT_MINIMIZED_DATA = 22;

	  private int inputLength_;
	  private string journalName_;
	  private string journalLibrary_;
	  private string formatName_;
	  private RetrieveJournalEntriesSelectionListener selection_;
	  private RetrieveJournalEntriesListener listener_;

	  private readonly char[] charBuffer_ = new char[30];

	  public RetrieveJournalEntries() : base("QJOURNAL", "QSYS", "QjoRetrieveJournalEntries", RETURN_VALUE_FORMAT_NONE)
	  {
		ParameterFormat = this;
	  }

	  public RetrieveJournalEntries(int lengthOfReceiverVariable, string journalName, string journalLibrary, string format, RetrieveJournalEntriesListener listener) : this()
	  {
		inputLength_ = (lengthOfReceiverVariable < 13 ? 13 : lengthOfReceiverVariable);
		journalName_ = journalName;
		journalLibrary_ = journalLibrary;
		formatName_ = format;
		listener_ = listener;
	  }

	  public virtual RetrieveJournalEntriesSelectionListener SelectionListener
	  {
		  get
		  {
			return selection_;
		  }
		  set
		  {
			selection_ = value;
		  }
	  }


	  public virtual int ParameterCount
	  {
		  get
		  {
			return 6;
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
			int total = 4;
			if (selection_ != null)
			{
			  int num = selection_.NumberOfVariableLengthRecords;
			  for (int i = 0; i < num; ++i)
			  {
				int len = selection_.getVariableLengthRecordDataLength(i);
				total += 12 + len;
			  }
			}
			return total;
		  case 5:
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
			Conv.stringToBlankPadEBCDICByteArray(journalName_, buffer, offset, 10);
			Conv.stringToBlankPadEBCDICByteArray(journalLibrary_, buffer, offset + 10, 10);
			break;
		  case 3:
			Conv.stringToEBCDICByteArray37(formatName_, buffer, offset);
			break;
		  case 4:
			if (selection_ == null)
			{
			  Conv.intToByteArray(0, buffer, offset);
			}
			else
			{
			  int total = selection_.NumberOfVariableLengthRecords;
			  Conv.intToByteArray(total, buffer, offset);
			  offset += 4;
			  for (int i = 0; i < total; ++i)
			  {
				int len = selection_.getVariableLengthRecordDataLength(i);
				int recLen = 12 + len;
				Conv.intToByteArray(recLen, buffer, offset);
				Conv.intToByteArray(selection_.getVariableLengthRecordKey(i), buffer, offset + 4);
				Conv.intToByteArray(len, buffer, offset + 8);
				selection_.setVariableLengthRecordData(i, buffer, offset + 12);
				offset += recLen;
			  }
			}
			break;
		  case 5:
			Conv.intToByteArray(0, buffer, offset);
			break;
		}
	  }

	  public virtual void setOutputData(int parmIndex, sbyte[] buffer, int offset)
	  {
		if (parmIndex == 0)
		{
		  if (formatName_.Equals(FORMAT_RJNE0100))
		  {
			// int bytesReturned = Conv.byteArrayToInt(buffer, offset);
			offset += 4;
			int offsetToFirstJournalEntryHeader = Conv.byteArrayToInt(buffer, offset);
			offset += 4;
			int numberOfEntriesRetrieved = Conv.byteArrayToInt(buffer, offset);
			offset += 4;
			char continuationHandle = Conv.ebcdicByteToChar(buffer[offset]);
			offset++;
			if (listener_ != null)
			{
			  listener_.newJournalEntries(numberOfEntriesRetrieved, continuationHandle);
			}
			if (offsetToFirstJournalEntryHeader > 0 && listener_ != null)
			{
			  offset = offsetToFirstJournalEntryHeader;
			  for (int i = 0; i < numberOfEntriesRetrieved; ++i)
			  {
				int displacementToNextHeader = Conv.byteArrayToInt(buffer, offset);
				// int displacementToNullValueIndicators = Conv.byteArrayToInt(buffer, offset+4);
				// int displacementToEntrySpecificData = Conv.byteArrayToInt(buffer, offset+8);
				int pointerHandle = Conv.byteArrayToInt(buffer, offset + 12);
				long sequenceNumber = Conv.zonedDecimalToLong(buffer, offset + 16, 20);
				char journalCode = Conv.ebcdicByteToChar(buffer[offset + 36]);
				string entryType = Conv.ebcdicByteArrayToString(buffer, offset + 37, 2, charBuffer_);
				string timeStamp = Conv.ebcdicByteArrayToString(buffer, offset + 39, 26, charBuffer_);
				string jobName = Conv.ebcdicByteArrayToString(buffer, offset + 65, 10, charBuffer_);
				string userName = Conv.ebcdicByteArrayToString(buffer, offset + 75, 10, charBuffer_);
				string jobNumber = Conv.ebcdicByteArrayToString(buffer, offset + 85, 6, charBuffer_);
				string programName = Conv.ebcdicByteArrayToString(buffer, offset + 91, 10, charBuffer_);
				string @object = Conv.ebcdicByteArrayToString(buffer, offset + 101, 30, charBuffer_);
				int count = Conv.zonedDecimalToInt(buffer, offset + 131, 10);
				char indicatorFlag = Conv.ebcdicByteToChar(buffer[offset + 141]);
				long commitCycleIdentifier = Conv.zonedDecimalToLong(buffer, offset + 142, 20);
				string userProfile = Conv.ebcdicByteArrayToString(buffer, offset + 162, 10, charBuffer_);
				string systemName = Conv.ebcdicByteArrayToString(buffer, offset + 172, 8, charBuffer_);
				string journalIdentifier = Conv.ebcdicByteArrayToString(buffer, offset + 180, 10, charBuffer_); //TODO - Should this be bytes?
				char referentialConstraint = Conv.ebcdicByteToChar(buffer[offset + 190]);
				char trigger = Conv.ebcdicByteToChar(buffer[offset + 191]);
				char incompleteData = Conv.ebcdicByteToChar(buffer[offset + 192]);
				char objectNameIndicator = Conv.ebcdicByteToChar(buffer[offset + 193]);
				char ignoreDuringJournalChange = Conv.ebcdicByteToChar(buffer[offset + 194]);
				char minimizedEntrySpecificData = Conv.ebcdicByteToChar(buffer[offset + 195]);
				listener_.newEntryData(pointerHandle, sequenceNumber, journalCode, entryType, timeStamp, jobName, userName, jobNumber, programName, @object, count, indicatorFlag, commitCycleIdentifier, userProfile, systemName, journalIdentifier, referentialConstraint, trigger, incompleteData, objectNameIndicator, ignoreDuringJournalChange, minimizedEntrySpecificData);
				offset += displacementToNextHeader;
			  }
			}
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


	  public virtual string JournalName
	  {
		  get
		  {
			return journalName_;
		  }
		  set
		  {
			journalName_ = value;
		  }
	  }


	  public virtual string JournalLibrary
	  {
		  get
		  {
			return journalLibrary_;
		  }
		  set
		  {
			journalLibrary_ = value;
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


	  public virtual RetrieveJournalEntriesListener Listener
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