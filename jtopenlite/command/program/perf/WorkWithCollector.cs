///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: WorkWithCollector.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.perf
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;


	/// <summary>
	/// Obtain performance data using the <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/QPMWKCOL.htm">QPMWKCOL</a>
	/// API. 
	/// 
	/// </summary>
	public class WorkWithCollector : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const string ACTION_START = "*START";
	  public const string ACTION_END = "*END";
	  public const string ACTION_CHANGE = "*CHANGE";

	  public const string RESOURCE_JOB = "*JOB";
	  public const string RESOURCE_POOL = "*POOL";
	  public const string RESOURCE_DISK = "*DISK";
	  public const string RESOURCE_IOP = "*IOP";
	  public const string RESOURCE_COMM = "*COMM";

	  public const int COLLECT_15 = 15;
	  public const int COLLECT_30 = 30;
	  public const int COLLECT_60 = 60;
	  public const int COLLECT_120 = 120;
	  public const int COLLECT_240 = 240;

	  private string typeOfActionToPerform_;
	  private string typeOfResource_;
	  private int timeBetweenCollections_;
	  private string userSpaceName_;
	  private string userSpaceLibrary_;

	  private int firstSequenceNumber_ = -1;

	  private readonly sbyte[] tempData_ = new sbyte[20];

	  public WorkWithCollector(string typeOfActionToPerform, string typeOfResource, int timeBetweenCollections, string userSpaceName, string userSpaceLibrary)
	  {
		typeOfActionToPerform_ = typeOfActionToPerform;
		typeOfResource_ = typeOfResource;
		timeBetweenCollections_ = timeBetweenCollections;
		userSpaceName_ = userSpaceName;
		userSpaceLibrary_ = userSpaceLibrary;
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QPMWKCOL";
		  }
	  }

	  public virtual void newCall()
	  {
		firstSequenceNumber_ = -1;
	  }

	  public virtual string TypeOfActionToPerform
	  {
		  set
		  {
			typeOfActionToPerform_ = value;
		  }
		  get
		  {
			return typeOfActionToPerform_;
		  }
	  }


	  public virtual string TypeOfResource
	  {
		  set
		  {
			typeOfResource_ = value;
		  }
		  get
		  {
			return typeOfResource_;
		  }
	  }


	  public virtual int TimeBetweenCollections
	  {
		  set
		  {
			timeBetweenCollections_ = value;
		  }
		  get
		  {
			return timeBetweenCollections_;
		  }
	  }


	  public virtual string UserSpaceName
	  {
		  set
		  {
			userSpaceName_ = value;
		  }
		  get
		  {
			return userSpaceName_;
		  }
	  }


	  public virtual string UserSpaceLibrary
	  {
		  set
		  {
			userSpaceLibrary_ = value;
		  }
		  get
		  {
			return userSpaceLibrary_;
		  }
	  }


	  public virtual int FirstSequenceNumber
	  {
		  get
		  {
			return firstSequenceNumber_;
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 6;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterInputLength(final int parmIndex)
	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 10;
		  case 1:
			  return 10;
		  case 2:
			  return 4;
		  case 3:
			  return 20;
		  case 5:
			  return 4;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterOutputLength(final int parmIndex)
	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 4:
			  return 4;
		  case 5:
			  return 4;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 4:
			  return Parameter.TYPE_OUTPUT;
		  case 5:
			  return Parameter.TYPE_INPUT_OUTPUT;
		}
		return Parameter.TYPE_INPUT;
	  }

	  public virtual sbyte[] TempDataBuffer
	  {
		  get
		  {
			return tempData_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public byte[] getParameterInputData(final int parmIndex)
	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  Conv.stringToBlankPadEBCDICByteArray(typeOfActionToPerform_, tempData_, 0, 10);
			  return tempData_;
		  case 1:
			  Conv.stringToBlankPadEBCDICByteArray(typeOfResource_, tempData_, 0, 10);
			  return tempData_;
		  case 2:
			  Conv.intToByteArray(timeBetweenCollections_, tempData_, 0);
			  return tempData_;
		  case 3:
			Conv.stringToBlankPadEBCDICByteArray(userSpaceName_, tempData_, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(userSpaceLibrary_, tempData_, 10, 10);
			return tempData_;
		  case 5:
			  return ZERO;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 4:
			firstSequenceNumber_ = Conv.byteArrayToInt(data, 0);
			break;
		  default:
			break;
		}
	  }
	}


}