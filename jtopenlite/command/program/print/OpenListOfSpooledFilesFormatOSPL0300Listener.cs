///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFormatOSPL0300Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{

	public interface OpenListOfSpooledFilesFormatOSPL0300Listener : OpenListOfSpooledFilesFormatListener
	{
	  void newSpooledFileEntry(string jobName, string jobUser, string jobNumber, string spooledFileName, int spooledFileNumber, int fileStatus, string dateOpened, string timeOpened, string spooledFileSchedule, string jobSystemName, string userData, string formType, string outputQueueName, string outputQueueLibrary, int auxiliaryStoragePool, long size, int totalPages, int copiesLeftToPrint, string priority, int internetPrintProtocolJobIdentifier);
	}

}