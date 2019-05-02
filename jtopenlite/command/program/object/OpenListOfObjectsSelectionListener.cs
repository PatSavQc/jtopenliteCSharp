///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  OpenListOfObjectsSelectionListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.@object
{
	public interface OpenListOfObjectsSelectionListener
	{
	  /// <summary>
	  /// Whether the statuses are for selecting objects or omitting objects from the list.
	  /// 
	  /// </summary>
	  bool Selected {get;}

	  int NumberOfStatuses {get;}

	  string getStatus(int index);
	}


}