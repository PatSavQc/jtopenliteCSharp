﻿///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsFormatOLJB0300Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{

	public interface OpenListOfJobsFormatOLJB0300Listener : JobKeyDataListener, OpenListOfJobsFormatListener
	{
	  void newJobEntry(string jobNameUsed, string userNameUsed, string jobNumberUsed, string activeJobStatus, string jobType, string jobSubtype);
	}

}