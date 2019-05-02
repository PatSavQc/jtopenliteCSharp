///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  IntegerInputParameter.java
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
	/// A specific kind of program parameter that represents a 4-byte integer value used as input.
	/// 
	/// </summary>
	public class IntegerInputParameter : InputParameter
	{
	  /// <summary>
	  /// Constructs a parameter using the provided value as the input data.
	  /// 
	  /// </summary>
	  public IntegerInputParameter(int val) : base(Conv.intToByteArray(val))
	  {
	  }
	}

}