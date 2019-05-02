///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveSystemStatus.java
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

	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qwcrssts.htm">QWCRSSTS</a>
	/// This class fully implements the V5R4 specification of QWCRSSTS.
	/// 
	/// </summary>
	public class RetrieveSystemStatus : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const int FORMAT_SSTS0100 = 0;
	  public const int FORMAT_SSTS0200 = 1;
	  public const int FORMAT_SSTS0300 = 2;
	  public const int FORMAT_SSTS0400 = 3;
	  public const int FORMAT_SSTS0500 = 4;

	  private int inputLength_;
	  private int inputFormat_;
	  private string inputReset_;

	  private int bytesAvailable_;
	  private int bytesReturned_;

	  private long currentDateAndTime_;
	  private string systemName_;

	  // Format 0100.
	  private int usersCurrentlySignedOn_;
	  private int usersTemporarilySignedOff_;
	  private int usersSuspendedBySystemRequest_;
	  private int usersSuspendedByGroupJobs_;
	  private int usersSignedOffWithPrinterOutputWaitingToPrint_;
	  private int batchJobsWaitingForMessages_;
	  private int batchJobsRunning_;
	  private int batchJobsHeldWhileRunning_;
	  private int batchJobsEnding_;
	  private int batchJobsWaitingToRunOrAlreadyScheduled_;
	  private int batchJobsHeldOnAJobQueue_;
	  private int batchJobsOnAHeldJobQueue_;
	  private int batchJobsOnAnUnassignedJobQueue_;
	  private int batchJobsEndedWithPrinterOutputWaitingToPrint_;

	  // Format 0200.
	  private string elapsedTime_;
	  private int percentProcessingUnitUsed_;
	  private int jobsInSystem_;
	  private int percentPermanentAddresses_;
	  private int percentTemporaryAddresses_;
	  private int systemASP_;
	  private int percentSystemASPUsed_;
	  private int totalAuxiliaryStorage_;
	  private int currentUnprotectedStorageUsed_;
	  private int maximumUnprotectedStorageUsed_;
	  private int percentDBCapability_;
	  private int mainStorageSize_;
	  private int numberOfPartitions_;
	  private int partitionIdentifier_;
	  private int currentProcessingCapacity_;
	  private sbyte processorSharingAttribute_;
	  public static readonly sbyte DEDICATED = unchecked((sbyte)0xF0);
	  public static readonly sbyte SHARED_CAPPED = unchecked((sbyte)0xF1);
	  public static readonly sbyte SHARED_UNCAPPED = unchecked((sbyte)0xF2);
	  private int numberOfProcessors_;
	  private int activeJobsInSystem_;
	  private int activeThreadsInSystem_;
	  private int maximumJobsInSystem_;
	  private int percentTemporary256MBSegmentsUsed_;
	  private int percentTemporary4GBSegmentsUsed_;
	  private int percentPermanent256MBSegmentsUsed_;
	  private int percentPermanent4GBSegmentsUsed_;
	  private int percentCurrentInteractivePerformance_;
	  private int percentUncappedCPUCapacityUsed_;
	  private int percentSharedProcessorPoolUsed_;
	  private long mainStorageSizeLong_;

	  // Format 0300.
	  private int numberOfPools_;
	  private RetrieveSystemStatusPoolListener poolListener_;

	  // Format 0400.
	  private int minimumMachinePoolSize_;
	  private int minimumBasePoolSize_;

	  private string poolSelectionTypeOfPool_;
	  private string poolSelectionSharedPoolName_;
	  private int poolSelectionSystemPoolIdentifier_;

	  public const string TYPE_SHARED = "*SHARED";
	  public const string TYPE_SYSTEM = "*SYSTEM";

	  public const string SELECT_ALL = "*ALL";
	  public const string SELECT_MACHINE = "*MACHINE";
	  public const string SELECT_BASE = "*BASE";
	  public const string SELECT_INTERACT = "*INTERACT";
	  public const string SELECT_SPOOL = "*SPOOL";

	  // Format 0500.
	  private int numberOfSubsystemsAvailable_;
	  private int numberOfSubsystemsReturned_;

	  private sbyte[] tempData_;

	  public RetrieveSystemStatus(int format, bool resetStatistics)
	  {
		inputFormat_ = format;
		inputReset_ = resetStatistics ? "*YES" : "*NO";
		inputLength_ = FormatSize;
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

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QWCRSSTS";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }

	  public virtual void newCall()
	  {
		bytesAvailable_ = 0;
		bytesReturned_ = 0;

		currentDateAndTime_ = 0;
		systemName_ = null;

		// Format 0100.
		usersCurrentlySignedOn_ = 0;
		usersTemporarilySignedOff_ = 0;
		usersSuspendedBySystemRequest_ = 0;
		usersSuspendedByGroupJobs_ = 0;
		usersSignedOffWithPrinterOutputWaitingToPrint_ = 0;
		batchJobsWaitingForMessages_ = 0;
		batchJobsRunning_ = 0;
		batchJobsHeldWhileRunning_ = 0;
		batchJobsEnding_ = 0;
		batchJobsWaitingToRunOrAlreadyScheduled_ = 0;
		batchJobsHeldOnAJobQueue_ = 0;
		batchJobsOnAHeldJobQueue_ = 0;
		batchJobsOnAnUnassignedJobQueue_ = 0;
		batchJobsEndedWithPrinterOutputWaitingToPrint_ = 0;

		// Format 0200.
		elapsedTime_ = null;
		percentProcessingUnitUsed_ = 0;
		jobsInSystem_ = 0;
		percentPermanentAddresses_ = 0;
		percentTemporaryAddresses_ = 0;
		systemASP_ = 0;
		percentSystemASPUsed_ = 0;
		totalAuxiliaryStorage_ = 0;
		currentUnprotectedStorageUsed_ = 0;
		maximumUnprotectedStorageUsed_ = 0;
		percentDBCapability_ = 0;
		mainStorageSize_ = 0;
		numberOfPartitions_ = 0;
		partitionIdentifier_ = 0;
		currentProcessingCapacity_ = 0;
		processorSharingAttribute_ = 0;
		numberOfProcessors_ = 0;
		activeJobsInSystem_ = 0;
		activeThreadsInSystem_ = 0;
		maximumJobsInSystem_ = 0;
		percentTemporary256MBSegmentsUsed_ = 0;
		percentTemporary4GBSegmentsUsed_ = 0;
		percentPermanent256MBSegmentsUsed_ = 0;
		percentPermanent4GBSegmentsUsed_ = 0;
		percentCurrentInteractivePerformance_ = 0;
		percentUncappedCPUCapacityUsed_ = 0;
		percentSharedProcessorPoolUsed_ = 0;
		mainStorageSizeLong_ = 0;

		// Format 0300.
		numberOfPools_ = 0;

		// Format 0400.
		minimumMachinePoolSize_ = 0;
		minimumBasePoolSize_ = 0;

		// Format 0500.
		numberOfSubsystemsAvailable_ = 0;
		numberOfSubsystemsReturned_ = 0;
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return string.ReferenceEquals(poolSelectionTypeOfPool_, null) ? 5 : 7;
		  }
	  }

	  public virtual RetrieveSystemStatusPoolListener PoolListener
	  {
		  set
		  {
			poolListener_ = value;
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
			inputLength_ = value <= 0 ? 1 : value;
		  }
	  }


	  public virtual int Format
	  {
		  set
		  {
			inputFormat_ = value;
			if (FormatSize > inputLength_)
			{
				inputLength_ = FormatSize;
			}
		  }
	  }

	  public virtual bool ResetStatistics
	  {
		  set
		  {
			inputReset_ = value ? "*YES" : "*NO";
		  }
	  }

	  public virtual int BytesAvailable
	  {
		  get
		  {
			return bytesAvailable_;
		  }
	  }

	  public virtual int BytesReturned
	  {
		  get
		  {
			return bytesReturned_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string SystemName
	  {
		  get
		  {
			return systemName_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual long CurrentDateAndTime
	  {
		  get
		  {
			return currentDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int UsersCurrentlySignedOn
	  {
		  get
		  {
			return usersCurrentlySignedOn_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int UsersTemporarilySignedOff
	  {
		  get
		  {
			return usersTemporarilySignedOff_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int UsersSuspendedBySystemRequest
	  {
		  get
		  {
			return usersSuspendedBySystemRequest_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int UsersSuspendedByGroupJobs
	  {
		  get
		  {
			return usersSuspendedByGroupJobs_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int UsersSignedOffWithPrinterOutputWaitingToPrint
	  {
		  get
		  {
			return usersSignedOffWithPrinterOutputWaitingToPrint_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsWaitingForMessages
	  {
		  get
		  {
			return batchJobsWaitingForMessages_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsRunning
	  {
		  get
		  {
			return batchJobsRunning_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsHeldWhileRunning
	  {
		  get
		  {
			return batchJobsHeldWhileRunning_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsEnding
	  {
		  get
		  {
			return batchJobsEnding_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsWaitingToRunOrAlreadyScheduled
	  {
		  get
		  {
			return batchJobsWaitingToRunOrAlreadyScheduled_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsHeldOnAJobQueue
	  {
		  get
		  {
			return batchJobsHeldOnAJobQueue_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsOnAHeldJobQueue
	  {
		  get
		  {
			return batchJobsOnAHeldJobQueue_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsOnAnUnassignedJobQueue
	  {
		  get
		  {
			return batchJobsOnAnUnassignedJobQueue_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0100.
	  /// 
	  /// </summary>
	  public virtual int BatchJobsEndedWithPrinterOutputWaitingToPrint
	  {
		  get
		  {
			return batchJobsEndedWithPrinterOutputWaitingToPrint_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200, FORMAT_SSTS0300.
	  /// 
	  /// </summary>
	  public virtual string ElapsedTime
	  {
		  get
		  {
			return elapsedTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentProcessingUnitUsed
	  {
		  get
		  {
			return percentProcessingUnitUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int JobsInSystem
	  {
		  get
		  {
			return jobsInSystem_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentPermanentAddresses
	  {
		  get
		  {
			return percentPermanentAddresses_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentTemporaryAddresses
	  {
		  get
		  {
			return percentTemporaryAddresses_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int SystemASP
	  {
		  get
		  {
			return systemASP_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentSystemASPUsed
	  {
		  get
		  {
			return percentSystemASPUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int TotalAuxiliaryStorage
	  {
		  get
		  {
			return totalAuxiliaryStorage_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int CurrentUnprotectedStorageUsed
	  {
		  get
		  {
			return currentUnprotectedStorageUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int MaximumUnprotectedStorageUsed
	  {
		  get
		  {
			return maximumUnprotectedStorageUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentDBCapability
	  {
		  get
		  {
			return percentDBCapability_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200, FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  public virtual int MainStorageSize
	  {
		  get
		  {
			return mainStorageSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int NumberOfPartitions
	  {
		  get
		  {
			return numberOfPartitions_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PartitionIdentifier
	  {
		  get
		  {
			return partitionIdentifier_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int CurrentProcessingCapacity
	  {
		  get
		  {
			return currentProcessingCapacity_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual sbyte ProcessorSharingAttribute
	  {
		  get
		  {
			return processorSharingAttribute_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int NumberOfProcessors
	  {
		  get
		  {
			return numberOfProcessors_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int ActiveJobsInSystem
	  {
		  get
		  {
			return activeJobsInSystem_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int ActiveThreadsInSystem
	  {
		  get
		  {
			return activeThreadsInSystem_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int MaximumJobsInSystem
	  {
		  get
		  {
			return maximumJobsInSystem_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentTemporary256MBSegmentsUsed
	  {
		  get
		  {
			return percentTemporary256MBSegmentsUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentTemporary4GBSegmentsUsed
	  {
		  get
		  {
			return percentTemporary4GBSegmentsUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentPermanent256MBSegmentsUsed
	  {
		  get
		  {
			return percentPermanent256MBSegmentsUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentPermanent4GBSegmentsUsed
	  {
		  get
		  {
			return percentPermanent4GBSegmentsUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentCurrentInteractivePerformance
	  {
		  get
		  {
			return percentCurrentInteractivePerformance_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentUncappedCPUCapacityUsed
	  {
		  get
		  {
			return percentUncappedCPUCapacityUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200.
	  /// 
	  /// </summary>
	  public virtual int PercentSharedProcessorPoolUsed
	  {
		  get
		  {
			return percentSharedProcessorPoolUsed_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0200, FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  public virtual long MainStorageSizeLong
	  {
		  get
		  {
			return mainStorageSizeLong_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0300, FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  public virtual int NumberOfPools
	  {
		  get
		  {
			return numberOfPools_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  public virtual int MinimumMachinePoolSize
	  {
		  get
		  {
			return minimumMachinePoolSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  public virtual int MinimumBasePoolSize
	  {
		  get
		  {
			return minimumBasePoolSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0500.
	  /// 
	  /// </summary>
	  public virtual int NumberOfSubsystemsAvailable
	  {
		  get
		  {
			return numberOfSubsystemsAvailable_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0500.
	  /// 
	  /// </summary>
	  public virtual int NumberOfSubsystemsReturned
	  {
		  get
		  {
			return numberOfSubsystemsReturned_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_SSTS0400, FORMAT_SSTS0500.
	  /// 
	  /// </summary>
	  public virtual void setPoolSelectionInformation(string typeOfPool, string sharedPoolName, int systemPoolIdentifier)
	  {
		poolSelectionTypeOfPool_ = typeOfPool;
		poolSelectionSharedPoolName_ = sharedPoolName;
		poolSelectionSystemPoolIdentifier_ = systemPoolIdentifier;
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
			  return 10;
		  case 4:
			  return 4;
		  case 5:
			  return 24;
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
		  case 4:
			  return 4;
		}
		return 0;
	  }

	  private int FormatSize
	  {
		  get
		  {
			switch (inputFormat_)
			{
			  case FORMAT_SSTS0100:
				  return 80;
			  case FORMAT_SSTS0200:
				  return 148;
			  case FORMAT_SSTS0300:
				  return 128; // Just a guess. Minimum to hold info for one pool entry.
			  case FORMAT_SSTS0400:
				  return 244; // Just a guess. Minimum to hold info for one pool entry.
			  case FORMAT_SSTS0500:
				  return 74; // Just a guess. Minimum to hold info for one subsystem entry.
			}
			return 0;
		  }
	  }

	  private string FormatName
	  {
		  get
		  {
			switch (inputFormat_)
			{
			  case FORMAT_SSTS0100:
				  return "SSTS0100";
			  case FORMAT_SSTS0200:
				  return "SSTS0200";
			  case FORMAT_SSTS0300:
				  return "SSTS0300";
			  case FORMAT_SSTS0400:
				  return "SSTS0400";
			  case FORMAT_SSTS0500:
				  return "SSTS0500";
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
			  return Parameter.TYPE_OUTPUT;
		  case 4:
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
			  Conv.stringToEBCDICByteArray37(FormatName, tempData, 0);
			  return tempData;
		  case 3:
			  Conv.stringToBlankPadEBCDICByteArray(inputReset_, tempData, 0, 10);
			  return tempData;
		  case 4:
			  return ZERO;
		  case 5:
			  Conv.stringToBlankPadEBCDICByteArray(poolSelectionTypeOfPool_, tempData, 0, 10);
			  Conv.stringToBlankPadEBCDICByteArray(poolSelectionSharedPoolName_, tempData, 10, 10);
			  Conv.intToByteArray(poolSelectionSystemPoolIdentifier_, tempData, 20);
			  return tempData;
		  case 6:
			  Conv.intToByteArray(24, tempData, 0);
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
			bytesAvailable_ = Conv.byteArrayToInt(data, 0);
			bytesReturned_ = Conv.byteArrayToInt(data, 4);
			int numRead = 8;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] c = new char[50];
			char[] c = new char[50];
			switch (inputFormat_)
			{
			  case FORMAT_SSTS0100:
				currentDateAndTime_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				systemName_ = Conv.ebcdicByteArrayToString(data, numRead, 8, c);
				numRead += 8;
				usersCurrentlySignedOn_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				usersTemporarilySignedOff_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				usersSuspendedBySystemRequest_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				usersSuspendedByGroupJobs_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				usersSignedOffWithPrinterOutputWaitingToPrint_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsWaitingForMessages_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsRunning_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsHeldWhileRunning_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsEnding_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsWaitingToRunOrAlreadyScheduled_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsHeldOnAJobQueue_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsOnAHeldJobQueue_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsOnAnUnassignedJobQueue_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				batchJobsEndedWithPrinterOutputWaitingToPrint_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				break;
			  case FORMAT_SSTS0200:
				currentDateAndTime_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				systemName_ = Conv.ebcdicByteArrayToString(data, numRead, 8, c);
				numRead += 8;
				elapsedTime_ = Conv.ebcdicByteArrayToString(data, numRead, 6, c);
				numRead += 6;
				numRead += 2;
				percentProcessingUnitUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				jobsInSystem_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentPermanentAddresses_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentTemporaryAddresses_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				systemASP_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentSystemASPUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				totalAuxiliaryStorage_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				currentUnprotectedStorageUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				maximumUnprotectedStorageUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentDBCapability_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				mainStorageSize_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				numberOfPartitions_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				partitionIdentifier_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				numRead += 4;
				currentProcessingCapacity_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				processorSharingAttribute_ = data[numRead];
				numRead += 4;
				numberOfProcessors_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				activeJobsInSystem_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				activeThreadsInSystem_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				maximumJobsInSystem_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentTemporary256MBSegmentsUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentTemporary4GBSegmentsUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentPermanent256MBSegmentsUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentPermanent4GBSegmentsUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentCurrentInteractivePerformance_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentUncappedCPUCapacityUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				percentSharedProcessorPoolUsed_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				if (bytesReturned_ >= numRead + 8)
				{
				  mainStorageSizeLong_ = Conv.byteArrayToLong(data, numRead);
				  numRead += 8;
				}
				break;
			  case FORMAT_SSTS0300:
				currentDateAndTime_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				systemName_ = Conv.ebcdicByteArrayToString(data, numRead, 8, c);
				numRead += 8;
				elapsedTime_ = Conv.ebcdicByteArrayToString(data, numRead, 6, c);
				numRead += 6;
				numRead += 2;
				numberOfPools_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				int offsetToPoolInfo = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				int lengthOfPoolInfoEntry = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				int skip = numRead - offsetToPoolInfo;
				numRead += skip;
				if (poolListener_ != null)
				{
				  //final byte[] b10 = new byte[10];
				  // final char[] c10 = new char[10];
				  while (numRead + lengthOfPoolInfoEntry <= maxLength)
				  {
					readBasicPoolInfo(data, numRead, c);
					numRead += 84;
					skip = lengthOfPoolInfoEntry - 84;
					numRead += skip;
				  }
				}
				break;
			  case FORMAT_SSTS0400:
				currentDateAndTime_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				systemName_ = Conv.ebcdicByteArrayToString(data, numRead, 8, c);
				numRead += 8;
				elapsedTime_ = Conv.ebcdicByteArrayToString(data, numRead, 6, c);
				numRead += 6;
				numRead += 2;
				mainStorageSize_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				minimumMachinePoolSize_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				minimumBasePoolSize_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				numberOfPools_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				offsetToPoolInfo = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				lengthOfPoolInfoEntry = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				mainStorageSizeLong_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				skip = numRead - offsetToPoolInfo;
				numRead += skip;
				if (poolListener_ != null)
				{
				  while (numRead + lengthOfPoolInfoEntry <= maxLength)
				  {
					int systemPoolIdentifier = readBasicPoolInfo(data, numRead, c);
					numRead += 84;
					int definedSize = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int currentThreads = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int currentIneligibleThreads = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningPriority = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningMinimumPoolSizePercent = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningMaximumPoolSizePercent = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningMinimumFaults = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningPerThreadFaults = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningMaximumFaults = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					string description = Conv.ebcdicByteArrayToString(data, numRead, 50, c);
					int status = data[numRead] & 0x00FF;
					numRead += 2;
					int tuningMinimumActivityLevel = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int tuningMaximumActivityLevel = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					poolListener_.extraPoolInfo(systemPoolIdentifier, definedSize, currentThreads, currentIneligibleThreads, tuningPriority, tuningMinimumPoolSizePercent, tuningMaximumPoolSizePercent, tuningMinimumFaults, tuningPerThreadFaults, tuningMaximumFaults, description, status, tuningMinimumActivityLevel, tuningMaximumActivityLevel);
					skip = lengthOfPoolInfoEntry - 180;
					numRead += skip;
				  }
				}
				break;
			  case FORMAT_SSTS0500:
				currentDateAndTime_ = Conv.byteArrayToLong(data, numRead);
				numRead += 8;
				systemName_ = Conv.ebcdicByteArrayToString(data, numRead, 8, c);
				numRead += 8;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int systemPoolIdentifier = Conv.byteArrayToInt(data, numRead);
				int systemPoolIdentifier = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				numberOfSubsystemsAvailable_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				numberOfSubsystemsReturned_ = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				int offsetToSubsystemInfo = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				int lengthOfSubsystemInfoEntry = Conv.byteArrayToInt(data, numRead);
				numRead += 4;
				string poolName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
				numRead += 10;
				skip = numRead - offsetToSubsystemInfo;
				numRead += skip;
				if (poolListener_ != null)
				{
				  for (int i = 0; i < numberOfSubsystemsReturned_; ++i)
				  {
					string subsystemName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					string subsystemLibrary = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					poolListener_.newSubsystemInfo(systemPoolIdentifier, poolName, subsystemName, subsystemLibrary);
					skip = lengthOfSubsystemInfoEntry - 20;
					numRead += skip;
				  }
				}
				break;
			}
			break;
		  default:
			break;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private int readBasicPoolInfo(final byte[] data, int numRead, final char[] c)
	  private int readBasicPoolInfo(sbyte[] data, int numRead, char[] c)
	  {
		int systemPoolIdentifier = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int size = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		long poolSize = (size < 0) ? ((long)size) & 0x00000000FFFFFFFFL : size;
		size = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		long reservedSize = (size < 0) ? ((long)size) & 0x00000000FFFFFFFFL : size;
		int maximumActiveThreads = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int databaseFaults = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int databasePages = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int nonDatabaseFaults = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int nonDatabasePages = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int activeToWait = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int waitToIneligible = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		int activeToIneligible = Conv.byteArrayToInt(data, numRead);
		numRead += 4;
		string poolName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
		numRead += 10;
		string subsystemName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
		numRead += 10;
		string subsystemLibrary = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
		numRead += 10;
		string pagingOption = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
		numRead += 10;
		poolListener_.newPoolInfo(systemPoolIdentifier, poolSize, reservedSize, maximumActiveThreads, databaseFaults, databasePages, nonDatabaseFaults, nonDatabasePages, activeToWait, waitToIneligible, activeToIneligible, poolName, subsystemName, subsystemLibrary, pagingOption);
		return systemPoolIdentifier;
	  }
	}


}