///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  About.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
//
// Major Change Log
// Date       Description
// ---------- ---------------------------------------
// 2014.08.04 Use client NLV for remote command server. 
//            Ignore various error responses which exchanging attributes with remote
//            command server. 
//           
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{
	/// <summary>
	/// This class provides information about the current version of JTOpenLite
	/// 
	/// </summary>
	public class About
	{
		public static string INTERFACE_NAME = "jtopenlite";
		/// <summary>
		/// The INTERFACE_LEVEL represents the level of the interface.  For now,
		/// we just use the current date.  This date is automatically adjusted
		/// each time jtopenlite.jar is built.
		/// </summary>
		public static string INTERFACE_LEVEL = "20190304";


	}

}