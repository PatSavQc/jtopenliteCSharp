///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveJournalEntriesSelectionListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.journal
{
	/// <summary>
	/// Listener interface used to pass parameters to a call to 
	/// RetrieveJournalEntries, which uses the QjoRetrieveJournalEntries API.
	/// 
	/// </summary>
	public interface RetrieveJournalEntriesSelectionListener
	{
	  int NumberOfVariableLengthRecords {get;}
	  int getVariableLengthRecordKey(int index);

	  int getVariableLengthRecordDataLength(int index);
	  void setVariableLengthRecordData(int index, sbyte[] buffer, int offset);
	}

}