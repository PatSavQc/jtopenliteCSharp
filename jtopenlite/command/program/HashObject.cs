///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  HashObject.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command.program
{
	public sealed class HashObject : object
	{
	  private int hash_;

	  public HashObject()
	  {
	  }

	  public int Hash
	  {
		  set
		  {
			hash_ = value;
		  }
	  }

	  public override int GetHashCode()
	  {
		return hash_;
	  }

	  public override bool Equals(object obj)
	  {
		return obj != null && obj is HashObject && ((HashObject)obj).hash_ == this.hash_;
	  }
	}

}