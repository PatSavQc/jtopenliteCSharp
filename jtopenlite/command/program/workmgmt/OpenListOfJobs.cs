///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobs.java
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
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.openlist;


	/// <summary>
	/// Use the <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qgyoljob.htm">QGYOLJOB</a>
	/// API to access the list of job on the IBM i. 
	/// This class fully implements the V5R4 specification of QGYOLJOB.
	/// 
	/// <para>
	/// Example:
	/// 
	/// <pre>
	/// CommandConnection conn = CommandConnection.getConnection(...);
	/// 
	/// OpenListOfJobsFormatOLJB0200 jobFormat = new OpenListOfJobsFormatOLJB0200(); // Use the OLJB0200 format to get key info back.
	/// int receiverSize = 100; // This should be large enough for the initial call.
	/// int numRecordsToReturn = 1; // Need to return at least one record to get the key definition back.
	/// int[] fieldsToReturn = new int[] { 1906 }; // Subsystem information.
	/// 
	/// OpenListOfJobs jobList = new OpenListOfJobs(jobFormat, receiverSize, numRecordsToReturn, fieldsToReturn);
	/// 
	/// OpenListOfJobsSelectionListener jobSelector = ...; // Define your own. Optional.
	/// jobList.setSelectionListener(jobSelector, OpenListOfJobs.SELECTION_OLJS0100);
	/// 
	/// OpenListOfJobsSortListener jobSorter = ...; // Define your own. Optional.
	/// jobList.setSortListener(jobSorter);
	/// 
	/// CommandResult result = conn.call(jobList);
	/// // Assuming it succeeded...
	/// 
	/// OpenListOfJobsKeyField[] keyDefinitions = jobList.getKeyFields();
	/// ListInformation listInfo = jobList.getListInformation();
	/// byte[] requestHandle = listInfo.getRequestHandle();
	/// int recordLength = listInfo.getRecordLength();
	/// 
	/// // Now, the list is building on the server.
	/// // Call GetListEntries once to wait for the list to finish building, for example.
	/// receiverSize = 100; // Should be good enough for the first call.
	/// numRecordsToReturn = 0;
	/// int startingRecord = -1; // Wait until whole list is built before returning. Optional.
	/// GetListEntries getJobs = new GetListEntries(receiverSize, requestHandle, recordLength, numRecordsToReturn, startingRecord, jobFormat);
	/// result = conn.call(getJobs);
	/// // Assuming it succeeded...
	/// 
	/// listInfo = getJobs.getListInformation();
	/// int totalRecords = listInfo.getTotalRecords();
	/// 
	/// // Now retrieve the job records in chunks of, for example, 300 at a time.
	/// numRecordsToReturn = 300;
	/// receiverSize = recordLength * numRecordsToReturn;
	/// startingRecord = 1;
	/// getJobs.setLengthOfReceiverVariable(receiverSize);
	/// getJobs.setNumberOfRecordsToReturn(numRecordsToReturn);
	/// getJobs.setStartingRecord(startingRecord);
	/// jobFormat.setKeyFields(keyDefinitions);
	/// OpenListOfJobsFormatOLJB0200Listener callback = ...; // Define your own.
	/// jobFormat.setListener(callback); // Ready to process.
	/// 
	/// while (startingRecord &lt;= totalRecords)
	/// {
	///   result = conn.call(getJobs);
	///   // Assuming it succeeded...
	///   listInfo = getJobs.getListInformation();
	///   startingRecord += listInfo.getRecordsReturned();
	///   getJobs.setStartingRecord(startingRecord);
	/// }
	/// 
	/// // All done.
	/// CloseList close = new CloseList(requestHandle);
	/// result = conn.call(close);
	/// conn.close();
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	public class OpenListOfJobs : OpenListProgram<OpenListOfJobsFormat, OpenListOfJobsFormatListener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const int SELECTION_OLJS0100 = 3;
	  public const int SELECTION_OLJS0200 = 4;

	  private OpenListOfJobsFormat formatter_;
	  private OpenListOfJobsFormatListener formatListener_;
	  private int inputLength_;
	  private int numberOfRecordsToReturn_;
	  private OpenListOfJobsSortListener sortListener_;
	  private int[] fieldsToReturn_;
	  private int selectionFormat_ = SELECTION_OLJS0100;
	  private OpenListOfJobsSelectionListener selectionListener_;
	  private bool resetStats_;
	  private long elapsedTime_;
	  private ListInformation info_;
	  private OpenListOfJobsKeyField[] keyFields_;

	  private sbyte[] tempData_;

	  public OpenListOfJobs()
	  {
	  }

	  public OpenListOfJobs(OpenListOfJobsFormat format, int lengthOfReceiverVariable, int numberOfRecordsToReturn, int[] fieldsToReturn)
	  {
		formatter_ = format;
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		fieldsToReturn_ = fieldsToReturn == null ? new int[0] : fieldsToReturn;
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

	  public virtual OpenListOfJobsFormat Formatter
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


	  public virtual OpenListOfJobsFormatListener FormatListener
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
			return "QGYOLJOB";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QGY";
		  }
	  }

	  public virtual void newCall()
	  {
		elapsedTime_ = 0;
		info_ = null;
		keyFields_ = null;
	  }

	  public virtual OpenListOfJobsKeyField[] KeyFields
	  {
		  get
		  {
			return keyFields_;
		  }
	  }

	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			int num = 13;
			if (selectionListener_ != null)
			{
			  num++;
			}
			if (formatter_.Type == OpenListOfJobsFormat_Fields.FORMAT_OLJB0300)
			{
			  num += 3;
			  if (selectionListener_ == null)
			  {
				num++;
			  }
			}
			return num;
		  }
	  }

	  public virtual OpenListOfJobsSortListener SortListener
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


	  public virtual bool ResetStatusStatistics
	  {
		  get
		  {
			return resetStats_;
		  }
		  set
		  {
			resetStats_ = value;
		  }
	  }


	  public virtual long ElapsedTime
	  {
		  get
		  {
			return elapsedTime_;
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


	  public virtual int[] FieldsToReturn
	  {
		  get
		  {
			return fieldsToReturn_;
		  }
		  set
		  {
			fieldsToReturn_ = value == null ? new int[0] : value;
		  }
	  }


	  public virtual OpenListOfJobsSelectionListener SelectionListener
	  {
		  get
		  {
			return selectionListener_;
		  }
	  }

	  public virtual void setSelectionListener(OpenListOfJobsSelectionListener selectionListener, int selectionFormat)
	  {
		selectionListener_ = selectionListener;
		selectionFormat_ = selectionListener_ == null ? SELECTION_OLJS0100 : selectionFormat;
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
			  return 8;
		  case 3:
			  return 0;
		  case 4:
			  return 4;
		  case 5:
			  return 0;
		  case 6:
			  return 4;
		  case 7:
			  return sortListener_ == null ? 4 : 4 + 12 * sortListener_.NumberOfSortKeys;
		  case 8:
			int num = 60;
			if (selectionListener_ != null)
			{
			  num += 10 * selectionListener_.PrimaryJobStatusCount + 4 * selectionListener_.ActiveJobStatusCount + 10 * selectionListener_.JobsOnJobQueueStatusCount + 20 * selectionListener_.JobQueueNameCount;
			  if (selectionFormat_ == SELECTION_OLJS0200)
			  {
				num += 48 + 10 * selectionListener_.CurrentUserProfileCount + 30 * selectionListener_.ServerTypeCount + 10 * selectionListener_.ActiveSubsystemCount + 4 * selectionListener_.MemoryPoolCount + 4 * selectionListener_.JobTypeEnhancedCount + 26 * selectionListener_.QualifiedJobNameCount;
			  }
			}
			return num;
		  case 9:
			  return 4;
		  case 10:
			  return 4;
		  case 11:
			  return 4 * fieldsToReturn_.Length;
		  case 12:
			  return 4;
		  case 13:
			  return 8;
		  case 14:
			  return 1;
		  case 15:
			  return 0;
		  case 16:
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
			  return 4 + 20 * fieldsToReturn_.Length;
		  case 5:
			  return 80;
		  case 12:
			  return 4;
		  case 15:
			  return 16;
		}
		return 0;
	  }

	  private string SelectionFormatName
	  {
		  get
		  {
			switch (selectionFormat_)
			{
			  case SELECTION_OLJS0100:
				  return "OLJS0100";
			  case SELECTION_OLJS0200:
				  return "OLJS0200";
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
		  case 3:
		  case 5:
		  case 15:
			return Parameter.TYPE_OUTPUT;
		  case 12:
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
		  case 2:
			  Conv.stringToEBCDICByteArray37(formatter_.Name, tempData, 0);
			  return tempData;
		  case 4:
			  Conv.intToByteArray(getParameterOutputLength(3), tempData, 0);
			  return tempData;
		  case 6:
			  Conv.intToByteArray(numberOfRecordsToReturn_, tempData, 0);
			  return tempData;
		  case 7: // Sort info.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numSortKeys = sortListener_ == null ? 0 : sortListener_.getNumberOfSortKeys();
			int numSortKeys = sortListener_ == null ? 0 : sortListener_.NumberOfSortKeys;
			Conv.intToByteArray(numSortKeys, tempData, 0);
			int offset = 4;
			for (int i = 0; i < numSortKeys; ++i)
			{
			  Conv.intToByteArray(sortListener_.getSortKeyFieldStartingPosition(i), tempData, offset);
			  offset += 4;
			  Conv.intToByteArray(sortListener_.getSortKeyFieldLength(i), tempData, offset);
			  offset += 4;
			  Conv.shortToByteArray(sortListener_.getSortKeyFieldDataType(i), tempData, offset);
			  offset += 2;
			  tempData[offset++] = sortListener_.isAscending(i) ? unchecked((sbyte)0xF1) : unchecked((sbyte)0xF2);
			  tempData[offset++] = 0;
			}
			return tempData;
		  case 8: // Job selection info.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jobName = selectionListener_ == null ? "*CURRENT" : selectionListener_.getJobName();
			string jobName = selectionListener_ == null ? "*CURRENT" : selectionListener_.JobName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String userName = selectionListener_ == null ? "*ALL" : selectionListener_.getUserName();
			string userName = selectionListener_ == null ? "*ALL" : selectionListener_.UserName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jobNumber = selectionListener_ == null ? "*ALL" : selectionListener_.getJobNumber();
			string jobNumber = selectionListener_ == null ? "*ALL" : selectionListener_.JobNumber;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jobType = selectionListener_ == null ? "*" : selectionListener_.getJobType();
			string jobType = selectionListener_ == null ? "*" : selectionListener_.JobType;
			Conv.stringToBlankPadEBCDICByteArray(jobName, tempData, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(userName, tempData, 10, 10);
			Conv.stringToBlankPadEBCDICByteArray(jobNumber, tempData, 20, 6);
			Conv.stringToBlankPadEBCDICByteArray(jobType, tempData, 26, 1);
			tempData[27] = 0;
			int tempOffset = 28;
			if (selectionListener_ == null)
			{
			  //for (int i=0; i<8; ++i) out.writeInt(0);
			  for (int i = 28; i < 60; i += 2)
			  {
				tempData[i] = 0;
				tempData[i + 1] = 0;
			  }
			}
			else
			{
			  offset = selectionFormat_ == SELECTION_OLJS0200 ? 108 : 60;
			  Conv.intToByteArray(offset, tempData, tempOffset);
			  tempOffset += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int primaryJobStatusCount = selectionListener_.getPrimaryJobStatusCount();
			  int primaryJobStatusCount = selectionListener_.PrimaryJobStatusCount;
			  Conv.intToByteArray(primaryJobStatusCount, tempData, tempOffset);
			  tempOffset += 4;
			  offset += 10 * primaryJobStatusCount;
			  Conv.intToByteArray(offset, tempData, tempOffset);
			  tempOffset += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int activeJobStatusCount = selectionListener_.getActiveJobStatusCount();
			  int activeJobStatusCount = selectionListener_.ActiveJobStatusCount;
			  Conv.intToByteArray(activeJobStatusCount, tempData, tempOffset);
			  tempOffset += 4;
			  offset += 4 * activeJobStatusCount;
			  Conv.intToByteArray(offset, tempData, tempOffset);
			  tempOffset += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jobsOnJobQueueStatusCount = selectionListener_.getJobsOnJobQueueStatusCount();
			  int jobsOnJobQueueStatusCount = selectionListener_.JobsOnJobQueueStatusCount;
			  Conv.intToByteArray(jobsOnJobQueueStatusCount, tempData, tempOffset);
			  tempOffset += 4;
			  offset += 10 * jobsOnJobQueueStatusCount;
			  Conv.intToByteArray(offset, tempData, tempOffset);
			  tempOffset += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jobQueueNameCount = selectionListener_.getJobQueueNameCount();
			  int jobQueueNameCount = selectionListener_.JobQueueNameCount;
			  Conv.intToByteArray(jobQueueNameCount, tempData, tempOffset);
			  tempOffset += 4;
			  offset += 20 * jobQueueNameCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currentUserProfileCount = selectionListener_.getCurrentUserProfileCount();
			  int currentUserProfileCount = selectionListener_.CurrentUserProfileCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int serverTypeCount = selectionListener_.getServerTypeCount();
			  int serverTypeCount = selectionListener_.ServerTypeCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int activeSubsystemCount = selectionListener_.getActiveSubsystemCount();
			  int activeSubsystemCount = selectionListener_.ActiveSubsystemCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int memoryPoolCount = selectionListener_.getMemoryPoolCount();
			  int memoryPoolCount = selectionListener_.MemoryPoolCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jobTypeEnhancedCount = selectionListener_.getJobTypeEnhancedCount();
			  int jobTypeEnhancedCount = selectionListener_.JobTypeEnhancedCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qualifiedJobNameCount = selectionListener_.getQualifiedJobNameCount();
			  int qualifiedJobNameCount = selectionListener_.QualifiedJobNameCount;
			  if (selectionFormat_ == SELECTION_OLJS0200)
			  {
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(currentUserProfileCount, tempData, tempOffset);
				tempOffset += 4;
				offset += 10 * currentUserProfileCount;
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(serverTypeCount, tempData, tempOffset);
				tempOffset += 4;
				offset += 30 * serverTypeCount;
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(activeSubsystemCount, tempData, tempOffset);
				tempOffset += 4;
				offset += 10 * activeSubsystemCount;
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(memoryPoolCount, tempData, tempOffset);
				tempOffset += 4;
				offset += 4 * memoryPoolCount;
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(jobTypeEnhancedCount, tempData, tempOffset);
				tempOffset += 4;
				offset += 4 * jobTypeEnhancedCount;
				Conv.intToByteArray(offset, tempData, tempOffset);
				tempOffset += 4;
				Conv.intToByteArray(qualifiedJobNameCount, tempData, tempOffset);
				tempOffset += 4;
			  }
			  for (int i = 0; i < primaryJobStatusCount; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getPrimaryJobStatus(i), tempData, tempOffset, 10);
				tempOffset += 10;
			  }
			  for (int i = 0; i < selectionListener_.ActiveJobStatusCount; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getActiveJobStatus(i), tempData, tempOffset, 4);
				tempOffset += 4;
			  }
			  for (int i = 0; i < selectionListener_.JobsOnJobQueueStatusCount; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getJobsOnJobQueueStatus(i), tempData, tempOffset, 10);
				tempOffset += 10;
			  }
			  for (int i = 0; i < selectionListener_.JobQueueNameCount; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getJobQueueName(i), tempData, tempOffset, 20);
				tempOffset += 20;
			  }
			  if (selectionFormat_ == SELECTION_OLJS0200)
			  {
				for (int i = 0; i < currentUserProfileCount; ++i)
				{
				  Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getCurrentUserProfile(i), tempData, tempOffset, 10);
				  tempOffset += 10;
				}
				for (int i = 0; i < serverTypeCount; ++i)
				{
				  Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getServerType(i), tempData, tempOffset, 30);
				  tempOffset += 30;
				}
				for (int i = 0; i < activeSubsystemCount; ++i)
				{
				  Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getActiveSubsystem(i), tempData, tempOffset, 10);
				  tempOffset += 10;
				}
				for (int i = 0; i < memoryPoolCount; ++i)
				{
				  Conv.intToByteArray(selectionListener_.getMemoryPool(i), tempData, tempOffset);
				  tempOffset += 4;
				}
				for (int i = 0; i < jobTypeEnhancedCount; ++i)
				{
				  Conv.intToByteArray(selectionListener_.getJobTypeEnhanced(i), tempData, tempOffset);
				  tempOffset += 4;
				}
				for (int i = 0; i < qualifiedJobNameCount; ++i)
				{
				  Conv.stringToBlankPadEBCDICByteArray(selectionListener_.getQualifiedJobName(i), tempData, tempOffset, 26);
				  tempOffset += 26;
				}
			  }
			}
			return tempData;
		  case 9:
			  Conv.intToByteArray(getParameterInputLength(8), tempData, 0);
			  return tempData;
		  case 10:
			  Conv.intToByteArray(fieldsToReturn_.Length, tempData, 0);
			  return tempData;
		  case 11: // Key fields.
			for (int i = 0; i < fieldsToReturn_.Length; ++i)
			{
			  Conv.intToByteArray(fieldsToReturn_[i], tempData, i * 4);
			}
			return tempData;
		  case 12:
			  return ZERO;
		  case 13:
			  Conv.stringToEBCDICByteArray37(SelectionFormatName, tempData, 0);
			  return tempData;
		  case 14:
			  tempData[0] = resetStats_ ? unchecked((sbyte)0xF1) : unchecked((sbyte)0xF0);
			  return tempData;
		  case 16:
			  Conv.intToByteArray(16, tempData, 0);
			  return tempData;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		int numRead = 0;
		switch (parmIndex)
		{
		  case 0:
			if (formatter_ != null)
			{
			  formatter_.format(data, maxLength, formatter_.MinimumRecordLength, formatListener_);
			}
			break;
		  case 3:
			keyFields_ = null;
			if (maxLength >= 4)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfFields = Conv.byteArrayToInt(data, numRead);
			  int numberOfFields = Conv.byteArrayToInt(data, numRead);
			  numRead += 4;
			  keyFields_ = new OpenListOfJobsKeyField[numberOfFields];
			  for (int i = 0; i < numberOfFields && numRead + 20 <= maxLength; ++i)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lengthOfFieldInfo = Conv.byteArrayToInt(data, numRead);
				int lengthOfFieldInfo = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int keyField = Conv.byteArrayToInt(data, numRead);
				int keyField = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int typeOfData = data[numRead] & 0x00FF;
				int typeOfData = data[numRead] & 0x00FF;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isBinary = typeOfData == 0xC2;
				bool isBinary = typeOfData == 0xC2;
				numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lengthOfData = Conv.byteArrayToInt(data, numRead);
				int lengthOfData = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int displacementToData = Conv.byteArrayToInt(data, numRead);
				int displacementToData = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				keyFields_[i] = new OpenListOfJobsKeyField(keyField, isBinary, lengthOfData, displacementToData);
				int skip = lengthOfFieldInfo - 20;
				if (maxLength - numRead >= skip)
				{
				  numRead += skip;
				}
			  }
			}
			break;
		  case 5:
			if (maxLength < 12)
			{
			  info_ = null;
			}
			else
			{
			  info_ = com.ibm.jtopenlite.command.program.openlist.Util.readOpenListInformationParameter(data, maxLength);
			}
			break;
		  case 15:
			if (maxLength >= 16)
			{
			  // int bytesReturned = Conv.byteArrayToInt(data, 0);
			  // int bytesAvailable = Conv.byteArrayToInt(data, 4);
			  elapsedTime_ = Conv.byteArrayToLong(data, 8);
			}
			break;
		  default:
			break;
		}
	  }
	}


}