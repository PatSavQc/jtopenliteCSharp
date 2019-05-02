///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: ListEntryFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{


	/// <summary>
	/// Responsible for formatting output data returned by a call to an OpenListProgram.
	/// 
	/// </summary>
	public interface ListEntryFormat<T> where T : ListFormatListener
	{
	  void format(sbyte[] data, int maxLength, int recordLength, T listener);
	}

}