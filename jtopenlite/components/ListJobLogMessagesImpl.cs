///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListJobLogMessagesImpl.java
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
	using com.ibm.jtopenlite.command.program.message;
	using com.ibm.jtopenlite.command.program.openlist;

	internal class ListJobLogMessagesImpl : OpenListOfJobLogMessagesOLJL0100Listener, OpenListOfJobLogMessagesSelectionListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			messageList_ = new OpenListOfJobLogMessages(1000, 1, messageFormat_);
			handler_ = new OpenListHandler(messageList_, messageFormat_, this);
		}

	  private readonly OpenListOfJobLogMessagesOLJL0100 messageFormat_ = new OpenListOfJobLogMessagesOLJL0100();
	  private OpenListOfJobLogMessages messageList_;
	  private OpenListHandler handler_;

	  private int counter_ = -1;
	  private MessageInfo[] messages_;
	  private readonly char[] charBuffer_ = new char[4096];

	  private string jobName_;

	  public ListJobLogMessagesImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		messageList_.SelectionListener = this;
	  }

	  public virtual void openComplete()
	  {
	  }

	  public virtual void totalRecordsInList(int total)
	  {
		messages_ = new MessageInfo[total];
		counter_ = -1;
	  }

	  public virtual bool stopProcessing()
	  {
		return false;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized MessageInfo[] getMessages(final CommandConnection conn, JobInfo job) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual MessageInfo[] getMessages(CommandConnection conn, JobInfo job)
	  {
		  lock (this)
		  {
			messages_ = null;
			counter_ = -1;
			jobName_ = job.JobName;
			while (jobName_.Length < 10)
			{
				jobName_ = jobName_ + " ";
			}
			jobName_ = jobName_ + job.UserName;
			while (jobName_.Length < 20)
			{
				jobName_ = jobName_ + " ";
			}
			jobName_ = jobName_ + job.JobNumber;
        
			handler_.process(conn, 1000);
			return messages_;
		  }
	  }

	  // OLJL0100 listener.

	  public virtual void newMessageEntry(int numberOfFieldsReturned, int messageSeverity, string messageIdentifier, string messageType, int messageKey, string messageFileName, string messageFileLibrarySpecifiedAtSendTime, string dateSent, string timeSent, string microseconds, sbyte[] threadID)
	  {
		messages_[++counter_] = new MessageInfo(messageSeverity, messageIdentifier, messageType, messageKey, dateSent, timeSent, microseconds);
	  }


	  public virtual void newIdentifierField(int identifierField, string typeOfData, string statusOfData, int lengthOfData, sbyte[] tempData, int offsetOfTempData)
	  {
		switch (identifierField)
		{
		  case 1001:
			messages_[counter_].ReplyStatus = Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_);
			break;
		  case 302:
			messages_[counter_].Text = Conv.ebcdicByteArrayToString(tempData, offsetOfTempData, lengthOfData, charBuffer_);
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

	  public virtual string QualifiedJobName
	  {
		  get
		  {
			return jobName_;
		  }
	  }

	  public virtual sbyte[] InternalJobIdentifier
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual int StartingMessageKey
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

	  public virtual string CallMessageQueueName
	  {
		  get
		  {
			return "*";
		  }
	  }
	}
}