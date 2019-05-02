///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveAuthorizedUsersListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.security
{
	public interface RetrieveAuthorizedUsersListener
	{
	  void newEntry(string profileName, bool isGroup, bool hasMembers, string textDescription, string[] groupProfiles);
	}



}