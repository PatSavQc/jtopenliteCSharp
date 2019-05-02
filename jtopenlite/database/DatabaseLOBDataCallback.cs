///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseLOBDataCallback.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	public interface DatabaseLOBDataCallback
	{
	  void newLOBLength(long length);

	  void newLOBData(int ccsid, int length);

	  sbyte[] LOBBuffer {get;set;}


	  void newLOBSegment(sbyte[] buffer, int offset, int length);
	}

}