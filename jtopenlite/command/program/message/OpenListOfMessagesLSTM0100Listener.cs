///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfMessagesLSTM0100Listener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.message
{
	using com.ibm.jtopenlite.command.program.openlist;

	public interface OpenListOfMessagesLSTM0100Listener : ListFormatListener
	{
	  void newMessageEntry(int numberOfFieldsReturned, int messageSeverity, string messageIdentifier, string messageType, int messageKey, string messageFileName, string messageFileLibrarySpecifiedAtSendTime, string messageQueueName, string messageQueueLibrary, string dateSent, string timeSent, string microseconds);

	  void newIdentifierField(int identifierField, string typeOfData, string statusOfData, int lengthOfData, sbyte[] tempData, int offsetOfTempData);
	}


}