///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  MessageInfoListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.components
{
	/// <summary>
	/// Order of operations:
	/// <ul>
	/// <li>totalRecords()</li>
	/// <li>start loop
	///  <ul>
	///   <li>newMessageInfo()</li>
	///   <li>replyStatus() and/or messageText()</li>
	///  </ul>
	/// </li>
	/// <li>end loop</li>
	/// </ul>
	/// 
	/// </summary>
	public interface MessageInfoListener
	{
	  void totalRecords(int total);

	  void newMessageInfo(MessageInfo info, int index);

	  void replyStatus(string status, int index);

	  void messageText(string text, int index);

	  /// <summary>
	  /// Return true to indicate you no longer want to process further messages.
	  /// 
	  /// </summary>
	  bool done();
	}




}