///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: OpenListOfJobsKeyField.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	public class OpenListOfJobsKeyField
	{
	  private int key_;
	  private bool isBinary_;
	  private int length_;
	  private int displacement_;

	  public OpenListOfJobsKeyField(int key, bool isBinary, int lengthOfData, int displacementToData)
	  {
		key_ = key;
		isBinary_ = isBinary;
		length_ = lengthOfData;
		displacement_ = displacementToData;
	  }

	  public virtual int Key
	  {
		  get
		  {
			return key_;
		  }
	  }

	  public virtual bool Binary
	  {
		  get
		  {
			return isBinary_;
		  }
	  }

	  public virtual int Length
	  {
		  get
		  {
			return length_;
		  }
	  }

	  public virtual int Displacement
	  {
		  get
		  {
			return displacement_;
		  }
	  }
	}

}