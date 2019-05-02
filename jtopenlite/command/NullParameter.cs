///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  NullParameter.java
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
	/// Represents a program parameter that is null.
	/// 
	/// </summary>
	public class NullParameter : Parameter
	{
	  /// <summary>
	  /// The instance to use when you need to specify a null program parameter.
	  /// 
	  /// </summary>
	  public static readonly NullParameter INSTANCE = new NullParameter();

	  private NullParameter() : base(TYPE_NULL)
	  {
	  }
	}


}