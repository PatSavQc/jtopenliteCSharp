///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  NotImplementedException.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{

	/// <summary>
	/// Thrown by any JDBC method that is not yet implemented by this driver.
	/// 
	/// </summary>
	public class NotImplementedException : SQLException
	{
	  /// 
		private const long serialVersionUID = 1877922429046020802L;

	internal NotImplementedException() : base("Not implemented")
	{
	}
	}

}