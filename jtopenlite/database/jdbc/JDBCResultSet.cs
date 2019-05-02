using System;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCResultSet.java
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

	/// <summary>
	/// Result sets created by this JDBC driver are FORWARD-ONLY and READ-ONLY;
	/// getArray(), getObject(), and getRef() are not supported;
	/// and LOBs and LOB locators have not been extensively tested.
	/// 
	/// </summary>
	public class JDBCResultSet : ResultSet, DatabaseFetchCallback
	{
		  // Fetch scroll options, for reference.
		  /*
		  private static int NEXT =         0x0000;
		  private static int PREVIOUS =     0x0001;
		  private static int FIRST =        0x0002;
		  private static int LAST =         0x0003;
		  private static int BEFORE_FIRST = 0x0004;
		  private static int AFTER_LAST =   0x0005;
		  private static int CURRENT =      0x0006;
		  private static int RELATIVE =     0x0007;
		  private static int DIRECT =       0x0008;
		  */

	  private JDBCStatement statement_;
	  private JDBCResultSetMetaData md_;
	  private string stName_;
	  private string cursorName_;

	  private int fetchSize_;
	  private bool closed_;
	  private bool afterLast_ = false;

	  private readonly DataCache dataCache_ = new DataCache();

	  private int currentRow_ = 0;
	  private bool lastNull_ = false;

	  private sbyte[] tempDataBuffer_;

	  protected internal bool isMetadataResultSet_ = false;

	  public JDBCResultSet(JDBCStatement statement, JDBCResultSetMetaData md, string statementName, string cursorName, int fetchSize)
	  {
		statement_ = statement;
		md_ = md;
		stName_ = statementName;
		cursorName_ = cursorName;
		fetchSize_ = fetchSize;
	  }

	  public virtual sbyte[] getTempDataBuffer(int rowSize)
	  {
		if (tempDataBuffer_ == null || tempDataBuffer_.Length < rowSize)
		{
		  tempDataBuffer_ = new sbyte[rowSize];
		}
		return tempDataBuffer_;
	  }

	  public virtual void newResultData(int rowCount, int columnCount, int rowSize)
	  {
		dataCache_.init(rowCount, columnCount, rowSize);
		if (tempDataBuffer_ == null || tempDataBuffer_.Length < rowSize)
		{
		  tempDataBuffer_ = new sbyte[rowSize];
		}
	  }

	  public virtual void newIndicator(int row, int column, sbyte[] tempIndicatorData)
	  {
		int i = Conv.byteArrayToShort(tempIndicatorData, 0);
		bool isNull = i == -1;
		dataCache_.setNull(row, column, isNull);
	  }

	  public virtual void newRowData(int row, sbyte[] tempData)
	  {
		dataCache_.setRow(row, tempData);
	  }

	  ////////////////////////////
	  //
	  // ResultSet methods
	  //
	  ////////////////////////////


	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean absolute(int row) throws SQLException
	  public virtual bool absolute(int row)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterLast() throws SQLException
	  public virtual void afterLast()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Positiong cursor not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void beforeFirst() throws SQLException
	  public virtual void beforeFirst()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void cancelRowUpdates() throws SQLException
	  public virtual void cancelRowUpdates()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clearWarnings() throws SQLException
	  public virtual void clearWarnings()
	  {
		  // No errors from unimplemented clear warnings
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws SQLException
	  public virtual void close()
	  {
		try
		{
		  if (string.ReferenceEquals(cursorName_, null))
		  {
			  return;
		  }
		  if (!closed_)
		  {
			DatabaseConnection conn = statement_.DatabaseConnection;
			DatabaseCloseCursorAttributes cca = statement_.RequestAttributes;
			cca.CursorName = cursorName_;
			try
			{
			  conn.CurrentRequestParameterBlockID = statement_.rpbID_;
			  conn.closeCursor(cca);
			  //
			  // 04/11/2012 -- Not sure why this is reset.  We want to keep the default RPM with
			  //            -- the cursor name and statement name
			  //
			  // DatabaseCreateRequestParameterBlockAttributes rpba = statement_.getRequestAttributes();
			  // conn.resetRequestParameterBlock(rpba, statement_.rpbID_);
			  //
			}
			catch (IOException io)
			{
			  throw JDBCConnection.convertException(io, statement_.LastSQLCode, statement_.LastSQLState);
			}
			closed_ = true; // Mark as closed before calling statement close
			if (isMetadataResultSet_)
			{
				statement_.close();
			}
		  }
		}
		finally
		{
		  closed_ = true;
		  statement_ = null;
		  md_ = null;
		}
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deleteRow() throws SQLException
	  public virtual void deleteRow()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int findColumn(String columnName) throws SQLException
	  public virtual int findColumn(string columnName)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		int columnIndex = md_.getColumnIndex(columnName) + 1;
		if (columnIndex <= 0)
		{
			 throw new SQLException("Column not found", "42703", -206);
		}
		return columnIndex;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean first() throws SQLException
	  public virtual bool first()
	  {
		throw new NotImplementedException();
	  }

	  /// 
	  /// 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getConcurrency() throws SQLException
	  public virtual int Concurrency
	  {
		  get
		  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			// We currently only support READONLY cursors
			return ResultSet.CONCUR_READ_ONLY;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCursorName() throws SQLException
	  public virtual string CursorName
	  {
		  get
		  {
    
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
			return cursorName_;
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
				if (value != ResultSet.FETCH_FORWARD)
				{
			throw new NotImplementedException();
				}
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
//ORIGINAL LINE: public ResultSetMetaData getMetaData() throws SQLException
	  public virtual ResultSetMetaData MetaData
	  {
		  get
		  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
    
			return md_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getRow() throws SQLException
	  public virtual int Row
	  {
		  get
		  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
    
			return currentRow_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Statement getStatement() throws SQLException
	  public virtual Statement Statement
	  {
		  get
		  {
			if (isMetadataResultSet_)
			{
			  // Do not expose statement objects for result set metadata
			  return null;
			}
			return statement_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getType() throws SQLException
	  public virtual int Type
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

	  /// <summary>
	  /// For the jtopenlite driver, no warnings will ever be reported
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
//ORIGINAL LINE: public void insertRow() throws SQLException
	  public virtual void insertRow()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isAfterLast() throws SQLException
	  public virtual bool AfterLast
	  {
		  get
		  {
			  if (closed_)
			  {
				  JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			  }
			throw new NotImplementedException();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isBeforeFirst() throws SQLException
	  public virtual bool BeforeFirst
	  {
		  get
		  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
    
			return currentRow_ <= 0;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isFirst() throws SQLException
	  public virtual bool First
	  {
		  get
		  {
			if (closed_)
			{
				JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
			}
    
			return currentRow_ == 1;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isLast() throws SQLException
	  public virtual bool Last
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
//ORIGINAL LINE: public boolean last() throws SQLException
	  public virtual bool last()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveToCurrentRow() throws SQLException
	  public virtual void moveToCurrentRow()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveToInsertRow() throws SQLException
	  public virtual void moveToInsertRow()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean next() throws SQLException
	  public virtual bool next()
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}


		// If our data cache is empty, or if our cache pointer is at the end.
		if (dataCache_.nextRow() >= dataCache_.NumRows)
		{
		  if (!fetch() || dataCache_.nextRow() >= dataCache_.NumRows)
		  {
			return false;
		  }
		}

		++currentRow_;
		return true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean fetch() throws SQLException
	  private bool fetch()
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (string.ReferenceEquals(cursorName_, null))
		{
			return false;
		}

		Message lastWarning = ((JDBCConnection)statement_.Connection).LastWarningMessage;
		if (lastWarning != null)
		{
		  if (lastWarning.ID.Equals("SQL0100"))
		  {
			// Row not found.
			afterLast_ = true;
			return false;
		  }
		}

		DatabaseConnection conn = statement_.DatabaseConnection;
		  DatabaseFetchAttributes fa = statement_.RequestAttributes;
		  fa.CursorName = cursorName_;
		  fa.setFetchScrollOption(0,0); // Next.
		  // TODO:  Get variable field compression working
		  fa.VariableFieldCompression = 0xE8;
		  if (fetchSize_ > 0)
		  {
			fa.BlockingFactor = fetchSize_;
		  }
		  else
		  {
			fa.FetchBufferSize = 256 * 1024;
		  }
		  try
		  {
			conn.CurrentRequestParameterBlockID = statement_.rpbID_;
			conn.fetch(fa, this);
			return true;
		  }
		  catch (IOException io)
		  {
			// io.printStackTrace();
			throw JDBCConnection.convertException(io, statement_.LastSQLCode, statement_.LastSQLState);
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean previous() throws SQLException
	  public virtual bool previous()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void refreshRow() throws SQLException
	  public virtual void refreshRow()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean relative(int rows) throws SQLException
	  public virtual bool relative(int rows)
	  {
		if (rows < 0)
		{
			throw new SQLException("Result set is forward only.");
		}
		for (int i = 0; i < rows; ++i)
		{
		  if (!next())
		  {
			return false;
		  }
		}
		return true;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rowDeleted() throws SQLException
	  public virtual bool rowDeleted()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rowInserted() throws SQLException
	  public virtual bool rowInserted()
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rowUpdated() throws SQLException
	  public virtual bool rowUpdated()
	  {
		throw new NotImplementedException();
	  }



	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateRow() throws SQLException
	  public virtual void updateRow()
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean wasNull() throws SQLException
	  public virtual bool wasNull()
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		return lastNull_;
	  }


	  ////////////////////////////
	  //
	  // ResultSet data getter methods
	  //
	  ////////////////////////////

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array getArray(int i) throws SQLException
	  public virtual Array getArray(int i)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array getArray(String colName) throws SQLException
	  public virtual Array getArray(string colName)
	  {
		return getArray(findColumn(colName));
	  }

	  /// <summary>
	  /// getAsciiStream is implemented as simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getAsciiStream(int i) throws SQLException
	  public virtual Stream getAsciiStream(int i)
	  {
		  try
		  {
		  string s = getString(i);
		  if (string.ReferenceEquals(s, null))
		  {
			  return null;
		  }
		  return new MemoryStream(s.GetBytes("ISO8859_1"));
		  }
		  catch (UnsupportedEncodingException e)
		  {
			SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			sqlex.initCause(e);
			throw sqlex;

		  }

	  }

	  /// <summary>
	  /// getAsciiStream is implemented as simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getAsciiStream(String colName) throws SQLException
	  public virtual Stream getAsciiStream(string colName)
	  {
		return getAsciiStream(findColumn(colName));

	  }

	  /// <summary>
	  /// Implemented as simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(int i) throws SQLException
	  public virtual decimal getBigDecimal(int i)
	  {
		  try
		  {
		  string s = getString(i);
		  if (string.ReferenceEquals(s, null))
		  {
			  return null;
		  }
			 return new decimal(s.Trim());
		  }
		  catch (System.FormatException e)
		  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  sqlex.initCause(e);
			  throw sqlex;
		  }
	  }

	  /// <summary>
	  /// Implemented as simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(String colName) throws SQLException
	  public virtual decimal getBigDecimal(string colName)
	  {
		return getBigDecimal(findColumn(colName));
	  }

	  /// <summary>
	  /// Implemented as simple wrapper around getString
	  /// @deprecated
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(int i, int scale) throws SQLException
	  public virtual decimal getBigDecimal(int i, int scale)
	  {
		  try
		  {
			if (scale < 0)
			{
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  throw sqlex;

			}
		  string s = getString(i);
		  if (string.ReferenceEquals(s, null))
		  {
			  return null;
		  }

			 decimal value = new decimal(s.Trim());
		 return value.setScale(scale, decimal.ROUND_DOWN);
		  }
		  catch (System.FormatException e)
		  {
			SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			sqlex.initCause(e);
			throw sqlex;
		  }

	  }

	  /// <summary>
	  /// Implemented as simple wrapper around getString
	  /// @deprecated
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(String colName, int scale) throws SQLException
	  public virtual decimal getBigDecimal(string colName, int scale)
	  {
		return getBigDecimal(findColumn(colName), scale);

	  }

	  /// <summary>
	  /// This is a ByteArrayInputStream wrapper around getBytes().
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getBinaryStream(int i) throws SQLException
	  public virtual Stream getBinaryStream(int i)
	  {
		sbyte[] b = getBytes(i);
		return b == null ? null : new MemoryStream(b);
	  }

	  /// <summary>
	  /// This is a ByteArrayInputStream wrapper around getBytes().
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getBinaryStream(String colName) throws SQLException
	  public virtual Stream getBinaryStream(string colName)
	  {
		sbyte[] b = getBytes(colName);
		return b == null ? null : new MemoryStream(b);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Blob getBlob(int i) throws SQLException
	  public virtual Blob getBlob(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToBlob(dataCache_.Data, dataCache_.RowOffset, (JDBCConnection)statement_.Connection);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Blob getBlob(String colName) throws SQLException
	  public virtual Blob getBlob(string colName)
	  {
		return getBlob(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(int i) throws SQLException
	  public virtual bool getBoolean(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return false;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToBoolean(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(String colName) throws SQLException
	  public virtual bool getBoolean(string colName)
	  {
		return getBoolean(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte getByte(int i) throws SQLException
	  public virtual sbyte getByte(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToByte(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte getByte(String colName) throws SQLException
	  public virtual sbyte getByte(string colName)
	  {
		return getByte(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(int i) throws SQLException
	  public virtual sbyte[] getBytes(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}

		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToOutputBytes(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(String colName) throws SQLException
	  public virtual sbyte[] getBytes(string colName)
	  {
		return getBytes(findColumn(colName));
	  }

	  /// <summary>
	  /// getCharacterStream is a simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reader getCharacterStream(int i) throws SQLException
	  public virtual Reader getCharacterStream(int i)
	  {
		  string s = getString(i);
		  if (string.ReferenceEquals(s, null))
		  {
			  return null;
		  }

		return new java.io.StringReader(s);
	  }

	  /// <summary>
	  /// getCharacterStream is a simple wrapper around getString
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reader getCharacterStream(String colName) throws SQLException
	  public virtual Reader getCharacterStream(string colName)
	  {
		return getCharacterStream(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Clob getClob(int i) throws SQLException
	  public virtual Clob getClob(int i)
	  {
		if (closed_)
		{
			throw new SQLException("ResultSet closed");
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToClob(dataCache_.Data, dataCache_.RowOffset, (JDBCConnection)statement_.Connection);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Clob getClob(String colName) throws SQLException
	  public virtual Clob getClob(string colName)
	  {
		if (closed_)
		{
			throw new SQLException("ResultSet closed");
		}
		if (dataCache_.isNull(md_.getColumnIndex(colName)))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(colName);
		return col.convertToClob(dataCache_.Data, dataCache_.RowOffset, (JDBCConnection)statement_.Connection);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Date getDate(int i) throws SQLException
	  public virtual DateTime getDate(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		return getDate(i, ((JDBCConnection)statement_.Connection).Calendar);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Date getDate(String colName) throws SQLException
	  public virtual DateTime getDate(string colName)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		return getDate(colName, ((JDBCConnection)statement_.Connection).Calendar);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Date getDate(int i, java.util.Calendar cal) throws SQLException
	  public virtual DateTime getDate(int i, DateTime cal)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (cal == null)
		{
			JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		}

		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToDate(dataCache_.Data, dataCache_.RowOffset, cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Date getDate(String colName, java.util.Calendar cal) throws SQLException
	  public virtual DateTime getDate(string colName, DateTime cal)
	  {
		return getDate(findColumn(colName), cal);
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(int i) throws SQLException
	  public virtual double getDouble(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToDouble(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(String colName) throws SQLException
	  public virtual double getDouble(string colName)
	  {
		return getDouble(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float getFloat(int i) throws SQLException
	  public virtual float getFloat(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToFloat(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float getFloat(String colName) throws SQLException
	  public virtual float getFloat(string colName)
	  {
		return getFloat(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(int i) throws SQLException
	  public virtual int getInt(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToInt(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(String colName) throws SQLException
	  public virtual int getInt(string colName)
	  {
		return getInt(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(int i) throws SQLException
	  public virtual long getLong(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToLong(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(String colName) throws SQLException
	  public virtual long getLong(string colName)
	  {
		return getLong(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(int i) throws SQLException
	  public virtual object getObject(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToObject(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(String colName) throws SQLException
	  public virtual object getObject(string colName)
	  {
		return getObject(findColumn(colName));
	  }

	//  public Object getObject(int i, Map<String,Class<?>> map) throws SQLException
	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(int i, java.util.Map map) throws SQLException
	  public virtual object getObject(int i, System.Collections.IDictionary map)
	  {
		throw new NotImplementedException();
	  }


	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Ref getRef(int i) throws SQLException
	  public virtual Ref getRef(int i)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Ref getRef(String colName) throws SQLException
	  public virtual Ref getRef(string colName)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short getShort(int i) throws SQLException
	  public virtual short getShort(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return 0;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToShort(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short getShort(String colName) throws SQLException
	  public virtual short getShort(string colName)
	  {
		return getShort(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(int i) throws SQLException
	  public virtual string getString(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToString(dataCache_.Data, dataCache_.RowOffset);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(String colName) throws SQLException
	  public virtual string getString(string colName)
	  {
		return getString(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Time getTime(int i) throws SQLException
	  public virtual Time getTime(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		return getTime(i, ((JDBCConnection)statement_.Connection).Calendar);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Time getTime(String colName) throws SQLException
	  public virtual Time getTime(string colName)
	  {
		return getTime(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Time getTime(int i, java.util.Calendar cal) throws SQLException
	  public virtual Time getTime(int i, DateTime cal)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (cal == null)
		{
			JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToTime(dataCache_.Data, dataCache_.RowOffset, cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Time getTime(String colName, java.util.Calendar cal) throws SQLException
	  public virtual Time getTime(string colName, DateTime cal)
	  {
		return getTime(findColumn(colName), cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Timestamp getTimestamp(int i) throws SQLException
	  public virtual Timestamp getTimestamp(int i)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		return getTimestamp(i, ((JDBCConnection)statement_.Connection).Calendar);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Timestamp getTimestamp(String colName) throws SQLException
	  public virtual Timestamp getTimestamp(string colName)
	  {
		return getTimestamp(findColumn(colName));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Timestamp getTimestamp(int i, java.util.Calendar cal) throws SQLException
	  public virtual Timestamp getTimestamp(int i, DateTime cal)
	  {
		if (closed_)
		{
			JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		}
		if (cal == null)
		{
			JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		}
		if (dataCache_.isNull(i - 1))
		{
		  lastNull_ = true;
		  return null;
		}
		lastNull_ = false;
		Column col = md_.getColumn(i - 1);
		return col.convertToTimestamp(dataCache_.Data, dataCache_.RowOffset, cal);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Timestamp getTimestamp(String colName, java.util.Calendar cal) throws SQLException
	  public virtual Timestamp getTimestamp(string colName, DateTime cal)
	  {
		return getTimestamp(findColumn(colName), cal);
	  }

	  /// <summary>
	  /// This a ByteArrayInputStream wrapper around getString().getBytes("UTF-16").
	  /// @deprecated
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getUnicodeStream(int i) throws SQLException
	  public virtual Stream getUnicodeStream(int i)
	  {
		string s = getString(i);
		try
		{
		  return string.ReferenceEquals(s, null) ? null : new MemoryStream(s.GetBytes("UTF-16"));
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

	  /// <summary>
	  /// This a ByteArrayInputStream wrapper around getString().getBytes("UTF-16").
	  /// @deprecated
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getUnicodeStream(String colName) throws SQLException
	  public virtual Stream getUnicodeStream(string colName)
	  {
		string s = getString(colName);
		try
		{
		  return string.ReferenceEquals(s, null) ? null : new MemoryStream(s.GetBytes("UTF-16"));
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getURL(int i) throws SQLException
	  public virtual Uri getURL(int i)
	  {
		string s = getString(i);
		if (string.ReferenceEquals(s, null))
		{
			return null;
		}
		try
		{
		  return new URL(s);
		}
		catch (MalformedURLException e)
		{
		  SQLException sql = new SQLException("Data conversion error");
		  sql.initCause(e);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getURL(String colName) throws SQLException
	  public virtual Uri getURL(string colName)
	  {
		return getURL(findColumn(colName));
	  }


	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateArray(int i, Array x) throws SQLException
	  public virtual void updateArray(int i, Array x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateArray(String colName, Array x) throws SQLException
	  public virtual void updateArray(string colName, Array x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(int i, InputStream x, int length) throws SQLException
	  public virtual void updateAsciiStream(int i, Stream x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(String colName, InputStream x, int length) throws SQLException
	  public virtual void updateAsciiStream(string colName, Stream x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBigDecimal(int i, java.math.BigDecimal x) throws SQLException
	  public virtual void updateBigDecimal(int i, decimal x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBigDecimal(String colName, java.math.BigDecimal x) throws SQLException
	  public virtual void updateBigDecimal(string colName, decimal x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(int i, InputStream x, int length) throws SQLException
	  public virtual void updateBinaryStream(int i, Stream x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(String colName, InputStream x, int length) throws SQLException
	  public virtual void updateBinaryStream(string colName, Stream x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(int i, Blob x) throws SQLException
	  public virtual void updateBlob(int i, Blob x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(String colName, Blob x) throws SQLException
	  public virtual void updateBlob(string colName, Blob x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBoolean(int i, boolean x) throws SQLException
	  public virtual void updateBoolean(int i, bool x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBoolean(String colName, boolean x) throws SQLException
	  public virtual void updateBoolean(string colName, bool x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateByte(int i, byte x) throws SQLException
	  public virtual void updateByte(int i, sbyte x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateByte(String colName, byte x) throws SQLException
	  public virtual void updateByte(string colName, sbyte x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBytes(int i, byte[] x) throws SQLException
	  public virtual void updateBytes(int i, sbyte[] x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBytes(String colName, byte[] x) throws SQLException
	  public virtual void updateBytes(string colName, sbyte[] x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(int i, Reader x, int length) throws SQLException
	  public virtual void updateCharacterStream(int i, Reader x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(String colName, Reader x, int length) throws SQLException
	  public virtual void updateCharacterStream(string colName, Reader x, int length)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(int i, Clob x) throws SQLException
	  public virtual void updateClob(int i, Clob x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(String colName, Clob x) throws SQLException
	  public virtual void updateClob(string colName, Clob x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateDate(int i, Date x) throws SQLException
	  public virtual void updateDate(int i, DateTime x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateDate(String colName, Date x) throws SQLException
	  public virtual void updateDate(string colName, DateTime x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateDouble(int i, double x) throws SQLException
	  public virtual void updateDouble(int i, double x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateDouble(String colName, double x) throws SQLException
	  public virtual void updateDouble(string colName, double x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateFloat(int i, float x) throws SQLException
	  public virtual void updateFloat(int i, float x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateFloat(String colName, float x) throws SQLException
	  public virtual void updateFloat(string colName, float x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateInt(int i, int x) throws SQLException
	  public virtual void updateInt(int i, int x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateInt(String colName, int x) throws SQLException
	  public virtual void updateInt(string colName, int x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateLong(int i, long x) throws SQLException
	  public virtual void updateLong(int i, long x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateLong(String colName, long x) throws SQLException
	  public virtual void updateLong(string colName, long x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNull(int i) throws SQLException
	  public virtual void updateNull(int i)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNull(String colName) throws SQLException
	  public virtual void updateNull(string colName)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateObject(int i, Object x) throws SQLException
	  public virtual void updateObject(int i, object x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateObject(String colName, Object x) throws SQLException
	  public virtual void updateObject(string colName, object x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateObject(int i, Object x, int scale) throws SQLException
	  public virtual void updateObject(int i, object x, int scale)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateObject(String colName, Object x, int scale) throws SQLException
	  public virtual void updateObject(string colName, object x, int scale)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateRef(int i, Ref x) throws SQLException
	  public virtual void updateRef(int i, Ref x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateRef(String colName, Ref x) throws SQLException
	  public virtual void updateRef(string colName, Ref x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateShort(int i, short x) throws SQLException
	  public virtual void updateShort(int i, short x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateShort(String colName, short x) throws SQLException
	  public virtual void updateShort(string colName, short x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateString(int i, String x) throws SQLException
	  public virtual void updateString(int i, string x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateString(String colName, String x) throws SQLException
	  public virtual void updateString(string colName, string x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateTime(int i, Time x) throws SQLException
	  public virtual void updateTime(int i, Time x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateTime(String colName, Time x) throws SQLException
	  public virtual void updateTime(string colName, Time x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateTimestamp(int i, Timestamp x) throws SQLException
	  public virtual void updateTimestamp(int i, Timestamp x)
	  {
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateTimestamp(String colName, Timestamp x) throws SQLException
	  public virtual void updateTimestamp(string colName, Timestamp x)
	  {
		throw new NotImplementedException();
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getHoldability() throws SQLException
	  public virtual int Holdability
	  {
		  get
		  {
			// Same as the statement, exception for stored procedure result sets
			return statement_.ResultSetHoldability;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reader getNCharacterStream(int columnIndex) throws SQLException
	  public virtual Reader getNCharacterStream(int columnIndex)
	  {
		return getCharacterStream(columnIndex);
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reader getNCharacterStream(String columnName) throws SQLException
	  public virtual Reader getNCharacterStream(string columnName)
	  {
		return getNCharacterStream(findColumn(columnName));
	  }




//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNString(int columnIndex) throws SQLException
	  public virtual string getNString(int columnIndex)
	  {
		return getString(columnIndex);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNString(String columnName) throws SQLException
	  public virtual string getNString(string columnName)
	  {
		return getNString(findColumn(columnName));
	  }






//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isClosed() throws SQLException
	  public virtual bool Closed
	  {
		  get
		  {
			return closed_;
		  }
	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(int arg0, InputStream arg1) throws SQLException
	  public virtual void updateAsciiStream(int arg0, Stream arg1)
	  {
		throw new NotImplementedException();
	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(String arg0, InputStream arg1) throws SQLException
	  public virtual void updateAsciiStream(string arg0, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(int arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateAsciiStream(int arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateAsciiStream(String arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateAsciiStream(string arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(int arg0, InputStream arg1) throws SQLException
	  public virtual void updateBinaryStream(int arg0, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(String arg0, InputStream arg1) throws SQLException
	  public virtual void updateBinaryStream(string arg0, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(int arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateBinaryStream(int arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBinaryStream(String arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateBinaryStream(string arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(int arg0, InputStream arg1) throws SQLException
	  public virtual void updateBlob(int arg0, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(String arg0, InputStream arg1) throws SQLException
	  public virtual void updateBlob(string arg0, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(int arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateBlob(int arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateBlob(String arg0, InputStream arg1, long arg2) throws SQLException
	  public virtual void updateBlob(string arg0, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(int arg0, Reader arg1) throws SQLException
	  public virtual void updateCharacterStream(int arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(String arg0, Reader arg1) throws SQLException
	  public virtual void updateCharacterStream(string arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(int arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateCharacterStream(int arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCharacterStream(String arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateCharacterStream(string arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(int arg0, Reader arg1) throws SQLException
	  public virtual void updateClob(int arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(String arg0, Reader arg1) throws SQLException
	  public virtual void updateClob(string arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(int arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateClob(int arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateClob(String arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateClob(string arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNCharacterStream(int arg0, Reader arg1) throws SQLException
	  public virtual void updateNCharacterStream(int arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNCharacterStream(String arg0, Reader arg1) throws SQLException
	  public virtual void updateNCharacterStream(string arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNCharacterStream(int arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateNCharacterStream(int arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNCharacterStream(String arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateNCharacterStream(string arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }




	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNClob(int arg0, Reader arg1) throws SQLException
	  public virtual void updateNClob(int arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNClob(String arg0, Reader arg1) throws SQLException
	  public virtual void updateNClob(string arg0, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNClob(int arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateNClob(int arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNClob(String arg0, Reader arg1, long arg2) throws SQLException
	  public virtual void updateNClob(string arg0, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNString(int arg0, String arg1) throws SQLException
	  public virtual void updateNString(int arg0, string arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0"> </param>
	  /// <param name="arg1">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateNString(String arg0, String arg1) throws SQLException
	  public virtual void updateNString(string arg0, string arg1)
	  {
		throw new NotImplementedException();

	  }





	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isWrapperFor(Class arg0) throws SQLException
	  public virtual bool isWrapperFor(Type arg0)
	  {
		throw new NotImplementedException();
	  }


	  /// <summary>
	  /// Not implemented. </summary>
	  /// <param name="arg0">
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T unwrap(Class<T> arg0) throws SQLException
	  public virtual T unwrap<T>(Type<T> arg0)
	  {
		throw new NotImplementedException();
	  }






//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(String columnLabel, java.util.Map map) throws SQLException
	  public virtual object getObject(string columnLabel, System.Collections.IDictionary map)
	  {
		return getObject(findColumn(columnLabel), map);
	  }
	}


}