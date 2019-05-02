///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveObjectDescription.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.@object
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qusrobjd.htm">QUSROBJD</a>
	/// This class fully implements the V5R4 specification of QUSROBJD.
	/// 
	/// </summary>
	public class RetrieveObjectDescription : Program
	{
	  public const int FORMAT_OBJD0100 = 0;
	  public const int FORMAT_OBJD0200 = 1;
	  public const int FORMAT_OBJD0300 = 2;
	  public const int FORMAT_OBJD0400 = 3;

	  private int inputFormat_;
	  private int inputLength_;

	  private string inputLibrary_;
	  private string inputName_;
	  private string inputType_;

	  // All formats.
	  private string objectName_;
	  private string objectLibrary_;
	  private string objectType_;
	  private string returnLibrary_;
	  private int objectASPNumber_;
	  private string objectOwner_;
	  private string objectDomain_;
	  private string creationDateAndTime_;
	  private string objectChangeDateAndTime_;

	  // FORMAT_OBJD0200 and higher.
	  private string extendedObjectAttribute_;
	  private string textDescription_;
	  private string sourceFileName_;
	  private string sourceFileLibrary_;
	  private string sourceFileMember_;

	  // FORMAT_OBJD0300 and higher.
	  private string sourceFileUpdatedDateAndTime_;
	  private string objectSavedDateAndTime_;
	  private string objectRestoredDateAndTime_;
	  private string creatorUserProfile_;
	  private string systemWhereObjectWasCreated_;
	  private string resetDate_;
	  private int savedSize_;
	  private int saveSequenceNumber_;
	  private string storage_;
	  private string saveCommand_;
	  private string saveVolumeID_;
	  private string saveDevice_;
	  private string saveFileName_;
	  private string saveFileLibrary_;
	  private string saveLabel_;
	  private string systemLevel_;
	  private string compiler_;
	  private string objectLevel_;
	  private string userChanged_;
	  private string licensedProgram_;
	  private string ptf_;
	  private string apar_;

	  // FORMAT_OBJD0400.
	  private string lastUsedDate_;
	  private string usageInformationUpdated_;
	  private int daysUsedCount_;
	  private int objectSize_;
	  private int objectSizeMultiplier_;
	  private string objectCompressionStatus_;
	  private string allowChangeByProgram_;
	  private string changedByProgram_;
	  private string userDefinedAttribute_;
	  private string objectOverflowedASPIndicator_;
	  private string saveActiveDateAndTime_;
	  private string objectAuditingValue_;
	  private string primaryGroup_;
	  private string journalStatus_;
	  private string journalName_;
	  private string journalLibrary_;
	  private string journalImages_;
	  private string journalEntriesToBeOmitted_;
	  private string journalStartDateAndTime_;
	  private string digitallySigned_;
	  private int savedSizeInUnits_;
	  private int savedSizeMultiplier_;
	  private int libraryASPNumber_;
	  private string objectASPDeviceName_;
	  private string libraryASPDeviceName_;
	  private string digitallySignedBySystemTrustedSource_;
	  private string digitallySignedMoreThanOnce_;
	  private int primaryAssociatedSpaceSize_;
	  private string optimumSpaceAlignment_;
	  private string objectASPGroupName_;
	  private string libraryASPGroupName_;
	  private string startingJournalReceiverNameForApply_;
	  private string startingJournalReceiverLibrary_;
	  private string startingJournalReceiverLibraryASPDeviceName_;
	  private string startingJournalReceiverLibraryASPGroupName_;


	  private readonly char[] c = new char[71]; // Buffer for conversion.

	  private sbyte[] tempData_;

	  public RetrieveObjectDescription(string objectLibrary, string objectName, string objectType, int format)
	  {
		inputLibrary_ = objectLibrary;
		inputName_ = objectName;
		inputType_ = objectType;
		inputFormat_ = format;
		inputLength_ = FormatSize;
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QUSROBJD";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 5;
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

	  public virtual void newCall()
	  {
		objectName_ = null;
		objectLibrary_ = null;
		objectType_ = null;
		returnLibrary_ = null;
		objectASPNumber_ = 0;
		objectOwner_ = null;
		objectDomain_ = null;
		creationDateAndTime_ = null;
		objectChangeDateAndTime_ = null;

		// FORMAT_OBJD0200 and higher.
		extendedObjectAttribute_ = null;
		textDescription_ = null;
		sourceFileName_ = null;
		sourceFileLibrary_ = null;
		sourceFileMember_ = null;

		// FORMAT_OBJD0300 and higher.
		sourceFileUpdatedDateAndTime_ = null;
		objectSavedDateAndTime_ = null;
		objectRestoredDateAndTime_ = null;
		creatorUserProfile_ = null;
		systemWhereObjectWasCreated_ = null;
		resetDate_ = null;
		savedSize_ = 0;
		saveSequenceNumber_ = 0;
		storage_ = null;
		saveCommand_ = null;
		saveVolumeID_ = null;
		saveDevice_ = null;
		saveFileName_ = null;
		saveFileLibrary_ = null;
		saveLabel_ = null;
		systemLevel_ = null;
		compiler_ = null;
		objectLevel_ = null;
		userChanged_ = null;
		licensedProgram_ = null;
		ptf_ = null;
		apar_ = null;

		// FORMAT_OBJD0400.
		lastUsedDate_ = null;
		usageInformationUpdated_ = null;
		daysUsedCount_ = 0;
		objectSize_ = 0;
		objectSizeMultiplier_ = 0;
		objectCompressionStatus_ = null;
		allowChangeByProgram_ = null;
		changedByProgram_ = null;
		userDefinedAttribute_ = null;
		objectOverflowedASPIndicator_ = null;
		saveActiveDateAndTime_ = null;
		objectAuditingValue_ = null;
		primaryGroup_ = null;
		journalStatus_ = null;
		journalName_ = null;
		journalLibrary_ = null;
		journalImages_ = null;
		journalEntriesToBeOmitted_ = null;
		journalStartDateAndTime_ = null;
		digitallySigned_ = null;
		savedSizeInUnits_ = 0;
		savedSizeMultiplier_ = 0;
		libraryASPNumber_ = 0;
		objectASPDeviceName_ = null;
		libraryASPDeviceName_ = null;
		digitallySignedBySystemTrustedSource_ = null;
		digitallySignedMoreThanOnce_ = null;
		primaryAssociatedSpaceSize_ = 0;
		 optimumSpaceAlignment_ = null;
		 objectASPGroupName_ = null;
		 libraryASPGroupName_ = null;
		 startingJournalReceiverNameForApply_ = null;
		 startingJournalReceiverLibrary_ = null;
		 startingJournalReceiverLibraryASPDeviceName_ = null;
		 startingJournalReceiverLibraryASPGroupName_ = null;
	  }

	  private int FormatSize
	  {
		  get
		  {
			switch (inputFormat_)
			{
			  case FORMAT_OBJD0100:
				  return 90;
			  case FORMAT_OBJD0200:
				  return 180;
			  case FORMAT_OBJD0300:
				  return 460;
			  case FORMAT_OBJD0400:
				  return 667; //@O3C it should be 667
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
			  case FORMAT_OBJD0100:
				  return "OBJD0100";
			  case FORMAT_OBJD0200:
				  return "OBJD0200";
			  case FORMAT_OBJD0300:
				  return "OBJD0300";
			  case FORMAT_OBJD0400:
				  return "OBJD0400";
			}
			return null;
		  }
	  }

	  public virtual int Format
	  {
		  set
		  {
			inputFormat_ = value;
			inputLength_ = FormatSize;
		  }
	  }

	  public virtual string ObjectNameToRetrieve
	  {
		  set
		  {
			inputName_ = value;
		  }
	  }

	  public virtual string ObjectLibraryToRetrieve
	  {
		  set
		  {
			inputLibrary_ = value;
		  }
	  }

	  public virtual string ObjectTypeToRetrieve
	  {
		  set
		  {
			inputType_ = value;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectName
	  {
		  get
		  {
			return objectName_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectLibrary
	  {
		  get
		  {
			return objectLibrary_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectType
	  {
		  get
		  {
			return objectType_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ReturnLibrary
	  {
		  get
		  {
			return returnLibrary_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual int ObjectASPNumber
	  {
		  get
		  {
			return objectASPNumber_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectOwner
	  {
		  get
		  {
			return objectOwner_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectDomain
	  {
		  get
		  {
			return objectDomain_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string CreationDateAndTime
	  {
		  get
		  {
			return creationDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// All formats.
	  /// 
	  /// </summary>
	  public virtual string ObjectChangeDateAndTime
	  {
		  get
		  {
			return objectChangeDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0200, FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ExtendedObjectAttribute
	  {
		  get
		  {
			return extendedObjectAttribute_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0200, FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string TextDescription
	  {
		  get
		  {
			return textDescription_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0200, FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SourceFileName
	  {
		  get
		  {
			return sourceFileName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0200, FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SourceFileLibrary
	  {
		  get
		  {
			return sourceFileLibrary_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0200, FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SourceFileMember
	  {
		  get
		  {
			return sourceFileMember_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SourceFileUpdatedDateAndTime
	  {
		  get
		  {
			return sourceFileUpdatedDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectSavedDateAndTime
	  {
		  get
		  {
			return objectSavedDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectRestoredDateAndTime
	  {
		  get
		  {
			return objectRestoredDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string CreatorUserProfile
	  {
		  get
		  {
			return creatorUserProfile_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SystemWhereObjectWasCreated
	  {
		  get
		  {
			return systemWhereObjectWasCreated_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ResetDate
	  {
		  get
		  {
			return resetDate_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int SavedSize
	  {
		  get
		  {
			return savedSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int SaveSequenceNumber
	  {
		  get
		  {
			return saveSequenceNumber_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string Storage
	  {
		  get
		  {
			return storage_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveCommand
	  {
		  get
		  {
			return saveCommand_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveVolumeID
	  {
		  get
		  {
			return saveVolumeID_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveDevice
	  {
		  get
		  {
			return saveDevice_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveFileName
	  {
		  get
		  {
			return saveFileName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveFileLibrary
	  {
		  get
		  {
			return saveFileLibrary_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveLabel
	  {
		  get
		  {
			return saveLabel_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SystemLevel
	  {
		  get
		  {
			return systemLevel_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string Compiler
	  {
		  get
		  {
			return compiler_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectLevel
	  {
		  get
		  {
			return objectLevel_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string UserChanged
	  {
		  get
		  {
			return userChanged_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string LicensedProgram
	  {
		  get
		  {
			return licensedProgram_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string PTF
	  {
		  get
		  {
			return ptf_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0300, FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string APAR
	  {
		  get
		  {
			return apar_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string LastUsedDate
	  {
		  get
		  {
			return lastUsedDate_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string UsageInformationUpdated
	  {
		  get
		  {
			return usageInformationUpdated_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int DaysUsedCount
	  {
		  get
		  {
			return daysUsedCount_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int ObjectSize
	  {
		  get
		  {
			return objectSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int ObjectSizeMultiplier
	  {
		  get
		  {
			return objectSizeMultiplier_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectCompressionStatus
	  {
		  get
		  {
			return objectCompressionStatus_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string AllowChangeByProgram
	  {
		  get
		  {
			return allowChangeByProgram_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ChangedByProgram
	  {
		  get
		  {
			return changedByProgram_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string UserDefinedAttribute
	  {
		  get
		  {
			return userDefinedAttribute_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectOverflowedASPIndicator
	  {
		  get
		  {
			return objectOverflowedASPIndicator_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string SaveActiveDateAndTime
	  {
		  get
		  {
			return saveActiveDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectAuditingValue
	  {
		  get
		  {
			return objectAuditingValue_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string PrimaryGroup
	  {
		  get
		  {
			return primaryGroup_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalStatus
	  {
		  get
		  {
			return journalStatus_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalName
	  {
		  get
		  {
			return journalName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalLibrary
	  {
		  get
		  {
			return journalLibrary_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalImages
	  {
		  get
		  {
			return journalImages_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalEntriesToBeOmitted
	  {
		  get
		  {
			return journalEntriesToBeOmitted_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string JournalStartDateAndTime
	  {
		  get
		  {
			return journalStartDateAndTime_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string DigitallySigned
	  {
		  get
		  {
			return digitallySigned_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int SavedSizeInUnits
	  {
		  get
		  {
			return savedSizeInUnits_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int SavedSizeMultiplier
	  {
		  get
		  {
			return savedSizeMultiplier_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int LibraryASPNumber
	  {
		  get
		  {
			return libraryASPNumber_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectASPDeviceName
	  {
		  get
		  {
			return objectASPDeviceName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string LibraryASPDeviceName
	  {
		  get
		  {
			return libraryASPDeviceName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string DigitallySignedBySystemTrustedSource
	  {
		  get
		  {
			return digitallySignedBySystemTrustedSource_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string DigitallySignedMoreThanOnce
	  {
		  get
		  {
			return digitallySignedMoreThanOnce_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual int PrimaryAssociatedSpaceSize
	  {
		  get
		  {
			return primaryAssociatedSpaceSize_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string OptimumSpaceAlignment
	  {
		  get
		  {
			return optimumSpaceAlignment_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string ObjectASPGroupName
	  {
		  get
		  {
			return objectASPGroupName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string LibraryASPGroupName
	  {
		  get
		  {
			return libraryASPGroupName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string StartingJournalReceiverNameForApply
	  {
		  get
		  {
			return startingJournalReceiverNameForApply_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string StartingJournalReceiverLibrary
	  {
		  get
		  {
			return startingJournalReceiverLibrary_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string StartingJournalReceiverLibraryASPDeviceName
	  {
		  get
		  {
			return startingJournalReceiverLibraryASPDeviceName_;
		  }
	  }

	  /// <summary>
	  /// FORMAT_OBJD0400.
	  /// 
	  /// </summary>
	  public virtual string StartingJournalReceiverLibraryASPGroupName
	  {
		  get
		  {
			return startingJournalReceiverLibraryASPGroupName_;
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
			  return 8;
		  case 3:
			  return 20;
		  case 4:
			  return 10;
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
			  return Parameter.TYPE_OUTPUT;
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
			  Conv.stringToBlankPadEBCDICByteArray(inputName_, tempData, 0, 10);
			  Conv.stringToBlankPadEBCDICByteArray(inputLibrary_, tempData, 10, 10);
			  return tempData;
		  case 4:
			  Conv.stringToBlankPadEBCDICByteArray(inputType_, tempData, 0, 10);
			  return tempData;
		}
		return null;
	  }

	  private const string BLANK10 = "          ";
	  private const string ZERO = "0";
	  private const string ONE = "1";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final boolean isBlank10(final byte[] data, final int offset)
	  private static bool isBlank10(sbyte[] data, int offset)
	  {
	//    final int stop = offset+10;
	//    for (int i=offset; i<stop; ++i)
	//    {
	//      if (data[i] != 0x40) return false;
	//    }
	//    return true;
		return data[offset] == 0x40; // Since this is for QSYS objects, and they cannot start with a space.
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final boolean isZeroOrOne(final byte b)
	  private static bool isZeroOrOne(sbyte b)
	  {
		return (b & 0x00FE) == 0x00F0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final String getZeroOrOne(final byte b)
	  private static string getZeroOrOne(sbyte b)
	  {
		return b == unchecked((sbyte)0xF0) ? ZERO : ONE;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 0:
			// int bytesReturned = Conv.byteArrayToInt(data, 0);
			// int bytesAvailable = Conv.byteArrayToInt(data, 4);
			objectName_ = Conv.ebcdicByteArrayToString(data, 8, 10, c);
			objectLibrary_ = Conv.ebcdicByteArrayToString(data, 18, 10, c);
			objectType_ = Conv.ebcdicByteArrayToString(data, 28, 10, c);
			returnLibrary_ = Conv.ebcdicByteArrayToString(data, 38, 10, c);
			objectASPNumber_ = Conv.byteArrayToInt(data, 48);
			objectOwner_ = Conv.ebcdicByteArrayToString(data, 52, 10, c);
			objectDomain_ = Conv.ebcdicByteArrayToString(data, 62, 2, c);
			creationDateAndTime_ = Conv.ebcdicByteArrayToString(data, 64, 13, c);
			objectChangeDateAndTime_ = Conv.ebcdicByteArrayToString(data, 77, 13, c);
			if (inputFormat_ >= FORMAT_OBJD0200)
			{
			  extendedObjectAttribute_ = Conv.ebcdicByteArrayToString(data, 90, 10, c);
			  textDescription_ = Conv.ebcdicByteArrayToString(data, 100, 50, c);
			  sourceFileName_ = isBlank10(data, 150) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 150, 10, c);
			  sourceFileLibrary_ = isBlank10(data, 160) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 160, 10, c);
			  sourceFileMember_ = isBlank10(data, 170) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 170, 10, c);

			  if (inputFormat_ >= FORMAT_OBJD0300)
			  {
				sourceFileUpdatedDateAndTime_ = Conv.ebcdicByteArrayToString(data, 180, 13, c);
				objectSavedDateAndTime_ = Conv.ebcdicByteArrayToString(data, 193, 13, c);
				objectRestoredDateAndTime_ = Conv.ebcdicByteArrayToString(data, 206, 13, c);
				creatorUserProfile_ = Conv.ebcdicByteArrayToString(data, 219, 10, c);
				systemWhereObjectWasCreated_ = Conv.ebcdicByteArrayToString(data, 229, 8, c);
				resetDate_ = Conv.ebcdicByteArrayToString(data, 237, 7, c);
				savedSize_ = Conv.byteArrayToInt(data, 244);
				saveSequenceNumber_ = Conv.byteArrayToInt(data, 248);
				storage_ = Conv.ebcdicByteArrayToString(data, 252, 10, c);
				saveCommand_ = Conv.ebcdicByteArrayToString(data, 262, 10, c);
				saveVolumeID_ = Conv.ebcdicByteArrayToString(data, 272, 71, c);
				saveDevice_ = isBlank10(data, 343) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 343, 10, c);
				saveFileName_ = isBlank10(data, 353) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 353, 10, c);
				saveFileLibrary_ = isBlank10(data, 363) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 363, 10, c);
				saveLabel_ = Conv.ebcdicByteArrayToString(data, 373, 17, c);
				systemLevel_ = Conv.ebcdicByteArrayToString(data, 390, 9, c);
				compiler_ = Conv.ebcdicByteArrayToString(data, 399, 16, c);
				objectLevel_ = Conv.ebcdicByteArrayToString(data, 415, 8, c);
				userChanged_ = isZeroOrOne(data[423]) ? getZeroOrOne(data[423]) : Conv.ebcdicByteArrayToString(data, 423, 1, c);
				licensedProgram_ = Conv.ebcdicByteArrayToString(data, 424, 16, c);
				ptf_ = isBlank10(data, 440) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 440, 10, c);
				apar_ = isBlank10(data, 450) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 450, 10, c);

				if (inputFormat_ == FORMAT_OBJD0400)
				{
				  lastUsedDate_ = Conv.ebcdicByteArrayToString(data, 460, 7, c);
				  usageInformationUpdated_ = Conv.ebcdicByteArrayToString(data, 467, 1, c);
				  daysUsedCount_ = Conv.byteArrayToInt(data, 468);
				  objectSize_ = Conv.byteArrayToInt(data, 472);
				  objectSizeMultiplier_ = Conv.byteArrayToInt(data, 476);
				  objectCompressionStatus_ = Conv.ebcdicByteArrayToString(data, 480, 1, c);
				  allowChangeByProgram_ = isZeroOrOne(data[481]) ? getZeroOrOne(data[481]) : Conv.ebcdicByteArrayToString(data, 481, 1, c);
				  changedByProgram_ = isZeroOrOne(data[482]) ? getZeroOrOne(data[482]) : Conv.ebcdicByteArrayToString(data, 482, 1, c);
				  userDefinedAttribute_ = isBlank10(data, 483) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 483, 10, c);
				  objectOverflowedASPIndicator_ = Conv.ebcdicByteArrayToString(data, 493, 1, c);
				  saveActiveDateAndTime_ = Conv.ebcdicByteArrayToString(data, 494, 13, c);
				  objectAuditingValue_ = Conv.ebcdicByteArrayToString(data, 507, 10, c);
				  primaryGroup_ = Conv.ebcdicByteArrayToString(data, 517, 10, c);
				  journalStatus_ = isZeroOrOne(data[527]) ? getZeroOrOne(data[527]) : Conv.ebcdicByteArrayToString(data, 527, 1, c);
				  journalName_ = isBlank10(data, 528) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 528, 10, c);
				  journalLibrary_ = isBlank10(data, 538) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 538, 10, c);
				  journalImages_ = isZeroOrOne(data[548]) ? getZeroOrOne(data[548]) : Conv.ebcdicByteArrayToString(data, 548, 1, c);
				  journalEntriesToBeOmitted_ = isZeroOrOne(data[549]) ? getZeroOrOne(data[549]) : Conv.ebcdicByteArrayToString(data, 549, 1, c);
				  journalStartDateAndTime_ = Conv.ebcdicByteArrayToString(data, 550, 13, c);
				  digitallySigned_ = isZeroOrOne(data[564]) ? getZeroOrOne(data[564]) : Conv.ebcdicByteArrayToString(data, 563, 1, c);
				  savedSizeInUnits_ = Conv.byteArrayToInt(data, 564);
				  savedSizeMultiplier_ = Conv.byteArrayToInt(data, 568);
				  libraryASPNumber_ = Conv.byteArrayToInt(data, 572);
				  objectASPDeviceName_ = isBlank10(data, 576) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 576, 10, c);
				  libraryASPDeviceName_ = isBlank10(data, 586) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 586, 10, c);
				  digitallySignedBySystemTrustedSource_ = isZeroOrOne(data[596]) ? getZeroOrOne(data[596]) : Conv.ebcdicByteArrayToString(data, 596, 1, c);
				  digitallySignedMoreThanOnce_ = isZeroOrOne(data[597]) ? getZeroOrOne(data[597]) : Conv.ebcdicByteArrayToString(data, 597, 1, c);
				  primaryAssociatedSpaceSize_ = Conv.byteArrayToInt(data, 600);
				  optimumSpaceAlignment_ = Conv.ebcdicByteArrayToString(data, 604, 1, c);
				  objectASPGroupName_ = Conv.ebcdicByteArrayToString(data, 605, 10, c);
				  libraryASPGroupName_ = Conv.ebcdicByteArrayToString(data, 615, 10, c);
				  startingJournalReceiverNameForApply_ = isBlank10(data, 625) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 625, 10, c);
				  startingJournalReceiverLibrary_ = isBlank10(data, 635) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 635, 10, c);
				  startingJournalReceiverLibraryASPDeviceName_ = isBlank10(data, 645) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 645, 10, c);
				  startingJournalReceiverLibraryASPGroupName_ = isBlank10(data, 655) ? BLANK10 : Conv.ebcdicByteArrayToString(data, 655, 10, c);
				}
			  }
			}
			break;
		  default:
			break;
		}
	  }
	}


}