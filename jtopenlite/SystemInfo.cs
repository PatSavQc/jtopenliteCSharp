///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  SystemInfo.java
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
	/// Initially obtained from a <seealso cref="SignonConnection SignonConnection"/> object; contains information about
	/// the IBM i host, such as VRM level (V5R4M0, etc) and password level. All host server connections will
	/// have an associated SystemInfo object.
	/// If a connection is constructed using a system name, user, and password, then an implicit SignonConnection
	/// is made to
	/// obtain the SystemInfo object, and then closed.
	/// <para>
	/// For performance reasons, when multiple connections need to be made to
	/// the same host (Command, DDM, File, etc), an application may want to explicitly retrieve the SystemInfo
	/// object directly
	/// from the SignonConnection, in order to avoid any implicit SignonConnections.
	/// 
	/// </para>
	/// </summary>
	public sealed class SystemInfo
	{
		public const int VERSION_540 = 0x050400;
		public const int VERSION_610 = 0x060100;
		public const int VERSION_710 = 0x070100;

	  private string system_;
	  private int serverVersion_;
	  private int serverLevel_;
	  private int serverCCSID_;
	  private int passwordLevel_;
	  private string jobName_;

	  internal SystemInfo(string system, int serverVersion, int serverLevel, int passwordLevel, string jobName)
	  {
		system_ = system;
		serverVersion_ = serverVersion;
		serverLevel_ = serverLevel;
		passwordLevel_ = passwordLevel;
		jobName_ = jobName;
	  }

	  /// <summary>
	  /// Returns the system name.
	  /// 
	  /// </summary>
	  public string System
	  {
		  get
		  {
			return system_;
		  }
	  }

	  internal int ServerCCSID
	  {
		  set
		  {
			serverCCSID_ = value;
		  }
		  get
		  {
			return serverCCSID_;
		  }
	  }


	  /// <summary>
	  /// Returns the server lipi level.
	  /// 
	  /// </summary>
	  public int ServerLevel
	  {
		  get
		  {
			return serverLevel_;
		  }
	  }

	  /// <summary>
	  /// Returns the server VRM version.
	  /// 
	  /// </summary>

	  public int ServerVersion
	  {
		  get
		  {
			 return serverVersion_;
		  }
	  }


	  /// <summary>
	  /// Returns the server password level.
	  /// 
	  /// </summary>
	  public int PasswordLevel
	  {
		  get
		  {
			return passwordLevel_;
		  }
	  }

	  internal string SignonJobName
	  {
		  get
		  {
			return jobName_;
		  }
	  }

	  public override int GetHashCode()
	  {
		return jobName_.GetHashCode();
	  }

	  public override bool Equals(object obj)
	  {
		if (obj != null && obj is SystemInfo)
		{
		  SystemInfo info = (SystemInfo)obj;
		  return info.serverVersion_ == this.serverVersion_ && info.serverLevel_ == this.serverLevel_ && info.passwordLevel_ == this.passwordLevel_ && info.system_.Equals(this.system_) && info.jobName_.Equals(this.jobName_);
		}
		return false;
	  }

	  public override string ToString()
	  {
		return system_ + "[" + serverVersion_.ToString("x") + "]:" + jobName_;
	  }
	}

}