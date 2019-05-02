///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveCurrentAttributesLibraryListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public interface RetrieveCurrentAttributesLibraryListener
	{
	  void newSystemLibrary(string library);

	  void newProductLibrary(string library);

	  void currentLibrary(string library);

	  void newUserLibrary(string library);

	}



}