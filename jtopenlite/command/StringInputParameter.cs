///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  StringInputParameter.java
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
	/// A specific kind of program parameter that represents a CCSID 37 String value as input.
	/// 
	/// </summary>
	public class StringInputParameter : InputParameter
	{
	  /// <summary>
	  /// Constructs a parameter using the provided value as the input data.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StringInputParameter(String s) throws IOException
	  public StringInputParameter(string s) : base(Conv.stringToEBCDICByteArray(s, 37))
	  {
		// super(s.getBytes("Cp037"));
	  }
	}

}