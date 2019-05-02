///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseServerAttributes.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public class DatabaseServerAttributes : AttributeTranslateIndicator
	{

	  public const int CC_NONE = 0;
	  public const int CC_CS = 1;
	  public const int CC_CHG = 2;
	  public const int CC_ALL = 3;
	  public const int CC_RR = 4;
	  public const int AUTOCOMMIT_OFF = 0xd5;
	  public const int AUTOCOMMIT_ON = 0xe8;


	  private int defaultClientCCSID_;
	  private bool defaultClientCCSIDSet_;
	  private string languageFeatureCode_;
	  private string clientFunctionalLevel_;
	  private int nlssIdentifier_;
	  private bool nlssIdentifierSet_;
	  private string nlssIdentifierLanguageID_;
	  private string nlssIdentifierLanguageTableName_;
	  private string nlssIdentifierLanguageTableLibrary_;
	  private int translateIndicator_;
	  private bool translateIndicatorSet_;
	  private int drdaPackageSize_;
	  private bool drdaPackageSizeSet_;
	  private int dateFormatParserOption_;
	  private bool dateFormatParserOptionSet_;
	  private int dateSeparatorParserOption_;
	  private bool dateSeparatorParserOptionSet_;
	  private int timeFormatParserOption_;
	  private bool timeFormatParserOptionSet_;
	  private int timeSeparatorParserOption_;
	  private bool timeSeparatorParserOptionSet_;
	  private int decimalSeparatorParserOption_;
	  private bool decimalSeparatorParserOptionSet_;
	  private int namingConventionParserOption_;
	  private bool namingConventionParserOptionSet_;
	  private int ignoreDecimalDataErrorParserOption_;
	  private bool ignoreDecimalDataErrorParserOptionSet_;
	  private int commitmentControlLevelParserOption_;
	  private bool commitmentControlLevelParserOptionSet_;
	  private string defaultSQLLibraryName_;
	  private int asciiCCSIDForTranslationTable_;
	  private bool asciiCCSIDForTranslationTableSet_;
	  private int ambiguousSelectOption_;
	  private bool ambiguousSelectOptionSet_;
	  private int packageAddStatementAllowed_;
	  private bool packageAddStatementAllowedSet_;
	  // Skipped the "Data Source Name" related parameters, JTOpen doesn't do these either.
	  private int useExtendedFormats_;
	  private bool useExtendedFormatsSet_;
	  private int lobFieldThreshold_;
	  private bool lobFieldThresholdSet_;
	  private int dataCompressionParameter_;
	  private bool dataCompressionParameterSet_;
	  private int trueAutoCommitIndicator_;
	  private bool trueAutoCommitIndicatorSet_;
	  private int clientSupportInformation_;
	  private bool clientSupportInformationSet_;
	  private string rdbName_;
	  private int maximumDecimalPrecision_;
	  private int maximumDecimalScale_;
	  private int minimumDivideScale_;
	  private bool decimalPrecisionAndScaleAttributesSet_;
	  private int hexadecimalConstantParserOption_;
	  private bool hexadecimalConstantParserOptionSet_;
	  private int inputLocatorType_;
	  private bool inputLocatorTypeSet_;
	  private int locatorPersistence_;
	  private bool locatorPersistenceSet_;
	  private sbyte[] ewlmCorrelator_;
	  private sbyte[] rleCompression_;
	  private int optimizationGoalIndicator_;
	  private bool optimizationGoalIndicatorSet_;
	  private int queryStorageLimit_;
	  private bool queryStorageLimitSet_;
	  private int decimalFloatingPointRoundingModeOption_;
	  private bool decimalFloatingPointRoundingModeOptionSet_;
	  private int decimalFloatingPointErrorReportingOption_;
	  private bool decimalFloatingPointErrorReportingOptionSet_;
	  private string clientAccountingInformation_;
	  private string clientApplicationName_;
	  private string clientUserIdentifier_;
	  private string clientWorkstationName_;
	  private string clientProgramIdentifier_;
	  private string interfaceType_;
	  private string interfaceName_;
	  private string interfaceLevel_;
	  private int closeOnEOF_;
	  private bool closeOnEOFSet_;

	  // These are set based on attributes in the reply from the server, they cannot be set by a client.
	  private int serverCCSID_;
	  private bool serverCCSIDSet_;
	  private string serverFunctionalLevel_;
	  private string serverJobName_;
	  private string serverJobUser_;
	  private string serverJobNumber_;
	  private bool serverJobSet_;


	  public DatabaseServerAttributes()
	  {
	  }

	  public virtual void clear()
	  {
		defaultClientCCSID_ = 0;
		defaultClientCCSIDSet_ = false;
		languageFeatureCode_ = null;
		clientFunctionalLevel_ = null;
		nlssIdentifier_ = 0;
		nlssIdentifierSet_ = false;
		nlssIdentifierLanguageID_ = null;
		nlssIdentifierLanguageTableName_ = null;
		nlssIdentifierLanguageTableLibrary_ = null;
		translateIndicator_ = 0;
		translateIndicatorSet_ = false;
		drdaPackageSize_ = 0;
		drdaPackageSizeSet_ = false;
		dateFormatParserOption_ = 0;
		dateFormatParserOptionSet_ = false;
		dateSeparatorParserOption_ = 0;
		dateSeparatorParserOptionSet_ = false;
		timeFormatParserOption_ = 0;
		timeFormatParserOptionSet_ = false;
		timeSeparatorParserOption_ = 0;
		timeSeparatorParserOptionSet_ = false;
		decimalSeparatorParserOption_ = 0;
		decimalSeparatorParserOptionSet_ = false;
		namingConventionParserOption_ = 0;
		namingConventionParserOptionSet_ = false;
		ignoreDecimalDataErrorParserOption_ = 0;
		ignoreDecimalDataErrorParserOptionSet_ = false;
		commitmentControlLevelParserOption_ = 0;
		commitmentControlLevelParserOptionSet_ = false;
		defaultSQLLibraryName_ = null;
		asciiCCSIDForTranslationTable_ = 0;
		asciiCCSIDForTranslationTableSet_ = false;
		ambiguousSelectOption_ = 0;
		ambiguousSelectOptionSet_ = false;
	packageAddStatementAllowed_ = 0;
	packageAddStatementAllowedSet_ = false;
		useExtendedFormats_ = 0;
		useExtendedFormatsSet_ = false;
		lobFieldThreshold_ = 0;
		lobFieldThresholdSet_ = false;
		dataCompressionParameter_ = 0;
		dataCompressionParameterSet_ = false;
		trueAutoCommitIndicator_ = 0;
		trueAutoCommitIndicatorSet_ = false;
		clientSupportInformation_ = 0;
		clientSupportInformationSet_ = false;
		rdbName_ = null;
		maximumDecimalPrecision_ = 0;
		maximumDecimalScale_ = 0;
		minimumDivideScale_ = 0;
		decimalPrecisionAndScaleAttributesSet_ = false;
		hexadecimalConstantParserOption_ = 0;
		hexadecimalConstantParserOptionSet_ = false;
		inputLocatorType_ = 0;
		inputLocatorTypeSet_ = false;
		locatorPersistence_ = 0;
		locatorPersistenceSet_ = false;
		ewlmCorrelator_ = null;
		rleCompression_ = null;
		optimizationGoalIndicator_ = 0;
		optimizationGoalIndicatorSet_ = false;
		queryStorageLimit_ = 0;
		queryStorageLimitSet_ = false;
		decimalFloatingPointRoundingModeOption_ = 0;
		decimalFloatingPointRoundingModeOptionSet_ = false;
		decimalFloatingPointErrorReportingOption_ = 0;
		decimalFloatingPointErrorReportingOptionSet_ = false;
		clientAccountingInformation_ = null;
		clientApplicationName_ = null;
		clientUserIdentifier_ = null;
		clientWorkstationName_ = null;
		clientProgramIdentifier_ = null;
		interfaceType_ = null;
		interfaceName_ = null;
		interfaceLevel_ = null;
		closeOnEOF_ = 0;
		closeOnEOFSet_ = false;


		serverCCSID_ = 0;
		serverCCSIDSet_ = false;
		serverFunctionalLevel_ = null;
		serverJobName_ = null;
		serverJobUser_ = null;
		serverJobNumber_ = null;
		serverJobSet_ = false;
	  }

	  public virtual int DefaultClientCCSID
	  {
		  get
		  {
			return defaultClientCCSID_;
		  }
		  set
		  {
			defaultClientCCSID_ = value;
			defaultClientCCSIDSet_ = true;
		  }
	  }

	  public virtual bool DefaultClientCCSIDSet
	  {
		  get
		  {
			return defaultClientCCSIDSet_;
		  }
	  }


	  public virtual string LanguageFeatureCode
	  {
		  get
		  {
			return languageFeatureCode_;
		  }
		  set
		  {
			languageFeatureCode_ = value;
		  }
	  }

	  public virtual bool LanguageFeatureCodeSet
	  {
		  get
		  {
			return !string.ReferenceEquals(languageFeatureCode_, null);
		  }
	  }


	  public virtual string ClientFunctionalLevel
	  {
		  get
		  {
			return clientFunctionalLevel_;
		  }
		  set
		  {
			clientFunctionalLevel_ = value;
		  }
	  }

	  public virtual bool ClientFunctionalLevelSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientFunctionalLevel_, null);
		  }
	  }


	  public virtual int NLSSIdentifier
	  {
		  get
		  {
			return nlssIdentifier_;
		  }
		  set
		  {
			nlssIdentifier_ = value;
			nlssIdentifierSet_ = true;
		  }
	  }

	  public virtual bool NLSSIdentifierSet
	  {
		  get
		  {
			return nlssIdentifierSet_;
		  }
	  }


	  public virtual string NLSSIdentifierLanguageID
	  {
		  get
		  {
			return nlssIdentifierLanguageID_;
		  }
		  set
		  {
			nlssIdentifierLanguageID_ = value;
		  }
	  }


	  public virtual string NLSSIdentifierLanguageTableName
	  {
		  get
		  {
			return nlssIdentifierLanguageTableName_;
		  }
		  set
		  {
			nlssIdentifierLanguageTableName_ = value;
		  }
	  }


	  public virtual string NLSSIdentifierLanguageTableLibrary
	  {
		  get
		  {
			return nlssIdentifierLanguageTableLibrary_;
		  }
		  set
		  {
			nlssIdentifierLanguageTableLibrary_ = value;
		  }
	  }


	  public virtual int TranslateIndicator
	  {
		  get
		  {
			return translateIndicator_;
		  }
		  set
		  {
			translateIndicator_ = value;
			translateIndicatorSet_ = true;
		  }
	  }

	  public virtual bool TranslateIndicatorSet
	  {
		  get
		  {
			return translateIndicatorSet_;
		  }
	  }


	  public virtual int DRDAPackageSize
	  {
		  get
		  {
			return drdaPackageSize_;
		  }
		  set
		  {
			drdaPackageSize_ = value;
			drdaPackageSizeSet_ = true;
		  }
	  }

	  public virtual bool DRDAPackageSizeSet
	  {
		  get
		  {
			return drdaPackageSizeSet_;
		  }
	  }


	  public virtual int DateFormatParserOption
	  {
		  get
		  {
			return dateFormatParserOption_;
		  }
		  set
		  {
			dateFormatParserOption_ = value;
			dateFormatParserOptionSet_ = true;
		  }
	  }

	  public virtual bool DateFormatParserOptionSet
	  {
		  get
		  {
			return dateFormatParserOptionSet_;
		  }
	  }


	  public virtual int DateSeparatorParserOption
	  {
		  get
		  {
			return dateSeparatorParserOption_;
		  }
		  set
		  {
			dateSeparatorParserOption_ = value;
			dateSeparatorParserOptionSet_ = true;
		  }
	  }

	  public virtual bool DateSeparatorParserOptionSet
	  {
		  get
		  {
			return dateSeparatorParserOptionSet_;
		  }
	  }


	  public virtual int TimeFormatParserOption
	  {
		  get
		  {
			return timeFormatParserOption_;
		  }
		  set
		  {
			timeFormatParserOption_ = value;
			timeFormatParserOptionSet_ = true;
		  }
	  }

	  public virtual bool TimeFormatParserOptionSet
	  {
		  get
		  {
			return timeFormatParserOptionSet_;
		  }
	  }


	  public virtual int TimeSeparatorParserOption
	  {
		  get
		  {
			return timeSeparatorParserOption_;
		  }
		  set
		  {
			timeSeparatorParserOption_ = value;
			timeSeparatorParserOptionSet_ = true;
		  }
	  }

	  public virtual bool TimeSeparatorParserOptionSet
	  {
		  get
		  {
			return timeSeparatorParserOptionSet_;
		  }
	  }


	  public virtual int DecimalSeparatorParserOption
	  {
		  get
		  {
			return decimalSeparatorParserOption_;
		  }
		  set
		  {
			decimalSeparatorParserOption_ = value;
			decimalSeparatorParserOptionSet_ = true;
		  }
	  }

	  public virtual bool DecimalSeparatorParserOptionSet
	  {
		  get
		  {
			return decimalSeparatorParserOptionSet_;
		  }
	  }


	  public virtual int NamingConventionParserOption
	  {
		  get
		  {
			return namingConventionParserOption_;
		  }
		  set
		  {
			namingConventionParserOption_ = value;
			namingConventionParserOptionSet_ = true;
		  }
	  }

	  public virtual bool NamingConventionParserOptionSet
	  {
		  get
		  {
			return namingConventionParserOptionSet_;
		  }
	  }


	  public virtual int IgnoreDecimalDataErrorParserOption
	  {
		  get
		  {
			return ignoreDecimalDataErrorParserOption_;
		  }
		  set
		  {
			ignoreDecimalDataErrorParserOption_ = value;
			ignoreDecimalDataErrorParserOptionSet_ = true;
		  }
	  }

	  public virtual bool IgnoreDecimalDataErrorParserOptionSet
	  {
		  get
		  {
			return ignoreDecimalDataErrorParserOptionSet_;
		  }
	  }


	  public virtual int CommitmentControlLevelParserOption
	  {
		  get
		  {
			return commitmentControlLevelParserOption_;
		  }
		  set
		  {
			commitmentControlLevelParserOption_ = value;
			commitmentControlLevelParserOptionSet_ = true;
		  }
	  }

	  public virtual bool CommitmentControlLevelParserOptionSet
	  {
		  get
		  {
			return commitmentControlLevelParserOptionSet_;
		  }
	  }


	  public virtual string DefaultSQLLibraryName
	  {
		  get
		  {
			return defaultSQLLibraryName_;
		  }
		  set
		  {
			defaultSQLLibraryName_ = value;
		  }
	  }

	  public virtual bool DefaultSQLLibraryNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(defaultSQLLibraryName_, null);
		  }
	  }


	  public virtual int ASCIICCSIDForTranslationTable
	  {
		  get
		  {
			return asciiCCSIDForTranslationTable_;
		  }
		  set
		  {
			asciiCCSIDForTranslationTable_ = value;
			asciiCCSIDForTranslationTableSet_ = true;
		  }
	  }

	  public virtual bool ASCIICCSIDForTranslationTableSet
	  {
		  get
		  {
			return asciiCCSIDForTranslationTableSet_;
		  }
	  }


	  public virtual int AmbiguousSelectOption
	  {
		  get
		  {
			return ambiguousSelectOption_;
		  }
		  set
		  {
			ambiguousSelectOption_ = value;
			ambiguousSelectOptionSet_ = true;
		  }
	  }

	  public virtual bool AmbiguousSelectOptionSet
	  {
		  get
		  {
			return ambiguousSelectOptionSet_;
		  }
	  }


	  public virtual int PackageAddStatementAllowed
	  {
		  get
		  {
			return packageAddStatementAllowed_;
		  }
		  set
		  {
		packageAddStatementAllowed_ = value;
		packageAddStatementAllowedSet_ = true;
		  }
	  }

	  public virtual bool PackageAddStatementAllowedSet
	  {
		  get
		  {
			return packageAddStatementAllowedSet_;
		  }
	  }


	  public virtual int UseExtendedFormats
	  {
		  get
		  {
			return useExtendedFormats_;
		  }
		  set
		  {
			useExtendedFormats_ = value;
			useExtendedFormatsSet_ = true;
		  }
	  }

	  public virtual bool UseExtendedFormatsSet
	  {
		  get
		  {
			return useExtendedFormatsSet_;
		  }
	  }


	  public virtual int LOBFieldThreshold
	  {
		  get
		  {
			return lobFieldThreshold_;
		  }
		  set
		  {
			lobFieldThreshold_ = value;
			lobFieldThresholdSet_ = true;
		  }
	  }

	  public virtual bool LOBFieldThresholdSet
	  {
		  get
		  {
			return lobFieldThresholdSet_;
		  }
	  }


	  public virtual int DataCompressionParameter
	  {
		  get
		  {
			return dataCompressionParameter_;
		  }
		  set
		  {
			dataCompressionParameter_ = value;
			dataCompressionParameterSet_ = true;
		  }
	  }

	  public virtual bool DataCompressionParameterSet
	  {
		  get
		  {
			return dataCompressionParameterSet_;
		  }
	  }


	  public virtual int TrueAutoCommitIndicator
	  {
		  get
		  {
			return trueAutoCommitIndicator_;
		  }
		  set
		  {
			trueAutoCommitIndicator_ = value;
			trueAutoCommitIndicatorSet_ = true;
		  }
	  }

	  public virtual bool TrueAutoCommitIndicatorSet
	  {
		  get
		  {
			return trueAutoCommitIndicatorSet_;
		  }
	  }


	  public virtual int ClientSupportInformation
	  {
		  get
		  {
			return clientSupportInformation_;
		  }
		  set
		  {
			clientSupportInformation_ = value;
			clientSupportInformationSet_ = true;
		  }
	  }

	  public virtual bool ClientSupportInformationSet
	  {
		  get
		  {
			return clientSupportInformationSet_;
		  }
	  }


	  public virtual string RDBName
	  {
		  get
		  {
			return rdbName_;
		  }
		  set
		  {
			rdbName_ = value;
		  }
	  }

	  public virtual bool RDBNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(rdbName_, null);
		  }
	  }


	  public virtual int MaximumDecimalPrecision
	  {
		  get
		  {
			return maximumDecimalPrecision_;
		  }
	  }

	  public virtual int MaximumDecimalScale
	  {
		  get
		  {
			return maximumDecimalScale_;
		  }
	  }

	  public virtual int MinimumDivideScale
	  {
		  get
		  {
			return minimumDivideScale_;
		  }
	  }

	  public virtual bool DecimalPrecisionAndScaleAttributesSet
	  {
		  get
		  {
			return decimalPrecisionAndScaleAttributesSet_;
		  }
	  }

	  public virtual void setDecimalPrecisionAndScaleAttributes(int maximumDecimalPrecision, int maximumDecimalScale, int minimumDivideScale)
	  {
		maximumDecimalPrecision_ = maximumDecimalPrecision;
		maximumDecimalScale_ = maximumDecimalScale;
		minimumDivideScale_ = minimumDivideScale;
		decimalPrecisionAndScaleAttributesSet_ = true;
	  }

	  public virtual int HexadecimalConstantParserOption
	  {
		  get
		  {
			return hexadecimalConstantParserOption_;
		  }
		  set
		  {
			hexadecimalConstantParserOption_ = value;
			hexadecimalConstantParserOptionSet_ = true;
		  }
	  }

	  public virtual bool HexadecimalConstantParserOptionSet
	  {
		  get
		  {
			return hexadecimalConstantParserOptionSet_;
		  }
	  }


	  public virtual int InputLocatorType
	  {
		  get
		  {
			return inputLocatorType_;
		  }
		  set
		  {
			inputLocatorType_ = value;
			inputLocatorTypeSet_ = true;
		  }
	  }

	  public virtual bool InputLocatorTypeSet
	  {
		  get
		  {
			return inputLocatorTypeSet_;
		  }
	  }


	  public virtual int LocatorPersistence
	  {
		  get
		  {
			return locatorPersistence_;
		  }
		  set
		  {
			locatorPersistence_ = value;
			locatorPersistenceSet_ = true;
		  }
	  }

	  public virtual bool LocatorPersistenceSet
	  {
		  get
		  {
			return locatorPersistenceSet_;
		  }
	  }


	  public virtual sbyte[] EWLMCorrelator
	  {
		  get
		  {
			return ewlmCorrelator_;
		  }
		  set
		  {
			ewlmCorrelator_ = value;
		  }
	  }

	  public virtual bool EWLMCorrelatorSet
	  {
		  get
		  {
			return ewlmCorrelator_ != null;
		  }
	  }


	  public virtual sbyte[] RLECompression
	  {
		  get
		  {
			return rleCompression_;
		  }
		  set
		  {
			rleCompression_ = value;
		  }
	  }

	  public virtual bool RLECompressionSet
	  {
		  get
		  {
			return rleCompression_ != null;
		  }
	  }


	  public virtual int OptimizationGoalIndicator
	  {
		  get
		  {
			return optimizationGoalIndicator_;
		  }
		  set
		  {
			optimizationGoalIndicator_ = value;
			optimizationGoalIndicatorSet_ = true;
		  }
	  }

	  public virtual bool OptimizationGoalIndicatorSet
	  {
		  get
		  {
			return optimizationGoalIndicatorSet_;
		  }
	  }


	  public virtual int QueryStorageLimit
	  {
		  get
		  {
			return queryStorageLimit_;
		  }
		  set
		  {
			queryStorageLimit_ = value;
			queryStorageLimitSet_ = true;
		  }
	  }

	  public virtual bool QueryStorageLimitSet
	  {
		  get
		  {
			return queryStorageLimitSet_;
		  }
	  }


	  public virtual int DecimalFloatingPointRoundingModeOption
	  {
		  get
		  {
			return decimalFloatingPointRoundingModeOption_;
		  }
		  set
		  {
			decimalFloatingPointRoundingModeOption_ = value;
			decimalFloatingPointRoundingModeOptionSet_ = true;
		  }
	  }

	  public virtual bool DecimalFloatingPointRoundingModeOptionSet
	  {
		  get
		  {
			return decimalFloatingPointRoundingModeOptionSet_;
		  }
	  }


	  public virtual int DecimalFloatingPointErrorReportingOption
	  {
		  get
		  {
			return decimalFloatingPointErrorReportingOption_;
		  }
		  set
		  {
			decimalFloatingPointErrorReportingOption_ = value;
			decimalFloatingPointErrorReportingOptionSet_ = true;
		  }
	  }

	  public virtual bool DecimalFloatingPointErrorReportingOptionSet
	  {
		  get
		  {
			return decimalFloatingPointErrorReportingOptionSet_;
		  }
	  }


	  public virtual string ClientAccountingInformation
	  {
		  get
		  {
			return clientAccountingInformation_;
		  }
		  set
		  {
			clientAccountingInformation_ = value;
		  }
	  }

	  public virtual bool ClientAccountingInformationSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientAccountingInformation_, null);
		  }
	  }


	  public virtual string ClientApplicationName
	  {
		  get
		  {
			return clientApplicationName_;
		  }
		  set
		  {
			clientApplicationName_ = value;
		  }
	  }

	  public virtual bool ClientApplicationNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientApplicationName_, null);
		  }
	  }


	  public virtual string ClientUserIdentifier
	  {
		  get
		  {
			return clientUserIdentifier_;
		  }
		  set
		  {
			clientUserIdentifier_ = value;
		  }
	  }

	  public virtual bool ClientUserIdentifierSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientUserIdentifier_, null);
		  }
	  }


	  public virtual string ClientWorkstationName
	  {
		  get
		  {
			return clientWorkstationName_;
		  }
		  set
		  {
			clientWorkstationName_ = value;
		  }
	  }

	  public virtual bool ClientWorkstationNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientWorkstationName_, null);
		  }
	  }


	  public virtual string ClientProgramIdentifier
	  {
		  get
		  {
			return clientProgramIdentifier_;
		  }
		  set
		  {
			clientProgramIdentifier_ = value;
		  }
	  }

	  public virtual bool ClientProgramIdentifierSet
	  {
		  get
		  {
			return !string.ReferenceEquals(clientProgramIdentifier_, null);
		  }
	  }


	  public virtual string InterfaceType
	  {
		  get
		  {
			return interfaceType_;
		  }
		  set
		  {
			interfaceType_ = value;
		  }
	  }

	  public virtual bool InterfaceTypeSet
	  {
		  get
		  {
			return !string.ReferenceEquals(interfaceType_, null);
		  }
	  }


	  public virtual string InterfaceName
	  {
		  get
		  {
			return interfaceName_;
		  }
		  set
		  {
			interfaceName_ = value;
		  }
	  }

	  public virtual bool InterfaceNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(interfaceName_, null);
		  }
	  }


	  public virtual string InterfaceLevel
	  {
		  get
		  {
			return interfaceLevel_;
		  }
		  set
		  {
			interfaceLevel_ = value;
		  }
	  }

	  public virtual bool InterfaceLevelSet
	  {
		  get
		  {
			return !string.ReferenceEquals(interfaceLevel_, null);
		  }
	  }


	  public virtual int CloseOnEOF
	  {
		  get
		  {
			return closeOnEOF_;
		  }
		  set
		  {
			closeOnEOF_ = value;
			closeOnEOFSet_ = true;
		  }
	  }

	  public virtual bool CloseOnEOFSet
	  {
		  get
		  {
			return closeOnEOFSet_;
		  }
	  }


	  public virtual int ServerCCSID
	  {
		  get
		  {
			return serverCCSID_;
		  }
		  set
		  {
			serverCCSID_ = value;
			serverCCSIDSet_ = true;
		  }
	  }

	  public virtual bool ServerCCSIDSet
	  {
		  get
		  {
			return serverCCSIDSet_;
		  }
	  }


	  public virtual string ServerFunctionalLevel
	  {
		  get
		  {
			return serverFunctionalLevel_;
		  }
		  set
		  {
			serverFunctionalLevel_ = value;
		  }
	  }

	  public virtual bool ServerFunctionalLevelSet
	  {
		  get
		  {
			return !string.ReferenceEquals(serverFunctionalLevel_, null);
		  }
	  }


	  public virtual string ServerJobName
	  {
		  get
		  {
			return serverJobName_;
		  }
	  }

	  public virtual string ServerJobUser
	  {
		  get
		  {
			return serverJobUser_;
		  }
	  }

	  public virtual string ServerJobNumber
	  {
		  get
		  {
			return serverJobNumber_;
		  }
	  }

	  public virtual bool ServerJobSet
	  {
		  get
		  {
			return serverJobSet_;
		  }
	  }

	  internal virtual void setServerJob(string jobName, string jobUser, string jobNumber)
	  {
		serverJobName_ = jobName;
		serverJobUser_ = jobUser;
		serverJobNumber_ = jobNumber;
		serverJobSet_ = true;
	  }
	}

}