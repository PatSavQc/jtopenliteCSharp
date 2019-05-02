///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: JobKeyDataListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public interface JobKeyDataListener
	{
	  void newKeyData(int key, string data, sbyte[] originalTempData, int originalOffset);

	  void newKeyData(int key, int data);

	//  public void newKeyData(int key, long data);
	}



}