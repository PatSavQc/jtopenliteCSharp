using System;
using System.Text;
using System.IO;

namespace com.ibm.jtopenlite.database.jdbc
{

	public class JDBCClob : Clob
	{
	  private readonly sbyte[] data_;
	  private readonly int offset_;
	  private int length_;
	  private readonly int ccsid_;

	  public JDBCClob(sbyte[] data, int offset, int len, int ccsid)
	  {
		data_ = data;
		offset_ = offset;
		length_ = len;
		ccsid_ = ccsid;
	  }

	  private sealed class JDBCClobOutputStream : Stream
	  {
		internal readonly JDBCClob clob_;
		internal int next_;

		internal JDBCClobOutputStream(JDBCClob clob, int start) // 0-based.
		{
		  clob_ = clob;
		  next_ = start + clob_.offset_;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
		public void write(int b)
		{
		  if (next_ < clob_.offset_ + clob_.length_)
		  {
			clob_.data_[next_++] = (sbyte)b;
		  }
		}
	  }

	  private sealed class JDBCClobWriter : Writer
	  {
		internal readonly JDBCClob clob_;
		internal int next_;

		internal JDBCClobWriter(JDBCClob clob, int start) // 0-based.
		{
		  clob_ = clob;
		  next_ = start + clob_.offset_;
		}

		public void close()
		{
		}

		public void flush()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char[] buf, int off, int len) throws IOException
		public void write(char[] buf, int off, int len)
		{
		  for (int i = off; i < off + len; ++i)
		  {
			if (next_ < clob_.offset_ + clob_.length_)
			{
			  clob_.data_[next_++] = (sbyte)buf[i]; //TODO
			}
		  }
		}
	  }

	  /// <summary>
	  /// This is a ByteArrayInputStream wrapper around String.getBytes("ASCII").
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStream getAsciiStream() throws SQLException
	  public virtual Stream getAsciiStream()
	  {
		try
		{
		  return new MemoryStream(Conv.ebcdicByteArrayToString(data_, offset_, length_, ccsid_).GetBytes(Encoding.ASCII));
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

	  /// <summary>
	  /// This is a StringReader wrapper.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reader getCharacterStream() throws SQLException
	  public virtual Reader getCharacterStream()
	  {
		try
		{
		  return new StringReader(Conv.ebcdicByteArrayToString(data_, offset_, length_, ccsid_));
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

	  // pos is 1-based!
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSubString(long pos, int length) throws SQLException
	  public virtual string getSubString(long pos, int length)
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
		try
		{
		  return Conv.ebcdicByteArrayToString(data_, offset_ + ipos - 1, total, ccsid_);
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long length() throws SQLException
	  public virtual long length()
	  {
		return length_;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(Clob pattern, long start) throws SQLException
	  public virtual long position(Clob pattern, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		string patternString = pattern.getSubString(0, (int)(pattern.length() & 0x7FFFFFFF));
		return position(patternString, start);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long position(String patternString, long start) throws SQLException
	  public virtual long position(string patternString, long start)
	  {
		if (start <= 0)
		{
			throw new SQLException("Bad start: " + start);
		}
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] pattern = com.ibm.jtopenlite.Conv.stringToEBCDICByteArray(patternString, ccsid_);
		  sbyte[] pattern = Conv.stringToEBCDICByteArray(patternString, ccsid_);
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
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputStream setAsciiStream(long pos) throws SQLException
	  public virtual Stream setAsciiStream(long pos)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		return new JDBCClobOutputStream(this, ipos - 1);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Writer setCharacterStream(long pos) throws SQLException
	  public virtual Writer setCharacterStream(long pos)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		int ipos = (int)(pos & 0x7FFFFFFF);
		return new JDBCClobWriter(this, ipos - 1);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setString(long pos, String str) throws SQLException
	  public virtual int setString(long pos, string str)
	  {
		if (pos <= 0)
		{
			throw new SQLException("Bad position: " + pos);
		}
		return setString(pos, str, 0, str.Length);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setString(long pos, String str, int offset, int len) throws SQLException
	  public virtual int setString(long pos, string str, int offset, int len)
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
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] bytes = com.ibm.jtopenlite.Conv.stringToEBCDICByteArray(str, ccsid_);
		  sbyte[] bytes = Conv.stringToEBCDICByteArray(str, ccsid_);
		  Array.Copy(bytes, offset, data_, offset_ + ipos - 1, total);
		  return total;
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = new SQLException(uee.ToString());
		  sql.initCause(uee);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void truncate(long len) throws SQLException
	  public virtual void truncate(long len)
	  {
		length_ = (len < 0) ? 0 : (int)(len & 0x7FFFFFFF);
	  }
	}




}