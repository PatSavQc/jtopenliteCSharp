///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListProgram.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Any of the various QGY APIs on the system that process lists of data.
	/// 
	/// </summary>
	public interface OpenListProgram<T, W> : Program where T : ListEntryFormat where W : ListFormatListener
	{
	  ListInformation ListInformation {get;}

	  /// <summary>
	  /// The format listener gets called by the formatter once the output data has been formatted.
	  /// 
	  /// </summary>
	  W FormatListener {get;set;}


	  /// <summary>
	  /// The formatter is the class that handles formatting the output data for each entry in the list.
	  /// 
	  /// </summary>
	  T Formatter {get;set;}

	}

}