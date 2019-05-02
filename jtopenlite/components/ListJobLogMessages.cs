///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListJobLogMessages.java
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
	/// Represents the information returned by the DSPJOBLOG command, but uses the OpenListOfJobLogMessages classes to obtain it.
	/// 
	/// </summary>
	public class ListJobLogMessages
	{
	  private readonly ListJobLogMessagesImpl impl_ = new ListJobLogMessagesImpl();

	  public ListJobLogMessages()
	  {
	  }

	  /// <summary>
	  /// Returns an array of messages, sorted by time, the way DSPJOBLOG does.
	  /// MessageInfo.toString() prints the message ID and text; MessageInfo.toString2()
	  /// prints the message details the way F1 on a message does. </summary>
	  /// <param name="conn"> The connection to use. </param>
	  /// <param name="job"> The job from which to retrieve log messages.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MessageInfo[] getMessages(final CommandConnection conn, JobInfo job) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual MessageInfo[] getMessages(CommandConnection conn, JobInfo job)
	  {
		return impl_.getMessages(conn, job);
	  }
	}


}