///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Connection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	/// <summary>
	/// Interface that represents a connection to an IBM i server.
	/// 
	/// </summary>
	public interface Connection
	{
	  /// <summary>
	  /// Closes the connection.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException;
	  void close();

	  /// <summary>
	  /// Returns true if this connection is closed.
	  /// 
	  /// </summary>
	  bool Closed {get;}

	  /// <summary>
	  /// Returns the current authenticated user of this connection.
	  /// 
	  /// </summary>
	  string User {get;}

	  /// <summary>
	  /// Returns the server job associated with this connection, if any.
	  /// 
	  /// </summary>
	  string JobName {get;}
	}

}