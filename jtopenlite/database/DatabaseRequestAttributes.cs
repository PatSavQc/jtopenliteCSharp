///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseRequestAttributes.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public class DatabaseRequestAttributes : DatabaseCloseCursorAttributes, DatabaseFetchAttributes, DatabaseCreateRequestParameterBlockAttributes, DatabaseDeleteRequestParameterBlockAttributes, DatabasePackageAttributes, DatabaseDescribeAttributes, DatabaseDescribeParameterMarkerAttributes, DatabaseRetrievePackageAttributes, DatabaseExecuteAttributes, DatabaseExecuteImmediateAttributes, DatabaseExecuteOrOpenAndDescribeAttributes, DatabaseOpenAndDescribeAttributes, DatabaseOpenDescribeFetchAttributes, DatabasePrepareAttributes, DatabasePrepareAndDescribeAttributes, DatabasePrepareAndExecuteAttributes, DatabaseChangeDescriptorAttributes, DatabaseDeleteDescriptorAttributes, DatabaseStreamFetchAttributes, DatabaseEndStreamFetchAttributes, DatabaseRetrieveLOBDataAttributes
	{
	  private string cursorName_;
	  private int reuseIndicator_;
	  private bool reuseIndicatorSet_;
	  private int translateIndicator_;
	  private bool translateIndicatorSet_;
	  private sbyte[] rleCompressedFunctionParameters_;
	  private long blockingFactor_;
	  private bool blockingFactorSet_;
	  private int fetchScrollOption_;
	  private int fetchScrollOptionRelativeValue_;
	  private bool fetchScrollOptionSet_;
	  private long fetchBufferSize_;
	  private bool fetchBufferSizeSet_;

	  private string packageLibrary_;
	  private string packageName_;
	  private string prepareStatementName_;
	  private string sqlStatementText_;
	  private int prepareOption_;
	  private bool prepareOptionSet_;
	  private int openAttributes_;
	  private bool openAttributesSet_;
	  private int describeOption_;
	  private bool describeOptionSet_;
	  private int scrollableCursorFlag_;
	  private bool scrollableCursorFlagSet_;
	  private int holdIndicator_;
	  private bool holdIndicatorSet_;
	  private int sqlStatementType_;
	  private bool sqlStatementTypeSet_;
	  private int sqlParameterMarkerBlockIndicator_;
	  private bool sqlParameterMarkerBlockIndicatorSet_;
	  private int queryTimeoutLimit_;
	  private bool queryTimeoutLimitSet_;
	  private int serverSideStaticCursorResultSetSize_;
	  private bool serverSideStaticCursorResultSetSizeSet_;
	  private int extendedColumnDescriptorOption_;
	  private bool extendedColumnDescriptorOptionSet_;
	  private int resultSetHoldabilityOption_;
	  private bool resultSetHoldabilityOptionSet_;
	  private string extendedSQLStatementText_;
	  private int variableFieldCompression_;
	  private bool variableFieldCompressionSet_;
	  private int returnOptimisticLockingColumns_;
	  private bool returnOptimisticLockingColumnsSet_;

	  // Return size is supposed to be a 4-byte unsigned number, which would require a long,
	  // but the LIPI says it only contains the values 0 through 15.
	  private int returnSize_;
	  private bool returnSizeSet_;

	  private sbyte[] sqlParameterMarkerData_;
	  private sbyte[] sqlExtendedParameterMarkerData_;

	  private sbyte[] sqlParameterMarkerDataFormat_;
	  private sbyte[] extendedSQLParameterMarkerDataFormat_;

	  private int syncPointCount_;
	  private bool syncPointCountSet_;

	  private int lobLocatorHandle_;
	  private bool lobLocatorHandleSet_;
	  private int requestedSize_;
	  private bool requestedSizeSet_;
	  private int startOffset_;
	  private bool startOffsetSet_;
	  private int compressionIndicator_;
	  private bool compressionIndicatorSet_;
	  private int returnCurrentLengthIndicator_;
	  private bool returnCurrentLengthIndicatorSet_;
	  private int columnIndex_;
	  private bool columnIndexSet_;


	  public DatabaseRequestAttributes()
	  {
	  }

	  public virtual void clear()
	  {
		cursorName_ = null;
		reuseIndicator_ = 0;
		reuseIndicatorSet_ = false;
		translateIndicator_ = 0;
		translateIndicatorSet_ = false;
		rleCompressedFunctionParameters_ = null;
		blockingFactor_ = 0;
		blockingFactorSet_ = false;
		fetchScrollOption_ = 0;
		fetchScrollOptionRelativeValue_ = 0;
		fetchScrollOptionSet_ = false;
		fetchBufferSize_ = 0;
		fetchBufferSizeSet_ = false;

	packageLibrary_ = null;
	packageName_ = null;
		prepareStatementName_ = null;
		sqlStatementText_ = null;
		prepareOption_ = 0;
		prepareOptionSet_ = false;
		openAttributes_ = 0;
		openAttributesSet_ = false;
		describeOption_ = 0;
		describeOptionSet_ = false;
		scrollableCursorFlag_ = 0;
		scrollableCursorFlagSet_ = false;
		holdIndicator_ = 0;
		holdIndicatorSet_ = false;
		sqlStatementType_ = 0;
		sqlStatementTypeSet_ = false;
		sqlParameterMarkerBlockIndicator_ = 0;
		sqlParameterMarkerBlockIndicatorSet_ = false;
		queryTimeoutLimit_ = 0;
		queryTimeoutLimitSet_ = false;
		serverSideStaticCursorResultSetSize_ = 0;
		serverSideStaticCursorResultSetSizeSet_ = false;
		extendedColumnDescriptorOption_ = 0;
		extendedColumnDescriptorOptionSet_ = false;
		resultSetHoldabilityOption_ = 0;
		resultSetHoldabilityOptionSet_ = false;
		extendedSQLStatementText_ = null;
		variableFieldCompression_ = 0;
		variableFieldCompressionSet_ = false;
		returnOptimisticLockingColumns_ = 0;
		returnOptimisticLockingColumnsSet_ = false;

		returnSize_ = 0;
		returnSizeSet_ = false;

		sqlParameterMarkerData_ = null;
		sqlExtendedParameterMarkerData_ = null;

		sqlParameterMarkerDataFormat_ = null;
		extendedSQLParameterMarkerDataFormat_ = null;

		syncPointCount_ = 0;
		syncPointCountSet_ = false;

		lobLocatorHandle_ = 0;
		lobLocatorHandleSet_ = false;
		requestedSize_ = 0;
		requestedSizeSet_ = false;
		startOffset_ = 0;
		startOffsetSet_ = false;
		compressionIndicator_ = 0;
		compressionIndicatorSet_ = false;
		returnCurrentLengthIndicator_ = 0;
		returnCurrentLengthIndicatorSet_ = false;
		columnIndex_ = 0;
		columnIndexSet_ = false;
	  }

	  public virtual DatabaseRequestAttributes copy()
	  {
		  DatabaseRequestAttributes newCopy = new DatabaseRequestAttributes();
		newCopy.cursorName_ = cursorName_;
		newCopy.reuseIndicator_ = reuseIndicator_;
		newCopy.reuseIndicatorSet_ = reuseIndicatorSet_;
		newCopy.translateIndicator_ = translateIndicator_;
		newCopy.translateIndicatorSet_ = translateIndicatorSet_;
		newCopy.rleCompressedFunctionParameters_ = rleCompressedFunctionParameters_;
		newCopy.blockingFactor_ = blockingFactor_;
		newCopy.blockingFactorSet_ = blockingFactorSet_;
		newCopy.fetchScrollOption_ = fetchScrollOption_;
		newCopy.fetchScrollOptionRelativeValue_ = fetchScrollOptionRelativeValue_;
		newCopy.fetchScrollOptionSet_ = fetchScrollOptionSet_;
		newCopy.fetchBufferSize_ = fetchBufferSize_;
		newCopy.fetchBufferSizeSet_ = fetchBufferSizeSet_;

		newCopy.packageLibrary_ = packageLibrary_;
		newCopy.packageName_ = packageName_;
		newCopy.prepareStatementName_ = prepareStatementName_;
		newCopy.sqlStatementText_ = sqlStatementText_;
		newCopy.prepareOption_ = prepareOption_;
		newCopy.prepareOptionSet_ = prepareOptionSet_;
		newCopy.openAttributes_ = openAttributes_;
		newCopy.openAttributesSet_ = openAttributesSet_;
		newCopy.describeOption_ = describeOption_;
		newCopy.describeOptionSet_ = describeOptionSet_;
		newCopy.scrollableCursorFlag_ = scrollableCursorFlag_;
		newCopy.scrollableCursorFlagSet_ = scrollableCursorFlagSet_;
		newCopy.holdIndicator_ = holdIndicator_;
		newCopy.holdIndicatorSet_ = holdIndicatorSet_;
		newCopy.sqlStatementType_ = sqlStatementType_;
		newCopy.sqlStatementTypeSet_ = sqlStatementTypeSet_;
		newCopy.sqlParameterMarkerBlockIndicator_ = sqlParameterMarkerBlockIndicator_;
		newCopy.sqlParameterMarkerBlockIndicatorSet_ = sqlParameterMarkerBlockIndicatorSet_;
		newCopy.queryTimeoutLimit_ = queryTimeoutLimit_;
		newCopy.queryTimeoutLimitSet_ = queryTimeoutLimitSet_;
		newCopy.serverSideStaticCursorResultSetSize_ = serverSideStaticCursorResultSetSize_;
		newCopy.serverSideStaticCursorResultSetSizeSet_ = serverSideStaticCursorResultSetSizeSet_;
		newCopy.extendedColumnDescriptorOption_ = extendedColumnDescriptorOption_;
		newCopy.extendedColumnDescriptorOptionSet_ = extendedColumnDescriptorOptionSet_;
		newCopy.resultSetHoldabilityOption_ = resultSetHoldabilityOption_;
		newCopy.resultSetHoldabilityOptionSet_ = resultSetHoldabilityOptionSet_;
		newCopy.extendedSQLStatementText_ = extendedSQLStatementText_;
		newCopy.variableFieldCompression_ = variableFieldCompression_;
		newCopy.variableFieldCompressionSet_ = variableFieldCompressionSet_;
		newCopy.returnOptimisticLockingColumns_ = returnOptimisticLockingColumns_;
		newCopy.returnOptimisticLockingColumnsSet_ = returnOptimisticLockingColumnsSet_;

		newCopy.returnSize_ = returnSize_;
		newCopy.returnSizeSet_ = returnSizeSet_;

		newCopy.sqlParameterMarkerData_ = sqlParameterMarkerData_;
		newCopy.sqlExtendedParameterMarkerData_ = sqlExtendedParameterMarkerData_;

		newCopy.sqlParameterMarkerDataFormat_ = sqlParameterMarkerDataFormat_;
		newCopy.extendedSQLParameterMarkerDataFormat_ = extendedSQLParameterMarkerDataFormat_;

		newCopy.syncPointCount_ = syncPointCount_;
		newCopy.syncPointCountSet_ = syncPointCountSet_;

		newCopy.lobLocatorHandle_ = lobLocatorHandle_;
		newCopy.lobLocatorHandleSet_ = lobLocatorHandleSet_;
		newCopy.requestedSize_ = requestedSize_;
		newCopy.requestedSizeSet_ = requestedSizeSet_;
		newCopy.startOffset_ = startOffset_;
		newCopy.startOffsetSet_ = startOffsetSet_;
		newCopy.compressionIndicator_ = compressionIndicator_;
		newCopy.compressionIndicatorSet_ = compressionIndicatorSet_;
		newCopy.returnCurrentLengthIndicator_ = returnCurrentLengthIndicator_;
		newCopy.returnCurrentLengthIndicatorSet_ = returnCurrentLengthIndicatorSet_;
		newCopy.columnIndex_ = columnIndex_;
		newCopy.columnIndexSet_ = columnIndexSet_;
		return newCopy;
	  }

	  public virtual string CursorName
	  {
		  get
		  {
			return cursorName_;
		  }
		  set
		  {
			cursorName_ = value;
		  }
	  }

	  public virtual bool CursorNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(cursorName_, null);
		  }
	  }


	  public virtual int ReuseIndicator
	  {
		  get
		  {
			return reuseIndicator_;
		  }
		  set
		  {
			reuseIndicator_ = value;
			reuseIndicatorSet_ = true;
		  }
	  }

	  public virtual bool ReuseIndicatorSet
	  {
		  get
		  {
			return reuseIndicatorSet_;
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


	  public virtual sbyte[] RLECompressedFunctionParameters
	  {
		  get
		  {
			return rleCompressedFunctionParameters_;
		  }
		  set
		  {
			rleCompressedFunctionParameters_ = value;
		  }
	  }

	  public virtual bool RLECompressedFunctionParametersSet
	  {
		  get
		  {
			return rleCompressedFunctionParameters_ != null;
		  }
	  }


	  public virtual long BlockingFactor
	  {
		  get
		  {
			return blockingFactor_;
		  }
		  set
		  {
			blockingFactor_ = value;
			blockingFactorSet_ = true;
		  }
	  }

	  public virtual bool BlockingFactorSet
	  {
		  get
		  {
			return blockingFactorSet_;
		  }
	  }


	  public virtual int FetchScrollOption
	  {
		  get
		  {
			return fetchScrollOption_;
		  }
	  }

	  public virtual int FetchScrollOptionRelativeValue
	  {
		  get
		  {
			return fetchScrollOptionRelativeValue_;
		  }
	  }

	  public virtual bool FetchScrollOptionSet
	  {
		  get
		  {
			return fetchScrollOptionSet_;
		  }
	  }

	  public virtual void setFetchScrollOption(int fetchScrollOption, int fetchScrollOptionRelativeValue)
	  {
		fetchScrollOption_ = fetchScrollOption;
		fetchScrollOptionRelativeValue_ = fetchScrollOptionRelativeValue;
		fetchScrollOptionSet_ = true;
	  }

	  public virtual long FetchBufferSize
	  {
		  get
		  {
			return fetchBufferSize_;
		  }
		  set
		  {
			fetchBufferSize_ = value;
			fetchBufferSizeSet_ = true;
		  }
	  }

	  public virtual bool FetchBufferSizeSet
	  {
		  get
		  {
			return fetchBufferSizeSet_;
		  }
	  }


	  public virtual string PackageLibrary
	  {
		  get
		  {
			return packageLibrary_;
		  }
		  set
		  {
		packageLibrary_ = value;
		  }
	  }

	  public virtual bool PackageLibrarySet
	  {
		  get
		  {
			return !string.ReferenceEquals(packageLibrary_, null);
		  }
	  }


	  public virtual string PackageName
	  {
		  get
		  {
			return packageName_;
		  }
		  set
		  {
		packageName_ = value;
		  }
	  }

	  public virtual bool PackageNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(packageName_, null);
		  }
	  }


	  public virtual string PrepareStatementName
	  {
		  get
		  {
			return prepareStatementName_;
		  }
		  set
		  {
			prepareStatementName_ = value;
		  }
	  }

	  public virtual bool PrepareStatementNameSet
	  {
		  get
		  {
			return !string.ReferenceEquals(prepareStatementName_, null);
		  }
	  }


	  public virtual string SQLStatementText
	  {
		  get
		  {
			return sqlStatementText_;
		  }
		  set
		  {
			sqlStatementText_ = value;
		  }
	  }

	  public virtual bool SQLStatementTextSet
	  {
		  get
		  {
			return !string.ReferenceEquals(sqlStatementText_, null);
		  }
	  }


	  public virtual int PrepareOption
	  {
		  get
		  {
			return prepareOption_;
		  }
		  set
		  {
			prepareOption_ = value;
			prepareOptionSet_ = true;
		  }
	  }

	  public virtual bool PrepareOptionSet
	  {
		  get
		  {
			return prepareOptionSet_;
		  }
	  }


	  public virtual int OpenAttributes
	  {
		  get
		  {
			return openAttributes_;
		  }
		  set
		  {
			openAttributes_ = value;
			openAttributesSet_ = true;
		  }
	  }

	  public virtual bool OpenAttributesSet
	  {
		  get
		  {
			return openAttributesSet_;
		  }
	  }


	  public virtual int DescribeOption
	  {
		  get
		  {
			return describeOption_;
		  }
		  set
		  {
			describeOption_ = value;
			describeOptionSet_ = true;
		  }
	  }

	  public virtual bool DescribeOptionSet
	  {
		  get
		  {
			return describeOptionSet_;
		  }
	  }


	  public virtual int ScrollableCursorFlag
	  {
		  get
		  {
			return scrollableCursorFlag_;
		  }
		  set
		  {
			scrollableCursorFlag_ = value;
			scrollableCursorFlagSet_ = true;
		  }
	  }

	  public virtual bool ScrollableCursorFlagSet
	  {
		  get
		  {
			return scrollableCursorFlagSet_;
		  }
	  }


	  public virtual int HoldIndicator
	  {
		  get
		  {
			return holdIndicator_;
		  }
		  set
		  {
			holdIndicator_ = value;
			holdIndicatorSet_ = true;
		  }
	  }

	  public virtual bool HoldIndicatorSet
	  {
		  get
		  {
			return holdIndicatorSet_;
		  }
	  }


	  public virtual int SQLStatementType
	  {
		  get
		  {
			return sqlStatementType_;
		  }
		  set
		  {
			sqlStatementType_ = value;
			sqlStatementTypeSet_ = true;
		  }
	  }

	  public virtual bool SQLStatementTypeSet
	  {
		  get
		  {
			return sqlStatementTypeSet_;
		  }
	  }


	  public virtual int SQLParameterMarkerBlockIndicator
	  {
		  get
		  {
			return sqlParameterMarkerBlockIndicator_;
		  }
		  set
		  {
			sqlParameterMarkerBlockIndicator_ = value;
			sqlParameterMarkerBlockIndicatorSet_ = true;
		  }
	  }

	  public virtual bool SQLParameterMarkerBlockIndicatorSet
	  {
		  get
		  {
			return sqlParameterMarkerBlockIndicatorSet_;
		  }
	  }


	  public virtual int QueryTimeoutLimit
	  {
		  get
		  {
			return queryTimeoutLimit_;
		  }
		  set
		  {
			queryTimeoutLimit_ = value;
			queryTimeoutLimitSet_ = true;
		  }
	  }

	  public virtual bool QueryTimeoutLimitSet
	  {
		  get
		  {
			return queryTimeoutLimitSet_;
		  }
	  }


	  public virtual int ServerSideStaticCursorResultSetSize
	  {
		  get
		  {
			return serverSideStaticCursorResultSetSize_;
		  }
		  set
		  {
			serverSideStaticCursorResultSetSize_ = value;
			serverSideStaticCursorResultSetSizeSet_ = true;
		  }
	  }

	  public virtual bool ServerSideStaticCursorResultSetSizeSet
	  {
		  get
		  {
			return serverSideStaticCursorResultSetSizeSet_;
		  }
	  }


	  public virtual int ExtendedColumnDescriptorOption
	  {
		  get
		  {
			return extendedColumnDescriptorOption_;
		  }
		  set
		  {
			extendedColumnDescriptorOption_ = value;
			extendedColumnDescriptorOptionSet_ = true;
		  }
	  }

	  public virtual bool ExtendedColumnDescriptorOptionSet
	  {
		  get
		  {
			return extendedColumnDescriptorOptionSet_;
		  }
	  }


	  public virtual int ResultSetHoldabilityOption
	  {
		  get
		  {
			return resultSetHoldabilityOption_;
		  }
		  set
		  {
			resultSetHoldabilityOption_ = value;
			resultSetHoldabilityOptionSet_ = true;
		  }
	  }

	  public virtual bool ResultSetHoldabilityOptionSet
	  {
		  get
		  {
			return resultSetHoldabilityOptionSet_;
		  }
	  }


	  public virtual string ExtendedSQLStatementText
	  {
		  get
		  {
			return extendedSQLStatementText_;
		  }
		  set
		  {
			extendedSQLStatementText_ = value;
		  }
	  }

	  public virtual bool ExtendedSQLStatementTextSet
	  {
		  get
		  {
			return !string.ReferenceEquals(extendedSQLStatementText_, null);
		  }
	  }


	  public virtual int VariableFieldCompression
	  {
		  get
		  {
			return variableFieldCompression_;
		  }
		  set
		  {
			variableFieldCompression_ = value;
			variableFieldCompressionSet_ = true;
		  }
	  }

	  public virtual bool VariableFieldCompressionSet
	  {
		  get
		  {
			return variableFieldCompressionSet_;
		  }
	  }


	  public virtual int ReturnOptimisticLockingColumns
	  {
		  get
		  {
			return returnOptimisticLockingColumns_;
		  }
		  set
		  {
			returnOptimisticLockingColumns_ = value;
			returnOptimisticLockingColumnsSet_ = true;
		  }
	  }

	  public virtual bool ReturnOptimisticLockingColumnsSet
	  {
		  get
		  {
			return returnOptimisticLockingColumnsSet_;
		  }
	  }


	  public virtual int ReturnSize
	  {
		  get
		  {
			return returnSize_;
		  }
		  set
		  {
			returnSize_ = value;
			returnSizeSet_ = true;
		  }
	  }

	  public virtual bool ReturnSizeSet
	  {
		  get
		  {
			return returnSizeSet_;
		  }
	  }


	  public virtual sbyte[] SQLParameterMarkerData
	  {
		  get
		  {
			return sqlParameterMarkerData_;
		  }
		  set
		  {
			sqlParameterMarkerData_ = value;
		  }
	  }

	  public virtual bool SQLParameterMarkerDataSet
	  {
		  get
		  {
			return sqlParameterMarkerData_ != null;
		  }
	  }


	  public virtual sbyte[] SQLExtendedParameterMarkerData
	  {
		  get
		  {
			return sqlExtendedParameterMarkerData_;
		  }
		  set
		  {
			sqlExtendedParameterMarkerData_ = value;
		  }
	  }

	  public virtual bool SQLExtendedParameterMarkerDataSet
	  {
		  get
		  {
			return sqlExtendedParameterMarkerData_ != null;
		  }
	  }


	  public virtual sbyte[] SQLParameterMarkerDataFormat
	  {
		  get
		  {
			return sqlParameterMarkerDataFormat_;
		  }
		  set
		  {
			sqlParameterMarkerDataFormat_ = value;
		  }
	  }

	  public virtual bool SQLParameterMarkerDataFormatSet
	  {
		  get
		  {
			return sqlParameterMarkerDataFormat_ != null;
		  }
	  }


	  public virtual sbyte[] ExtendedSQLParameterMarkerDataFormat
	  {
		  get
		  {
			return extendedSQLParameterMarkerDataFormat_;
		  }
		  set
		  {
			extendedSQLParameterMarkerDataFormat_ = value;
		  }
	  }

	  public virtual bool ExtendedSQLParameterMarkerDataFormatSet
	  {
		  get
		  {
			return extendedSQLParameterMarkerDataFormat_ != null;
		  }
	  }


	  public virtual int SyncPointCount
	  {
		  get
		  {
			return syncPointCount_;
		  }
		  set
		  {
			syncPointCount_ = value;
			syncPointCountSet_ = true;
		  }
	  }

	  public virtual bool SyncPointCountSet
	  {
		  get
		  {
			return syncPointCountSet_;
		  }
	  }


	  public virtual int LOBLocatorHandle
	  {
		  get
		  {
			return lobLocatorHandle_;
		  }
		  set
		  {
			lobLocatorHandle_ = value;
			lobLocatorHandleSet_ = true;
		  }
	  }

	  public virtual bool LOBLocatorHandleSet
	  {
		  get
		  {
			return lobLocatorHandleSet_;
		  }
	  }


	  public virtual int RequestedSize
	  {
		  get
		  {
			return requestedSize_;
		  }
		  set
		  {
			requestedSize_ = value;
			requestedSizeSet_ = true;
		  }
	  }

	  public virtual bool RequestedSizeSet
	  {
		  get
		  {
			return requestedSizeSet_;
		  }
	  }


	  public virtual int StartOffset
	  {
		  get
		  {
			return startOffset_;
		  }
		  set
		  {
			startOffset_ = value;
			startOffsetSet_ = true;
		  }
	  }

	  public virtual bool StartOffsetSet
	  {
		  get
		  {
			return startOffsetSet_;
		  }
	  }


	  public virtual int CompressionIndicator
	  {
		  get
		  {
			return compressionIndicator_;
		  }
		  set
		  {
			compressionIndicator_ = value;
			compressionIndicatorSet_ = true;
		  }
	  }

	  public virtual bool CompressionIndicatorSet
	  {
		  get
		  {
			return compressionIndicatorSet_;
		  }
	  }


	  public virtual int ReturnCurrentLengthIndicator
	  {
		  get
		  {
			return returnCurrentLengthIndicator_;
		  }
		  set
		  {
			returnCurrentLengthIndicator_ = value;
			returnCurrentLengthIndicatorSet_ = true;
		  }
	  }

	  public virtual bool ReturnCurrentLengthIndicatorSet
	  {
		  get
		  {
			return returnCurrentLengthIndicatorSet_;
		  }
	  }


	  public virtual int ColumnIndex
	  {
		  get
		  {
			return columnIndex_;
		  }
		  set
		  {
			columnIndex_ = value;
			columnIndexSet_ = true;
		  }
	  }

	  public virtual bool ColumnIndexSet
	  {
		  get
		  {
			return columnIndexSet_;
		  }
	  }

	}

}