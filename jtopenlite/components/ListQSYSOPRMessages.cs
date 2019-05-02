///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListQSYSOPRMessages.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Represents the information returned by the DSPMSG QSYSOPR command, but uses the OpenListOfMessages classes to obtain it.
	/// 
	/// </summary>
	public class ListQSYSOPRMessages
	{
	  private readonly ListQSYSOPRMessagesImpl impl_ = new ListQSYSOPRMessagesImpl();

	  public ListQSYSOPRMessages()
	  {
	  }

	  /// <summary>
	  /// Returns an array of messages, sorted by time, the way DSPMSG does.
	  /// MessageInfo.toString() prints the message ID and text; MessageInfo.toString2()
	  /// prints the message details the way F1 on a message does. </summary>
	  /// <param name="conn"> The connection to use.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MessageInfo[] getMessages(final CommandConnection conn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual MessageInfo[] getMessages(CommandConnection conn)
	  {
		impl_.MessageInfoListener = impl_;
		return impl_.getMessages(conn);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getMessages(final CommandConnection conn, final MessageInfoListener miListener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void getMessages(CommandConnection conn, MessageInfoListener miListener)
	  {
		impl_.MessageInfoListener = miListener;
		impl_.getMessages(conn);
	  }
	}


}