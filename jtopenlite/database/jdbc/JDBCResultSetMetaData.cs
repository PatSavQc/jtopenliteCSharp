using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCResultSetMetaData.java
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

	public class JDBCResultSetMetaData : ResultSetMetaData, DatabaseDescribeCallback
	{
	  private Column[] columns_;
	  private int offset_;
	  private string catalog_;

	  internal readonly int serverCCSID_;

	  private readonly DateTime calendar_;

	  public JDBCResultSetMetaData(int serverCCSID, DateTime calendarUsedForConversions, string catalog)
	  {
		serverCCSID_ = serverCCSID;
		calendar_ = calendarUsedForConversions;
		catalog_ = catalog;
	  }

	  public virtual void resultSetDescription(int numFields, int dateFormat, int timeFormat, int dateSeparator, int timeSeparator, int recordSize)
	  {
		columns_ = new Column[numFields];
		for (int i = 0; i < numFields; ++i)
		{
		  columns_[i] = new Column(calendar_, i + 1, false);
		  columns_[i].DateFormat = dateFormat;
		  columns_[i].TimeFormat = timeFormat;
		  columns_[i].DateSeparator = dateSeparator;
		  columns_[i].TimeSeparator = timeSeparator;
		}
		offset_ = 0;
	  }

	  public virtual void fieldDescription(int fieldIndex, int type, int length, int scale, int precision, int ccsid, int joinRefPosition, int attributeBitmap, int lobMaxSize)
	  {
		columns_[fieldIndex].Type = type;
		columns_[fieldIndex].Length = length;
		columns_[fieldIndex].Scale = scale;
		columns_[fieldIndex].Precision = precision;
		// Looks like this was set to translate binary.. Keep this out for now...
		// A translate binary property could be added later.
		// columns_[fieldIndex].setCCSID(ccsid == 65535 ? serverCCSID_ : ccsid);
		columns_[fieldIndex].CCSID = ccsid;
		columns_[fieldIndex].Offset = offset_;
		columns_[fieldIndex].LobMaxSize = lobMaxSize;
		offset_ += length;
	  }

	  public virtual void fieldName(int fieldIndex, string name)
	  {
		columns_[fieldIndex].Name = name;
	  }

	  public virtual void udtName(int fieldIndex, string name)
	  {
			columns_[fieldIndex].UdtName = name;
	  }

	  public virtual void baseColumnName(int fieldIndex, string name)
	  {
	  }

	  public virtual void baseTableName(int fieldIndex, string name)
	  {
		columns_[fieldIndex].Table = name;
	  }

	  public virtual void columnLabel(int fieldIndex, string name)
	  {
		columns_[fieldIndex].Label = name;
	  }

	  public virtual void baseSchemaName(int fieldIndex, string name)
	  {
		columns_[fieldIndex].Schema = name;
	  }

	  public virtual void sqlFromTable(int fieldIndex, string name)
	  {
	  }

	  public virtual void sqlFromSchema(int fieldIndex, string name)
	  {
	  }

	  public virtual void columnAttributes(int fieldIndex, int updateable, int searchable, bool isIdentity, bool isAlwaysGenerated, bool isPartOfAnyIndex, bool isLoneUniqueIndex, bool isPartOfUniqueIndex, bool isExpression, bool isPrimaryKey, bool isNamed, bool isRowID, bool isRowChangeTimestamp)
	  {
		  if (columns_ != null)
		  {
		  columns_[fieldIndex].AutoIncrement = isIdentity; //TODO
		  columns_[fieldIndex].DefinitelyWritable = updateable == 0xF1;
		  columns_[fieldIndex].ReadOnly = updateable == 0xF0;
		  columns_[fieldIndex].Searchable = searchable != 0xF0;
		  columns_[fieldIndex].Writable = updateable != 0xF0;
		  }
	  }

	  /// <summary>
	  /// Caches the last Date returned by ResultSet.getDate(<i>column</i>), and returns that same object
	  /// on the next call to ResultSet.getDate(<i>column</i>) if the value returned from the database is identical.
	  /// This also works for repeated calls to ResultSet.getDate(<i>column</i>) when the ResultSet has not changed rows.
	  /// 
	  /// </summary>
	  public virtual void setUseDateCache(int column, bool b)
	  {
		Column c = getColumn(column - 1);
		c.UseDateCache = b;
	  }

	  /// <summary>
	  /// You know you want this, if you're going to be calling getDate() a lot.
	  /// 
	  /// </summary>
	  public virtual void setUseDateCache(string column, bool b)
	  {
		Column c = getColumn(column);
		c.UseDateCache = b;
	  }

	  /// <summary>
	  /// Caches the last Time returned by ResultSet.getTime(<i>column</i>), and returns that same object
	  /// on the next call to ResultSet.getTime(<i>column</i>) if the value returned from the database is identical.
	  /// This also works for repeated calls to ResultSet.getTime(<i>column</i>) when the ResultSet has not changed rows.
	  /// 
	  /// </summary>
	  public virtual void setUseTimeCache(int column, bool b)
	  {
		Column c = getColumn(column - 1);
		c.UseTimeCache = b;
	  }

	  /// <summary>
	  /// You know you want this, if you're going to be calling getTime() a lot.
	  /// 
	  /// </summary>
	  public virtual void setUseTimeCache(string column, bool b)
	  {
		Column c = getColumn(column);
		c.UseTimeCache = b;
	  }

	  /// <summary>
	  /// Caches all unique Strings returned by ResultSet.getString(<i>column</i>). Any subsequent call to
	  /// ResultSet.getString(<i>column</i>) will attempt to return a previously cached object if the value
	  /// returned from the database matches something in the cache. This also works for repeated calls to ResultSet.getString(<i>column</i>)
	  /// when the ResultSet has not changed rows. Note this will cache *ALL* Strings for this column, so unless you know your
	  /// column values will only ever be a finite set, you should also call <seealso cref="#setCacheLastOnly setCacheLastOnly()"/> and use an ORDER BY clause.
	  /// 
	  /// </summary>
	  public virtual void setUseStringCache(int column, bool b)
	  {
		Column c = getColumn(column - 1);
		c.UseStringCache = b;
	  }

	  /// <summary>
	  /// You know you want this, if you're going to be calling getString() a lot.
	  /// 
	  /// </summary>
	  public virtual void setUseStringCache(string column, bool b)
	  {
		Column c = getColumn(column);
		c.UseStringCache = b;
	  }

	  /// <summary>
	  /// Caches the last String returned by ResultSet.getString(<i>column</i>), and returns that same object
	  /// on the next call to ResultSet.getString(<i>column</i>) if the value returned from the database is identical.
	  /// This also works for repeated calls to ResultSet.getString(<i>column</i>) when the ResultSet has not changed rows.
	  /// This setting only takes effect if <seealso cref="#setUseStringCache setUseStringCache()"/> was called with a value of <i>true</i>
	  /// for this column.
	  /// 
	  /// </summary>
	  public virtual void setCacheLastOnly(int column, bool b)
	  {
		Column c = getColumn(column - 1);
		c.CacheLastOnly = b;
	  }

	  /// <summary>
	  /// You know you want this, if you're going to be calling getString() a lot.
	  /// 
	  /// </summary>
	  public virtual void setCacheLastOnly(string column, bool b)
	  {
		Column c = getColumn(column);
		c.CacheLastOnly = b;
	  }

	  internal virtual Column getColumn(int fieldIndex)
	  {
		return columns_[fieldIndex];
	  }

	  internal virtual Column getColumn(string name)
	  {
		for (int i = 0; i < columns_.Length; ++i)
		{
		  if (columns_[i].Name.Equals(name))
		  {
			  return columns_[i];
		  }
		}
		return null;
	  }

	  internal virtual int getColumnIndex(string name)
	  {
		for (int i = 0; i < columns_.Length; ++i)
		{
		  if (columns_[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
		  {
			  return i;
		  }
		}
		return -1;
	  }

	  ////////////////////////////
	  //
	  // ResultSetMetaData methods
	  //
	  ////////////////////////////

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCatalogName(int column) throws SQLException
	  public virtual string getCatalogName(int column)
	  {
		checkColumn(column);

		return catalog_;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getColumnClassName(int column) throws SQLException
	  public virtual string getColumnClassName(int column)
	  {
		  switch (getColumnType(column))
		  {
		  case Types.SMALLINT: // The spec says that SMALLINT also
			  // return "java.lang.Short"; //returns java.lang.Integer objects.
		  case Types.INTEGER:
			  return "java.lang.Integer";
		  case Types.BIGINT:
			  return "java.lang.Long";
		  case Types.FLOAT:
			  return "java.lang.Double";
		  case Types.REAL:
			  return "java.lang.Float";
		  case Types.DOUBLE:
			  return "java.lang.Double";
		  case Types.OTHER: // DECFLOAT
		  case Types.NUMERIC:
		  case Types.DECIMAL:
			  return "java.math.BigDecimal";
		  case Types.CHAR:
		  case Types.VARCHAR:
		  case Types.LONGVARCHAR:
			  return "java.lang.String";
		  case Types.BINARY:
		  case Types.VARBINARY:
			  return "[B";
		  case Types.TIME:
			  return "java.sql.Time";
		  case Types.DATE:
			  return "java.sql.Date";
		  case Types.TIMESTAMP:
			  return "java.sql.Timestamp";
		  case Types.BLOB:
			  return "com.ibm.db2.jdbc.app.DB2Blob";
		  case Types.CLOB:
			  return "com.ibm.db2.jdbc.app.DB2Clob";
		  case Types.DATALINK:
			return "java.net.URL";

		  case 2009: //SQLXML
			  return "java.sql.SQLXML";

		  default:
			  return "UNKNOWN";
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getColumnCount() throws SQLException
	  public virtual int ColumnCount
	  {
		  get
		  {
			return columns_.Length;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getColumnDisplaySize(int column) throws SQLException
	  public virtual int getColumnDisplaySize(int column)
	  {
		switch (getColumnType(column))
		{
		case Types.SMALLINT:
			return 6;
		case Types.INTEGER:
			return 11;
		case Types.BIGINT:
			return 20;
		case Types.REAL:
			return 13;
		case Types.DOUBLE:
		case Types.FLOAT:
			return 22;
		case Types.DATE:
			return 10;
		case Types.TIME:
			return 8;
		case Types.TIMESTAMP:
			return 26;
		case Types.CLOB:
		{
			return getPrecision(column);
		}

		case Types.DECIMAL:
		case Types.NUMERIC:
			return (getPrecision(column) + 2); // sign and decimal point must be added.
		case Types.OTHER: // DECFLOAT
		{
			  int precision = getPrecision(column);
			  switch (precision)
			  {
				  case 16:
				 /* add sign,decimalpoint,e,+/-,3-digitexponent */
				precision = 23;
			  break;
			  case 34:
			  /* add sign,decimalpoint,e,+/-,4-digitexponent */
			  precision = 42;
			  break;
			  default:
				  precision += 2;
			  break;
			  }
		  return (precision);
		}
		default:
			return getPrecision(column);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getColumnLabel(int column) throws SQLException
	  public virtual string getColumnLabel(int column)
	  {
		  checkColumn(column);
		  string label = columns_[column - 1].Label;
		  if (string.ReferenceEquals(label, null))
		  {
			  label = columns_[column - 1].Name;
		  }
		  return label;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getColumnName(int column) throws SQLException
	  public virtual string getColumnName(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].Name;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getColumnType(int column) throws SQLException
	  public virtual int getColumnType(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].SQLType;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getColumnTypeName(int column) throws SQLException
	  public virtual string getColumnTypeName(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].SQLTypeName;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getPrecision(int column) throws SQLException
	  public virtual int getPrecision(int column)
	  {
		checkColumn(column);
		return JDBCColumnMetaData.getPrecision(columns_[column - 1]);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getScale(int column) throws SQLException
	  public virtual int getScale(int column)
	  {
		  checkColumn(column);
		  return JDBCColumnMetaData.getScale(columns_[column - 1]);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSchemaName(int column) throws SQLException
	  public virtual string getSchemaName(int column)
	  {

		  checkColumn(column);
		return columns_[column - 1].Schema;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getTableName(int column) throws SQLException
	  public virtual string getTableName(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].Table;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isAutoIncrement(int column) throws SQLException
	  public virtual bool isAutoIncrement(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].AutoIncrement;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isCaseSensitive(int column) throws SQLException
	  public virtual bool isCaseSensitive(int column)
	  {
		  switch (getColumnType(column))
		  {
		  case Types.BIT:
		  case Types.TINYINT:
		  case Types.SMALLINT:
		  case Types.INTEGER:
		  case Types.BIGINT:
		  case Types.FLOAT:
		  case Types.REAL:
		  case Types.DOUBLE:
		  case Types.NUMERIC:
			  // case Types.BINARY:     Because they are really char data, this comes back as yes it is case sensitive.
			  // case Types.VARBINARY:
		  case Types.DATE:
		  case Types.TIME:
		  case Types.TIMESTAMP:
		  case Types.DECIMAL:
	  case Types.OTHER: // DECFLOAT @C2A
			  return false;
		  default:
			  return true;
		  }
	  }

	   /// <summary>
	   /// Indicates if the column is a currency value. </summary>
	   /// <param name="column">     The column index (1-based). </param>
	   /// <returns>                 Always false.  DB2 for IBM i
	   ///                         does not directly support currency
	   ///                         values. </returns>
	   /// <exception cref="SQLException">    If the column index is not valid.
	   ///  </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isCurrency(int column) throws SQLException
	  public virtual bool isCurrency(int column)
	  {
		  checkColumn(column);
		  return false;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isDefinitelyWritable(int column) throws SQLException
	  public virtual bool isDefinitelyWritable(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].DefinitelyWritable;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int isNullable(int column) throws SQLException
	  public virtual int isNullable(int column)
	  {
		  checkColumn(column);
		  return columns_[column - 1].Nullable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReadOnly(int column) throws SQLException
	  public virtual bool isReadOnly(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].ReadOnly;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isSearchable(int column) throws SQLException
	  public virtual bool isSearchable(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].Searchable;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isSigned(int column) throws SQLException
	  public virtual bool isSigned(int column)
	  {
		  switch (getColumnType(column))
		  {
		  case Types.BIT:
		  case Types.TINYINT:
		  case Types.SMALLINT:
		  case Types.INTEGER:
		  case Types.BIGINT:
		  case Types.FLOAT:
		  case Types.REAL:
		  case Types.DOUBLE:
		  case Types.NUMERIC:
		  case Types.DECIMAL:
		  case Types.OTHER:
			  return true;
		  default:
			  return false;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isWritable(int column) throws SQLException
	  public virtual bool isWritable(int column)
	  {
		  checkColumn(column);
		return columns_[column - 1].Writable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkColumn(int column) throws SQLException
	  private void checkColumn(int column)
	  {
		  if ((column < 1) || (column > columns_.Length))
		  {
		  JDBCError.throwSQLException(JDBCError.EXC_DESCRIPTOR_INDEX_INVALID);
		  }
	  }

	}



}