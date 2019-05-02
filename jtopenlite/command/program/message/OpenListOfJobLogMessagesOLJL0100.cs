using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobLogMessagesOLJL0100.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.message
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command.program.openlist;

	public class OpenListOfJobLogMessagesOLJL0100 : ListEntryFormat<OpenListOfJobLogMessagesOLJL0100Listener>
	{
	  private readonly char[] c = new char[10];

	  public OpenListOfJobLogMessagesOLJL0100()
	  {
	  }

	  private readonly sbyte[] lastMessageIdentifierBytes_ = new sbyte[7];
	  private string lastMessageIdentifier_ = "       ";
	  private readonly sbyte[] lastMessageFileNameBytes_ = new sbyte[10];
	  private string lastMessageFileName_ = "          ";
	  private readonly sbyte[] lastMessageFileLibraryBytes_ = new sbyte[10];
	  private string lastMessageFileLibrary_ = "          ";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static boolean matches(final byte[] data, final int offset, final byte[] data2)
	  private static bool matches(sbyte[] data, int offset, sbyte[] data2)
	  {
		for (int i = 0; i < data2.Length; ++i)
		{
		  if (data[offset + i] != data2[i])
		  {
			  return false;
		  }
		}
		return true;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getMessageIdentifier(final byte[] data, final int offset)
	  private void getMessageIdentifier(sbyte[] data, int offset)
	  {
		if (!matches(data, offset, lastMessageIdentifierBytes_))
		{
		  Array.Copy(data, offset, lastMessageIdentifierBytes_, 0, 7);
		  lastMessageIdentifier_ = Conv.ebcdicByteArrayToString(data, offset, 7, c);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getMessageFileName(final byte[] data, final int offset)
	  private void getMessageFileName(sbyte[] data, int offset)
	  {
		if (!matches(data, offset, lastMessageFileNameBytes_))
		{
		  Array.Copy(data, offset, lastMessageFileNameBytes_, 0, 10);
		  lastMessageFileName_ = Conv.ebcdicByteArrayToString(data, offset, 10, c);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getMessageFileLibrary(final byte[] data, final int offset)
	  private void getMessageFileLibrary(sbyte[] data, int offset)
	  {
		if (!matches(data, offset, lastMessageFileLibraryBytes_))
		{
		  Array.Copy(data, offset, lastMessageFileLibraryBytes_, 0, 10);
		  lastMessageFileLibrary_ = Conv.ebcdicByteArrayToString(data, offset, 10, c);
		}
	  }

	  private readonly HashObject hashObject_ = new HashObject();
	  private readonly Dictionary<HashObject, string> messageTypeCache_ = new Dictionary<HashObject, string>();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private String getMessageType(final byte[] data, final int offset)
	  private string getMessageType(sbyte[] data, int offset)
	  {
		int num = Conv.byteArrayToShort(data, offset);
		hashObject_.Hash = num;
		string messageType = (string)messageTypeCache_[hashObject_];
		if (string.ReferenceEquals(messageType, null))
		{
		  HashObject obj = new HashObject();
		  obj.Hash = num;
		  messageType = Conv.ebcdicByteArrayToString(data, offset, 2, c);
		  messageTypeCache_[obj] = messageType;
		}
		return messageType;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfJobLogMessagesOLJL0100Listener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfJobLogMessagesOLJL0100Listener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int offset = 0;
		int offsetToTheNextEntry = 1;
		while (offset < maxLength && offsetToTheNextEntry > 0)
		{
		  offsetToTheNextEntry = Conv.byteArrayToInt(data, offset);
		  int offsetToFieldsReturned = Conv.byteArrayToInt(data, offset + 4);
		  int numberOfFieldsReturned = Conv.byteArrayToInt(data, offset + 8);
		  int messageSeverity = Conv.byteArrayToInt(data, offset + 12);
		  //String messageIdentifier = Conv.ebcdicByteArrayToString(data, offset+16, 7, c);
		  getMessageIdentifier(data, offset + 16);
		  string messageIdentifier = lastMessageIdentifier_;
		  //String messageType = Conv.ebcdicByteArrayToString(data, offset+23, 2, c);
		  string messageType = getMessageType(data, offset + 23);
		  int messageKey = Conv.byteArrayToInt(data, offset + 25);
		  //String messageFileName = Conv.ebcdicByteArrayToString(data, offset+29, 10, c);
		  getMessageFileName(data, offset + 29);
		  string messageFileName = lastMessageFileName_;
		  //String messageFileLibrary = Conv.ebcdicByteArrayToString(data, offset+39, 10, c);
		  getMessageFileLibrary(data, offset + 39);
		  string messageFileLibrary = lastMessageFileLibrary_;
		  //String messageQueue = Conv.ebcdicByteArrayToString(data, offset+49, 10, c);
		  //String messageQueueLibrary = Conv.ebcdicByteArrayToString(data, offset+59, 10, c);
		  string dateSent = Conv.ebcdicByteArrayToString(data, offset + 49, 7, c);
		  string timeSent = Conv.ebcdicByteArrayToString(data, offset + 56, 6, c);
		  string microseconds = Conv.ebcdicByteArrayToString(data, offset + 62, 6, c);
		  sbyte[] threadID = new sbyte[8];
		  Array.Copy(data, offset + 68, threadID, 0, 8);
		  listener.newMessageEntry(numberOfFieldsReturned, messageSeverity, messageIdentifier, messageType, messageKey, messageFileName, messageFileLibrary, dateSent, timeSent, microseconds, threadID);
		  offset = offsetToFieldsReturned;
		  for (int i = 0; i < numberOfFieldsReturned; ++i)
		  {
			int offsetToTheNextFieldInformationReturned = Conv.byteArrayToInt(data, offset);
			// int lengthOfFieldInformationReturned = Conv.byteArrayToInt(data, offset+4);
			int identifierField = Conv.byteArrayToInt(data, offset + 8);
			string typeOfData = Conv.ebcdicByteArrayToString(data, offset + 12, 1, c);
			string statusOfData = Conv.ebcdicByteArrayToString(data, offset + 13, 1, c);
			int lengthOfData = Conv.byteArrayToInt(data, offset + 28);
			listener.newIdentifierField(identifierField, typeOfData, statusOfData, lengthOfData, data, offset + 32);
			offset = offsetToTheNextFieldInformationReturned;
		  }
		  offset = offsetToTheNextEntry;
		}
	  }
	}


}