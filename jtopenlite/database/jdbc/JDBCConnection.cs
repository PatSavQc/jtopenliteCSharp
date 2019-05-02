using System;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCConnection.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{
	using com.ibm.jtopenlite.database;


	/// <summary>
	/// <para>The JDBCConnection class provides a JDBC connection
	/// to a specific DB2 for IBM i database.  Use
	/// DriverManager.getConnection() with a jdbc:jtopenlite://SYSTENAME URL to create AS400JDBCConnection
	/// objects.
	/// 
	/// </para>
	/// </summary>
	public class JDBCConnection : java.sql.Connection, DatabaseWarningCallback
	{
	  private DatabaseConnection conn_;

	  private int statementCounter_ = 0;
	  private int cursorCounter_ = 0;
	  private int descriptorCounter_ = 0;
	  private readonly char[] statementName_ = new char[] {'S', 'T', 'M', 'T', '0', '0', '0', '0', '0', '0', '0', '0'};
	  private readonly char[] cursorName_ = new char[] {'C', 'R', 'S', 'R', '0', '0', '0', '0', '0', '0', '0', '0'};
	  private static readonly char[] CHARS = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

	  private int lastWarningClass_;
	  private int lastWarningReturnCode_;
	  private string lastWarningMessageID_;
	  private string lastWarningMessageText_;
	  private string lastWarningSecondLevelText_;
	  private string catalog_ = null;
	  private int serverVersion_;
	  private string serverJobIdentifier_;

	  private bool autoCommit_ = true; // JDBC view of autocommit
	  private int transactionIsolation_ = Connection.TRANSACTION_NONE; // JDBC view of transaction isolation level
											  /* None means that transaction isolation has not been set */
	  private readonly bool[] usedRPBs_ = new bool[32768];

	  private readonly DateTime calendar_ = new DateTime(); // Master calendar used for conversions.

	  private string userName_;

	  private List<object> freeStatements_ = new List<object>();
	  private List<object> freeCursors_ = new List<object>();


	  private JDBCConnection(DatabaseConnection conn)
	  {
		conn_ = conn;
	  }

	  internal virtual DateTime Calendar
	  {
		  get
		  {
			return calendar_;
		  }
	  }

	  internal virtual string NextStatementName
	  {
		  get
		  {
			  lock (this)
			  {
				  int freeStatementCount = freeStatements_.Count;
				  if (freeStatementCount > 0)
				  {
					  return (string) freeStatements_.RemoveAt(freeStatementCount - 1);
				  }
				  statementCounter_ = statementCounter_ == 0x7FFFFFFF ? 1 : statementCounter_ + 1;
				  int counter = statementCounter_;
				  for (int i = 11; i >= 4; --i)
				  {
					statementName_[i] = CHARS[counter & 0x0F];
					counter = counter >> 4;
				  }
				  return new string(statementName_);
			  }
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void freeStatementAndCursorNames(String statementName, String cursorName) throws SQLException
	  internal virtual void freeStatementAndCursorNames(string statementName, string cursorName)
	  {
		  lock (this)
		  {
			  if (!string.ReferenceEquals(statementName, null))
			  {
				  freeStatements_.Add(statementName);
			  }
			  if (!string.ReferenceEquals(cursorName, null))
			  {
				  freeCursors_.Add(cursorName);
			  }
		  }
	  }



	  internal virtual string NextCursorName
	  {
		  get
		  {
			  lock (this)
			  {
				  int freeCursorCount = freeCursors_.Count;
				  if (freeCursorCount > 0)
				  {
					  return (string) freeCursors_.RemoveAt(freeCursorCount - 1);
				  }
            
				  cursorCounter_ = cursorCounter_ == 0x7FFFFFFF ? 1 : cursorCounter_ + 1;
				  int counter = cursorCounter_;
				  for (int i = 11; i >= 4; --i)
				  {
					cursorName_[i] = CHARS[counter & 0x0F];
					counter = counter >> 4;
				  }
				  return new string(cursorName_);
			  }
		  }
	  }

	  internal virtual int NextDescriptorHandle
	  {
		  get
		  {
			  descriptorCounter_ = descriptorCounter_ == 0x7FFF ? 1 : descriptorCounter_ + 1;
			  return descriptorCounter_;
		  }
	  }

	  internal virtual int NextRPBID
	  {
		  get
		  {
			  lock (this)
			  {
				  // 0 is the default RPB on the server.
				  for (int i = 1; i < usedRPBs_.Length; ++i)
				  {
					if (!usedRPBs_[i])
					{
					  usedRPBs_[i] = true;
					  return i;
					}
				  }
				return -1;
			  }
		  }
	  }

	  internal virtual void freeRPBID(int id)
	  {
		  lock (this)
		  {
			  usedRPBs_[id] = false;
		  }
	  }

	  public virtual void newWarning(int rcClass, int rcClassReturnCode)
	  {
		lastWarningClass_ = rcClass;
		lastWarningReturnCode_ = rcClassReturnCode;
	  }

	  public virtual void noWarnings()
	  {
		lastWarningClass_ = 0;
		lastWarningReturnCode_ = 0;
		lastWarningMessageID_ = null;
		lastWarningMessageText_ = null;
		lastWarningSecondLevelText_ = null;
	  }

	  public virtual void newMessageID(string id)
	  {
		lastWarningMessageID_ = id;
	  }

	  public virtual void newMessageText(string text)
	  {
		lastWarningMessageText_ = text;
	  }

	  public virtual void newSecondLevelText(string text)
	  {
		lastWarningSecondLevelText_ = text;
	  }

	  internal virtual int LastWarningClass
	  {
		  get
		  {
			return lastWarningClass_;
		  }
	  }

	  internal virtual int LastWarningReturnCode
	  {
		  get
		  {
			return lastWarningReturnCode_;
		  }
	  }

	  internal virtual Message LastWarningMessage
	  {
		  get
		  {
			if (!string.ReferenceEquals(lastWarningMessageID_, null))
			{
			  return new Message(lastWarningMessageID_, lastWarningMessageText_);
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JDBCConnection getConnection(String system, String user, String password, boolean debug) throws SQLException
	  public static JDBCConnection getConnection(string system, string user, string password, bool debug)
	  {
		return getConnection(false, system, user, password, debug);
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JDBCConnection getConnection(boolean isSSL, String system, String user, String password, boolean debug) throws SQLException
	  public static JDBCConnection getConnection(bool isSSL, string system, string user, string password, bool debug)
	  {
		try
		{
		  // The first boolean parameter indicates that SSL should be used.
		  DatabaseConnection conn = DatabaseConnection.getConnection(isSSL, system, user, password);
		  conn.MessageInfoReturned = true;
		  conn.Debug = debug;
		  DatabaseServerAttributes dsa = new DatabaseServerAttributes();
		  dsa.NamingConventionParserOption = 0;
		  dsa.UseExtendedFormats = 0xF2;
		  dsa.DefaultClientCCSID = 13488;
		  dsa.DateFormatParserOption = 5; // ISO.
		  dsa.LOBFieldThreshold = 1024 * 1024; // Use a locator for any LOB data fields longer than 1 MB.
		  dsa.ClientSupportInformation = 0x40000000; // Client supports True autocommit
		  dsa.InterfaceType = "JDBC";
		  dsa.InterfaceName = About.INTERFACE_NAME;
		  dsa.InterfaceLevel = About.INTERFACE_LEVEL;

		  //
		  // Do not set any commitment control levels.
		  // The default behavior is true auto commit = off
		  // and a transaction isolation of *NONE.
		  // This means that connection behaves like autocommit = on.
		  //

		  conn.ServerAttributes = dsa;

		  //
		  // Remember the serverJobIdentifier
		  //


	//      DatabaseRequestAttributes rpb = new DatabaseRequestAttributes();
	//      conn.createRequestParameterBlock(rpb);
	//      JDBCConnection j = new JDBCConnection(conn, rpb);
		  JDBCConnection j = new JDBCConnection(conn);
		  conn.WarningCallback = j;
		  j.serverJobIdentifier_ = dsa.ServerJobName + dsa.ServerJobUser + dsa.ServerJobNumber;
		  return j;
		}
		catch (IOException io)
		{
		  throw convertException(io);
		}
	  }

	  internal virtual DatabaseConnection DatabaseConnection
	  {
		  get
		  {
			return conn_;
		  }
	  }

	  internal static SQLException convertException(IOException io)
	  {
		return convertException(io, -99999, "");
	  }

	  internal static SQLException convertException(IOException io, int sqlCode, string sqlState)
	  {
		SQLException sql = null;
		if (io is MessageException)
		{
		  MessageException me = (MessageException)io;
		  Message[] messages = me.Messages;
		  string reason = messages[0].ToString();
		  sql = new SQLException(reason, sqlState, sqlCode);
		  sql.initCause(io);

		}
		else
		{
		  string reason = io.ToString();
		  sql = new SQLException(reason, sqlState, sqlCode);
		  sql.initCause(io);
		}

		return sql;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clearWarnings() throws SQLException
	  public virtual void clearWarnings()
	  {
		noWarnings();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws SQLException
	  public virtual void close()
	  {
		if (Closed)
		{
			return;
		}
		try
		{
		  conn_.close();
		}
		catch (IOException io)
		{
		  throw convertException(io);
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkOpen() throws SQLException
	  private void checkOpen()
	  {
		  if (Closed)
		  {
		  throw JDBCError.getSQLException(JDBCError.EXC_CONNECTION_NONE);
		  }
	  }

	  /// <summary>
	  /// Commit the current transaction.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void commit() throws SQLException
	  public virtual void commit()
	  {
		try
		{
		  conn_.commit();
		}
		catch (IOException io)
		{
		  throw convertException(io);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Statement createStatement() throws SQLException
	  public virtual Statement createStatement()
	  {
		  string stName = NextStatementName;
		  int rpbId = NextRPBID;
		  string cursorName = NextCursorName;
		  return new JDBCStatement(this, stName, cursorName, rpbId);
	  }


	  /// <summary>
	  /// Only valid for ResultSet.TYPE_FORWARD_ONLY and ResultSet.CONCUR_READ_ONLY.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Statement createStatement(int resultSetType, int resultSetConcurrency) throws SQLException
	  public virtual Statement createStatement(int resultSetType, int resultSetConcurrency)
	  {
		  // Just create the statement if the setting match the default settings
		  if (resultSetType == ResultSet.TYPE_FORWARD_ONLY && resultSetConcurrency == ResultSet.CONCUR_READ_ONLY)
		  {

		  return createStatement();
		  }

		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Only valid for ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_READ_ONLY, and ResultSet.HOLD_CURSORS_OVER_COMMIT.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Statement createStatement(int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException
	  public virtual Statement createStatement(int resultSetType, int resultSetConcurrency, int resultSetHoldability)
	  {
		/* if default values are used, then just call the one with default values */
		if ((resultSetType == ResultSet.TYPE_FORWARD_ONLY) && (resultSetConcurrency == ResultSet.CONCUR_READ_ONLY) && (resultSetHoldability == ResultSet.HOLD_CURSORS_OVER_COMMIT))
		{
			return createStatement();
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Return the autocommit setting.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getAutoCommit() throws SQLException
	  public virtual bool AutoCommit
	  {
		  get
		  {
			  checkOpen();
			return autoCommit_;
		  }
		  set
		  {
			  checkOpen();
			  if (value)
			  {
			  // Turn on auto commit
			  if (autoCommit_)
			  {
				  // value already set .. ignore
			  }
			  else
			  {
				  // Changing from autocommit off to autocommit on
				  // Call commit to commit existing work
				  commit();
				  // set the transaction isolation level to none.
				  transactionIsolation_ = Connection.TRANSACTION_NONE;
				  updateServerTransactionAttributes();
				  // For now, we don't worry about supporting true autocommit
				  autoCommit_ = true;
			  }
			  }
			  else
			  {
			  // Turn off autocommit
			  if (!autoCommit_)
			  {
				  // autocommit if already off ignore
			  }
			  else
			  {
				  // If the transaction isolation level is NONE, bump it to
				  // *CHG
				  if (transactionIsolation_ == Connection.TRANSACTION_NONE)
				  {
					  transactionIsolation_ = Connection.TRANSACTION_READ_UNCOMMITTED;
				  }
				  updateServerTransactionAttributes();
				  autoCommit_ = false;
			  }
			  }
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCatalog() throws SQLException
	  public virtual string Catalog
	  {
		  get
		  {
			  if (string.ReferenceEquals(catalog_, null))
			  {
			  JDBCStatement stmt = (JDBCStatement) createStatement();
			  stmt.Catalog = "LOCAL";
			  ResultSet rs = stmt.executeQuery("select CATALOG_NAME from qsys2.syscatalogs where RDBTYPE='LOCAL'");
			  rs.next();
			  catalog_ = rs.getString(1);
			  rs.close();
			  stmt.close();
			  }
			  return catalog_;
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// The holdability is always ResultSet.HOLD_CURSORS_OVER_COMMIT.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getHoldability() throws SQLException
	  public virtual int Holdability
	  {
		  get
		  {
			return ResultSet.HOLD_CURSORS_OVER_COMMIT;
		  }
		  set
		  {
				checkOpen();
			if (value != ResultSet.HOLD_CURSORS_OVER_COMMIT)
			{
			  throw new NotImplementedException();
			}
		  }
	  }

	  /// <summary>
	  /// Returns the metadata for this connection.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatabaseMetaData getMetaData() throws SQLException
	  public virtual DatabaseMetaData MetaData
	  {
		  get
		  {
			  return new JDBCDatabaseMetaData(this);
    
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getTransactionIsolation() throws SQLException
	  public virtual int TransactionIsolation
	  {
		  get
		  {
			  // NONE is not really a valid JDBC option
			  if (transactionIsolation_ == Connection.TRANSACTION_NONE)
			  {
			  return Connection.TRANSACTION_READ_UNCOMMITTED;
			  }
			  return transactionIsolation_;
		  }
		  set
		  {
			  switch (value)
			  {
			  case Connection.TRANSACTION_NONE:
			  case Connection.TRANSACTION_READ_UNCOMMITTED:
			  case Connection.TRANSACTION_READ_COMMITTED:
			  case Connection.TRANSACTION_REPEATABLE_READ:
			  case Connection.TRANSACTION_SERIALIZABLE:
				  transactionIsolation_ = value;
				  updateServerTransactionAttributes();
				  return;
			  }
    
			  throw JDBCError.getSQLException(JDBCError.EXC_ATTRIBUTE_VALUE_INVALID);
		  }
	  }

	//  public Map<String,Class<?>> getTypeMap() throws SQLException
	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Map getTypeMap() throws SQLException
	  public virtual System.Collections.IDictionary TypeMap
	  {
		  get
		  {
			throw new NotImplementedException();
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// Returns null because get warnings is not implemented on this driver.
	  /// 
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SQLWarning getWarnings() throws SQLException
	  public virtual SQLWarning Warnings
	  {
		  get
		  {
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isClosed() throws SQLException
	  public virtual bool Closed
	  {
		  get
		  {
			return conn_.Closed;
		  }
	  }

	  /// <summary>
	  /// The driver does not allow readonly to be specified.  All connections will
	  /// allow writes to be done.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReadOnly() throws SQLException
	  public virtual bool ReadOnly
	  {
		  get
		  {
				checkOpen();
			return false;
		  }
		  set
		  {
				checkOpen();
			if (value == true)
			{
				throw new NotImplementedException();
			}
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nativeSQL(String sql) throws SQLException
	  public virtual string nativeSQL(string sql)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CallableStatement prepareCall(String sql) throws SQLException
	  public virtual CallableStatement prepareCall(string sql)
	  {

			string stName = NextStatementName;
			int rpbId = NextRPBID;
			string cursorName = NextCursorName;

			JDBCCallableStatement cs = new JDBCCallableStatement(this, sql, calendar_, stName, cursorName, rpbId);
			cs.CursorNameInternal = cursorName;
			return cs;

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CallableStatement prepareCall(String sql, int resultSetType, int resultSetConcurrency) throws SQLException
	  public virtual CallableStatement prepareCall(string sql, int resultSetType, int resultSetConcurrency)
	  {
		if (resultSetType == ResultSet.TYPE_FORWARD_ONLY && resultSetConcurrency == ResultSet.CONCUR_READ_ONLY)
		{
			return prepareCall(sql);
		}
		else
		{
		   throw new NotImplementedException();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CallableStatement prepareCall(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException
	  public virtual CallableStatement prepareCall(string sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability)
	  {
			if (resultSetType == ResultSet.TYPE_FORWARD_ONLY && resultSetConcurrency == ResultSet.CONCUR_READ_ONLY)
			{
				return prepareCall(sql);
			}
			else
			{
			   throw new NotImplementedException();
			}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreparedStatement prepareStatement(String sql) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql)
	  {
		return prepareStatement(sql, Statement.NO_GENERATED_KEYS);
	  }



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized PreparedStatement prepareStatement(String sql, int autoGeneratedKeys) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql, int autoGeneratedKeys)
	  {
		  lock (this)
		  {
			  string stName = NextStatementName;
			  int rpbId = NextRPBID;
			  string cursorName = NextCursorName;
        
				JDBCPreparedStatement ps = new JDBCPreparedStatement(this, sql, calendar_, stName, cursorName, rpbId);
				ps.CursorNameInternal = cursorName;
				switch (autoGeneratedKeys)
				{
				  case Statement.NO_GENERATED_KEYS:
					ps.ReturnGeneratedKeys = false;
					break;
				  case Statement.RETURN_GENERATED_KEYS:
					ps.ReturnGeneratedKeys = true;
					break;
				  default:
					throw new SQLException("Bad value for autoGeneratedKeys parameter");
				}
				return ps;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreparedStatement prepareStatement(String sql, int[] columnIndices) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql, int[] columnIndices)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Only implemented for ResultSet.TYPE_FORWARD_ONLY and ResultSet.CONCUR_READ_ONLY.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreparedStatement prepareStatement(String sql, int resultSetType, int resultSetConcurrency) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql, int resultSetType, int resultSetConcurrency)
	  {
		/* if default values are used, then just call the one with default values */
		if ((resultSetType == ResultSet.TYPE_FORWARD_ONLY) && (resultSetConcurrency == ResultSet.CONCUR_READ_ONLY))
		{
			return prepareStatement(sql);
		}
		else
		{
			throw new NotImplementedException();
		}
	  }

	  /// <summary>
	  /// Only implemented for ResultSet.TYPE_FORWARD_ONLY and ResultSet.CONCUR_READ_ONLY and ResultSet.HOLD_CURSORS_OVER_COMMIT.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreparedStatement prepareStatement(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability)
	  {
		if ((resultSetType == ResultSet.TYPE_FORWARD_ONLY) && (resultSetConcurrency == ResultSet.CONCUR_READ_ONLY) && (resultSetHoldability == ResultSet.HOLD_CURSORS_OVER_COMMIT))
		{
			return prepareStatement(sql);
		}
		else
		{
			throw new NotImplementedException();
		}
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreparedStatement prepareStatement(String sql, String[] columnNames) throws SQLException
	  public virtual PreparedStatement prepareStatement(string sql, string[] columnNames)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void releaseSavepoint(Savepoint savepoint) throws SQLException
	  public virtual void releaseSavepoint(Savepoint savepoint)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Rollback the current transaction.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void rollback() throws SQLException
	  public virtual void rollback()
	  {
		try
		{
		  conn_.rollback();
		}
		catch (IOException io)
		{
		  throw convertException(io);
		}
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void rollback(Savepoint savepoint) throws SQLException
	  public virtual void rollback(Savepoint savepoint)
	  {
		throw new NotImplementedException();
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void updateServerTransactionAttributes() throws SQLException
	  private void updateServerTransactionAttributes()
	  {
		  try
		  {
		  DatabaseServerAttributes dsa = new DatabaseServerAttributes();
		  int lipiTransactionOption = LipiTransactionOption;
		  dsa.CommitmentControlLevelParserOption = lipiTransactionOption;
		  if (lipiTransactionOption == DatabaseServerAttributes.CC_NONE)
		  {
			  // Make sure true auto commit is off
			  dsa.TrueAutoCommitIndicator = DatabaseServerAttributes.AUTOCOMMIT_OFF;
		  }
		  else
		  {
			  if (autoCommit_)
			  {
				 dsa.TrueAutoCommitIndicator = DatabaseServerAttributes.AUTOCOMMIT_ON;
			  }
			  else
			  {
				dsa.TrueAutoCommitIndicator = DatabaseServerAttributes.AUTOCOMMIT_OFF;
			  }
		  }
		  conn_.ServerAttributes = dsa;
		  }
		  catch (IOException io)
		  {
		  throw convertException(io);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int getLipiTransactionOption() throws SQLException
	  private int LipiTransactionOption
	  {
		  get
		  {
			  switch (transactionIsolation_)
			  {
			  case Connection.TRANSACTION_NONE:
				  return DatabaseServerAttributes.CC_NONE;
			  case Connection.TRANSACTION_READ_UNCOMMITTED:
				  return DatabaseServerAttributes.CC_CHG;
			  case Connection.TRANSACTION_READ_COMMITTED:
				  return DatabaseServerAttributes.CC_CS;
			  case Connection.TRANSACTION_REPEATABLE_READ:
				  return DatabaseServerAttributes.CC_RR;
			  case Connection.TRANSACTION_SERIALIZABLE:
				  return DatabaseServerAttributes.CC_ALL;
			  }
			  throw JDBCError.getSQLException(JDBCError.EXC_ATTRIBUTE_VALUE_INVALID);
		  }
	  }






	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Savepoint setSavepoint() throws SQLException
	  public virtual Savepoint setSavepoint()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Savepoint setSavepoint(String name) throws SQLException
	  public virtual Savepoint setSavepoint(string name)
	  {
		throw new NotImplementedException();
	  }


	  /// <summary>
	  /// Return the version level.  See SystemInfo.VERSION_VxRx constants for possible values. </summary>
	  /// <returns> the version level of the server </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int getServerVersion() throws SQLException
	  protected internal virtual int ServerVersion
	  {
		  get
		  {
			  checkOpen();
			 if (serverVersion_ == 0)
			 {
				 serverVersion_ = conn_.Info.ServerVersion;
			 }
			 return serverVersion_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getUserName() throws SQLException
	public virtual string UserName
	{
		get
		{
			checkOpen();
			if (string.ReferenceEquals(userName_, null))
			{
				userName_ = conn_.User;
			}
			return userName_;
		}
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getURL() throws SQLException
	public virtual string URL
	{
		get
		{
			checkOpen();
			return JDBCDriver.URL_PREFIX_ + conn_.Info.System;
		}
	}

	public virtual string ServerJobIdentifier
	{
		get
		{
			return serverJobIdentifier_;
		}
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createRequestParameterBlock(DatabaseRequestAttributes rpb, int rpbId) throws SQLException
	internal virtual void createRequestParameterBlock(DatabaseRequestAttributes rpb, int rpbId)
	{
		try
		{
		conn_.createRequestParameterBlock(rpb, rpbId);
		}
		catch (IOException io)
		{
		  throw convertException(io);
		}

	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void prepareAndDescribe(DatabaseRequestAttributes attribs, DatabaseDescribeCallback listener, JDBCParameterMetaData pmd) throws SQLException
	internal virtual void prepareAndDescribe(DatabaseRequestAttributes attribs, DatabaseDescribeCallback listener, JDBCParameterMetaData pmd)
	{
		try
		{
		conn_.prepareAndDescribe(attribs, listener, pmd);
		}
		catch (IOException io)
		{
			  throw convertException(io);
		}

	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void changeDescriptor(DatabaseChangeDescriptorAttributes cda, int handle) throws SQLException
	internal virtual void changeDescriptor(DatabaseChangeDescriptorAttributes cda, int handle)
	{
		try
		{
			conn_.changeDescriptor(cda, handle);
		}
		catch (IOException io)
		{
			  throw convertException(io);
		}
	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array createArrayOf(String arg0, Object[] arg1) throws SQLException
	public virtual Array createArrayOf(string arg0, object[] arg1)
	{
	  throw new NotImplementedException();
	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Blob createBlob() throws SQLException
	public virtual Blob createBlob()
	{
	  throw new NotImplementedException();
	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Clob createClob() throws SQLException
	public virtual Clob createClob()
	{
	  throw new NotImplementedException();
	}




	/// <param name="arg0"> </param>
	/// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Struct createStruct(String arg0, Object[] arg1) throws SQLException
	public virtual Struct createStruct(string arg0, object[] arg1)
	{
	  throw new NotImplementedException();
	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Properties getClientInfo() throws SQLException
	public virtual Properties ClientInfo
	{
		get
		{
		  throw new NotImplementedException();
		}
	}


	/// <param name="arg0"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getClientInfo(String arg0) throws SQLException
	public virtual string getClientInfo(string arg0)
	{
	  throw new NotImplementedException();
	}

	/// <param name="arg0"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isValid(int arg0) throws SQLException
	public virtual bool isValid(int arg0)
	{
	  throw new NotImplementedException();
	}




	internal virtual SystemInfo DatabaseInfo
	{
		get
		{
			return conn_.Info;
		}
	}

	}


}