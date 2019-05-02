///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  InputParameter.java
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
	/// Represents a program parameter to be used as input.
	/// 
	/// </summary>
	public class InputParameter : Parameter
	{
	  private sbyte[] data_;

	  /// <summary>
	  /// Constructs a parameter with the provided input data.
	  /// 
	  /// </summary>
	  public InputParameter(sbyte[] inputData) : base(TYPE_INPUT)
	  {
		data_ = inputData;
	  }

	  /// <summary>
	  /// Returns the input data.
	  /// 
	  /// </summary>
	  public override sbyte[] InputData
	  {
		  get
		  {
			return data_;
		  }
	  }

	  /// <summary>
	  /// Returns the input length.
	  /// 
	  /// </summary>
	  public override int InputLength
	  {
		  get
		  {
			return data_.Length;
		  }
	  }

	  /// <summary>
	  /// Returns the maximum length of this parameter, which is the input length.
	  /// 
	  /// </summary>
	  public override int MaxLength
	  {
		  get
		  {
			return InputLength;
		  }
	  }
	}


}