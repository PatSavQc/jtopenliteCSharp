using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCStatement.java
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


	public class JDBCStatement : Statement, DatabaseSQLCommunicationsAreaCallback //, DatabaseDescribeCallback
	{

		public const int TYPE_UNKNOWN = 0;
		public const int TYPE_INSERT_UPDATE_DELETE = 1;
		public const int TYPE_SELECT = 2;
		public const int TYPE_CALL = 3;
		public const int TYPE_COMMIT = 4;
		public const int TYPE_ROLLBACK = 5;
		public const int TYPE_CONNECT = 6;
		public const int TYPE_BLOCKED_INSERT = 7;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int getStatementType(String sql) throws SQLException
		  public static int getStatementType(string sql)
		  {
			  // Check for null string
			  if (string.ReferenceEquals(sql, null))
			  {
				  JDBCError.throwSQLException(JDBCError.EXC_SYNTAX_ERROR);
			  }

			  string st = sql.ToUpper().Trim();


			  while (st.Length > 0 && st[0] == '(')
			  {
				  st = st.Substring(1).Trim();
			  }
			  int sqlStatementType = st.StartsWith("SELECT", StringComparison.Ordinal) || st.StartsWith("VALUES", StringComparison.Ordinal) ? TYPE_SELECT : st.StartsWith("INSERT", StringComparison.Ordinal) || st.StartsWith("UPDATE", StringComparison.Ordinal) || st.StartsWith("DELETE", StringComparison.Ordinal) ? TYPE_INSERT_UPDATE_DELETE : st.StartsWith("CALL", StringComparison.Ordinal) ? TYPE_CALL : st.StartsWith("COMMIT", StringComparison.Ordinal) ? TYPE_COMMIT : st.StartsWith("ROLLBACK", StringComparison.Ordinal) ? TYPE_ROLLBACK : st.StartsWith("CONNECT", StringComparison.Ordinal) || st.StartsWith("SET", StringComparison.Ordinal) || st.StartsWith("RELEASE", StringComparison.Ordinal) || st.StartsWith("DISCONNECT", StringComparison.Ordinal) ? TYPE_CONNECT : st.StartsWith("BLOCKED INSERT", StringComparison.Ordinal) ? TYPE_BLOCKED_INSERT : TYPE_UNKNOWN;


			  // Check for statement too long
			  if (st.Length > (2097152 / 2))
			  {
				  JDBCError.throwSQLException(JDBCError.EXC_SQL_STATEMENT_TOO_LONG);
			  }
			  return sqlStatementType;
		  }






	  internal JDBCConnection conn_;
	  internal DatabaseRequestAttributes statementAttributes_; // Used to hold statement type
	  internal DatabaseRequestAttributes attribs_;
	  internal int rpbID_;
	  internal int fetchSize_;

	  internal string cursorName_ = null;
	  internal JDBCResultSet currentResultSet_;
	  internal bool closed_;

	  internal string generatedKey_;
	  internal int updateCount_ = -1;
	  internal int lastUpdateCount_ = -1;
	  internal int resultSetsCount_ = -1;
	  internal int lastSQLCode_;
	  internal string lastSQLState_;

	  internal string statementName_ = null;
	  internal string catalog_ = null;
	  internal bool poolable_ = false; // Default is false

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JDBCStatement(JDBCConnection conn, String statementName, String cursorName, int rpbID) throws SQLException
	  public JDBCStatement(JDBCConnection conn, string statementName, string cursorName, int rpbID)
	  {
		  cursorName_ = cursorName;
		  statementName_ = statementName;
			if (rpbID != 0)
			{
				DatabaseRequestAttributes rpb = new DatabaseRequestAttributes();
				rpb.CursorName = cursorName;
				rpb.PrepareStatementName = statementName;
				conn.createRequestParameterBlock(rpb, rpbID);
				attribs_ = rpb;
				statementAttributes_ = rpb.copy();

			}
			conn_ = conn;
			rpbID_ = rpbID;
	  }

	  public virtual void newSQLCommunicationsAreaData(int sqlCode, string sqlState, string generatedKey, int updateCount, int resultSetsCount)
	  {
		if (sqlCode == 0)
		{
		  generatedKey_ = generatedKey;
		}
		else
		{
		  generatedKey_ = null;
		}
		lastUpdateCount_ = updateCount;
		lastSQLCode_ = sqlCode;
		lastSQLState_ = sqlState;
		resultSetsCount_ = resultSetsCount;
	  }

	  internal virtual int LastSQLCode
	  {
		  get
		  {
			return lastSQLCode_;
		  }
	  }

	  internal virtual string LastSQLState
	  {
		  get
		  {
			return lastSQLState_;
		  }
	  }

	  internal virtual DatabaseConnection DatabaseConnection
	  {
		  get
		  {
			return conn_.DatabaseConnection;
		  }
	  }

	  internal virtual DatabaseRequestAttributes RequestAttributes
	  {
		  get
		  {
			attribs_.clear();
			return attribs_;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addBatch(String sql) throws SQLException
	  public virtual void addBatch(string sql)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void cancel() throws SQLException
	  public virtual void cancel()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clearBatch() throws SQLException
	  public virtual void clearBatch()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Warning are not supported.  This is a noop.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clearWarnings() throws SQLException
	  public virtual void clearWarnings()
	  {

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws SQLException
	  public virtual void close()
	  {
		if (closed_)
		{
			return;
		}
		try
		{
		  generatedKey_ = null;
		  if (currentResultSet_ != null)
		  {
			currentResultSet_.close();
			currentResultSet_ = null;
		  }
		  attribs_.clear();
		  conn_.DatabaseConnection.deleteRequestParameterBlock(attribs_, rpbID_);
		  conn_.freeRPBID(rpbID_);

		  if (!string.ReferenceEquals(statementName_, null))
		  {
			conn_.freeStatementAndCursorNames(statementName_, cursorName_);
		  }
		  statementName_ = null;
		  cursorName_ = null;
		}
		catch (IOException io)
		{
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		}
		finally
		{
		  closed_ = true;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean execute(String sql) throws SQLException
	  public virtual bool execute(string sql)
	  {
		return execute(sql, Statement.NO_GENERATED_KEYS);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean execute(String sql, int autoGeneratedKeys) throws SQLException
	  public virtual bool execute(string sql, int autoGeneratedKeys)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (currentResultSet_ != null)
		{
		  currentResultSet_.close();
		  currentResultSet_ = null;
		}

		int statementType = getStatementType(sql);
		if (statementType == TYPE_SELECT)
		{
			currentResultSet_ = (JDBCResultSet) executeQuery(sql);
			return true;
		}


		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
		DatabaseExecuteImmediateAttributes deia = RequestAttributes; //conn_.getRequestAttributes();
		deia.SQLStatementText = sql;

		if (statementType == TYPE_CALL)
		{
		  deia.SQLStatementType = TYPE_CALL;
		  deia.OpenAttributes = 0x80; // READ
		  deia.PrepareOption = 0; // normal prepare
		}
	/*    switch (autoGeneratedKeys)
	    {
	      case Statement.NO_GENERATED_KEYS:
	        conn.setSQLCommunicationsAreaCallback(null);
	        break;
	      case Statement.RETURN_GENERATED_KEYS:
	        conn.setSQLCommunicationsAreaCallback(this);
	        break;
	      default:
	        throw new SQLException("Bad value for autoGeneratedKeys parameter");
	    }
	*/
		bool resultSetAvailable = false;
		try
		{
		  conn.CurrentRequestParameterBlockID = rpbID_;
		  generatedKey_ = null;
		  conn.executeImmediate(deia);
		  updateCount_ = lastUpdateCount_;
		  //
		  // Todo:  Need to check for result sets
		  //
		  if (resultSetsCount_ > 0)
		  {
			  resultSetAvailable = true;
			  DatabaseOpenAndDescribeAttributes oada = RequestAttributes;


			oada.OpenAttributes = 0x80;
			oada.ScrollableCursorFlag = 0;
			oada.VariableFieldCompression = 0xe8;
			  if (string.ReferenceEquals(catalog_, null))
			  {
				  catalog_ = conn_.Catalog;
			  }

			JDBCResultSetMetaData md = new JDBCResultSetMetaData(conn.Info.ServerCCSID, conn_.Calendar, catalog_);

			try
			{
			  conn.CurrentRequestParameterBlockID = rpbID_;
			  if (currentResultSet_ != null)
			  {
				currentResultSet_.close();
				currentResultSet_ = null;
			  }
			  conn.openAndDescribe(oada, md);
			}
			catch (IOException io)
			{
			  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
			}
			currentResultSet_ = new JDBCResultSet(this, md, statementName_, cursorName_, fetchSize_);
		  updateCount_ = -1;


		  }
		}
		catch (IOException io)
		{
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		}
		finally
		{
	//      conn.setSQLCommunicationsAreaCallback(null);
		}
		return resultSetAvailable;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean execute(String sql, int[] columnIndices) throws SQLException
	  public virtual bool execute(string sql, int[] columnIndices)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean execute(String sql, String[] columnNames) throws SQLException
	  public virtual bool execute(string sql, string[] columnNames)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int[] executeBatch() throws SQLException
	  public virtual int[] executeBatch()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSet executeQuery(String sql) throws SQLException
	  public virtual ResultSet executeQuery(string sql)
	  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
		if (currentResultSet_ != null)
		{
		  currentResultSet_.close();
		  currentResultSet_ = null;
		}
		if (string.ReferenceEquals(catalog_, null))
		{
			catalog_ = conn_.Catalog;
		}

		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
	//      DatabasePrepareAttributes pea = getRequestAttributes(); //conn_.getRequestAttributes();
		DatabasePrepareAndDescribeAttributes pea = RequestAttributes;
		pea.ExtendedSQLStatementText = sql;
		//      pea.setPrepareOption(0x01); // Enhanced.


		// Verify that the statement could return a result set

		int statementType = getStatementType(sql);
		switch (statementType)
		{
			case TYPE_SELECT:
				// Only request extended column descriptor for select statements (not call )
				pea.OpenAttributes = 0x80;
				pea.ExtendedColumnDescriptorOption = 0xF1;
				goto case TYPE_CALL;
			case TYPE_CALL:
				break;
			default:
				// Not a query -- throw an exception
				throw new SQLException("Not a query");
		}
		pea.SQLStatementType = statementType; // SELECT. This has to be set in order to get extended column metadata back.
		try
		{
		  conn.CurrentRequestParameterBlockID = rpbID_;
		  generatedKey_ = null;

	//        conn.prepare(pea);
		  JDBCResultSetMetaData md = new JDBCResultSetMetaData(conn.Info.ServerCCSID, conn_.Calendar, catalog_);
		  conn.prepareAndDescribe(pea, md, null); // Just a plain prepare doesn't give us extended column metadata back.


		  if (statementType == TYPE_SELECT)
		  {


				DatabaseOpenAndDescribeAttributes oada = (DatabaseOpenAndDescribeAttributes)pea;
				if (fetchSize_ > 0)
				{
					oada.BlockingFactor = fetchSize_;
				}
				oada.DescribeOption = 0xD5;
				oada.ScrollableCursorFlag = 0;
				oada.VariableFieldCompression = 0xe8;
				conn.openAndDescribe(oada, null);
				currentResultSet_ = new JDBCResultSet(this, md, statementName_, cursorName_, fetchSize_);
				updateCount_ = -1;
				return currentResultSet_;
		  }
		  else
		  {


				DatabaseExecuteAttributes dea = RequestAttributes;

				// Flags set by normal toolbox
				((DatabaseOpenAndDescribeAttributes)dea).ScrollableCursorFlag = 0;
				((DatabaseOpenAndDescribeAttributes)dea).ResultSetHoldabilityOption = 0xe8; // Y
				((DatabaseOpenAndDescribeAttributes)dea).VariableFieldCompression = 0xe8;
				if (fetchSize_ > 0)
				{
					((DatabaseOpenAndDescribeAttributes)dea).BlockingFactor = fetchSize_;
				}


				dea.SQLStatementType = JDBCStatement.TYPE_CALL;

				conn.execute(dea);


				// TODO:  Determine if result set is available from the call.  If so, then call openDescribe using the existing cursor name if it exists
				if (resultSetsCount_ > 0)
				{

						DatabaseOpenAndDescribeAttributes oada = RequestAttributes;


						oada.OpenAttributes = 0x80;
						oada.ScrollableCursorFlag = 0;
						oada.VariableFieldCompression = 0xe8;
						  if (string.ReferenceEquals(catalog_, null))
						  {
							  catalog_ = conn_.Catalog;
						  }

						md = new JDBCResultSetMetaData(conn.Info.ServerCCSID, conn_.Calendar, catalog_);

				conn.CurrentRequestParameterBlockID = rpbID_;
				if (currentResultSet_ != null)
				{
				currentResultSet_.close();
				currentResultSet_ = null;
				}
				conn.openAndDescribe(oada, md);

						currentResultSet_ = new JDBCResultSet(this, md, statementName_, cursorName_, fetchSize_);
					  updateCount_ = -1;

						return currentResultSet_;
				}
				else
				{
					// Did not return result set
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
					return null;
				}



		  }
		}
		  catch (IOException io)
		  {
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int executeUpdate(String sql) throws SQLException
	  public virtual int executeUpdate(string sql)
	  {
		return executeUpdate(sql, Statement.NO_GENERATED_KEYS);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int executeUpdate(String sql, int autoGeneratedKeys) throws SQLException
	  public virtual int executeUpdate(string sql, int autoGeneratedKeys)
	  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			if (currentResultSet_ != null)
			{
				currentResultSet_.close();
				currentResultSet_ = null;
			}
			// Check statement
			getStatementType(sql);

			DatabaseConnection conn = conn_.DatabaseConnection;
			conn.SQLCommunicationsAreaCallback = this;
			DatabasePrepareAndExecuteAttributes pea = RequestAttributes;
			pea.ExtendedSQLStatementText = sql;
			pea.OpenAttributes = 0x80;
			pea.DescribeOption = 0xD5;
			pea.ScrollableCursorFlag = 0;

			/*
			 * switch (autoGeneratedKeys) { case Statement.NO_GENERATED_KEYS:
			 * conn.setSQLCommunicationsAreaCallback(null); break; case
			 * Statement.RETURN_GENERATED_KEYS:
			 * conn.setSQLCommunicationsAreaCallback(this); break; default: throw
			 * new SQLException("Bad value for autoGeneratedKeys parameter"); }
			 */
			try
			{
				conn.CurrentRequestParameterBlockID = rpbID_;
				generatedKey_ = null;
				updateCount_ = 0;
				conn.SQLCommunicationsAreaCallback = this;
				try
				{
					conn.prepareAndExecute(pea, null);
					updateCount_ = lastUpdateCount_;
				}
				finally
				{
					// conn.setSQLCommunicationsAreaCallback(null);
				}
			}
			catch (IOException io)
			{
				throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
			}
			finally
			{
				// conn.setSQLCommunicationsAreaCallback(null);
			}
			return updateCount_;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int executeUpdate(String sql, int[] columnIndices) throws SQLException
	  public virtual int executeUpdate(string sql, int[] columnIndices)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int executeUpdate(String sql, String[] columnNames) throws SQLException
	  public virtual int executeUpdate(string sql, string[] columnNames)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Connection getConnection() throws SQLException
	  public virtual Connection Connection
	  {
		  get
		  {
			  // Return connection -- even if closed
			  // if (closed_) throw new SQLException("Statement closed");
			  //
			return conn_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getFetchDirection() throws SQLException
	  public virtual int FetchDirection
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return ResultSet.FETCH_FORWARD;
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getFetchSize() throws SQLException
	  public virtual int FetchSize
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return fetchSize_;
		  }
		  set
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			if (value < 0)
			{
				throw new SQLException("Bad value for fetch size: " + value);
			}
			fetchSize_ = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSet getGeneratedKeys() throws SQLException
	  public virtual ResultSet GeneratedKeys
	  {
		  get
		  {
			if (!string.ReferenceEquals(generatedKey_, null))
			{
			  // The SQLCA contains a 30-digit packed decimal, but I'm always going to treat
			  // this as a long, until I see a case where a generated key is bigger than a long.
				if (string.ReferenceEquals(catalog_, null))
				{
					catalog_ = conn_.Catalog;
				}
    
			  JDBCResultSetMetaData md = new JDBCResultSetMetaData(37, conn_.Calendar, catalog_);
			  md.resultSetDescription(1, 0, 0, 0, 0, 8);
			  md.fieldDescription(0, 492, 8, 0, 0, 0, 0, 0, 0); // BIGINT
			  md.fieldName(0, "GENERATED_KEY");
    
			  JDBCResultSet rs = new JDBCResultSet(this, md, null, null, 0);
			  rs.newResultData(1, 1, 8);
			  rs.newRowData(0, Conv.longToByteArray((Convert.ToInt64(generatedKey_))));
    
			  return rs;
			}
			return null;
		  }
	  }

	  /// <summary>
	  /// Retrieves the maximum number of bytes that can be returned for character and binary column values in a ResultSet object produced by this Statement object.
	  /// 
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxFieldSize() throws SQLException
	  public virtual int MaxFieldSize
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return 0;
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// Retrieves the maximum number of rows that a ResultSet object produced by this Statement object can contain. </summary>
	  /// <returns> 0 -- there is no limit
	  ///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxRows() throws SQLException
	  public virtual int MaxRows
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return 0;
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getMoreResults() throws SQLException
	  public virtual bool MoreResults
	  {
		  get
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getMoreResults(int current) throws SQLException
	  public virtual bool getMoreResults(int current)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Retrieves the number of seconds the driver will wait for a Statement object to execute </summary>
	  /// <returns> 0:  This driver does not support query timeout. .
	  ///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getQueryTimeout() throws SQLException
	  public virtual int QueryTimeout
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return 0;
		  }
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSet getResultSet() throws SQLException
	  public virtual ResultSet ResultSet
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return currentResultSet_;
		  }
	  }

	  /// <summary>
	  /// Retrieves the result set concurrency for ResultSet objects generated by this Statement object. </summary>
	  /// <returns>  ResultSet.CONCUR_READ_ONLY: This driver only supports READ_ONLY cursors.
	  ///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getResultSetConcurrency() throws SQLException
	  public virtual int ResultSetConcurrency
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return ResultSet.CONCUR_READ_ONLY;
		  }
	  }

	  /// <summary>
	  /// Retrieves the result set holdability for ResultSet objects generated by this Statement object.
	  /// returns ResultSet.HOLD_CURSORS_OVER_COMMIT
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getResultSetHoldability() throws SQLException
	  public virtual int ResultSetHoldability
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return ResultSet.HOLD_CURSORS_OVER_COMMIT;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getResultSetType() throws SQLException
	  public virtual int ResultSetType
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return ResultSet.TYPE_FORWARD_ONLY;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getUpdateCount() throws SQLException
	  public virtual int UpdateCount
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			return updateCount_;
		  }
	  }

	  /// <summary>
	  /// Not implemented, but we return null to avoid problems with existing applications
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SQLWarning getWarnings() throws SQLException
	  public virtual SQLWarning Warnings
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			  return null;
    
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCursorName(String name) throws SQLException
	  public virtual string CursorName
	  {
		  set
		  {
			throw new NotImplementedException();
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setEscapeProcessing(boolean enable) throws SQLException
	  public virtual bool EscapeProcessing
	  {
		  set
		  {
			throw new NotImplementedException();
		  }
	  }






	  public virtual bool Closed
	  {
		  get
		  {
			  return closed_;
		  }
	  }

	  protected internal virtual string CursorNameInternal
	  {
		  set
		  {
			  cursorName_ = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isPoolable() throws SQLException
	  public virtual bool Poolable
	  {
		  get
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			  return poolable_;
		  }
		  set
		  {
				if (closed_)
				{
					JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
			  poolable_ = value;
		  }
	  }


	  public virtual string Catalog
	  {
		  set
		  {
			catalog_ = value;
		  }
	  }






	}

}