///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  UserInfo.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	/// <summary>
	/// Represents user information returned by the ListUsers class.
	/// The toString() method will print the fields in a format similar to WRKUSRPRF.
	/// 
	/// </summary>
	public sealed class UserInfo
	{
	  private readonly string name_;
	  private readonly string userClass_;
	  private readonly string expired_;
	  private readonly long maxStorage_;
	  private readonly long storageUsed_;
	  private readonly string description_;
	  private readonly string locked_;
	  private readonly string damaged_;
	  private readonly string status_;
	  private readonly long uid_;
	  private readonly long gid_;

	  internal UserInfo(string userName, string userClass, string passwordExpired, long maxStorage, long storageUsed, string description, string locked, string damaged, string status, long uid, long gid)
	  {
		name_ = userName;
		userClass_ = userClass;
		expired_ = passwordExpired;
		maxStorage_ = maxStorage;
		storageUsed_ = storageUsed;
		description_ = description;
		locked_ = locked;
		damaged_ = damaged;
		status_ = status;
		uid_ = uid;
		gid_ = gid;
	  }

	  public string Name
	  {
		  get
		  {
			return name_;
		  }
	  }

	  public string UserClass
	  {
		  get
		  {
			return userClass_;
		  }
	  }

	  public string Expired
	  {
		  get
		  {
			return expired_;
		  }
	  }

	  /// <summary>
	  /// Returns 1 if the setting is *NOMAX.
	  /// 
	  /// </summary>
	  public long StorageMax
	  {
		  get
		  {
			return maxStorage_;
		  }
	  }

	  public long StorageUsed
	  {
		  get
		  {
			return storageUsed_;
		  }
	  }

	  public string Description
	  {
		  get
		  {
			return description_;
		  }
	  }

	  public string Locked
	  {
		  get
		  {
			return locked_;
		  }
	  }

	  public string Damaged
	  {
		  get
		  {
			return damaged_;
		  }
	  }

	  public string Status
	  {
		  get
		  {
			return status_;
		  }
	  }

	  public long UID
	  {
		  get
		  {
			return uid_;
		  }
	  }

	  public long GID
	  {
		  get
		  {
			return gid_;
		  }
	  }

	  public override string ToString()
	  {
		return name_ + "  " + description_;
	  }
	}

}