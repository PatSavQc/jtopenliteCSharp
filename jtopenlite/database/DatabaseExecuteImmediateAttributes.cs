﻿///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseExecuteImmediateAttributes.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public interface DatabaseExecuteImmediateAttributes : AttributeSQLStatementText, AttributeCursorName, AttributeSQLStatementType, AttributeOpenAttributes, AttributeDescribeOption, AttributeBlockingFactor, AttributeScrollableCursorFlag, AttributeSQLParameterMarkerData, AttributeSQLExtendedParameterMarkerData, AttributeSQLParameterMarkerBlockIndicator, AttributeTranslateIndicator, AttributeRLECompressedFunctionParameters, AttributeExtendedSQLStatementText, AttributePrepareOption
	{
	}

}