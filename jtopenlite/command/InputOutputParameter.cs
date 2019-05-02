///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  InputOutputParameter.java
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
	/// Represents a program parameter to be used as both input and output.
	/// 
	/// </summary>
	public class InputOutputParameter : Parameter
	{
	  private int outputLength_;
	  private sbyte[] inputData_;
	  private sbyte[] outputData_;

	  /// <summary>
	  /// Constructs a parameter with the provided input data and output length.
	  /// 
	  /// </summary>
	  public InputOutputParameter(sbyte[] inputData, int outputLength) : base(TYPE_INPUT_OUTPUT)
	  {
		inputData_ = inputData;
		outputLength_ = outputLength;
	  }

	  /// <summary>
	  /// Returns the input data.
	  /// 
	  /// </summary>
	  public override sbyte[] InputData
	  {
		  get
		  {
			return inputData_;
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
			return inputData_.Length;
		  }
	  }

	  /// <summary>
	  /// Returns the output length.
	  /// 
	  /// </summary>
	  public override int OutputLength
	  {
		  get
		  {
			return outputLength_;
		  }
	  }

	  /// <summary>
	  /// Returns the maximum of the output length and input length of this parameter.
	  /// 
	  /// </summary>
	  public override int MaxLength
	  {
		  get
		  {
			return outputLength_ > inputData_.Length ? outputLength_ : inputData_.Length;
		  }
	  }

	  protected internal override sbyte[] OutputData
	  {
		  set
		  {
			outputData_ = value;
		  }
		  get
		  {
			return outputData_;
		  }
	  }

	}

}