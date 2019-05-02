///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ProgramAdapter.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command.program
{
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Base class for all program call classes in this package.
	/// 
	/// </summary>
	public abstract class ProgramAdapter : Program
	{
	  internal static readonly sbyte[] ZERO = new sbyte[4];

	  private string library_;
	  private string name_;
	  private int numParms_;

	  private sbyte[] tempData_;

	  protected internal ProgramAdapter(string library, string name, int numberOfParameters)
	  {
		library_ = library;
		name_ = name;
		numParms_ = numberOfParameters;
	  }

	  public void newCall()
	  {
		clearOutputData();
	  }

	  internal abstract void clearOutputData();

	  public string ProgramLibrary
	  {
		  get
		  {
			return library_;
		  }
	  }

	  public string ProgramName
	  {
		  get
		  {
			return name_;
		  }
	  }

	  public int NumberOfParameters
	  {
		  get
		  {
			return NumberOfParametersSubclass;
		  }
	  }

	  internal virtual int NumberOfParametersSubclass
	  {
		  get
		  {
			return numParms_;
		  }
	  }

	  public int getParameterType(int parmIndex)
	  {
		return getParameterTypeSubclass(parmIndex);
	  }

	  internal abstract int getParameterTypeSubclass(int parmIndex);

	  public int getParameterInputLength(int parmIndex)
	  {
		return getParameterInputLengthSubclass(parmIndex);
	  }

	  internal abstract int getParameterInputLengthSubclass(int parmIndex);

	  public int getParameterOutputLength(int parmIndex)
	  {
		return getParameterOutputLengthSubclass(parmIndex);
	  }

	  internal abstract int getParameterOutputLengthSubclass(int parmIndex);

	//  public final void writeParameterInputData(HostOutputStream out, int parmIndex) throws IOException
	//  {
	//    writeParameterInputDataSubclass(out, parmIndex);
	//  }

	  public sbyte[] getParameterInputData(int parmIndex)
	  {
		return getParameterInputDataSubclass(parmIndex);
	  }

	//  abstract void writeParameterInputDataSubclass(HostOutputStream out, int parmIndex) throws IOException;

	  internal abstract sbyte[] getParameterInputDataSubclass(int parmIndex);

	//  public final void readParameterOutputData(HostInputStream in, int parmIndex, int maxLength) throws IOException
	//  {
	//    readParameterOutputDataSubclass(in, parmIndex, maxLength);
	//  }

	  public void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
		setParameterOutputDataSubclass(parmIndex, tempData, maxLength);
	  }

	//  abstract void readParameterOutputDataSubclass(HostInputStream in, int parmIndex, int maxLength) throws IOException;

	  internal abstract void setParameterOutputDataSubclass(int parmIndex, sbyte[] tempData, int maxLength);

	  public sbyte[] TempDataBuffer
	  {
		  get
		  {
			int maxSize = 0;
			for (int i = 0; i < NumberOfParameters; ++i)
			{
			  int len = getParameterOutputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			  len = getParameterInputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			}
			if (tempData_ == null || tempData_.Length < maxSize)
			{
			  tempData_ = new sbyte[maxSize];
			}
			return tempData_;
		  }
	  }
	}


}