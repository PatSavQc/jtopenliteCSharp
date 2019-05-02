///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListObjects.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Represents the information returned by the WRKOBJ command, but uses the OpenListOfObjects classes to obtain it.
	/// 
	/// </summary>
	public class ListObjects
	{
	  private readonly ListObjectsImpl impl_ = new ListObjectsImpl();

	  public ListObjects()
	  {
	  }

	  /// <summary>
	  /// Returns an array of objects, sorted by library list order and object name, the way WRKOBJ does.
	  /// ObjectInfo.toString() prints the fields the way WRKJOB does. </summary>
	  /// <param name="conn"> The connection to use. </param>
	  /// <param name="name"> The object name for which to search. For example, "CRT*". </param>
	  /// <param name="library"> The library name in which to search. For example, "*LIBL". </param>
	  /// <param name="type"> The object type for which to search. For example, "*CMD".
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectInfo[] getObjects(final CommandConnection conn, final String name, final String library, final String type) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual ObjectInfo[] getObjects(CommandConnection conn, string name, string library, string type)
	  {
		return impl_.getObjects(conn, name, library, type);
	  }
	}


}