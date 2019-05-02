using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMConnection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{
	using com.ibm.jtopenlite;


	/// <summary>
	/// Represents a TCP/IP socket connection to the System i Distributed Data Management (DDM) host server (QUSRWRK/QRWTSRVR job).
	/// 
	/// See <seealso cref="com.ibm.jtopenlite.samples.DDMRead"/> for an example of using DDM. 
	/// 
	/// </summary>
	public class DDMConnection : HostServerConnection
	{
	  private const bool DEBUG = false;

	  public const int DEFAULT_DDM_SERVER_PORT = 446;
	  public const int DEFAULT_SSL_DDM_SERVER_PORT = 446;

	  private readonly sbyte[] messageBuf_ = new sbyte[1024];
	  private readonly char[] charBuffer_ = new char[1024];

	  private int dclNamCounter_ = 0;
	  private int correlationID_ = 1;

	  private int newCorrelationID()
	  {
		if (correlationID_ == 0x7FFF)
		{
			correlationID_ = 0;
		}
		return ++correlationID_;
	  }

	  private DDMConnection(SystemInfo info, Socket socket, HostInputStream @in, HostOutputStream @out, string user, string jobString) : base(info, user, jobString, socket, @in, @out)
	  {
	  }

	  /// <summary>
	  /// No-op. The DDM host server does not use an end job datastream.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendEndJobRequest() throws IOException
	  protected internal override void sendEndJobRequest()
	  {
	  }

	  /// <summary>
	  /// Establishes a new socket connection to the specified system and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(String system, String user, String password) throws IOException
	  public static DDMConnection getConnection(string system, string user, string password)
	  {
		return getConnection(false, system, user, password);
	  }
	  /// <summary>
	  /// Establishes a new socket connection to the specified system and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(final boolean isSSL, String system, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DDMConnection getConnection(bool isSSL, string system, string user, string password)
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
	  /// Establishes a new socket connection to the specified system and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(SystemInfo info, String user, String password) throws IOException
	  public static DDMConnection getConnection(SystemInfo info, string user, string password)
	  {
		return getConnection(false, info, user, password);
	  }

	  /// <summary>
	  /// Establishes a new socket connection to the specified system and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DDMConnection getConnection(bool isSSL, SystemInfo info, string user, string password)
	  {
		return getConnection(isSSL, info, user, password, isSSL ? DEFAULT_SSL_DDM_SERVER_PORT: DEFAULT_DDM_SERVER_PORT);
	  }

	  /// <summary>
	  /// Establishes a new socket connection to the specified system and port, and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(SystemInfo info, String user, String password, int ddmPort) throws IOException
	  public static DDMConnection getConnection(SystemInfo info, string user, string password, int ddmPort)
	  {
		return getConnection(false, info, user, password, ddmPort);
	  }

	  /// <summary>
	  /// Establishes a new socket connection to the specified system and port, and authenticates the specified user and password.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DDMConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password, int ddmPort) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DDMConnection getConnection(bool isSSL, SystemInfo info, string user, string password, int ddmPort)
	  {
		if (ddmPort < 0 || ddmPort > 65535)
		{
		  throw new IOException("Bad DDM port: " + ddmPort);
		}
		DDMConnection conn = null;

		Socket ddmServer = isSSL? SSLSocketFactory.Default.createSocket(info.System, ddmPort) : new Socket(info.System, ddmPort);
		Stream @in = ddmServer.InputStream;
		Stream @out = ddmServer.OutputStream;
		try
		{
		  // Exchange random seeds.
		  HostOutputStream dout = new HostOutputStream(new BufferedOutputStream(@out, 1024));
		  sendEXCSATRequest(dout);
		  dout.flush();

		  HostInputStream din = new HostInputStream(new BufferedInputStream(@in, 32768));
		  int length = din.readShort();
		  if (length < 10)
		  {
			throw DataStreamException.badLength("ddmExchangeServerAttributes", length);
		  }
		  int gdsID = din.read();
		  int typeCorrelationChainedContinue = din.read(); // bit mask
		  int correlation = din.readShort();
		  int excSATLength = din.readShort(); // LL.
		  int codepoint = din.readShort(); // CP.
		  if (codepoint != 0x1443) // EXCSATRD
		  {
			throw DataStreamException.badReply("ddmExchangeServerAttributes", codepoint);
		  }
		  int numRead = 10;
		  sbyte[] extNam = null; // Job string.
		  while (length - numRead > 4 && extNam == null)
		  {
			int extNamLL = din.readShort();
			int extNamCP = din.readShort();
			numRead += 4;
			if (extNamCP == 0x115E) // EXTNAM
			{
			  extNam = new sbyte[extNamLL - 4];
			  din.readFully(extNam);
			  numRead += extNam.Length;
			}
			else
			{
			  din.skipBytes(extNamLL - 4);
			  numRead += (extNamLL - 4);
			}
		  }
		  din.skipBytes(length - numRead);
		  din.end();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jobString = extNam != null ? Conv.ebcdicByteArrayToString(extNam, 0, extNam.length) : null;
		  string jobString = extNam != null ? Conv.ebcdicByteArrayToString(extNam, 0, extNam.Length) : null;

		  long seed = sendACCSECRequest(dout, info.PasswordLevel >= 2);
		  sbyte[] clientSeed = Conv.longToByteArray(seed);
		  dout.flush();

		  length = din.readShort();
		  if (length < 28)
		  {
			throw DataStreamException.badLength("ddmAccessMethodExchange", length);
		  }
		  gdsID = din.read();
		  typeCorrelationChainedContinue = din.read();
		  correlation = din.readShort();
		  //length = din.readShort(); // LL.
		  din.skipBytes(2);
		  codepoint = din.readShort(); // CP.
		  if (codepoint != 0x14AC) // ACCSECRD
		  {
			throw DataStreamException.badReply("ddmAccessMethodExchange", codepoint);
		  }
		  din.skipBytes(10);
		  sbyte[] serverSeed = new sbyte[8];
		  din.readFully(serverSeed);
		  din.skipBytes(length - 28);
		  din.end();

		  sbyte[] userBytes = getUserBytes(user, info.PasswordLevel);
		  sbyte[] passwordBytes = getPasswordBytes(password, info.PasswordLevel);
		  password = null;
		  sbyte[] encryptedPassword = getEncryptedPassword(userBytes, passwordBytes, clientSeed, serverSeed, info.PasswordLevel);

		  sbyte[] userEBCDICBytes = (info.PasswordLevel < 2) ? userBytes : getUserBytes(user, 0);
		  sendSECCHKRequest(dout, userEBCDICBytes, encryptedPassword);
		  dout.flush();

		  length = din.readShort();
		  if (length < 21)
		  {
			throw DataStreamException.badLength("ddmSecurityCheck", length);
		  }
		  gdsID = din.read();
		  typeCorrelationChainedContinue = din.read();
		  correlation = din.readShort();
		  //length = din.readShort(); // LL.
		  din.skipBytes(2);
		  codepoint = din.readShort(); // CP.
		  if (codepoint != 0x1219) // SECCHKRD
		  {
			throw DataStreamException.badReply("ddmSecurityCheckSECCHKRD", codepoint);
		  }
		  din.skipBytes(8);
		  codepoint = din.readShort();
		  if (codepoint != 0x11A4) // SECCHKCD.
		  {
			throw DataStreamException.badReply("ddmSecurityCheckSECCHKCD", codepoint);
		  }
		  int rc = din.read();
		  if (rc != 0)
		  {
			throw DataStreamException.badReturnCode("ddmSecurityCheck", rc);
		  }
		  din.skipBytes(length - 21);
		  din.end();

		  conn = new DDMConnection(info, ddmServer, din, dout, user, jobString);
		  return conn;
		}
		finally
		{
		  if (conn == null)
		  {
			@in.Close();
			@out.Close();
			ddmServer.close();
		  }
		}
	  }

	  // Copied from HostServerConnection.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static byte[] getUserBytes(String user, int level) throws IOException
	  internal static sbyte[] getUserBytes(string user, int level)
	  {
		if (level < 2)
		{
		if (user.Length > 10)
		{
		  throw new IOException("User too long");
		}
		  sbyte[] user37 = Conv.blankPadEBCDIC10(user.ToUpper());
		  return user37;
		}
		else
		{
		  sbyte[] b = new sbyte[20];
		  Conv.stringToBlankPadUnicodeByteArray(user.ToUpper(), b, 0, 20);
		  return b;
		}
	  }

	  // Copied from HostServerConnection.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static byte[] getPasswordBytes(String password, int level) throws IOException
	  internal static sbyte[] getPasswordBytes(string password, int level)
	  {
		if (level < 2)
		{
		// Prepend a Q to numeric password.
		if (password.Length > 0 && char.IsDigit(password[0]))
		{
		  password = "Q" + password;
		}
		if (password.Length > 10)
		{
		  throw new IOException("Password too long");
		}
		  sbyte[] password37 = Conv.blankPadEBCDIC10(password.ToUpper());
		  return password37;
		}
		else
		{
		  return Conv.stringToUnicodeByteArray(password);
		}
	  }

	  /// <summary>
	  /// Closes the specified file and returns any messages that were issued.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Message[] close(DDMFile file) throws IOException
	  public virtual Message[] close(DDMFile file)
	  {
			  IList<Message> messageList = closeReturnMessageList(file);
			  Message[] outMessages = new Message[messageList.Count];
			  for (int i = 0; i < messageList.Count; i++)
			  {
				  outMessages[i] = messageList[i];
			  }
			  return outMessages;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List<Message> closeReturnMessageList(DDMFile file) throws IOException
	  public virtual IList<Message> closeReturnMessageList(DDMFile file)
	  {
		sbyte[] dclNam = file.DCLNAM;
		sendS38CloseRequest(out_, dclNam);
		out_.flush();

		int length = in_.readShort();
		if (length < 10)
		{
		  throw DataStreamException.badLength("ddmS38Close", length);
		}
		int gdsID = in_.read();
		int typeCorrelationChainedContinue = in_.read(); // bit mask
		int correlation = in_.readShort();
		int numRead = 8;
		List<Message> messages = new List<Message>();
		int[] msgNumRead = new int[1];
		while (numRead + 4 < length)
		{
		  int ll = in_.readShort();
		  int cp = in_.readShort();
		  numRead += 4;
		  if (cp == 0xD201) // S38MSGRM
		  {
			Message msg = getMessage(in_, ll, msgNumRead);
			numRead += msgNumRead[0];
			if (msg != null)
			{
			  messages.Add(msg);
			}
		  }
		  else
		  {
			in_.skipBytes(ll - 4);
			numRead += ll - 4;
		  }
		}
		in_.skipBytes(length - numRead);
		in_.end();
		return messages;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Message getMessage(HostInputStream din, final int ll, int[] saved) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private Message getMessage(HostInputStream din, int ll, int[] saved)
	  {
		int severity = -1;
		string messageID = null;
		string messageText = null;
		sbyte[] b = null;
		char[] c = null;
		int msgNumRead = 0;
		while (msgNumRead < ll - 4)
		{
		  int msgLength = din.readShort();
		  int msgCodepoint = din.readShort();
		  switch (msgCodepoint)
		  {
			case 0x1149: // SVRCOD
			  severity = din.readShort();
			  din.skipBytes(msgLength - 6);
			  break;
			case 0xD112: // S38MID
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean tooLong4 = msgLength > 1028;
			  bool tooLong4 = msgLength > 1028;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int msgLength4 = msgLength-4;
			  int msgLength4 = msgLength - 4;
			  b = tooLong4 ? new sbyte[msgLength4] : messageBuf_;
			  c = tooLong4 ? new char[msgLength4] : charBuffer_;
			  din.readFully(b, 0, msgLength4);
			  messageID = Conv.ebcdicByteArrayToString(b, 0, msgLength4, c);
			  break;
			case 0xD116: // S38MTEXT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean tooLong6 = msgLength > 1030;
			  bool tooLong6 = msgLength > 1030;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int msgLength6 = msgLength-6;
			  int msgLength6 = msgLength - 6;
			  b = tooLong6 ? new sbyte[msgLength6] : messageBuf_;
			  c = tooLong6 ? new char[msgLength6] : charBuffer_;
			  din.skipBytes(2);
			  din.readFully(b, 0, msgLength6);
			  messageText = Conv.ebcdicByteArrayToString(b, 0, msgLength6, c);
			  break;
			default:
			  din.skipBytes(msgLength - 4);
			  break;
		  }
		  msgNumRead += msgLength;
		}
		saved[0] = msgNumRead;
		return string.ReferenceEquals(messageID, null) ? null : new Message(messageID, messageText, severity);
	  }

	  /// <summary>
	  /// Opens the specified file for sequential read-only access and a preferred batch size of 100. </summary>
	  /// <param name="library"> The name of the library in which the file resides. For example, "QSYS". </param>
	  /// <param name="file"> The name of the physical or logical file. </param>
	  /// <param name="member"> The member within the file. This can be a special value such as "*FIRST". </param>
	  /// <param name="recordFormat"> The name of the record format. This value can also be obtained from <seealso cref="DDMRecordFormat#getName DDMRecordFormat.getName()"/>.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DDMFile open(String library, String file, String member, String recordFormat) throws IOException
	  public virtual DDMFile open(string library, string file, string member, string recordFormat)
	  {
		return open(library, file, member, recordFormat, DDMFile.READ_ONLY, false, 100, 1);
	  }

	  /// <summary>
	  /// Opens the specified file for reading or writing records. </summary>
	  /// <param name="library"> The name of the library in which the file resides. For example, "QSYS". </param>
	  /// <param name="file"> The name of the physical or logical file. </param>
	  /// <param name="member"> The member within the file. This can be a special value such as "*FIRST". </param>
	  /// <param name="recordFormat"> The name of the record format. This value can also be obtained from <seealso cref="DDMRecordFormat#getName DDMRecordFormat.getName()"/>. </param>
	  /// <param name="readWriteType"> The read-write access type to use. Allowed values are:
	  /// <ul>
	  /// <li><seealso cref="DDMFile#READ_ONLY DDMFile.READ_ONLY"/></li>
	  /// <li><seealso cref="DDMFile#READ_WRITE DDMFile.READ_WRITE"/></li>
	  /// <li><seealso cref="DDMFile#WRITE_ONLY DDMFile.WRITE_ONLY"/></li>
	  /// </ul> </param>
	  /// <param name="keyed"> Indicates if the file should be opened for sequential or keyed access. </param>
	  /// <param name="preferredBatchSize"> The number of records to read or write at a time. This is a preferred number because the DDMConnection
	  /// may decide to use a different batch size depending on circumstances (e.g. READ_WRITE files always use a batch size of 1). </param>
	  /// <param name="numBuffers"> The number of data buffers to allocate for use when reading new records. The DDMConnection will round-robin
	  /// through the data buffers as it calls <seealso cref="DDMReadCallback#newRecord DDMReadCallback.newRecord()"/>. This can be useful
	  /// for multi-threaded readers that process new record data, such as <seealso cref="DDMThreadedReader DDMThreadedReader"/>. In such cases,
	  /// each processing thread could be assigned a specific data buffer, to avoid contention. See <seealso cref="DDMDataBuffer DDMDataBuffer"/>.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DDMFile open(String library, String file, String member, String recordFormat, int readWriteType, boolean keyed, int preferredBatchSize, int numBuffers) throws IOException
	  public virtual DDMFile open(string library, string file, string member, string recordFormat, int readWriteType, bool keyed, int preferredBatchSize, int numBuffers)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean doWrite = readWriteType == DDMFile.READ_WRITE || readWriteType == DDMFile.WRITE_ONLY;
		bool doWrite = readWriteType == DDMFile.READ_WRITE || readWriteType == DDMFile.WRITE_ONLY;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean doRead = readWriteType == DDMFile.READ_WRITE || readWriteType == DDMFile.READ_ONLY || !doWrite;
		bool doRead = readWriteType == DDMFile.READ_WRITE || readWriteType == DDMFile.READ_ONLY || !doWrite;
		if ((doWrite && doRead) || preferredBatchSize <= 0)
		{
			preferredBatchSize = 1;
		}
		preferredBatchSize = preferredBatchSize & 0x7FFF;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] dclNam = generateDCLNAM();
		sbyte[] dclNam = generateDCLNAM();
		sendS38OpenRequest(out_, file, library, member, doRead, doWrite, keyed, recordFormat, dclNam, preferredBatchSize);
		out_.flush();

		int length = in_.readShort();
		if (length < 10)
		{
		  throw DataStreamException.badLength("ddmS38Open", length);
		}
		int gdsID = in_.read();
		int type = in_.read(); // bit mask
		bool isChained = (type & 0x40) != 0;
		int correlation = in_.readShort();
		int ll = in_.readShort();
		int cp = in_.readShort();
		int numRead = 10;
		if (DEBUG)
		{
			Console.WriteLine("open: " + gdsID + "," + type + "," + isChained + "," + correlation + "," + ll + "," + cp);
		}
		while (cp != 0xD404 && numRead + 4 <= length)
		{
		  if (cp == 0xD201) // S38MSGRM
		  {
			int[] numMsgRead = new int[1];
			Message msg = getMessage(in_, ll, numMsgRead);
			numRead += numMsgRead[0];
			if (msg != null)
			{
			  throw DataStreamException.errorMessage("ddmS38Open", msg);
			}
		  }
		  else
		  {
			if (DEBUG)
			{
				Console.WriteLine("Got cp " + cp.ToString("x"));
			}
			in_.skipBytes(ll - 4);
			numRead += (ll - 4);
		  }
		  ll = in_.readShort();
		  cp = in_.readShort();
		  numRead += 4;
		}
		if (cp != 0xD404) // S38OPNFB
		{
		  throw DataStreamException.badReply("ddmS38Open", cp);
		}
		int openType = in_.readByte();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] b = new byte[10];
		sbyte[] b = new sbyte[10];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] c = new char[10];
		char[] c = new char[10];
		in_.readFully(b);
		string realFile = Conv.ebcdicByteArrayToString(b, 0, b.Length, c);
		in_.readFully(b);
		string realLibrary = Conv.ebcdicByteArrayToString(b, 0, b.Length, c);
		in_.readFully(b);
		string realMember = Conv.ebcdicByteArrayToString(b, 0, b.Length, c);
		int recordLength = in_.readShort();
		in_.skipBytes(10);
		int numRecordsReturned = in_.readInt();
		in_.readFully(b, 0, 2);
		string accessType = Conv.ebcdicByteArrayToString(b, 0, 2, c);
		bool supportDuplicateKeys = in_.readByte() == 0x00C4;
		bool isSourceFile = in_.readByte() == 0x00E8;
		in_.skipBytes(10); // UFCB parameters
		int maxBlockedRecordsTransferred = in_.readShort();
		int recordIncrement = in_.readShort();
		int openFlags1 = in_.readByte();
		in_.skipBytes(6); // Number of associated physical file members, and other stuff?
		int maxRecordLength = in_.readShort();
		int recordWaitTime = in_.readInt();
		int openFlags2 = in_.readShort();
		int nullFieldByteMapOffset = in_.readShort();
		int nullKeyFieldByteMapOffset = in_.readShort();
		in_.skipBytes(4); // Other stuff?
		int ccsid = in_.readShort();
		int totalFixedFieldLength = in_.readShort();
		numRead += 92;
		in_.skipBytes(length - numRead); // Min record length and other stuff?

		while (isChained)
		{
		  // Skip the rest.
		  length = in_.readShort();
		  if (length < 10)
		  {
			throw DataStreamException.badLength("ddmS38Open", length);
		  }
		  gdsID = in_.read();
		  type = in_.read(); // bit mask
		  isChained = (type & 0x40) != 0;
		  correlation = in_.readShort();
		  numRead = 6;
		  in_.skipBytes(length - 6);
		}
		in_.end();

		sbyte[] recordFormatName = Conv.blankPadEBCDIC10(recordFormat);
		return new DDMFile(realLibrary, realFile, realMember, recordFormatName, dclNam, readWriteType, recordLength, recordIncrement, preferredBatchSize, nullFieldByteMapOffset, numBuffers);
	  }

	  /// <summary>
	  /// Retrieves the member descriptions for the specified file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List<DDMFileMemberDescription> getFileMemberDescriptions(final String library, final String file) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual IList<DDMFileMemberDescription> getFileMemberDescriptions(string library, string file)
	  {
		DDMFile f = new DDMFile(library, file, null, null, null, DDMFile.READ_ONLY, 0, 0, 0, 0, 1);
		return getFileMemberDescriptions(f);
	  }

	  /// <summary>
	  /// Retrieves the member descriptions for the specified file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List<DDMFileMemberDescription> getFileMemberDescriptions(final DDMFile file) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual IList<DDMFileMemberDescription> getFileMemberDescriptions(DDMFile file)
	  {
		IList<Message> messages = executeReturnMessageList("DSPFD FILE(" + file.Library.Trim() + "/" + file.File.Trim() + ") TYPE(*MBR) OUTPUT(*OUTFILE) OUTFILE(QTEMP/TB2FD)");
		if (messages.Count == 0)
		{
		  throw new IOException("DSPFFD failed to return success message");
		}
		bool error = false;
		for (int i = 0; !error && i < messages.Count; ++i)
		{
		  if (DEBUG)
		  {
			  Console.WriteLine(messages[i]);
		  }
		  if (!messages[i].ID.Equals("CPF9861") && !messages[i].ID.Equals("CPF9862") && !messages[i].ID.Equals("CPF3030")) // Records added to member.
		  {
			error = true;
		  }
		}
		if (error)
		{
		  DataStreamException dse = DataStreamException.errorMessage("DSPFD", messages[0]);
		  for (int i = 1; i < messages.Count; ++i)
		  {
			dse.addMessage(messages[i]);
		  }
		  throw dse;
		}
		if (DEBUG)
		{
			Console.WriteLine("Opening file...");
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMFile temp = open("QTEMP", "TB2FD", "TB2FD", "QWHFDMBR", DDMFile.READ_ONLY, false, 100, 1);
		DDMFile temp = open("QTEMP", "TB2FD", "TB2FD", "QWHFDMBR", DDMFile.READ_ONLY, false, 100, 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMFileMemberDescriptionReader reader = new DDMFileMemberDescriptionReader(getInfo().getServerCCSID());
		DDMFileMemberDescriptionReader reader = new DDMFileMemberDescriptionReader(Info.ServerCCSID);
		while (!reader.eof())
		{
		  readNext(temp, reader);
		}
		close(temp);
		return reader.MemberDescriptions;
	  }

	  /// <summary>
	  /// Retrieves the record format of the specified file. This currently only retrieves the first record format in a multi-format file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DDMRecordFormat getRecordFormat(final String library, final String file) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual DDMRecordFormat getRecordFormat(string library, string file)
	  {
		DDMFile f = new DDMFile(library, file, null, null, null, DDMFile.READ_ONLY, 0, 0, 0, 0, 1);
		return getRecordFormat(f);
	  }

	  /// <summary>
	  /// Retrieves the record format of the specified file. This currently only retrieves the first record format in a multi-format file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DDMRecordFormat getRecordFormat(DDMFile file) throws IOException
	  public virtual DDMRecordFormat getRecordFormat(DDMFile file)
	  {
		IList<Message> messages = executeReturnMessageList("DSPFFD FILE(" + file.Library.Trim() + "/" + file.File.Trim() + ") OUTPUT(*OUTFILE) OUTFILE(QTEMP/TB2FFD)");
		if (messages.Count == 0)
		{
		  throw new IOException("DSPFFD failed to return success message");
		}
		bool error = false;
		for (int i = 0; !error && i < messages.Count; ++i)
		{
		  if (DEBUG)
		  {
			  Console.WriteLine(messages[i]);
		  }
		  if (!messages[i].ID.Equals("CPF9861") && !messages[i].ID.Equals("CPF9862") && !messages[i].ID.Equals("CPF3030")) // Records added to member.
		  {
			error = true;
		  }
		}
		if (error)
		{
		  DataStreamException dse = DataStreamException.errorMessage("DSPFFD", messages[0]);
		  for (int i = 1; i < messages.Count; ++i)
		  {
			dse.addMessage(messages[i]);
		  }
		  throw dse;
		}
		if (DEBUG)
		{
			Console.WriteLine("Opening file...");
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMFile temp = open("QTEMP", "TB2FFD", "TB2FFD", "QWHDRFFD", DDMFile.READ_ONLY, false, 100, 1);
		DDMFile temp = open("QTEMP", "TB2FFD", "TB2FFD", "QWHDRFFD", DDMFile.READ_ONLY, false, 100, 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMRecordFormatReader reader = new DDMRecordFormatReader(getInfo().getServerCCSID());
		DDMRecordFormatReader reader = new DDMRecordFormatReader(Info.ServerCCSID);
		while (!reader.eof())
		{
		  readNext(temp, reader);
		}
		close(temp);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMRecordFormat rf = new DDMRecordFormat(reader.getLibrary(), reader.getFile(), reader.getName(), reader.getType(), reader.getText(), reader.getFields(), reader.getLength());
		DDMRecordFormat rf = new DDMRecordFormat(reader.Library, reader.File, reader.Name, reader.Type, reader.Text, reader.Fields, reader.Length);

		messages = executeReturnMessageList("DLTF FILE(" + temp.Library.Trim() + "/" + temp.File.Trim() + ")");
		error = false;
		for (int i = 0; !error && i < messages.Count; ++i)
		{
		  if (DEBUG)
		  {
			  Console.WriteLine(messages[i]);
		  }
		  if (!messages[i].ID.Equals("CPC2191")) // Object deleted.
		  {
			error = true;
		  }
		}
		if (error)
		{
		  DataStreamException dse = DataStreamException.errorMessage("DLTF", messages[0]);
		  for (int i = 1; i < messages.Count; ++i)
		  {
			dse.addMessage(messages[i]);
		  }
		  throw dse;
		}

		return rf;
	  }

	  /// <summary>
	  /// Reads the record with the specified record number from the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(DDMFile file, DDMReadCallback listener, int recordNumber) throws IOException
	  public virtual void read(DDMFile file, DDMReadCallback listener, int recordNumber)
	  {
		sendS38GetDRequest(out_, file.DCLNAM, file.ReadWriteType == DDMFile.READ_WRITE, file.RecordFormatName, recordNumber, false);
		out_.flush();

		handleReply(file, "ddmS38GetD", listener);
	  }

	  /// <summary>
	  /// Reads the next record whose key is equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readKeyEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void readKeyEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0B, false);
	  }

	  /// <summary>
	  /// Reads the next record whose key is greater than the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readKeyGreater(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void readKeyGreater(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0D, false);
	  }

	  /// <summary>
	  /// Reads the next record whose key is greater than or equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readKeyGreaterOrEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void readKeyGreaterOrEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0C, false);
	  }

	  /// <summary>
	  /// Reads the next record whose key is less than the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readKeyLess(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void readKeyLess(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x09, false);
	  }

	  /// <summary>
	  /// Reads the next record whose key is less than or equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readKeyLessOrEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void readKeyLessOrEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0A, false);
	  }

	  /// <summary>
	  /// Positions the file cursor to the record whose key is equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToKeyEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void positionToKeyEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0B, true);
	  }

	  /// <summary>
	  /// Positions the file cursor to the record whose key is greater than the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToKeyGreater(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void positionToKeyGreater(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0D, true);
	  }

	  /// <summary>
	  /// Positions the file cursor to the record whose key is greater than or equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToKeyGreaterOrEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void positionToKeyGreaterOrEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0C, true);
	  }

	  /// <summary>
	  /// Positions the file cursor to the record whose key is less than the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToKeyLess(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void positionToKeyLess(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x09, true);
	  }

	  /// <summary>
	  /// Positions the file cursor to the record whose key is less than or equal to the specified key.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToKeyLessOrEqual(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields) throws IOException
	  public virtual void positionToKeyLessOrEqual(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields)
	  {
		readTypeKey(file, listener, key, numberOfKeyFields, 0x0A, true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readTypeKey(DDMFile file, DDMReadCallback listener, byte[] key, int numberOfKeyFields, int readType, boolean doPosition) throws IOException
	  private void readTypeKey(DDMFile file, DDMReadCallback listener, sbyte[] key, int numberOfKeyFields, int readType, bool doPosition)
	  {
		sendS38GetKRequest(out_, file.DCLNAM, file.ReadWriteType == DDMFile.READ_WRITE, file.RecordFormatName, readType, key, numberOfKeyFields, doPosition);
		out_.flush();

		handleReply(file, "ddmS38GetK", listener);
	  }


	  /// <summary>
	  /// Reads the next record from the specified file and positions the cursor on or after it.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readNext(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void readNext(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 3, false);
	  }

	  /// <summary>
	  /// Reads the previous record from the specified file and positions the cursor on or before it.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readPrevious(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void readPrevious(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 4, false);
	  }

	  /// <summary>
	  /// Reads the first record from the specified file and positions the cursor on or after it.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFirst(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void readFirst(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 1, false);
	  }

	  /// <summary>
	  /// Reads the last record from the specified file and positions the cursor on or after it.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readLast(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void readLast(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 2, false);
	  }

	  /// <summary>
	  /// Reads the current record from the specified file and positions the cursor on or after it.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readCurrent(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void readCurrent(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 33, false);
	  }

	  /// <summary>
	  /// Positions the cursor to the next record in the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToNext(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void positionToNext(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 3, true);
	  }

	  /// <summary>
	  /// Positions the cursor to the previous record in the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToPrevious(DDMFile file, DDMReadCallback listener) throws IOException
	  public virtual void positionToPrevious(DDMFile file, DDMReadCallback listener)
	  {
		readType(file, listener, 4, true);
	  }

	  /// <summary>
	  /// Positions the cursor to the first record in the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToFirst(DDMFile file) throws IOException
	  public virtual void positionToFirst(DDMFile file)
	  {
		readType(file, null, 1, true);
	  }

	  /// <summary>
	  /// Positions the cursor to the last record in the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionToLast(DDMFile file) throws IOException
	  public virtual void positionToLast(DDMFile file)
	  {
		readType(file, null, 2, true);
	  }

	  /// <summary>
	  /// Positions the cursor to the end of the file (after the last record).
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionAfterLast(DDMFile file) throws IOException
	  public virtual void positionAfterLast(DDMFile file)
	  {
		position(file, 2);
	  }

	  /// <summary>
	  /// Positions the cursor to the beginning of the file (before the first record).
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void positionBeforeFirst(DDMFile file) throws IOException
	  public virtual void positionBeforeFirst(DDMFile file)
	  {
		position(file, 1);
	  }

	  /// <summary>
	  /// Positions the cursor to the specified record number.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void position(DDMFile file, DDMReadCallback listener, int recordNumber) throws IOException
	  public virtual void position(DDMFile file, DDMReadCallback listener, int recordNumber)
	  {
		sendS38GetDRequest(out_, file.DCLNAM, file.ReadWriteType == DDMFile.READ_WRITE, file.RecordFormatName, recordNumber, true);
		out_.flush();

		handleReply(file, "ddmS38GetD", listener);
	  }

	  /// <summary>
	  /// Writes a single record to the end of the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(DDMFile file, byte[] data, int offset, boolean[] nullFieldValues, DDMReadCallback listener) throws IOException
	  public virtual void write(DDMFile file, sbyte[] data, int offset, bool[] nullFieldValues, DDMReadCallback listener)
	  {
		int id = newCorrelationID();
		sendS38PUTMRequest(out_, file.DCLNAM, id);
		file.EventBuffer.EventType = DDMCallbackEvent.EVENT_WRITE;
		sendS38BUFRequest(out_, id, file.RecordIncrement, data, offset, file.RecordLength, nullFieldValues);
		out_.flush();

		handleReply(file, "ddmS38PUTM", listener);
	  }

	  /// <summary>
	  /// Writes records provided by the callback to the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(DDMFile file, DDMWriteCallback listener) throws IOException
	  public virtual void write(DDMFile file, DDMWriteCallback listener)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMCallbackEvent event = file.getEventBuffer();
		DDMCallbackEvent @event = file.EventBuffer;
		@event.EventType = DDMCallbackEvent.EVENT_WRITE;

		int blockingFactor = file.BatchSize;
		int numRecords = listener.getNumberOfRecords(@event);
		int startingRecordNumber = 0;
		int batchSize = numRecords > blockingFactor ? blockingFactor : numRecords;
		int id = newCorrelationID();
		while (startingRecordNumber < numRecords)
		{
		  if (startingRecordNumber + batchSize >= numRecords)
		  {
			  batchSize = numRecords - startingRecordNumber;
		  }
		  sendS38PUTMRequest(out_, file.DCLNAM, id);
		  sendS38BUFRequest(file, out_, id, file.RecordIncrement, listener, file.RecordLength, startingRecordNumber, batchSize);
		  out_.flush();

		  handleReply(file, "ddmS38PUTM", null);
		  startingRecordNumber += batchSize;
		}
	  }

	  /// <summary>
	  /// Updates the current record with the specified data.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCurrent(DDMFile file, byte[] data, int offset, boolean[] nullFieldValues) throws IOException
	  public virtual void updateCurrent(DDMFile file, sbyte[] data, int offset, bool[] nullFieldValues)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = sendS38UPDATRequest(out_, file.getDCLNAM());
		int id = sendS38UPDATRequest(out_, file.DCLNAM);
		file.EventBuffer.EventType = DDMCallbackEvent.EVENT_UPDATE;
		sendS38BUFRequest(out_, id, file.RecordIncrement, data, offset, file.RecordLength, nullFieldValues);
		out_.flush();

		handleReply(file, "ddmS38UPDAT", null);
	  }

	  /// <summary>
	  /// Updates the current record with the first record provided by the callback.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCurrent(DDMFile file, DDMWriteCallback listener) throws IOException
	  public virtual void updateCurrent(DDMFile file, DDMWriteCallback listener)
	  {
		int id = sendS38UPDATRequest(out_, file.DCLNAM);
		file.EventBuffer.EventType = DDMCallbackEvent.EVENT_UPDATE;
		sendS38BUFRequest(file, out_, id, file.RecordIncrement, listener, file.RecordLength, 0, 1);
		out_.flush();

		handleReply(file, "ddmS38UPDAT", null);
	  }

	  /// <summary>
	  /// Executes the specified CL command within the DDM host server job.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Message[] execute(final String command) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual Message[] execute(string command)
	  {
		  IList<Message> messageList = executeReturnMessageList(command);
		  Message[] messages = new Message[messageList.Count];
		  for (int i = 0; i < messages.Length; i++)
		  {
			  messages[i] = messageList[i];
		  }
		  return messages;
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List<Message> executeReturnMessageList(final String command) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual IList<Message> executeReturnMessageList(string command)
	  {
		List<Message> messages = new List<Message>();
		sendS38CMDRequest(out_, command);
		out_.flush();

		handleReply(null, "ddmS38CMD", null, messages);
		return messages;
	  }

	  /// <summary>
	  /// Removes the current record from the file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deleteCurrent(DDMFile file) throws IOException
	  public virtual void deleteCurrent(DDMFile file)
	  {
		sendS38DELRequest(out_, file.DCLNAM);
		out_.flush();

		handleReply(file, "ddmS38DEL", null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void position(DDMFile file, int positionType) throws IOException
	  private void position(DDMFile file, int positionType)
	  {
		sendS38FEODRequest(out_, file.DCLNAM, positionType);
		out_.flush();

		handleReply(file, "ddmS38FEOD", null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readType(DDMFile file, DDMReadCallback listener, int readType, boolean doPosition) throws IOException
	  private void readType(DDMFile file, DDMReadCallback listener, int readType, bool doPosition)
	  {
		sendS38GetRequest(out_, file.DCLNAM, file.ReadWriteType == DDMFile.READ_WRITE, readType, doPosition);
		out_.flush();

		handleReply(file, "ddmS38Get", listener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void handleReply(final DDMFile file, final String ds, final DDMReadCallback listener) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void handleReply(DDMFile file, string ds, DDMReadCallback listener)
	  {
		handleReply(file, ds, listener, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void handleReply(final DDMFile file, final String ds, final DDMReadCallback listener, final List<Message> messages) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void handleReply(DDMFile file, string ds, DDMReadCallback listener, IList<Message> messages)
	  {
		if (DEBUG)
		{
			Console.WriteLine("---- HANDLE REPLY ----");
		}
		int length = in_.readShort();
		bool isContinued = (length > 0x7FFF);
		if (isContinued)
		{
		  length = length & 0x7FFF; // The last 2 bytes are the size of the next continued packet.
		}
		if (DEBUG)
		{
			Console.WriteLine("LENGTH: " + length);
		}
		if (DEBUG)
		{
			Console.WriteLine("isContinued? " + isContinued);
		}
		if (length < 10)
		{
		  throw DataStreamException.badLength(ds, length);
		}
		int gdsID = in_.read();
		int type = in_.read(); // bit mask
		if (DEBUG)
		{
			Console.WriteLine("GDS, TYPE: " + gdsID + ", " + type);
		}
		bool isChained = (type & 0x40) != 0;
		int correlation = in_.readShort();
		int numRead = 6;
		if (DEBUG)
		{
			Console.WriteLine("CHAINED: " + isChained);
		}
		DataStreamException exception = null;
		while (numRead < length)
		{
		  if (DEBUG)
		  {
			  Console.WriteLine(numRead + " vs " + length);
		  }
		  int ll = in_.readShort();
		  int cp = in_.readShort();
		  numRead += 4;
		  if (DEBUG)
		  {
			  Console.WriteLine("HANDLEREPLY: ll=" + ll + ", cp=" + cp.ToString("x"));
		  }
		  if (cp == 0xD402) // S38IOFB
		  {
			// Probably end of file or record not found.
			bool end = !isChained;
			in_.skipBytes(length - numRead);
			numRead += length - numRead;
			while (isChained)
			{
			  int localLength = in_.readShort();
			  if (localLength < 10)
			  {
				throw DataStreamException.badLength(ds, localLength);
			  }
			  gdsID = in_.read();
			  type = in_.read(); // bit mask
			  isChained = (type & 0x40) != 0;
			  correlation = in_.readShort();
			  ll = in_.readShort();
			  cp = in_.readShort();
			  int localNumRead = 10;
			  numRead += 10;
			  if (DEBUG)
			  {
				  Console.WriteLine("Next is chained? " + isChained + ", " + cp.ToString("x"));
			  }
			  if (cp == 0xD201) // S38MSGRM
			  {
				int[] numMsgRead = new int[1];
				Message msg = getMessage(in_, ll, numMsgRead);
				numRead += numMsgRead[0];
				localNumRead += numMsgRead[0];
				if (DEBUG)
				{
					Console.WriteLine("Got message " + msg);
				}
				if (msg != null)
				{
				  if (messages != null)
				  {
					  messages.Add(msg);
				  }
				  string id = msg.ID;
				  if (!string.ReferenceEquals(id, null))
				  {
					if (id.Equals("CPF5001") || id.Equals("CPF5025"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.endOfFile(file.EventBuffer);
					  }
					  end = true;
					}
					else if (id.Equals("CPF5006"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.recordNotFound(file.EventBuffer);
					  }
					  end = true;
					}
					else if (messages == null)
					{
					  if (exception == null)
					  {
						exception = DataStreamException.errorMessage(ds + "_chained", msg);
						exception.fillInStackTrace();
					  }
					  else
					  {
						exception.addMessage(msg);
					  }
					  end = true;
					}
				  }
				  else if (messages == null)
				  {
					if (exception == null)
					{
					  exception = DataStreamException.errorMessage(ds + "_chained", msg);
					  exception.fillInStackTrace();
					}
					else
					{
					  exception.addMessage(msg);
					}
					end = true;
				  }
				}
			  }
			  else
			  {
				in_.skipBytes(ll - 4);
				numRead += ll - 4;
				localNumRead += ll - 4;
			  }
			  in_.skipBytes(localLength - localNumRead);
			  numRead += localLength - localNumRead;
			}
			if (!end)
			{
			  throw DataStreamException.badReply(ds, cp);
			}
		  }
		  else if (cp == 0xD405) // S38BUF
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean largeBuffer = ll > 0x7FFF;
			bool largeBuffer = ll > 0x7FFF;
			if (DEBUG)
			{
				Console.WriteLine("LARGE BUFFER: " + largeBuffer);
			}
			int ioFeedbackOffset = largeBuffer ? in_.readInt() + 18 : ll + 10;
			if (DEBUG)
			{
				Console.WriteLine("IOFB offset: " + ioFeedbackOffset);
			}
			numRead += largeBuffer ? 4 : 0;

			if (listener == null)
			{
			  int toSkip = ioFeedbackOffset - numRead - 4;
			  if (toSkip > 0)
			  {
			  in_.skipBytes(toSkip);
			  numRead += toSkip;
			  }
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int recordDataLength = file.getRecordLength();
			  int recordDataLength = file.RecordLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int recordIncrement = file.getRecordIncrement();
			  int recordIncrement = file.RecordIncrement;
			  if (DEBUG)
			  {
				  Console.WriteLine("Record data length: " + recordDataLength);
			  }
			  if (DEBUG)
			  {
				  Console.WriteLine("Record increment: " + recordIncrement);
			  }
			  while (numRead + recordIncrement <= ioFeedbackOffset)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] recordData = file.getRecordDataBuffer();
				sbyte[] recordData = file.RecordDataBuffer;

				bool didExtraBytes = false;
				if (isContinued && numRead + recordIncrement > length)
				{
				  // Not enough bytes left.
				  int diff = length - numRead;
				  if (DEBUG)
				  {
					  Console.WriteLine("Not enough bytes to read another record. Length=" + length + ", numRead=" + numRead + ", so remaining=" + diff);
				  }
				  int dataToRead = diff > recordData.Length ? recordData.Length : diff;
				  if (DEBUG)
				  {
					  Console.WriteLine("Data to read: " + dataToRead + " will make numRead: " + (numRead + dataToRead));
				  }
				  in_.readFully(recordData, 0, dataToRead);
				  numRead += dataToRead;
				  int remainingRecordData = recordData.Length - dataToRead;
				  if (DEBUG)
				  {
					  Console.WriteLine("Remaining record data: " + remainingRecordData);
				  }
				  if (remainingRecordData > 0)
				  {
					int nextPacketLength = in_.readShort();
					if (DEBUG)
					{
						Console.WriteLine("Next packet length: " + nextPacketLength);
					}
					if (nextPacketLength <= 0x7FFF)
					{
					  isContinued = false;
					}
					int extraLength = (nextPacketLength & 0x7FFF) - 2;
					if (DEBUG)
					{
						Console.WriteLine("Current length: " + length + "; Still continued? " + isContinued + "; next packet length: " + extraLength);
					}
					length += extraLength;
					if (DEBUG)
					{
						Console.WriteLine("New length: " + length);
					}
					in_.readFully(recordData, dataToRead, remainingRecordData);
					numRead += remainingRecordData;
				  }
				  else
				  {
					//TODO - This section can't be right, it's not handling the null field map.

					diff -= recordData.Length;
					// This reads in the remaining data for the record, including the 2 bytes for the next packet length.
					sbyte[] packetBuffer = file.PacketBuffer;
					in_.readFully(packetBuffer);
	//                if (DEBUG) System.out.println("JUST read "+packetBuffer.length+" bytes instead of "+diff);
					numRead += packetBuffer.Length;
	//                numRead += diff+2;
					int nextPacketLength = ((packetBuffer[diff] & 0x00FF) << 8) | (packetBuffer[diff + 1] & 0x00FF);
					if (DEBUG)
					{
						Console.WriteLine("NEXT packet length: " + nextPacketLength);
					}
					if (nextPacketLength <= 0x7FFF)
					{
					  isContinued = false;
					}
					int extraLength = (nextPacketLength & 0x7FFF); // - 2;
					if (DEBUG)
					{
						Console.WriteLine("CURRENT length: " + length + "; Still continued? " + isContinued + "; next packet length: " + extraLength);
					}
					length += extraLength;
					if (DEBUG)
					{
						Console.WriteLine("NEW length: " + length);
					}

					int relative = 0;
					int recordNumber = -1;
					if (diff < 3)
					{
					  recordNumber = Conv.byteArrayToInt(packetBuffer, 4);
					  relative = 8;
					}
					else if (diff > 5)
					{
					  recordNumber = Conv.byteArrayToInt(packetBuffer, 2);
					  relative = 6;
					}
					else
					{
					  // The packet busted us in the middle of the record number.
					  int offset = 2;
					  int b1 = packetBuffer[offset++];
					  if (diff == offset)
					  {
						  offset += 2;
					  }
					  int b2 = packetBuffer[offset++];
					  if (diff == offset)
					  {
						  offset += 2;
					  }
					  int b3 = packetBuffer[offset++];
					  if (diff == offset)
					  {
						  offset += 2;
					  }
					  int b4 = packetBuffer[offset++];
					  recordNumber = ((b1 & 0x00FF) << 24) | ((b2 & 0x00FF) << 16) | ((b3 & 0x00FF) << 8) | (b4 & 0x00FF);
					  relative = offset;
					}

					if (DEBUG)
					{
						Console.WriteLine("Relative is " + relative + " and +5 for the null field map.");
					}
	//                relative = file.getNullFieldByteMapOffset()-file.getRecordLength()-relative;
					relative += 5;

					// Read null field map.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] nullFieldMap = file.getNullFieldMap();
					sbyte[] nullFieldMap = file.NullFieldMap;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] nullFieldValues = file.getNullFieldValues();
					bool[] nullFieldValues = file.NullFieldValues;
					for (int i = 0; i < nullFieldValues.Length; ++i)
					{
	//                  nullFieldValues[i] = (packetBuffer[relative++] == (byte)0xF1);
					  if (relative == diff || relative == diff + 1)
					  {
						// Skip the next packet header.
						--i;
					  }
					  else
					  {
						sbyte b = packetBuffer[relative];
						if (b == unchecked((sbyte)0xF1))
						{
						  nullFieldValues[i] = true;
						  nullFieldMap[i] = b;
						}
						else if (b == unchecked((sbyte)0xF0))
						{
						  nullFieldValues[i] = false;
						  nullFieldMap[i] = b;
						}
						else
						{
						  if (DEBUG)
						  {
							  Console.WriteLine("Packet buffer length is " + packetBuffer.Length);
						  }
						  if (DEBUG)
						  {
							  Console.WriteLine("Bad null field map value " + i + " at offset " + (relative-1) + ": 0x" + (b & 0x00FF).ToString("x"));
						  }
						  throw new IOException("Bad null field map value: " + b.ToString("x"));
						}
					  }
					  ++relative;
					}
					DDMCallbackEvent ev = file.EventBuffer;
					ev.EventType = DDMCallbackEvent.EVENT_READ;
					DDMDataBuffer dataBuffer = file.getDataBuffer(file.CurrentBufferIndex);
					dataBuffer.RecordNumber = recordNumber;
					listener.newRecord(ev, dataBuffer);
					file.nextBuffer();
					didExtraBytes = true;
					if (DEBUG)
					{
						Console.WriteLine("Did extra bytes.");
					}
				  }
				}
				else
				{
				  if (DEBUG)
				  {
					  Console.WriteLine("Read normal record. " + recordData.Length + " bytes:");
				  }
				  in_.readFully(recordData);
				  numRead += recordData.Length;
				}
				if (!didExtraBytes)
				{
				  in_.skipBytes(2);
				  int recordNumber = in_.readInt();
				  // Skip bytes between here and null field map.
				  numRead += 6;
	//              int relative = file.getNullFieldByteMapOffset()-file.getRecordLength()-6;
				  int relative = 5;
				  if (DEBUG)
				  {
					  Console.WriteLine("Skipping " + relative + " bytes because null field map offset is " + file.NullFieldByteMapOffset);
				  }
				  in_.skipBytes(relative);
				  numRead += relative;
				  // Read null field map.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] nullFieldMap = file.getNullFieldMap();
				  sbyte[] nullFieldMap = file.NullFieldMap;
				  in_.readFully(nullFieldMap);
				  numRead += nullFieldMap.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] nullFieldValues = file.getNullFieldValues();
				  bool[] nullFieldValues = file.NullFieldValues;
				  for (int i = 0; i < nullFieldMap.Length; ++i)
				  {
					sbyte b = nullFieldMap[i];
					if (b == unchecked((sbyte)0xF1))
					{
					  nullFieldValues[i] = true;
					}
					else if (b == unchecked((sbyte)0xF0))
					{
					  nullFieldValues[i] = false;
					}
					else
					{
					  throw new IOException("Bad null field map value: " + b.ToString("x"));
					}
	//                nullFieldValues[i] = (nullFieldMap[i] == (byte)0xF1);
				  }
				  DDMCallbackEvent ev = file.EventBuffer;
				  ev.EventType = DDMCallbackEvent.EVENT_READ;
				  DDMDataBuffer dataBuffer = file.getDataBuffer(file.CurrentBufferIndex);
				  dataBuffer.RecordNumber = recordNumber;
				  listener.newRecord(ev, dataBuffer);
				  file.nextBuffer();
				}
				if (DEBUG)
				{
					Console.WriteLine("NUMREAD: " + numRead);
				}
			  }
			  int toSkip = ioFeedbackOffset - numRead - 4;
	//          int lenSkip = length-numRead;
	//          if (toSkip > lenSkip) toSkip = lenSkip;
			  if (DEBUG)
			  {
				  Console.WriteLine("internal skip " + toSkip);
			  }
			  if (toSkip > 0)
			  {
			  in_.skipBytes(toSkip);
			  numRead += toSkip;
			  }
			  if (ll < 32768 && ll < file.RecordIncrement * file.BatchSize)
			  {
				//TODO: Have probably read all the records??
				file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
				listener.endOfFile(file.EventBuffer);
			  }
			}
			// IOFB
			if (DEBUG)
			{
				Console.WriteLine("Before IOFB, numRead = " + numRead + ", length=" + length);
			}
			int toSkip = length - numRead;
			if (DEBUG)
			{
				Console.WriteLine("Skipping " + toSkip + " maybe");
			}
			if (toSkip > 0)
			{
			  in_.skipBytes(toSkip);
			  numRead += toSkip;
			}
			while (isChained)
			{
			  int localLength = in_.readShort();
			  if (DEBUG)
			  {
				  Console.WriteLine("In loop, length = " + localLength);
			  }
			  if (DEBUG)
			  {
				  Console.WriteLine("3isContinued? " + ((localLength & 0x8000) != 0));
			  }
			  if (localLength < 10)
			  {
				throw DataStreamException.badLength(ds + "_chained", localLength);
			  }
			  gdsID = in_.read();
			  type = in_.read(); // bit mask
			  isChained = (type & 0x40) != 0;
			  if (DEBUG)
			  {
				  Console.WriteLine("CHAIN2: " + isChained);
			  }
			  correlation = in_.readShort();
			  ll = in_.readShort();
			  cp = in_.readShort();
			  if (DEBUG)
			  {
				  Console.WriteLine("CP in chain: " + cp.ToString("x"));
			  }
			  int localNumRead = 10;
			  numRead += 10;
			  if (cp == 0xD201) // S38MSGRM
			  {
				int[] numMsgRead = new int[1];
				Message msg = getMessage(in_, ll, numMsgRead);
				localNumRead += numMsgRead[0];
				numRead += numMsgRead[0];
				if (msg != null)
				{
				  if (messages != null)
				  {
					  messages.Add(msg);
				  }
				  string id = msg.ID;
				  if (!string.ReferenceEquals(id, null))
				  {
					if (id.Equals("CPF5001") || id.Equals("CPF5025"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.endOfFile(file.EventBuffer);
					  }
					}
					else if (id.Equals("CPF5006"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.recordNotFound(file.EventBuffer);
					  }
					}
					else if (messages == null)
					{
					  throw DataStreamException.errorMessage(ds + "_chained", msg);
					}
				  }
				  else if (messages == null)
				  {
					throw DataStreamException.errorMessage(ds + "_chained", msg);
				  }
				}
			  }
			  else if (cp == 0xD402) // S38IOFB
			  {
				if (DEBUG)
				{
					Console.WriteLine("SKipping " + (localLength - localNumRead));
				}
				in_.skipBytes(localLength - localNumRead);
				numRead += (localLength - localNumRead);
				localNumRead = localLength;
			  }
			  else
			  {
				throw DataStreamException.badReply(ds + "_chained", cp);
			  }
			  if (DEBUG)
			  {
				  Console.WriteLine("Skipping extra " + (localLength - localNumRead));
			  }
			  in_.skipBytes(localLength - localNumRead);
			  numRead += localLength - localNumRead;
			}
			if (DEBUG)
			{
				Console.WriteLine("Should be at end: " + numRead + " and " + length);
			}
		  }
		  else if (cp == 0xD201) // S38MSGRM
		  {
			int[] numMsgRead = new int[1];
			Message msg = getMessage(in_, ll, numMsgRead);
			numRead += numMsgRead[0];
			if (msg != null)
			{
			  if (messages != null)
			  {
				messages.Add(msg);
			  }
			  else
			  {
				throw DataStreamException.errorMessage(ds, msg);
			  }
			}
			int toSkip = length - numRead;
			if (DEBUG)
			{
				Console.WriteLine("Skipping " + toSkip + " mmaybe");
			}
			if (toSkip > 0)
			{
			  in_.skipBytes(toSkip);
			  numRead += toSkip;
			}
			while (isChained)
			{
			  int localLength = in_.readShort();
			  if (DEBUG)
			  {
				  Console.WriteLine("In loop, length = " + localLength);
			  }
			  if (DEBUG)
			  {
				  Console.WriteLine("3isContinued? " + ((localLength & 0x8000) != 0));
			  }
			  if (localLength < 10)
			  {
				throw DataStreamException.badLength(ds + "_chained", localLength);
			  }
			  gdsID = in_.read();
			  type = in_.read(); // bit mask
			  isChained = (type & 0x40) != 0;
			  if (DEBUG)
			  {
				  Console.WriteLine("CHAIN2: " + isChained);
			  }
			  correlation = in_.readShort();
			  ll = in_.readShort();
			  cp = in_.readShort();
			  if (DEBUG)
			  {
				  Console.WriteLine("CP in chain: " + cp.ToString("x"));
			  }
			  int localNumRead = 10;
			  numRead += 10;
			  if (cp == 0xD201) // S38MSGRM
			  {
				numMsgRead = new int[1];
				msg = getMessage(in_, ll, numMsgRead);
				localNumRead += numMsgRead[0];
				numRead += numMsgRead[0];
				if (msg != null)
				{
				  if (messages != null)
				  {
					  messages.Add(msg);
				  }
				  string id = msg.ID;
				  if (!string.ReferenceEquals(id, null))
				  {
					if (id.Equals("CPF5001") || id.Equals("CPF5025"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.endOfFile(file.EventBuffer);
					  }
					}
					else if (id.Equals("CPF5006"))
					{
					  if (listener != null)
					  {
						file.EventBuffer.EventType = DDMCallbackEvent.EVENT_READ;
						listener.recordNotFound(file.EventBuffer);
					  }
					}
					else if (messages == null)
					{
					  throw DataStreamException.errorMessage(ds + "_chained", msg);
					}
				  }
				  else if (messages == null)
				  {
					throw DataStreamException.errorMessage(ds + "_chained", msg);
				  }
				}
			  }
			  else if (cp == 0xD402) // S38IOFB
			  {
				if (DEBUG)
				{
					Console.WriteLine("SKipping " + (localLength - localNumRead));
				}
				in_.skipBytes(localLength - localNumRead);
				numRead += (localLength - localNumRead);
				localNumRead = localLength;
			  }
			  else
			  {
				throw DataStreamException.badReply(ds + "_chained", cp);
			  }
			  if (DEBUG)
			  {
				  Console.WriteLine("Skipping extra " + (localLength - localNumRead));
			  }
			  in_.skipBytes(localLength - localNumRead);
			  numRead += localLength - localNumRead;
			}
		  }
		  else
		  {
			throw DataStreamException.badReply(ds, cp);
		  }
		}
		in_.end();
		if (exception != null)
		{
		  throw exception;
		}
	  }

	  // The declared name is an 8-byte unique handle that DDM will use to quickly access the file.
	  // I assume it is unique within a given connection. It must be EBCDIC and blank-padded.
	  // Which means, if we use an 8-digit number, we can only have 100,000,000 files open simultaneously. :-)
	  private sbyte[] generateDCLNAM()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] dclName = new byte[8];
		sbyte[] dclName = new sbyte[8];

		int num = dclNamCounter_++; // No need to synchronize, this class isn't threadsafe anyway.

		// Hooray for loop-unrolling and temp variables!
		// This works great with the IBM 1.5 compiler and JIT. Not so much with Sun 1.4.
		int mod = num % 10;
		dclName[7] = unchecked((sbyte)(mod + 0xF0));
		num = num / 10;
		mod = num % 10;
		dclName[6] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[5] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[4] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[3] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[2] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[1] = unchecked((sbyte)(mod + 0xF0));
		num = num == 0 ? 0 : num / 10;
		mod = num == 0 ? 0 : num % 10;
		dclName[0] = unchecked((sbyte)(mod + 0xF0));
		return dclName;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38CMDRequest(HostOutputStream out, String command) throws IOException
	  private void sendS38CMDRequest(HostOutputStream @out, string command)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] commandBytes = Conv.stringToEBCDICByteArray37(command);
		sbyte[] commandBytes = Conv.stringToEBCDICByteArray37(command);
		@out.writeShort(14 + commandBytes.Length); // Length.
		@out.write(0xD0); // SNA GDS architecture ID.
		@out.write(1); // Format ID.
		@out.writeShort(newCorrelationID());
		@out.writeShort(8 + commandBytes.Length); // S38CMD LL.
		@out.writeShort(0xD006); // S38CMD CP.
		@out.writeShort(4 + commandBytes.Length); // S38CMDST LL.
		@out.writeShort(0xD103); // S38CMDST CP.
		@out.write(commandBytes);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38DELRequest(final HostOutputStream out, final byte[] dclNam) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendS38DELRequest(HostOutputStream @out, sbyte[] dclNam)
	  {
		@out.writeInt(0x0016D001); // Combined length, SNA GDS arch ID, format ID.
		@out.writeShort(newCorrelationID());
		@out.writeInt(0x0010D007); // Combined S38DEL LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int sendS38UPDATRequest(HostOutputStream out, byte[] dclNam) throws IOException
	  private int sendS38UPDATRequest(HostOutputStream @out, sbyte[] dclNam)
	  {
		@out.writeInt(0x001ED051); // Combined length, SNA GDS arch ID, and format ID.
		int id = newCorrelationID();
		@out.writeShort(id);
		@out.writeInt(0x0018D019); // Combined S38UPDAT LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
		@out.writeInt(0x0008D119); // Combined S38OPTL LL and CP.
	//    out.write(33); // Type of read (current).
	//    out.write(3); // Share - update norm.
	//    out.write(1); // Data - don't retrieve record, ignore deleted records.
	//    out.write(7); // Operation - update.
		@out.writeInt(0x21030107); // Combined options.
		return id;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38PUTMRequest(HostOutputStream out, byte[] dclNam, final int correlationID) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendS38PUTMRequest(HostOutputStream @out, sbyte[] dclNam, int correlationID)
	  {
		@out.writeInt(0x0016D051); // Combined length, SNA GDS arch ID, and format ID.
		@out.writeShort(correlationID);
		@out.writeInt(0x0010D013); // Combined S38PUTM LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38BUFRequest(HostOutputStream out, final int correlationID, final int recordIncrement, final byte[] data, final int offset, final int length, final boolean[] nullFieldValues) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendS38BUFRequest(HostOutputStream @out, int correlationID, int recordIncrement, sbyte[] data, int offset, int length, bool[] nullFieldValues)
	  {
		// Now send S38BUF with data in it.
		@out.writeShort(recordIncrement + 10); // Length.
		@out.writeShort(0xD003); // Combined SNA GDS arch ID and format ID.
		@out.writeShort(correlationID);
		@out.writeShort(recordIncrement + 4); // S38BUF LL.
		@out.writeShort(0xD405); // S38BUF CP.
		@out.write(data, offset, length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int remaining = recordIncrement - length;
		int remaining = recordIncrement - length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean doNullFields = nullFieldValues != null;
		bool doNullFields = nullFieldValues != null;
		for (int i = 0; i < remaining; ++i)
		{
		  int toWrite = (doNullFields && i < nullFieldValues.Length) ? (nullFieldValues[i] ? 0xF1 : 0xF0) : 0xF0;
		  @out.write(toWrite);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38BUFRequest(DDMFile file, HostOutputStream out, final int correlationID, final int recordIncrement, DDMWriteCallback listener, final int length, final int startingRecordNumber, final int batchSize) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendS38BUFRequest(DDMFile file, HostOutputStream @out, int correlationID, int recordIncrement, DDMWriteCallback listener, int length, int startingRecordNumber, int batchSize)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int total = batchSize*recordIncrement;
		int total = batchSize * recordIncrement;

		// Now send S38BUF with data in it.
		@out.writeShort(total + 10); // Length.
		@out.write(0xD003); // Combined SNA GDS arch ID and format ID.
		@out.writeShort(correlationID);
		@out.writeShort(total + 4); // S38BUF LL.
		@out.writeShort(0xD405); // S38BUF CP.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int limit = startingRecordNumber + batchSize;
		int limit = startingRecordNumber + batchSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMCallbackEvent event = file.getEventBuffer();
		DDMCallbackEvent @event = file.EventBuffer;
		for (int i = startingRecordNumber; i < limit; ++i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] data = listener.getRecordData(event, i);
		  sbyte[] data = listener.getRecordData(@event, i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = listener.getRecordDataOffset(event, i);
		  int offset = listener.getRecordDataOffset(@event, i);
		  @out.write(data, offset, length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int remaining = recordIncrement - length;
		  int remaining = recordIncrement - length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] nullFieldValues = listener.getNullFieldValues(event, i);
		  bool[] nullFieldValues = listener.getNullFieldValues(@event, i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean doNullFields = nullFieldValues != null;
		  bool doNullFields = nullFieldValues != null;
		  for (int j = 0; j < remaining; ++j)
		  {
			int toWrite = (doNullFields && j < nullFieldValues.Length) ? (nullFieldValues[j] ? 0xF1 : 0xF0) : 0xF0;
			@out.write(toWrite);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38GetRequest(HostOutputStream out, byte[] dclNam, boolean forUpdate, int readType, boolean doPosition) throws IOException
	  private void sendS38GetRequest(HostOutputStream @out, sbyte[] dclNam, bool forUpdate, int readType, bool doPosition)
	  {
		@out.writeInt(0x001ED001); // Combined length, SNA GDS architecture ID, and format ID.
		@out.writeShort(newCorrelationID()); // Request correlation ID.
		@out.writeInt(0x0018D00C); // Combined S38GET LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
		@out.writeInt(0x0008D119); // Combined S38OPTL LL and CP.
		@out.write(readType); // Type of read (next, prev, first, last, current).
		@out.write(forUpdate ? 3 : 0); // Share - update norm, or read norm.
		@out.write(doPosition ? 1 : 0); // Data - don't retrieve or do retrieve record, ignore deleted records.
		@out.write(1); // Operation - get.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38GetKRequest(HostOutputStream out, byte[] dclNam, boolean forUpdate, byte[] recordFormatName, int readType, byte[] key, int numberOfKeyFields, boolean doPosition) throws IOException
	  private void sendS38GetKRequest(HostOutputStream @out, sbyte[] dclNam, bool forUpdate, sbyte[] recordFormatName, int readType, sbyte[] key, int numberOfKeyFields, bool doPosition)
	  {
		@out.writeShort(63 + key.Length); // Length;
		@out.writeShort(0xD001); // Combined SNA GDS architecture ID and format ID.
		@out.writeShort(newCorrelationID()); // Request correlation ID.
		@out.writeShort(57 + key.Length); // S38GETK LL.
		@out.writeInt(unchecked((int)0xD00E000C)); // Combined S38GETK CP and DCLNAM LL.
		@out.writeShort(0x1136); // DCLNAM CP.
		@out.write(dclNam, 0, 8);
		@out.writeInt(0x0008D119); // Combined S38OPTL LL and CP.
		@out.write(readType); // Type of read (next, prev, first, last, current).
		if (DEBUG)
		{
			Console.WriteLine("Read type? 0x" + readType.ToString("x"));
		}
		if (DEBUG)
		{
			Console.WriteLine("For UPDATE? " + forUpdate);
		}
		@out.write(forUpdate ? 3 : 0); // Share - update norm, or read norm.
		@out.writeShort(doPosition ? 0x0103 : 0x0003); // Combined data (retrieve record, ignore deleted records) and operation (getk).

		@out.writeShort(33 + key.Length); // S38CTLL LL.
		@out.writeInt(unchecked((int)0xD1050100)); // Combined S38CTLL CP (Control list), record format ID, and record format length.
		@out.write(10);
		if (DEBUG)
		{
			Console.WriteLine("recfmt length is " + recordFormatName.Length);
		}
		@out.write(recordFormatName);
		@out.writeInt(0x0F000200); // Combined member number ID, member number length, half of member number value.
		@out.writeInt(0x00080004); // Combined half of member number value, number of fields ID, and number of fields length.
		@out.writeInt(numberOfKeyFields);

		@out.write(7);
		@out.writeShort(key.Length);
		@out.write(key);

		@out.write(0xFF); // Control list end.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38GetDRequest(HostOutputStream out, byte[] dclNam, boolean forUpdate, byte[] recordFormatName, int recordNumber, boolean doPosition) throws IOException
	  private void sendS38GetDRequest(HostOutputStream @out, sbyte[] dclNam, bool forUpdate, sbyte[] recordFormatName, int recordNumber, bool doPosition)
	  {
		@out.writeInt(0x003CD001); // Combined length, SNA GDS architecture ID, and format ID.
		@out.writeShort(newCorrelationID()); // Request correlation ID.
		@out.writeInt(0x0036D00D); // Combined S38GETD LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
		@out.writeInt(0x0008D119); // Combined S38OPTL LL and CP.
		@out.write(8); // Type of read - definite.
		@out.write(forUpdate ? 3 : 0); // Share - update norm, or read norm.
		@out.write(doPosition ? 1 : 0); // Data - don't retrieve or do retrieve record, ignore deleted records.
		@out.writeInt(0x02001ED1); // Combind operation (getd), S38CTLLL, and half of S38CTLL CP (Control list).
		@out.writeInt(0x0501000C); // Combined half of S38CTLL CP (Control list), record format ID, and record format length.
		@out.write(recordFormatName);
		@out.writeInt(0x0F000200); // Combined member number ID, member number length, and half of member number value.
		@out.writeInt(0x00020004); // Combined half of member number value, relative record number ID, and relative record number length.
		@out.writeInt(recordNumber);
		@out.write(0xFF); // Control list end.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38FEODRequest(HostOutputStream out, byte[] dclNam, int positionType) throws IOException
	  private void sendS38FEODRequest(HostOutputStream @out, sbyte[] dclNam, int positionType)
	  {
		@out.writeInt(0x001ED051); // Combined length, SNA GDS architecture ID, and format ID (this is a chained request, same correlation ID on chain).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = newCorrelationID();
		int id = newCorrelationID();
		@out.writeShort(id); // Request correlation ID.
		@out.writeInt(0x0018D00B); // Combined S38FEOD LL and CP.
		@out.writeInt(0x000C1136); // Combined DCLNAM LL and CP.
		@out.write(dclNam, 0, 8);
		@out.writeInt(0x0008D119); // Combined S38OPTL LL and CP.
		@out.write(positionType); // Type of read (next, prev, first, last, current).
		@out.writeInt(0x02010100); // Combined share (read and release previous lock), data (do not retrieve record, ignore deleted records), operation (get), and half of chained S38BUF length.

		// Now send chained S38BUF. This is what the protocol expects.
		@out.write(0x0B); // Other half of chained S38BUF length.
		@out.writeShort(0xD003); // Combined SNA GDS architecture ID and format ID.
		@out.writeShort(id);
		@out.writeInt(0x0005D405); // Combined S38BUF LL and CP.
		@out.write(0); // Value.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38CloseRequest(HostOutputStream out, byte[] dclNam) throws IOException
	  private void sendS38CloseRequest(HostOutputStream @out, sbyte[] dclNam)
	  {
		@out.writeInt(0x001BD001); // Combined length, SNA GDS architecture ID, and format ID.
		@out.writeShort(newCorrelationID()); // Request correlation ID.
		@out.writeShort(21); // S38CLOSE LL.
		@out.writeShort(0xD004); // S38CLOSE CP.
		@out.writeShort(12); // DCLNAM LL.
		@out.writeShort(0x1136); // DCLNAM CP.
		@out.write(dclNam, 0, 8);
		@out.writeShort(5); // S38CLOST LL.
		@out.writeShort(0xD121); // S38CLOST CP.
		@out.write(0x02); // S38CLOST value. 2 means permanent close.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendS38OpenRequest(HostOutputStream out, String file, String library, String member, boolean doRead, boolean doWrite, boolean keyed, String recordFormatName, final byte[] dclNam, int batchSize) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendS38OpenRequest(HostOutputStream @out, string file, string library, string member, bool doRead, bool doWrite, bool keyed, string recordFormatName, sbyte[] dclNam, int batchSize)
	  {
		if (!doRead && !doWrite)
		{
			doRead = true;
		}
		const bool commitmentControl = false;
		const bool userBuffer = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ufcbLength = 106 + (commitmentControl ? 3 : 0) + (keyed || doRead ? 3 : 0);
		int ufcbLength = 106 + (commitmentControl ? 3 : 0) + (keyed || doRead ? 3 : 0);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int totalLength = 26+ufcbLength;
		int totalLength = 26 + ufcbLength;

		if (batchSize < 1 || (doRead && doWrite))
		{
		  batchSize = 1;
		}
		batchSize = batchSize & 0x7FFF;

		@out.writeShort(totalLength);
		@out.write(0xD0); // SNA GDS architecture ID.
		// Format ID:
		// continue on error mask = 0010 0000
		// type mask              = 0000 0011  1 = RQSDSS, 2 = RPYDSS, 3 = OBJDSS
		// same correlation mask  = 0001 0000
		// chained mask           = 0100 0000
		@out.write(1); // Format ID.
		@out.writeShort(newCorrelationID()); // Request correlation ID.
		@out.writeShort(20 + ufcbLength); // S38OPEN LL.
		@out.writeShort(0xD011); // S38OPEN CP.
		@out.writeShort(12); // DCLNAM LL.
		@out.writeShort(0x1136); // DCLNAM CP.
		@out.write(dclNam); // 8 bytes.
		@out.writeShort(4 + ufcbLength); // S38UFCB LL.
		@out.writeShort(0xD11F); // S38UFCB CP.
		int numWritten = 18 + dclNam.Length;
		// UFCB.
		writePadEBCDIC10(file, @out); // Filename.
		@out.writeShort(72); // WDMHLIB.
		writePadEBCDIC10(library, @out); // Library.
		@out.writeShort(73); // WDMHMBR.
		writePadEBCDIC10(member, @out); // Member.
		numWritten += 34;
		@out.writeInt(0);
		@out.writeInt(0);
		@out.writeInt(0); // Skip 12 bytes.
		numWritten += 12;
		int openOptions = userBuffer ? 0x1003 : 0x1002;
		if (doRead && !doWrite)
		{
			openOptions |= 0x0020; // Read-only.
		}
		else if (doRead && doWrite)
		{
			openOptions |= 0x3C; // Read-write.
		}
		else
		{
			openOptions |= 0x10;
		}
		@out.writeShort(openOptions); // index 46
		@out.writeInt(unchecked((int)0xF0F1F0F0)); // Release and version numbers;
		@out.writeInt(0); // Skip 4 bytes.
		@out.writeInt(0x20000000); // Record blocking on, skip 3 bytes.
		@out.writeInt(0x02000000); // Handle null-capable fields, skip 3 bytes.  index 60-63
		@out.writeInt(0);
		@out.writeInt(0);
		@out.writeInt(0);
		@out.writeInt(0); // Skip 16 bytes.
		@out.writeShort(6); // LVLCHK CP.
		@out.write(0); // Don't do LVLCHK.
		numWritten += 37;
		if (keyed || doRead)
		{
		  @out.writeShort(60); // ARRSEQ CP.
		  if (DEBUG)
		  {
			  Console.WriteLine("Opening keyed? " + keyed + ", read? " + doRead);
		  }
		  @out.write(!keyed || !doRead ? 0x80 : 0x00); // 0x80 = arrival sequenece, 0x00 = keyed access path.
		  numWritten += 3;
		}
		if (commitmentControl)
		{
		  @out.writeShort(59); // COMMIT CP.
		  @out.write(0x00); // Commit lock level: none = 0x00, default = 0x80, *CHG = 0x82, *CS = 0x86, *ALL = 0x87.
		  numWritten += 3;
		}
		@out.writeShort(58); // SEQONLY CP.
		@out.write(doRead && doWrite ? 0x40 : 0xC0); // 0x40 = NO, 0xC0 = YES.
		@out.writeShort(batchSize); // Blocking factor.
		@out.writeShort(9); // Record format group CP.
		@out.writeShort(1); // Max # of record formats.
		@out.writeShort(1); // Cur # of record formats.
		writePadEBCDIC10(recordFormatName, @out);
		@out.writeShort(32767); // End of variable-length UFCB.
		numWritten += 23;
		if (DEBUG)
		{
			Console.WriteLine("NUM written vs TOTAL length on OPEN: " + numWritten + ", " + totalLength);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendSECCHKRequest(HostOutputStream out, byte[] userBytes, byte[] encryptedPassword) throws IOException
	  private static void sendSECCHKRequest(HostOutputStream @out, sbyte[] userBytes, sbyte[] encryptedPassword)
	  {
		@out.writeShort(34 + encryptedPassword.Length);
		@out.write(0xD0); // GDS ID.
		@out.write(1); // Type is RQSDSS.
		@out.writeShort(0); // Skip 2 bytes.
		@out.writeShort(28 + encryptedPassword.Length); // SECCHK LL.
		@out.writeShort(0x106E); // SECCHK CP.
		@out.writeShort(6); // SECMEC LL.
		@out.writeShort(0x11A2); // SECMEC CP.
		@out.writeShort(encryptedPassword.Length == 20 ? 8 : 6); // SHA or DES.
		@out.writeShort(14); // USRID LL.
		@out.writeShort(0x11A0); // USRID CP.
		@out.write(userBytes, 0, 10);
		@out.writeShort(4 + encryptedPassword.Length); // PASSWORD LL.
		@out.writeShort(0x11A1); // PASSWORD CP.
		@out.write(encryptedPassword);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long sendACCSECRequest(HostOutputStream out, boolean useStrongEncryption) throws IOException
	  private static long sendACCSECRequest(HostOutputStream @out, bool useStrongEncryption)
	  {
		@out.writeShort(28); // Length.
		@out.write(0xD0); // GDS ID.
		@out.write(1); // Type is RQSDSS.
		@out.writeShort(1); // Correlation ID.
		@out.writeShort(22); // ACCSEC LL.
		@out.writeShort(0x106D); // ACCSEC CP.
		@out.writeShort(6); // SECMEC LL.
		@out.writeShort(0x11A2); // SECMEC CP.
		@out.writeShort(useStrongEncryption ? 8 : 6);
		@out.writeShort(12); // SECTKN LL.
		@out.writeShort(0x11DC); // SECTKN CP.
		long clientSeed = DateTimeHelper.CurrentUnixTimeMillis();
		@out.writeLong(clientSeed);
		return clientSeed;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendEXCSATRequest(HostOutputStream out) throws IOException
	  private static void sendEXCSATRequest(HostOutputStream @out)
	  {
		@out.writeShort(126); // Length.
		@out.write(0xD0); // GDS ID.
		@out.write(1); // Type is RQSDSS.
		@out.writeShort(0); // Skip 2 bytes.
		@out.writeShort(120); // EXCSAT LL.
		@out.writeShort(0x1041); // EXCSAT CP.
		@out.writeShort(9); // EXTNAM LL.
		@out.writeShort(0x115E); // EXTNAM CP.
		@out.writeInt(unchecked((int)0xE3C2D6E7));
		@out.write(0xF2); // EXTNAM - EBCDIC "TBOX2".
		@out.writeShort(11); // SRVCLSNM LL.
		@out.writeShort(0x1147); // SRVCLSNM CP.
		@out.writeShort(7); // CHRSTRDR LL.
		@out.writeShort(0x0009); // CHRSTRDR CP.
		@out.writeShort(0xD8C1);
		@out.write(0xE2); // SRVCLSNM - EBCDIC "QA5".
		@out.writeShort(96); // MGRLVLLS LL.
		@out.writeShort(0x1404); // MGRLVLLS CP.
		@out.writeShort(0x1403); // AGENT CP.
		@out.writeShort(3);
		@out.writeShort(0x1423); // ALTINDF CP.
		@out.writeShort(3);
		@out.writeShort(0x1405); // CMBACCAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1406); // CMBKEYAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1407); // CMBRNBAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1474); // CMNTCPIP CP.
		@out.writeShort(5);
		@out.writeShort(0x1458); // DICTIONARY CP.
		@out.writeShort(1);
		@out.writeShort(0x1457); // DIRECTORY CP.
		@out.writeShort(3);
		@out.writeShort(0x140C); // DIRFIL CP.
		@out.writeShort(3);
		@out.writeShort(0x1419); // DRCAM CP.
		@out.writeShort(3);
		@out.writeShort(0x141E); // KEYFIL CP.
		@out.writeShort(3);
		@out.writeShort(0x1422); // LCKMGR CP.
		@out.writeShort(3);
		@out.writeShort(0x240F); // RDB CP.
		@out.writeShort(3);
		@out.writeShort(0x1432); // RELKEYAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1433); // RELRNBAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1440); // SECMGR CP.
		@out.writeShort(1);
		@out.writeShort(0x143B); // SEQFIL CP.
		@out.writeShort(3);
		@out.writeShort(0x2407); // SQLAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1463); // STRAM CP.
		@out.writeShort(3);
		@out.writeShort(0x1465); // STRFIL CP.
		@out.writeShort(3);
		@out.writeShort(0x143C); // SUPERVISOR CP.
		@out.writeShort(3);
		@out.writeShort(0x147F); // SYSCMDMGR CP.
		@out.writeShort(4);
		@out.writeShort(0x14A0); // RSCRCVM CP.
		@out.writeShort(4);
	  }
	}

}