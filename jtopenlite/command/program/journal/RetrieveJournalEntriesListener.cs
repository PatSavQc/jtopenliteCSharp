///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveJournalEntriesListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.journal
{
	public interface RetrieveJournalEntriesListener
	{
	  void newJournalEntries(int numberOfEntriesRetrieved, char continuationHandle);

	  void newEntryData(int pointerHandle, long sequenceNumber, char journalCode, string entryType, string timestamp, string jobName, string userName, string jobNumber, string programName, string @object, int count, char indicatorFlag, long commitCycleIdentifier, string userProfile, string systemName, string journalIdentifier, char referentialConstraint, char trigger, char incompleteData, char objectNameIndicator, char ignoreDuringJournalChange, char minimizedEntrySpecificData);

	}


}