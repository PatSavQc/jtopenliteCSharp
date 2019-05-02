using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Trace.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	/// <summary>
	/// Class representing the Tracing provided by JTOpenLite.
	/// 
	/// <para>Trace can be enabled by setting the JVM property com.ibm.jtopenlite.Trace.category to ALL or TRUE.
	/// </para>
	/// <para>The trace output can be directed to a file by setting the JVM property com.ibm.jtopenlite.Trace.file.
	/// </para>
	/// <para>The trace currently consists only of the datastream information.
	/// </para>
	/// </summary>
	public class Trace
	{

	  public const int CATEGORY_NONE = 0x00;
	  public const int CATEGORY_DATASTREAM = 0x01;
	  public const int CATEGORY_ALL = 0xFF;

	  internal static int traceCategory_ = CATEGORY_NONE;

	  //internal static PrintStream printStream = System.out;

	  static Trace()
	  {
		  string category = System.getProperty("com.ibm.jtopenlite.Trace.category");

		  string filename = System.getProperty("com.ibm.jtopenlite.Trace.file");
		  if (!string.ReferenceEquals(filename, null))
		  {
			File file = new File(filename);
			PrintFile = file;
		  }

		  if (!string.ReferenceEquals(category, null))
		  {
			category = category.ToUpper().Trim();
			if (category.Equals("TRUE") || category.Equals("ALL"))
			{
			  traceCategory_ = CATEGORY_ALL;
			  Console.WriteLine("Tracing " + About.INTERFACE_NAME + " : level " + About.INTERFACE_LEVEL);
			  //if (printStream == System.out)
			  //{
				//printStream.println("Tracing " + About.INTERFACE_NAME + " : level " + About.INTERFACE_LEVEL);
			  //}
			}
		  }

	  }



	  public static int TraceCategory
	  {
		  set
		  {
			traceCategory_ = value;
		  }
		  get
		  {
			return traceCategory_;
		  }
	  }


//	  public static File PrintFile
//	  {
//		  set
//		  {
//			 try
//			 {
//			   printStream = new PrintStream(value);
//			   printStream.println("Tracing " + About.INTERFACE_NAME + " : level " + About.INTERFACE_LEVEL);
//			 }
//			 catch (Exception e)
//			 {
//			   dumpException(e);
//			 }
//		  }
//	  }

//	  public static PrintStream PrintStream
//	  {
//		  get
//		  {
//			return printStream;
//		  }
//	  }

	  public static bool StreamTracingEnabled
	  {
		  get
		  {
			return (traceCategory_ & CATEGORY_DATASTREAM) != 0;
		  }
	  }

	  public static void dumpException(Exception e)
	  {
		if (printStream != null)
		{
		   e.printStackTrace(printStream);
		}
	  }

	}

}