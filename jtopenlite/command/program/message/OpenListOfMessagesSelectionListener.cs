///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfMessageSelectionListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.message
{
	public interface OpenListOfMessagesSelectionListener
	{
	  string ListDirection {get;}

	  int SeverityCriteria {get;}

	  int MaximumMessageLength {get;}

	  int MaximumMessageHelpLength {get;}

	  int SelectionCriteriaCount {get;}

	  string getSelectionCriteria(int index);

	  int StartingMessageKeyCount {get;}

	  int getStartingMessageKey(int index);

	  int FieldIdentifierCount {get;}

	  int getFieldIdentifier(int index);
	}

}