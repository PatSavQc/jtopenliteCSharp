using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CallServiceProgramProcedure
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2014 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;


	/// <summary>
	/// Service program call - <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qzruclsp.htm">QZRUCLSP</a>
	/// This class fully implements the V5R4 specification of QZRUCLSP.  This API supports up to seven parameters.
	/// 
	/// 
	/// <para>This is designed so that classes to call a service program procedure can 
	/// be created by extending this class.  For examples, see the Direct Known Subclasses 
	/// information in the javadoc. 
	/// 
	/// </para>
	/// <para>A class that extends this class must implement the following methods corresponding to
	/// CallServiceProgramParameterFormat. 
	/// <ul>
	/// <li>void 	fillInputData(int index, byte[] dataBuffer, int offset) 
	/// <li>int 	getParameterCount() 
	/// <li>int 	getParameterFormat(int index) 
	/// <li>int 	getParameterLength(int index) 
	/// <li>void 	setOutputData(int index, byte[] dataBuffer, int offset) 
	/// </ul>
	/// </para>
	/// </summary>
	/// <seealso cref= com.ibm.jtopenlite.command.program.CallServiceProgramParameterFormat
	///  </seealso>
	public class CallServiceProgramProcedure : Program
	{
	  public const int RETURN_VALUE_FORMAT_NONE = 0;
	  public const int RETURN_VALUE_FORMAT_INTEGER = 1;
	  public const int RETURN_VALUE_FORMAT_POINTER = 2;
	  public const int RETURN_VALUE_FORMAT_INTEGER_AND_ERROR_NUMBER = 3;

	  private static readonly sbyte[] ZERO = new sbyte[8];

	  private string serviceProgramName_;
	  private string serviceProgramLibrary_;
	  private string exportName_;
	  private int returnValueFormat_;
	  private CallServiceProgramParameterFormat parameterFormat_;

	  private sbyte[] tempData_;

	  private int returnValueInteger_;
	  private sbyte[] returnValuePointer_;
	  private int returnValueErrno_;

	  public CallServiceProgramProcedure()
	  {
	  }

	  public CallServiceProgramProcedure(string serviceProgramName, string serviceProgramLibrary, string exportName, int returnValueFormat)
	  {
		serviceProgramName_ = serviceProgramName;
		serviceProgramLibrary_ = serviceProgramLibrary;
		exportName_ = exportName;
		returnValueFormat_ = returnValueFormat;
	  }

	  public virtual string ServiceProgramName
	  {
		  get
		  {
			return serviceProgramName_;
		  }
		  set
		  {
			serviceProgramName_ = value;
		  }
	  }


	  public virtual string ServiceProgramLibrary
	  {
		  get
		  {
			return serviceProgramLibrary_;
		  }
		  set
		  {
			serviceProgramLibrary_ = value;
		  }
	  }


	  public virtual string ExportName
	  {
		  get
		  {
			return exportName_;
		  }
		  set
		  {
			exportName_ = value;
		  }
	  }


	  public virtual int ReturnValueFormat
	  {
		  get
		  {
			return returnValueFormat_;
		  }
		  set
		  {
			returnValueFormat_ = value;
		  }
	  }


	  public virtual CallServiceProgramParameterFormat ParameterFormat
	  {
		  get
		  {
			return parameterFormat_;
		  }
		  set
		  {
			parameterFormat_ = value;
		  }
	  }


	  public virtual string ProgramName
	  {
		  get
		  {
			return "QZRUCLSP";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 7 + parameterFormat_.ParameterCount;
		  }
	  }

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

	  public virtual void newCall()
	  {
		returnValueInteger_ = 0;
		returnValuePointer_ = null;
		returnValueErrno_ = 0;
	  }

	  public virtual int ReturnValueInteger
	  {
		  get
		  {
			return returnValueInteger_;
		  }
	  }

	  public virtual int ReturnValueErrorNumber
	  {
		  get
		  {
			return returnValueErrno_;
		  }
	  }

	  public virtual sbyte[] ReturnValuePointer
	  {
		  get
		  {
			return returnValuePointer_;
		  }
	  }

	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 20;
		  case 1:
			  return exportName_.Length + 1;
		  case 2:
			  return 4;
		  case 3:
			  return parameterFormat_.ParameterCount * 4;
		  case 4:
			  return 4;
		  case 5:
			// Align to 16-byte boundary for those programs that need it.
			int total = 20; // Service program.
			total += exportName_.Length + 1; // Export name.
			total += 4; // Return value format.
			total += (parameterFormat_.ParameterCount * 4);
			total += 4; // Number of parameters.
			total += ZERO.Length;
			int val = 0;
			switch (returnValueFormat_)
			{
			  case 0:
				val = 4;
				break;
			  case 1:
				val = 4;
				break;
			  case 2:
				val = 16;
				break;
			  case 3:
				val = 8;
				break;
			}
			total += val;
			int mod = total % 16;
			int extra = (mod == 0 ? 0 : 16 - mod);
			return ZERO.Length + extra;

		  case 7:
			  return parameterFormat_.getParameterLength(0);
		  case 8:
			  return parameterFormat_.getParameterLength(1);
		  case 9:
			  return parameterFormat_.getParameterLength(2);
		  case 10:
			  return parameterFormat_.getParameterLength(3);
		  case 11:
			  return parameterFormat_.getParameterLength(4);
		  case 12:
			  return parameterFormat_.getParameterLength(5);
		  case 13:
			  return parameterFormat_.getParameterLength(6);
		}
		return 0;
	  }

	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 5:
			  return getParameterInputLength(parmIndex);
		  case 6:
			switch (returnValueFormat_)
			{
			  case 0:
				  return 4; // Ignored.
			  case 1:
				  return 4; // Integer.
			  case 2:
				  return 16; // Pointer.
			  case 3:
				  return 8; // Integer + ERRNO.
			}
		  case 7:
			  return parameterFormat_.getParameterLength(0);
		  case 8:
			  return parameterFormat_.getParameterLength(1);
		  case 9:
			  return parameterFormat_.getParameterLength(2);
		  case 10:
			  return parameterFormat_.getParameterLength(3);
		  case 11:
			  return parameterFormat_.getParameterLength(4);
		  case 12:
			  return parameterFormat_.getParameterLength(5);
		  case 13:
			  return parameterFormat_.getParameterLength(6);
		}
		return 0;
	  }

	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
		  case 1:
		  case 2:
		  case 3:
		  case 4:
			return Parameter.TYPE_INPUT;
		  case 6:
			return Parameter.TYPE_OUTPUT;
		}
		return Parameter.TYPE_INPUT_OUTPUT;
	  }

	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
		sbyte[] tempData = TempDataBuffer;
		switch (parmIndex)
		{
		  case 0:
			Conv.stringToBlankPadEBCDICByteArray(serviceProgramName_, tempData, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(serviceProgramLibrary_, tempData, 10, 10);
			break;
		  case 1:
			int len = Conv.stringToEBCDICByteArray37(exportName_, tempData, 0);
			tempData[len] = 0;
			break;
		  case 2:
			Conv.intToByteArray(returnValueFormat_, tempData, 0);
			break;
		  case 3:
			for (int i = 0; i < parameterFormat_.ParameterCount; ++i)
			{
			  Conv.intToByteArray(parameterFormat_.getParameterFormat(i), tempData, i * 4);
			}
			break;
		  case 4:
			Conv.intToByteArray(parameterFormat_.ParameterCount, tempData, 0);
			break;
		  case 5:
			for (int i = 0; i < 16; ++i)
			{
				tempData[i] = 0;
			}
			break;
		  case 7:
		  case 8:
		  case 9:
		  case 10:
		  case 11:
		  case 12:
		  case 13:
			parameterFormat_.fillInputData(parmIndex - 7, tempData, 0);
			break;
		}
		return tempData;
	  }


	  public virtual void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 6:
			switch (returnValueFormat_)
			{
			  case 0: // Ignore.
				break;
			  case 1: // Integer.
				returnValueInteger_ = Conv.byteArrayToInt(tempData, 0);
				break;
			  case 2: // Pointer.
				returnValuePointer_ = new sbyte[16];
				Array.Copy(tempData, 0, returnValuePointer_, 0, 16);
				break;
			  case 3: // Integer + ERRNO.
				returnValueInteger_ = Conv.byteArrayToInt(tempData, 0);
				returnValueErrno_ = Conv.byteArrayToInt(tempData, 4);
				break;
			}
			break;
		  case 7:
		  case 8:
		  case 9:
		  case 10:
		  case 11:
		  case 12:
		  case 13:
			parameterFormat_.setOutputData(parmIndex - 7, tempData, 0);
			break;
		}
	  }
	}

}