///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfSpooledFilesFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.print
{
	using com.ibm.jtopenlite.command.program.openlist;

	public interface OpenListOfSpooledFilesFormat<T> : ListEntryFormat<T> where T : OpenListOfSpooledFilesFormatListener
	{

	  string Name {get;}

	  int Type {get;}
	}

	public static class OpenListOfSpooledFilesFormat_Fields
	{
	  public const int FORMAT_OSPL0100 = 0;
	  public const int FORMAT_OSPL0200 = 1;
	  public const int FORMAT_OSPL0300 = 2;
	}


}