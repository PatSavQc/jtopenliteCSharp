///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsSelectionListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public interface OpenListOfJobsSelectionListener
	{
	  string JobName {get;}

	  string UserName {get;}

	  string JobNumber {get;}

	  string JobType {get;}

	  int PrimaryJobStatusCount {get;}

	  string getPrimaryJobStatus(int index);

	  int ActiveJobStatusCount {get;}

	  string getActiveJobStatus(int index);

	  int JobsOnJobQueueStatusCount {get;}

	  string getJobsOnJobQueueStatus(int index);

	  int JobQueueNameCount {get;}

	  string getJobQueueName(int index);

	  int CurrentUserProfileCount {get;}

	  string getCurrentUserProfile(int index);

	  int ServerTypeCount {get;}

	  string getServerType(int index);

	  int ActiveSubsystemCount {get;}

	  string getActiveSubsystem(int index);

	  int MemoryPoolCount {get;}

	  int getMemoryPool(int index);

	  int JobTypeEnhancedCount {get;}

	  int getJobTypeEnhanced(int index);

	  int QualifiedJobNameCount {get;}

	  string getQualifiedJobName(int index);
	}

}