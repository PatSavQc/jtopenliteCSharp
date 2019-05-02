using System.Collections.Generic;
using System.Text;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  MessageException.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	/// <summary>
	/// Represents an IOException that includes one or more Messages as part of its exception text.
	/// 
	/// </summary>
	public class MessageException : IOException
	{
	  /// 
		private const long serialVersionUID = 3994984723312909458L;
	  private readonly string text_;
	  private Message[] messages_;
	  private IList<Message> messagesList_;

	  /// <summary>
	  /// Constructs a MessageException with the specified preamble and messages.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public MessageException(final String preamble, final Message[] messages)
	  public MessageException(string preamble, Message[] messages) : base(buildString(preamble, messages))
	  {
		text_ = preamble;
		messages_ = messages;
		messagesList_ = null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public MessageException(final String preamble, final java.util.List<Message> messages)
	  public MessageException(string preamble, IList<Message> messages) : base(buildString(preamble, messages))
	  {
		text_ = preamble;
		messagesList_ = messages;
		messages_ = null;
	  }

	  /// <summary>
	  /// Constructs a MessageException with no preamble.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public MessageException(final Message[] messages)
	  public MessageException(Message[] messages) : base(buildString(null, messages))
	  {
		text_ = null;
		messages_ = messages;
		messagesList_ = null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public MessageException(final java.util.List<Message> messages)
	  public MessageException(IList<Message> messages) : base(buildString(null, messages))
	  {
		text_ = null;
		messages_ = null;
		messagesList_ = messages;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final String buildString(final String text, final java.util.List<Message> messages)
	  private static string buildString(string text, IList<Message> messages)
	  {
		StringBuilder s = new StringBuilder();
		if (!string.ReferenceEquals(text, null))
		{
			s.Append(text);
			s.Append("\n");

		}
		s.Append(messages[0].ToString());

		for (int i = 1; i < messages.Count; ++i)
		{
		  s.Append("\n");
		  s.Append(messages[i].ToString());
		}
		return s.ToString();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final String buildString(final String text, final Message[] messages)
	  private static string buildString(string text, Message[] messages)
	  {
		   // Use string buffer to improve performance 
		StringBuilder s = new StringBuilder();
		if (string.ReferenceEquals(text, null))
		{
			s.Append(messages[0].ToString());
		}
		else
		{
			s.Append(text);
			s.Append("\n");
			s.Append(messages[0].ToString());
		}
		for (int i = 1; i < messages.Length; ++i)
		{
		s.Append("\n");
		s.Append(messages[i].ToString());
		}
		return s.ToString();
	  }

	  /// <summary>
	  /// Returns the preamble, which may be null.
	  /// 
	  /// </summary>
	  public virtual string Preamble
	  {
		  get
		  {
			return text_;
		  }
	  }

	  /// <summary>
	  /// Returns the array of Messages.
	  /// 
	  /// </summary>
	  public virtual Message[] Messages
	  {
		  get
		  {
			if (messages_ == null)
			{
				if (messagesList_ != null)
				{
					messages_ = new Message[messagesList_.Count];
					for (int i = 0 ;i < messages_.Length; i++)
					{
						messages_[i] = messagesList_[i];
					}
				}
			}
			return messages_;
		  }
	  }

	  /// <summary>
	  /// Returns the list of Messages.
	  /// 
	  /// </summary>
	  public virtual IList<Message> MessagesList
	  {
		  get
		  {
			if (messagesList_ == null)
			{
				if (messages_ != null)
				{
					messagesList_ = new LinkedList<Message>();
					for (int i = 0; i < messages_.Length ; i++)
					{
						messagesList_.Add(messages_[i]);
					}
				}
			}
			return messagesList_;
		  }
	  }

	}


}