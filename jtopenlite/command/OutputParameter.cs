///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  OutputParameter.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command
{
	/// <summary>
	/// Represents a program parameter to be used as output.
	/// 
	/// </summary>
	public class OutputParameter : Parameter
	{
	  private int length_;
	  private sbyte[] data_;

	  /// <summary>
	  /// Constructs a parameter with the specified output length.
	  /// 
	  /// </summary>
	  public OutputParameter(int outputLength) : base(TYPE_OUTPUT)
	  {
		length_ = outputLength;
	  }

	  /// <summary>
	  /// Returns the output length.
	  /// 
	  /// </summary>
	  public override int OutputLength
	  {
		  get
		  {
			return length_;
		  }
	  }

	  /// <summary>
	  /// Returns the maximum length of this parameter, which is the output length.
	  /// 
	  /// </summary>
	  public override int MaxLength
	  {
		  get
		  {
			return OutputLength;
		  }
	  }

	  protected internal override sbyte[] OutputData
	  {
		  set
		  {
			data_ = value;
		  }
		  get
		  {
			return data_;
		  }
	  }

	}

}