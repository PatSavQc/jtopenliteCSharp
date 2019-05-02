using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCBlobLocatorInputStream.java
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

	public class JDBCBlobLocatorInputStream : Stream, DatabaseLOBDataCallback
	{
	  private readonly DatabaseConnection conn_;
	  private readonly DatabaseRetrieveLOBDataAttributes attribs_;

	  private long numRead_ = 0;
	  private long length_;
	  private int nextRead_;
	  private sbyte[] buffer_;

	  public JDBCBlobLocatorInputStream(DatabaseConnection conn, DatabaseRetrieveLOBDataAttributes attribs, long length)
	  {
		conn_ = conn;
		attribs_ = attribs; //TODO - use separate attribs?
		length_ = length;
	  }

	  public virtual void newLOBLength(long length)
	  {
		length_ = length;
	  }

	  public virtual void newLOBData(int ccsid, int length)
	  {
	  }

	  public virtual sbyte[] LOBBuffer
	  {
		  get
		  {
			return buffer_;
		  }
		  set
		  {
			buffer_ = value;
		  }
	  }


	  public virtual void newLOBSegment(sbyte[] buffer, int offset, int length)
	  {
		nextRead_ = 0;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
	  public virtual int read()
	  {
		if (length_ >= 0 && numRead_ >= length_)
		{
			return -1;
		}

		if (buffer_ == null)
		{
			buffer_ = new sbyte[8192];
		}

		if (length_ < 0 || nextRead_ == 0 || nextRead_ >= buffer_.Length)
		{
		  attribs_.StartOffset = (int)(numRead_ & 0x7FFFFFFF);
		  attribs_.RequestedSize = buffer_.Length;
		  attribs_.ReturnCurrentLengthIndicator = 0xF1; // Return the current length.
		  conn_.retrieveLOBData(attribs_, this);
		  if (numRead_ >= length_)
		  {
			  return -1;
		  }
		}
		++numRead_;
		return buffer_[nextRead_++] & 0x00FF;
	  }
	}


}