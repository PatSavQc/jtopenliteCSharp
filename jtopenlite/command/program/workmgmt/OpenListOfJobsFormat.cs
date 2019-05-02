///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListsOfJobsFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	using com.ibm.jtopenlite.command.program.openlist;

	public interface OpenListOfJobsFormat<T> : ListEntryFormat<T> where T : OpenListOfJobsFormatListener
	{

	  string Name {get;}

	  int Type {get;}

	  int MinimumRecordLength {get;}

	  OpenListOfJobsKeyField[] KeyFields {set;}
	}

	public static class OpenListOfJobsFormat_Fields
	{
	  public const int FORMAT_OLJB0100 = 0;
	  public const int FORMAT_OLJB0200 = 1;
	  public const int FORMAT_OLJB0300 = 2;
	}

}