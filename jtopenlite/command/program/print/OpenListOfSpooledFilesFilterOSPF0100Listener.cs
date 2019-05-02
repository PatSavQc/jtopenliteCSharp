///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFilterOSPF0100Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{
	public interface OpenListOfSpooledFilesFilterOSPF0100Listener : OpenListOfSpooledFilesFilterListener
	{
	  int NumberOfUserNames {get;}

	  string getUserName(int index);

	  int NumberOfOutputQueues {get;}

	  string getOutputQueueName(int index);

	  string getOutputQueueLibrary(int index);

	  string FormType {get;}

	  string UserSpecifiedData {get;}

	  int NumberOfStatuses {get;}

	  string getStatus(int index);

	  int NumberOfPrinterDevices {get;}

	  string getPrinterDevice(int index);
	}

}