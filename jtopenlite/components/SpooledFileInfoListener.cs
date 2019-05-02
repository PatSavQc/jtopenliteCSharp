///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  SpooledFileInfoListener.java
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
	///   <ul>
	///    <li>newSpooledFileInfo()</li>
	///   </ul>
	/// </li>
	/// <li>end loop</li>
	/// </ul>
	/// 
	/// </summary>
	public interface SpooledFileInfoListener
	{
	  void totalRecords(int total);

	  void newSpooledFileInfo(SpooledFileInfo info, int index);
	}




}