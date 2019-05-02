///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListSpooledFiles.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.components
{

	using CommandConnection = com.ibm.jtopenlite.command.CommandConnection;

	/// <summary>
	/// Represents the information returned by the WRKSPLF command, but uses the OpenListOfSpooledFiles classes to obtain it.
	/// 
	/// </summary>
	public class ListSpooledFiles
	{
	  private readonly ListSpooledFilesImpl impl_ = new ListSpooledFilesImpl();

	  public ListSpooledFiles()
	  {
	  }

	  /// <summary>
	  /// Returns an array of spooled files for the current user, similar to the way WRKSPLF does.
	  /// The various SpooledFileInfo.toString() methods print the fields the way WRKSPLF does. </summary>
	  /// <param name="conn"> The connection to use.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpooledFileInfo[] getSpooledFiles(final com.ibm.jtopenlite.command.CommandConnection conn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual SpooledFileInfo[] getSpooledFiles(CommandConnection conn)
	  {
		return getSpooledFiles(conn, "*CURRENT");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getSpooledFiles(final com.ibm.jtopenlite.command.CommandConnection conn, final SpooledFileInfoListener listener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void getSpooledFiles(CommandConnection conn, SpooledFileInfoListener listener)
	  {
		getSpooledFiles(conn, "*CURRENT", listener);
	  }

	  /// <summary>
	  /// Returns an array of spooled files for the specified user, similar to the way WRKSPLF does.
	  /// The various SpooledFileInfo.toString() methods print the fields the way WRKSPLF does. </summary>
	  /// <param name="conn"> The connection to use. </param>
	  /// <param name="user"> The user name, or *CURRENT, or *ALL.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpooledFileInfo[] getSpooledFiles(final com.ibm.jtopenlite.command.CommandConnection conn, String user) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual SpooledFileInfo[] getSpooledFiles(CommandConnection conn, string user)
	  {
		impl_.SpooledFileInfoListener = impl_;
		return impl_.getSpooledFiles(conn, user);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getSpooledFiles(final com.ibm.jtopenlite.command.CommandConnection conn, String user, final SpooledFileInfoListener listener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void getSpooledFiles(CommandConnection conn, string user, SpooledFileInfoListener listener)
	  {
		impl_.SpooledFileInfoListener = listener;
		impl_.getSpooledFiles(conn, user);
	  }
	}


}