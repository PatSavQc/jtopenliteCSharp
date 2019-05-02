using System;
using System.IO;
using System.Net.Sockets;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseConnection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
//
// Major Change Log
// Version   Date       Description
// -------   ---------- ---------------------------------------
// 1.7       2012.10.04 Moved counting of bytes actually read from the 
//                      datastream to the in_ object.  This removed a lot of 
//                      the counting logic that was added to deal with 
//                      compression. 
//           
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	using com.ibm.jtopenlite;


	/// <summary>
	/// Represents a TCP/IP socket connection to the System i Database host server (QUSRWRK/QZDASOINIT job).
	/// 
	/// </summary>
	public class DatabaseConnection : HostServerConnection, OperationalResultBitmap
	{
	  private readonly sbyte[] byteBuffer_ = new sbyte[1024];
	  private char[] charBuffer_ = new char[1024];

	  private const bool DEBUG = false;

	  public const int DEFAULT_DATABASE_SERVER_PORT = 8471;
	  public const int DEFAULT_SSL_DATABASE_SERVER_PORT = 9471;
	private const int TYPE_CALL = 3;

	  private int correlationID_ = 1;
	  private int currentRPB_;

	  private bool compress_ = true;

	  private int newCorrelationID()
	  {
		if (correlationID_ == 0x7FFFFFFF)
		{
			correlationID_ = 0;
		}
		return ++correlationID_;
	  }

	  private DatabaseWarningCallback warningCallback_;
	  private DatabaseSQLCommunicationsAreaCallback sqlcaCallback_;

	  private bool returnMessageInfo_ = false;

	  private DatabaseConnection(SystemInfo info, Socket socket, HostInputStream @in, HostOutputStream @out, string user, string jobName) : base(info, user, jobName, socket, @in, @out)
	  {

		// When we run locally on the iSeries, don't use data stream compression, it slows us down.
		InetAddress i = socket.InetAddress;
		if (i.LoopbackAddress)
		{
		  compress_ = false;
		}
		else
		{
		  string sys = info.System;
		  if (sys.Equals("localhost", StringComparison.OrdinalIgnoreCase) || sys.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
		  {
			compress_ = false;
		  }
		  else
		  {
			try
			{
			  if (i.Equals(InetAddress.LocalHost))
			  {
				compress_ = false;
			  }
			}
			catch (Exception)
			{
			}
		  }
		}
	  }

	  /// <summary>
	  /// Indicates if the MESSAGE_ID, FIRST_LEVEL_TEXT, and SECOND_LEVEL_TEXT bits are set on
	  /// the operational result bitmap for a database request.
	  /// 
	  /// </summary>
	  public virtual bool MessageInfoReturned
	  {
		  get
		  {
			return returnMessageInfo_;
		  }
		  set
		  {
			returnMessageInfo_ = value;
		  }
	  }


	  public virtual bool Debug
	  {
		  set
		  {
			in_.Debug = value;
			out_.Debug = value;
		  }
	  }

	  public virtual DatabaseWarningCallback WarningCallback
	  {
		  set
		  {
			warningCallback_ = value;
		  }
	  }

	  public virtual DatabaseSQLCommunicationsAreaCallback SQLCommunicationsAreaCallback
	  {
		  set
		  {
			sqlcaCallback_ = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendEndJobRequest() throws IOException
	  protected internal override void sendEndJobRequest()
	  {
		// Header.
		out_.writeInt(40); // Length is 40 if server ID is E004.
		out_.writeShort(0); // Header ID.
		out_.writeShort(0xE004); // Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(0); // Correlation ID.
		out_.writeShort(0); // Template length.
		out_.writeShort(0x1FFF); // ReqRep ID for database server.

		// Template.
		out_.writeInt(0);
		out_.writeInt(0);
		out_.writeInt(0);
		out_.writeInt(0);
		out_.writeInt(0);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatabaseConnection getConnection(String system, String user, String password) throws IOException
	  public static DatabaseConnection getConnection(string system, string user, string password)
	  {
		return getConnection(false, system, user, password);
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatabaseConnection getConnection(final boolean isSSL, String system, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DatabaseConnection getConnection(bool isSSL, string system, string user, string password)
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
//ORIGINAL LINE: public static DatabaseConnection getConnection(SystemInfo info, String user, String password) throws IOException
	  public static DatabaseConnection getConnection(SystemInfo info, string user, string password)
	  {
		return getConnection(false, info, user, password);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatabaseConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DatabaseConnection getConnection(bool isSSL, SystemInfo info, string user, string password)
	  {
		return getConnection(isSSL, info, user, password, isSSL ? DEFAULT_SSL_DATABASE_SERVER_PORT : DEFAULT_DATABASE_SERVER_PORT);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatabaseConnection getConnection(SystemInfo info, String user, String password, int databasePort) throws IOException
	  public static DatabaseConnection getConnection(SystemInfo info, string user, string password, int databasePort)
	  {
		  return getConnection(false, info, user, password, databasePort);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DatabaseConnection getConnection(final boolean isSSL, SystemInfo info, String user, String password, int databasePort) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static DatabaseConnection getConnection(bool isSSL, SystemInfo info, string user, string password, int databasePort)
	  {
		if (databasePort < 0 || databasePort > 65535)
		{
		  throw new IOException("Bad database port: " + databasePort);
		}
		DatabaseConnection conn = null;

		Socket databaseServer = isSSL? SSLSocketFactory.Default.createSocket(info.System, databasePort) : new Socket(info.System, databasePort);
	//    databaseServer.setKeepAlive(false);
	//    databaseServer.setReceiveBufferSize(8192);
	//    databaseServer.setSendBufferSize(8192);
	//    databaseServer.setSoLinger(true, 0);
	//    databaseServer.setSoTimeout(0);
	//    databaseServer.setTcpNoDelay(false);
		databaseServer.setPerformancePreferences(0,1,2);
		Stream @in = databaseServer.InputStream;
		Stream @out = databaseServer.OutputStream;
		try
		{
		  // Exchange random seeds.
		  HostOutputStream dout = new HostOutputStream(new BufferedOutputStream(@out, 1024));
		  HostInputStream din = new HostInputStream(new BufferedInputStream(@in, 32768));
		  string jobName = connect(info, dout, din, 0xE004, user, password);

	//      din.setDebug(true);
	//      dout.setDebug(true);
		  conn = new DatabaseConnection(info, databaseServer, din, dout, user, jobName);
		  return conn;
		}
		finally
		{
		  if (conn == null)
		  {
			@in.Close();
			@out.Close();
			databaseServer.close();
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFullReply(String name) throws IOException
	  private void readFullReply(string name)
	  {
		int length = readReplyHeader(name);
		skipBytes(length - 40);
		in_.end();
	  }

	  public virtual int CurrentRequestParameterBlockID
	  {
		  get
		  {
			return currentRPB_;
		  }
		  set
		  {
			currentRPB_ = value;
		  }
	  }


	  /// <summary>
	  /// Sends a request to create an RPB and sets the current RPB ID to be the one specified.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createRequestParameterBlock(DatabaseCreateRequestParameterBlockAttributes attribs, int rpbID) throws IOException
	  public virtual void createRequestParameterBlock(DatabaseCreateRequestParameterBlockAttributes attribs, int rpbID)
	  {
		sendCreateSQLRPBRequest(attribs, true, rpbID);
		out_.flush();

		readFullReply("createSQLRPB");
		currentRPB_ = rpbID;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deleteRequestParameterBlock(DatabaseDeleteRequestParameterBlockAttributes attribs, int rpbID) throws IOException
	  public virtual void deleteRequestParameterBlock(DatabaseDeleteRequestParameterBlockAttributes attribs, int rpbID)
	  {
		sendDeleteSQLRPBRequest(attribs, rpbID);
		out_.flush();

		readFullReply("deleteSQLRPB");
	  }

	  /// <summary>
	  /// Sends a request to reset an RPB and sets the current RPB ID to be the one specified.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetRequestParameterBlock(DatabaseCreateRequestParameterBlockAttributes attribs, int rpbID) throws IOException
	  public virtual void resetRequestParameterBlock(DatabaseCreateRequestParameterBlockAttributes attribs, int rpbID)
	  {
		sendResetSQLRPBRequest(attribs, true, rpbID);
		out_.flush();

		readFullReply("resetSQLRPB");
		currentRPB_ = rpbID;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void prepare(DatabasePrepareAttributes attribs) throws IOException
	  public virtual void prepare(DatabasePrepareAttributes attribs)
	  {
		sendPrepareRequest(attribs);
		out_.flush();

		readFullReply("prepare");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void prepareAndDescribe(DatabasePrepareAndDescribeAttributes attribs, DatabaseDescribeCallback listener, DatabaseParameterMarkerCallback pmListener) throws IOException
	  public virtual void prepareAndDescribe(DatabasePrepareAndDescribeAttributes attribs, DatabaseDescribeCallback listener, DatabaseParameterMarkerCallback pmListener)
	  {
		sendPrepareAndDescribeRequest(attribs);
		out_.flush();

		parseReply("prepareAndDescribe", listener, null, null, pmListener, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void prepareAndExecute(DatabasePrepareAndExecuteAttributes attribs, DatabaseDescribeCallback listener) throws IOException
	  public virtual void prepareAndExecute(DatabasePrepareAndExecuteAttributes attribs, DatabaseDescribeCallback listener)
	  {
		sendPrepareAndExecuteRequest(attribs);
		out_.flush();

		parseReply("prepareAndExecute", listener, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendPrepareAndExecuteRequest(DatabasePrepareAndExecuteAttributes attribs) throws IOException
	  private void sendPrepareAndExecuteRequest(DatabasePrepareAndExecuteAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			length += getPrepareStatementNameLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			length += getSQLStatementTextLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			length += getSQLStatementTypeLength(attribs);
			++parms;
		  }
		  if (attribs.PrepareOptionSet)
		  {
			length += getPrepareOptionLength(attribs);
			++parms;
		  }
		  if (attribs.OpenAttributesSet)
		  {
			length += getOpenAttributesLength(attribs);
			++parms;
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			length += getPackageNameLength(attribs);
			++parms;
		  }
		  if (attribs.PackageLibrarySet)
		  {
			length += getPackageLibraryLength(attribs);
			++parms;
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			length += getExtendedColumnDescriptorOptionLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			length += getExtendedSQLStatementTextLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x180D);
		writeTemplate(parms);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			writePrepareOption(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			writeExtendedColumnDescriptorOption(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		}
	  }

	  private int getPackageLibraryLength(AttributePackageLibrary a)
	  {
		return 10 + a.PackageLibrary.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writePackageLibrary(AttributePackageLibrary a) throws IOException
	  private void writePackageLibrary(AttributePackageLibrary a)
	  {
		string lib = a.PackageLibrary;
		out_.writeInt(10 + lib.Length);
		out_.writeShort(0x3801);
		out_.writeShort(37);
		out_.writeShort(lib.Length);
		writePadEBCDIC(lib, lib.Length, out_);
	  }

	  private int getPackageNameLength(AttributePackageName a)
	  {
		return 10 + a.PackageName.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writePackageName(AttributePackageName a) throws IOException
	  private void writePackageName(AttributePackageName a)
	  {
		string name = a.PackageName;
		out_.writeInt(10 + name.Length);
		out_.writeShort(0x3804);
		out_.writeShort(37);
		out_.writeShort(name.Length);
		writePadEBCDIC(name, name.Length, out_);
	  }

	  private int getPrepareStatementNameLength(AttributePrepareStatementName a)
	  {
		return 10 + a.PrepareStatementName.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writePrepareStatementName(AttributePrepareStatementName a) throws IOException
	  private void writePrepareStatementName(AttributePrepareStatementName a)
	  {
		string name = a.PrepareStatementName;
		out_.writeInt(10 + name.Length);
		out_.writeShort(0x3806);
		out_.writeShort(37);
		out_.writeShort(name.Length);
		writePadEBCDIC(name, name.Length, out_);
	  }

	  private int getSQLStatementTextLength(AttributeSQLStatementText a)
	  {
		return 10 + (a.SQLStatementText.Length * 2);
	//    return 10 + a.getSQLStatementText().length();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLStatementText(AttributeSQLStatementText a) throws IOException
	  private void writeSQLStatementText(AttributeSQLStatementText a)
	  {
		string text = a.SQLStatementText;
		out_.writeInt(10 + (text.Length * 2));
	//    out_.writeInt(10+text.length());
		out_.writeShort(0x3807);
		out_.writeShort(13488);
	//    out_.writeShort(37);
	//    byte[] b = text.getBytes("Cp037");
	//    out_.writeShort(b.length);
	//    out_.write(b);
		out_.writeShort(text.Length * 2);
		writeStringToUnicodeBytes(text, out_);
	  }

	  private int getSQLStatementTypeLength(AttributeSQLStatementType a)
	  {
		return 8;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLStatementType(AttributeSQLStatementType a) throws IOException
	  private void writeSQLStatementType(AttributeSQLStatementType a)
	  {
		out_.writeInt(8);
		out_.writeShort(0x3812);
		out_.writeShort(a.SQLStatementType);
	  }

	  private int getPrepareOptionLength(AttributePrepareOption a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writePrepareOption(AttributePrepareOption a) throws IOException
	  private void writePrepareOption(AttributePrepareOption a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3808);
		out_.writeByte(a.PrepareOption);
	  }

	  private int getOpenAttributesLength(AttributeOpenAttributes a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeOpenAttributes(AttributeOpenAttributes a) throws IOException
	  private void writeOpenAttributes(AttributeOpenAttributes a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3809);
		out_.writeByte(a.OpenAttributes);
	  }

	  private int getTranslateIndicatorLength(AttributeTranslateIndicator a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTranslateIndicator(AttributeTranslateIndicator a) throws IOException
	  private void writeTranslateIndicator(AttributeTranslateIndicator a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3805);
		out_.writeByte(a.TranslateIndicator);
	  }

	  private int getRLECompressedFunctionParametersLength(AttributeRLECompressedFunctionParameters a)
	  {
		return 10 + a.RLECompressedFunctionParameters.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeRLECompressedFunctionParameters(AttributeRLECompressedFunctionParameters a) throws IOException
	  private void writeRLECompressedFunctionParameters(AttributeRLECompressedFunctionParameters a)
	  {
		sbyte[] data = a.RLECompressedFunctionParameters;
		out_.writeInt(10 + data.Length);
		out_.writeShort(0x3832);
		out_.writeInt(data.Length);
		out_.write(data);
	  }

	  private int getExtendedColumnDescriptorOptionLength(AttributeExtendedColumnDescriptorOption a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeExtendedColumnDescriptorOption(AttributeExtendedColumnDescriptorOption a) throws IOException
	  private void writeExtendedColumnDescriptorOption(AttributeExtendedColumnDescriptorOption a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3829);
		out_.writeByte(a.ExtendedColumnDescriptorOption);
	  }

	  private int getExtendedSQLStatementTextLength(AttributeExtendedSQLStatementText a)
	  {
		return 12 + (a.ExtendedSQLStatementText.Length * 2);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeExtendedSQLStatementText(AttributeExtendedSQLStatementText a) throws IOException
	  private void writeExtendedSQLStatementText(AttributeExtendedSQLStatementText a)
	  {
		string text = a.ExtendedSQLStatementText;
		out_.writeInt(12 + (text.Length * 2));
		out_.writeShort(0x3831);
		out_.writeShort(13488);
		out_.writeInt(text.Length * 2);
		writeStringToUnicodeBytes(text, out_);
	  }

	  private int getSyncPointCountLength(AttributeSyncPointCount a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSyncPointCount(AttributeSyncPointCount a) throws IOException
	  private void writeSyncPointCount(AttributeSyncPointCount a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3816);
		out_.writeInt(a.SyncPointCount);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendPrepareRequest(DatabasePrepareAttributes attribs) throws IOException
	  private void sendPrepareRequest(DatabasePrepareAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			length += getPrepareStatementNameLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			length += getSQLStatementTextLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			length += getSQLStatementTypeLength(attribs);
			++parms;
		  }
		  if (attribs.PrepareOptionSet)
		  {
			length += getPrepareOptionLength(attribs);
			++parms;
		  }
		  if (attribs.OpenAttributesSet)
		  {
			length += getOpenAttributesLength(attribs);
			++parms;
		  }
		  if (attribs.PackageNameSet)
		  {
			length += getPackageNameLength(attribs);
			++parms;
		  }
		  if (attribs.PackageLibrarySet)
		  {
			length += getPackageLibraryLength(attribs);
			++parms;
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			length += getExtendedColumnDescriptorOptionLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			length += getExtendedSQLStatementTextLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x1800);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			writePrepareOption(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			writeExtendedColumnDescriptorOption(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendStreamFetchRequest(DatabaseStreamFetchAttributes attribs) throws IOException
	  private void sendStreamFetchRequest(DatabaseStreamFetchAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			length += getPrepareStatementNameLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			length += getSQLStatementTextLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			length += getSQLStatementTypeLength(attribs);
			++parms;
		  }
		  if (attribs.PackageNameSet)
		  {
			length += getPackageNameLength(attribs);
			++parms;
		  }
		  if (attribs.PackageLibrarySet)
		  {
			length += getPackageLibraryLength(attribs);
			++parms;
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.SyncPointCountSet)
		  {
			length += getSyncPointCountLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			length += getExtendedSQLStatementTextLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x180C);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.RESULT_DATA;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.SyncPointCountSet)
		  {
			writeSyncPointCount(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendEndStreamFetchRequest(DatabaseEndStreamFetchAttributes attribs) throws IOException
	  private void sendEndStreamFetchRequest(DatabaseEndStreamFetchAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x1813);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHeader(final int length, final int reqRepID) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void writeHeader(int length, int reqRepID)
	  {
		out_.writeInt(length); // Length.
	//    out_.writeShort(0); // Header ID.
	//    out_.writeShort(0xE004); // Server ID.
		out_.writeInt(0x0000E004); // Header ID and Server ID.
		out_.writeInt(0); // CS instance.
		out_.writeInt(newCorrelationID()); // Correlation ID.
		out_.writeShort(20); // Template length.
		out_.writeShort(reqRepID); // ReqRep ID.
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTemplate(final int parms) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void writeTemplate(int parms)
	  {
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT;
		writeTemplate(parms, template);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTemplate(final int parms, final int orsBitmap) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void writeTemplate(int parms, int orsBitmap)
	  {
		writeTemplate(parms, orsBitmap, 0);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTemplate(final int parms, final int orsBitmap, final int pmHandle) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void writeTemplate(int parms, int orsBitmap, int pmHandle)
	  {
		writeTemplate(parms, orsBitmap, pmHandle, currentRPB_);
	  }

	  /// <summary>
	  /// writes the template, adding the SQLCA, REPLY_RELCOMPRESSED, and MESSAGE_ID bits if needed
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTemplate(final int parms, final int orsBitmap, final int pmHandle, final int rpbHandle) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void writeTemplate(int parms, int orsBitmap, int pmHandle, int rpbHandle)
	  {
		int bitmap = sqlcaCallback_ != null ? (orsBitmap | OperationalResultBitmap_Fields.SQLCA) : orsBitmap;
		if (compress_)
		{
			bitmap = bitmap | OperationalResultBitmap_Fields.REPLY_RLE_COMPRESSED;
		}
		if (returnMessageInfo_)
		{
			bitmap = bitmap | OperationalResultBitmap_Fields.MESSAGE_ID | OperationalResultBitmap_Fields.FIRST_LEVEL_TEXT | OperationalResultBitmap_Fields.SECOND_LEVEL_TEXT;
		}
		out_.writeInt(bitmap); // Operational result (ORS) bitmap.
		out_.writeInt(0); // Reserved.
	//    out_.writeShort(1); // Return ORS handle - after operation completes.
	//    out_.writeShort(1); // Fill ORS handle.
		out_.writeInt(0x00010001); // Return ORS handle, Fill ORS handle.
		out_.writeShort(0); // Based on ORS handle.
		out_.writeShort(rpbHandle); // Request parameter block (RPB) handle.
	//    out_.writeInt(0x00000001); // Based on ORS handle, Request parameter block (RPB) handle.
		out_.writeShort(pmHandle); // Parameter marker descriptor handle.
		out_.writeShort(parms); // Parameter count.
	  }

	  private int getSQLParameterMarkerDataLength(AttributeSQLParameterMarkerData a)
	  {
		return 6 + a.SQLParameterMarkerData.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLParameterMarkerData(AttributeSQLParameterMarkerData a) throws IOException
	  private void writeSQLParameterMarkerData(AttributeSQLParameterMarkerData a)
	  {
		sbyte[] data = a.SQLParameterMarkerData;
		out_.writeInt(6 + data.Length);
		out_.writeShort(0x3811);
		out_.write(data);
	  }

	  private int getSQLExtendedParameterMarkerDataLength(AttributeSQLExtendedParameterMarkerData a)
	  {
		return 6 + a.SQLExtendedParameterMarkerData.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLExtendedParameterMarkerData(AttributeSQLExtendedParameterMarkerData a) throws IOException
	  private void writeSQLExtendedParameterMarkerData(AttributeSQLExtendedParameterMarkerData a)
	  {
		sbyte[] data = a.SQLExtendedParameterMarkerData;
		out_.writeInt(6 + data.Length);
		out_.writeShort(0x381F);
		out_.write(data);
	  }

	  private int getSQLParameterMarkerBlockIndicatorLength(AttributeSQLParameterMarkerBlockIndicator a)
	  {
		return 8;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLParameterMarkerBlockIndicator(AttributeSQLParameterMarkerBlockIndicator a) throws IOException
	  private void writeSQLParameterMarkerBlockIndicator(AttributeSQLParameterMarkerBlockIndicator a)
	  {
		out_.writeInt(8);
		out_.writeShort(0x3814);
		out_.writeShort(a.SQLParameterMarkerBlockIndicator);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendFetchRequest(DatabaseFetchAttributes attribs) throws IOException
	  private void sendFetchRequest(DatabaseFetchAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			++parms;
			length += getVariableFieldCompressionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			++parms;
			length += getFetchScrollOptionLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			++parms;
			length += getFetchBufferSizeLength(attribs);
		  }
		}

		writeHeader(length, 0x180B);
		// writeTemplate(parms, compress_ ? 0x84040000 : 0x84000000);
		// Note:  The new writeTemplate adds compression if needed.  
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.RESULT_DATA;
		writeTemplate(parms, template);


		if (attribs != null)
		{
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			writeVariableFieldCompression(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			writeFetchScrollOption(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			writeFetchBufferSize(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendPackageRequest(DatabasePackageAttributes attribs, boolean createOrDelete) throws IOException
	  private void sendPackageRequest(DatabasePackageAttributes attribs, bool createOrDelete)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		}

		writeHeader(length, createOrDelete ? 0x180F : 0x1811);
		// writeTemplate(parms, 0xF2000000);

		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.SQLCA;
		writeTemplate(parms, template);


		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

	  private int getReturnSizeLength(AttributeReturnSize a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeReturnSize(AttributeReturnSize a) throws IOException
	  private void writeReturnSize(AttributeReturnSize a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3815);
		out_.writeInt(a.ReturnSize);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendRetrievePackageRequest(DatabaseRetrievePackageAttributes attribs) throws IOException
	  private void sendRetrievePackageRequest(DatabaseRetrievePackageAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.ReturnSizeSet)
		  {
			++parms;
			length += getReturnSizeLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		}

		writeHeader(length, 0x1815);
		// writeTemplate(parms, 0x80100000);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.PACKAGE_INFORMATION;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.ReturnSizeSet)
		  {
			writeReturnSize(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

	  private int getSQLParameterMarkerDataFormatLength(AttributeSQLParameterMarkerDataFormat a)
	  {
		return 6 + a.SQLParameterMarkerDataFormat.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeSQLParameterMarkerDataFormat(AttributeSQLParameterMarkerDataFormat a) throws IOException
	  private void writeSQLParameterMarkerDataFormat(AttributeSQLParameterMarkerDataFormat a)
	  {
		out_.writeInt(getSQLParameterMarkerDataFormatLength(a));
		out_.writeShort(0x3801);
		out_.write(a.SQLParameterMarkerDataFormat);
	  }

	  private int getExtendedSQLParameterMarkerDataFormatLength(AttributeExtendedSQLParameterMarkerDataFormat a)
	  {
		return 6 + a.ExtendedSQLParameterMarkerDataFormat.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeExtendedSQLParameterMarkerDataFormat(AttributeExtendedSQLParameterMarkerDataFormat a) throws IOException
	  private void writeExtendedSQLParameterMarkerDataFormat(AttributeExtendedSQLParameterMarkerDataFormat a)
	  {
		out_.writeInt(getExtendedSQLParameterMarkerDataFormatLength(a));
		out_.writeShort(0x381E);
		out_.write(a.ExtendedSQLParameterMarkerDataFormat);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendDeleteDescriptorRequest(DatabaseDeleteDescriptorAttributes attribs, int descriptorHandle) throws IOException
	  private void sendDeleteDescriptorRequest(DatabaseDeleteDescriptorAttributes attribs, int descriptorHandle)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		}

		writeHeader(length, 0x1E01);
		// writeTemplate(parms, 0x80800000, descriptorHandle);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.PARAMETER_MARKER_FORMAT;
		writeTemplate(parms, template, descriptorHandle);


		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendChangeDescriptorRequest(DatabaseChangeDescriptorAttributes attribs, int descriptorHandle) throws IOException
	  private void sendChangeDescriptorRequest(DatabaseChangeDescriptorAttributes attribs, int descriptorHandle)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.SQLParameterMarkerDataFormatSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataFormatLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.ExtendedSQLParameterMarkerDataFormatSet)
		  {
			++parms;
			length += getExtendedSQLParameterMarkerDataFormatLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		}

		writeHeader(length, 0x1E00);
		// writeTemplate(parms, 0x00040000, descriptorHandle);
		int template = OperationalResultBitmap_Fields.REPLY_RLE_COMPRESSED;
		writeTemplate(parms, template, descriptorHandle);


		if (attribs != null)
		{
		  if (attribs.SQLParameterMarkerDataFormatSet)
		  {
			writeSQLParameterMarkerDataFormat(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.ExtendedSQLParameterMarkerDataFormatSet)
		  {
			writeExtendedSQLParameterMarkerDataFormat(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendDescribeParameterMarkerRequest(DatabaseDescribeParameterMarkerAttributes attribs) throws IOException
	  private void sendDescribeParameterMarkerRequest(DatabaseDescribeParameterMarkerAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		}

		writeHeader(length, 0x1802);
		// writeTemplate(parms, 0xF0800000);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.PARAMETER_MARKER_FORMAT; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT | PARAMETER_MARKER_FORMAT;
		writeTemplate(parms, template);


		if (attribs != null)
		{
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendDescribeRequest(DatabaseDescribeAttributes attribs) throws IOException
	  private void sendDescribeRequest(DatabaseDescribeAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		}

		writeHeader(length, 0x1801);
		// writeTemplate(parms, 0x88000000);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.DATA_FORMAT;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendOpenAndDescribeRequest(DatabaseOpenAndDescribeAttributes attribs, int pmDescriptorHandle) throws IOException
	  private void sendOpenAndDescribeRequest(DatabaseOpenAndDescribeAttributes attribs, int pmDescriptorHandle)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			++parms;
			length += getOpenAttributesLength(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			++parms;
			length += getVariableFieldCompressionLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			++parms;
			length += getResultSetHoldabilityOptionLength(attribs);
		  }
		}

		writeHeader(length, 0x1804);
	//    writeTemplate(parms, 0xF8000000, pmDescriptorHandle);
	//    writeTemplate(parms, 0x86040000, pmDescriptorHandle);
	//    writeTemplate(parms, 0xFE040000, pmDescriptorHandle);
	//    Statement before update to use constants. 
	//    writeTemplate(parms, 0xF8040000, pmDescriptorHandle);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.DATA_FORMAT | OperationalResultBitmap_Fields.REPLY_RLE_COMPRESSED; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT | DATA_FORMAT | REPLY_RLE_COMPRESSED;
		writeTemplate(parms, template, pmDescriptorHandle);


		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			writeVariableFieldCompression(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			writeResultSetHoldabilityOption(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendExecuteOrOpenAndDescribeRequest(DatabaseExecuteOrOpenAndDescribeAttributes attribs) throws IOException
	  private void sendExecuteOrOpenAndDescribeRequest(DatabaseExecuteOrOpenAndDescribeAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			++parms;
			length += getOpenAttributesLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			++parms;
			length += getResultSetHoldabilityOptionLength(attribs);
		  }
		}

		writeHeader(length, 0x1812);
		// writeTemplate(parms, 0x88000000);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.DATA_FORMAT;
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			writeResultSetHoldabilityOption(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendOpenDescribeFetchRequest(DatabaseOpenDescribeFetchAttributes attribs) throws IOException
	  private void sendOpenDescribeFetchRequest(DatabaseOpenDescribeFetchAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			++parms;
			length += getOpenAttributesLength(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			++parms;
			length += getVariableFieldCompressionLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			++parms;
			length += getResultSetHoldabilityOptionLength(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			++parms;
			length += getFetchScrollOptionLength(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			++parms;
			length += getFetchBufferSizeLength(attribs);
		  }
		}

		writeHeader(length, 0x180E);
		// writeTemplate(parms, 0x86048000);//0x8C000000);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.RESULT_DATA | OperationalResultBitmap_Fields.SQLCA | OperationalResultBitmap_Fields.REPLY_RLE_COMPRESSED | OperationalResultBitmap_Fields.RETURN_RESULT_SET_ATTRIBUTES;
		writeTemplate(parms, template); //0x8C000000);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			writeVariableFieldCompression(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			writeResultSetHoldabilityOption(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			writeFetchScrollOption(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			writeFetchBufferSize(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void openAndDescribe(DatabaseOpenAndDescribeAttributes attribs, DatabaseDescribeCallback listener) throws IOException
	  public virtual void openAndDescribe(DatabaseOpenAndDescribeAttributes attribs, DatabaseDescribeCallback listener)
	  {
		openAndDescribe(attribs, 0, listener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void openAndDescribe(DatabaseOpenAndDescribeAttributes attribs, int parameterMarkerDescriptorHandle, DatabaseDescribeCallback listener) throws IOException
	  public virtual void openAndDescribe(DatabaseOpenAndDescribeAttributes attribs, int parameterMarkerDescriptorHandle, DatabaseDescribeCallback listener)
	  {
		sendOpenAndDescribeRequest(attribs, parameterMarkerDescriptorHandle);
		out_.flush();

		parseReply("openAndDescribe", listener, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void openDescribeFetch(DatabaseOpenDescribeFetchAttributes attribs, DatabaseDescribeCallback describeListener, DatabaseFetchCallback fetchListener) throws IOException
	  public virtual void openDescribeFetch(DatabaseOpenDescribeFetchAttributes attribs, DatabaseDescribeCallback describeListener, DatabaseFetchCallback fetchListener)
	  {
		sendOpenDescribeFetchRequest(attribs);
		out_.flush();

		parseReply("openDescribeFetch", describeListener, fetchListener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createPackage(DatabasePackageAttributes attribs) throws IOException
	  public virtual void createPackage(DatabasePackageAttributes attribs)
	  {
		sendPackageRequest(attribs, true);
		out_.flush();

		parseReply("createPackage", null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deletePackage(DatabasePackageAttributes attribs) throws IOException
	  public virtual void deletePackage(DatabasePackageAttributes attribs)
	  {
		sendPackageRequest(attribs, false);
		out_.flush();

		parseReply("deletePackage", null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void retrievePackage(DatabaseRetrievePackageAttributes attribs, DatabasePackageCallback listener) throws IOException
	  public virtual void retrievePackage(DatabaseRetrievePackageAttributes attribs, DatabasePackageCallback listener)
	  {
		sendRetrievePackageRequest(attribs);
		out_.flush();

		parseReply("retrievePackage", listener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void describeParameterMarker(DatabaseDescribeParameterMarkerAttributes attribs, DatabaseParameterMarkerCallback callback) throws IOException
	  public virtual void describeParameterMarker(DatabaseDescribeParameterMarkerAttributes attribs, DatabaseParameterMarkerCallback callback)
	  {
		sendDescribeParameterMarkerRequest(attribs);
		out_.flush();

		parseReply("describeParameterMarker", callback);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void changeDescriptor(DatabaseChangeDescriptorAttributes attribs, int descriptorHandle) throws IOException
	  public virtual void changeDescriptor(DatabaseChangeDescriptorAttributes attribs, int descriptorHandle)
	  {
		sendChangeDescriptorRequest(attribs, descriptorHandle);
		out_.flush();

	//    parseReply("changeDescriptor", null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deleteDescriptor(DatabaseDeleteDescriptorAttributes attribs, int descriptorHandle) throws IOException
	  public virtual void deleteDescriptor(DatabaseDeleteDescriptorAttributes attribs, int descriptorHandle)
	  {
		sendDeleteDescriptorRequest(attribs, descriptorHandle);
		out_.flush();

		parseReply("deleteDescriptor", null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void describe(DatabaseDescribeAttributes attribs, DatabaseDescribeCallback listener) throws IOException
	  public virtual void describe(DatabaseDescribeAttributes attribs, DatabaseDescribeCallback listener)
	  {
		sendDescribeRequest(attribs);
		out_.flush();

		parseReply("describe", listener, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseReply(String datastream, DatabaseParameterMarkerCallback callback) throws IOException
	  private void parseReply(string datastream, DatabaseParameterMarkerCallback callback)
	  {
		parseReply(datastream, null, null, null, callback, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseReply(String datastream, DatabasePackageCallback callback) throws IOException
	  private void parseReply(string datastream, DatabasePackageCallback callback)
	  {
		parseReply(datastream, null, null, callback, null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseReply(String datastream, DatabaseDescribeCallback describeCallback, DatabaseFetchCallback fetchCallback) throws IOException
	  private void parseReply(string datastream, DatabaseDescribeCallback describeCallback, DatabaseFetchCallback fetchCallback)
	  {
		parseReply(datastream, describeCallback, fetchCallback, null, null, null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readCompressedInt() throws IOException
	  private int readCompressedInt()
	  {
		int b1 = readCompressedByte();
		int b2 = readCompressedByte();
		int b3 = readCompressedByte();
		int b4 = readCompressedByte();
		return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readCompressedShort() throws IOException
	  private int readCompressedShort()
	  {
		int b1 = readCompressedByte();
		int b2 = readCompressedByte();
		return (b1 << 8) | b2;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readCompressedFully(final byte[] b) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void readCompressedFully(sbyte[] b)
	  {
		for (int i = 0; i < b.Length; ++i)
		{
		  b[i] = (sbyte)readCompressedByte();
		}
	  }

	  // Reads compressed bytes from the stream.  Returned the number of bytes actually read.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readCompressedFully(final byte[] b, final int off, final int len) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void readCompressedFully(sbyte[] b, int off, int len)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = off+len;
		int stop = off + len;
		for (int i = off; i < stop; ++i)
		{
		  b[i] = (sbyte)readCompressedByte();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void skipCompressedBytes(final int num) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void skipCompressedBytes(int num)
	  {
		for (int i = 0; i < num; ++i)
		{
		  readCompressedByte();
		}
	  }

	  private int rleRepeatValue1_ = 0;
	  private int rleRepeatValue2_ = 0;
	  private int rleRepeatTotal_ = 0;
	  private int rleRepeatCount_ = 0;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readCompressedByte() throws IOException
	  private int readCompressedByte()
	  {
		if (rleRepeatCount_ < rleRepeatTotal_)
		{
		  return (rleRepeatCount_++ % 2) == 0 ? rleRepeatValue1_ : rleRepeatValue2_;
		}
		int b1 = in_.readByte();
		if (b1 == 0x1B)
		{
		  // Escape byte.
		  int b2 = in_.readByte();

		  if (b2 == 0x1B)
		  {
			// Escape byte.
			return 0x1B;
		  }
		  else
		  {
			// Regular byte -- repeater record.
			rleRepeatValue1_ = b2;
			rleRepeatValue2_ = in_.readByte();
			int b4 = in_.readByte();
			int b5 = in_.readByte();
			rleRepeatTotal_ = ((b4 << 8) | b5) * 2;
			rleRepeatCount_ = 1;
			return rleRepeatValue1_;
		  }
		}
		else
		{
		  // Regular byte.
		  return b1;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readInt() throws IOException
	  private int readInt()
	  {
		return rleCompression_ ? readCompressedInt() : in_.readInt();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readShort() throws IOException
	  private int readShort()
	  {
		return rleCompression_ ? readCompressedShort() : in_.readShort();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readByte() throws IOException
	  private int readByte()
	  {
		return rleCompression_ ? readCompressedByte() : in_.readByte();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFullyReturnCount(byte[] b) throws IOException
	  private void readFullyReturnCount(sbyte[] b)
	  {
		if (rleCompression_)
		{
		  readCompressedFully(b);
		}
		else
		{
		  in_.readFully(b);
		}
	  }

	  //
	  // Reads len uncompressed bytes from the buffer
	  // Returns the number of bytes actually consumed from the
	  // input stream.
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFully(byte[] b, int off, int len) throws IOException
	  private void readFully(sbyte[] b, int off, int len)
	  {
		if (rleCompression_)
		{
		  readCompressedFully(b, off, len);
		}
		else
		{
		  in_.readFully(b, off, len);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void skipBytes(int num) throws IOException
	  private void skipBytes(int num)
	  {
		if (rleCompression_)
		{
		  skipCompressedBytes(num);
		}
		else
		{
		  in_.skipBytes(num);
		}
	  }

	  private bool rleCompression_ = false;

	  private readonly sbyte[] tempIndicator_ = new sbyte[2];

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseReply(String datastream, DatabaseDescribeCallback describeCallback, DatabaseFetchCallback fetchCallback, DatabasePackageCallback packageCallback, DatabaseParameterMarkerCallback pmCallback, DatabaseLOBDataCallback lobCallback) throws IOException
	  private void parseReply(string datastream, DatabaseDescribeCallback describeCallback, DatabaseFetchCallback fetchCallback, DatabasePackageCallback packageCallback, DatabaseParameterMarkerCallback pmCallback, DatabaseLOBDataCallback lobCallback)
	  {
		in_.resetLatestBytesReceived();
		int length = readReplyHeader(datastream);
		// int numRead = 40;
		// numRead is now obtained from in_.getLatestBytesReceived
		rleCompression_ = false;
		while (in_.LatestBytesReceived < length)
		{
		  int ll = readInt();
		  int cp = readShort();

		  if (cp == 0x3832)
		  {
			int realLength = readInt();
			rleCompression_ = true;
		  }
		  else if (cp == 0x3801 && warningCallback_ != null)
		  {
			if (ll > 6)
			{
			  // Message ID.
			  int shortBytesRead = 2;
			  int ccsid = readShort();

			  sbyte[] messageID = new sbyte[ll - 8];
			  readFullyReturnCount(messageID);
			  if (ll > charBuffer_.Length)
			  {
				charBuffer_ = new char[ll];
			  }
			  warningCallback_.newMessageID(Conv.ebcdicByteArrayToString(messageID, charBuffer_));

			}
		  }
		  else if (cp == 0x3802 && warningCallback_ != null)
		  {
			if (ll > 6)
			{
			  // First level message text.
			  int ccsid = readShort();
			  int len = readShort();

			  sbyte[] firstLevelMessageText = new sbyte[ll - 10];
			  readFullyReturnCount(firstLevelMessageText);
			  if (ll > charBuffer_.Length)
			  {
				charBuffer_ = new char[ll];
			  }

			  warningCallback_.newMessageText(Conv.ebcdicByteArrayToString(firstLevelMessageText, charBuffer_));
			}
		  }
		  else if (cp == 0x3803 && warningCallback_ != null)
		  {
			if (ll > 6)
			{
			  // Second level message text.
			  int ccsid = readShort();
			  int len = readShort();
			  sbyte[] secondLevelMessageText = new sbyte[ll - 10];
			  readFullyReturnCount(secondLevelMessageText);
			  if (ll > charBuffer_.Length)
			  {
				charBuffer_ = new char[ll];
			  }
			  warningCallback_.newSecondLevelText(Conv.ebcdicByteArrayToString(secondLevelMessageText, charBuffer_));
			}
		  }
		  else if (cp == 0x3811 && describeCallback != null)
		  {
			if (ll > 6)
			{
			  // oldNumRead is the number of bytes processed before processing the
			  // current datastream parameter.  As such, we subtract 6
			  // bytes for the header. 
			  int oldNumRead = (int) in_.LatestBytesReceived - 6;
			  int virtualRead = oldNumRead + 6;
			  // Extended column descriptors.
			  int numColumns = readInt();

			  int[] offsets = new int[numColumns];
			  int[] lengths = new int[numColumns];
			  skipBytes(6); // Reserved.
			  virtualRead += 10;

			  for (int i = 0; i < numColumns; ++i)
			  {
				int updateable = readByte();
				int searchable = readByte();
				int attributeBitmap = readShort();
				describeCallback.columnAttributes(i, updateable, searchable, (attributeBitmap & 0x8000) != 0, (attributeBitmap & 0x4000) == 0, (attributeBitmap & 0x2000) != 0, (attributeBitmap & 0x1000) != 0, (attributeBitmap & 0x0800) != 0, (attributeBitmap & 0x0400) != 0, (attributeBitmap & 0x0200) != 0, (attributeBitmap & 0x0100) == 0, (attributeBitmap & 0x0080) != 0, (attributeBitmap & 0x0040) != 0); // Row change timestamp.
				offsets[i] = readInt();
				lengths[i] = readInt();
				readInt(); // Reserved
				virtualRead += 16;
			  }
			  for (int i = 0; i < numColumns; ++i)
			  {
				int @base = virtualRead - oldNumRead;
				int toSkip = offsets[i] - @base;
				if (toSkip > 0)
				{
				  skipBytes(toSkip);
				  virtualRead += toSkip;
				}
				if (lengths[i] >= 8)
				{
				  int descRead = 0;
				  while (descRead < lengths[i])
				  {
					int oldDescRead = descRead;

					int descriptorLength = readInt();
					int codepoint = readShort();
					descRead += 6;
					virtualRead += 6;

					int ccsid = 37;
					int len = descriptorLength - 6;
					if (codepoint == 0x3902)
					{
					  ccsid = readShort();
					  descRead += 2;
					  virtualRead += 2;

					  len = descriptorLength - 8;
			  if (ccsid == 65535)
			  {
				  ccsid = 37;
			  }
					}
					readFully(byteBuffer_, 0, len);
					descRead += len;
					virtualRead += len;

					string name = Conv.ebcdicByteArrayToString(byteBuffer_, 0, len, charBuffer_, ccsid);
					switch (codepoint)
					{
					  case 0x3900:
						  describeCallback.baseColumnName(i, name);
						  break;
					  case 0x3901:
						  describeCallback.baseTableName(i, name);
						  break;
					  case 0x3902:
						  describeCallback.columnLabel(i, name);
						  break;
					  case 0x3904:
						  describeCallback.baseSchemaName(i, name);
						  break;
					  case 0x3905:
						  describeCallback.sqlFromTable(i, name);
						  break;
					  case 0x3906:
						  describeCallback.sqlFromSchema(i, name);
						  break;
					}
					int descSkip = descriptorLength - descRead + oldDescRead;
					if (descSkip > 0)
					{
					  skipBytes(descSkip);
					  descRead += descSkip;
					  virtualRead += descSkip;
					}
				  }
				}
			  }
			  int remaining = ll - virtualRead + oldNumRead;
			  skipBytes(remaining);
			  virtualRead += remaining;
			}
		  }
		  else if (cp == 0x3812)
		  {
			if (ll > 6 && describeCallback != null)
			{
			  int oldNumRead = (int) in_.LatestBytesReceived - 6;
			  int virtualRead = oldNumRead;
			  virtualRead += 6;

			  // Super extended data format.
			  int consistencyToken = readInt();
			  int numFields = readInt();
			  int dateFormat = readByte();
			  int timeFormat = readByte();
			  int dateSeparator = readByte();
			  int timeSeparator = readByte();
			  int recordSize = readInt();

			  describeCallback.resultSetDescription(numFields, dateFormat, timeFormat, dateSeparator, timeSeparator, recordSize);

			  virtualRead += 16;

			  int[] offsets = new int[numFields];
			  int[] lengths = new int[numFields];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fixedLengthRead = 48*numFields;
			  int fixedLengthRead = 48 * numFields;
			  for (int i = 0; i < numFields; ++i)
			  {

				int fieldLL = readShort();
				int fieldType = readShort();
				int fieldLength = readInt();
				int fieldScale = readShort();
				int fieldPrecision = readShort();
				int fieldCCSID = readShort();
				readByte(); // reserved
				int fieldJoinRefPosition = readShort();
				readInt(); // reserved
				int fieldAttributeBitmap = readByte();
				readInt(); // reserved
				int fieldLOBMaxSize = readInt();
				readShort(); // reserved
				int offsetToVariableLengthInformation = readInt();
				int lengthOfVariableLengthInformation = readInt();
				readInt(); // Reserved
				readInt(); // Reserved
				virtualRead += 48;


				describeCallback.fieldDescription(i, fieldType, fieldLength, fieldScale, fieldPrecision, fieldCCSID, fieldJoinRefPosition, fieldAttributeBitmap, fieldLOBMaxSize);
				offsets[i] = (48 * i) + offsetToVariableLengthInformation - fixedLengthRead;
				lengths[i] = lengthOfVariableLengthInformation;
			  }
			  int varLengthRead = 0;
			  for (int i = 0; i < numFields; ++i)
			  {
				int toSkip = offsets[i] - varLengthRead;
				skipBytes(toSkip);
				virtualRead += toSkip;
				varLengthRead += toSkip; // Also count the skipped bytes
				int variableLength = lengths[i];
				while (variableLength > 0)
				{
					int varFieldLL = readInt();
					int varFieldCP = readShort();
					int varFieldCCSID = readShort(); // Always 65535?

					int varFieldNameLength = varFieldLL - 8;
					readFully(byteBuffer_, 0, varFieldNameLength);

					virtualRead += varFieldLL;
					string varFieldName = Conv.ebcdicByteArrayToString(byteBuffer_, 0, varFieldNameLength, charBuffer_);
					switch (varFieldCP)
					{
						case 0x3840:
							describeCallback.fieldName(i, varFieldName);
							break;
						case 0x3841:
							describeCallback.udtName(i, varFieldName);
							break;
					}
					varLengthRead += varFieldLL;
					// adjusted above by readFully 
					// numRead += varFieldLL;
					variableLength -= varFieldLL;

				}
			  }
			  int remaining = ll - virtualRead + oldNumRead;
			  skipBytes(remaining);
			}
			else
			{
			  skipBytes(ll - 6);
			}
		  }
		  else if (cp == 0x380E && fetchCallback != null)
		  {
			int oldNumRead = (int) in_.LatestBytesReceived - 6;
			int virtualRead = oldNumRead + 6;
			if (virtualRead + 20 <= length)
			{
			  // Extended result data.
			  int consistencyToken = readInt();
			  int rowCount = readInt();
			  int columnCount = readShort();
			  int indicatorSize = readShort();
			  readInt(); // reserved
			  int rowSize = readInt();

			  fetchCallback.newResultData(rowCount, columnCount, rowSize);
			  virtualRead += 20;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempIndicator;
			  sbyte[] tempIndicator;
			  if (indicatorSize == 0)
			  {
				 tempIndicator_[0] = 0;
				 tempIndicator_[1] = 0;
				 tempIndicator = tempIndicator_;
			  }
			  else
			  {
				tempIndicator = indicatorSize == 2 ? tempIndicator_ : new sbyte[indicatorSize];
			  }
			  for (int i = 0; i < rowCount; ++i)
			  {
				for (int j = 0; j < columnCount; ++j)
				{
				  if (indicatorSize > 0)
				  {
					  readFullyReturnCount(tempIndicator);
					  virtualRead += indicatorSize;
				  }
				  fetchCallback.newIndicator(i, j, tempIndicator);
				}
			  }

			  sbyte[] callbackBuffer = fetchCallback.getTempDataBuffer(rowSize);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempData = callbackBuffer != null && callbackBuffer.length >= rowSize ? callbackBuffer : new byte[rowSize];
			  sbyte[] tempData = callbackBuffer != null && callbackBuffer.Length >= rowSize ? callbackBuffer : new sbyte[rowSize];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = ll+oldNumRead;
			  int max = ll + oldNumRead;
			  for (int i = 0; i < rowCount && virtualRead < max; ++i)
			  {
				// Todo.. Think about the numRead calculation as well as the
				// skip bytes calculation below
				readFully(tempData, 0, rowSize);
				fetchCallback.newRowData(i, tempData);
				virtualRead += rowSize;
			  }
			}
			int remaining = ll - (virtualRead - oldNumRead);
			skipBytes(remaining);
		  }
		  else if (cp == 0x380B && packageCallback != null)
		  {
			int oldNumRead = (int) in_.LatestBytesReceived - 6;
			int virtualRead = oldNumRead;
			virtualRead += 6;
			int packageLength = readInt();
			int packageCCSID = readShort();
			sbyte[] buf18 = new sbyte[18];
			readFullyReturnCount(buf18);
			string packageDefaultCollection = Conv.ebcdicByteArrayToString(buf18, charBuffer_);
			int numStatements = readShort();
			int pieceBytesRead = 26;
			virtualRead += 26;

			skipBytes(16); // Reserved.
			virtualRead += 16;
		packageCallback.newPackageInfo(packageCCSID, packageDefaultCollection, numStatements);

			int packageOffset = 48;
			int[] textOffsets = new int[numStatements];
			int[] textLengths = new int[numStatements];
			int[] formatOffsets = new int[numStatements];
			int[] formatLengths = new int[numStatements];
			int[] parameterMarkerOffsets = new int[numStatements];
			int[] parameterMarkerLengths = new int[numStatements];
			for (int i = 0; i < numStatements; ++i)
			{
			  int statementNeedsDefaultCollection = readByte();
			  int statementType = readShort();
			  readFullyReturnCount(buf18);
			  string statementName = Conv.ebcdicByteArrayToString(buf18, charBuffer_);
		  packageCallback.newStatementInfo(i, statementNeedsDefaultCollection, statementType, statementName);
			  skipBytes(19); // Reserved.
			  virtualRead += 21 + 19;
			  int formatOffset = readInt();
			  int formatLength = readInt();
			  formatOffsets[i] = formatOffset;
			  formatLengths[i] = formatLength;
			  int textOffset = readInt();
			  int textLength = readInt();
			  textOffsets[i] = textOffset;
			  textLengths[i] = textLength;
			  int parameterMarkerOffset = readInt();
			  int parameterMarkerLength = readInt();
			  parameterMarkerOffsets[i] = parameterMarkerOffset;
			  parameterMarkerLengths[i] = parameterMarkerLength;
			  virtualRead += 24;
		  packageOffset += 64;
			}
			for (int i = 0; i < numStatements; ++i)
			{
			  if (textLengths[i] > 0)
			  {
				int diff = textOffsets[i] - packageOffset;
				skipBytes(diff);
				virtualRead += diff;
			packageOffset += diff;
				sbyte[] buf = new sbyte[textLengths[i]];
				readFullyReturnCount(buf);
				virtualRead += textLengths[i];
			packageOffset += textLengths[i];
				string text = Conv.unicodeByteArrayToString(buf, 0, buf.Length);
			packageCallback.statementText(i, text);
			  }
			  if (formatLengths[i] > 0)
			  {
				int diff = formatOffsets[i] - packageOffset;
				skipBytes(diff);
				virtualRead += diff;
			packageOffset += diff;
				sbyte[] statementFormat = new sbyte[formatLengths[i]];
				readFullyReturnCount(statementFormat);
			packageOffset += formatLengths[i];
				virtualRead += formatLengths[i];
			packageCallback.statementDataFormat(i, statementFormat);
			  }
			  if (parameterMarkerLengths[i] > 0)
			  {
				int diff = parameterMarkerOffsets[i] - packageOffset;
				skipBytes(diff);
				virtualRead += diff;
			packageOffset += diff;
				sbyte[] parameterMarkerFormat = new sbyte[parameterMarkerLengths[i]];
				readFullyReturnCount(parameterMarkerFormat);
				virtualRead += parameterMarkerLengths[i];
			packageOffset += parameterMarkerLengths[i];
			packageCallback.statementParameterMarkerFormat(i, parameterMarkerFormat);
			  }
			}
			int remaining = ll - virtualRead + oldNumRead;
			skipBytes(remaining);
		  }
		  else if (cp == 0x3807 && sqlcaCallback_ != null)
		  {
			// SQL CA.
			readFully(byteBuffer_, 0, ll - 6);
			parseSQLCA();
		  }
		  else if (cp == 0x3813 && pmCallback != null)
		  {
			// Super extended parameter marker format.
			int oldNumRead = (int) in_.LatestBytesReceived - 6;
			// The header for the 3813 is a size of virtual bytes, but may not 
			// match the physical bytes read.  Track the physical and virtual bytes read
			// separately. 
			int virtualRead = oldNumRead;
			virtualRead += 6;

			if ((oldNumRead + 6) + 16 <= length && ll > 6)
			{

			  int consistencyToken = readInt();
			  int numFields = readInt();
			  readInt(); // reserved
			  int recordSize = readInt();
			  pmCallback.parameterMarkerDescription(numFields, recordSize);
			  virtualRead += 16;

			  int[] offsets = new int[numFields];
			  int[] lengths = new int[numFields];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fixedLengthRead = 48*numFields;
			  int fixedLengthRead = 48 * numFields;
			  for (int i = 0; i < numFields; ++i)
			  {

				int descLL = readShort();
				int type = readShort();
				int fieldLength = readInt();
				int scale = readShort(); // Scale for numeric, character count for GRAPHIC.
				int precision = readShort();
				int fieldCCSID = readShort();
				int parmType = readByte();
				int joinRefPosition = readShort();
				//Error in LIPI doc.          skipBytes(2); // Reserved.
				int lobLocator = readInt();
				// numRead+= skipBytesReturnCount(5); // Reserved.
				readInt(); // Reserved
				readByte(); // Reserved
				int lobMaxSize = readInt();
				// numRead+= skipBytesReturnCount(2); // Reserved.
				readShort(); // Reserved
				int offsetToVariableLengthInformation = readInt(); // Based on start of fixed length info.
				int lengthOfVariableLengthInformation = readInt();
				// numRead +=  skipBytesReturnCount(8); // Reserved.
				readInt(); // Reserved
				readInt(); // Reserved
				virtualRead += 48;

				pmCallback.parameterMarkerFieldDescription(i, type, fieldLength, scale, precision, fieldCCSID, parmType, joinRefPosition, lobLocator, lobMaxSize);

				offsets[i] = (48 * i) + offsetToVariableLengthInformation - fixedLengthRead;
				lengths[i] = lengthOfVariableLengthInformation;

			  }

			  int varLengthRead = 0;
			  for (int i = 0; i < numFields; ++i)
			  {
				int toSkip = offsets[i] - varLengthRead;
				skipBytes(toSkip);
				virtualRead += toSkip;

				int varFieldLL = readInt();
				int varFieldCP = readShort();
				int varFieldCCSID = readShort(); // Always 65535?

				int varFieldNameLength = lengths[i] - 8;
				readFully(byteBuffer_, 0, varFieldNameLength);
				string varFieldName = Conv.ebcdicByteArrayToString(byteBuffer_, 0, varFieldNameLength, charBuffer_);
				if (varFieldCP == 0x3840)
				{
				  pmCallback.parameterMarkerFieldName(i, varFieldName);
				}
				else if (varFieldCP == 0x3841)
				{
				  pmCallback.parameterMarkerUDTName(i, varFieldName);
				}
				varLengthRead += lengths[i];
				// 
				// Actual bytes read is calculated with readFully
				// numRead += lengths[i];
				// 
				virtualRead += lengths[i];
			  }
			}
			else
			{
			  // Statement was prepared but has no parameter markers.
			  pmCallback.parameterMarkerDescription(0, 0);
			}
			int remaining = ll - virtualRead + oldNumRead;
			skipBytes(remaining);
		  }
		  else if (cp == 0x3810 && lobCallback != null) // LOB data length
		  {
			int num = readShort();
			int moreSkip = 0;
			if (num == 0)
			{
			  lobCallback.newLOBLength(0);
			  moreSkip = 2;
			}
			else if (num == 4)
			{
			  int loblen = readInt();
			  lobCallback.newLOBLength(loblen);
			  moreSkip = 6;
			}
			else
			{
			  readShort();
			  int upperLen = readInt();
			  int lowerLen = readInt();
			  long totalLen = ((long)upperLen << 32) | (long)lowerLen;
			  lobCallback.newLOBLength(totalLen);
			  moreSkip = 12;
			}
			int physicalRead = moreSkip;
			skipBytes(ll - 6 - moreSkip);
		  }
		  else if (cp == 0x380F && lobCallback != null) // LOB data
		  {
			int oldNumRead = (int) in_.LatestBytesReceived - 6;
			int virtualRead = oldNumRead + 6;
			if (in_.LatestBytesReceived + 6 <= length && ll > 6)
			{
			  int ccsid = readShort();
			  int lobLL = readInt();
			  lobCallback.newLOBData(ccsid, lobLL);
			  sbyte[] buffer = lobCallback.LOBBuffer;
			  if (buffer == null || buffer.Length == 0)
			  {
				int max = 0x00FFFF > lobLL ? lobLL : 0x00FFFF; // 64 kB
				buffer = new sbyte[max];
				lobCallback.LOBBuffer = buffer;
			  }
			  int remainingLob = lobLL;
			  int segmentLimit = buffer.Length > remainingLob ? remainingLob : buffer.Length;
			  readFully(buffer, 0, segmentLimit);
			  virtualRead += segmentLimit;

			  lobCallback.newLOBSegment(buffer, 0, segmentLimit);
			  remainingLob -= segmentLimit;
			  while (remainingLob > 0)
			  {
				segmentLimit = buffer.Length > remainingLob ? remainingLob : buffer.Length;
				readFully(buffer, 0, segmentLimit);
				virtualRead += segmentLimit;

				lobCallback.newLOBSegment(buffer, 0, segmentLimit);
				remainingLob -= segmentLimit;
			  }
			}
			int remaining = ll - virtualRead + oldNumRead;
			skipBytes(remaining);
		  }
		  else
		  {
			skipBytes(ll - 6);
		  }
		}
		in_.end();
	  }

	  private void parseSQLCA()
	  {
		int sqlCode = Conv.byteArrayToInt(byteBuffer_, 12);
		int updateCount = Conv.byteArrayToInt(byteBuffer_, 104);
		string sqlState = Conv.ebcdicByteArrayToString(byteBuffer_, 131, 5, charBuffer_);
		string generatedKey = Conv.packedDecimalToString(byteBuffer_, 72, 30, 0, charBuffer_);
		int resultSetsCount = Conv.byteArrayToInt(byteBuffer_, 100);
		sqlcaCallback_.newSQLCommunicationsAreaData(sqlCode, sqlState, generatedKey, updateCount, resultSetsCount);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fetch(DatabaseFetchAttributes attribs, DatabaseFetchCallback listener) throws IOException
	  public virtual void fetch(DatabaseFetchAttributes attribs, DatabaseFetchCallback listener)
	  {
		sendFetchRequest(attribs);
		out_.flush();

		parseReply("fetch", null, listener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void streamFetch(DatabaseStreamFetchAttributes attribs, DatabaseFetchCallback listener) throws IOException
	  public virtual void streamFetch(DatabaseStreamFetchAttributes attribs, DatabaseFetchCallback listener)
	  {
		sendStreamFetchRequest(attribs);
		out_.flush();

		parseReply("streamFetch", null, listener);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endStreamFetch(DatabaseEndStreamFetchAttributes attribs) throws IOException
	  public virtual void endStreamFetch(DatabaseEndStreamFetchAttributes attribs)
	  {
		sendEndStreamFetchRequest(attribs);
		out_.flush();

		readFullReply("endStreamFetch");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeImmediate(DatabaseExecuteImmediateAttributes attribs) throws IOException
	  public virtual void executeImmediate(DatabaseExecuteImmediateAttributes attribs)
	  {
		sendExecuteImmediateRequest(attribs);
		out_.flush();

		if (sqlcaCallback_ != null)
		{
		  parseReply("executeImmediate", null, null, null, null, null); // In case someone wants generated keys.
		}
		else
		{
		  readFullReply("executeImmediate");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(DatabaseExecuteAttributes attribs) throws IOException
	  public virtual void execute(DatabaseExecuteAttributes attribs)
	  {
		execute(attribs, 0);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(DatabaseExecuteAttributes attribs, int parameterMarkerDescriptorHandle) throws IOException
	  public virtual void execute(DatabaseExecuteAttributes attribs, int parameterMarkerDescriptorHandle)
	  {
		sendExecuteRequest(attribs, parameterMarkerDescriptorHandle);
		out_.flush();

		if (sqlcaCallback_ != null)
		{
		  parseReply("execute", null, null, null, null, null); // In case someone wants generated keys.
		}
		else
		{
		  readFullReply("execute");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeOrOpenAndDescribe(DatabaseExecuteOrOpenAndDescribeAttributes attrib, DatabaseDescribeCallback listener) throws IOException
	  public virtual void executeOrOpenAndDescribe(DatabaseExecuteOrOpenAndDescribeAttributes attrib, DatabaseDescribeCallback listener)
	  {
		sendExecuteOrOpenAndDescribeRequest(attrib);
		out_.flush();

		parseReply("executeOrOpenAndDescribe", listener, null);
	  }

	  private int getScrollableCursorFlagLength(AttributeScrollableCursorFlag a)
	  {
		return 8;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeScrollableCursorFlag(AttributeScrollableCursorFlag a) throws IOException
	  private void writeScrollableCursorFlag(AttributeScrollableCursorFlag a)
	  {
		out_.writeInt(8);
		out_.writeShort(0x380D);
		out_.writeShort(a.ScrollableCursorFlag);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendExecuteImmediateRequest(DatabaseExecuteImmediateAttributes attribs) throws IOException
	  private void sendExecuteImmediateRequest(DatabaseExecuteImmediateAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.SQLStatementTextSet)
		  {
			++parms;
			length += getSQLStatementTextLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			++parms;
			length += getSQLStatementTypeLength(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			++parms;
			length += getOpenAttributesLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			++parms;
			length += getExtendedSQLStatementTextLength(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			++parms;
			length += getPrepareOptionLength(attribs);
		  }
		}

		writeHeader(length, 0x1806);
		writeTemplate(parms);

		if (attribs != null)
		{
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			writePrepareOption(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendExecuteRequest(DatabaseExecuteAttributes attribs, int pmDescriptorHandle) throws IOException
	  private void sendExecuteRequest(DatabaseExecuteAttributes attribs, int pmDescriptorHandle)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			++parms;
			length += getSQLExtendedParameterMarkerDataLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }


		  if (attribs.SQLStatementTypeSet)
		  {
			++parms;
			length += getSQLStatementTypeLength(attribs);
		  }

		}

		writeHeader(length, 0x1805);
		// writeTemplate(parms, 0xF2000000, pmDescriptorHandle);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.SQLCA;
		writeTemplate(parms, template, pmDescriptorHandle);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.SQLParameterMarkerDataSet)
		  {
			writeSQLParameterMarkerData(attribs);
		  }
		  if (attribs.SQLExtendedParameterMarkerDataSet)
		  {
			writeSQLExtendedParameterMarkerData(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }

		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }

		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeCursor(DatabaseCloseCursorAttributes attribs) throws IOException
	  public virtual void closeCursor(DatabaseCloseCursorAttributes attribs)
	  {
		sendCloseCursorRequest(attribs, true);
		out_.flush();

		readFullReply("closeCursor");
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void commit() throws IOException
	  public virtual void commit()
	  {
		  sendCommitRequest(out_);
		  out_.flush();
		  readFullReply("commit");
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void rollback() throws IOException
	  public virtual void rollback()
	  {
		  sendRollbackRequest(out_);
		  out_.flush();
		  readFullReply("rollback");
	  }



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setServerAttributes(DatabaseServerAttributes attributes) throws IOException
	  public virtual DatabaseServerAttributes ServerAttributes
	  {
		  set
		  {
			sendSetServerAttributesRequest(out_, value);
			out_.flush();
    
			int length = readReplyHeader("setServerAttributes");
			int virtualRead = 40;
			if (length > 46)
			{
			  int ll = readInt();
			  int cp = readShort();
			  if (cp != 0x3804)
			  {
				throw DataStreamException.badReply("setServerAttributes-reply", cp);
			  }
			  if (ll < 122)
			  {
				throw DataStreamException.badLength("setServerAttributes-reply", ll);
			  }
			  int ccsid = readShort();
			  int dateFormatParserOption = readShort();
			  value.DateFormatParserOption = dateFormatParserOption;
			  int dateSeparatorParserOption = readShort();
			  value.DateSeparatorParserOption = dateSeparatorParserOption;
			  int timeFormatParserOption = readShort();
			  value.TimeFormatParserOption = timeFormatParserOption;
			  int timeSeparatorParserOption = readShort();
			  value.TimeSeparatorParserOption = timeSeparatorParserOption;
			  int decimalSeparatorParserOption = readShort();
			  value.DecimalSeparatorParserOption = decimalSeparatorParserOption;
			  int namingConventionParserOption = readShort();
			  value.NamingConventionParserOption = namingConventionParserOption;
			  int ignoreDecimalDataErrorParserOption = readShort();
			  value.IgnoreDecimalDataErrorParserOption = ignoreDecimalDataErrorParserOption;
			  int commitmentControlLevelParserOption = readShort();
			  value.CommitmentControlLevelParserOption = commitmentControlLevelParserOption;
			  int drdaPackageSize = readShort();
			  value.DRDAPackageSize = drdaPackageSize;
			  int translationIndicator = readByte();
			  value.TranslateIndicator = translationIndicator;
			  int serverCCSIDValue = readShort();
			  value.ServerCCSID = serverCCSIDValue;
			  int serverNLSSValue = readShort();
			  value.NLSSIdentifier = serverNLSSValue;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final byte[] buf = new byte[32];
			  sbyte[] buf = new sbyte[32];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final char[] cbuf = new char[32];
			  char[] cbuf = new char[32];
			  readFully(buf, 0, 3);
			  string serverLanguageID = Conv.ebcdicByteArrayToString(buf, 0, 3, cbuf);
			  value.NLSSIdentifierLanguageID = serverLanguageID;
			  readFully(buf, 0, 10);
			  string serverLanguageTableName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
			  value.NLSSIdentifierLanguageTableName = serverLanguageTableName;
			  readFully(buf, 0, 10);
			  string serverLanguageTableLibrary = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
			  value.NLSSIdentifierLanguageTableLibrary = serverLanguageTableLibrary;
			  readFully(buf, 0, 4);
			  string serverLanguageFeatureCode = Conv.ebcdicByteArrayToString(buf, 0, 4, cbuf);
			  value.LanguageFeatureCode = serverLanguageFeatureCode;
			  readFully(buf, 0, 10);
			  string serverFunctionalLevel = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
			  value.ServerFunctionalLevel = serverFunctionalLevel;
			  readFully(buf, 0, 18);
			  string relationalDBName = Conv.ebcdicByteArrayToString(buf, 0, 18, cbuf);
			  value.RDBName = relationalDBName;
			  readFully(buf, 0, 10);
			  string defaultSQLLibraryName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
			  value.DefaultSQLLibraryName = defaultSQLLibraryName;
			  readFully(buf, 0, 26);
			  string jobName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
			  string userName = Conv.ebcdicByteArrayToString(buf, 10, 10, cbuf);
			  string jobNumber = Conv.ebcdicByteArrayToString(buf, 20, 6, cbuf);
			  value.setServerJob(jobName, userName, jobNumber);
			  skipBytes(ll - 122);
			  virtualRead += ll;
			}
			skipBytes(length - virtualRead);
			in_.end();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void retrieveLOBData(DatabaseRetrieveLOBDataAttributes attribs, DatabaseLOBDataCallback lobCallback) throws IOException
	  public virtual void retrieveLOBData(DatabaseRetrieveLOBDataAttributes attribs, DatabaseLOBDataCallback lobCallback)
	  {
		sendRetrieveLOBDataRequest(out_, attribs, currentRPB_);
		out_.flush();

		parseReply("retrieveLOBData", null, null, null, null, lobCallback);
	  }

	  // I don't think this is needed anymore.

	/*  private void sendDeleteResultSetRequest() throws IOException
	  {
	    writeHeader(40, 0x1F01);
	
	    // Write template.
	    out_.writeInt(0x00000000); // Operational result (ORS) bitmap.
	    out_.writeInt(0); // Reserved.
	    out_.writeShort(1); // Return ORS handle - after operation completes.
	    out_.writeShort(1); // Fill ORS handle.
	    out_.writeShort(0); // Based on ORS handle.
	    out_.writeShort(1); // Request parameter block (RPB) handle.
	    out_.writeShort(0); // Parameter marker descriptor handle.
	    out_.writeShort(0); // Parameter count.
	  }
	*/

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendDeleteSQLRPBRequest(DatabaseDeleteRequestParameterBlockAttributes attribs, final int rpbID) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendDeleteSQLRPBRequest(DatabaseDeleteRequestParameterBlockAttributes attribs, int rpbID)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x1D02);
		// writeTemplate(parms, 0x80000000, 0, rpbID);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED;
		writeTemplate(parms, template, 0, rpbID);


		// Write template.
	/*    out_.writeInt(0x80000000); // Operational result (ORS) bitmap.
	    out_.writeInt(0); // Reserved.
	    out_.writeShort(1); // Return ORS handle - after operation completes.
	    out_.writeShort(1); // Fill ORS handle.
	    out_.writeShort(0); // Based on ORS handle.
	    out_.writeShort(rpbID); // Request parameter block (RPB) handle.
	    out_.writeShort(0); // Parameter marker descriptor handle.
	    out_.writeShort(parms); // Parameter count.
	*/
		if (attribs != null)
		{
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

	  private int getCursorNameLength(AttributeCursorName a)
	  {
		return 10 + a.CursorName.Length;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeCursorName(AttributeCursorName a) throws IOException
	  private void writeCursorName(AttributeCursorName a)
	  {
		string name = a.CursorName;
		out_.writeInt(10 + name.Length);
		out_.writeShort(0x380B);
		out_.writeShort(37);
		out_.writeShort(name.Length);
		writePadEBCDIC(name, name.Length, out_);
	  }

	  private int getReuseIndicatorLength(AttributeReuseIndicator a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeReuseIndicator(AttributeReuseIndicator a) throws IOException
	  private void writeReuseIndicator(AttributeReuseIndicator a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3810);
		out_.write(a.ReuseIndicator);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendCloseCursorRequest(DatabaseCloseCursorAttributes attribs, final boolean doReply) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendCloseCursorRequest(DatabaseCloseCursorAttributes attribs, bool doReply)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.CursorNameSet)
		  {
			length += getCursorNameLength(attribs);
			++parms;
		  }
		  if (attribs.ReuseIndicatorSet)
		  {
			length += getReuseIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		}

		writeHeader(length, 0x180A);

		// Write template.
		out_.writeInt(doReply ? unchecked((int)0x80000000) : 0x00000000); // Operational result (ORS) bitmap.
		out_.writeInt(0); // Reserved.
		out_.writeShort(1); // Return ORS handle - after operation completes.
		out_.writeShort(1); // Fill ORS handle.
		out_.writeShort(0); // Based on ORS handle.
		out_.writeShort(currentRPB_); // Request parameter block (RPB) handle.
		out_.writeShort(0); // Parameter marker descriptor handle.
		out_.writeShort(parms); // Parameter count.

		if (attribs != null)
		{
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.ReuseIndicatorSet)
		  {
			writeReuseIndicator(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		}
	  }

	  private int getDescribeOptionLength(AttributeDescribeOption a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeDescribeOption(AttributeDescribeOption a) throws IOException
	  private void writeDescribeOption(AttributeDescribeOption a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x380A);
		out_.writeByte(a.DescribeOption);
	  }

	  private int getBlockingFactorLength(AttributeBlockingFactor a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeBlockingFactor(AttributeBlockingFactor a) throws IOException
	  private void writeBlockingFactor(AttributeBlockingFactor a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x380C);
		out_.writeInt((int)(a.BlockingFactor));
	  }

	  private int getFetchScrollOptionLength(AttributeFetchScrollOption a)
	  {
		int option = a.FetchScrollOption;
		return (option == 0x0007 || option == 0x0008) ? 12 : 8;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeFetchScrollOption(AttributeFetchScrollOption a) throws IOException
	  private void writeFetchScrollOption(AttributeFetchScrollOption a)
	  {
		int option = a.FetchScrollOption;
		bool relative = option == 0x0007 || option == 0x0008;
		out_.writeInt(relative ? 12 : 8);
		out_.writeShort(0x380E);
		out_.writeShort(option);
		if (relative)
		{
			out_.writeInt(a.FetchScrollOptionRelativeValue);
		}
	  }

	  private int getFetchBufferSizeLength(AttributeFetchBufferSize a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeFetchBufferSize(AttributeFetchBufferSize a) throws IOException
	  private void writeFetchBufferSize(AttributeFetchBufferSize a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3834);
		out_.writeInt((int)a.FetchBufferSize);
	  }

	  private int getHoldIndicatorLength(AttributeHoldIndicator a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHoldIndicator(AttributeHoldIndicator a) throws IOException
	  private void writeHoldIndicator(AttributeHoldIndicator a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x380F);
		out_.writeByte(a.HoldIndicator);
	  }

	  private int getQueryTimeoutLimitLength(AttributeQueryTimeoutLimit a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeQueryTimeoutLimit(AttributeQueryTimeoutLimit a) throws IOException
	  private void writeQueryTimeoutLimit(AttributeQueryTimeoutLimit a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3817);
		out_.writeInt(a.QueryTimeoutLimit);
	  }

	  private int getServerSideStaticCursorResultSetSizeLength(AttributeServerSideStaticCursorResultSetSize a)
	  {
		return 10;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeServerSideStaticCursorResultSetSize(AttributeServerSideStaticCursorResultSetSize a) throws IOException
	  private void writeServerSideStaticCursorResultSetSize(AttributeServerSideStaticCursorResultSetSize a)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3827);
		out_.writeInt(a.ServerSideStaticCursorResultSetSize);
	  }

	  private int getResultSetHoldabilityOptionLength(AttributeResultSetHoldabilityOption a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeResultSetHoldabilityOption(AttributeResultSetHoldabilityOption a) throws IOException
	  private void writeResultSetHoldabilityOption(AttributeResultSetHoldabilityOption a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3830);
		out_.writeByte(a.ResultSetHoldabilityOption);
	  }

	  private int getVariableFieldCompressionLength(AttributeVariableFieldCompression a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeVariableFieldCompression(AttributeVariableFieldCompression a) throws IOException
	  private void writeVariableFieldCompression(AttributeVariableFieldCompression a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3833);
		out_.writeByte(a.VariableFieldCompression);
	  }

	  private int getReturnOptimisticLockingColumnsLength(AttributeReturnOptimisticLockingColumns a)
	  {
		return 7;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeReturnOptimisticLockingColumns(AttributeReturnOptimisticLockingColumns a) throws IOException
	  private void writeReturnOptimisticLockingColumns(AttributeReturnOptimisticLockingColumns a)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3835);
		out_.writeByte(a.ReturnOptimisticLockingColumns);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendCreateSQLRPBRequest(final DatabaseCreateRequestParameterBlockAttributes attribs, final boolean doReply, final int rpbID) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendCreateSQLRPBRequest(DatabaseCreateRequestParameterBlockAttributes attribs, bool doReply, int rpbID)
	  {
		sendSQLRPBRequest(attribs, doReply, rpbID, 0x1D00);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendResetSQLRPBRequest(final DatabaseCreateRequestParameterBlockAttributes attribs, final boolean doReply, final int rpbID) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendResetSQLRPBRequest(DatabaseCreateRequestParameterBlockAttributes attribs, bool doReply, int rpbID)
	  {
		sendSQLRPBRequest(attribs, doReply, rpbID, 0x1D04);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendSQLRPBRequest(final DatabaseCreateRequestParameterBlockAttributes attribs, final boolean doReply, final int rpbID, final int datastream) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private void sendSQLRPBRequest(DatabaseCreateRequestParameterBlockAttributes attribs, bool doReply, int rpbID, int datastream)
	  {
		int length = 40;
		int parms = 0;
		if (attribs != null)
		{
		  if (attribs.PackageLibrarySet)
		  {
			++parms;
			length += getPackageLibraryLength(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			++parms;
			length += getPackageNameLength(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			++parms;
			length += getTranslateIndicatorLength(attribs);
		  }
		  if (attribs.PrepareStatementNameSet)
		  {
			++parms;
			length += getPrepareStatementNameLength(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			++parms;
			length += getSQLStatementTextLength(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			++parms;
			length += getPrepareOptionLength(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			++parms;
			length += getOpenAttributesLength(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			++parms;
			length += getDescribeOptionLength(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			++parms;
			length += getCursorNameLength(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			++parms;
			length += getVariableFieldCompressionLength(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			++parms;
			length += getBlockingFactorLength(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			++parms;
			length += getScrollableCursorFlagLength(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			++parms;
			length += getFetchScrollOptionLength(attribs);
		  }
		  if (attribs.HoldIndicatorSet)
		  {
			++parms;
			length += getHoldIndicatorLength(attribs);
		  }
		  if (attribs.ReuseIndicatorSet)
		  {
			++parms;
			length += getReuseIndicatorLength(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			++parms;
			length += getSQLStatementTypeLength(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			++parms;
			length += getSQLParameterMarkerBlockIndicatorLength(attribs);
		  }
		  if (attribs.QueryTimeoutLimitSet)
		  {
			++parms;
			length += getQueryTimeoutLimitLength(attribs);
		  }
		  if (attribs.ServerSideStaticCursorResultSetSizeSet)
		  {
			++parms;
			length += getServerSideStaticCursorResultSetSizeLength(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			++parms;
			length += getRLECompressedFunctionParametersLength(attribs);
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			++parms;
			length += getExtendedColumnDescriptorOptionLength(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			++parms;
			length += getResultSetHoldabilityOptionLength(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			++parms;
			length += getExtendedSQLStatementTextLength(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			++parms;
			length += getFetchBufferSizeLength(attribs);
		  }
		  if (attribs.ReturnOptimisticLockingColumnsSet)
		  {
			++parms;
			length += getReturnOptimisticLockingColumnsLength(attribs);
		  }
		}

		writeHeader(length, datastream);
		writeTemplate(parms, doReply ? OperationalResultBitmap_Fields.SEND_REPLY_IMMED : 0x00000000, 0, rpbID);

		if (attribs != null)
		{
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			writePrepareOption(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.CursorNameSet)
		  {
			writeCursorName(attribs);
		  }
		  if (attribs.VariableFieldCompressionSet)
		  {
			writeVariableFieldCompression(attribs);
		  }
		  if (attribs.BlockingFactorSet)
		  {
			writeBlockingFactor(attribs);
		  }
		  if (attribs.ScrollableCursorFlagSet)
		  {
			writeScrollableCursorFlag(attribs);
		  }
		  if (attribs.FetchScrollOptionSet)
		  {
			writeFetchScrollOption(attribs);
		  }
		  if (attribs.HoldIndicatorSet)
		  {
			writeHoldIndicator(attribs);
		  }
		  if (attribs.ReuseIndicatorSet)
		  {
			writeReuseIndicator(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.SQLParameterMarkerBlockIndicatorSet)
		  {
			writeSQLParameterMarkerBlockIndicator(attribs);
		  }
		  if (attribs.QueryTimeoutLimitSet)
		  {
			writeQueryTimeoutLimit(attribs);
		  }
		  if (attribs.ServerSideStaticCursorResultSetSizeSet)
		  {
			writeServerSideStaticCursorResultSetSize(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			writeExtendedColumnDescriptorOption(attribs);
		  }
		  if (attribs.ResultSetHoldabilityOptionSet)
		  {
			writeResultSetHoldabilityOption(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		  if (attribs.FetchBufferSizeSet)
		  {
			writeFetchBufferSize(attribs);
		  }
		  if (attribs.ReturnOptimisticLockingColumnsSet)
		  {
			writeReturnOptimisticLockingColumns(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendPrepareAndDescribeRequest(DatabasePrepareAndDescribeAttributes attribs) throws IOException
	  private void sendPrepareAndDescribeRequest(DatabasePrepareAndDescribeAttributes attribs)
	  {
		int length = 40;
		int parms = 0;
		bool hasParameterMarkers = true; // To be on the safe side, default to requesting parameter marker info.
		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			length += getPrepareStatementNameLength(attribs);
			++parms;
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			length += getSQLStatementTextLength(attribs);
			++parms;
			if (attribs.SQLStatementText.IndexOf("?", StringComparison.Ordinal) < 0)
			{
				hasParameterMarkers = false;
			}
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			length += getSQLStatementTypeLength(attribs);
			++parms;
		  }
		  if (attribs.PrepareOptionSet)
		  {
			length += getPrepareOptionLength(attribs);
			++parms;
		  }
		  if (attribs.DescribeOptionSet)
		  {
			length += getDescribeOptionLength(attribs);
			++parms;
		  }
		  if (attribs.OpenAttributesSet)
		  {
			length += getOpenAttributesLength(attribs);
			++parms;
		  }
		  if (attribs.PackageNameSet)
		  {
			length += getPackageNameLength(attribs);
			++parms;
		  }
		  if (attribs.PackageLibrarySet)
		  {
			length += getPackageLibraryLength(attribs);
			++parms;
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			length += getTranslateIndicatorLength(attribs);
			++parms;
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			length += getRLECompressedFunctionParametersLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			length += getExtendedColumnDescriptorOptionLength(attribs);
			++parms;
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			length += getExtendedSQLStatementTextLength(attribs);
			++parms;
			if (attribs.ExtendedSQLStatementText.IndexOf("?", StringComparison.Ordinal) < 0)
			{
				hasParameterMarkers = false;
			}
		  }
		}

		writeHeader(length, 0x1803);
	//    writeTemplate(parms, 0xF8020000);
	//    writeTemplate(parms, 0x8A840000);
		//Writing BITMAP --
		// ORS Bitmap:                    Bitmap: 0xFA820000
		// Bit  1: Send Results Immediately
		// Bit  2: Send Message ID
		// Bit  3: Send First Level Text
		// Bit  4: Send Second Level Text
		// Bit  5: Send Data Format
		// Bit  7: Send SQLCA
		// Bit  9: Send Parameter Marker Format
		// Bit 15: Send Extended Column Descriptors
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.DATA_FORMAT; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT | DATA_FORMAT | PARAMETER_MARKER_FORMAT | EXTENDED_COLUMN_DESCRIPTORS;


		if (attribs != null && attribs.SQLStatementTypeSet && attribs.SQLStatementType == TYPE_CALL)
		{
			// Do not request column descriptors for call statement
			// writeTemplate(parms, 0xF8800000);
		}
		else
		{
			// writeTemplate(parms, 0xF8820000); 
			template |= OperationalResultBitmap_Fields.EXTENDED_COLUMN_DESCRIPTORS;
		}
		if (hasParameterMarkers)
		{
			template |= OperationalResultBitmap_Fields.PARAMETER_MARKER_FORMAT;
		}
		writeTemplate(parms, template);

		if (attribs != null)
		{
		  if (attribs.PrepareStatementNameSet)
		  {
			writePrepareStatementName(attribs);
		  }
		  if (attribs.SQLStatementTextSet)
		  {
			writeSQLStatementText(attribs);
		  }
		  if (attribs.SQLStatementTypeSet)
		  {
			writeSQLStatementType(attribs);
		  }
		  if (attribs.PrepareOptionSet)
		  {
			writePrepareOption(attribs);
		  }
		  if (attribs.DescribeOptionSet)
		  {
			writeDescribeOption(attribs);
		  }
		  if (attribs.OpenAttributesSet)
		  {
			writeOpenAttributes(attribs);
		  }
		  if (attribs.PackageLibrarySet)
		  {
			writePackageLibrary(attribs);
		  }
		  if (attribs.PackageNameSet)
		  {
			writePackageName(attribs);
		  }
		  if (attribs.TranslateIndicatorSet)
		  {
			writeTranslateIndicator(attribs);
		  }
		  if (attribs.RLECompressedFunctionParametersSet)
		  {
			writeRLECompressedFunctionParameters(attribs);
		  }
		  if (attribs.ExtendedColumnDescriptorOptionSet)
		  {
			writeExtendedColumnDescriptorOption(attribs);
		  }
		  if (attribs.ExtendedSQLStatementTextSet)
		  {
			writeExtendedSQLStatementText(attribs);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readReplyHeader(String datastream) throws IOException
	  private int readReplyHeader(string datastream)
	  {
		// Read header.
		int length = in_.readInt();
		if (length < 40)
		{
		  throw DataStreamException.badLength(datastream, length);
		}
		int headerID = in_.readShort();
		int serverID = in_.readShort();
		int csInstance = in_.readInt();
		int correlationID = in_.readInt();
		int templateLength = in_.readShort();
		int reqRepID = in_.readShort();
		if (reqRepID != 0x2800)
		{
	//      in_.skipBytes(length-20);
		  in_.end();
		  throw DataStreamException.badReply(datastream, reqRepID);
		}

		// Read template.
		int orsBitmap = in_.readInt();
		int compressed = in_.readInt(); // First byte counts, last 3 reserved.
		int returnORSHandle = in_.readShort();
		int returnDataFunctionID = in_.readShort();
		int requestDataFunctionID = in_.readShort();
		int rcClass = in_.readShort();
		int rcClassReturnCode = in_.readInt();
		int numRead = 40;
		if (rcClass != 0)
		{
		  if (rcClassReturnCode >= 0)
		  {
			// Warning.
			if (warningCallback_ != null)
			{
			  warningCallback_.newWarning(rcClass, rcClassReturnCode);
			}
		  }
	//      if (rcClass == 2 && rcClassReturnCode == 0x02BD)
	//      {
	//        // Last record warning.
	//      }
		  else
		  {
			string msgID = null;
			string msgText = null;
			string msgText2 = null;
			while (numRead < length)
			{
			  int ll = in_.readInt();

			  int cp = in_.readShort();

			  if (cp == 0x3801)
			  {
				// Message ID.
				int ccsid = in_.readShort();
				sbyte[] messageID = new sbyte[ll - 8];
				in_.readFully(messageID);
				if (ll > charBuffer_.Length)
				{
					charBuffer_ = new char[ll];
				}
				msgID = Conv.ebcdicByteArrayToString(messageID, charBuffer_);
			  }
			  else if (cp == 0x3802)
			  {
				// First level message text.
				int ccsid = in_.readShort();
				int len = in_.readShort();
				sbyte[] firstLevelMessageText = new sbyte[ll - 10];
				in_.readFully(firstLevelMessageText);
				if (ll > charBuffer_.Length)
				{
				  charBuffer_ = new char[ll];
				}
				msgText = Conv.ebcdicByteArrayToString(firstLevelMessageText, charBuffer_);
			  }
			  else if (cp == 0x3803)
			  {
				// Second level message text.
				int ccsid = in_.readShort();
				int len = in_.readShort();
				sbyte[] secondLevelMessageText = new sbyte[ll - 10];
				in_.readFully(secondLevelMessageText);
				if (ll > charBuffer_.Length)
				{
				  charBuffer_ = new char[ll];
				}
				msgText2 = Conv.ebcdicByteArrayToString(secondLevelMessageText, charBuffer_);
			  }
			  else if (cp == 0x3807 && sqlcaCallback_ != null)
			  {
				// SQLCA - Communication Area.
				in_.readFully(byteBuffer_, 0, ll - 6);
				parseSQLCA();
			  }
			  else
			  {
				skipBytes(ll - 6);
			  }
			  numRead += ll;
			}
			skipBytes(length - numRead);
			in_.end();
			if (!string.ReferenceEquals(msgID, null))
			{
			  string text = string.ReferenceEquals(msgText, null) ? "" : msgText;
			  text = text + (string.ReferenceEquals(msgText2, null) ? "" : " " + msgText2);
			  throw new MessageException(new Message[] {new Message(msgID, text)});
			}
			throw new DatabaseException(datastream, rcClass, rcClassReturnCode);
		  }
		}
		else if (warningCallback_ != null)
		{
		  warningCallback_.noWarnings();
		}

		return length;
	  }

	/*    while (numRead < length)
	    {
	      int ll = in_.readInt();
	      int cp = in_.readShort();
	      System.out.println("Reply CP 0x"+Integer.toHexString(cp));
	      if (cp == 0x3801)
	      {
	        int ccsid = in_.readShort();
	        byte[] messageID = new byte[ll-8];
	        in_.readFully(messageID);
	      }
	      else if (cp == 0x3802)
	      {
	        int ccsid = in_.readShort();
	        byte[] firstLevelMessageText = new byte[ll-8];
	        in_.readFully(firstLevelMessageText);
	      }
	      else if (cp == 0x3803)
	      {
	        int ccsid = in_.readShort();
	        byte[] secondLevelMessageText = new byte[ll-8];
	        in_.readFully(secondLevelMessageText);
	      }
	      else if (cp == 0x3804)
	      {
	        int ccsid = in_.readShort();
	        System.out.println("CCSID: "+ccsid);
	        int dateFormatParserOption = in_.readShort();
	        int dateSeparatorParserOption = in_.readShort();
	        int timeFormatParserOption = in_.readShort();
	        int timeSeparatorParserOption = in_.readShort();
	        int decimalSeparatorParserOption = in_.readShort();
	        int namingConventionParserOption = in_.readShort();
	        int ignoreDecimalDataErrorParserOption = in_.readShort();
	        int commitmentControlLevelParserOption = in_.readShort();
	        int drdaPackageSize = in_.readShort();
	        int translationIndicator = in_.readByte();
	        int serverCCSIDValue = in_.readShort();
	        int serverNLSSValue = in_.readShort();
	        final byte[] buf = new byte[32];
	        final char[] cbuf = new char[32];
	        in_.readFully(buf, 0, 3);
	        String serverLanguageID = Conv.ebcdicByteArrayToString(buf, 0, 3, cbuf);
	        System.out.println(serverLanguageID);
	        in_.readFully(buf, 0, 10);
	        String serverLanguageTableName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
	        System.out.println(serverLanguageTableName);
	        in_.readFully(buf, 0, 10);
	        String serverLanguageTableLibrary = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
	        System.out.println(serverLanguageTableLibrary);
	        in_.readFully(buf, 0, 4);
	        String serverLanguageFeatureCode = Conv.ebcdicByteArrayToString(buf, 0, 4, cbuf);
	        System.out.println(serverLanguageFeatureCode);
	        in_.readFully(buf, 0, 10);
	        String serverFunctionalLevel = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
	        System.out.println(serverFunctionalLevel);
	        in_.readFully(buf, 0, 18);
	        String relationalDBName = Conv.ebcdicByteArrayToString(buf, 0, 18, cbuf);
	        System.out.println(relationalDBName);
	        in_.readFully(buf, 0, 10);
	        String defaultSQLLibraryName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
	        System.out.println(defaultSQLLibraryName);
	        in_.readFully(buf, 0, 26);
	        String jobName = Conv.ebcdicByteArrayToString(buf, 0, 10, cbuf);
	        String userName = Conv.ebcdicByteArrayToString(buf, 10, 10, cbuf);
	        String jobNumber = Conv.ebcdicByteArrayToString(buf, 20, 6, cbuf);
	        System.out.println(jobName+"/"+userName+"/"+jobNumber);
	        in_.skipBytes(ll-122);
	      }
	//      else if (cp == 0x3805)
	//      {
	//        // Data Format.
	//      }
	//      else if (cp == 0x3806)
	//      {
	//        // Result Data.
	//      }
	      else if (cp == 0x3807)
	      {
	        byte[] sqlCA = new byte[ll-6]; // SQL Communication Area
	        in_.readFully(sqlCA);
	      }
	//      else if (cp == 0x3808)
	//      {
	//        // Parameter Marker Format.
	//      }
	//      else if (cp == 0x3809)
	//      {
	//        // Translation Table Information.
	//      }
	//      else if (cp == 0x380A)
	//      {
	//        // Data Source Name (DSN) Attributes
	//      }
	//      else if (cp == 0x380B)
	//      {
	//        // Package Return Info.
	//      }
	//      else if (cp == 0x380C)
	//      {
	//        // Extended Data Format.
	//      }
	//      else if (cp == 0x380D)
	//      {
	//        // Extended Parameter Marker Format.
	//      }
	//      else if (cp == 0x380E)
	//      {
	//        // Extended Result Data.
	//      }
	      else if (cp == 0x380F)
	      {
	        // LOB Data.
	        if (ll > 6)
	        {
	          int ccsid = in_.readShort();
	          int secondLL = in_.readInt();
	          byte[] lobData = new byte[secondLL];
	          in_.readFully(lobData);
	        }
	      }
	//      else if (cp == 0x3810)
	//      {
	//        // Current LOB Length.
	//      }
	//      else if (cp == 0x3811)
	//      {
	//        // Extended Column Descriptors.
	//      }
	//      else if (cp == 0x3812)
	//      {
	//        // Super Extended Data Format.
	//      }
	//      else if (cp == 0x3813)
	//      {
	//        // Super Extended Parameter Marker Format.
	//      }
	      else if (cp == 0x3814)
	      {
	        // Cursor Attributes.
	        int cursorHoldability = in_.readByte();
	        int cursorScrollability = in_.readByte();
	        int cursorConcurrency = in_.readByte();
	        int cursorSensitivity = in_.readByte();
	        in_.skipBytes(ll-10);
	      }
	//      else if (cp == 0x3832)
	//      {
	//        // RLE Compressed function parameters.
	//      }
	      else
	      {
	        in_.skipBytes(ll-6);
	      }
	      numRead += ll;
	    }
	    in_.skipBytes(length-numRead);
	  }
	*/

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeLOBLocatorHandle(AttributeLOBLocatorHandle attrib) throws IOException
	  private void writeLOBLocatorHandle(AttributeLOBLocatorHandle attrib)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3818);
		out_.writeInt(attrib.LOBLocatorHandle);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeRequestedSize(AttributeRequestedSize attrib) throws IOException
	  private void writeRequestedSize(AttributeRequestedSize attrib)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3819);
		out_.writeInt(attrib.RequestedSize);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeStartOffset(AttributeStartOffset attrib) throws IOException
	  private void writeStartOffset(AttributeStartOffset attrib)
	  {
		out_.writeInt(10);
		out_.writeShort(0x381A);
		out_.writeInt(attrib.StartOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeCompressionIndicator(AttributeCompressionIndicator attrib) throws IOException
	  private void writeCompressionIndicator(AttributeCompressionIndicator attrib)
	  {
		out_.writeInt(7);
		out_.writeShort(0x381B);
		out_.write(attrib.CompressionIndicator);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeReturnCurrentLengthIndicator(AttributeReturnCurrentLengthIndicator attrib) throws IOException
	  private void writeReturnCurrentLengthIndicator(AttributeReturnCurrentLengthIndicator attrib)
	  {
		out_.writeInt(7);
		out_.writeShort(0x3821);
		out_.write(attrib.ReturnCurrentLengthIndicator);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeColumnIndex(AttributeColumnIndex attrib) throws IOException
	  private void writeColumnIndex(AttributeColumnIndex attrib)
	  {
		out_.writeInt(10);
		out_.writeShort(0x3828);
		out_.writeInt(attrib.ColumnIndex);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendRetrieveLOBDataRequest(HostOutputStream out, DatabaseRetrieveLOBDataAttributes attribs, int rpbID) throws IOException
	  private void sendRetrieveLOBDataRequest(HostOutputStream @out, DatabaseRetrieveLOBDataAttributes attribs, int rpbID)
	  {
		int length = 40;
		int parms = 0;
		if (attribs.LOBLocatorHandleSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.RequestedSizeSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.StartOffsetSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.CompressionIndicatorSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.TranslateIndicatorSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.ReturnCurrentLengthIndicatorSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.ColumnIndexSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.RLECompressedFunctionParametersSet)
		{
		  ++parms;
		  sbyte[] comp = attribs.RLECompressedFunctionParameters;
		  length += 10 + comp.Length;
		}

		writeHeader(length, 0x1816);
		// writeTemplate(parms, 0xFC000000, 0, rpbID);
		int template = OperationalResultBitmap_Fields.SEND_REPLY_IMMED | OperationalResultBitmap_Fields.DATA_FORMAT | OperationalResultBitmap_Fields.RESULT_DATA; // | MESSAGE_ID | FIRST_LEVEL_TEXT | SECOND_LEVEL_TEXT | DATA_FORMAT | RESULT_DATA;
		writeTemplate(parms, template, 0, rpbID);

		if (attribs.LOBLocatorHandleSet)
		{
		  writeLOBLocatorHandle(attribs);
		}
		if (attribs.RequestedSizeSet)
		{
		  writeRequestedSize(attribs);
		}
		if (attribs.StartOffsetSet)
		{
		  writeStartOffset(attribs);
		}
		if (attribs.CompressionIndicatorSet)
		{
		  writeCompressionIndicator(attribs);
		}
		if (attribs.TranslateIndicatorSet)
		{
		  writeTranslateIndicator(attribs);
		}
		if (attribs.ReturnCurrentLengthIndicatorSet)
		{
		  writeReturnCurrentLengthIndicator(attribs);
		}
		if (attribs.ColumnIndexSet)
		{
		  writeColumnIndex(attribs);
		}
		if (attribs.RLECompressedFunctionParametersSet)
		{
		  writeRLECompressedFunctionParameters(attribs);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendSetServerAttributesRequest(HostOutputStream out, DatabaseServerAttributes attribs) throws IOException
	  private void sendSetServerAttributesRequest(HostOutputStream @out, DatabaseServerAttributes attribs)
	  {
		// Calculate total length.
		int length = 40;
		int parms = 0;
		if (attribs.DefaultClientCCSIDSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.LanguageFeatureCodeSet)
		{
		  length += 12;
		  ++parms;
		}
		if (attribs.ClientFunctionalLevelSet)
		{
		  length += 18;
		  ++parms;
		}
		if (attribs.NLSSIdentifierSet)
		{
		  ++parms;
		  int val = attribs.NLSSIdentifier;
		  length += 8;
		  if (val == 1 || val == 2)
		  {
			length += 5;
		  }
		  else if (val == 3)
		  {
			length += 6;
			string tableName = attribs.NLSSIdentifierLanguageTableName;
			length += tableName.Length;
			string tableLibrary = attribs.NLSSIdentifierLanguageTableLibrary;
			length += tableLibrary.Length;
		  }
		}
		if (attribs.TranslateIndicatorSet)
		{
		  length += getTranslateIndicatorLength(attribs);
		  ++parms;
		}
		if (attribs.DRDAPackageSizeSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.DateFormatParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.DateSeparatorParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.TimeFormatParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.TimeSeparatorParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.DecimalSeparatorParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.NamingConventionParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.IgnoreDecimalDataErrorParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.CommitmentControlLevelParserOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.DefaultSQLLibraryNameSet)
		{
		  ++parms;
		  string library = attribs.DefaultSQLLibraryName;
		  length += 10 + library.Length;
		}
		if (attribs.ASCIICCSIDForTranslationTableSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.AmbiguousSelectOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.PackageAddStatementAllowedSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.UseExtendedFormatsSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.LOBFieldThresholdSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.DataCompressionParameterSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.TrueAutoCommitIndicatorSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.ClientSupportInformationSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.RDBNameSet)
		{
		  ++parms;
		  string name = attribs.RDBName;
		  length += 8 + name.Length;
		}
		if (attribs.DecimalPrecisionAndScaleAttributesSet)
		{
		  length += 12;
		  ++parms;
		}
		if (attribs.HexadecimalConstantParserOptionSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.InputLocatorTypeSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.LocatorPersistenceSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.EWLMCorrelatorSet)
		{
		  ++parms;
		  sbyte[] corr = attribs.EWLMCorrelator;
		  length += 6 + corr.Length;
		}
		if (attribs.RLECompressionSet)
		{
		  ++parms;
		  sbyte[] comp = attribs.RLECompression;
		  length += 10 + comp.Length;
		}
		if (attribs.OptimizationGoalIndicatorSet)
		{
		  length += 7;
		  ++parms;
		}
		if (attribs.QueryStorageLimitSet)
		{
		  length += 10;
		  ++parms;
		}
		if (attribs.DecimalFloatingPointRoundingModeOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.DecimalFloatingPointErrorReportingOptionSet)
		{
		  length += 8;
		  ++parms;
		}
		if (attribs.ClientAccountingInformationSet)
		{
		  ++parms;
		  string info = attribs.ClientAccountingInformation;
		  length += 10 + info.Length;
		}
		if (attribs.ClientApplicationNameSet)
		{
		  ++parms;
		  string name = attribs.ClientApplicationName;
		  length += 10 + name.Length;
		}
		if (attribs.ClientUserIdentifierSet)
		{
		  ++parms;
		  string user = attribs.ClientUserIdentifier;
		  length += 10 + user.Length;
		}
		if (attribs.ClientWorkstationNameSet)
		{
		  ++parms;
		  string name = attribs.ClientWorkstationName;
		  length += 10 + name.Length;
		}
		if (attribs.ClientProgramIdentifierSet)
		{
		  ++parms;
		  string prog = attribs.ClientProgramIdentifier;
		  length += 10 + prog.Length;
		}
		if (attribs.InterfaceTypeSet)
		{
		  ++parms;
		  string type = attribs.InterfaceType;
		  length += 10 + type.Length;
		}
		if (attribs.InterfaceNameSet)
		{
		  ++parms;
		  string name = attribs.InterfaceName;
		  length += 10 + name.Length;
		}
		if (attribs.InterfaceLevelSet)
		{
		  ++parms;
		  string level = attribs.InterfaceLevel;
		  length += 10 + level.Length;
		}
		if (attribs.CloseOnEOFSet)
		{
		  length += 7;
		  ++parms;
		}

		writeHeader(length, 0x1F80);

		// Write template.
		@out.writeInt(unchecked((int)0x81000000)); // Operational result (ORS) bitmap - return data + server attributes (no RLE compression).
		@out.writeInt(0); // Reserved.
		@out.writeShort(0); // Return ORS handle - after operation completes.
		@out.writeShort(0); // Fill ORS handle.
		@out.writeShort(0); // Based on ORS handle.
		@out.writeShort(0); // Request parameter block (RPB) handle.
		@out.writeShort(0); // Parameter marker descriptor handle.
		@out.writeShort(parms); // Parameter count.


		// Write parameters.
		if (attribs.DefaultClientCCSIDSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3801);
		  @out.writeShort(attribs.DefaultClientCCSID);
		}
		if (attribs.LanguageFeatureCodeSet)
		{
		  @out.writeInt(12);
		  @out.writeShort(0x3802);
		  @out.writeShort(37);
		  writePadEBCDIC(attribs.LanguageFeatureCode, 4, @out);
		}
		if (attribs.ClientFunctionalLevelSet)
		{
		  @out.writeInt(18);
		  @out.writeShort(0x3803);
		  @out.writeShort(37);
		  writePadEBCDIC(attribs.ClientFunctionalLevel, 10, @out);
		}
		if (attribs.NLSSIdentifierSet)
		{
		  int val = attribs.NLSSIdentifier;
		  int ll = 8;
		  if (val == 1 || val == 2)
		  {
			ll += 5;
		  }
		  else if (val == 3)
		  {
			ll += 6;
			string tableName = attribs.NLSSIdentifierLanguageTableName;
			ll += tableName.Length;
			string tableLibrary = attribs.NLSSIdentifierLanguageTableLibrary;
			ll += tableLibrary.Length;
		  }
		  @out.writeInt(ll);
		  @out.writeShort(0x3804);
		  @out.writeShort(val);
		  if (val == 1 || val == 2)
		  {
			@out.writeShort(37);
			writePadEBCDIC(attribs.NLSSIdentifierLanguageID, 3, @out);
		  }
		  else if (val == 3)
		  {
			@out.writeShort(37);
			string tableName = attribs.NLSSIdentifierLanguageTableName;
			@out.writeShort(tableName.Length);
			writePadEBCDIC(tableName, tableName.Length, @out);
			string tableLibrary = attribs.NLSSIdentifierLanguageTableLibrary;
			@out.writeShort(tableLibrary.Length);
			writePadEBCDIC(tableLibrary, tableLibrary.Length, @out);
		  }
		}
		if (attribs.TranslateIndicatorSet)
		{
		  writeTranslateIndicator(attribs);
		}
		if (attribs.DRDAPackageSizeSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3806);
		  @out.writeShort(attribs.DRDAPackageSize);
		}
		if (attribs.DateFormatParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3807);
		  @out.writeShort(attribs.DateFormatParserOption);
		}
		if (attribs.DateSeparatorParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3808);
		  @out.writeShort(attribs.DateSeparatorParserOption);
		}
		if (attribs.TimeFormatParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3809);
		  @out.writeShort(attribs.TimeFormatParserOption);
		}
		if (attribs.TimeSeparatorParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x380A);
		  @out.writeShort(attribs.TimeSeparatorParserOption);
		}
		if (attribs.DecimalSeparatorParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x380B);
		  @out.writeShort(attribs.DecimalSeparatorParserOption);
		}
		if (attribs.NamingConventionParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x380C);
		  @out.writeShort(attribs.NamingConventionParserOption);
		}
		if (attribs.IgnoreDecimalDataErrorParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x380D);
		  @out.writeShort(attribs.IgnoreDecimalDataErrorParserOption);
		}
		if (attribs.CommitmentControlLevelParserOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x380E);
		  @out.writeShort(attribs.CommitmentControlLevelParserOption);
		}
		if (attribs.DefaultSQLLibraryNameSet)
		{
		  string library = attribs.DefaultSQLLibraryName;
		  @out.writeInt(10 + library.Length);
		  @out.writeShort(0x380F);
		  @out.writeShort(37);
		  @out.writeShort(library.Length);
		  writePadEBCDIC(library, library.Length, @out);
		}
		if (attribs.ASCIICCSIDForTranslationTableSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3810);
		  @out.writeShort(attribs.ASCIICCSIDForTranslationTable);
		}
		if (attribs.AmbiguousSelectOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3811);
		  @out.writeShort(attribs.AmbiguousSelectOption);
		}
		if (attribs.PackageAddStatementAllowedSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3812);
		  @out.writeShort(attribs.PackageAddStatementAllowed);
		}
		// Skip Data Source Name (DSN) parameters, this is what JTOpen does, too.
		if (attribs.UseExtendedFormatsSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x3821);
		  @out.writeByte(attribs.UseExtendedFormats);
		}
		if (attribs.LOBFieldThresholdSet)
		{
		  @out.writeInt(10);
		  @out.writeShort(0x3822);
		  @out.writeInt(attribs.LOBFieldThreshold);
		}
		if (attribs.DataCompressionParameterSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3823);
		  @out.writeShort(attribs.DataCompressionParameter);
		}
		if (attribs.TrueAutoCommitIndicatorSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x3824);
		  @out.writeByte(attribs.TrueAutoCommitIndicator);
		}
		if (attribs.ClientSupportInformationSet)
		{
		  @out.writeInt(10);
		  @out.writeShort(0x3825);
		  @out.writeInt(attribs.ClientSupportInformation);
		}
		if (attribs.RDBNameSet)
		{
		  string name = attribs.RDBName;
		  @out.writeInt(8 + name.Length);
		  @out.writeShort(0x3826);
		  @out.writeShort(37);
		  writePadEBCDIC(name, name.Length, @out);
		}
		if (attribs.DecimalPrecisionAndScaleAttributesSet)
		{
		  @out.writeInt(12);
		  @out.writeShort(0x3827);
		  @out.writeShort(attribs.MaximumDecimalPrecision);
		  @out.writeShort(attribs.MaximumDecimalScale);
		  @out.writeShort(attribs.MinimumDivideScale);
		}
		if (attribs.HexadecimalConstantParserOptionSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x3828);
		  @out.writeByte(attribs.HexadecimalConstantParserOption);
		}
		if (attribs.InputLocatorTypeSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x3829);
		  @out.writeByte(attribs.InputLocatorType);
		}
		// Don't ask me why someone skipped 0x382A-0x382F... my guess is they don't know how to count.
		if (attribs.LocatorPersistenceSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3830);
		  @out.writeShort(attribs.LocatorPersistence);
		}
		if (attribs.EWLMCorrelatorSet)
		{
		  sbyte[] corr = attribs.EWLMCorrelator;
		  @out.writeInt(6 + corr.Length);
		  @out.writeShort(0x3831);
		  @out.write(corr);
		}
		if (attribs.RLECompressionSet)
		{
		  sbyte[] comp = attribs.RLECompression;
		  @out.writeInt(10 + comp.Length);
		  @out.writeShort(0x3832);
		  @out.writeInt(comp.Length);
		  @out.write(comp);
		}
		if (attribs.OptimizationGoalIndicatorSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x3833);
		  @out.writeByte(attribs.OptimizationGoalIndicator);
		}
		if (attribs.QueryStorageLimitSet)
		{
		  @out.writeInt(10);
		  @out.writeShort(0x3834);
		  @out.writeInt(attribs.QueryStorageLimit);
		}
		if (attribs.DecimalFloatingPointRoundingModeOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3835);
		  @out.writeShort(attribs.DecimalFloatingPointRoundingModeOption);
		}
		if (attribs.DecimalFloatingPointErrorReportingOptionSet)
		{
		  @out.writeInt(8);
		  @out.writeShort(0x3836);
		  @out.writeShort(attribs.DecimalFloatingPointErrorReportingOption);
		}
		if (attribs.ClientAccountingInformationSet)
		{
		  string info = attribs.ClientAccountingInformation;
		  @out.writeInt(10 + info.Length);
		  @out.writeShort(0x3837);
		  @out.writeShort(37);
		  @out.writeShort(info.Length);
		  writePadEBCDIC(info, info.Length, @out);
		}
		if (attribs.ClientApplicationNameSet)
		{
		  string name = attribs.ClientApplicationName;
		  @out.writeInt(10 + name.Length);
		  @out.writeShort(0x3838);
		  @out.writeShort(37);
		  @out.writeShort(name.Length);
		  writePadEBCDIC(name, name.Length, @out);
		}
		if (attribs.ClientUserIdentifierSet)
		{
		  string user = attribs.ClientUserIdentifier;
		  @out.writeInt(10 + user.Length);
		  @out.writeShort(0x3839);
		  @out.writeShort(37);
		  @out.writeShort(user.Length);
		  writePadEBCDIC(user, user.Length, @out);
		}
		if (attribs.ClientWorkstationNameSet)
		{
		  string name = attribs.ClientWorkstationName;
		  @out.writeInt(10 + name.Length);
		  @out.writeShort(0x383A);
		  @out.writeShort(37);
		  @out.writeShort(name.Length);
		  writePadEBCDIC(name, name.Length, @out);
		}
		if (attribs.ClientProgramIdentifierSet)
		{
		  string prog = attribs.ClientProgramIdentifier;
		  @out.writeInt(10 + prog.Length);
		  @out.writeShort(0x383B);
		  @out.writeShort(37);
		  @out.writeShort(prog.Length);
		  writePadEBCDIC(prog, prog.Length, @out);
		}
		if (attribs.InterfaceTypeSet)
		{
		  string type = attribs.InterfaceType;
		  @out.writeInt(10 + type.Length);
		  @out.writeShort(0x383C);
		  @out.writeShort(37);
		  @out.writeShort(type.Length);
		  writePadEBCDIC(type, type.Length, @out);
		}
		if (attribs.InterfaceNameSet)
		{
		  string name = attribs.InterfaceName;
		  @out.writeInt(10 + name.Length);
		  @out.writeShort(0x383D);
		  @out.writeShort(37);
		  @out.writeShort(name.Length);
		  writePadEBCDIC(name, name.Length, @out);
		}
		if (attribs.InterfaceLevelSet)
		{
		  string level = attribs.InterfaceLevel;
		  @out.writeInt(10 + level.Length);
		  @out.writeShort(0x383E);
		  @out.writeShort(37);
		  @out.writeShort(level.Length);
		  writePadEBCDIC(level, level.Length, @out);
		}
		if (attribs.CloseOnEOFSet)
		{
		  @out.writeInt(7);
		  @out.writeShort(0x383F);
		  @out.writeByte(attribs.CloseOnEOF);
		}
	  }







//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendCommitRequest(HostOutputStream out) throws IOException
	  private void sendCommitRequest(HostOutputStream @out)
	  {
		int length = 40;

		// Write header (20 bytes)
		writeHeader(length, 0x1807);

		// Write template (20 bytes)
		@out.writeInt(unchecked((int)0x80000000)); // Operational result (ORS) bitmap - return data .
		@out.writeInt(0); // Reserved.
		@out.writeShort(0); // Return ORS handle - after operation completes.
		@out.writeShort(0); // Fill ORS handle.
		@out.writeShort(0); // Based on ORS handle.
		@out.writeShort(0); // Request parameter block (RPB) handle.
		@out.writeShort(0); // Parameter marker descriptor handle.
		@out.writeShort(0); // Parameter count.

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sendRollbackRequest(HostOutputStream out) throws IOException
	  private void sendRollbackRequest(HostOutputStream @out)
	  {
		int length = 40;

		// Write header (20 bytes)
		writeHeader(length, 0x1808);

		// Write template (20 bytes)
		@out.writeInt(unchecked((int)0x80000000)); // Operational result (ORS) bitmap - return data .
		@out.writeInt(0); // Reserved.
		@out.writeShort(0); // Return ORS handle - after operation completes.
		@out.writeShort(0); // Fill ORS handle.
		@out.writeShort(0); // Based on ORS handle.
		@out.writeShort(0); // Request parameter block (RPB) handle.
		@out.writeShort(0); // Parameter marker descriptor handle.
		@out.writeShort(0); // Parameter count.

	  }


	}

}