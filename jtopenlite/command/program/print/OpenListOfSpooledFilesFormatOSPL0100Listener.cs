///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFormatOSPL0100Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{
	public interface OpenListOfSpooledFilesFormatOSPL0100Listener : OpenListOfSpooledFilesFormatListener
	{
	  void newSpooledFileEntry(string spooledFileName, string jobName, string jobUser, string jobNumber, int spooledFileNumber, int totalPages, int currentPage, int copiesLeftToPrint, string outputQueueName, string outputQueueLibrary, string userData, string status, string formType, string priority, sbyte[] internalJobIdentifier, sbyte[] internalSpooledFileIdentifier, string deviceType, string jobSystemName, string dateOpened, string timeOpened);
	}

}