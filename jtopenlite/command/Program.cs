///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Program.java
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
	/// Used by classes that wish to implement a program call, this essentially represents a System i program (PGM).
	/// The <seealso cref="CommandConnection#call(Program) CommandConnection.call()"/> method will internally call the methods
	/// on this interface when it needs information about the Program being called.
	/// <para>
	/// The current order of operations (subject to change) that CommandConnection uses when call(Program) is invoked is as follows:
	/// <ol>
	/// <li>CommandConnection.call(program)</li>
	/// <li>--&gt; program.newCall()</li>
	/// <li>--&gt; program.getNumberOfParameters()</li>
	/// <li>--&gt; begin loop</li>
	/// <li>------&gt; program.getParameterInputLength()</li>
	/// <li>--&gt; end loop</li>
	/// <li>--&gt; program.getProgramName()</li>
	/// <li>--&gt; program.getProgramLibrary()</li>
	/// <li>--&gt; begin loop</li>
	/// <li>------&gt; program.getParameterInputLength()</li>
	/// <li>------&gt; program.getParameterOutputLength()</li>
	/// <li>------&gt; program.getParameterType()</li>
	/// <li>------&gt; program.getParameterInputData()</li>
	/// <li>--&gt; end loop</li>
	/// <li>--&gt; program.getNumberOfParameters()</li>
	/// <li>--&gt; begin loop</li>
	/// <li>------&gt; program.getParameterOutputLength()</li>
	/// <li>------&gt; program.getTempDataBuffer()</li>
	/// <li>------&gt; program.setParameterOutputData()</li>
	/// <li>--&gt; end loop</li>
	/// </ol>
	/// </para>
	/// </summary>
	/// <seealso cref= CommandConnection#call(Program)
	///  </seealso>
	public interface Program
	{
	  /// <summary>
	  /// Invoked before any other methods on this interface by CommandConnection whenever this Program is called.
	  /// 
	  /// </summary>
	  void newCall();

	  /// <summary>
	  /// Returns the number of parameters for this program.
	  /// 
	  /// </summary>
	  int NumberOfParameters {get;}

	  /// <summary>
	  /// Returns the input length of the parameter at the specified index.
	  /// 
	  /// </summary>
	  int getParameterInputLength(int parmIndex);

	  /// <summary>
	  /// Returns the output length of the parameter at the specified index.
	  /// 
	  /// </summary>
	  int getParameterOutputLength(int parmIndex);

	  /// <summary>
	  /// Returns the type of parameter at the specified index. </summary>
	  /// <seealso cref= Parameter
	  ///  </seealso>
	  int getParameterType(int parmIndex);

	  /// <summary>
	  /// Returns the input data of the parameter at the specified index.
	  /// 
	  /// </summary>
	  sbyte[] getParameterInputData(int parmIndex);

	  /// <summary>
	  /// The implementor can create their own temp byte array for the output parameter size and reuse it each time a call is performed,
	  /// or for more than one parameter on the same call.
	  /// The implementor can choose to ignore this, and simply return null. The command connection checks to see if the
	  /// buffer returned by this method is not null and large enough to accommodate the output parameter size.
	  /// 
	  /// </summary>
	  sbyte[] TempDataBuffer {get;}

	  /// <summary>
	  /// Sets the output data for the parameter at the specified index.
	  /// 
	  /// </summary>
	  void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength);

	  /// <summary>
	  /// Returns the name of the program object.
	  /// 
	  /// </summary>
	  string ProgramName {get;}

	  /// <summary>
	  /// Returns the library of the program object.
	  /// 
	  /// </summary>
	  string ProgramLibrary {get;}
	}


}