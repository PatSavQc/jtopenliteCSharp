///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ObjectInfo.java
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
	/// Represents object information returned by the ListObjects class.
	/// The toString() and toString2() methods will print
	/// the various fields in a format similar to what WRKOBJ does.
	/// 
	/// </summary>
	public class ObjectInfo
	{
	  private string name_;
	  private string library_;
	  private string type_;
	  private string status_;

	  private string attribute_;
	  private string description_;

	  internal ObjectInfo(string name, string lib, string type, string status)
	  {
		name_ = name;
		library_ = lib;
		type_ = type;
		status_ = status;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_;
		  }
	  }

	  public virtual string Library
	  {
		  get
		  {
			return library_;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  public virtual string Status
	  {
		  get
		  {
			return status_;
		  }
	  }

	  internal virtual string TextDescription
	  {
		  set
		  {
			description_ = value;
		  }
		  get
		  {
			return description_;
		  }
	  }


	  internal virtual string Attribute
	  {
		  set
		  {
			attribute_ = value;
		  }
		  get
		  {
			return attribute_;
		  }
	  }


	  public override string ToString()
	  {
		return name_ + "  " + type_ + "  " + library_ + (!string.ReferenceEquals(attribute_, null) ? "  " + attribute_ : "") + (!string.ReferenceEquals(description_, null) ? "  " + description_.Trim() : "");
	  }

	  public virtual string toString2()
	  {
		return name_ + "  " + type_ + "  " + library_;
	  }
	}


}