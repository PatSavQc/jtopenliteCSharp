using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  PortMapper.java
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
	/// Connects to the System i Port Mapper host server (QSYSWRK/QZSOMAPD daemon job) listening on TCP port 449.
	/// 
	/// </summary>
	public class PortMapper
	{
	  /// <summary>
	  /// Constant representing the System i Signon Host Server.
	  /// 
	  /// </summary>
	  public const string SIGNON_SERVICE = "as-signon";

	  /// <summary>
	  /// Constant representing the System i Remote Command Host Server.
	  /// 
	  /// </summary>
	  public const string COMMAND_SERVICE = "as-rmtcmd";

	  /// <summary>
	  /// Constant representing the System i DDM/DRDA Host Server.
	  /// 
	  /// </summary>
	  public const string DDM_SERVICE = "drda"; //"as-ddm";

	  /// <summary>
	  /// Constant representing the System i File Host Server.
	  /// 
	  /// </summary>
	  public const string FILE_SERVICE = "as-file";

	  /// <summary>
	  /// Constant representing the System i Database Host Server.
	  /// 
	  /// </summary>
	  public const string DATABASE_SERVICE = "as-database";

	  private PortMapper()
	  {
	  }

	  /// <summary>
	  /// Issues a request to the Port Mapper host server on the specified system
	  /// to determine the TCP/IP port the specified service is listening on.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int getPort(String system, String service) throws IOException
	  public static int getPort(string system, string service)
	  {
		Socket portMapper = new Socket(system, 449);
		Stream @in = portMapper.InputStream;
		Stream @out = portMapper.OutputStream;
		try
		{
		  // Port mapper request.
		  sbyte[] serviceName = service.GetBytes("ISO_8859-1");
		  @out.Write(serviceName, 0, serviceName.Length);
		  @out.Flush();

		  // Unused variable
		  // int portNum = -1;

		  int i = @in.Read();
		  if (i == 0x002B)
		  {
			// unused variable
			// int num = 0;
			int b1 = @in.Read();
			if (b1 < 0)
			{
				throw new EOFException();
			}
			int b2 = @in.Read();
			if (b2 < 0)
			{
				throw new EOFException();
			}
			int b3 = @in.Read();
			if (b3 < 0)
			{
				throw new EOFException();
			}
			int b4 = @in.Read();
			if (b4 < 0)
			{
				throw new EOFException();
			}
			return ((b1 & 0x00FF) << 24) | ((b2 & 0x00FF) << 16) | ((b3 & 0x00FF) << 8) | (b4 & 0x00FF);
		  }
		  else
		  {
			throw new IOException("Bad result from port mapper: " + i);
		  }
		}
		finally
		{
		  // The port mapper host server only handles one request per socket, apparently.
		  @in.Close();
		  @out.Close();
		  portMapper.close();
		}
	  }
	}


}