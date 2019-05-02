///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListActiveJobsImpl.java
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
	using com.ibm.jtopenlite.command.program.openlist;
	using com.ibm.jtopenlite.command.program.workmgmt;

	internal class ListActiveJobsImpl : OpenListOfJobsFormatOLJB0300Listener, OpenListOfJobsSelectionListener, OpenListOfJobsSortListener, ActiveJobsListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			jobList_ = new OpenListOfJobs(jobFormat_, 120, 1, fieldsToReturn_);
			handler_ = new OpenListHandler(jobList_, jobFormat_, this);
		}

	  private readonly OpenListOfJobsFormatOLJB0300 jobFormat_ = new OpenListOfJobsFormatOLJB0300();
	  private readonly int[] fieldsToReturn_ = new int[] {1906, 314, 602, 601, 305, 2008, 1802, 312, 1306}; // Subsystem information, % CPU used, function type, function name, current user profile, thread count, run priority, CPU used total, memory pool name
	  private OpenListOfJobs jobList_;
	  // private final GetListEntries getJobs_ = new GetListEntries(0, null, 0, 0, 0, jobFormat_);
	  // private final CloseList close_ = new CloseList(null);

	  private OpenListHandler handler_;

	  private ActiveJobsListener ajListener_;

	  private int counter_ = -1;
	  private JobInfo[] jobs_;

	  internal ListActiveJobsImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		jobList_.setSelectionListener(this, OpenListOfJobs.SELECTION_OLJS0100);
		jobList_.SortListener = this;
	  }

	  public virtual ActiveJobsListener ActiveJobsListener
	  {
		  set
		  {
			ajListener_ = value;
		  }
	  }

	  public virtual long ElapsedTime
	  {
		  get
		  {
			return jobList_.ElapsedTime;
		  }
	  }
	  public virtual void totalRecords(int totalRecords)
	  {
		jobs_ = new JobInfo[totalRecords];
	  }

	  public virtual bool stopProcessing()
	  {
		return false;
	  }

	  public virtual void totalRecordsInList(int total)
	  {
		ajListener_.totalRecords(total);
	  }

	  public virtual void openComplete()
	  {
		OpenListOfJobsKeyField[] keyDefinitions = jobList_.KeyFields;
		jobFormat_.KeyFields = keyDefinitions;
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized JobInfo[] getJobs(final CommandConnection conn, final boolean reset) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual JobInfo[] getJobs(CommandConnection conn, bool reset)
	  {
		  lock (this)
		  {
			jobs_ = null;
			counter_ = -1;
			jobList_.ResetStatusStatistics = reset;
			handler_.process(conn, 600);
			  return jobs_;
		  }
	  }

	  private void sort()
	  {
		for (int i = 1; i < jobs_.Length - 1; ++i)
		{
		  JobInfo j = jobs_[i];
		  if (j.JobType.Equals("SBS"))
		  {
			int currentIndex = i;
			// Make sure it's at the top of all the other jobs underneath it.
			JobInfo previous = jobs_[currentIndex - 1];
			while (previous.Subsystem.Equals(j.JobName))
			{
			  jobs_[currentIndex - 1] = j;
			  jobs_[currentIndex] = previous;
			  --currentIndex;
			  j = jobs_[currentIndex];
			  previous = jobs_[currentIndex - 1];
			}
		  }
		}
		// Move SYS jobs to the end.
		int marker = -1;
		for (int i = 0; marker == -1 && i < jobs_.Length; ++i)
		{
		  if (!jobs_[i].JobType.Equals("SYS"))
		  {
			marker = i;
		  }
		}
		while (marker > 0)
		{
		  JobInfo saved = jobs_[0];
		  for (int i = 1; i < jobs_.Length; ++i)
		  {
			jobs_[i - 1] = jobs_[i];
		  }
		  jobs_[jobs_.Length - 1] = saved;
		  --marker;
		}
	  }
	  ////////////////////////////////////////
	  //
	  // Sort methods.
	  //
	  ////////////////////////////////////////

	  public virtual int NumberOfSortKeys
	  {
		  get
		  {
			return 2;
		  }
	  }

	  public virtual int getSortKeyFieldStartingPosition(int keyIndex)
	  {
		if (keyIndex == 0)
		{
		  // Subsystem key.
		  return 41;
		}
		return 1; // Job name, user name, job number.
	  }

	  public virtual int getSortKeyFieldLength(int keyIndex)
	  {
		if (keyIndex == 0)
		{
		  return 20;
		}
		return 26;
	  }

	  public virtual int getSortKeyFieldDataType(int keyIndex)
	  {
		return 6; // Character with no national language sort sequence applied.
	  }

	  public virtual bool isAscending(int keyIndex)
	  {
		return true;
	  }


	  ////////////////////////////////////////
	  //
	  // Conversion methods.
	  //
	  ////////////////////////////////////////

	  public static string getWRKACTJOBType(string type, string subtype)
	  {
		char t = type[0];
		char s = subtype[0];
		switch (t)
		{
		  case 'A':
			  return "ASJ";
		  case 'B':
			switch (s)
			{
			  case ' ':
				  return "BCH";
			  case 'D':
				  return "BCI";
			  case 'E':
				  return "EVK";
			  case 'F':
				  return "M36";
			  case 'T':
				  return "MRT";
			  case 'J':
				  return "PJ ";
			  case 'U':
				  return "   ";
			}
			return null;
		  case 'I':
			  return "INT";
		  case 'W':
			switch (s)
			{
			  case 'P':
				  return "PDJ";
			  case ' ':
				  return "WTR";
			}
			return null;
		  case 'R':
			  return "RDR";
		  case 'S':
		  case 'X':
			  return "SYS";
		  case 'M':
			  return "SBS";
		}
		return null;
	  }

	  public static string getFunctionPrefix(string data)
	  {
		char c = data[0];
		switch (c)
		{
		  case ' ':
		  case '\u0000':
			  return "    ";
		  case 'C':
			  return "CMD-";
		  case 'D':
			  return "DLY-";
		  case 'G':
			  return "GRP-";
		  case 'I':
			  return "IDX-";
		  case 'J':
			  return "JVM-";
		  case 'L':
			  return "LOG-"; // LOG - QHST
		  case 'M':
			  return "MRT-";
		  case 'N':
			  return "MNU-";
		  case 'O':
			  return "I/O-";
		  case 'P':
			  return "PGM-";
		  case 'R':
			  return "PRC-";
		  case '*':
			  return "*  -";
		}
		return null;
	  }

	  public virtual void newJobInfo(JobInfo info, int index)
	  {
		jobs_[index] = info;
	  }

	  public virtual void subsystem(string s, int index)
	  {
		jobs_[index].Subsystem = s;
	  }

	  public virtual void functionPrefix(string s, int index)
	  {
		jobs_[index].FunctionPrefix = s;
	  }

	  public virtual void functionName(string s, int index)
	  {
		jobs_[index].FunctionName = s;
	  }

	  public virtual void currentUser(string s, int index)
	  {
		jobs_[index].CurrentUser = s;
	  }

	  public virtual void totalCPUUsed(long cpu, int index)
	  {
		jobs_[index].TotalCPUUsed = cpu;
	  }

	  public virtual void memoryPool(string s, int index)
	  {
		jobs_[index].MemoryPool = s;
	  }

	  public virtual void cpuPercent(int i, int index)
	  {
		jobs_[index].setCPUPercent(i);
	  }

	  public virtual void threadCount(int i, int index)
	  {
		jobs_[index].ThreadCount = i;
	  }

	  public virtual void runPriority(int i, int index)
	  {
		jobs_[index].RunPriority = i;
	  }

	  ////////////////////////////////////////
	  //
	  // List entry format methods.
	  //
	  ////////////////////////////////////////

	  public virtual void newJobEntry(string jobName, string userName, string jobNumber, string status, string type, string subtype)
	  {
		string wrkactjobType = getWRKACTJOBType(type, subtype);
		ajListener_.newJobInfo(new JobInfo(jobName, userName, jobNumber, wrkactjobType, status), ++counter_);
	  }

	  public virtual void newKeyData(int key, string data, sbyte[] originalTempData, int originalOffset)
	  {
		  lock (this)
		  {
			switch (key)
			{
			  case 1906:
				ajListener_.subsystem(data.Substring(0,10), counter_);
				break;
			  case 602:
				ajListener_.functionPrefix(getFunctionPrefix(data), counter_);
				break;
			  case 601:
				ajListener_.functionName(data[0] == '\u0000' ? "          " : data, counter_);
				break;
			  case 305:
				ajListener_.currentUser(data, counter_);
				break;
			  case 312:
				ajListener_.totalCPUUsed(Conv.byteArrayToLong(originalTempData, originalOffset), counter_);
				break;
			  case 1306:
				ajListener_.memoryPool(data, counter_);
				break;
			}
		  }
	  }

	  public virtual void newKeyData(int key, int data)
	  {
		switch (key)
		{
		  case 314:
			ajListener_.cpuPercent(data, counter_);
			break;
		  case 2008:
			ajListener_.threadCount(data, counter_);
			break;
		  case 1802:
			ajListener_.runPriority(data, counter_);
			break;
		}
	  }

	  ////////////////////////////////////////
	  //
	  // Selection methods.
	  //
	  ////////////////////////////////////////

	  public virtual string JobName
	  {
		  get
		  {
			return "*ALL";
		  }
	  }

	  public virtual string UserName
	  {
		  get
		  {
			return "*ALL";
		  }
	  }

	  public virtual string JobNumber
	  {
		  get
		  {
			return "*ALL";
		  }
	  }

	  public virtual string JobType
	  {
		  get
		  {
			return "*";
		  }
	  }

	  public virtual int PrimaryJobStatusCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getPrimaryJobStatus(int index)
	  {
		return null;
	  }

	  public virtual int ActiveJobStatusCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getActiveJobStatus(int index)
	  {
		return null;
	  }

	  public virtual int JobsOnJobQueueStatusCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getJobsOnJobQueueStatus(int index)
	  {
		return null;
	  }

	  public virtual int JobQueueNameCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getJobQueueName(int index)
	  {
		return null;
	  }

	  public virtual int CurrentUserProfileCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getCurrentUserProfile(int index)
	  {
		return null;
	  }

	  public virtual int ServerTypeCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getServerType(int index)
	  {
		return null;
	  }

	  public virtual int ActiveSubsystemCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getActiveSubsystem(int index)
	  {
		return null;
	  }

	  public virtual int MemoryPoolCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int getMemoryPool(int index)
	  {
		return 0;
	  }

	  public virtual int JobTypeEnhancedCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int getJobTypeEnhanced(int index)
	  {
		return 0;
	  }

	  public virtual int QualifiedJobNameCount
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual string getQualifiedJobName(int index)
	  {
		return null;
	  }
	}

}