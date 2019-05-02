///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CallServiceProgramParameterFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2014 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program
{
	/// <summary>
	/// Interface used to utilize the parameters of a service program call. 
	/// See the implementing subclasses for examples. 
	/// </summary>
	public interface CallServiceProgramParameterFormat
	{

	  /// <summary>
	  /// This method is called to get the number of parameters used
	  /// by the service program procedure call. </summary>
	  /// <returns> parameterCount </returns>
	  int ParameterCount {get;}

	  /// <summary>
	  /// This method is used to obtain the length of the specified parameter
	  /// </summary>
	  /// <param name="index">  0-based parameter identifier. </param>
	  /// <returns> parameter length </returns>
	  int getParameterLength(int index);

	  /// <summary>
	  /// This method is used to obtain the format of the specified parameter
	  /// </summary>
	  /// <param name="index">  0-based parameter identifier. </param>
	  /// <returns> parameter format which is one of the following:  
	  /// PARAMETER_FORMAT_BY_VALUE
	  /// PARAMETER_FORMAT_BY_REFERENCE </returns>
	  int getParameterFormat(int index);

	  /// <summary>
	  /// This method is used to fill an output buffer with the parameter information
	  /// before the procedure is called. </summary>
	  /// <param name="index"> 0-based parameter identifier </param>
	  /// <param name="dataBuffer">  buffer containing the data </param>
	  /// <param name="offset">  offset to where the data should be placed </param>
	  void fillInputData(int index, sbyte[] dataBuffer, int offset);

	  /// <summary>
	  /// This method is used to set the internal value of the parameter from a dataBuffer </summary>
	  /// <param name="index"> 0-based parameter identifier </param>
	  /// <param name="dataBuffer"> buffer containing the data </param>
	  /// <param name="offset"> offset to where the data should be retrieved </param>
	  void setOutputData(int index, sbyte[] dataBuffer, int offset);
	}

	public static class CallServiceProgramParameterFormat_Fields
	{
	  public const int PARAMETER_FORMAT_BY_VALUE = 1;
	  public const int PARAMETER_FORMAT_BY_REFERENCE = 2;
	}


}