///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveSystemStatusPoolListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public interface RetrieveSystemStatusPoolListener
	{
	  /// <summary>
	  /// FORMAT_SSTS0300, FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  void newPoolInfo(int systemPoolIdentifier, long poolSize, long reservedSize, int maximumActiveThreads, int databaseFaults, int databasePages, int nonDatabaseFaults, int nonDatabasePages, int activeToWait, int waitToIneligible, int activeToIneligible, string poolName, string subsystemName, string subsystemLibrary, string pagingOption);

	  /// <summary>
	  /// FORMAT_SSTS0400.
	  /// 
	  /// </summary>
	  void extraPoolInfo(int systemPoolIdentifier, int definedSize, int currentThreads, int currentIneligibleThreads, int tuningPriority, int tuningMinimumPoolSizePercent, int tuningMaximumPoolSizePercent, int tuningMinimumFaults, int tuningPerThreadFaults, int tuningMaximumFaults, string description, int status, int tuningMinimumActivityLevel, int tuningMaximumActivityLevel);

	  /// <summary>
	  /// FORMAT_SSTS0500.
	  /// 
	  /// </summary>
	  void newSubsystemInfo(int systemPoolIdentifier, string poolName, string subsystemName, string subsystemLibrary);
	}

}