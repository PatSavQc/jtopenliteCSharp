using System;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCPreparedStatement.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{


	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.database;


	public class JDBCPreparedStatement : JDBCStatement, PreparedStatement
	{
	  private JDBCParameterMetaData pmd_;
	  private int descriptorHandle_;
	  private bool returnGeneratedKeys_;
	  private int sqlStatementType_;
	  private JDBCResultSetMetaData rsmd_;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JDBCPreparedStatement(JDBCConnection conn, String sql, java.util.Calendar calendar, String statementName, String cursorName, int rpbID) throws SQLException
	  public JDBCPreparedStatement(JDBCConnection conn, string sql, DateTime calendar, string statementName, string cursorName, int rpbID) : base(conn, statementName, cursorName, rpbID)
	  {
		poolable_ = true;
		if (string.ReferenceEquals(sql, null))
		{
				 JDBCError.throwSQLException(JDBCError.EXC_SYNTAX_ERROR);
				return;
		}
		rsmd_ = null;
		// Check for null statement

		DatabaseRequestAttributes dpa = new DatabaseRequestAttributes();
		//    dpa.setDescribeOption(0xD5); // Name/alias.
		//
		// Only set the statement name and cursor name in the RPB
		//
		sqlStatementType_ = JDBCStatement.getStatementType(sql);

		statementAttributes_.SQLStatementType = sqlStatementType_;

		  dpa.SQLStatementType = sqlStatementType_;
		  dpa.PrepareOption = 0; // Normal prepare.
		  if (sqlStatementType_ == JDBCStatement.TYPE_SELECT)
		  { // Only set for select statement
			dpa.OpenAttributes = 0x80; // Read only. Otherwise blocking doesn't work.
		  }

		JDBCParameterMetaData pmd = new JDBCParameterMetaData(calendar);
		string catalog = conn_.Catalog;
		// Getting the catalog may change the current rpb for the connection.  
		// Reset it after getting back.  Otherwise the call to 
		// prepareAndDescribe may fail with a PWS0001

		DatabaseConnection databaseConn = conn_.DatabaseConnection;
		databaseConn.CurrentRequestParameterBlockID = rpbID_;
		rsmd_ = new JDBCResultSetMetaData(conn.DatabaseInfo.ServerCCSID, calendar, catalog);

		dpa.ExtendedSQLStatementText = sql;
		conn.prepareAndDescribe(dpa, rsmd_, pmd);

		int handle = -1;
			// Only change the descriptor if there are parameters available
		DatabaseChangeDescriptorAttributes cda = (DatabaseChangeDescriptorAttributes)dpa;
		  sbyte[] b = pmd.ExtendedSQLParameterMarkerDataFormat;
		  cda.ExtendedSQLParameterMarkerDataFormat = b;
		  handle = b == null ? -1 : conn.NextDescriptorHandle;
		  if (handle >= 0)
		  {
			conn.changeDescriptor(cda, handle);
		  }


		pmd_ = pmd;
		pmd_.Statement = this;
		descriptorHandle_ = handle;
	  }

	  internal virtual bool ReturnGeneratedKeys
	  {
		  set
		  {
			returnGeneratedKeys_ = value;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addBatch() throws SQLException
	  public virtual void addBatch()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clearParameters() throws SQLException
	  public virtual void clearParameters()
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		for (int i = 0; i < pmd_.ParameterCount; ++i)
		{
		  Column col = pmd_.getColumn(i);
		  col.clearValue();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean execute() throws SQLException
	  public virtual bool execute()
	  {
		bool callStatement = false;
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}


		//
		// If this is a select statement, use the executeQuery path
		//
		if (sqlStatementType_ == JDBCStatement.TYPE_SELECT)
		{ // Only set for select statement
			executeQuery();
			return true;
		}

		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
		DatabaseExecuteAttributes dea = RequestAttributes;
		// Not necessary -- part of RPB
		// dea.setPrepareStatementName(statementName_);

		// Flags set by normal toolbox
		((DatabaseOpenAndDescribeAttributes)dea).ScrollableCursorFlag = 0;
		((DatabaseOpenAndDescribeAttributes)dea).ResultSetHoldabilityOption = 0xe8; // Y
		((DatabaseOpenAndDescribeAttributes)dea).VariableFieldCompression = 0xe8; // Y


		if (statementAttributes_.SQLStatementType == JDBCStatement.TYPE_CALL)
		{ // if call
			  dea.SQLStatementType = JDBCStatement.TYPE_CALL;
			  callStatement = true;
		}



		if (pmd_.ParameterCount > 0)
		{
		   sbyte[] pmData = ExtendedParameterMarkerData;
		   dea.SQLExtendedParameterMarkerData = pmData;
		}


		try
		{

		  conn.CurrentRequestParameterBlockID = rpbID_;
		  if (currentResultSet_ != null)
		  {
			currentResultSet_.close();
			currentResultSet_ = null;
		  }
	//      conn.setSQLCommunicationsAreaCallback(returnGeneratedKeys_ ? this : null);
		  updateCount_ = 0;
		  if (descriptorHandle_ < 0)
		  {
			conn.execute(dea);
		  }
		  else
		  {
			conn.execute(dea, descriptorHandle_);
		  }
		  updateCount_ = lastUpdateCount_;

		  // TODO:  Determine if result set is available.  If so, then call openDescribe
		  if (callStatement && resultSetsCount_ > 0)
		  {
				DatabaseOpenAndDescribeAttributes oada = RequestAttributes;

				oada.OpenAttributes = 0x80;
				oada.ScrollableCursorFlag = 0;
				oada.VariableFieldCompression = 0xe8;
				JDBCResultSetMetaData md = new JDBCResultSetMetaData(conn.Info.ServerCCSID, conn_.Calendar, conn_.Catalog);

				try
				{
				  conn.CurrentRequestParameterBlockID = rpbID_;
				  if (currentResultSet_ != null)
				  {
					currentResultSet_.close();
					currentResultSet_ = null;
				  }
				  if (descriptorHandle_ < 0)
				  {
					conn.openAndDescribe(oada, md);
				  }
				  else
				  {
					conn.openAndDescribe(oada, descriptorHandle_, md);
				  }
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
		return true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSet executeQuery() throws SQLException
	  public virtual ResultSet executeQuery()
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		switch (sqlStatementType_)
		{
			case JDBCStatement.TYPE_SELECT:
				// Valid
				break;
			case JDBCStatement.TYPE_CALL:
			{
				bool result = execute();
				if (result)
				{
					ResultSet rs = ResultSet;
					if (rs == null)
					{
						throw JDBCError.getSQLException(JDBCError.EXC_CURSOR_STATE_INVALID);
					}
					else
					{
					  return rs;
					}
				}
				else
				{
					throw JDBCError.getSQLException(JDBCError.EXC_CURSOR_STATE_INVALID);
				}
			}
				goto default;
			default:
				throw JDBCError.getSQLException(JDBCError.EXC_CURSOR_STATE_INVALID);
		}

		if (currentResultSet_ != null)
		{
		  currentResultSet_.close();
		  currentResultSet_ = null;
		}


		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
		DatabaseOpenAndDescribeAttributes dea = RequestAttributes; //conn_.getRequestAttributes();
		dea.PrepareStatementName = statementName_;
		if (string.ReferenceEquals(cursorName_, null))
		{
			cursorName_ = conn_.NextCursorName;
		}
		dea.CursorName = cursorName_;
		if (fetchSize_ > 0)
		{
			dea.BlockingFactor = fetchSize_;
		}
		dea.DescribeOption = 0xD5;
		dea.ScrollableCursorFlag = 0;
		dea.VariableFieldCompression = 0xe8;

		if (descriptorHandle_ >= 0)
		{
		  sbyte[] pmData = ExtendedParameterMarkerData;
		  dea.SQLExtendedParameterMarkerData = pmData;
		}
		JDBCResultSetMetaData md = new JDBCResultSetMetaData(conn.Info.ServerCCSID, conn_.Calendar, conn_.Catalog);

		try
		{
		  conn.CurrentRequestParameterBlockID = rpbID_;
		  if (descriptorHandle_ < 0)
		  {
			conn.openAndDescribe(dea, md);
		  }
		  else
		  {
			conn.openAndDescribe(dea, descriptorHandle_, md);
		  }
		}
		catch (IOException io)
		{
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		}

		currentResultSet_ = new JDBCResultSet(this, md, statementName_, cursorName_, fetchSize_);
		return currentResultSet_;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] getExtendedParameterMarkerData() throws SQLException
	  private sbyte[] ExtendedParameterMarkerData
	  {
		  get
		  {
			const int indicatorSize = 2;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int numCols = pmd_.getParameterCount();
			int numCols = pmd_.ParameterCount;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int size = 20+(numCols*indicatorSize)+pmd_.getRowSize();
			int size = 20 + (numCols * indicatorSize) + pmd_.RowSize;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final byte[] data = new byte[size];
			sbyte[] data = new sbyte[size];
			Conv.intToByteArray(1, data, 0); // Consistency token.
			Conv.intToByteArray(1, data, 4); // Row count.
			Conv.shortToByteArray(numCols, data, 8); // Column count.
			Conv.shortToByteArray(indicatorSize, data, 10); // Indicator size.
			Conv.intToByteArray(pmd_.RowSize, data, 16); // Row size.
    
			// Indicators and data.
			int indicatorOffset = 20;
			int dataOffset = 20 + (numCols * indicatorSize);
			for (int i = 0; i < numCols; ++i)
			{
			  Column col = pmd_.getColumn(i);
			  if (col.Null)
			  {
				data[indicatorOffset] = unchecked((sbyte)0xFF);
				data[indicatorOffset + 1] = unchecked((sbyte)0xFF);
			  }
			  else
			  {
				col.convertToBytes(data, dataOffset);
			  }
			  indicatorOffset += 2;
			  dataOffset += col.Length;
			}
			return data;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws SQLException
	  public override void close()
	  {
		if (closed_)
		{
			return;
		}
		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
		try
		{
		  if (descriptorHandle_ >= 0)
		  {
			conn.deleteDescriptor(null, descriptorHandle_);
		  }
		}
		catch (IOException io)
		{
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		}
		base.close();
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int executeUpdate() throws SQLException
	  public virtual int executeUpdate()
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		DatabaseConnection conn = conn_.DatabaseConnection;
		conn.SQLCommunicationsAreaCallback = this;
		DatabaseExecuteAttributes dea = RequestAttributes;
		dea.PrepareStatementName = statementName_;

		if (pmd_.ParameterCount > 0)
		{
		  sbyte[] pmData = ExtendedParameterMarkerData;
		  dea.SQLExtendedParameterMarkerData = pmData;
		}

		if (statementAttributes_.SQLStatementType == 3)
		{ // if call
			dea.SQLStatementType = 3;
		}
		try
		{
		  conn.CurrentRequestParameterBlockID = rpbID_;
		  if (currentResultSet_ != null)
		  {
			currentResultSet_.close();
			currentResultSet_ = null;
		  }
	//      conn.setSQLCommunicationsAreaCallback(this);
		  try
		  {
			updateCount_ = 0;
			if (descriptorHandle_ < 0)
			{
			  conn.execute(dea);
			}
			else
			{
			  conn.execute(dea, descriptorHandle_);
			}
			updateCount_ = lastUpdateCount_;
		  }
		  finally
		  {
	//        conn.setSQLCommunicationsAreaCallback(null);
		  }

		  return updateCount_;
		}
		catch (IOException io)
		{
		  throw JDBCConnection.convertException(io, lastSQLCode_, lastSQLState_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSetMetaData getMetaData() throws SQLException
	  public virtual ResultSetMetaData MetaData
	  {
		  get
		  {
				if (closed_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
				}
				return rsmd_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParameterMetaData getParameterMetaData() throws SQLException
	  public virtual ParameterMetaData ParameterMetaData
	  {
		  get
		  {
			if (closed_)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
    
			return pmd_;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setArray(int parameterIndex, Array x) throws SQLException
	  public virtual void setArray(int parameterIndex, Array x)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(int parameterIndex, java.io.InputStream x, int length) throws SQLException
	  public virtual void setAsciiStream(int parameterIndex, Stream x, int length)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.setAsciiStreamValue(x, length);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBigDecimal(int parameterIndex, java.math.BigDecimal x) throws SQLException
	  public virtual void setBigDecimal(int parameterIndex, decimal x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(int parameterIndex, java.io.InputStream x, int length) throws SQLException
	  public virtual void setBinaryStream(int parameterIndex, Stream x, int length)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.setBinaryStreamValue(x, length);
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(int parameterIndex, Blob x) throws SQLException
	  public virtual void setBlob(int parameterIndex, Blob x)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBoolean(int parameterIndex, boolean x) throws SQLException
	  public virtual void setBoolean(int parameterIndex, bool x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setByte(int parameterIndex, byte x) throws SQLException
	  public virtual void setByte(int parameterIndex, sbyte x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBytes(int parameterIndex, byte[] x) throws SQLException
	  public virtual void setBytes(int parameterIndex, sbyte[] x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(int parameterIndex, java.io.Reader x, int length) throws SQLException
	  public virtual void setCharacterStream(int parameterIndex, Reader x, int length)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.setCharacterStreamValue(x, length);
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(int parameterIndex, Clob x) throws SQLException
	  public virtual void setClob(int parameterIndex, Clob x)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDate(int parameterIndex, Date x) throws SQLException
	  public virtual void setDate(int parameterIndex, DateTime x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDate(int parameterIndex, Date x, java.util.Calendar cal) throws SQLException
	  public virtual void setDate(int parameterIndex, DateTime x, DateTime cal)
	  {
			if (closed_)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			if (cal == null)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_TYPE_INVALID, "cal is null");
			}

			Column col = pmd_.getColumn(parameterIndex - 1);
			col.setValue(x, cal);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDouble(int parameterIndex, double x) throws SQLException
	  public virtual void setDouble(int parameterIndex, double x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFloat(int parameterIndex, float x) throws SQLException
	  public virtual void setFloat(int parameterIndex, float x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInt(int parameterIndex, int x) throws SQLException
	  public virtual void setInt(int parameterIndex, int x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLong(int parameterIndex, long x) throws SQLException
	  public virtual void setLong(int parameterIndex, long x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNull(int parameterIndex, int sqlType) throws SQLException
	  public virtual void setNull(int parameterIndex, int sqlType)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Null = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNull(int parameterIndex, int sqlType, String typeName) throws SQLException
	  public virtual void setNull(int parameterIndex, int sqlType, string typeName)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Null = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(int parameterIndex, Object x) throws SQLException
	  public virtual void setObject(int parameterIndex, object x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(int parameterIndex, Object x, int targetSQLType) throws SQLException
	  public virtual void setObject(int parameterIndex, object x, int targetSQLType)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(int parameterIndex, Object x, int targetSQLType, int scale) throws SQLException
	  public virtual void setObject(int parameterIndex, object x, int targetSQLType, int scale)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRef(int parameterIndex, Ref x) throws SQLException
	  public virtual void setRef(int parameterIndex, Ref x)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setShort(int parameterIndex, short x) throws SQLException
	  public virtual void setShort(int parameterIndex, short x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setString(int parameterIndex, String x) throws SQLException
	  public virtual void setString(int parameterIndex, string x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTime(int parameterIndex, Time x) throws SQLException
	  public virtual void setTime(int parameterIndex, Time x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTime(int parameterIndex, Time x, java.util.Calendar cal) throws SQLException
	  public virtual void setTime(int parameterIndex, Time x, DateTime cal)
	  {
			if (closed_)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			if (cal == null)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_ATTRIBUTE_VALUE_INVALID, "cal is null");
			}
			Column col = pmd_.getColumn(parameterIndex - 1);
			col.setValue(x, cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTimestamp(int parameterIndex, Timestamp x) throws SQLException
	  public virtual void setTimestamp(int parameterIndex, Timestamp x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTimestamp(int parameterIndex, Timestamp x, java.util.Calendar cal) throws SQLException
	  public virtual void setTimestamp(int parameterIndex, Timestamp x, DateTime cal)
	  {
			if (closed_)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			if (cal == null)
			{
				throw JDBCError.getSQLException(JDBCError.EXC_ATTRIBUTE_VALUE_INVALID, "cal is null");
			}
			Column col = pmd_.getColumn(parameterIndex - 1);
			col.setValue(x, cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUnicodeStream(int parameterIndex, java.io.InputStream x, int length) throws SQLException
	  public virtual void setUnicodeStream(int parameterIndex, Stream x, int length)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		Column col = pmd_.getColumn(parameterIndex - 1);
		col.setUnicodeStreamValue(x, length);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setURL(int parameterIndex, java.net.URL x) throws SQLException
	  public virtual void setURL(int parameterIndex, Uri x)
	  {
		if (closed_)
		{
			throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		Column col = pmd_.getColumn(parameterIndex - 1);
		col.Value = x;
	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(int parameterIndex, java.io.InputStream arg1) throws SQLException
	  public virtual void setAsciiStream(int parameterIndex, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(int parameterIndex, java.io.InputStream arg1, long arg2) throws SQLException
	  public virtual void setAsciiStream(int parameterIndex, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(int parameterIndex, java.io.InputStream arg1) throws SQLException
	  public virtual void setBinaryStream(int parameterIndex, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(int parameterIndex, java.io.InputStream arg1, long arg2) throws SQLException
	  public virtual void setBinaryStream(int parameterIndex, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(int parameterIndex, java.io.InputStream arg1) throws SQLException
	  public virtual void setBlob(int parameterIndex, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(int parameterIndex, java.io.InputStream arg1, long arg2) throws SQLException
	  public virtual void setBlob(int parameterIndex, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(int parameterIndex, java.io.Reader arg1) throws SQLException
	  public virtual void setCharacterStream(int parameterIndex, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(int parameterIndex, java.io.Reader arg1, long arg2) throws SQLException
	  public virtual void setCharacterStream(int parameterIndex, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(int parameterIndex, java.io.Reader arg1) throws SQLException
	  public virtual void setClob(int parameterIndex, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(int parameterIndex, java.io.Reader arg1, long arg2) throws SQLException
	  public virtual void setClob(int parameterIndex, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNCharacterStream(int parameterIndex, java.io.Reader arg1) throws SQLException
	  public virtual void setNCharacterStream(int parameterIndex, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNCharacterStream(int parameterIndex, java.io.Reader arg1, long arg2) throws SQLException
	  public virtual void setNCharacterStream(int parameterIndex, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }




	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNClob(int parameterIndex, java.io.Reader arg1) throws SQLException
	  public virtual void setNClob(int parameterIndex, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNClob(int parameterIndex, java.io.Reader arg1, long arg2) throws SQLException
	  public virtual void setNClob(int parameterIndex, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parameterIndex"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNString(int parameterIndex, String arg1) throws SQLException
	  public virtual void setNString(int parameterIndex, string arg1)
	  {
		throw new NotImplementedException();

	  }


	}

}