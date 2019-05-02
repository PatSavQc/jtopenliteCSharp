///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Parameter.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command
{
	using com.ibm.jtopenlite;

	/// <summary>
	/// Abstract parent class for all program parameters.
	/// 
	/// </summary>
	public abstract class Parameter
	{
	  /// <summary>
	  /// Constant representing a null parameter.
	  /// 
	  /// </summary>
	  public const int TYPE_NULL = 0;

	  /// <summary>
	  /// Constant representing an input parameter.
	  /// 
	  /// </summary>
	  public const int TYPE_INPUT = 1;

	  /// <summary>
	  /// Constant representing an output parameter.
	  /// 
	  /// </summary>
	  public const int TYPE_OUTPUT = 2;

	  /// <summary>
	  /// Constant representing an input/output parameter.
	  /// 
	  /// </summary>
	  public const int TYPE_INPUT_OUTPUT = 3;

	  private int type_;

	  protected internal Parameter(int type)
	  {
		type_ = type;
	  }

	  /// <summary>
	  /// Returns the input data for this parameter.
	  /// The default is null.
	  /// 
	  /// </summary>
	  public virtual sbyte[] InputData
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// Returns the input length of this parameter.
	  /// The default is 0.
	  /// 
	  /// </summary>
	  public virtual int InputLength
	  {
		  get
		  {
			return 0;
		  }
	  }

	  /// <summary>
	  /// Returns the output length of this parameter.
	  /// The default is 0.
	  /// 
	  /// </summary>
	  public virtual int OutputLength
	  {
		  get
		  {
			return 0;
		  }
	  }

	  /// <summary>
	  /// Returns the maximum length of this parameter.
	  /// The default is 0.
	  /// 
	  /// </summary>
	  public virtual int MaxLength
	  {
		  get
		  {
			return 0;
		  }
	  }

	  protected internal virtual sbyte[] OutputData
	  {
		  set
		  {
		  }
		  get
		  {
			return null;
		  }
	  }


	  /// <summary>
	  /// Returns true if the type of this parameter is input or input/output.
	  /// 
	  /// </summary>
	  public virtual bool Input
	  {
		  get
		  {
			return type_ == TYPE_INPUT || type_ == TYPE_INPUT_OUTPUT;
		  }
	  }

	  /// <summary>
	  /// Returns true if the type of this parameter is output or input/output.
	  /// 
	  /// </summary>
	  public virtual bool Output
	  {
		  get
		  {
			return type_ == TYPE_OUTPUT || type_ == TYPE_INPUT_OUTPUT;
		  }
	  }

	  /// <summary>
	  /// Returns the type of this parameter.
	  /// 
	  /// </summary>
	  public virtual int Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  /// <summary>
	  /// Convenience method to retrieve the 4-byte integer value in the output data at the specified offset.
	  /// 
	  /// </summary>
	  public virtual int parseInt(int offset)
	  {
		return Conv.byteArrayToInt(OutputData, offset);
	  }

	  /// <summary>
	  /// Convenience method to retrieve the CCSID 37 String in the output data at the specified offset and length.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String parseString(int offset, int length) throws IOException
	  public virtual string parseString(int offset, int length)
	  {
		// return new String(getOutputData(), offset, length, "Cp037");
		return Conv.ebcdicByteArrayToString(OutputData, offset, length);
	  }
	}


}