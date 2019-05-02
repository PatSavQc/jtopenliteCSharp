using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JobInfo.java
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
	/// Represents job information returned by the ListActiveJobs class.
	/// The toString(), toString2(), and toString3() methods will print
	/// the various fields in a format similar to what WRKACTJOB does.
	/// 
	/// </summary>
	public class JobInfo
	{
	  private string jobName_;
	  private string userName_;
	  private string jobNumber_;
	  private string jobType_;
	  private string status_;
	  private string subsystem_;
	  private string functionPrefix_;
	  private string functionName_;
	  private string currentUser_;
	  private int cpu_; // Percentage in tenths.
	  private int threadCount_;
	  private int runPriority_;
	  private long totalCPU_; // In milliseconds.
	  private string memoryPool_;

	  internal JobInfo(string jobName, string userName, string jobNumber, string jobType, string status)
	  {
		jobName_ = jobName;
		userName_ = userName;
		jobNumber_ = jobNumber;
		jobType_ = jobType;
		status_ = status;
	  }

	  public static void arrangeBySubsystem(JobInfo[] jobs)
	  {
		Array.Sort(jobs, comparator_);
	/*    for (int i=1; i<jobs.length-1; ++i)
	    {
	      JobInfo j = jobs[i];
	      if (j.getJobType().equals("SBS"))
	      {
	        int currentIndex = i;
	        // Make sure it's at the top of all the other jobs underneath it.
	        JobInfo previous = jobs[currentIndex-1];
	        while (previous.getSubsystem().equals(j.getJobName()))
	        {
	          jobs[currentIndex-1] = j;
	          jobs[currentIndex] = previous;
	          --currentIndex;
	          j = jobs[currentIndex];
	          previous = jobs[currentIndex-1];
	        }
	      }
	    }
	    // Move SYS jobs to the end.
	    int marker = -1;
	    for (int i=0; marker == -1 && i<jobs.length; ++i)
	    {
	      if (!jobs[i].getJobType().equals("SYS"))
	      {
	        marker = i;
	      }
	    }
	    while (marker > 0)
	    {
	      JobInfo saved = jobs[0];
	      for (int i=1; i<jobs.length; ++i)
	      {
	        jobs[i-1] = jobs[i];
	      }
	      jobs[jobs.length-1] = saved;
	      --marker;
	    }
	*/
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final boolean isDigit(final char c)
	  private static bool isDigit(char c)
	  {
		return (c <= '9' && c >= '0');
	  }

	  private static int compareStrings(string s1, string s2)
	  {
		for (int i = 0; i < s1.Length && i < s2.Length; ++i)
		{
		  char c1 = s1[i];
		  char c2 = s2[i];
		  if (c1 != c2)
		  {
			// The iSeries sorts digits after letters, for some reason.
			if (isDigit(c1))
			{
			  if (isDigit(c2))
			  {
				return c1 - c2;
			  }
			  return 1;
			}
			if (isDigit(c2))
			{
			  return -1;
			}
			return c1 - c2;
		  }
		}
		return s1.Length - s2.Length;
	  }


	  private static readonly IComparer<JobInfo> comparator_ = new ComparatorAnonymousInnerClass();

	  private class ComparatorAnonymousInnerClass : IComparer<JobInfo>
	  {
		  public int compare(JobInfo j1, JobInfo j2)
		  {
			string n1 = j1.JobName.Trim();
			string n2 = j2.JobName.Trim();
			string t1 = j1.JobType.Trim();
			string t2 = j2.JobType.Trim();
			string s1 = j1.Subsystem;
			string s2 = j2.Subsystem;
			s1 = (string.ReferenceEquals(s1, null) ? "" : s1.Trim());
			s2 = (string.ReferenceEquals(s2, null) ? "" : s2.Trim());
			if (t1.Equals("SBS"))
			{
			  if (t2.Equals("SBS"))
			  {
				  return compareStrings(n1,n2); //n1.compareTo(n2);
			  }
			  if (t2.Equals("SYS"))
			  {
				  return -1;
			  }
			  if (n1.Equals(s2))
			  {
				  return -1;
			  }
			  return compareStrings(n1,s2); //n1.compareTo(s2);
			}
			if (t2.Equals("SBS"))
			{
			  if (t1.Equals("SYS"))
			  {
				  return 1;
			  }
			  if (n2.Equals(s1))
			  {
				  return 1;
			  }
			  return compareStrings(s1,n2); //s1.compareTo(n2);
			}
			if (t1.Equals("SYS") && t2.Equals("SYS"))
			{
			  return compareStrings(n1,n2); //n1.compareTo(n2);
			}
			if (t1.Equals("SYS"))
			{
			  return 1;
			}
			if (t2.Equals("SYS"))
			{
			  return -1;
			}
			if (s1.Equals(s2))
			{
			  return compareStrings(n1,n2); //n1.compareTo(n2);
			}
			return compareStrings(s1,s2); //s1.compareTo(s2);
		  }
	  }

	  public static void arrangeBySubsystem(IList<JobInfo> jobs)
	  {
		jobs.Sort(comparator_);


	/*    for (int i=1; i<jobs.size()-1; ++i)
	    {
	      JobInfo j = jobs.get(i);
	      if (j.getJobType().equals("SBS"))
	      {
	        int currentIndex = i;
	        // Make sure it's at the top of all the other jobs underneath it.
	        JobInfo previous = jobs.get(currentIndex-1);
	        while (previous.getSubsystem().equals(j.getJobName()))
	        {
	          jobs.set(currentIndex-1, j);
	          jobs.set(currentIndex, previous);
	          --currentIndex;
	          j = jobs.get(currentIndex);
	          previous = jobs.get(currentIndex-1);
	        }
	      }
	    }
	    // Move SYS jobs to the end.
	    int marker = -1;
	    for (int i=0; marker == -1 && i<jobs.size(); ++i)
	    {
	      if (!jobs.get(i).getJobType().equals("SYS"))
	      {
	        marker = i;
	      }
	    }
	    while (marker > 0)
	    {
	      JobInfo saved = jobs.get(0);
	      for (int i=1; i<jobs.size(); ++i)
	      {
	        jobs.set(i-1, jobs.get(i));
	      }
	      jobs.set(jobs.size()-1, saved);
	      --marker;
	    }
	*/
	  }

	  public virtual string JobName
	  {
		  get
		  {
			return jobName_;
		  }
	  }

	  public virtual string UserName
	  {
		  get
		  {
			return userName_;
		  }
	  }

	  public virtual string CurrentUser
	  {
		  set
		  {
			currentUser_ = value;
		  }
		  get
		  {
			return currentUser_;
		  }
	  }


	  public virtual string JobNumber
	  {
		  get
		  {
			return jobNumber_;
		  }
	  }

	  public virtual string JobType
	  {
		  get
		  {
			return jobType_;
		  }
	  }

	  public virtual string Status
	  {
		  get
		  {
			return status_;
		  }
	  }

	  public virtual string Subsystem
	  {
		  set
		  {
			subsystem_ = value;
		  }
		  get
		  {
			return subsystem_;
		  }
	  }


	  public virtual string FunctionPrefix
	  {
		  set
		  {
			functionPrefix_ = (string.ReferenceEquals(value, null) ? "" : value);
		  }
	  }

	  public virtual string FunctionName
	  {
		  set
		  {
			functionName_ = (string.ReferenceEquals(value, null) ? "" : value);
		  }
	  }

	  public virtual string Function
	  {
		  get
		  {
			return functionPrefix_ + functionName_;
		  }
	  }

	  public virtual void setCPUPercent(int data)
	  {
		cpu_ = data;
	  }

	  public virtual int CPUPercentInTenths
	  {
		  get
		  {
			return cpu_;
		  }
	  }

	  public virtual float getCPUPercent()
	  {
		return ((float)cpu_) / 10.0f;
	  }

	  public virtual int ThreadCount
	  {
		  set
		  {
			threadCount_ = value;
		  }
		  get
		  {
			return threadCount_;
		  }
	  }


	  public virtual int RunPriority
	  {
		  set
		  {
			runPriority_ = value;
		  }
		  get
		  {
			return runPriority_;
		  }
	  }


	  public virtual long TotalCPUUsed
	  {
		  set
		  {
			totalCPU_ = value;
		  }
		  get
		  {
			return totalCPU_;
		  }
	  }


	  internal virtual string MemoryPool
	  {
		  set
		  {
			memoryPool_ = value;
		  }
		  get
		  {
			return memoryPool_;
		  }
	  }


	  public virtual int MemoryPoolID
	  {
		  get
		  {
			if (string.ReferenceEquals(memoryPool_, null))
			{
				return 0;
			}
    
			if (memoryPool_.Equals("*BASE     "))
			{
			  return 2; // Always.
			}
			if (memoryPool_.Equals("*INTERACT "))
			{
			  return 4; // Always??
			}
			if (memoryPool_.Equals("*SPOOL    "))
			{
			  return 3; // Always??
			}
			if (memoryPool_.Equals("*MACHINE  "))
			{
			  return 1; // Always.
			}
			return 0; // Unknown.
		  }
	  }

	  private string CPUString
	  {
		  get
		  {
			int num = cpu_ / 10;
			int dec = cpu_ % 10;
			string cpu = num == 0 ? "   ." : (num + ".");
			cpu = cpu + dec;
			while (cpu.Length < 5)
			{
				cpu = " " + cpu;
			}
			return cpu;
		  }
	  }

	  public override string ToString()
	  {
		bool isSubsystem = jobType_.Equals("SBS") || jobType_.Equals("SYS");
		string cpu = CPUString;
		return (isSubsystem ? "" : "  ") + jobName_ + (isSubsystem ? "   " : " ") + currentUser_ + " " + jobType_ + " " + cpu + " " + Function + " " + status_;
	  }

	  private string TotalCPUString
	  {
		  get
		  {
			int total = (int)(totalCPU_ / 100L);
			int num = total / 10;
			int dec = total % 10;
			string cpu = num == 0 ? "      ." : (num + ".");
			cpu = cpu + dec;
			while (cpu.Length < 8)
			{
				cpu = " " + cpu;
			}
			return cpu;
		  }
	  }

	  public virtual string toString2()
	  {
		bool isSubsystem = jobType_.Equals("SBS") || jobType_.Equals("SYS");
		string cpu = TotalCPUString;
		return (isSubsystem ? "" : "  ") + jobName_ + (isSubsystem ? "   " : " ") + jobType_ + " " + MemoryPoolID + " " + (runPriority_ < 10 ? " " : "") + runPriority_ + " " + cpu;
	  }

	  private string ThreadString
	  {
		  get
		  {
			string s = threadCount_.ToString();
			while (s.Length < 8)
			{
				s = " " + s;
			}
			return s;
		  }
	  }

	  public virtual string toString3()
	  {
		bool isSubsystem = jobType_.Equals("SBS") || jobType_.Equals("SYS");
		string cpu = CPUString;
		string thread = ThreadString;
		return (isSubsystem ? "" : "  ") + jobName_ + (isSubsystem ? "   " : " ") + userName_ + " " + jobNumber_ + " " + jobType_ + " " + cpu + " " + thread;
	  }
	}

}