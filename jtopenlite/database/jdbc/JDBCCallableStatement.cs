using System;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCCallableStatement.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{


	public class JDBCCallableStatement : JDBCPreparedStatement, CallableStatement
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JDBCCallableStatement(JDBCConnection conn, String sql, java.util.Calendar calendar, String statementName, String cursorName, int rpbID) throws java.sql.SQLException
		public JDBCCallableStatement(JDBCConnection conn, string sql, DateTime calendar, string statementName, string cursorName, int rpbID) : base(conn, sql, calendar, statementName, cursorName, rpbID)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Array getArray(int i) throws java.sql.SQLException
		public virtual Array getArray(int i)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Array getArray(String parameterName) throws java.sql.SQLException
		public virtual Array getArray(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(int parameterIndex) throws java.sql.SQLException
		public virtual decimal getBigDecimal(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(String parameterName) throws java.sql.SQLException
		public virtual decimal getBigDecimal(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.math.BigDecimal getBigDecimal(int parameterIndex, int scale) throws java.sql.SQLException
		public virtual decimal getBigDecimal(int parameterIndex, int scale)
		{
		throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Blob getBlob(int i) throws java.sql.SQLException
		public virtual Blob getBlob(int i)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Blob getBlob(String parameterName) throws java.sql.SQLException
		public virtual Blob getBlob(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(int parameterIndex) throws java.sql.SQLException
		public virtual bool getBoolean(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(String parameterName) throws java.sql.SQLException
		public virtual bool getBoolean(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte getByte(int parameterIndex) throws java.sql.SQLException
		public virtual sbyte getByte(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte getByte(String parameterName) throws java.sql.SQLException
		public virtual sbyte getByte(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(int parameterIndex) throws java.sql.SQLException
		public virtual sbyte[] getBytes(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(String parameterName) throws java.sql.SQLException
		public virtual sbyte[] getBytes(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Clob getClob(int i) throws java.sql.SQLException
		public virtual Clob getClob(int i)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Clob getClob(String parameterName) throws java.sql.SQLException
		public virtual Clob getClob(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Date getDate(int parameterIndex) throws java.sql.SQLException
		public virtual Date getDate(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Date getDate(String parameterName) throws java.sql.SQLException
		public virtual Date getDate(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Date getDate(int parameterIndex, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Date getDate(int parameterIndex, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Date getDate(String parameterName, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Date getDate(string parameterName, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(int parameterIndex) throws java.sql.SQLException
		public virtual double getDouble(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(String parameterName) throws java.sql.SQLException
		public virtual double getDouble(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float getFloat(int parameterIndex) throws java.sql.SQLException
		public virtual float getFloat(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float getFloat(String parameterName) throws java.sql.SQLException
		public virtual float getFloat(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(int parameterIndex) throws java.sql.SQLException
		public virtual int getInt(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(String parameterName) throws java.sql.SQLException
		public virtual int getInt(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(int parameterIndex) throws java.sql.SQLException
		public virtual long getLong(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(String parameterName) throws java.sql.SQLException
		public virtual long getLong(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(int parameterIndex) throws java.sql.SQLException
		public virtual object getObject(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(String parameterName) throws java.sql.SQLException
		public virtual object getObject(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(int i, java.util.Map<String, Class> map) throws java.sql.SQLException
		public virtual object getObject(int i, IDictionary<string, Type> map)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObject(String parameterName, java.util.Map<String, Class> map) throws java.sql.SQLException
		public virtual object getObject(string parameterName, IDictionary<string, Type> map)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Ref getRef(int i) throws java.sql.SQLException
		public virtual Ref getRef(int i)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Ref getRef(String parameterName) throws java.sql.SQLException
		public virtual Ref getRef(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short getShort(int parameterIndex) throws java.sql.SQLException
		public virtual short getShort(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short getShort(String parameterName) throws java.sql.SQLException
		public virtual short getShort(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(int parameterIndex) throws java.sql.SQLException
		public virtual string getString(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(String parameterName) throws java.sql.SQLException
		public virtual string getString(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Time getTime(int parameterIndex) throws java.sql.SQLException
		public virtual Time getTime(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Time getTime(String parameterName) throws java.sql.SQLException
		public virtual Time getTime(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Time getTime(int parameterIndex, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Time getTime(int parameterIndex, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Time getTime(String parameterName, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Time getTime(string parameterName, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Timestamp getTimestamp(int parameterIndex) throws java.sql.SQLException
		public virtual Timestamp getTimestamp(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Timestamp getTimestamp(String parameterName) throws java.sql.SQLException
		public virtual Timestamp getTimestamp(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Timestamp getTimestamp(int parameterIndex, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Timestamp getTimestamp(int parameterIndex, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Timestamp getTimestamp(String parameterName, java.util.Calendar cal) throws java.sql.SQLException
		public virtual Timestamp getTimestamp(string parameterName, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getURL(int parameterIndex) throws java.sql.SQLException
		public virtual Uri getURL(int parameterIndex)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getURL(String parameterName) throws java.sql.SQLException
		public virtual Uri getURL(string parameterName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(int parameterIndex, int sqlType) throws java.sql.SQLException
		public virtual void registerOutParameter(int parameterIndex, int sqlType)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(String parameterName, int sqlType) throws java.sql.SQLException
		public virtual void registerOutParameter(string parameterName, int sqlType)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(int parameterIndex, int sqlType, int scale) throws java.sql.SQLException
		public virtual void registerOutParameter(int parameterIndex, int sqlType, int scale)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(int paramIndex, int sqlType, String typeName) throws java.sql.SQLException
		public virtual void registerOutParameter(int paramIndex, int sqlType, string typeName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(String parameterName, int sqlType, int scale) throws java.sql.SQLException
		public virtual void registerOutParameter(string parameterName, int sqlType, int scale)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutParameter(String parameterName, int sqlType, String typeName) throws java.sql.SQLException
		public virtual void registerOutParameter(string parameterName, int sqlType, string typeName)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(String parameterName, java.io.InputStream x, int length) throws java.sql.SQLException
		public virtual void setAsciiStream(string parameterName, Stream x, int length)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBigDecimal(String parameterName, java.math.BigDecimal x) throws java.sql.SQLException
		public virtual void setBigDecimal(string parameterName, decimal x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(String parameterName, java.io.InputStream x, int length) throws java.sql.SQLException
		public virtual void setBinaryStream(string parameterName, Stream x, int length)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBoolean(String parameterName, boolean x) throws java.sql.SQLException
		public virtual void setBoolean(string parameterName, bool x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setByte(String parameterName, byte x) throws java.sql.SQLException
		public virtual void setByte(string parameterName, sbyte x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBytes(String parameterName, byte[] x) throws java.sql.SQLException
		public virtual void setBytes(string parameterName, sbyte[] x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(String parameterName, java.io.Reader reader, int length) throws java.sql.SQLException
		public virtual void setCharacterStream(string parameterName, Reader reader, int length)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDate(String parameterName, java.sql.Date x) throws java.sql.SQLException
		public virtual void setDate(string parameterName, Date x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDate(String parameterName, java.sql.Date x, java.util.Calendar cal) throws java.sql.SQLException
		public virtual void setDate(string parameterName, Date x, DateTime cal)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDouble(String parameterName, double x) throws java.sql.SQLException
		public virtual void setDouble(string parameterName, double x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFloat(String parameterName, float x) throws java.sql.SQLException
		public virtual void setFloat(string parameterName, float x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInt(String parameterName, int x) throws java.sql.SQLException
		public virtual void setInt(string parameterName, int x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLong(String parameterName, long x) throws java.sql.SQLException
		public virtual void setLong(string parameterName, long x)
		{
			throw new NotImplementedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNull(String parameterName, int sqlType) throws java.sql.SQLException
		public virtual void setNull(string parameterName, int sqlType)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNull(String parameterName, int sqlType, String typeName) throws java.sql.SQLException
		public virtual void setNull(string parameterName, int sqlType, string typeName)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(String parameterName, Object x) throws java.sql.SQLException
		public virtual void setObject(string parameterName, object x)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(String parameterName, Object x, int targetSqlType) throws java.sql.SQLException
		public virtual void setObject(string parameterName, object x, int targetSqlType)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setObject(String parameterName, Object x, int targetSqlType, int scale) throws java.sql.SQLException
		public virtual void setObject(string parameterName, object x, int targetSqlType, int scale)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setShort(String parameterName, short x) throws java.sql.SQLException
		public virtual void setShort(string parameterName, short x)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setString(String parameterName, String x) throws java.sql.SQLException
		public virtual void setString(string parameterName, string x)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTime(String parameterName, java.sql.Time x) throws java.sql.SQLException
		public virtual void setTime(string parameterName, Time x)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTime(String parameterName, java.sql.Time x, java.util.Calendar cal) throws java.sql.SQLException
		public virtual void setTime(string parameterName, Time x, DateTime cal)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTimestamp(String parameterName, java.sql.Timestamp x) throws java.sql.SQLException
		public virtual void setTimestamp(string parameterName, Timestamp x)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTimestamp(String parameterName, java.sql.Timestamp x, java.util.Calendar cal) throws java.sql.SQLException
		public virtual void setTimestamp(string parameterName, Timestamp x, DateTime cal)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setURL(String parameterName, java.net.URL val) throws java.sql.SQLException
		public virtual void setURL(string parameterName, Uri val)
		{
			throw new NotImplementedException();

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean wasNull() throws java.sql.SQLException
		public virtual bool wasNull()
		{
			throw new NotImplementedException();
		}


	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Reader getCharacterStream(int parm) throws java.sql.SQLException
	  public virtual Reader getCharacterStream(int parm)
	  {
		throw new NotImplementedException();
	  }


	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Reader getCharacterStream(String parm) throws java.sql.SQLException
	  public virtual Reader getCharacterStream(string parm)
	  {
		throw new NotImplementedException();
	  }


	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Reader getNCharacterStream(int parm) throws java.sql.SQLException
	  public virtual Reader getNCharacterStream(int parm)
	  {
		return getCharacterStream(parm);
	  }


	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Reader getNCharacterStream(String parm) throws java.sql.SQLException
	  public virtual Reader getNCharacterStream(string parm)
	  {
		throw new NotImplementedException();
	  }




	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNString(int parm) throws java.sql.SQLException
	  public virtual string getNString(int parm)
	  {
		return getString(parm);
	  }


	  /// <param name="parm"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNString(String parm) throws java.sql.SQLException
	  public virtual string getNString(string parm)
	  {
		return getString(parm);
	  }







	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(String parm, java.io.InputStream arg1) throws java.sql.SQLException
	  public virtual void setAsciiStream(string parm, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAsciiStream(String parm, java.io.InputStream arg1, long arg2) throws java.sql.SQLException
	  public virtual void setAsciiStream(string parm, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(String parm, java.io.InputStream arg1) throws java.sql.SQLException
	  public virtual void setBinaryStream(string parm, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBinaryStream(String parm, java.io.InputStream arg1, long arg2) throws java.sql.SQLException
	  public virtual void setBinaryStream(string parm, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(String parm, java.sql.Blob arg1) throws java.sql.SQLException
	  public virtual void setBlob(string parm, Blob arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(String parm, java.io.InputStream arg1) throws java.sql.SQLException
	  public virtual void setBlob(string parm, Stream arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBlob(String parm, java.io.InputStream arg1, long arg2) throws java.sql.SQLException
	  public virtual void setBlob(string parm, Stream arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(String parm, java.io.Reader arg1) throws java.sql.SQLException
	  public virtual void setCharacterStream(string parm, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCharacterStream(String parm, java.io.Reader arg1, long arg2) throws java.sql.SQLException
	  public virtual void setCharacterStream(string parm, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(String parm, java.sql.Clob arg1) throws java.sql.SQLException
	  public virtual void setClob(string parm, Clob arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(String parm, java.io.Reader arg1) throws java.sql.SQLException
	  public virtual void setClob(string parm, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setClob(String parm, java.io.Reader arg1, long arg2) throws java.sql.SQLException
	  public virtual void setClob(string parm, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNCharacterStream(String parm, java.io.Reader arg1) throws java.sql.SQLException
	  public virtual void setNCharacterStream(string parm, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNCharacterStream(String parm, java.io.Reader arg1, long arg2) throws java.sql.SQLException
	  public virtual void setNCharacterStream(string parm, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }




	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNClob(String parm, java.io.Reader arg1) throws java.sql.SQLException
	  public virtual void setNClob(string parm, Reader arg1)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
	  /// <param name="arg2"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNClob(String parm, java.io.Reader arg1, long arg2) throws java.sql.SQLException
	  public virtual void setNClob(string parm, Reader arg1, long arg2)
	  {
		throw new NotImplementedException();

	  }


	  /// <param name="parm"> </param>
	  /// <param name="arg1"> </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNString(String parm, String arg1) throws java.sql.SQLException
	  public virtual void setNString(string parm, string arg1)
	  {
		throw new NotImplementedException();

	  }



	}

}