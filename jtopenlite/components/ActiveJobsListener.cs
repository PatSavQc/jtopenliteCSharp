///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DiskStatus.java
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
	/// <ul>
	/// <li>newJobInfo()</li>
	/// <li>other various setters</li>
	/// </ul>
	/// </li>
	/// <li>end loop</li>
	/// </ul>
	/// 
	/// </summary>
	public interface ActiveJobsListener
	{
	  void totalRecords(int total);

	  void newJobInfo(JobInfo info, int index);

	  void subsystem(string s, int index);

	  void functionPrefix(string s, int index);

	  void functionName(string s, int index);

	  void currentUser(string s, int index);

	  void totalCPUUsed(long cpu, int index);

	  void memoryPool(string s, int index);

	  void cpuPercent(int i, int index);

	  void threadCount(int i, int index);

	  void runPriority(int i, int index);
	}




}