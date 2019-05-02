using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  FileConnection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.file
{
	using com.ibm.jtopenlite;

	/// <summary>
	/// <para>Represents a TCP/IP socket connection to the System i File host server
	///  (QSERVER/QPWFSERVSO prestart jobs).
	/// </para>
	/// <para>For an example, see <seealso cref="com.ibm.jtopenlite.samples.AccessIfsFile"/>. 
	/// 
	/// </para>
	/// </summary>
	public class FileConnection : HostServerConnection //implements Connection
	{
		public const int DEFAULT_FILE_SERVER_PORT = 8473;
		public const int DEFAULT_SSL_FILE_SERVER_PORT = 9473;

	  private int ccsid_;
	  private int datastreamLevel_;
	  private int maxDataBlockSize_;

	  private int correlationID_ = 1;

	  private int newCorrelationID()
	  {
		if (correlationID_ == 0x7FFFFFFF)
		{
			correlationID_ = 0;
		}
		return ++correlationID_;
	  }

	  private FileConnection(SystemInfo info, Socket socket, HostInputStream @in, HostOutputStream @out, int ccsid, int datastreamLevel, int maxDataBlockSize, string user, string jobName) : base(info, user, jobName, socket, @in, @out)
	  {
		ccsid_ = ccsid;
		datastreamLevel_ = datastreamLevel;
		maxDataBlockSize_ = maxDataBlockSize;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendEndJobRequest() throws IOException
	  protected internal override void sendEndJobRequest()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(String system, String user, String password) throws IOException
	  public static FileConnection getConnection(string system, string user, string password)
	  {
		return getConnection(false, system, user, password);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(final boolean isSSL, String system, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static FileConnection getConnection(bool isSSL, string system, string user, string password)
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(SystemInfo info, String user, String password) throws IOException
	  public static FileConnection getConnection(SystemInfo info, string user, string password)
	  {
		return getConnection(false, info, user, password);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static FileConnection getConnection(bool isSSL, SystemInfo info, string user, string password)
	  {
		return getConnection(isSSL, info, user, password, isSSL ? DEFAULT_SSL_FILE_SERVER_PORT : DEFAULT_FILE_SERVER_PORT);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(SystemInfo info, String user, String password, int filePort) throws IOException
	  public static FileConnection getConnection(SystemInfo info, string user, string password, int filePort)
	  {
		return getConnection(false, info, user, password, filePort);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static FileConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password, int filePort) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static FileConnection getConnection(bool isSSL, SystemInfo info, string user, string password, int filePort)
	  {
		if (filePort < 0 || filePort > 65535)
		{
		  throw new IOException("Bad file port: " + filePort);
		}
		FileConnection conn = null;

		Socket fileServer = isSSL? SSLSocketFactory.Default.createSocket(info.System, filePort) : new Socket(info.System, filePort);
		Stream @in = fileServer.InputStream;
		Stream @out = fileServer.OutputStream;
		try
		{
		  HostOutputStream dout = new HostOutputStream(new BufferedOutputStream(@out));
		  HostInputStream din = new HostInputStream(new BufferedInputStream(@in));
		  string jobName = connect(info, dout, din, 0xE002, user, password);

		  sendExchangeAttributesRequest(dout);
		  dout.flush();

		  int length = din.readInt();
		  if (length < 22)
		  {
			throw DataStreamException.badLength("fileExchangeAttributes", length);
		  }
		  din.skipBytes(16);
		  int rc = din.readShort();
		  if (rc != 0)
		  {
			throw DataStreamException.badReturnCode("fileExchangeAttributes", rc);
		  }
		  int datastreamLevel = din.readShort();
		  din.skipBytes(2);
		  int maxDataBlockSize = din.readInt();
		  //int ccsidLL = din.readInt();
		  //int ccsidCP = din.readShort();
		  din.skipBytes(6);
		  int ccsid = din.readShort();
		  int remaining = length - 38;
		  din.skipBytes(remaining);

		  conn = new FileConnection(info, fileServer, din, dout, ccsid, datastreamLevel, maxDataBlockSize, user, jobName);
		  return conn;
		}
		finally
		{
		  if (conn == null)
		  {
			@in.Close();
			@out.Close();
			fileServer.close();
		  }
		}
	  }

	  /// <summary>
	  /// Returns a list of files under the specified directory or file spec.
	  /// For example, these all return the same file listing:
	  /// <ul>
	  /// <li>list("/home/smith")</li>
	  /// <li>list("/home/smith/*")</li>
	  /// <li>list("/home/smith/../smith")</li>
	  /// </ul>
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<FileHandle> listFiles(String dir) throws IOException
	  public virtual IList<FileHandle> listFiles(string dir)
	  {
		if (dir.IndexOf("*", StringComparison.Ordinal) < 0)
		{
		  if (!dir.EndsWith("/", StringComparison.Ordinal))
		  {
			dir = dir + "/*";
		  }
		  else
		  {
			dir = dir + "*";
		  }
		}

		string parent = dir;
		int slash = parent.LastIndexOf("/", StringComparison.Ordinal);
		if (slash < 0)
		{
		  parent = "";
		}
		else if (slash > 0)
		{
		  parent = parent.Substring(0,slash + 1);
		}

		sendListFilesRequest(Conv.stringToUnicodeByteArray(dir));
		out_.flush();

		List<FileHandle> files = new List<FileHandle>();
		int chain = 1;
		while (chain != 0)
		{
		  int length = in_.readInt();
		  if (length < 24)
		  {
			throw DataStreamException.badLength("listFiles", length);
		  }
		  int headerID = in_.readShort();
		  int serverID = in_.readShort();
		  int csInstance = in_.readInt();
		  int correlationID = in_.readInt();
		  int templateLength = in_.readShort();
		  int reqRepID = in_.readShort();
		  chain = in_.readShort();
		  if (reqRepID == 0x8001)
		  {
			int rc = in_.readShort();
			int remaining = length - 24;
			in_.skipBytes(remaining);
			if (rc != FileConstants.RC_NO_MORE_FILES)
			{
			  throw DataStreamException.badReturnCode("listFiles", rc);
			}
		  }
		  else if (reqRepID == 0x8005)
		  {
			int numRead = 22;
			long createDate = convertDate(in_);
			long modifyDate = convertDate(in_);
			long accessDate = convertDate(in_);
			int fileSize = in_.readInt();
			int fixedAttributes = in_.readInt();
			int objectType = in_.readShort();
			int numExtAttrs = in_.readShort();
			int bytesEANames = in_.readInt();
			int bytesEAValues = in_.readInt();
			int version = in_.readInt();
			int amountAccessed = in_.readShort();
			int accessHistory = in_.readByte();
			int fileCCSID = in_.readShort();
			int checkoutCCSID = in_.readShort();
			int restartID = in_.readInt();
			long largeFileSize = in_.readLong();
			in_.skipBytes(2);
			int symlink = in_.readByte(); // 91
			numRead = 92;
			int fileNameLLOffset = 20 + templateLength;
			int toSkip = fileNameLLOffset - numRead;
			in_.skipBytes(toSkip);
			numRead += toSkip;
			int fileNameLength = in_.readInt() - 6;
			in_.skipBytes(2); // 0x0002 -- Name CP.
			numRead += 6;
			sbyte[] nameBuf = new sbyte[fileNameLength];
			in_.readFully(nameBuf);
			numRead += fileNameLength;
			string name = Conv.unicodeByteArrayToString(nameBuf, 0, nameBuf.Length);
			if (!name.Equals(".") && !name.Equals(".."))
			{
			  FileHandle h = FileHandle.createEmptyHandle();
			  h.Name = name;
			  h.Path = parent + name;
			  h.DataCCSID = fileCCSID;
			  h.CreateDate = createDate;
			  h.ModifyDate = modifyDate;
			  h.AccessDate = accessDate;
			  h.Size = largeFileSize;
			  h.Version = version;
			  h.Symlink = symlink == 1;
			  h.Directory = objectType == 2;
			  files.Add(h);
			}
			int remaining = length - numRead;
			in_.skipBytes(remaining);
		  }
		  else
		  {
			int remaining = length - 22;
			in_.skipBytes(remaining);
			throw DataStreamException.badReply("listFiles", reqRepID);
		  }
		}
		return files;
	  }

	  /// <summary>
	  /// Returns a list of filenames under the specified directory or file spec.
	  /// For example, these all return the same file listing:
	  /// <ul>
	  /// <li>list("/home/smith")</li>
	  /// <li>list("/home/smith/*")</li>
	  /// <li>list("/home/smith/../smith")</li>
	  /// </ul>
	  /// To return more than just the names of files, use <seealso cref="#listFiles listFiles()"/>.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<String> list(String dir) throws IOException
	  public virtual IList<string> list(string dir)
	  {
	//    System.out.println("Server CCSID: "+getInfo().getServerCCSID());
		if (dir.IndexOf("*", StringComparison.Ordinal) < 0)
		{
		  if (!dir.EndsWith("/", StringComparison.Ordinal))
		  {
			dir = dir + "/*";
		  }
		  else
		  {
			dir = dir + "*";
		  }
		}

		sendListFilesRequest(Conv.stringToUnicodeByteArray(dir));
		out_.flush();

		List<string> files = new List<string>();
		int chain = 1;
		while (chain != 0)
		{
		  int length = in_.readInt();
		  if (length < 24)
		  {
			throw DataStreamException.badLength("listFiles", length);
		  }
		  int headerID = in_.readShort();
		  int serverID = in_.readShort();
		  int csInstance = in_.readInt();
		  int correlationID = in_.readInt();
		  int templateLength = in_.readShort();
		  int reqRepID = in_.readShort();
		  chain = in_.readShort();
		  if (reqRepID == 0x8001)
		  {
			int rc = in_.readShort();
			int remaining = length - 24;
			in_.skipBytes(remaining);
			if (rc != FileConstants.RC_NO_MORE_FILES)
			{
			  throw DataStreamException.badReturnCode("listFiles", rc);
			}
		  }
		  else if (reqRepID == 0x8005)
		  {
			int numRead = 22;
	/*      long createDate = in_.readLong();
	      long modifyDate = in_.readLong();
	      long actual = ((modifyDate >> 32) & 0x00FFFFFFFFL)*1000L + ((modifyDate & 0x00FFFFFFFFL)/1000);
	      System.out.println(new java.util.Date(actual));
	      long accessDate = in_.readLong();
	      int fileSize = in_.readInt();
	      int fixedAttributes = in_.readInt();
	      int objectType = in_.readShort();
	      int numExtAttrs = in_.readShort();
	      int bytesEANames = in_.readInt();
	      int bytesEAValues = in_.readInt();
	      int version = in_.readInt();
	      int amountAccessed = in_.readShort();
	      int accessHistory = in_.readByte();
	      int fileCCSID = in_.readShort();
	      int checkoutCCSID = in_.readShort();
	      int restartID = in_.readInt();
	      long largeFileSize = in_.readLong();
	      in_.skipBytes(2);
	      int symlink = in_.readByte(); // 91
	*/
			in_.skipBytes(70);
			numRead = 92;
			int fileNameLLOffset = 20 + templateLength;
			int toSkip = fileNameLLOffset - numRead;
			in_.skipBytes(toSkip);
			numRead += toSkip;
			int fileNameLength = in_.readInt() - 6;
			in_.skipBytes(2); // 0x0002 -- Name CP.
			numRead += 6;
			sbyte[] nameBuf = new sbyte[fileNameLength];
			in_.readFully(nameBuf);
			numRead += fileNameLength;
			string name = Conv.unicodeByteArrayToString(nameBuf, 0, nameBuf.Length);
			if (!name.Equals(".") && !name.Equals(".."))
			{
			  files.Add(name);
			}
			int remaining = length - numRead;
			in_.skipBytes(remaining);
		  }
		  else
		  {
			int remaining = length - 22;
			in_.skipBytes(remaining);
			throw DataStreamException.badReply("listFiles", reqRepID);
		  }
		}
		return files;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendListFilesRequest(byte[] dirnameUnicode) throws IOException
	  private void sendListFilesRequest(sbyte[] dirnameUnicode)
	  {
		// 20 + 20 + 6 +
		out_.writeInt(46 + dirnameUnicode.Length); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(20); // Template length.
		out_.writeShort(0x000A); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.
		out_.writeInt(0); // File handle.
		out_.writeShort(1200); // CCSID.
		out_.writeInt(1); // Working dir handle.
		out_.writeShort(0); // Check authority. 0=none required, 1=read required, 2=write requried, 4=exec required.
		out_.writeShort(0xFFFF); // Maximum get count (-1 = no max).
		out_.writeShort(0x0101); // File attribute list level (0x0101 = return 8-byte file size; 0x0001 = reserved flag).
		out_.writeShort(1); // Pattern matching (POSIX=0, POSIX-all=1, OS/2=2).
		out_.writeInt(dirnameUnicode.Length + 6); // Filename LL.
		out_.writeShort(2); // Filename CP.
		out_.write(dirnameUnicode);
	  }

	  /// <summary>
	  /// Deletes the specified file.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int deleteFile(String filename) throws IOException
	  public virtual int deleteFile(string filename)
	  {
		sendDeleteFileRequest(out_, Conv.stringToUnicodeByteArray(filename));
		out_.flush();

		int length = in_.readInt();
		if (length < 24)
		{
		  throw DataStreamException.badLength("deleteFile", length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		in_.skipBytes(2);
		if (reqRepID == 0x8001)
		{
		  int rc = in_.readShort();
		  int remaining = length - 24;
		  in_.skipBytes(remaining);
		  return rc;
		}
		else
		{
		  int remaining = length - 22;
		  in_.skipBytes(remaining);
		  throw DataStreamException.badReply("deleteFile", reqRepID);
		}
	  }

	  /// <summary>
	  /// Opens a file for read-write access, a share option of SHARE_ALL, and a data CCSID of 1208 (UTF-8).
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int openFile(String filename, FileHandle buffer) throws IOException
	  public virtual int openFile(string filename, FileHandle buffer)
	  {
		return openFile(filename, buffer, FileHandle.OPEN_READ_WRITE, FileHandle.SHARE_ALL, true, 1208);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int openFile(String filename, FileHandle buffer, int openType, int shareOption, boolean createIfNotExist, int dataCCSID) throws IOException
	  public virtual int openFile(string filename, FileHandle buffer, int openType, int shareOption, bool createIfNotExist, int dataCCSID)
	  {
		if (buffer.Open)
		{
		  // Someone is re-using their buffer without closing the file!
		  //TODO closeFile(buffer);
		}
		sendOpenFileRequest(Conv.stringToUnicodeByteArray(filename), openType, shareOption, createIfNotExist, dataCCSID);
		out_.flush();

		int length = in_.readInt();
		if (length < 24)
		{
		  throw DataStreamException.badLength("openFile", length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		in_.skipBytes(2);
		int remaining = length - 22;
		int rc = 0;
		if (reqRepID == 0x8001)
		{
		  rc = in_.readShort();
		  remaining -= 2;
		}
		else if (reqRepID == 0x8002)
		{
		  int handle = in_.readInt();
		  long id = in_.readLong();
		  dataCCSID = in_.readShort();
		  int actionTaken = in_.readShort();
		  long createDate = convertDate(in_);
		  long modifyDate = convertDate(in_);
		  long accessDate = convertDate(in_);
		  int fileSize = in_.readInt();
		  long actualFileSize = fileSize;
		  int fixedAttribs = in_.readInt();
		  int needExtAttribs = in_.readShort();
		  int numExtAttribs = in_.readShort();
		  int charsExtAttribsNames = in_.readInt();
		  int bytesExtAttribsValues = in_.readInt();
		  int version = in_.readInt();
		  int amountAccessed = in_.readShort();
		  int accessHistory = in_.readByte();
		  remaining -= 67;
		  if (length >= 97)
		  {
			long largeFileSize = in_.readLong();
			remaining -= 8;
			actualFileSize = largeFileSize;
		  }
		  buffer.Open = true;
		  buffer.OpenType = openType;
		  buffer.Name = filename;
		  buffer.Handle = handle;
		  buffer.ID = id;
		  buffer.DataCCSID = dataCCSID;
		  buffer.CreateDate = createDate;
		  buffer.ModifyDate = modifyDate;
		  buffer.AccessDate = accessDate;
		  buffer.Size = actualFileSize;
		  buffer.Version = version;
		}
		else
		{
		  in_.skipBytes(remaining);
		  throw DataStreamException.badReply("openFile", reqRepID);
		}
		in_.skipBytes(remaining - 2);
		return rc;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int closeFile(FileHandle handle) throws IOException
	  public virtual int closeFile(FileHandle handle)
	  {
		if (!handle.Open)
		{
		  // Someone is trying to close an already-closed handle!
		  //TODO
		  return -1;
		}
		sendCloseFileRequest(handle.Handle);
		out_.flush();

		int length = in_.readInt();
		if (length < 24)
		{
		  throw DataStreamException.badLength("closeFile", length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		in_.skipBytes(2);
		int remaining = length - 22;
		if (reqRepID == 0x8001 || reqRepID == 0x8004)
		{
		  int rc = in_.readShort();
		  in_.skipBytes(remaining - 2);
		  return rc;
		}
		else
		{
		  in_.skipBytes(remaining);
		  throw DataStreamException.badReply("closeFile", reqRepID);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readFile(FileHandle handle, byte[] buffer, int bufferOffset, int bufferLength) throws IOException
	  public virtual int readFile(FileHandle handle, sbyte[] buffer, int bufferOffset, int bufferLength)
	  {
		long currentOffset = handle.Offset;
		sendReadRequest(handle.Handle, currentOffset, bufferLength);
		out_.flush();

		int length = in_.readInt();
		if (length < 24)
		{
		  throw DataStreamException.badLength("readFile", length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		int chain = in_.readShort(); //in_.skipBytes(2);
		int numRead = 22;
		int remaining = length - 22;
		int rc = 0;
		int numToRead = -1;
		if (reqRepID == 0x8001)
		{
		  rc = in_.readShort();
		  remaining -= 2;
		  numRead += 2;
		}
		else if (reqRepID == 0x8003)
		{
		  int ccsid = in_.readShort();
		  int dataLength = in_.readInt();
		  in_.skipBytes(2);
		  remaining -= 8;
		  int numBytes = dataLength - 6;
		  numToRead = numBytes > length ? length : numBytes;
		  int numToSkip = numToRead >= numBytes ? 0 : numBytes - length;
		  in_.readFully(buffer, bufferOffset, numToRead);
		  in_.skipBytes(numToSkip);
		  handle.Offset = currentOffset + numToRead;
		  remaining -= numBytes;
		  numRead += 6;
		  numRead += numToRead;
		  numRead += numToSkip;
		}
		else
		{
		  in_.skipBytes(remaining);
		  numRead += remaining;
		  throw DataStreamException.badReply("readFile", reqRepID);
		}
		in_.skipBytes(remaining - 2);
		handle.LastStatus = rc;
		return numToRead;
	  }

	  /// <summary>
	  /// Writes data to the file starting at the current file offset in the handle, and optionally sync-ing the data to disk;
	  /// upon a successful write, the current file offset and size are incremented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeFile(FileHandle handle, byte[] buffer, int bufferOffset, int bufferLength, boolean sync) throws IOException
	  public virtual void writeFile(FileHandle handle, sbyte[] buffer, int bufferOffset, int bufferLength, bool sync)
	  {
		long currentOffset = handle.Offset;
		sendWriteRequest(handle.Handle, currentOffset, buffer, bufferOffset, bufferLength, handle.DataCCSID, sync);
		out_.flush();

		int length = in_.readInt();
		if (length < 24)
		{
		  throw DataStreamException.badLength("writeFile", length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		int chain = in_.readShort(); //in_.skipBytes(2);
		int numRead = 22;
		int rc = 0;
		if (reqRepID == 0x8001)
		{
		  rc = in_.readShort();
		  numRead += 2;
		}
		else if (reqRepID == 0x800B)
		{
		  rc = in_.readShort();
		  int previousFileSize = in_.readInt();
		  int bytesNotWritten = in_.readInt();
		  numRead += 10;
		  handle.Offset = currentOffset + (bufferLength - bytesNotWritten);
		  handle.Size = handle.Size + (bufferLength - bytesNotWritten);
		}
		else
		{
		  in_.skipBytes(length - numRead);
		  numRead = length;
		  throw DataStreamException.badReply("writeFile", reqRepID);
		}
		in_.skipBytes(length - numRead);
		handle.LastStatus = rc;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendWriteRequest(int handle, long fileOffset, byte[] data, int dataOffset, int dataLength, int dataCCSID, boolean sync) throws IOException
	  private void sendWriteRequest(int handle, long fileOffset, sbyte[] data, int dataOffset, int dataLength, int dataCCSID, bool sync)
	  {
		if (datastreamLevel_ < 16 && fileOffset > 0x007FFFFFFFL)
		{
		  throw new IOException("File offset too large: " + fileOffset);
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int templateLength = datastreamLevel_ < 16 ? 18 : 34;
		int templateLength = datastreamLevel_ < 16 ? 18 : 34;
		out_.writeInt(26 + templateLength + dataLength); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(templateLength); // Template length.
		out_.writeShort(0x0004); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.
		out_.writeInt(handle); // File handle.
		if (datastreamLevel_ < 16)
		{
		  out_.writeInt(0); // Base offset.
		  out_.writeInt((int)fileOffset); // Relative offset.
		}
		else
		{
		  out_.writeInt(0); // Base offset.
		  out_.writeInt(0); // Relative offset.
		}
		out_.writeShort(sync ? 3 : 2); // Data flags. 3=forceToStorage(sync),2=return immediately without sync.
		out_.writeShort(dataCCSID); // Data CCSID.

		if (datastreamLevel_ >= 16)
		{
		  out_.writeLong(0); // Large base offset.
		  out_.writeLong(fileOffset); // Large relative offset.
		}

		out_.writeInt(dataLength + 6); // Data LL.
		out_.writeShort(0x0020); // Data CP.
		out_.write(data, dataOffset, dataLength); // Data.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendReadRequest(int handle, long fileOffset, int length) throws IOException
	  private void sendReadRequest(int handle, long fileOffset, int length)
	  {
		if (datastreamLevel_ < 16 && fileOffset > 0x007FFFFFFFL)
		{
		  throw new IOException("File offset too large: " + fileOffset);
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int templateLength = datastreamLevel_ < 16 ? 22 : 38;
		int templateLength = datastreamLevel_ < 16 ? 22 : 38;
		out_.writeInt(20 + templateLength); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(templateLength); // Template length.
		out_.writeShort(0x0003); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.
		out_.writeInt(handle); // File handle.
		if (datastreamLevel_ < 16)
		{
		  out_.writeInt(0); // Base offset.
		  out_.writeInt((int)fileOffset); // Relative offset.
		}
		else
		{
		  out_.writeInt(0); // Base offset.
		  out_.writeInt(0); // Relative offset.
		}
		out_.writeInt(length); // Read length.
		out_.writeInt(0); // Pre-read length.
		if (datastreamLevel_ >= 16)
		{
		  out_.writeLong(0); // Large base offset.
		  out_.writeLong(fileOffset); // Large relative offset.
		}
	  }

	  // Only used to get the owner of a file. Boooo.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendListAttributesOA1Request(int handle, int flags1, int flags2) throws IOException
	  private void sendListAttributesOA1Request(int handle, int flags1, int flags2)
	  {
		out_.writeInt(54); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(20); // Template length.
		out_.writeShort(0x000A); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.

		out_.writeInt(handle); // File handle.
		out_.writeShort(0); // CCSID.
		out_.writeInt(1); // Working dir handle.
		out_.writeShort(0); // Check authority: 0=no auth required, 1=read required, 2=write required, 4=exec required.
		out_.writeShort(0xFFFF); // Max get count.
		out_.writeShort(0x42); // Use OA1 with open file handle.
		out_.writeShort(0); // Pattern matching. 0=posix

		out_.writeInt(14); // OA1 flags LL.
		out_.writeShort(0x0010); // OA1 flags CP.
		out_.writeInt(flags1); // OA1 flags1.
		out_.writeInt(flags2); // OA1 flags2.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendCloseFileRequest(int handle) throws IOException
	  private void sendCloseFileRequest(int handle)
	  {
		out_.writeInt(41); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(21); // Template length.
		out_.writeShort(0x0009); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.

		out_.writeInt(handle); // File handle.
		out_.writeShort(2); // Data flags.
		out_.writeShort(0xFFFF); // CCSID.
		out_.writeShort(100); // Amount accessed.
		out_.writeByte(0); // Access history.
		out_.writeLong(0); // Modify date.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendOpenFileRequest(byte[] filenameUnicode, int openType, int share, boolean create, int dataCCSID) throws IOException
	  private void sendOpenFileRequest(sbyte[] filenameUnicode, int openType, int share, bool create, int dataCCSID)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int templateLength = datastreamLevel_ < 16 ? 36 : 44;
		int templateLength = datastreamLevel_ < 16 ? 36 : 44;
		out_.writeInt(26 + templateLength + filenameUnicode.Length); // Length.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE002); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(templateLength); // Template length.
		out_.writeShort(0x0002); // ReqRep ID.
		out_.writeShort(0); // Chain indicator.
		out_.writeShort(1200); // Filename CCSID.
		out_.writeInt(1); // Working dir handle.
		out_.writeShort(dataCCSID); // File data CCSID.
		out_.writeShort(openType); // Access intent. 1=read, 2=write, 4=program load
		out_.writeShort(share); // Share option. 0=all, 3=none, 2=read, 1=write.
		out_.writeShort(0); // Data conversion. 0=none, 1=use client CCSID, 2=use server CCSID.
		out_.writeShort(create ? 1 : 8); // Duplicate file option. 16=failIfNoExist+replaceIfExist, 8=failIfNoExist+openIfExist, 4=createOpenIfNoExist+failIfExist, 2=createOpenIfNoExist+replaceIfExist, 1=createOpenIfNoExist+openIfExist
		out_.writeInt(0); // Create size.
		out_.writeInt(0); // Fixed attributes.
		out_.writeShort(1); // Attribute list level.
		out_.writeInt(0); // Pre-read offset.
		out_.writeInt(0); // Pre-read length.
		if (datastreamLevel_ >= 16)
		{
		  out_.writeLong(0); // Large create size.
		}
		out_.writeInt(filenameUnicode.Length + 6); // Filename LL.
		out_.writeShort(2); // Filename CP.
		out_.write(filenameUnicode);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendDeleteFileRequest(HostOutputStream out, byte[] filenameUnicode) throws IOException
	  private void sendDeleteFileRequest(HostOutputStream @out, sbyte[] filenameUnicode)
	  {
		@out.writeInt(34 + filenameUnicode.Length); // Length.
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE002); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(newCorrelationID()); // Correlation ID.
		@out.writeShort(8); // Template length.
		@out.writeShort(0x000C); // ReqRep ID.
		@out.writeShort(0); // Chain indicator.
		@out.writeShort(1200); // CCSID.
		@out.writeInt(1); // Working dir handle.
		@out.writeInt(filenameUnicode.Length + 6); // Filename LL.
		@out.writeShort(2); // Filename CP.
		@out.write(filenameUnicode);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void sendExchangeAttributesRequest(HostOutputStream out) throws IOException
	  private static void sendExchangeAttributesRequest(HostOutputStream @out)
	  {
		@out.writeInt(42); // Length.
		@out.writeShort(0); // Header ID.
		@out.writeShort(0xE002); // Server ID.
		@out.writeInt(0); // CS instance.
		@out.writeInt(0); // Correlation ID.
		@out.writeShort(10); // Template length.
		@out.writeShort(0x0016); // ReqRep ID.
		@out.writeShort(0); // Chain indicator.
		@out.writeShort(8); // Datastream level. V5R3 or higher.
		@out.writeShort(6); // Don't use posix return codes (8), use GMT (4), use PC pattern match (2).
		@out.writeInt(unchecked((int)0xFFFFFFFF)); // Max data block.
		@out.writeInt(12); // CCSID LL.
		@out.writeShort(10); // CCSID CP.
		@out.writeShort(1200); // Preferred CCSID UTF-16.
		@out.writeShort(13488); // Preferred CCSID UnicodeBig.
		@out.writeShort(61952); // Preferred CCSID old IFS UnicodeBig.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long convertDate(HostInputStream in) throws IOException
	  private static long convertDate(HostInputStream @in)
	  {
		// The IFS format is 32-bit seconds, 32-bit milliseconds.
		int seconds = @in.readInt();
		int microseconds = @in.readInt();
		long milliseconds = (seconds * 1000L) + (microseconds / 1000);
		return milliseconds;
	  }
	}

}