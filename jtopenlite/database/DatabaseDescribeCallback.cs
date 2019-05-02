///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseDescribeCallback.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public interface DatabaseDescribeCallback
	{
	  void resultSetDescription(int numFields, int dateFormat, int timeFormat, int dateSeparator, int timeSeparator, int recordSize);

	  void fieldDescription(int fieldIndex, int type, int length, int scale, int precision, int ccsid, int joinRefPosition, int attributeBitmap, int lobMaxSize);

	  void fieldName(int fieldIndex, string name);

	  void udtName(int fieldIndex, string name);

	  void baseColumnName(int fieldIndex, string name);

	  void baseTableName(int fieldIndex, string name);

	  void columnLabel(int fieldIndex, string name);

	  void baseSchemaName(int fieldIndex, string name);

	  void sqlFromTable(int fieldIndex, string name);

	  void sqlFromSchema(int fieldIndex, string name);

	  void columnAttributes(int fieldIndex, int updateable, int searchable, bool isIdentity, bool isAlwaysGenerated, bool isPartOfAnyIndex, bool isLoneUniqueIndex, bool isPartOfUniqueIndex, bool isExpression, bool isPrimaryKey, bool isNamed, bool isRowID, bool isRowChangeTimestamp);


	}

}