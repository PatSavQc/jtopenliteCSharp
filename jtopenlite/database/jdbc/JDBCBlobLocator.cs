using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCBlobLocator.java
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

	public sealed class JDBCBlobLocator : Blob, DatabaseLOBDataCallback
	{
	  private readonly DatabaseConnection conn_;
	  private readonly DatabaseRetrieveLOBDataAttributes attribs_;

	  private long length_ = -1;

	  private sbyte[] currentBuffer_ = new sbyte[8192];

	  private MemoryStream tempOutput_;

	  public JDBCBlobLocator(DatabaseConnection conn, DatabaseRetrieveLOBDataAttributes attrib)
	  {
		conn_ = conn;
		attribs_ = attrib;
	  }

	  public void newLOBLength(long length)
	  {
		length_ = length;
	  }

	  public void newLOBData(int ccsid, int length)
	  {
	  }

	  public sbyte[] LOBBuffer
	  {
		  get
		  {
			return currentBuffer_;
		  }
		  set
		  {
			currentBuffer_ = value;
		  }
	  }


	  public void newLOBSegment(sbyte[] buffer, int offset, int length)
	  {
		if (tempOutput_ != null)
		{
		  tempOutput_.Write(buffer, offset, length);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getBinaryStream() throws SQLException
	  public Stream getBinaryStream()
	  {
		return new JDBCBlobLocatorInputStream(conn_, attribs_, length_);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getBinaryStream(long pos, long length) throws SQLException
	  public Stream getBinaryStream(long pos, long length)
	  {
		// JDBC 4.0 method not yet implemented

		JDBCError.throwSQLException(JDBCError.EXC_FUNCTION_NOT_SUPPORTED);
		  return null;
	  }

	  // pos is 1-based!
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(long pos, int length) throws SQLException
	  public sbyte[] getBytes(long pos, int length)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		attribs_.StartOffset = ipos - 1;
		attribs_.RequestedSize = length;
		attribs_.ReturnCurrentLengthIndicator = 0xF1;
		try
		{
		  tempOutput_ = new MemoryStream();
		  conn_.retrieveLOBData(attribs_, this);
		  tempOutput_.Flush();
		  tempOutput_.Close();
		  return tempOutput_.toByteArray();
		}
		catch (IOException io)
		{
		  SQLException sql = new SQLException(io.ToString());
		  sql.initCause(io);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void refreshLength() throws SQLException
	  public void refreshLength()
	  {
		length_ = -1;
		length();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long length() throws SQLException
	  public long length()
	  {
		if (length_ == -1)
		{
		  attribs_.StartOffset = 0;
		  attribs_.RequestedSize = 0;
		  attribs_.ReturnCurrentLengthIndicator = 0xF1;
		  try
		  {
			length_ = -1;
			conn_.retrieveLOBData(attribs_, this);
			if (length_ == -1)
			{
			  throw new IOException("LOB length not retrieved.");
			}
		  }
		  catch (IOException io)
		  {
			SQLException sql = new SQLException(io.ToString());
			sql.initCause(io);
			throw sql;
		  }
		}
		return length_;
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(Blob pattern, long start) throws SQLException
	  public long position(Blob pattern, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(byte[] pattern, long start) throws SQLException
	  public long position(sbyte[] pattern, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputStream setBinaryStream(long pos) throws SQLException
	  public Stream setBinaryStream(long pos)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setBytes(long pos, byte[] bytes) throws SQLException
	  public int setBytes(long pos, sbyte[] bytes)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setBytes(long pos, byte[] bytes, int offset, int len) throws SQLException
	  public int setBytes(long pos, sbyte[] bytes, int offset, int len)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void truncate(long len) throws SQLException
	  public void truncate(long len)
	  {
		throw new NotImplementedException();
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void free() throws SQLException
	  public void free()
	  {
		// TODO  -- Not implemented yet.  Just no-op for now.

	  }



	}




}