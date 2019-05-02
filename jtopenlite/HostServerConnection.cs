using System;
using System.IO;
using System.Net.Sockets;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  HostServerConnection.java
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
	/// Represents a TCP/IP socket connection to an IBM i Host Server job. All
	/// HostServerConnections have associated system information provided via the
	/// SystemInfo class.
	/// 
	/// </summary>
	public abstract class HostServerConnection : Connection
	{
		private readonly SystemInfo info_;
		private readonly string user_;
		private readonly string jobName_;
		private bool closed_ = false;

		private readonly Socket socket_;
		protected internal readonly HostInputStream in_;
		protected internal readonly HostOutputStream out_;

		protected internal HostServerConnection(SystemInfo info, string user, string jobName, Socket socket, HostInputStream @in, HostOutputStream @out)
		{
			info_ = info;
			user_ = user;
			jobName_ = jobName;
			socket_ = socket;
			in_ = @in;
			out_ = @out;
		}

		/// <summary>
		/// Returns true if debugging is enabled by default for all
		/// HostServerConnection datastreams.
		/// 
		/// </summary>
		public static bool DefaultDatastreamDebug
		{
			get
			{
				return Trace.StreamTracingEnabled;
			}
			set
			{
				HostInputStream.AllDebug = value;
				HostOutputStream.AllDebug = value;
			}
		}


		  /// <summary>
		  /// Returns the total number of bytes this connection has read since it was
		  /// opened. If an I/O exception has occurred with this stream, the number
		  /// returned by this method may not reflect any partial datastream bytes that
		  /// may have actually been read before the exception took place.
		  /// 
		  /// </summary>
		  public virtual long BytesReceived
		  {
			  get
			  {
				return in_.BytesReceived;
			  }
		  }


		  /// <summary>
		  /// Returns the total number of bytes this connection has written since it was opened.
		  /// If an I/O exception has occurred with this stream, the number returned by this method
		  /// may not reflect any partial datastream bytes that may have actually been written before
		  /// the exception took place. Additionally, the number of bytes written over the TCP socket
		  /// may be less, if the underlying stream has not yet been flushed.
		  /// 
		  /// </summary>
		  public virtual long BytesSent
		  {
			  get
			  {
				return out_.BytesSent;
			  }
		  }

		/// <summary>
		/// Returns true if datastream debugging is currently enabled.
		/// 
		/// </summary>
		public virtual bool DatastreamDebug
		{
			get
			{
				return in_.debug_;
			}
			set
			{
				in_.Debug = value;
				out_.Debug = value;
			}
		}


		/// <summary>
		/// Sends an "end job" datastream (if supported) and closes the underlying
		/// socket.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void close() throws IOException
		public void close()
		{
			if (closed_)
			{
				return;
			}
			try
			{
				sendEndJobRequest();
			}
			finally
			{
				closed_ = true;
				forceClose();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void sendEndJobRequest() throws IOException;
		protected internal abstract void sendEndJobRequest();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void forceClose() throws IOException
		private void forceClose()
		{
			IOException rethrownException = null;
			try
			{
				in_.close();
				out_.close();
			}
			catch (java.net.SocketException socketException)
			{
				if (socketException.ToString().IndexOf("Socket closed", StringComparison.Ordinal) >= 0)
				{
					// Ignore this exception
				}
				else
				{
					rethrownException = socketException;
					throw rethrownException;
				}
			}
			finally
			{
				try
				{
					socket_.close();
				}
				catch (java.net.SocketException socketException)
				{
					if (socketException.ToString().IndexOf("Socket closed", StringComparison.Ordinal) >= 0)
					{
						// Ignore this exception
					}
					else
					{
						// Only throw an exception if one has not yet been thrown.
						if (rethrownException == null)
						{
							throw socketException;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void finalize() throws Throwable
		~HostServerConnection()
		{
			close();
		}

		/// <summary>
		/// Returns true if close() has been called on this connection.
		/// 
		/// </summary>
		public bool Closed
		{
			get
			{
				return closed_;
			}
		}

		/// <summary>
		/// Returns the system information associated with this connection.
		/// 
		/// </summary>
		public SystemInfo Info
		{
			get
			{
				return info_;
			}
		}

		/// <summary>
		/// Returns the currently authenticated user of this connection.
		/// 
		/// </summary>
		public string User
		{
			get
			{
				return user_;
			}
		}

		/// <summary>
		/// Returns the host server job string for the job that is connected to this
		/// connection.
		/// 
		/// </summary>
		public string JobName
		{
			get
			{
				return jobName_;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static byte[] getEncryptedPassword(byte[] userBytes, byte[] passwordBytes, byte[] clientSeed, byte[] serverSeed, int passwordLevel) throws IOException
		protected internal static sbyte[] getEncryptedPassword(sbyte[] userBytes, sbyte[] passwordBytes, sbyte[] clientSeed, sbyte[] serverSeed, int passwordLevel)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean doSHAInsteadOfDES = passwordLevel >= 2;
			bool doSHAInsteadOfDES = passwordLevel >= 2;
			sbyte[] encryptedPassword = null;
		if (doSHAInsteadOfDES)
		{
		  encryptedPassword = EncryptPassword.encryptPasswordSHA(userBytes, passwordBytes, clientSeed, serverSeed);
		}
		else
		{
				// Normal DES encryption.
				encryptedPassword = EncryptPassword.encryptPasswordDES(userBytes, passwordBytes, clientSeed, serverSeed);
		}
			return encryptedPassword;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static String connect(SystemInfo info, HostOutputStream dout, HostInputStream din, int serverID, String user, String password) throws IOException
		protected internal static string connect(SystemInfo info, HostOutputStream dout, HostInputStream din, int serverID, string user, string password)
		{
			// Exchange random seeds.
			long seed = sendExchangeRandomSeedsRequest(dout, serverID);
			sbyte[] clientSeed = Conv.longToByteArray(seed);
			dout.flush();

			int length = din.readInt();
			if (length < 20)
			{
				throw DataStreamException.badLength("exchangeRandomSeeds-" + serverID, length);
			}
			din.skipBytes(16);
			int rc = din.readInt();
			if (rc != 0)
			{
				throw DataStreamException.badReturnCode("exchangeRandomSeeds-" + serverID, rc);
			}
			sbyte[] serverSeed = new sbyte[8];
			din.readFully(serverSeed);

		sbyte[] userBytes = getUserBytes(user, info.PasswordLevel);
		sbyte[] passwordBytes = getPasswordBytes(password, info.PasswordLevel);
			password = null;
		sbyte[] encryptedPassword = getEncryptedPassword(userBytes, passwordBytes, clientSeed, serverSeed, info.PasswordLevel);

			din.end();

		sbyte[] userEBCDICBytes = (info.PasswordLevel < 2) ? userBytes : getUserBytes(user, 0);
		sendStartServerRequest(dout, userEBCDICBytes, encryptedPassword, serverID);
			dout.flush();

			length = din.readInt();
			if (length < 20)
			{
				throw DataStreamException.badLength("startServer-" + serverID, length);
			}
			din.skipBytes(16);
			rc = din.readInt();
			if (rc != 0)
			{
				string msg = getReturnCodeMessage(rc);
				throw string.ReferenceEquals(msg, null) ? DataStreamException.badReturnCode("startServer-" + serverID, rc) : DataStreamException.errorMessage("startServer-" + serverID, new Message(rc.ToString(), msg));
			}
			string jobName = null;
			int remaining = length - 24;
			while (remaining > 10)
			{
				int ll = din.readInt();
				int cp = din.readShort();
				remaining -= 6;
				if (cp == 0x111F) // Job name.
				{
					din.skipBytes(4); // CCSID is always 0.
					remaining -= 4;
					int jobLength = ll - 10;
					sbyte[] jobBytes = new sbyte[jobLength];
					din.readFully(jobBytes);
					jobName = Conv.ebcdicByteArrayToString(jobBytes, 0, jobLength);
					remaining -= jobLength;
				}
				else
				{
					din.skipBytes(ll - 6);
					remaining -= (ll - 6);
				}
			}
			din.skipBytes(remaining);
			din.end();
			return jobName;
		}

		private static string getReturnCodeMessage(int rc)
		{
			if ((rc & 0xFFFF0000) == 0x00010000)
			{
				return "Error on request data";
			}
			if ((rc & 0xFFFF0000) == 0x00040000)
			{
				return "General security error, function not performed";
			}
			if ((rc & 0xFFFF0000) == 0x00060000)
			{
				return "Authentication Token error";
			}
			switch (rc)
			{
			case 0x00020001:
				return "Userid error: User Id unknown";
			case 0x00020002:
				return "Userid error: User Id valid, but revoked";
			case 0x00020003:
				return "Userid error: User Id mismatch with authentication token";
			case 0x0003000B:
				return "Password error: Password or Passphrase incorrect";
			case 0x0003000C:
				return "Password error: User profile will be revoked on next invalid password or passphrase";
			case 0x0003000D:
				return "Password error: Password or Passphrase correct, but expired";
			case 0x0003000E:
				return "Password error: Pre-V2R2 encrypted password";
			case 0x00030010:
				return "Password error: Password is *NONE";
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void sendStartServerRequest(HostOutputStream out, byte[] userBytes, byte[] encryptedPassword, int serverID) throws IOException
		internal static void sendStartServerRequest(HostOutputStream @out, sbyte[] userBytes, sbyte[] encryptedPassword, int serverID)
		{
			@out.writeInt(44 + encryptedPassword.Length);
			@out.writeByte(2); // Client attributes, 2 means return job info.
			@out.writeByte(0); // Server attribute.
			@out.writeShort(serverID); // Server ID.
			@out.writeInt(0); // CS instance.
			@out.writeInt(0); // Correlation ID.
			@out.writeShort(2); // Template length.
			@out.writeShort(0x7002); // ReqRep ID.
			@out.writeByte(encryptedPassword.Length == 8 ? 1 : 3); // Password
																	// encryption
																	// type.
			@out.writeByte(1); // Send reply.
			@out.writeInt(6 + encryptedPassword.Length); // Password LL.
			@out.writeShort(0x1105); // Password CP. 0x1115 is other.
			@out.write(encryptedPassword);
			@out.writeInt(16); // User ID LL.
			@out.writeShort(0x1104); // User ID CP.
			@out.write(userBytes);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static long sendExchangeRandomSeedsRequest(HostOutputStream out, int serverID) throws IOException
		internal static long sendExchangeRandomSeedsRequest(HostOutputStream @out, int serverID)
		{
			@out.writeInt(28); // Length.
			@out.writeByte(1); // Client attributes, 1 means capable of SHA-1.
			@out.writeByte(0); // Server attributes.
			@out.writeShort(serverID); // Server ID.
			@out.writeInt(0); // CS instance.
			@out.writeInt(0); // Correlation ID.
			@out.writeShort(8); // Template length.
			@out.writeShort(0x7001); // ReqRep ID.
			long clientSeed = DateTimeHelper.CurrentUnixTimeMillis();
			@out.writeLong(clientSeed);
			return clientSeed;
		}

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

		protected internal sealed class HostInputStream
		{
			internal static bool allDebug_ = false;

			internal readonly Stream in_;
			internal bool debug_;
			internal int debugCounter_ = 0;

			internal readonly sbyte[] shortArray_ = new sbyte[2];
			internal readonly sbyte[] intArray_ = new sbyte[4];
			internal readonly sbyte[] longArray_ = new sbyte[8];

			//internal PrintStream tracePrintStream = null;
			internal long bytesReceivedAtLastReset_ = 0;
			internal long latestBytesReceived_ = 0;

			public static bool AllDebug
			{
				set
				{
				   allDebug_ = value;
				}
			}

			public HostInputStream(Stream @in)
			{
				in_ = @in;
				debug_ = Trace.StreamTracingEnabled;
				if (debug_ || allDebug_)
				{
//					tracePrintStream = Trace.PrintStream;
//					if (tracePrintStream == null)
//					{
//						tracePrintStream = System.out;
//					}
				}

			}

			public void resetLatestBytesReceived()
			{
				bytesReceivedAtLastReset_ += latestBytesReceived_;
				latestBytesReceived_ = 0;
			}
			public long LatestBytesReceived
			{
				get
				{
					return latestBytesReceived_;
				}
			}
			public long BytesReceived
			{
				get
				{
					return bytesReceivedAtLastReset_ + latestBytesReceived_;
				}
			}

			public bool Debug
			{
				set
				{
					debug_ = value;
//					if (debug_)
//					{
//						tracePrintStream = Trace.PrintStream;
//					}
    
				}
			}

			internal void debugByte(int i)
			{
//				if (tracePrintStream != null)
//				{
//					if (debugCounter_ == 0)
//					{
//						lock (HostOutputStream.formatter_)
//						{
//
//							tracePrintStream.print(HostOutputStream.formatter_.format(DateTime.Now));
//							tracePrintStream.println(" Data stream data received...");
//						}
//					}
//					int highNibble = (0x00FF & i) >> 4;
//					int lowNibble = 0x000F & i;
//					tracePrintStream.print(HostOutputStream.CHAR[highNibble]);
//					tracePrintStream.print(HostOutputStream.CHAR[lowNibble]);
//					if (++debugCounter_ % 16 == 0)
//					{
//						tracePrintStream.println();
//					}
//					else
//					{
//						tracePrintStream.print(" ");
//					}
//				}
			}

			internal void debugBytes(sbyte[] b, int offset, int length)
			{
				for (int i = offset; i < offset + length; ++i)
				{
					debugByte(b[i]);
				}
			}

			/// <summary>
			/// Used to note the end of a datastream when debugging is enabled.
			/// 
			/// </summary>
			public void end()
			{
//				if (debug_ && tracePrintStream != null)
//				{
//					tracePrintStream.println();
//					debugCounter_ = 0;
//				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
			public int read()
			{
				int i = in_.Read();
				if (i < 0)
				{
					throw new EOFException();
				}
				if (debug_)
				{
					debugByte(i);
				}
				++latestBytesReceived_;
				return i;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readByte() throws IOException
			public int readByte()
			{
				return read();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readShort() throws IOException
			public int readShort()
			{
				int i = in_.Read(shortArray_, 0, shortArray_.Length);
				if (i != 2)
				{
					int numRead = (i >= 0 ? i : 0);
					latestBytesReceived_ += numRead;
					while (i >= 0 && numRead < 2)
					{
						i = in_.Read(shortArray_, numRead, 2 - numRead);
						numRead += (i >= 0 ? i : 0);
					  latestBytesReceived_ += numRead;

					}
					if (numRead < 2)
					{
						throw new EOFException();
					}
				}
		  else
		  {
			latestBytesReceived_ += 2;
		  }
				if (debug_)
				{
					debugBytes(shortArray_, 0, 2);
				}
				return ((shortArray_[0] & 0x00FF) << 8) | (shortArray_[1] & 0x00FF);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int readInt() throws IOException
			public int readInt()
			{
				int i = in_.Read(intArray_, 0, intArray_.Length);
				if (i != 4)
				{
					int numRead = (i >= 0 ? i : 0);
					latestBytesReceived_ += numRead;
					while (i >= 0 && numRead < 4)
					{
						i = in_.Read(intArray_, numRead, 4 - numRead);
						numRead += (i >= 0 ? i : 0);
						latestBytesReceived_ += numRead;
					}
					if (numRead < 4)
					{
						throw new EOFException();
					}
				}
				else
				{
					latestBytesReceived_ += 4;
				}
				if (debug_)
				{
					debugBytes(intArray_, 0, 4);
				}
				return Conv.byteArrayToInt(intArray_, 0);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long readLong() throws IOException
			public long readLong()
			{
				int i = in_.Read(longArray_, 0, longArray_.Length);
				if (i != 8)
				{
					int numRead = (i >= 0 ? i : 0);
					latestBytesReceived_ += numRead;
					while (i >= 0 && numRead < 8)
					{
						i = in_.Read(longArray_, numRead, 8 - numRead);
						numRead += (i >= 0 ? i : 0);
						latestBytesReceived_ += numRead;
					}
					if (numRead < 8)
					{
						throw new EOFException();
					}
				}
				else
				{
					latestBytesReceived_ += 8;
				}
				if (debug_)
				{
					debugBytes(longArray_, 0, 8);
				}
				return Conv.byteArrayToLong(longArray_, 0);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int skipBytes(final int n) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public int skipBytes(int n)
			{
				if (debug_)
				{
					int num = 0;
					while (num < n)
					{
						read();
						++num;
					}
					return num;
				}

				int i = (int) in_.skip(n);
				if (i != n)
				{
					int numSkipped = (i >= 0 ? i : 0);
					latestBytesReceived_ += numSkipped;
					while (i >= 0 && numSkipped < n)
					{
						i = (int) in_.skip(n - numSkipped);
						numSkipped += (i >= 0 ? i : 0);
						latestBytesReceived_ += numSkipped;
					}
					if (numSkipped < n)
					{
						throw new EOFException();
					}
				}
				else
				{
					latestBytesReceived_ += n;
				}
				return i;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public void close()
			{
				in_.Close();
				if (debug_ && tracePrintStream != null)
				{
					tracePrintStream.println();
					debugCounter_ = 0;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(final byte[] b) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void readFully(sbyte[] b)
			{
				int i = in_.Read(b, 0, b.Length);
				if (i != b.Length)
				{
					int numRead = (i >= 0 ? i : 0);
					latestBytesReceived_ += numRead;
					while (i >= 0 && numRead < b.Length)
					{
						i = in_.Read(b, numRead, b.Length - numRead);
						numRead += (i >= 0 ? i : 0);
						latestBytesReceived_ += numRead;
					}
					if (numRead < b.Length)
					{
						throw new EOFException();
					}
				}
				else
				{
					latestBytesReceived_ += i;
				}
				if (debug_)
				{
					debugBytes(b, 0, b.Length);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFully(final byte[] b, final int offset, final int length) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void readFully(sbyte[] b, int offset, int length)
			{
				int i = in_.Read(b, offset, length);
				if (i != length)
				{
					int numRead = (i >= 0 ? i : 0);
					latestBytesReceived_ += numRead;
					while (i >= 0 && numRead < length)
					{
						i = in_.Read(b, offset + numRead, length - numRead);
						numRead += (i >= 0 ? i : 0);
						latestBytesReceived_ += numRead;
					}
					if (numRead < length)
					{
						throw new EOFException();
					}
				}
				else
				{
					latestBytesReceived_ += length;
				}
				if (debug_)
				{
					debugBytes(b, offset, length);
				}
			}
		}

		protected internal sealed class HostOutputStream
		{
			//internal static SimpleDateFormat formatter_ = new SimpleDateFormat("EEE MMM d HH:mm:ss:SSS z yyyy");
			internal static readonly char[] CHAR = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

			internal static bool allDebug_ = false;

			internal readonly Stream out_;
			internal bool debug_;
			internal int debugCounter_ = 0;
			internal int bytesSent_ = 0;
			//internal PrintStream tracePrintStream = null;

			public static bool AllDebug
			{
				set
				{
				 allDebug_ = value;
				}
			}

			public long BytesSent
			{
				get
				{
					return bytesSent_;
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public HostOutputStream(final OutputStream out)
			public HostOutputStream(Stream @out)
			{
				out_ = @out;
				debug_ = Trace.StreamTracingEnabled;
				if (debug_ || allDebug_)
				{
					tracePrintStream = Trace.PrintStream;
					if (tracePrintStream == null)
					{
						tracePrintStream = System.out;
					}
				}
			}

			public bool Debug
			{
				set
				{
					debug_ = value;
					if (debug_)
					{
						tracePrintStream = Trace.PrintStream;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInt(final int i) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void writeInt(int i)
			{
				out_.WriteByte(i >> 24);
				out_.WriteByte(i >> 16);
				out_.WriteByte(i >> 8);
				out_.WriteByte(i);
		  bytesSent_ += 4;
				if (debug_)
				{
					debugInt(i);
				}
			}

			internal void debugInt(int i)
			{
				debugByte(i >> 24);
				debugByte(i >> 16);
				debugByte(i >> 8);
				debugByte(i);
			}

			internal void debugShort(int i)
			{
				debugByte(i >> 8);
				debugByte(i);
			}

			internal void debugByte(int i)
			{
				if (tracePrintStream != null)
				{
					if (debugCounter_ == 0)
					{
						lock (formatter_)
						{
							tracePrintStream.print(formatter_.format(DateTime.Now));
							tracePrintStream.println(" Data stream sent...");
						}
					}
					int highNibble = (0x00FF & i) >> 4;
					int lowNibble = 0x000F & i;
					tracePrintStream.print(CHAR[highNibble]);
					tracePrintStream.print(CHAR[lowNibble]);
					if (++debugCounter_ % 16 == 0)
					{
						tracePrintStream.println();
					}
					else
					{
						tracePrintStream.print(" ");
					}
				}
			}

			internal void debugBytes(sbyte[] b, int offset, int length)
			{
				for (int i = offset; i < offset + length; ++i)
				{
					debugByte(b[i]);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeShort(final int i) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void writeShort(int i)
			{
				out_.WriteByte(i >> 8);
				out_.WriteByte(i);
		  bytesSent_ += 2;
				if (debug_)
				{
					debugShort(i);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeLong(final long l) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void writeLong(long l)
			{
				int i1 = (int)(l >> 32);
				int i2 = (int) l;
				writeInt(i1);
				writeInt(i2);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(final byte[] b) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void write(sbyte[] b)
			{
				out_.Write(b, 0, b.Length);
		  bytesSent_ += b.Length;
				if (debug_)
				{
					debugBytes(b, 0, b.Length);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(final byte[] b, final int offset, final int length) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void write(sbyte[] b, int offset, int length)
			{
				out_.Write(b, offset, length);
		  bytesSent_ += length;
				if (debug_)
				{
					debugBytes(b, offset, length);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(final int i) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void write(int i)
			{
				out_.WriteByte(i);
		  ++bytesSent_;
				if (debug_)
				{
					debugByte(i);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeByte(final int i) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
			public void writeByte(int i)
			{
				out_.WriteByte(i);
		  ++bytesSent_;
				if (debug_)
				{
					debugByte(i);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
			public void close()
			{
				out_.Close();
				if (debug_ && tracePrintStream != null)
				{
					tracePrintStream.println();
					debugCounter_ = 0;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
			public void flush()
			{
				out_.Flush();
				if (debug_ && tracePrintStream != null)
				{
					tracePrintStream.println();
					debugCounter_ = 0;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void writePadEBCDIC10(String s, HostOutputStream out) throws IOException
		protected internal static void writePadEBCDIC10(string s, HostOutputStream @out)
		{
			Conv.writePadEBCDIC10(s, @out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void writePadEBCDIC(String s, int len, HostOutputStream out) throws IOException
		protected internal static void writePadEBCDIC(string s, int len, HostOutputStream @out)
		{
			Conv.writePadEBCDIC(s, len, @out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void writeStringToUnicodeBytes(String s, HostOutputStream out) throws IOException
		protected internal static void writeStringToUnicodeBytes(string s, HostOutputStream @out)
		{
			Conv.writeStringToUnicodeBytes(s, @out);
		}
	}

}