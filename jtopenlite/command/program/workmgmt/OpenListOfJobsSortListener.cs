///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsSortListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public interface OpenListOfJobsSortListener
	{
	  int NumberOfSortKeys {get;}

	  int getSortKeyFieldStartingPosition(int keyIndex);

	  int getSortKeyFieldLength(int keyIndex);

	  int getSortKeyFieldDataType(int keyIndex);

	  bool isAscending(int keyIndex);
	}

}