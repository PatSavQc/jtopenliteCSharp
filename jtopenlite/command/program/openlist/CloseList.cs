///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: CloseList.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{

	using com.ibm.jtopenlite.command;


	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qgyclst.htm">QGYCLST</a>
	/// 
	/// </summary>
	public class CloseList : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];
	  private readonly sbyte[] tempData_ = new sbyte[10];

	  private sbyte[] requestHandle_;

	  public CloseList()
	  {
	  }

	  public CloseList(sbyte[] requestHandle)
	  {
	//    super("QGY", "QGYCLST", 2);

		requestHandle_ = requestHandle;
	  }

	  public virtual sbyte[] TempDataBuffer
	  {
		  get
		  {
			return tempData_;
		  }
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QGYCLST";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QGY";
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 2;
		  }
	  }

	  public virtual void newCall()
	  {
	  }

	  public virtual sbyte[] RequestHandle
	  {
		  get
		  {
			return requestHandle_;
		  }
		  set
		  {
			requestHandle_ = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterInputLength(final int parmIndex)
	  public virtual int getParameterInputLength(int parmIndex)
	  {
		return 4;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterOutputLength(final int parmIndex)
	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		return parmIndex == 1 ? 4 : 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		return parmIndex == 1 ? Parameter.TYPE_INPUT_OUTPUT : Parameter.TYPE_INPUT;
	  }

	//  void writeParameterInputDataSubclass(final HostOutputStream out, final int parmIndex) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public byte[] getParameterInputData(final int parmIndex)
	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return requestHandle_;
		  case 1:
			  return ZERO;
		}
		return null;
	  }

	//  void readParameterOutputDataSubclass(final HostInputStream in, final int parmIndex, final int maxLength) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] tempData, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
	//    in.skipBytes(maxLength);
	  }
	}


}