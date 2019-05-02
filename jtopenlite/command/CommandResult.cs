using System.Collections.Generic;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CommandResult.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command
{

	using com.ibm.jtopenlite;

	/// <summary>
	/// The result of calling a program or command. </summary>
	/// <seealso cref= CommandConnection#call </seealso>
	/// <seealso cref= CommandConnection#execute
	///  </seealso>
	public class CommandResult
	{
	  private bool success_;
	  private Message[] messages_;
	  private IList<Message> messageList_;
	  private int rc_;

	  internal CommandResult(bool success, Message[] messages, int rc)
	  {
		success_ = success;
		messages_ = messages;
		messageList_ = null;
		rc_ = rc;
	  }

	  internal CommandResult(bool success, IList<Message> messages, int rc)
	  {
		success_ = success;
		messages_ = null;
		messageList_ = messages;
		rc_ = rc;
	  }

	  /// <summary>
	  /// Returns true if the call was successful.
	  /// 
	  /// </summary>
	  public virtual bool succeeded()
	  {
		return success_;
	  }

	  /// <summary>
	  /// Returns the return code from the call, if any.
	  /// 
	  /// </summary>
	  public virtual int ReturnCode
	  {
		  get
		  {
			return rc_;
		  }
	  }

	  /// <summary>
	  /// Returns any messages that were issued during the call.
	  /// 
	  /// </summary>
	  public virtual Message[] Messages
	  {
		  get
		  {
			  if (messages_ == null)
			  {
				  if (messageList_ != null)
				  {
					int size = messageList_.Count;
					messages_ = new Message[size];
					for (int i = 0; i < size; i++)
					{
						messages_[i] = messageList_[i];
					}
				  }
			  }
			  return messages_;
		  }
	  }

		public virtual IList<Message> MessagesList
		{
			get
			{
				if (messageList_ == null)
				{
					if (messages_ != null)
					{
						messageList_ = new LinkedList<Message>();
						for (int i = 0; i < messages_.Length; i++)
						{
							messageList_.Add(messages_[i]);
						}
					}
				}
				return messageList_;
			}
		}

	  public override string ToString()
	  {
		// Use string buffer to improve performance 
		StringBuilder s = new StringBuilder("" + success_);
		s.Append("; rc=0x");
		s.Append(rc_.ToString("x"));
		if (messageList_ != null)
		{
			Messages;
		}
		if (messages_ != null)
		{
		  for (int i = 0; i < messages_.Length; ++i)
		  {
			s.Append("\n");
			s.Append(messages_[i].ToString());
		  }
		}
		return s.ToString();
	  }
	}

}