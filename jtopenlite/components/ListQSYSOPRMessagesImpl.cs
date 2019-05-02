///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListQSYSOPRMessagesImpl.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.openlist;
	using com.ibm.jtopenlite.command.program.message;

	internal class ListQSYSOPRMessagesImpl : OpenListOfMessagesLSTM0100Listener, OpenListOfMessagesSelectionListener, MessageInfoListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			messageList_ = new OpenListOfMessages(1200, 1, "0", "1", "QSYSOPR", "QSYS", messageFormat_);
			handler_ = new OpenListHandler(messageList_, messageFormat_, this);
		}

	  private readonly OpenListOfMessagesLSTM0100 messageFormat_ = new OpenListOfMessagesLSTM0100();
	  private OpenListOfMessages messageList_;
	  private OpenListHandler handler_;

	  private int counter_ = -1;
	  private MessageInfo[] messages_;
	  private readonly char[] charBuffer_ = new char[4096];

	  private MessageInfoListener miListener_;

	  public ListQSYSOPRMessagesImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		messageList_.SelectionListener = this;
	  }

	  public virtual MessageInfoListener MessageInfoListener
	  {
		  set
		  {
			miListener_ = value;
		  }
	  }

	  public virtual void openComplete()
	  {
	  }

	  public virtual void totalRecordsInList(int totalRecords)
	  {
		miListener_.totalRecords(totalRecords);
	  }

	  public virtual void totalRecords(int totalRecords)
	  {
		messages_ = new MessageInfo[totalRecords];
		counter_ = -1;
	  }

	  public virtual bool stopProcessing()
	  {
		return miListener_.done();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized MessageInfo[] getMessages(final CommandConnection conn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual MessageInfo[] getMessages(CommandConnection conn)
	  {
		  lock (this)
		  {
				messages_ = null;
				counter_ = -1;
				handler_.process(conn, 1000);
				return messages_;
		  }
	  }

	  public virtual bool done()
	  {
		return false;
	  }

	  public virtual void newMessageInfo(MessageInfo info, int index)
	  {
		messages_[index] = info;
	  }

	  public virtual void replyStatus(string status, int index)
	  {
		messages_[index].ReplyStatus = status;
	  }

	  public virtual void messageText(string text, int index)
	  {
		messages_[index].Text = text;
	  }

	  // LSTM0100 listener.

	  public virtual void newMessageEntry(int numberOfFieldsReturned, int messageSeverity, string messageIdentifier, string messageType, int messageKey, string messageFileName, string messageFileLibrarySpecifiedAtSendTime, string messageQueueName, string messageQueueLibrary, string dateSent, string timeSent, string microseconds)
	  {
	//    messages_[++counter_] = new MessageInfo(messageSeverity, messageIdentifier, messageType, messageKey, dateSent, timeSent, microseconds);
		miListener_.newMessageInfo(new MessageInfo(messageSeverity, messageIdentifier, messageType, messageKey, dateSent, timeSent, microseconds), ++counter_);
	  }


	  public virtual void newIdentifierField(int identifierField, string typeOfData, string statusOfData, int lengthOfData, sbyte[] tempData, int offsetOfTempData)
	  {
	//    System.out.println("New identifier: "+identifierField+","+typeOfData+","+statusOfData+","+lengthOfData+","+offsetOfTempData);
		switch (identifierField)
		{
		  case 1001:
	//        messages_[counter_].setReplyStatus(Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_));
			miListener_.replyStatus(Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_), counter_);
			break;
		  case 302:
	//        messages_[counter_].setText(Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_));
			miListener_.messageText(Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_), counter_);
			break;
		}
	  }

	  // Selection listener.

	  public virtual string ListDirection
	  {
		  get
		  {
			return "*NEXT";
		  }
	  }

	  public virtual int SeverityCriteria
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int MaximumMessageLength
	  {
		  get
		  {
			return 8192;
		  }
	  }

	  public virtual int MaximumMessageHelpLength
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int SelectionCriteriaCount
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual string getSelectionCriteria(int index)
	  {
		return "*ALL";
	  }

	  public virtual int StartingMessageKeyCount
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual int getStartingMessageKey(int index)
	  {
		return 0;
	  }

	  private static readonly int[] fields_ = new int[] {1001, 302};

	  public virtual int FieldIdentifierCount
	  {
		  get
		  {
			return fields_.Length;
		  }
	  }

	  public virtual int getFieldIdentifier(int index)
	  {
		return fields_[index];
	  }

	}

}