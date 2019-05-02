using System;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCBlob.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{

	public class JDBCBlob : Blob
	{
	  private readonly sbyte[] data_;
	  private readonly int offset_;
	  private int length_;

	  public JDBCBlob(sbyte[] data, int offset, int len)
	  {
		data_ = data;
		offset_ = offset;
		length_ = len;
	  }

	  private sealed class JDBCBlobOutputStream : Stream
	  {
		internal readonly JDBCBlob blob_;
		internal int next_;

		internal JDBCBlobOutputStream(JDBCBlob blob, int start) // 0-based.
		{
		  blob_ = blob;
		  next_ = start + blob_.offset_;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
		public void write(int b)
		{
		  if (next_ < blob_.offset_ + blob_.length_)
		  {
			blob_.data_[next_++] = (sbyte)b;
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getBinaryStream() throws SQLException
	  public virtual Stream getBinaryStream()
	  {
		return new MemoryStream(data_, offset_, length_);
	  }

	  // pos is 1-based!
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(long pos, int length) throws SQLException
	  public virtual sbyte[] getBytes(long pos, int length)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		int total = length_ - ipos + 1;
		if (total > length)
		{
			total = length;
		}
		sbyte[] data = new sbyte[total];
		Array.Copy(data_, offset_ + ipos - 1, data, 0, total);
		return data;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long length() throws SQLException
	  public virtual long length()
	  {
		return length_;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(Blob pattern, long start) throws SQLException
	  public virtual long position(Blob pattern, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		sbyte[] patternBytes = pattern.getBytes(0, (int)(pattern.length() & 0x7FFFFFFF));
		return position(patternBytes, start);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(byte[] pattern, long start) throws SQLException
	  public virtual long position(sbyte[] pattern, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		for (int i = (int)(start & 0x7FFFFFFF) + offset_ - 1; i < offset_ + length_; ++i)
		{
		  bool match = true;
		  for (int j = 0; match && j < pattern.Length; ++j)
		  {
			if (data_[i] != pattern[j])
			{
				match = false;
			}
		  }
		  if (match)
		  {
			return i - offset_;
		  }
		}
		return -1;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputStream setBinaryStream(long pos) throws SQLException
	  public virtual Stream setBinaryStream(long pos)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		return new JDBCBlobOutputStream(this, ipos - 1);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setBytes(long pos, byte[] bytes) throws SQLException
	  public virtual int setBytes(long pos, sbyte[] bytes)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		return setBytes(pos, bytes, 0, bytes.Length);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setBytes(long pos, byte[] bytes, int offset, int len) throws SQLException
	  public virtual int setBytes(long pos, sbyte[] bytes, int offset, int len)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		int total = length_ - ipos + 1;
		if (total > len)
		{
			total = len;
		}
		Array.Copy(bytes, offset, data_, offset_ + ipos - 1, total);
		return total;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void truncate(long len) throws SQLException
	  public virtual void truncate(long len)
	  {
		length_ = (len < 0) ? 0 : (int)(len & 0x7FFFFFFF);
	  }
	}




}