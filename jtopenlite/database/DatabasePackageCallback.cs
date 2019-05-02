///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabasePackageCallback.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public interface DatabasePackageCallback
	{
	  void newPackageInfo(int ccsid, string defaultCollection, int numStatements);

	  void newStatementInfo(int statementIndex, int needsDefaultCollection, int type, string name);

	  void statementText(int statementIndex, string text);

	  void statementDataFormat(int statementIndex, sbyte[] format);

	  void statementParameterMarkerFormat(int statementIndex, sbyte[] format);
	}

}