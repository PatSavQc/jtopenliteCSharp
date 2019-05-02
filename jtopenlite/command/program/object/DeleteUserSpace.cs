///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DeleteUserSpace.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.@object
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;


	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qusdltus.htm">QUSDLTUS</a>
	/// 
	/// </summary>
	public class DeleteUserSpace : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private readonly sbyte[] tempData_ = new sbyte[20];

	  private string userSpaceName_;
	  private string userSpaceLibrary_;

	  public DeleteUserSpace(string userSpaceName, string userSpaceLibrary)
	  {
		userSpaceName_ = userSpaceName;
		userSpaceLibrary_ = userSpaceLibrary;
	  }

	  public virtual string UserSpaceName
	  {
		  get
		  {
			return userSpaceName_;
		  }
		  set
		  {
			userSpaceName_ = value;
		  }
	  }


	  public virtual string UserSpaceLibrary
	  {
		  get
		  {
			return userSpaceLibrary_;
		  }
		  set
		  {
			userSpaceLibrary_ = value;
		  }
	  }


	  public virtual void newCall()
	  {
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 2;
		  }
	  }

	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 20;
		  case 1:
			  return 4;
		}
		return 0;
	  }

	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 1:
			  return 4;
		}
		return 0;
	  }

	  public virtual int getParameterType(int parmIndex)
	  {
		return (parmIndex == 1 ? Parameter.TYPE_INPUT_OUTPUT : Parameter.TYPE_INPUT);
	  }

	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			Conv.stringToBlankPadEBCDICByteArray(userSpaceName_, tempData_, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(userSpaceLibrary_, tempData_, 10, 10);
			break;
		  case 1:
			return ZERO;
		}
		return tempData_;
	  }


	  public virtual sbyte[] TempDataBuffer
	  {
		  get
		  {
			return tempData_;
		  }
	  }

	  public virtual void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QUSDLTUS";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }
	}


}