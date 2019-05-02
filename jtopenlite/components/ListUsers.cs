///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListUsers.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite.ddm;

	/// <summary>
	/// Represents the information returned by the WRKUSRPRF command.
	/// 
	/// </summary>
	public class ListUsers
	{
	  private readonly ListUsersImpl impl_ = new ListUsersImpl();

	  public ListUsers()
	  {
	  }

	  /// <summary>
	  /// Returns an array of users, the way WRKUSRPRF does. </summary>
	  /// <param name="conn"> The connection to use.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UserInfo[] getUsers(final DDMConnection conn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual UserInfo[] getUsers(DDMConnection conn)
	  {
		impl_.UserInfoListener = impl_;
		return impl_.getUsers(conn);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getUsers(final DDMConnection conn, final UserInfoListener listener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void getUsers(DDMConnection conn, UserInfoListener listener)
	  {
		impl_.UserInfoListener = listener;
		impl_.getUsers(conn);
	  }
	}


}