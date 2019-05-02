///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Message.java
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
	/// Represents a IBM i message.
	/// 
	/// </summary>
	public class Message
	{
	  private string id_;
	  private string text_;
	  private int severity_;

	  /// <summary>
	  /// Constructs a new message with the provided ID and message text.
	  /// 
	  /// </summary>
	  public Message(string id, string text)
	  {
		id_ = id;
		text_ = text;
	  }

	  /// <summary>
	  /// Constructs a new message with the provided ID, message text, and severity.
	  /// 
	  /// </summary>
	  public Message(string id, string text, int severity)
	  {
		id_ = id;
		text_ = text;
		severity_ = severity;
	  }

	  /// <summary>
	  /// Returns the severity of this message, or 0 if unknown.
	  /// 
	  /// </summary>
	  public virtual int Severity
	  {
		  get
		  {
			return severity_;
		  }
	  }

	  /// <summary>
	  /// Returns the message ID of this message.
	  /// 
	  /// </summary>
	  public virtual string ID
	  {
		  get
		  {
			return id_;
		  }
	  }

	  /// <summary>
	  /// Returns the message text of this message.
	  /// 
	  /// </summary>
	  public virtual string Text
	  {
		  get
		  {
			return text_;
		  }
	  }

	  /// <summary>
	  /// Returns a String representation of this message which consists of the message ID and message text.
	  /// 
	  /// </summary>
	  public override string ToString()
	  {
		return id_ + ": " + text_;
	  }
	}

}