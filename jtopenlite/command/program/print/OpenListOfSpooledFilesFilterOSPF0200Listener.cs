///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFilterOSPF0200Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{
	public interface OpenListOfSpooledFilesFilterOSPF0200Listener : OpenListOfSpooledFilesFilterOSPF0100Listener
	{
	  string SystemName {get;}

	  string StartingSpooledFileCreateDate {get;}

	  string StartingSpooledFileCreateTime {get;}

	  string EndingSpooledFileCreateDate {get;}

	  string EndingSpooledFileCreateTime {get;}
	}

}