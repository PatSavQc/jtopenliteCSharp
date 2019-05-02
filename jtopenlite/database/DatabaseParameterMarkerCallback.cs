///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseParameterMarkerCallback.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public interface DatabaseParameterMarkerCallback
	{
	  void parameterMarkerDescription(int numFields, int recordSize);

	  void parameterMarkerFieldDescription(int fieldIndex, int fieldType, int length, int scale, int precision, int ccsid, int parameterType, int joinRefPosition, int lobLocator, int lobMaxSize);

	  void parameterMarkerFieldName(int fieldIndex, string name);

	  void parameterMarkerUDTName(int fieldIndex, string name);
	}

}