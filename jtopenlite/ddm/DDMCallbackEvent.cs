///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMCallbackEvent.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{
	/// <summary>
	/// Contains a reference to the file that generated the event.
	/// 
	/// </summary>
	public class DDMCallbackEvent
	{
	  public const int EVENT_WRITE = 0;
	  public const int EVENT_UPDATE = 1;
	  public const int EVENT_READ = 2;

	  private DDMFile file_;
	  private int type_ = EVENT_WRITE;

	  internal DDMCallbackEvent(DDMFile f)
	  {
		file_ = f;
	  }

	  internal virtual int EventType
	  {
		  set
		  {
			type_ = value;
		  }
		  get
		  {
			return type_;
		  }
	  }


	  public virtual DDMFile File
	  {
		  get
		  {
			return file_;
		  }
	  }
	}

}