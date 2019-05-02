///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListActiveJobs.java
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
	/// Represents the information returned by the WRKACTJOB command, but uses the OpenListOfJobs classes to obtain it.
	/// 
	/// </summary>
	public class ListActiveJobs
	{
	  private readonly ListActiveJobsImpl impl_ = new ListActiveJobsImpl();

	  public ListActiveJobs()
	  {
	  }

	  /// <summary>
	  /// Returns the elapsed time since job statistics were reset, in milliseconds.
	  /// 
	  /// </summary>
	  public virtual long ElapsedTime
	  {
		  get
		  {
			return impl_.ElapsedTime;
		  }
	  }

	  /// <summary>
	  /// Returns an array of active jobs, sorted by subsystem and job name, the way WRKACTJOB does.
	  /// JobInfo.toString() prints the fields the way WRKACTJOB does. </summary>
	  /// <param name="conn"> The connection to use. </param>
	  /// <param name="reset"> Indicates if the job statistics should be reset on this invocation, like F10 in WRKACTJOB does.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JobInfo[] getJobs(final CommandConnection conn, final boolean reset) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual JobInfo[] getJobs(CommandConnection conn, bool reset)
	  {
		impl_.ActiveJobsListener = impl_;
		return impl_.getJobs(conn, reset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getJobs(final CommandConnection conn, final boolean reset, final ActiveJobsListener ajListener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void getJobs(CommandConnection conn, bool reset, ActiveJobsListener ajListener)
	  {
		impl_.ActiveJobsListener = ajListener;
		impl_.getJobs(conn, reset);
	  }
	}


}