///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsFormatOLJB0100Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{

	public interface OpenListOfJobsFormatOLJB0100Listener : OpenListOfJobsFormatListener
	{
	  void newJobEntry(string jobNameUsed, string userNameUsed, string jobNumberUsed, sbyte[] internalJobIdentifier, string status, string jobType, string jobSubtype);
	}

}