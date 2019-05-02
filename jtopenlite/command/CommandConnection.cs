using System.IO;
using System.Net.Sockets;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CommandConnection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.command
{
	using com.ibm.jtopenlite;
	//import com.ibm.jtopenlite.base.experimental.*;

	/// <summary>
	/// Represents a TCP/IP socket connection to the System i Remote Command host server 
	/// (QSYSWRK/QZRCSRVS prestart jobs).  This connection can be used to call programs
	/// and execute CL commands. 
	/// See <seealso cref="com.ibm.jtopenlite.samples.CallCommand"/> for sample program that uses CommandConnection.
	/// 
	/// </summary>
	public class CommandConnection : HostServerConnection
	{
	  /// <summary>
	  /// The default TCP/IP port the Remote Command host server listens on.
	  /// If your system has been configured to use a different port, use
	  /// the <seealso cref="PortMapper PortMapper"/> class to determine the port.
	  /// 
	  /// 
	  /// </summary>
		public const int DEFAULT_COMMAND_SERVER_PORT = 8475;
		public const int DEFAULT_SSL_COMMAND_SERVER_PORT = 9475;

	  private int ccsid_;
	  private int datastreamLevel_;
	  private string NLV_;

	  private CommandConnection(SystemInfo info, Socket socket, HostInputStream @in, HostOutputStream @out, int ccsid, int datastreamLevel, string user, string jobName, string NLV) : base(info, user, jobName, socket, @in, @out)
	  {
		ccsid_ = ccsid;
		datastreamLevel_ = datastreamLevel;
		NLV_ = NLV;
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendEndJobRequest() throws IOException
	  protected internal override void sendEndJobRequest()
	  {
		out_.writeInt(20); // Length is 40 if server ID is E004.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE008); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(0); // Correlation ID.
		out_.writeShort(0); // Template length.
		out_.writeShort(0x1004); // ReqRep ID for remote command server.
		out_.flush();
	  }

	  /// <summary>
	  /// Get the NLV used by the connection </summary>
	  /// <returns> NLV used by the connection </returns>
	  public virtual string NLV
	  {
		  get
		  {
				return NLV_;
		  }
	  }
	/// <summary>
	/// Get the CCSID used by the connection </summary>
	/// <returns> CCSID used by the connection  </returns>
	  public virtual int Ccsid
	  {
		  get
		  {
				return ccsid_;
		  }
	  }

	  /// <summary>
	  /// Connects to the Remote Command host server on the default port after first connecting
	  /// to the Signon host server and authenticating the specified user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(String system, String user, String password) throws IOException
	  public static CommandConnection getConnection(string system, string user, string password)
	  {
		return getConnection(false, system, user, password);
	  }

	  /// <summary>
	  /// Connects to the Remote Command host server on the default port after first connecting
	  /// to the Signon host server and authenticating the specified user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(final boolean isSSL, String system, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static CommandConnection getConnection(bool isSSL, string system, string user, string password)
	  {
		SignonConnection conn = SignonConnection.getConnection(isSSL, system, user, password);
		try
		{
		  return getConnection(isSSL, conn.Info, user, password);
		}
		finally
		{
		  conn.close();
		}
	  }

	  /// <summary>
	  /// Connects to the Remote Command host server on the default port using the specified system information and user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(SystemInfo info, String user, String password) throws IOException
	  public static CommandConnection getConnection(SystemInfo info, string user, string password)
	  {
		return getConnection(false, info, user, password);
	  }
	  /// <summary>
	  /// Connects to the Remote Command host server on the default port using the specified system information and user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static CommandConnection getConnection(bool isSSL, SystemInfo info, string user, string password)
	  {
		return getConnection(isSSL, info, user, password, isSSL ? DEFAULT_SSL_COMMAND_SERVER_PORT :DEFAULT_COMMAND_SERVER_PORT, false);
	  }

	  /// <summary>
	  /// Connects to the Remote Command host server on the specified port using the specified system information and user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(SystemInfo info, String user, String password, int commandPort, boolean compress) throws IOException
	  public static CommandConnection getConnection(SystemInfo info, string user, string password, int commandPort, bool compress)
	  {
		  return getConnection(false, info, user, password, commandPort, compress);
	  }
	  /// <summary>
	  /// Connects to the Remote Command host server on the specified port using the specified system information and user.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CommandConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password, int commandPort, boolean compress) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static CommandConnection getConnection(bool isSSL, SystemInfo info, string user, string password, int commandPort, bool compress)
	  {
		if (commandPort < 0 || commandPort > 65535)
		{
		  throw new IOException("Bad command port: " + commandPort);
		}
		CommandConnection conn = null;

		Socket commandServer = isSSL? SSLSocketFactory.Default.createSocket(info.System, commandPort) : new Socket(info.System, commandPort);
		Stream @in = commandServer.InputStream;
		Stream @out = commandServer.OutputStream;
		try
		{
		  if (compress)
		  {
			throw new IOException("Experimental compression streams not enabled.");
	//        in = new CompressionInputStream(in);
	//        out = new CompressionOutputStream(new BufferedOutputStream(out));
		  }

		  // Exchange random seeds.
		  HostOutputStream dout = new HostOutputStream(compress ? @out : new BufferedOutputStream(@out));
		  HostInputStream din = new HostInputStream(new BufferedInputStream(@in));
		  string jobName = connect(info, dout, din, 0xE008, user, password);

		  string NLV = Conv.DefaultNLV;
		  sendExchangeAttributesRequest(dout, NLV);
		  dout.flush();

		  int length = din.readInt();
		  if (length < 20)
		  {
			throw DataStreamException.badLength("commandExchangeAttributes", length);
		  }
		  din.skipBytes(16);
		  int rc = din.readShort();
		  // We ignore the same return codes that JTOPEN ignores
		  if (rc != 0x0100 && rc != 0x0104 && rc != 0x0105 && rc != 0x0106 && rc != 0x0107 && rc != 0x0108 && rc != 0)
		  {
			throw DataStreamException.badReturnCode("commandExchangeAttributes", rc);
		  }
		  int ccsid = din.readInt();
		  sbyte[] nlvBytes = new sbyte[4];
		  nlvBytes[0] = (sbyte) din.readByte();
		  nlvBytes[1] = (sbyte) din.readByte();
		  nlvBytes[2] = (sbyte) din.readByte();
		  nlvBytes[3] = (sbyte) din.readByte();
		  NLV = Conv.ebcdicByteArrayToString(nlvBytes, 0, 4);
		  // Server version 
		  din.skipBytes(4);

		  int datastreamLevel = din.readShort();
		  din.skipBytes(length - 36);
		  din.end();

		  conn = new CommandConnection(info, commandServer, din, dout, ccsid, datastreamLevel, user, jobName, NLV);
		  return conn;
		}
		finally
		{
		  if (conn == null)
		  {
			@in.Close();
			@out.Close();
			commandServer.close();
		  }
		}
	  }

	  /// <summary>
	  /// Calls the specified program using the specified parameter data and returns the result.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CommandResult call(String pgmLibrary, String pgmName, Parameter[] parms) throws IOException
	  public virtual CommandResult call(string pgmLibrary, string pgmName, Parameter[] parms)
	  {
		if (Closed)
		{
			throw new IOException("Connection closed");
		}

		sendCallProgramRequest(out_, pgmLibrary, pgmName, parms, datastreamLevel_);
		out_.flush();

		int length = in_.readInt();
		if (length < 20)
		{
		  throw DataStreamException.badLength("commandCallProgram", length);
		}
		in_.skipBytes(16);
		int rc = in_.readShort();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean success = rc == 0;
		bool success = rc == 0;
		Message[] messages = null;
		if (rc == 0)
		{
		  in_.skipBytes(2);
		  for (int i = 0; i < parms.Length; ++i)
		  {
			if (parms[i].OutputLength > 0)
			{
			  int byteLength = in_.readInt();
			  in_.skipBytes(2);
			  int outputLength = in_.readInt();
			  int usage = in_.readShort();
			  sbyte[] outputData = new sbyte[byteLength - 12];
			  in_.readFully(outputData);
			  parms[i].OutputData = outputData;
			}
		  }
		  messages = new Message[0];
		}
		else
		{
		  messages = getMessages(length);
	//      throw DataStreamException.badReturnCode("commandCallProgram", rc);
		}
		in_.end();
		return new CommandResult(success, messages, rc);
	  }

	  /// <summary>
	  /// Calls the specified program and returns the result.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CommandResult call(Program pgm) throws IOException
	  public virtual CommandResult call(Program pgm)
	  {
		if (Closed)
		{
			throw new IOException("Connection closed");
		}

		pgm.newCall();

		sendCallProgramRequest(out_, pgm, datastreamLevel_);
		out_.flush();

		int length = in_.readInt();
		if (length < 20)
		{
		  throw DataStreamException.badLength("commandCallProgram", length);
		}
		in_.skipBytes(16);
		int rc = in_.readShort();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean success = rc == 0;
		bool success = rc == 0;
		Message[] messages = null;
		if (rc == 0)
		{
		  in_.skipBytes(2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numParms = pgm.getNumberOfParameters();
		  int numParms = pgm.NumberOfParameters;
		  for (int i = 0; i < numParms; ++i)
		  {
			if (pgm.getParameterOutputLength(i) > 0)
			{
			  int byteLength = in_.readInt();
			  in_.skipBytes(2);
			  int outputLength = in_.readInt();
			  int usage = in_.readShort();
			  int parmLength = byteLength - 12;
			  sbyte[] buf = pgm.TempDataBuffer;
			  if (buf == null || buf.Length < parmLength)
			  {
				  buf = new sbyte[parmLength];
			  }
			  in_.readFully(buf, 0, parmLength);
			  pgm.setParameterOutputData(i, buf, parmLength);
			}
		  }
		  messages = new Message[0];
		}
		else
		{
		  messages = getMessages(length);
	//      throw DataStreamException.badReturnCode("commandCallProgram", rc);
		}
		in_.end();
		return new CommandResult(success, messages, rc);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendCallProgramRequest(HostOutputStream out, Program pgm, int datastreamLevel) throws IOException
	  private static void sendCallProgramRequest(HostOutputStream @out, Program pgm, int datastreamLevel)
	  {
		int length = 43;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numParms = pgm.getNumberOfParameters();
		int numParms = pgm.NumberOfParameters;
		for (int i = 0; i < numParms; ++i)
		{
		  length += 12 + pgm.getParameterInputLength(i);
		}

		@out.writeInt(length); // Length;
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE008); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(0); // Correlation ID.
		@out.writeShort(23); // Template length.
		@out.writeShort(0x1003); // ReqRep ID.

	//    byte[] programBytes = Util.blankPadEBCDIC10(pgm.getProgramName());
	//    out.write(programBytes, 0, 10);
		writePadEBCDIC10(pgm.ProgramName, @out);
	//    byte[] libraryBytes = Util.blankPadEBCDIC10(pgm.getProgramLibrary());
	//    out.write(libraryBytes, 0, 10);
		writePadEBCDIC10(pgm.ProgramLibrary, @out);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean newerServer = datastreamLevel >= 10;
		bool newerServer = datastreamLevel >= 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2);
		int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2); // Always return all messages when possible.
		@out.writeByte(messageOption);

		@out.writeShort(numParms); // Number of parameters.
		for (int i = 0; i < numParms; ++i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int inputLength = pgm.getParameterInputLength(i);
		  int inputLength = pgm.getParameterInputLength(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int outputLength = pgm.getParameterOutputLength(i);
		  int outputLength = pgm.getParameterOutputLength(i);
		  @out.writeInt(12 + inputLength); // Parameter LL.
		  @out.writeShort(0x1103); // Parameter CP.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxLength = inputLength > outputLength ? inputLength : outputLength;
		  int maxLength = inputLength > outputLength ? inputLength : outputLength;
		  @out.writeInt(maxLength); // Either the input length or output length, whichever is larger.
		  switch (pgm.getParameterType(i))
		  {
			case Parameter.TYPE_NULL:
			  if (datastreamLevel < 6)
			  {
				// Nulls not allowed.
				@out.writeShort(1); // Treat as input.
			  }
			  else
			  {
				@out.writeShort(255);
			  }
			  break;
			case Parameter.TYPE_INPUT:
			  @out.writeShort(11);
			  sbyte[] inputBuf = pgm.getParameterInputData(i);
			  @out.write(inputBuf, 0, inputLength);
			  break;
			case Parameter.TYPE_OUTPUT:
			  @out.writeShort(12);
			  break;
			case Parameter.TYPE_INPUT_OUTPUT:
			  @out.writeShort(13);
			  inputBuf = pgm.getParameterInputData(i);
			  @out.write(inputBuf, 0, inputLength);
			  break;
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendCallProgramRequest(HostOutputStream out, String pgmLibrary, String pgmName, Parameter[] parms, int datastreamLevel) throws IOException
	  private static void sendCallProgramRequest(HostOutputStream @out, string pgmLibrary, string pgmName, Parameter[] parms, int datastreamLevel)
	  {
		int length = 43;
		for (int i = 0; i < parms.Length; ++i)
		{
		  length += 12 + parms[i].InputLength;
		}

		@out.writeInt(length); // Length;
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE008); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(0); // Correlation ID.
		@out.writeShort(23); // Template length.
		@out.writeShort(0x1003); // ReqRep ID.

	//    byte[] programBytes = Util.blankPadEBCDIC10(pgmName);
	//    out.write(programBytes, 0, 10);
		writePadEBCDIC10(pgmName, @out);
	//    byte[] libraryBytes = Util.blankPadEBCDIC10(pgmLibrary);
	//    out.write(libraryBytes, 0, 10);
		writePadEBCDIC10(pgmLibrary, @out);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean newerServer = datastreamLevel >= 10;
		bool newerServer = datastreamLevel >= 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2);
		int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2); // Always return all messages when possible.
		@out.writeByte(messageOption);

		@out.writeShort(parms.Length); // Number of parameters.
		for (int i = 0; i < parms.Length; ++i)
		{
		  @out.writeInt(12 + parms[i].InputLength); // Parameter LL.
		  @out.writeShort(0x1103); // Parameter CP.
		  @out.writeInt(parms[i].MaxLength); // Either the input length or output length, whichever is larger.
		  switch (parms[i].Type)
		  {
			case Parameter.TYPE_NULL:
			  if (datastreamLevel < 6)
			  {
				// Nulls not allowed.
				@out.writeShort(1); // Treat as input.
			  }
			  else
			  {
				@out.writeShort(255);
			  }
			  break;
			case Parameter.TYPE_INPUT:
			  @out.writeShort(11);
			  @out.write(parms[i].InputData, 0, parms[i].InputLength);
			  break;
			case Parameter.TYPE_OUTPUT:
			  @out.writeShort(12);
			  break;
			case Parameter.TYPE_INPUT_OUTPUT:
			  @out.writeShort(13);
			  @out.write(parms[i].InputData, 0, parms[i].InputLength);
			  break;
		  }
		}
	  }

	  /// <summary>
	  /// Executes the specified CL command string and returns the result.
	  /// The command must be non-interactive.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CommandResult execute(String cmd) throws IOException
	  public virtual CommandResult execute(string cmd)
	  {
		if (Closed)
		{
			throw new IOException("Connection closed");
		}

		sendRunCommandRequest(out_, cmd, datastreamLevel_);
		out_.flush();

		int length = in_.readInt();
		if (length < 20)
		{
		  throw DataStreamException.badLength("commandRunCommand", length);
		}
		in_.skipBytes(16);
		int rc = in_.readShort();
		if (rc != 0 && rc != 0x0400)
		{
		  in_.skipBytes(length - 22);
		  throw DataStreamException.badReturnCode("commandRunCommand", rc);
		}
		Message[] messages = getMessages(length);
		in_.end();
		return new CommandResult(rc == 0, messages, rc);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Message[] getMessages(int length) throws IOException
	  private Message[] getMessages(int length)
	  {
		char[] buffer = new char[1024];
		int numMessages = in_.readShort();
		Message[] messages = new Message[numMessages];
		int curLength = 24;
		for (int i = 0; i < numMessages; ++i)
		{
		  int oldLength = curLength;
		  int messageLength = in_.readInt();
		  curLength += 4;
		  int messageCP = in_.readShort();
		  curLength += 2;
		  if (messageCP == 0x1106)
		  {
			int textCCSID = in_.readInt();
			int substitutionCCSID = in_.readInt();
			int severity = in_.readShort();
			int messageTypeLength = in_.readInt();
			int messageType = in_.readShort();
			in_.skipBytes(messageTypeLength - 2);
			int messageIDLength = in_.readInt();
			sbyte[] messageID = new sbyte[messageIDLength];
			in_.readFully(messageID);
			int messageFileNameLength = in_.readInt();
			sbyte[] messageFileName = new sbyte[messageFileNameLength];
			in_.readFully(messageFileName);
			int messageFileLibraryNameLength = in_.readInt();
			sbyte[] messageFileLibrary = new sbyte[messageFileLibraryNameLength];
			in_.readFully(messageFileLibrary);
			int messageTextLength = in_.readInt();
			sbyte[] messageText = new sbyte[messageTextLength];
			in_.readFully(messageText);
			int messageSubstitutionTextLength = in_.readInt();
			sbyte[] substitutionData = new sbyte[messageSubstitutionTextLength];
			in_.readFully(substitutionData);
			int messageHelpLength = in_.readInt();
			sbyte[] messageHelp = new sbyte[messageHelpLength];
			in_.readFully(messageHelp);
			// String messageIDString = new String(messageID, "Cp037");
			if (messageID.Length > buffer.Length)
			{
				buffer = new char[messageID.Length];
			}
			string messageIDString = Conv.ebcdicByteArrayToString(messageID, buffer);

			// String messageTextString = new String(messageText, "Cp037");
			if (messageText.Length > buffer.Length)
			{
			  buffer = new char[messageText.Length];
			}
			string messageTextString = Conv.ebcdicByteArrayToString(messageText, buffer);

			messages[i] = new Message(messageIDString, messageTextString);
			curLength += 40 + messageTypeLength - 2 + messageIDLength + messageFileNameLength + messageFileLibraryNameLength + messageTextLength + messageSubstitutionTextLength + messageHelpLength;
		  }
		  else if (messageCP == 0x1102)
		  {
			sbyte[] messageID = new sbyte[7];
			in_.readFully(messageID);
			int messageType = in_.readShort();
			int severity = in_.readShort();
			sbyte[] fileName = new sbyte[10];
			in_.readFully(fileName);
			sbyte[] libraryName = new sbyte[10];
			in_.readFully(libraryName);
			int substitutionDataLength = in_.readShort();
			int textLength = in_.readShort();
			sbyte[] substitutionData = new sbyte[substitutionDataLength];
			in_.readFully(substitutionData);
			sbyte[] text = new sbyte[textLength];
			in_.readFully(text);
			curLength += 35 + substitutionDataLength + textLength;
			// String messageIDString = new String(messageID, "Cp037");
			if (messageID.Length > buffer.Length)
			{
				buffer = new char[messageID.Length];
			}
			string messageIDString = Conv.ebcdicByteArrayToString(messageID, buffer);

			// String messageTextString = new String(messageText, "Cp037");
			if (text.Length > buffer.Length)
			{
			  buffer = new char[text.Length];
			}
			string messageTextString = Conv.ebcdicByteArrayToString(text, buffer);


			messages[i] = new Message(messageIDString, messageTextString);
		  }
		  int remaining = messageLength - (curLength - oldLength);
		  in_.skipBytes(remaining);
		}
		in_.skipBytes(length - curLength);
		return messages;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendRunCommandRequest(HostOutputStream out, String cmd, int datastreamLevel) throws IOException
	  private static void sendRunCommandRequest(HostOutputStream @out, string cmd, int datastreamLevel)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean newerServer = datastreamLevel >= 10;
		bool newerServer = datastreamLevel >= 10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] commandBytes = newerServer ? Conv.stringToUnicodeByteArray(cmd) : Conv.stringToEBCDICByteArray37(cmd);
		sbyte[] commandBytes = newerServer ? Conv.stringToUnicodeByteArray(cmd) : Conv.stringToEBCDICByteArray37(cmd);
		@out.writeInt(newerServer ? 31 + commandBytes.Length : 27 + commandBytes.Length); // Length.
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE008); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(0); // Correlation ID.
		@out.writeShort(1); // Template length.
		@out.writeShort(0x1002); // ReqRep ID.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2);
		int messageOption = newerServer ? 4 : (datastreamLevel < 7 ? 0 : 2); // Always return all messages when possible.
		@out.writeByte(messageOption);

		if (newerServer)
		{
		  @out.writeInt(10 + commandBytes.Length); // Command LL.
		  @out.writeShort(0x1104); // Command CP.
		  @out.writeInt(1200); // Command CCSID.
		  @out.write(commandBytes);
		}
		else
		{
		  @out.writeInt(6 + commandBytes.Length); // Command LL.
		  @out.writeShort(0x1101); // Command CP.
		  @out.write(commandBytes);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendExchangeAttributesRequest(HostOutputStream out, String NLV) throws IOException
	  private static void sendExchangeAttributesRequest(HostOutputStream @out, string NLV)
	  {
		@out.writeInt(34); // Length.
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE008); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(0); // Correlation ID.
		@out.writeShort(14); // Template length.
		@out.writeShort(0x1001); // ReqRep ID.
		@out.writeInt(1200); // CCSID.
		// out.write("2924".getBytes("Cp037")); // NLV.
		sbyte[] NLVbytes = Conv.stringToEBCDICByteArray37(NLV);
		@out.write(NLVbytes);
		@out.writeInt(1); // Client version.
		@out.writeShort(0); // Client datastream level.
	  }



	}

}