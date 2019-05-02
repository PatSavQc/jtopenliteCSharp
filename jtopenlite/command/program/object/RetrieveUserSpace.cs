using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveUserSpace.java
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
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qusrtvus.htm">QUSRTVUS</a>
	/// 
	/// </summary>
	public class RetrieveUserSpace : Program
	{


	  private readonly sbyte[] tempData_ = new sbyte[20];

	  private string userSpaceName_;
	  private string userSpaceLibrary_;
	  private int startingPosition_;
	  private int lengthOfData_;

	  private sbyte[] contents_;

	  public RetrieveUserSpace(string userSpaceName, string userSpaceLibrary, int startingPosition, int lengthOfData)
	  {
		userSpaceName_ = userSpaceName;
		userSpaceLibrary_ = userSpaceLibrary;
		startingPosition_ = startingPosition;
		lengthOfData_ = lengthOfData;
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


	  public virtual int StartingPosition
	  {
		  get
		  {
			return startingPosition_;
		  }
		  set
		  {
			startingPosition_ = value;
		  }
	  }


	  public virtual int LengthOfData
	  {
		  get
		  {
			return lengthOfData_;
		  }
		  set
		  {
			lengthOfData_ = value;
		  }
	  }


	  public virtual sbyte[] Contents
	  {
		  get
		  {
			return contents_;
		  }
	  }

	  public virtual void newCall()
	  {
		contents_ = null;
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 4;
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
		  case 2:
			  return 4;
		}
		return 0;
	  }

	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 3:
			  return lengthOfData_;
		}
		return 0;
	  }

	  public virtual int getParameterType(int parmIndex)
	  {
		return (parmIndex == 3 ? Parameter.TYPE_OUTPUT : Parameter.TYPE_INPUT);
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
			Conv.intToByteArray(startingPosition_, tempData_, 0);
			break;
		  case 2:
			Conv.intToByteArray(lengthOfData_, tempData_, 0);
			break;
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
		contents_ = new sbyte[lengthOfData_];
		Array.Copy(tempData, 0, contents_, 0, lengthOfData_);
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QUSRTVUS";
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