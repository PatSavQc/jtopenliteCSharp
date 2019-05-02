using System;
using System.Collections;
using System.Text;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Column.java
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



	internal sealed class Column
	{
	  private static readonly char[] NUMS = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

	  private string name_;
	  private string udtName_;
	  private int type_;
	  private int length_; // Length of data type in the buffer sent to / from host server
	  private int declaredLength_ = 0; // This is the declared length of the type, calculated when needed
	  private int scale_;
	  private int precision_;
	  private int ccsid_;
	  private bool isForBitData_;
	  private int lobMaxSize_;

	  private int offset_;

	  private int index_; // parameter / column number 1 based
	  private bool parameter_; // is this a parameter

	  private char[] buffer_;

	  private int dateFormat_;
	  private int timeFormat_;
	  private int dateSeparator_;
	  private int timeSeparator_;

	  private string table_;
	  private string label_;
	  private string schema_;
	  private bool autoIncrement_;
	  private bool definitelyWritable_;
	  private bool readOnly_ = true;
	  private bool searchable_ = true;
	  private bool writable_;

	  private bool useDateCache_ = false;
	  private Hashtable dateCache_;
	  // private Date dateZero_;

	  private bool useTimeCache_ = false;
	  private Hashtable timeCache_;
	  // private Time timeZero_;

	  private bool useStringCache_ = false;
	  private bool cacheLastOnly_ = false;
	  private Hashtable cache_ = null;

	  private bool null_ = false;
	  private string stringValue_;
	  private int intValue_;
	  private long longValue_;
	  private short shortValue_;
	  private float floatValue_;
	  private double doubleValue_;
	  private sbyte byteValue_;
	  private bool booleanValue_;
	  private DateTime dateValue_; // Used for date, time, and timestamp.
	  private sbyte[] byteArrayValue_;
	  private decimal bigDecimalValue_;
	  private object objectValue_;
	  private Uri urlValue_;
	  private Stream inputStreamValue_; // Used for ascii, binary, and unicode streams.
	  private int inputStreamLength_; // Used for ascii, binary, character, and unicode streams.
	  private Reader readerValue_;
	  private int valueType_ = 0;
	  private const int TYPE_STRING = 1;
	  private const int TYPE_INT = 2;
	  private const int TYPE_LONG = 3;
	  private const int TYPE_SHORT = 4;
	  private const int TYPE_FLOAT = 5;
	  private const int TYPE_DOUBLE = 6;
	  private const int TYPE_BYTE = 7;
	  private const int TYPE_BOOLEAN = 8;
	  private const int TYPE_DATE = 9;
	  private const int TYPE_TIME = 10;
	  private const int TYPE_TIMESTAMP = 11;
	  private const int TYPE_BYTE_ARRAY = 12;
	  private const int TYPE_BIG_DECIMAL = 13;
	  private const int TYPE_OBJECT = 14;
	  private const int TYPE_URL = 15;
	  private const int TYPE_ASCII_STREAM = 16;
	  private const int TYPE_BINARY_STREAM = 17;
	  private const int TYPE_UNICODE_STREAM = 18;
	  private const int TYPE_CHARACTER_STREAM = 19;





	  private DateTime calendar_;

	  internal Column(DateTime cal, int index, bool parameter)
	  {
		calendar_ = cal;

		index_ = index;
		parameter_ = parameter;
	  }

	  internal void clearValue()
	  {
		null_ = false;
		stringValue_ = null;
		intValue_ = 0;
		longValue_ = 0;
		shortValue_ = 0;
		floatValue_ = 0;
		doubleValue_ = 0;
		byteValue_ = 0;
		booleanValue_ = false;
		dateValue_ = null;
		byteArrayValue_ = null;
		bigDecimalValue_ = null;
		objectValue_ = null;
		urlValue_ = null;
		inputStreamValue_ = null;
		inputStreamLength_ = 0;
		readerValue_ = null;
		valueType_ = 0;
	  }

	  internal string Value
	  {
		  set
		  {
			stringValue_ = value;
			null_ = string.ReferenceEquals(value, null);
			valueType_ = TYPE_STRING;
    
		  }
	  }

	  internal int Value
	  {
		  set
		  {
			intValue_ = value;
			null_ = false;
			valueType_ = TYPE_INT;
		  }
	  }

	  internal long Value
	  {
		  set
		  {
			longValue_ = value;
			null_ = false;
			valueType_ = TYPE_LONG;
		  }
	  }

	  internal short Value
	  {
		  set
		  {
			shortValue_ = value;
			null_ = false;
			valueType_ = TYPE_SHORT;
		  }
	  }

	  internal float Value
	  {
		  set
		  {
			floatValue_ = value;
			null_ = false;
			valueType_ = TYPE_FLOAT;
		  }
	  }

	  internal double Value
	  {
		  set
		  {
			doubleValue_ = value;
			null_ = false;
			valueType_ = TYPE_DOUBLE;
		  }
	  }

	  internal sbyte Value
	  {
		  set
		  {
			byteValue_ = value;
			null_ = false;
			valueType_ = TYPE_BYTE;
		  }
	  }

	  internal bool Value
	  {
		  set
		  {
			booleanValue_ = value;
			null_ = false;
			valueType_ = TYPE_BOOLEAN;
		  }
	  }

	  internal DateTime Value
	  {
		  set
		  {
			dateValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_DATE;
		  }
	  }

	  internal void setValue(DateTime d, DateTime cal)
	  {
		dateValue_ = d;
		null_ = d == null;
		valueType_ = TYPE_DATE;
		calendar_ = cal;
	  }

	  internal Time Value
	  {
		  set
		  {
			dateValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_TIME;
		  }
	  }

	  internal void setValue(Time t, DateTime cal)
	  {
		dateValue_ = t;
		null_ = t == null;
		valueType_ = TYPE_TIME;
		calendar_ = cal;
	  }

	  internal Timestamp Value
	  {
		  set
		  {
			dateValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_TIMESTAMP;
		  }
	  }

	  internal void setValue(Timestamp t, DateTime cal)
	  {
		dateValue_ = t;
		null_ = t == null;
		valueType_ = TYPE_TIMESTAMP;
		calendar_ = cal;
	  }

	  internal sbyte[] Value
	  {
		  set
		  {
			byteArrayValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_BYTE_ARRAY;
		  }
	  }

	  internal decimal Value
	  {
		  set
		  {
			bigDecimalValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_BIG_DECIMAL;
		  }
	  }

	  internal object Value
	  {
		  set
		  {
			// Use the right setValue for supported types
			if (value is string)
			{
				Value = (string)value;
			}
			else if (value is java.sql.Date)
			{
				Value = (java.sql.Date)value;
			}
			else if (value is java.sql.Time)
			{
				Value = (java.sql.Time)value;
			}
			else if (value is java.sql.Timestamp)
			{
				Value = (java.sql.Timestamp)value;
			}
			else if (value is sbyte[])
			{
				Value = (sbyte[])value;
			}
			else if (value is decimal)
			{
				Value = (decimal)value;
			}
			else if (value is URL)
			{
				Value = (URL)value;
			}
			else
			{
				objectValue_ = value;
				null_ = value == null;
				valueType_ = TYPE_OBJECT;
			}
		  }
	  }

	  internal Uri Value
	  {
		  set
		  {
			urlValue_ = value;
			null_ = value == null;
			valueType_ = TYPE_URL;
		  }
	  }

	  internal void setAsciiStreamValue(Stream @in, int length)
	  {
		inputStreamValue_ = @in;
		inputStreamLength_ = length;
		null_ = @in == null;
		valueType_ = TYPE_ASCII_STREAM;
	  }

	  internal void setBinaryStreamValue(Stream @in, int length)
	  {
		inputStreamValue_ = @in;
		inputStreamLength_ = length;
		null_ = @in == null;
		valueType_ = TYPE_BINARY_STREAM;
	  }

	  internal void setUnicodeStreamValue(Stream @in, int length)
	  {
		inputStreamValue_ = @in;
		inputStreamLength_ = length;
		null_ = @in == null;
		valueType_ = TYPE_UNICODE_STREAM;
	  }

	  internal void setCharacterStreamValue(Reader r, int length)
	  {
		readerValue_ = r;
		inputStreamLength_ = length;
		null_ = r == null;
		valueType_ = TYPE_CHARACTER_STREAM;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getNonexponentValueString() throws SQLException
	  private string NonexponentValueString
	  {
		  get
		  {
			  string s;
		switch (valueType_)
		{
					case TYPE_FLOAT:
						s = floatValue_.ToString();
						if (s.IndexOf("E", StringComparison.Ordinal) > 0)
						{
							s = (new decimal(floatValue_)).toPlainString();
						}
						break;
					  case TYPE_DOUBLE:
						s = doubleValue_.ToString();
						if (s.IndexOf("E", StringComparison.Ordinal) > 0)
						{
							s = (new decimal(doubleValue_)).toPlainString();
						}
						break;
					  case TYPE_BIG_DECIMAL:
						  s = bigDecimalValue_.toPlainString();
						  break;
					  case TYPE_BOOLEAN:
						  s = booleanValue_ ? "1" : "0";
						  break;
					  default:
							return ValueString;
		}
				return s;
		  }
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getValueString() throws SQLException
	  private string ValueString
	  {
		  get
		  {
			string s;
    
			try
			{
			  switch (valueType_)
			  {
				case TYPE_STRING :
					s = stringValue_;
					break;
				case TYPE_INT:
				  s = intValue_.ToString();
				  break;
				case TYPE_SHORT:
				  s = shortValue_.ToString();
				  break;
				case TYPE_LONG:
				  s = longValue_.ToString();
				  break;
				case TYPE_FLOAT:
				  s = floatValue_.ToString();
				  break;
				case TYPE_DOUBLE:
				  s = doubleValue_.ToString();
				  break;
				case TYPE_BYTE:
				  s = byteValue_.ToString();
				  break;
				case TYPE_BOOLEAN:
				  s = booleanValue_.ToString();
				  break;
				case TYPE_DATE:
				case TYPE_TIME:
				case TYPE_TIMESTAMP:
				  s = dateValue_.ToString();
				  break;
				case TYPE_BYTE_ARRAY:
				  //s = Conv.unicodeByteArrayToString(byteArrayValue_, 0, byteArrayValue_.length);
				  s = Conv.bytesToHexString(byteArrayValue_, 0, byteArrayValue_.Length);
				  break;
				case TYPE_BIG_DECIMAL:
				  s = bigDecimalValue_.ToString();
				  break;
				case TYPE_OBJECT:
				  s = objectValue_.ToString();
				  break;
				case TYPE_URL:
				  s = urlValue_.ToString();
				  break;
				case TYPE_ASCII_STREAM:
				case TYPE_BINARY_STREAM:
				  StringBuilder buf = new StringBuilder();
				  int count = 0;
				  int i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  break;
				case TYPE_UNICODE_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  sbyte hi = (sbyte)i;
					  i = inputStreamValue_.Read();
					  if (i >= 0)
					  {
						sbyte lo = (sbyte)i;
						// Make sure that the low byte is masked
						char c = (char)((hi << 8) | (lo & 0xff));
						buf.Append(c);
					  }
					}
					++count;
				  }
				  s = buf.ToString();
				  break;
				case TYPE_CHARACTER_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = readerValue_.read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  break;
    
				  default:
					  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			  }
			}
			catch (IOException io)
			{
			  SQLException sql = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Error reading from parameter stream: " + io.ToString());
			  sql.initCause(io);
			  throw sql;
			}
			return s;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getValueTimeAsString() throws SQLException
	  private string ValueTimeAsString
	  {
		  get
		  {
    
			string s = "UNSET";
			try
			{
			  switch (valueType_)
			  {
					case 0:
				  if (parameter_)
				  {
					  throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				  }
    
					case TYPE_INT:
					case TYPE_SHORT:
					case TYPE_LONG:
					case TYPE_FLOAT:
					case TYPE_DOUBLE:
					case TYPE_BYTE:
					case TYPE_BOOLEAN:
					case TYPE_BYTE_ARRAY:
					case TYPE_BIG_DECIMAL:
					case TYPE_URL:
					case TYPE_DATE:
					case TYPE_TIMESTAMP:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
    
				case TYPE_TIME:
				{
				  Time t = new Time(dateValue_.Ticks);
				  s = t.ToString();
				  break;
				}
				case TYPE_OBJECT:
				  s = objectValue_.ToString();
				  {
				  Time t = Time.valueOf(s);
				  s = t.ToString();
				  }
    
				  break;
				case TYPE_ASCII_STREAM:
				case TYPE_BINARY_STREAM:
				  StringBuilder buf = new StringBuilder();
				  int count = 0;
				  int i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				Time t = Time.valueOf(s);
				s = t.ToString();
				  }
    
				  break;
				case TYPE_UNICODE_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  sbyte hi = (sbyte)i;
					  i = inputStreamValue_.Read();
					  if (i >= 0)
					  {
						sbyte lo = (sbyte)i;
						// Make sure that the low byte is masked
						char c = (char)((hi << 8) | (lo & 0xff));
						buf.Append(c);
					  }
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				Time t = Time.valueOf(s);
				s = t.ToString();
				  }
    
				  break;
				case TYPE_CHARACTER_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = readerValue_.read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				Time ts = Time.valueOf(s);
				s = ts.ToString();
				  }
    
				  break;
				case TYPE_STRING :
				{
					s = stringValue_;
					Time t = Time.valueOf(s);
					s = t.ToString();
				}
    
				  goto default;
			  default:
				throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
    
    
			  }
			  //
			  // At this point, the time should have come from
			  // Time.toString() so it will be in the right format
			  //
			}
			catch (IOException io)
			{
			  SQLException sql = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Error reading from parameter stream: " + io.ToString());
			  sql.initCause(io);
			  throw sql;
			}
			catch (System.ArgumentException e)
			{
			SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, s);
			sqlex.initCause(e);
			throw sqlex;
			}
    
			return s;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getValueTimestampAsString() throws SQLException
	  private string ValueTimestampAsString
	  {
		  get
		  {
    
			string s = "NOT SET";
			try
			{
			  switch (valueType_)
			  {
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
    
				case TYPE_INT:
				case TYPE_SHORT:
				case TYPE_LONG:
				case TYPE_FLOAT:
				case TYPE_DOUBLE:
				case TYPE_BYTE:
				case TYPE_BOOLEAN:
				case TYPE_TIME:
				case TYPE_BYTE_ARRAY:
				case TYPE_BIG_DECIMAL:
				case TYPE_URL:
					throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of type " + valueType_ + " to timestamp not supported.");
    
				case TYPE_DATE:
			  s = dateValue_.ToString() + " 00:00:00";
			  {
				Timestamp ts = Timestamp.valueOf(s);
				s = ts.ToString();
			  }
			  break;
				case TYPE_TIMESTAMP:
				  s = dateValue_.ToString();
				  break;
				case TYPE_OBJECT:
				  s = objectValue_.ToString();
				  {
				Timestamp ts = Timestamp.valueOf(s);
				s = ts.ToString();
				  }
    
				  break;
				case TYPE_ASCII_STREAM:
				case TYPE_BINARY_STREAM:
				  StringBuilder buf = new StringBuilder();
				  int count = 0;
				  int i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				  Timestamp ts = Timestamp.valueOf(s);
				  s = ts.ToString();
				  }
    
				  break;
				case TYPE_UNICODE_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = inputStreamValue_.Read();
					if (i >= 0)
					{
					  sbyte hi = (sbyte)i;
					  i = inputStreamValue_.Read();
					  if (i >= 0)
					  {
						sbyte lo = (sbyte)i;
						// Make sure that the low byte is masked
						char c = (char)((hi << 8) | (lo & 0xff));
						buf.Append(c);
					  }
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				  Timestamp ts = Timestamp.valueOf(s);
				  s = ts.ToString();
				  }
    
				  break;
				case TYPE_CHARACTER_STREAM:
				  buf = new StringBuilder();
				  count = 0;
				  i = 0;
				  while (i >= 0 && count < inputStreamLength_)
				  {
					i = readerValue_.read();
					if (i >= 0)
					{
					  buf.Append((char)i);
					}
					++count;
				  }
				  s = buf.ToString();
				  {
				  Timestamp ts = Timestamp.valueOf(s);
				  s = ts.ToString();
				  }
    
				  break;
				case TYPE_STRING :
				{
				  s = isoTimestamp(stringValue_);
					Timestamp ts = Timestamp.valueOf(s);
					s = ts.ToString();
				}
    
				  goto default;
			  default:
				  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
			  }
			  while (s.Length < 26)
			  {
			  s = s + "0";
			  }
			  // Convert to IBM I format.
			  s = s.Substring(0,10) + "-" + s.Substring(11, 2) + "." + s.Substring(14, 2) + "." + s.Substring(17);
			}
			catch (IOException io)
			{
			  SQLException sql = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Error reading from parameter stream: " + io.ToString());
			  sql.initCause(io);
			  throw sql;
			}
			catch (System.ArgumentException e)
			{
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, s);
			  sqlex.initCause(e);
			  throw sqlex;
			}
    
			return s;
		  }
	  }

	  // Make sure the string is in ISO format.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String isoTimestamp(String s) throws SQLException
	  private string isoTimestamp(string s)
	  {
		//               1         2
		//     01234567890123456789012345
		// IBM 1999-10-10-12.12.12.123456
		// ISO 1999-10-10 12:12:12.123456
		if (s.Length < 18)
		{
		  JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		}
		if (s[10] == '-' || s[13] == '.' || s[16] == '.')
		{
		  char[] chars = s.ToCharArray();
		  int l = chars.Length;
		  if (l >= 11)
		  {
			  chars[10] = ' ';
		  }
		  if (l >= 14)
		  {
			  chars[13] = ':';
		  }
		  if (l >= 17)
		  {
			  chars[16] = ':';
		  }
		  return new string(chars);
		}
		return s;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private float getValueFloat() throws SQLException
	  private float ValueFloat
	  {
		  get
		  {
			  string stringValue = null;
			  try
			  {
			float f = floatValue_;
			switch (valueType_)
			{
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
    
			  case TYPE_INT:
				f = intValue_;
				break;
			  case TYPE_SHORT:
				f = shortValue_;
				break;
			  case TYPE_LONG:
				  f = longValue_;
				  break;
			  case TYPE_STRING:
			  stringValue = stringValue_;
				f = float.Parse(stringValue);
				break;
			  case TYPE_FLOAT:
				f = floatValue_;
				break;
			  case TYPE_DOUBLE:
				f = (float)doubleValue_;
				break;
			  case TYPE_BYTE:
				f = (float)byteValue_;
				break;
			  case TYPE_BOOLEAN:
				f = booleanValue_ ? (float) 1.0 : (float) 0.0;
				break;
			  case TYPE_DATE:
			  case TYPE_TIME:
			  case TYPE_TIMESTAMP:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of date/time/timestamp to float not supported.");
			  case TYPE_BYTE_ARRAY:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of byte array to float not supported.");
			  case TYPE_BIG_DECIMAL:
				f = bigDecimalValue_.floatValue();
				break;
			  case TYPE_OBJECT:
			  stringValue = objectValue_.ToString();
				f = float.Parse(stringValue);
				break;
			  case TYPE_URL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to float not supported.");
			  case TYPE_ASCII_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to float not supported.");
			  case TYPE_BINARY_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to float not supported.");
			  case TYPE_UNICODE_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to float not supported.");
			  case TYPE_CHARACTER_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to float not supported.");
			  default:
				  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			}
			return f;
			  }
			  catch (System.FormatException e)
			  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e);
			  throw sqlex;
			  }
    
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getValueDouble() throws SQLException
	  private double ValueDouble
	  {
		  get
		  {
			  string stringValue = null;
			  try
			  {
			double d = doubleValue_;
			switch (valueType_)
			{
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
    
			  case TYPE_INT:
				d = intValue_;
				break;
			  case TYPE_SHORT:
				d = shortValue_;
				break;
			  case TYPE_LONG:
				  d = longValue_;
				  break;
    
			  case TYPE_STRING:
			  stringValue = stringValue_;
				d = double.Parse(stringValue_);
				break;
			  case TYPE_FLOAT:
				d = floatValue_;
				break;
			  case TYPE_DOUBLE:
				d = doubleValue_;
				break;
			  case TYPE_BYTE:
				d = (double)byteValue_;
				break;
			  case TYPE_BOOLEAN:
				d = booleanValue_ ? (double) 1.0 : (double) 0.0;
				break;
			  case TYPE_DATE:
			  case TYPE_TIME:
			  case TYPE_TIMESTAMP:
					throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of date/time/timestamp to double not supported.");
			  case TYPE_BYTE_ARRAY:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of byte array to double not supported.");
			  case TYPE_BIG_DECIMAL:
				d = bigDecimalValue_.doubleValue();
				break;
			  case TYPE_OBJECT:
			  stringValue = objectValue_.ToString();
				d = double.Parse(stringValue);
				break;
			  case TYPE_URL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to double not supported.");
			  case TYPE_ASCII_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to double not supported.");
			  case TYPE_BINARY_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to double not supported.");
			  case TYPE_UNICODE_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to double not supported.");
			  case TYPE_CHARACTER_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to double not supported.");
			  default:
				  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			}
			return d;
			  }
			  catch (System.FormatException e)
			  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e);
			  throw sqlex;
			  }
    
		  }
	  }



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private long getValueLong() throws SQLException
	  private long ValueLong
	  {
		  get
		  {
    
			  string stringValue = null;
			  try
			  {
			long l;
			switch (valueType_)
			{
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
				case TYPE_LONG :
					l = longValue_;
					break;
			  case TYPE_INT:
				l = intValue_;
				break;
			  case TYPE_SHORT:
				l = shortValue_;
				break;
			  case TYPE_OBJECT:
				  // fall through
			  case TYPE_STRING:
			  {
				if (valueType_ == TYPE_OBJECT)
				{
					stringValue = objectValue_.ToString();
				}
				else
				{
					stringValue = stringValue_;
				}
				double doubleValue = double.Parse(stringValue);
				if (doubleValue > long.MaxValue || doubleValue < long.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				// If the doublevalue is in the range that can be accurately convert to
				// a long, just use the double conversion.  A double value has an implied 1.x
				// followed by 52 bits, so the precision is 53 bits.
				// Otherwise, we must rely on
				// parseLong, which will not handle strings of the form 134.53 or 12.3E20
				// 2^53=9,007,199,254,740,992
				if ((doubleValue < 9007199254740992L) && (doubleValue > -9007199254740992L))
				{
					l = (long) doubleValue;
				}
				else
				{
					try
					{
						l = long.Parse(stringValue);
					}
					catch (System.FormatException nfe)
					{
						int dotIndex = stringValue.IndexOf(".", StringComparison.Ordinal);
						if (dotIndex > 0)
						{
							stringValue = stringValue.Substring(0,dotIndex);
							l = long.Parse(stringValue);
						}
						else
						{
							throw nfe;
						}
					}
				}
    
			  }
				break;
			  case TYPE_FLOAT:
				if (floatValue_ > long.MaxValue || floatValue_ < long.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
    
				l = (long)floatValue_;
				break;
			  case TYPE_DOUBLE:
				if (doubleValue_ > long.MaxValue || doubleValue_ < long.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				l = (long)doubleValue_;
				break;
			  case TYPE_BYTE:
				l = (long)byteValue_;
				break;
			  case TYPE_BOOLEAN:
				l = booleanValue_ ? 1L : 0L;
				break;
			  case TYPE_DATE:
			  case TYPE_TIME:
			  case TYPE_TIMESTAMP:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  case TYPE_BYTE_ARRAY:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of byte array to long not supported.");
			  case TYPE_BIG_DECIMAL:
			  {
				double doubleValue = bigDecimalValue_.doubleValue();
				if (doubleValue > long.MaxValue || doubleValue < long.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
    
				l = bigDecimalValue_.longValue();
			  }
				break;
			  case TYPE_URL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to long not supported.");
			  case TYPE_ASCII_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to long not supported.");
			  case TYPE_BINARY_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to long not supported.");
			  case TYPE_UNICODE_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to long not supported.");
			  case TYPE_CHARACTER_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to long not supported.");
				default:
					throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			}
			return l;
			  }
			  catch (System.FormatException e)
			  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e);
			  throw sqlex;
			  }
    
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int getValueInt() throws SQLException
	  private int ValueInt
	  {
		  get
		  {
			  string stringValue = null;
			  try
			  {
			  int i;
			  switch (valueType_)
			  {
			  case 0:
				if (parameter_)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
			  case TYPE_INT :
				  i = intValue_;
				  break;
			  case TYPE_LONG:
				if (longValue_ > int.MaxValue || longValue_ < int.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				i = (int) longValue_;
				  break;
				  case TYPE_SHORT:
				  i = shortValue_;
				  break;
				  case TYPE_STRING:
				  {
				double doubleValue = double.Parse(stringValue_);
				if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
    
				i = (int) doubleValue;
				  }
				break;
				  case TYPE_FLOAT:
					if (floatValue_ > int.MaxValue || floatValue_ < int.MinValue)
					{
					  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
					}
					i = (int)floatValue_;
				  break;
				  case TYPE_DOUBLE:
					if (doubleValue_ > int.MaxValue || doubleValue_ < int.MinValue)
					{
					  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
					}
				  i = (int)doubleValue_;
				  break;
				  case TYPE_BYTE:
				  i = (int)byteValue_;
				  break;
				  case TYPE_BOOLEAN:
				  i = booleanValue_ ? 1 : 0;
				  break;
				  case TYPE_DATE:
				  case TYPE_TIME:
				  case TYPE_TIMESTAMP:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				  case TYPE_BYTE_ARRAY:
					  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, "Conversion from byte array to integer not supported");
				  case TYPE_BIG_DECIMAL:
				  {
					double doubleValue = bigDecimalValue_.doubleValue();
					if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
					{
					  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
					}
    
					i = (int) doubleValue;
				  }
    
				  break;
				  case TYPE_OBJECT:
				  {
					double doubleValue = double.Parse(objectValue_.ToString());
					if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
					{
					  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
					}
    
					i = (int) doubleValue;
				  }
				  break;
				  case TYPE_URL:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to int not supported.");
				  case TYPE_ASCII_STREAM:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to int not supported.");
				  case TYPE_BINARY_STREAM:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to int not supported.");
				  case TYPE_UNICODE_STREAM:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to int not supported.");
				  case TYPE_CHARACTER_STREAM:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to int not supported.");
    
    
				  default:
						throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			  }
			  return i;
			  }
			  catch (System.FormatException e)
			  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e);
			  throw sqlex;
			  }
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private short getValueShort() throws SQLException
	  private short ValueShort
	  {
		  get
		  {
			  string stringValue = null;
			  try
			  {
			short sh;
			switch (valueType_)
			{
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
				case TYPE_SHORT :
					sh = shortValue_;
					break;
			  case TYPE_LONG:
				if (longValue_ > short.MaxValue || longValue_ < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short)longValue_;
				break;
			  case TYPE_INT:
				if (intValue_ > short.MaxValue || intValue_ < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short)intValue_;
				break;
			  case TYPE_STRING:
			  {
				double doubleValue = Convert.ToDouble(stringValue_);
				if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short) doubleValue;
			  }
				break;
			  case TYPE_FLOAT:
				if (floatValue_ > short.MaxValue || floatValue_ < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short)floatValue_;
				break;
			  case TYPE_DOUBLE:
				if (doubleValue_ > short.MaxValue || doubleValue_ < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short)doubleValue_;
				break;
			  case TYPE_BYTE:
				sh = (short)byteValue_;
				break;
			  case TYPE_BOOLEAN:
				sh = (short)(booleanValue_ ? 1 : 0);
				break;
			  case TYPE_DATE:
			  case TYPE_TIME:
			  case TYPE_TIMESTAMP:
				  // This conversion not possible
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  case TYPE_BYTE_ARRAY:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of byte array to short not supported.");
			  case TYPE_BIG_DECIMAL:
			  {
				double doubleValue = bigDecimalValue_.doubleValue();
				if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short) doubleValue;
			  }
				break;
			  case TYPE_OBJECT:
			  {
				double doubleValue = Convert.ToDouble(objectValue_.ToString());
				if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
				{
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
				}
				sh = (short) doubleValue;
			  }
				break;
			  case TYPE_URL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to short not supported.");
			  case TYPE_ASCII_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to short not supported.");
			  case TYPE_BINARY_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to short not supported.");
			  case TYPE_UNICODE_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to short not supported.");
			  case TYPE_CHARACTER_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to short not supported.");
    
    
    
			  default:
					throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
			}
			return sh;
			  }
			  catch (System.FormatException e)
			  {
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e);
			  throw sqlex;
			  }
    
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] getValueByteArray() throws SQLException
	  private sbyte[] ValueByteArray
	  {
		  get
		  {
			  sbyte[] ba = byteArrayValue_;
			switch (valueType_)
			{
				case 0:
				if (parameter_)
				{
					throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
				}
			  case TYPE_BYTE_ARRAY:
				ba = byteArrayValue_;
				break;
    
			  case TYPE_STRING:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of string to byte array not supported.");
			  case TYPE_LONG:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of long to byte array not supported.");
			  case TYPE_INT:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of int to byte array not supported.");
			  case TYPE_SHORT:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of short to byte array not supported.");
			  case TYPE_FLOAT:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of float to byte array not supported.");
			  case TYPE_DOUBLE:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of doubleto byte array not supported.");
			  case TYPE_BYTE:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of byte to byte array not supported.");
			  case TYPE_BOOLEAN:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of boolean to byte array not supported.");
			  case TYPE_DATE:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of date to byte array not supported.");
			  case TYPE_TIME:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of time to byte array not supported.");
			  case TYPE_TIMESTAMP:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of timestamp to byte array not supported.");
			  case TYPE_BIG_DECIMAL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of BIGDECIMAL  to byte array not supported.");
			  case TYPE_OBJECT:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of OBJECT to byte array not supported.");
			  case TYPE_URL:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of URL to byte array not supported.");
			  case TYPE_ASCII_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of ASCII stream to byte array not supported.");
			  case TYPE_BINARY_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of binary stream to byte array not supported.");
			  case TYPE_UNICODE_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of Unicode stream to byte array not supported.");
			  case TYPE_CHARACTER_STREAM:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Conversion of character stream to byte array not supported.");
    
			  default:
					throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unrecognized valueType " + valueType_);
    
    
			}
			return ba;
    
		  }
	  }




	  internal bool Null
	  {
		  get
		  {
			return null_;
		  }
		  set
		  {
			null_ = value;
		  }
	  }


	  internal int Offset
	  {
		  get
		  {
			return offset_;
		  }
		  set
		  {
			offset_ = value;
		  }
	  }


	  internal string Name
	  {
		  get
		  {
			return name_;
		  }
		  set
		  {
			name_ = value;
		  }
	  }


	  internal string UdtName
	  {
		  get
		  {
			return udtName_;
		  }
		  set
		  {
			udtName_ = value;
		  }
	  }


	  internal string Table
	  {
		  get
		  {
			return table_;
		  }
		  set
		  {
			table_ = value;
		  }
	  }


	  internal string Schema
	  {
		  get
		  {
			return schema_;
		  }
		  set
		  {
			schema_ = value;
		  }
	  }


	  internal string Label
	  {
		  get
		  {
			return label_;
		  }
		  set
		  {
			label_ = value;
		  }
	  }


	  internal bool AutoIncrement
	  {
		  get
		  {
			return autoIncrement_;
		  }
		  set
		  {
			autoIncrement_ = value;
		  }
	  }


	  internal bool DefinitelyWritable
	  {
		  get
		  {
			return definitelyWritable_;
		  }
		  set
		  {
			definitelyWritable_ = value;
		  }
	  }


	  internal bool ReadOnly
	  {
		  get
		  {
			return readOnly_;
		  }
		  set
		  {
			readOnly_ = value;
		  }
	  }


	  internal bool Searchable
	  {
		  get
		  {
			return searchable_;
		  }
		  set
		  {
			searchable_ = value;
		  }
	  }


	  internal bool Writable
	  {
		  get
		  {
			return writable_;
		  }
		  set
		  {
			writable_ = value;
		  }
	  }


	  internal int Type
	  {
		  get
		  {
			return type_;
		  }
		  set
		  {
			type_ = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getSQLType() throws SQLException
	  internal int SQLType
	  {
		  get
		  {
			switch (type_ & 0xFFFE)
			{
			  case DB2Type.DATE: // DATE
				return java.sql.Types.DATE;
			  case DB2Type.TIME: // TIME
				return java.sql.Types.TIME;
			  case DB2Type.TIMESTAMP: // TIMESTAMP
				return java.sql.Types.TIMESTAMP;
			  case DB2Type.DATALINK:
				return java.sql.Types.DATALINK;
			  case DB2Type.BLOB: // BLOB
				return java.sql.Types.BLOB;
			  case DB2Type.CLOB: // CLOB
			  case DB2Type.DBCLOB: // DBCLOB
				return java.sql.Types.CLOB;
			  case DB2Type.VARCHAR: // VARCHAR
				return java.sql.Types.VARCHAR;
			  case DB2Type.CHAR: // CHAR
				return java.sql.Types.CHAR;
			  case DB2Type.LONGVARCHAR: // LONG VARCHAR
				return java.sql.Types.VARCHAR;
			  case DB2Type.VARGRAPHIC: // VARGRAPHIC
				return java.sql.Types.VARCHAR;
			  case DB2Type.GRAPHIC: // GRAPHIC
				return java.sql.Types.CHAR;
			  case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
				return java.sql.Types.VARCHAR;
			  case DB2Type.FLOATINGPOINT: // floating point
			  {
				if (length_ == 4)
				{
				  return java.sql.Types.REAL;
				}
				else
				{
				  return java.sql.Types.DOUBLE;
				}
    
			  }
				  goto case DB2Type.DECIMAL;
			  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
				return java.sql.Types.DECIMAL;
			  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
				return java.sql.Types.NUMERIC;
			  case DB2Type.BIGINT: // BIGINT
				return java.sql.Types.BIGINT;
			  case DB2Type.INTEGER: // INTEGER
				return java.sql.Types.INTEGER;
			  case DB2Type.SMALLINT: // SMALLINT
				return java.sql.Types.SMALLINT;
			  case DB2Type.ROWID: // ROWID
			  {
				  // ROWID was added in JDK 1.6
				  // Since this is a simple driver, we'll call it varbinary
				  return java.sql.Types.VARBINARY;
			  }
			  case DB2Type.VARBINARY: // VARBINARY
				return java.sql.Types.VARBINARY;
			  case DB2Type.BINARY: // BINARY
				return java.sql.Types.BINARY;
			  case DB2Type.BLOB_LOCATOR: // BLOB locator
				return java.sql.Types.BLOB;
			  case DB2Type.CLOB_LOCATOR: // CLOB locator
			  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
				return java.sql.Types.CLOB;
			  case DB2Type.XML: // XML
			  case DB2Type.XML_LOCATOR: // XML
			  {
				  // SQLXML was added in JDK 1.6.
				  // For now, call it a clob
				  return java.sql.Types.CLOB;
			  }
			  case DB2Type.DECFLOAT: // DECFLOAT
				return java.sql.Types.OTHER;
			  default:
				throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database column type: " + type_);
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSQLTypeName() throws SQLException
	  internal string SQLTypeName
	  {
		  get
		  {
			switch (type_ & 0xFFFE)
			{
			  case DB2Type.DATE: // DATE
				return "DATE";
			  case DB2Type.TIME: // TIME
				return "TIME";
			  case DB2Type.TIMESTAMP: // TIMESTAMP
				return "TIMESTAMP";
			  case DB2Type.DATALINK: // DATALINK
			  return "DATALINK";
			  case DB2Type.BLOB: // BLOB
			  return "BLOB";
			  case DB2Type.CLOB: // CLOB
				return "CLOB";
			  case DB2Type.DBCLOB: // DBCLOB
				  return "DBCLOB";
    
			  case DB2Type.VARCHAR: // VARCHAR
				if (isForBitData_)
				{
				  return "VARCHAR () FOR BIT DATA";
				}
				else
				{
				  return "VARCHAR";
				}
			  case DB2Type.CHAR: // CHAR
				if (isForBitData_)
				{
				  return "CHAR () FOR BIT DATA";
				}
				else
				{
				  return "CHAR";
				}
			  case DB2Type.LONGVARCHAR: // LONG VARCHAR
				return "VARCHAR";
			  case DB2Type.VARGRAPHIC: // VARGRAPHIC
				return "VARGRAPHIC";
			  case DB2Type.GRAPHIC: // GRAPHIC
				return "GRAPHIC";
			  case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
				return "VARGRAPHIC";
			  case DB2Type.FLOATINGPOINT:
			  {
				if (length_ == 4)
				{
				  return "REAL";
				}
				else
				{
				  return "DOUBLE";
				}
			  }
				  goto case DB2Type.DECIMAL;
			  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
				return "DECIMAL";
			  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
				return "NUMERIC";
			  case DB2Type.BIGINT: // BIGINT
				return "BIGINT";
			  case DB2Type.INTEGER: // INTEGER
				return "INTEGER";
			  case DB2Type.SMALLINT: // SMALLINT
				return "SMALLINT";
			  case DB2Type.ROWID: // ROWID
				return "ROWID";
			  case DB2Type.VARBINARY: // VARBINARY
				if (isForBitData_)
				{
				  return "VARCHAR () FOR BIT DATA";
				}
				else
				{
				  return "VARBINARY";
				}
			  case DB2Type.BINARY: // BINARY
				if (isForBitData_)
				{
				  return "CHAR () FOR BIT DATA";
				}
				else
				{
				return "BINARY";
				}
			  case DB2Type.BLOB_LOCATOR: // BLOB LOCATOR
				return "BLOB LOCATOR";
			  case DB2Type.CLOB_LOCATOR: // CLOB locator
				return "CLOB LOCATOR";
			  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
				return "DBCLOB";
			  case DB2Type.XML: // SQLXML
			  return "SQLXML";
			  case DB2Type.XML_LOCATOR: // SQLXML
				  return "XML"; // Matches other JDBC drivers
			  case DB2Type.DECFLOAT: // DECFLOAT
			return "DECFLOAT";
    
			  default:
				throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database column type: " + type_);
			}
		  }
	  }

	  internal int Length
	  {
		  get
		  {
			return length_;
		  }
		  set
		  {
			length_ = value;
		  }
	  }

	  /*
	   * Returns the length of the declared type.
	   */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getDeclaredLength() throws SQLException
	  internal int DeclaredLength
	  {
		  get
		  {
			  if (declaredLength_ == 0)
			  {
					switch (type_ & 0xFFFE)
					{
					case DB2Type.VARBINARY:
					case DB2Type.VARCHAR:
					case DB2Type.DATALINK:
					case DB2Type.LONGVARCHAR:
					case DB2Type.ROWID:
						declaredLength_ = length_ - 2;
						break;
					case DB2Type.VARGRAPHIC:
					case DB2Type.LONGVARGRAPHIC:
						declaredLength_ = (length_ - 2) / 2;
						break;
					case DB2Type.BLOB:
					case DB2Type.CLOB:
						declaredLength_ = length_ - 4;
						break;
					case DB2Type.DBCLOB:
						declaredLength_ = (length_ - 4) / 2;
						break;
					case DB2Type.GRAPHIC:
						declaredLength_ = length_ / 2;
						break;
					case DB2Type.DECIMAL:
					case DB2Type.NUMERIC:
						declaredLength_ = precision_;
						break;
					case DB2Type.BLOB_LOCATOR:
					case DB2Type.CLOB_LOCATOR:
					case DB2Type.DBCLOB_LOCATOR:
					case DB2Type.XML_LOCATOR:
						declaredLength_ = lobMaxSize_;
						break;
					case DB2Type.DECFLOAT:
						if (length_ == 8)
						{
							declaredLength_ = 16;
						}
						else if (length_ == 16)
						{
							declaredLength_ = 34;
						}
						else
						{
							throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL, "Unknown DECFLOAT length= " + length_);
						}
						break;
					case DB2Type.TIMESTAMP:
						declaredLength_ = 26;
						break;
					case DB2Type.TIME:
						declaredLength_ = 8;
						break;
					case DB2Type.DATE:
						declaredLength_ = 10;
						break;
    
							// TBD
					default:
						declaredLength_ = length_;
    
    
					break;
					}
			  }
			  return declaredLength_;
		  }
	  }

	  internal int Scale
	  {
		  get
		  {
			return scale_;
		  }
		  set
		  {
			scale_ = value;
		  }
	  }


	  internal int Precision
	  {
		  get
		  {
			return precision_;
		  }
		  set
		  {
			precision_ = value;
		  }
	  }


	  public int LobMaxSize
	  {
		  set
		  {
			  lobMaxSize_ = value;
		  }
		  get
		  {
			  return lobMaxSize_;
		  }
	  }


	  internal int CCSID
	  {
		  get
		  {
			return ccsid_;
		  }
		  set
		  {
			ccsid_ = value;
			// If the value is 65535 switch the type if chartype
			if (ccsid_ == 65535)
			{
			   switch (type_)
			   {
				 case DB2Type.CHAR:
					 type_ = DB2Type.BINARY;
					 isForBitData_ = true;
					 break;
				 case DB2Type.CHAR + 1:
					 type_ = DB2Type.BINARY + 1;
					 isForBitData_ = true;
					 break;
				 case DB2Type.VARCHAR:
					 type_ = DB2Type.VARBINARY;
					 isForBitData_ = true;
					 break;
				 case DB2Type.VARCHAR + 1:
					 type_ = DB2Type.VARBINARY + 1;
					 isForBitData_ = true;
					 break;
				 case DB2Type.LONGVARCHAR:
					 type_ = DB2Type.VARBINARY;
					 isForBitData_ = true;
					 break;
				 case DB2Type.LONGVARCHAR + 1:
					 type_ = DB2Type.VARBINARY + 1;
					 isForBitData_ = true;
					 break;
			   }
    
			}
		  }
	  }


	  internal int DateFormat
	  {
		  set
		  {
			dateFormat_ = value;
		  }
	  }

	  internal int TimeFormat
	  {
		  set
		  {
			timeFormat_ = value;
		  }
	  }

	  internal int DateSeparator
	  {
		  set
		  {
			dateSeparator_ = value;
		  }
	  }

	  internal int TimeSeparator
	  {
		  set
		  {
			timeSeparator_ = value;
		  }
	  }

	  internal bool UseDateCache
	  {
		  set
		  {
			useDateCache_ = value;
			dateCache_ = value ? new Hashtable() : null;
		  }
	  }

	  internal bool UseTimeCache
	  {
		  set
		  {
			useTimeCache_ = value;
			timeCache_ = value ? new Hashtable() : null;
		  }
	  }

	  internal bool UseStringCache
	  {
		  set
		  {
			useStringCache_ = value;
			cache_ = value ? new Hashtable() : null;
		  }
	  }

	  internal bool CacheLastOnly
	  {
		  set
		  {
			cacheLastOnly_ = value;
		  }
	  }

	  private readonly ByteArrayKey key_ = new ByteArrayKey();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private String lookupString(final byte[] data, final int offset, final int length)
	  private string lookupString(sbyte[] data, int offset, int length)
	  {
		key_.setHashData(data, offset, length);
		return (string)cache_[key_];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Date lookupDate(final byte[] data, final int offset, final int length)
	  private DateTime lookupDate(sbyte[] data, int offset, int length)
	  {
		key_.setHashData(data, offset, length);
		return (DateTime)dateCache_[key_];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Time lookupTime(final byte[] data, final int offset, final int length)
	  private Time lookupTime(sbyte[] data, int offset, int length)
	  {
		key_.setHashData(data, offset, length);
		return (Time)timeCache_[key_];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void cache(final byte[] data, final int offset, final int length, final String value)
	  private void cache(sbyte[] data, int offset, int length, string value)
	  {
		sbyte[] key = new sbyte[length];
		Array.Copy(data, offset, key, 0, length);
		if (cacheLastOnly_)
		{
			cache_.Clear();
		}
		cache_[new ByteArrayKey(key)] = value;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void cache(final byte[] data, final int offset, final int length, final Date value)
	  private void cache(sbyte[] data, int offset, int length, DateTime value)
	  {
		sbyte[] key = new sbyte[length];
		Array.Copy(data, offset, key, 0, length);
		dateCache_.Clear();
		dateCache_[new ByteArrayKey(key)] = value;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void cache(final byte[] data, final int offset, final int length, final Time value)
	  private void cache(sbyte[] data, int offset, int length, Time value)
	  {
		sbyte[] key = new sbyte[length];
		Array.Copy(data, offset, key, 0, length);
		timeCache_.Clear();
		timeCache_[new ByteArrayKey(key)] = value;
	  }

	  // Convert a string to bytes in an output buffer.
	  // Throws truncation exception after
	  // converting as much as possible.
	  // returns length in bytes
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int convertString(final String s, final byte[] data, final int offset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  private int convertString(string s, sbyte[] data, int offset)
	  {
		  int outLength = 0;
		  bool truncated = false;
		int length = s.Length;
		int declaredLength = DeclaredLength;
		if (length > declaredLength)
		{
			  truncated = true;
			  length = declaredLength;
		}
		switch (ccsid_)
		{
		  case 13488:
		  case 1200:
			outLength = Conv.stringToUnicodeByteArray(s, length, data, offset);
			break;
		  case 65535:
			outLength = Conv.stringToEBCDICByteArray37(s, length, data, offset);
			break;
		  case 1208:
			outLength = Conv.stringToUtf8ByteArray(s, length, data, offset);
			break;
		  default:
			try
			{
					  outLength = Conv.stringToEBCDICByteArray(s, length, data, offset, ccsid_);
			}
				  catch (UnsupportedEncodingException e)
				  {
					  SQLException sqlex = JDBCError.getSQLException("22524");
					  sqlex.initCause(e);
					  throw sqlex;
				  }
		}

		if (truncated)
		{
		throw new DataTruncation(index_, parameter_, false, s.Length, length); // transferSize
		}
		return outLength;
	  }

	  //
	  // Convert column to bytes.  Throws truncation exception after converting as much as possible.
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void convertToBytes(final byte[] data, final int offset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal void convertToBytes(sbyte[] data, int offset)
	  {
		bool truncated = false;
		int length = 0;
		int dataSize = 0;
		try
		{
		  if ((valueType_ == 0) && parameter_)
		  {
			  throw JDBCError.getSQLException(JDBCError.EXC_PARAMETER_COUNT_MISMATCH);
		  }


		if (!null_)
		{
		  int len = 0;
		  string s = stringValue_;
		  float f = floatValue_;
		  double d = doubleValue_;

		  long l = longValue_;
		  int i = intValue_;
		  short sh = shortValue_;
		  sbyte[] ba = byteArrayValue_;
		  switch (type_ & 0xFFFE)
		  {

			case DB2Type.DATE: // DATE
			  switch (dateFormat_)
			  {
				case 1: // MDY
				default:
				  s = ValueString;
				  convertString(s, data, offset);
				  break;
			  }
			  break;
			case DB2Type.TIME: // TIME
			  s = ValueTimeAsString;
			  convertString(s, data, offset);
			  break;

			case DB2Type.TIMESTAMP: // TIMESTAMP
		  s = ValueTimestampAsString;
		  convertString(s, data, offset);
		  break;
			  //TODO
			case DB2Type.DATALINK: // Datalink
			  s = ValueString;
			  len = convertString(s, data, offset + 2);
			  Conv.shortToByteArray(len, data, offset);
			  break;

		  case DB2Type.BLOB: // BLOB
		  case DB2Type.CLOB: // CLOB
		  case DB2Type.DBCLOB: // DBCLOB
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Lob  not implemented yet");

			case DB2Type.VARCHAR: // VARCHAR
			case DB2Type.LONGVARCHAR: // LONG VARCHAR
			  s = ValueString;
			  len = convertString(s, data, offset + 2);
			  Conv.shortToByteArray(len, data, offset);
			  break;

			case DB2Type.CHAR: // CHAR
			  s = ValueString;
			  while (s.Length < length_)
			  {
				  s = s + " "; //TODO
			  }
			  convertString(s, data, offset);
			  break;
			case DB2Type.VARGRAPHIC: // VARGRAPHIC
			case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
			  s = ValueString;
		  // length is in bytes
			  len = Conv.stringToUnicodeByteArray(s, data, offset + 2);
			  Conv.shortToByteArray(len / 2, data, offset);
			  break;
			case DB2Type.GRAPHIC: // GRAPHIC
			  s = ValueString;
			  Conv.stringToUnicodeByteArray(s, data, offset, length_);
			  break;
			case DB2Type.FLOATINGPOINT: // Float
			if (length_ == 4)
			{
			f = ValueFloat;
			Conv.floatToByteArray(f, data, offset);
			}
			else if (length_ == 8)
			{
			d = ValueDouble;
			Conv.doubleToByteArray(d, data, offset);
			}
			else
			{
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_ + " length= " + length_);
			}
			break;
			case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			  s = NonexponentValueString;
			  s = formatDecimal(s, precision_, scale_);
			  Conv.stringToPackedDecimal(s, precision_, data, offset);
			  break;

			case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			  s = NonexponentValueString;
				s = formatDecimal(s, precision_, scale_);
			  Conv.stringToZonedDecimal(s, precision_, data, offset);
			  break;
			case DB2Type.BIGINT: // BIGINT
			  l = ValueLong;
			  Conv.longToByteArray(l, data, offset);
			  break;
			case DB2Type.INTEGER: // INTEGER
			  i = ValueInt;
			  Conv.intToByteArray(i, data, offset);
			  break;
			case DB2Type.SMALLINT: // SMALLINT
			  sh = ValueShort;
			  Conv.shortToByteArray(sh, data, offset);
			  break;
		  case DB2Type.ROWID: // ROWID
		  case DB2Type.VARBINARY: // VARBINARY
		  ba = ValueByteArray;
		  length = ba.Length;
		  if (length + 2 > length_)
		  {
			truncated = true;
			dataSize = length;
			length = length_ - 2;
		  }
		  for (int z = 0; z < length; z++)
		  {
			  data[offset + 2 + z] = ba[z];
		  }
			  Conv.shortToByteArray(ba.Length, data, offset);

			  break;
			case DB2Type.BINARY: // BINARY
		  ba = ValueByteArray;
		length = ba.Length;
		if (length > length_)
		{
		  dataSize = length;
		  truncated = true;
		  length = length_;
		}

		  for (int z = 0; z < length; z++)
		  {
			  data[offset + z] = ba[z];
		  }

			  break;
		  case DB2Type.BLOB_LOCATOR: // BLOB locator
		  case DB2Type.CLOB_LOCATOR: // CLOB locator
		  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
		  case DB2Type.XML_LOCATOR:
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"LOB locator not implemented yet");

		  case DB2Type.XML: // XML
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"XML not implemented yet");
		  case DB2Type.DECFLOAT: // DECFLOAT
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"DECFLOAT not implemented yet");

			default:
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_);
		  }
		}
		}
		catch (System.FormatException nfe)
		{
			SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			sqlex.initCause(nfe);
			throw sqlex;
		}

		if (truncated)
		{
		  throw new DataTruncation(index_, parameter_, false, dataSize, length); // transferSize
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte convertToByte(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal sbyte convertToByte(sbyte[] data, int rowOffset)
	  {
		string stringValue = null;
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset + offset_;
		  int offset = rowOffset + offset_;
		  switch (type_ & 0xFFFE)
		  {
		  case DB2Type.BIGINT: // BIGINT
		  {
			long longValue = convertToLong(data, rowOffset);
			if (longValue > sbyte.MaxValue || longValue < sbyte.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (sbyte) longValue;
			}
		  }
			  goto case DB2Type.INTEGER;
		  case DB2Type.INTEGER: // INTEGER
		  {
			int intValue = convertToInt(data, rowOffset);
			if (intValue > sbyte.MaxValue || intValue < sbyte.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (sbyte) intValue;
			}
		  }
			  goto case DB2Type.SMALLINT;
		  case DB2Type.SMALLINT: // SMALLINT
		  {
			int shortValue = convertToShort(data, rowOffset);
			if (shortValue > sbyte.MaxValue || shortValue < sbyte.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (sbyte) shortValue;
			}
		  }
			  goto case DB2Type.FLOATINGPOINT;
		  case DB2Type.FLOATINGPOINT: // floating point:
			if (length_ == 4)
			{
			  float floatValue = Conv.byteArrayToFloat(data, offset);
			  if (floatValue > sbyte.MaxValue || floatValue < sbyte.MinValue)
			  {
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  }
			  else
			  {
				return (sbyte) floatValue;
			  }
			}
			else if (length_ == 8)
			{
			  double doubleValue = Conv.byteArrayToDouble(data, offset);
			  if (doubleValue > sbyte.MaxValue || doubleValue < sbyte.MinValue)
			  {
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  }
			  else
			  {
				return (sbyte) doubleValue;
			  }
			}
			  goto case DB2Type.VARBINARY;
		  case DB2Type.VARBINARY: // VARBINARY
			int varblen = Conv.byteArrayToShort(data, offset);
			if (varblen == 1)
			{
			  return data[offset + 2];
			}
			else
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  case DB2Type.BINARY: // BINARY
			if (length_ == 1)
			{
			  return data[offset];
			}
			else
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  default:
			stringValue = convertToString(data, rowOffset);
			double? doubleValue = Convert.ToDouble(stringValue);
			double d = doubleValue.Value;


			if (d > sbyte.MaxValue || d < sbyte.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return doubleValue.Value;
			}

		  }
		}
		catch (System.FormatException e)
		{
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double convertToDouble(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal double convertToDouble(sbyte[] data, int rowOffset)
	  {
		  string stringValue = null;
		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			return Conv.packedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			return Conv.zonedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.BIGINT: // BIGINT
			return (double)Conv.byteArrayToLong(data, offset);
		  case DB2Type.INTEGER: // INTEGER
			return (double)Conv.byteArrayToInt(data, offset);
		  case DB2Type.SMALLINT: // SMALLINT
			return (double)Conv.byteArrayToShort(data, offset);
		  case DB2Type.FLOATINGPOINT: // floating point:
			if (length_ == 4)
			{
			  return Conv.byteArrayToFloat(data, offset);
			}
			else if (length_ == 8)
			{
			  return Conv.byteArrayToDouble(data, offset);
			}
		  case DB2Type.VARBINARY: // VARBINARY
		  case DB2Type.BINARY: // BINARY
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  default:
		  stringValue = convertToString(data, rowOffset);
			return double.Parse(stringValue);
		}
		  }
		  catch (System.FormatException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: float convertToFloat(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal float convertToFloat(sbyte[] data, int rowOffset)
	  {
		  string stringValue = null;
		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			return (float)Conv.packedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			return (float)Conv.zonedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.BIGINT: // BIGINT
			return (float)Conv.byteArrayToLong(data, offset);
		  case DB2Type.INTEGER: // INTEGER
			return (float)Conv.byteArrayToInt(data, offset);
		  case DB2Type.SMALLINT: // SMALLINT
			return (float)Conv.byteArrayToShort(data, offset);
		  case DB2Type.VARBINARY: // VARBINARY
		  case DB2Type.BINARY: // BINARY
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  case DB2Type.FLOATINGPOINT: // floating point:
			if (length_ == 4)
			{
			  return Conv.byteArrayToFloat(data, offset);
			}
			else if (length_ == 8)
			{
			  double d = Conv.byteArrayToDouble(data, offset);
			  // Allow these special values to be returned.
			  if (d == double.NegativeInfinity || d == double.PositiveInfinity || double.IsNaN(d))
			  {
				return (float) d;
			  }
			  if (d > float.MaxValue || d < - float.MaxValue)
			  {
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  }
			  else
			  {
				return (float) d;
			  }
			}

			  goto default;
		  default:
			stringValue = convertToString(data, rowOffset).Trim();
		  double? doubleValue = Convert.ToDouble(stringValue);
		  double d = doubleValue.Value;

		  if (d == double.NegativeInfinity || d == double.PositiveInfinity || double.IsNaN(d))
		  {
			 return (float) d;
		  }

		  if (d > float.MaxValue || d < - float.MaxValue)
		  {
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  }
		  else
		  {
			return doubleValue.Value;
		  }

		}
		  }
		  catch (System.FormatException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: short convertToShort(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal short convertToShort(sbyte[] data, int rowOffset)
	  {
		  string stringValue = null;
		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		case DB2Type.FLOATINGPOINT: // floating point:
		  if (length_ == 4)
		  {
			float floatValue = Conv.byteArrayToFloat(data, offset);
			if (floatValue > short.MaxValue || floatValue < short.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (short) floatValue;
			}
		  }
		  else if (length_ == 8)
		  {
			double doubleValue = Conv.byteArrayToDouble(data, offset);
			if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (short) doubleValue;
			}
		  }
			  goto case DB2Type.DECIMAL;
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
		  {
			double doubleValue = Conv.packedDecimalToDouble(data, offset, precision_, scale_);
			if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (short) doubleValue;
			}

		  }
			  goto case DB2Type.NUMERIC;
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
		  {
			double doubleValue = Conv.zonedDecimalToDouble(data, offset, precision_, scale_);
			if (doubleValue > short.MaxValue || doubleValue < short.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (short) doubleValue;
			}
		  }
			  goto case DB2Type.BIGINT;
		  case DB2Type.BIGINT: // BIGINT
		  {
			long longValue = Conv.byteArrayToLong(data, offset);
			if (longValue > short.MaxValue || longValue < short.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (short) longValue;
			}
		  }

			  goto case DB2Type.INTEGER;
		  case DB2Type.INTEGER: // INTEGER
		  {
			  int intValue = Conv.byteArrayToInt(data, offset);
			  if (intValue > short.MaxValue || intValue < short.MinValue)
			  {
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  }
			  else
			  {
				return (short) intValue;
			  }
		  }
			  goto case DB2Type.SMALLINT;
		  case DB2Type.SMALLINT: // SMALLINT
			return (short)Conv.byteArrayToShort(data, offset);
		  case DB2Type.VARBINARY: // VARBINARY
			int varblen = Conv.byteArrayToShort(data, offset);
			if (varblen > 2)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			switch (varblen)
			{
			  case 0:
				  return 0;
			  case 1:
				  return (short)(data[offset + 2] & 0x00FF);
			  case 2:
				  return (short)Conv.byteArrayToShort(data, offset + 2);
			  default:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  case DB2Type.BINARY: // BINARY
			int len = length_;
			if (len > 2)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			switch (len)
			{
			  case 0:
				  return 0;
			  case 1:
				  return (short)(data[offset] & 0x00FF);
			  case 2:
				  return (short)Conv.byteArrayToShort(data, offset);
			  default:
				throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  default:
			  stringValue = convertToString(data, rowOffset);
			double? doubleValue = Convert.ToDouble(stringValue.Trim());
			double d = doubleValue.Value;
			if (d > short.MaxValue || d < short.MinValue)
			{
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  throw sqlex;
			}
			return doubleValue.Value;
		}
		  }
		  catch (System.FormatException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int convertToInt(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal int convertToInt(sbyte[] data, int rowOffset)
	  {
		  string stringValue = null;
		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
		  {
			double doubleValue = Conv.packedDecimalToDouble(data, offset, precision_, scale_);
			if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (int) doubleValue;
			}
		  }
			  goto case DB2Type.NUMERIC;
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
		  {
			double doubleValue = Conv.zonedDecimalToDouble(data, offset, precision_, scale_);
			if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			else
			{
			  return (int) doubleValue;
			}
		  }

			  goto case DB2Type.BIGINT;
		  case DB2Type.BIGINT: // BIGINT
		  {
			   long longValue = Conv.byteArrayToLong(data, offset);
			   if (longValue > int.MaxValue || longValue < int.MinValue)
			   {
				 throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			   }
			   else
			   {
				 return (int) longValue;
			   }
		  }


			  goto case DB2Type.INTEGER;
		  case DB2Type.INTEGER: // INTEGER
			return Conv.byteArrayToInt(data, offset);
		  case DB2Type.SMALLINT: // SMALLINT
			return Conv.byteArrayToShort(data, offset);
		  case DB2Type.VARBINARY: // VARBINARY
			int varblen = Conv.byteArrayToShort(data, offset);
			switch (varblen)
			{
			  case 0:
				  return 0;
			  case 1:
				  return data[offset + 2] & 0x00FF;
			  case 2:
				  return Conv.byteArrayToShort(data, offset + 2);
			  case 3:
				  return ((data[offset + 2] << 16) | Conv.byteArrayToShort(data, offset + 3)) & 0x00FFFFFF;
			  case 4:
				  return Conv.byteArrayToInt(data, offset + 2);
			  default:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  case DB2Type.BINARY: // BINARY
			int len = length_;
			switch (len)
			{
			  case 0:
				  return 0;
			  case 1:
				  return data[offset] & 0x00FF;
			  case 2:
				  return Conv.byteArrayToShort(data, offset);
			  case 3:
				  return ((data[offset] << 16) | Conv.byteArrayToShort(data, offset + 1)) & 0x00FFFFFF;
			  case 4:
				  return Conv.byteArrayToInt(data, offset + 2);
			  default:
				  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
		  default:
		  stringValue = convertToString(data, rowOffset);
		  double? doubleValue = Convert.ToDouble(stringValue.Trim());
		  double d = doubleValue.Value;
		  if (d > int.MaxValue || d < int.MinValue)
		  {
			SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			throw sqlex;
		  }
		  return doubleValue.Value;
		}
		  }
		  catch (System.FormatException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long convertToLong(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal long convertToLong(sbyte[] data, int rowOffset)
	  {
		  string stringValue = "";
		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DATE: // DATE
			/*
			 * This conversion is not supported by any other JDBC drivers.
			int year = (data[offset] & 0x0F)*1000 +
			           (data[offset+1] & 0x0F)*100 +
			           (data[offset+2] & 0x0F)*10 +
			           (data[offset+3] & 0x0F);
			int month = (data[offset+5] & 0x0F)*10 +
			            (data[offset+6] & 0x0F);
			int day = (data[offset+8] & 0x0F)*10 +
			            (data[offset+9] & 0x0F);
			calendar_.clear();
			calendar_.set(Calendar.YEAR, year);
			calendar_.set(Calendar.MONTH, month-1);
			calendar_.set(Calendar.DATE, day);
			return calendar_.getTimeInMillis();
			 */
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  case DB2Type.TIME: // TIME
			/*
			 * This conversion is not supported by any other JDBC drivers.
			int hours = (data[offset] & 0x0F)*10 +
			            (data[offset+1] & 0x0F);
			int minutes = (data[offset+3] & 0x0F)*10 +
			              (data[offset+4] & 0x0F);
			int seconds = (data[offset+6] & 0x0F)*10 +
			              (data[offset+7] & 0x0F);
			long millis = (hours*60*60*1000L) + (minutes*60*1000L) + (seconds*1000L);
			return millis;
			*/
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  case DB2Type.TIMESTAMP: // TIMESTAMP
			/*
			 * This conversion is not supported by any other JDBC drivers.
			year = (data[offset] & 0x0F)*1000 +
			       (data[offset+1] & 0x0F)*100 +
			       (data[offset+2] & 0x0F)*10 +
			       (data[offset+3] & 0x0F);
			month = (data[offset+5] & 0x0F)*10 +
			        (data[offset+6] & 0x0F);
			day = (data[offset+8] & 0x0F)*10 +
			        (data[offset+9] & 0x0F);
			calendar_.clear();
			calendar_.set(Calendar.YEAR, year);
			calendar_.set(Calendar.MONTH, month-1);
			calendar_.set(Calendar.DATE, day);
			hours = (data[offset+11] & 0x0F)*10 +
			        (data[offset+12] & 0x0F);
			minutes = (data[offset+14] & 0x0F)*10 +
			          (data[offset+15] & 0x0F);
			seconds = (data[offset+17] & 0x0F)*10 +
			          (data[offset+18] & 0x0F);
			calendar_.set(Calendar.HOUR_OF_DAY, hours);
			calendar_.set(Calendar.MINUTE, minutes);
			calendar_.set(Calendar.SECOND, seconds);
			int ms = (data[offset+20] & 0x0F)*100 +
			         (data[offset+21] & 0x0F)*10 +
			         (data[offset+22] & 0x0F);
			int half = (data[offset+23] & 0x0F);
			if (half >= 5) ++ms; // Round.
			calendar_.set(Calendar.MILLISECOND, ms);
			return calendar_.getTimeInMillis();
			*/
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);

		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			return (long)Conv.packedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			return (long)Conv.zonedDecimalToDouble(data, offset, precision_, scale_);
		  case DB2Type.BIGINT: // BIGINT
			return Conv.byteArrayToLong(data, offset);
		  case DB2Type.INTEGER: // INTEGER
			return Conv.byteArrayToInt(data, offset);
		  case DB2Type.SMALLINT: // SMALLINT
			return Conv.byteArrayToShort(data, offset);
		  case DB2Type.VARBINARY: // VARBINARY
			int varblen = Conv.byteArrayToShort(data, offset);
			if (varblen > 8)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			switch (varblen)
			{
			  case 0:
				  return 0;
			  case 1:
				  return data[offset + 2] & 0x00FF;
			  case 2:
				  return Conv.byteArrayToShort(data, offset + 2);
			  case 3:
				  return ((data[offset + 2] << 16) | Conv.byteArrayToShort(data, offset + 3)) & 0x00FFFFFF;
			  case 4:
				  return Conv.byteArrayToInt(data, offset + 2);
			  case 5:
				  return (((((long)data[offset + 2]) & 0xFFFFFFFF) << 32) | Conv.byteArrayToInt(data, offset + 3)) & 0x00FFFFFFFFFFL;
			  case 6:
				  return (((((long)data[offset + 2]) & 0xFFFFFFFF) << 40) + ((((long)data[offset + 3]) & 0xFFFFFFFF) << 32) + Conv.byteArrayToInt(data, offset + 4)) & 0x00FFFFFFFFFFFFL;
			  case 7:
				  return (((((long)data[offset + 2]) & 0xFFFFFFFF) << 48) + ((((long)data[offset + 3]) & 0xFFFFFFFF) << 40) + ((((long)data[offset + 4]) & 0xFFFFFFFF) << 32) + Conv.byteArrayToInt(data, offset + 5)) & 0x00FFFFFFFFFFFFFFL;
			  case 8:
				  return Conv.byteArrayToLong(data, offset + 2);
			}
			return 0;
		  case DB2Type.BINARY: // BINARY
			int len = length_;
			if (len > 8)
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			}
			switch (len)
			{
			  case 0:
				  return 0;
			  case 1:
				  return data[offset] & 0x00FF;
			  case 2:
				  return Conv.byteArrayToShort(data, offset);
			  case 3:
				  return ((data[offset] << 16) | Conv.byteArrayToShort(data, offset + 1)) & 0x00FFFFFF;
			  case 4:
				  return Conv.byteArrayToInt(data, offset + 2);
			  case 5:
				  return (((((long)data[offset]) & 0xFFFFFFFF) << 32) | Conv.byteArrayToInt(data, offset + 1)) & 0x00FFFFFFFFFFL;
			  case 6:
				  return (((((long)data[offset]) & 0xFFFFFFFF) << 40) + ((((long)data[offset + 1]) & 0xFFFFFFFF) << 32) + Conv.byteArrayToInt(data, offset + 2)) & 0x00FFFFFFFFFFFFL;
			  case 7:
				  return (((((long)data[offset]) & 0xFFFFFFFF) << 48) + ((((long)data[offset + 1]) & 0xFFFFFFFF) << 40) + ((((long)data[offset + 2]) & 0xFFFFFFFF) << 32) + Conv.byteArrayToInt(data, offset + 3)) & 0x00FFFFFFFFFFFFFFL;
			  case 8:
				  return Conv.byteArrayToLong(data, offset + 2);
			}
			return 0;
		  default:
		  stringValue = convertToString(data, rowOffset);
			return long.Parse(stringValue);
		}
		  }
		  catch (System.FormatException e)
		  {
			try
			{
			  double? doubleValue = Convert.ToDouble(stringValue.Trim());
			  double d = doubleValue.Value; //@trunc

			  if (d > long.MaxValue || d < long.MinValue)
			  {
				SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
				sqlex.initCause(e);
				throw sqlex;
			  }
			  return doubleValue.Value;
			}
			catch (System.FormatException e2)
			{
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
			  sqlex.initCause(e2);
			  throw sqlex;
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Date convertToDate(final byte[] data, final int rowOffset, java.util.Calendar cal) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal DateTime convertToDate(sbyte[] data, int rowOffset, DateTime cal)
	  {
		  string stringValue = null;
		  try
		  {
		if (buffer_ == null)
		{
		  int size = length_ + 2;
		  if (precision_ + 2 > size)
		  {
			  size = precision_ + 2;
		  }
		  buffer_ = new char[size];
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DATE: // DATE
		  case DB2Type.TIMESTAMP: // TIMESTAMP
			if (useDateCache_)
			{
			  DateTime value = lookupDate(data, offset, length_);
			  if (value != null)
			  {
				  return value;
			  }
			}
			// We told the server to send us dates in ISO, and EBCDIC makes it easy to mask off.
			int year = (data[offset] & 0x0F) * 1000 + (data[offset + 1] & 0x0F) * 100 + (data[offset + 2] & 0x0F) * 10 + (data[offset + 3] & 0x0F);
			int month = (data[offset + 5] & 0x0F) * 10 + (data[offset + 6] & 0x0F);
			int day = (data[offset + 8] & 0x0F) * 10 + (data[offset + 9] & 0x0F);
			cal.clear();
			cal.set(DateTime.YEAR, year);
			cal.set(DateTime.MONTH, month - 1);
			cal.set(DateTime.DATE, day);
			DateTime val = new DateTime(cal.Ticks); // This is way faster than doing java.sql.Date.valueOf().
			if (useDateCache_)
			{
			  cache(data, offset, length_, val);
			}
			return val;
		  // It does not make send to return a date from any one of these values.
		  case DB2Type.TIME: // TIME
		  case DB2Type.BIGINT: // BIGINT
		  case DB2Type.INTEGER: // INTEGER
		  case DB2Type.SMALLINT: // SMALLINT
		  case DB2Type.VARBINARY: // VARBINARY
		  case DB2Type.BINARY: // BINARY
			throw JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			 /*
			if (useDateCache_)
			{
			  Date value = lookupDate(data, offset, length_);
			  if (value != null) return value;
			}
			Date value = new Date(convertToLong(data, rowOffset));
			if (useDateCache_)
			{
			  cache(data, offset, length_, value);
			}
			return value;
		*/
		  default:
		  stringValue = convertToString(data, rowOffset).Trim();
			return DateTime.valueOf(stringValue);
		}
		  }
		  catch (System.ArgumentException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Time convertToTime(final byte[] data, final int rowOffset, java.util.Calendar cal) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal Time convertToTime(sbyte[] data, int rowOffset, DateTime cal)
	  {
		  string stringValue = null;
		  try
		  {
		if (buffer_ == null)
		{
		  int size = length_ + 2;
		  if (precision_ + 2 > size)
		  {
			  size = precision_ + 2;
		  }
		  buffer_ = new char[size];
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.TIME: // TIME
			switch (timeFormat_)
			{
			  case 0: // HMS
			  default:
				if (useTimeCache_)
				{
				  Time value = lookupTime(data, offset, length_);
				  if (value != null)
				  {
					  return value;
				  }
				}

				int hours = (data[offset] & 0x0F) * 10 + (data[offset + 1] & 0x0F);
				int minutes = (data[offset + 3] & 0x0F) * 10 + (data[offset + 4] & 0x0F);
				int seconds = (data[offset + 6] & 0x0F) * 10 + (data[offset + 7] & 0x0F);
				cal.clear();
				cal.set(DateTime.HOUR_OF_DAY, hours);
				cal.set(DateTime.MINUTE, minutes);
				cal.set(DateTime.SECOND, seconds);
				long millis = cal.Ticks;
				Time value = new Time(millis);
				if (useTimeCache_)
				{
				  cache(data, offset, length_, value);
				}
				return value;
			}
		  case DB2Type.TIMESTAMP: // TIMESTAMP
			if (useTimeCache_)
			{
			  Time value = lookupTime(data, offset, length_);
			  if (value != null)
			  {
				  return value;
			  }
			}
			int hours = (data[offset + 11] & 0x0F) * 10 + (data[offset + 12] & 0x0F);
			int minutes = (data[offset + 14] & 0x0F) * 10 + (data[offset + 15] & 0x0F);
			int seconds = (data[offset + 17] & 0x0F) * 10 + (data[offset + 18] & 0x0F);
			cal.clear();
			cal.set(DateTime.HOUR_OF_DAY, hours);
			cal.set(DateTime.MINUTE, minutes);
			cal.set(DateTime.SECOND, seconds);
			long millis = cal.Ticks;


			Time val = new Time(millis);
			if (useTimeCache_)
			{
			  cache(data, offset, length_, val);
			}
			return val;
		// It is not valid to create a time from these types
		  case DB2Type.DATE: // DATE
		  case DB2Type.BIGINT: // BIGINT
		  case DB2Type.INTEGER: // INTEGER
		  case DB2Type.SMALLINT: // SMALLINT
		  case DB2Type.VARBINARY: // VARBINARY
		  case DB2Type.BINARY: // BINARY
		  JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  return null;
		  default:
		  stringValue = convertToString(data, rowOffset).Trim();
			return Time.valueOf(stringValue);
		} // switch
		  }
		  catch (System.ArgumentException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Timestamp convertToTimestamp(final byte[] data, final int rowOffset, java.util.Calendar cal) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal Timestamp convertToTimestamp(sbyte[] data, int rowOffset, DateTime cal)
	  {
		   string stringValue = null;
		 try
		 {
		if (buffer_ == null)
		{
		  int size = length_ + 2;
		  if (precision_ + 2 > size)
		  {
			  size = precision_ + 2;
		  }
		  buffer_ = new char[size];
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		int year = 0;
		int month = 0;
		int day = 0;
		int hours = 0;
		int minutes = 0;
		int seconds = 0;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DATE: // DATE
			year = (data[offset] & 0x0F) * 1000 + (data[offset + 1] & 0x0F) * 100 + (data[offset + 2] & 0x0F) * 10 + (data[offset + 3] & 0x0F);
			month = (data[offset + 5] & 0x0F) * 10 + (data[offset + 6] & 0x0F);
			day = (data[offset + 8] & 0x0F) * 10 + (data[offset + 9] & 0x0F);
			cal.clear();
			cal.set(DateTime.YEAR, year);
			cal.set(DateTime.MONTH, month - 1);
			cal.set(DateTime.DATE, day);
			return new Timestamp(cal.Ticks);

		  case DB2Type.TIMESTAMP: // TIMESTAMP
			year = (data[offset] & 0x0F) * 1000 + (data[offset + 1] & 0x0F) * 100 + (data[offset + 2] & 0x0F) * 10 + (data[offset + 3] & 0x0F);
			month = (data[offset + 5] & 0x0F) * 10 + (data[offset + 6] & 0x0F);
			day = (data[offset + 8] & 0x0F) * 10 + (data[offset + 9] & 0x0F);
			cal.clear();
			cal.set(DateTime.YEAR, year);
			cal.set(DateTime.MONTH, month - 1);
			cal.set(DateTime.DATE, day);
			hours = (data[offset + 11] & 0x0F) * 10 + (data[offset + 12] & 0x0F);
			minutes = (data[offset + 14] & 0x0F) * 10 + (data[offset + 15] & 0x0F);
			seconds = (data[offset + 17] & 0x0F) * 10 + (data[offset + 18] & 0x0F);
			cal.set(DateTime.HOUR_OF_DAY, hours);
			cal.set(DateTime.MINUTE, minutes);
			cal.set(DateTime.SECOND, seconds);
			Timestamp stamp = new Timestamp(cal.Ticks);
			int micros = (data[offset + 20] & 0x0F) * 100000 + (data[offset + 21] & 0x0F) * 10000 + (data[offset + 22] & 0x0F) * 1000 + (data[offset + 23] & 0x0F) * 100 + (data[offset + 24] & 0x0F) * 10 + (data[offset + 25] & 0x0F);
			int nanos = micros * 1000;
			stamp.Nanos = nanos;
			return stamp;

		// It is not valid to get a timestamp from any of these types
		  case DB2Type.TIME: // TIME
		  case DB2Type.BIGINT: // BIGINT
		  case DB2Type.INTEGER: // INTEGER
		  case DB2Type.SMALLINT: // SMALLINT
		  case DB2Type.VARBINARY: // VARBINARY
		  case DB2Type.BINARY: // BINARY
		   JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  return null;
		  default:
		  stringValue = convertToString(data, rowOffset).Trim();
			return Timestamp.valueOf(stringValue);
		}
		 }
		  catch (System.ArgumentException e)
		  {
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH, stringValue);
		  sqlex.initCause(e);
		  throw sqlex;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte[] convertToOutputBytes(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal sbyte[] convertToOutputBytes(sbyte[] data, int rowOffset)
	  {
		int offset = rowOffset + offset_;
		int length = length_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DATE: // DATE
			break;
		  case DB2Type.TIME: // TIME
			break;
		  case DB2Type.TIMESTAMP: // TIMESTAMP
			length = 26;
			break;
		  case DB2Type.CLOB: // LOB
			length = Conv.byteArrayToInt(data, offset);
			offset += 4;
			break;
		  case DB2Type.VARCHAR: // VARCHAR
		  case DB2Type.LONGVARCHAR: // LONG VARCHAR
		  case DB2Type.DATALINK:
			length = Conv.byteArrayToShort(data, offset);
			offset += 2;
			break;
		  case DB2Type.CHAR: // CHAR
			break;
		  case DB2Type.VARGRAPHIC: // VARGRAPHIC
		  case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
			length = Conv.byteArrayToShort(data, offset) * 2;
			offset += 2;
			break;
		  case DB2Type.GRAPHIC: // GRAPHIC
			/* length = length * 2 */
			; // length already in bytes
			break;
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
		  case DB2Type.BIGINT: // BIGINT
		  case DB2Type.INTEGER: // INTEGER
		  case DB2Type.SMALLINT: // SMALLINT
		  case DB2Type.FLOATINGPOINT: // floating point:
			break;
		  case DB2Type.VARBINARY: // VARBINARY
			length = Conv.byteArrayToShort(data, offset);
		  offset += 2;
			break;

		  case DB2Type.BINARY: // BINARY
			break;
		  case DB2Type.CLOB_LOCATOR: //TODO - LOB locator
		  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locate
	//        System.out.println("CCSID! "+ccsid_);
	//        System.out.println(length_+", "+offset+", "+buffer_.length+", "+scale_+", "+precision_);
			int locatorHandle = Conv.byteArrayToInt(data, offset);
	//        System.out.println("Handle: "+Integer.toHexString(locatorHandle));
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unhandled database type: " + type_);
		  default:
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_);
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] b = new byte[length];
		sbyte[] b = new sbyte[length];
		Array.Copy(data, offset, b, 0, length);
		return b;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String convertToString(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal string convertToString(sbyte[] data, int rowOffset)
	  {
	//    System.out.println("Convert to string "+name_+", "+ccsid_);
		if (buffer_ == null)
		{
		  int size = length_ + 2;
		  if (precision_ + 2 > size)
		  {
			  size = precision_ + 2;
		  }
		  buffer_ = new char[size];
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		try
		{
		  switch (type_ & 0xFFFE)
		  {
			case DB2Type.DATE: // DATE
			  switch (dateFormat_)
			  {
				case 1: // MDY
				default:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset, length_);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.ebcdicByteArrayToString(data, offset, length_, buffer_);
					cache(data, offset, length_, value);
					return value;
				  }
				  return Conv.ebcdicByteArrayToString(data, offset, length_, buffer_);
			  }
			case DB2Type.TIME: // TIME
			  switch (timeFormat_)
			  {
				case 0: // HMS
				default:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset, length_);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.ebcdicByteArrayToString(data, offset, length_, buffer_);
					cache(data, offset, length_, value);
					return value;
				  }
				  return Conv.ebcdicByteArrayToString(data, offset, length_, buffer_);
			  }
			case DB2Type.TIMESTAMP: // TIMESTAMP
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset, length_);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
			  }
			  buffer_[0] = NUMS[data[offset] & 0x0F];
			  buffer_[1] = NUMS[data[offset + 1] & 0x0F];
			  buffer_[2] = NUMS[data[offset + 2] & 0x0F];
			  buffer_[3] = NUMS[data[offset + 3] & 0x0F];
			  buffer_[4] = '-';
			  buffer_[5] = NUMS[data[offset + 5] & 0x0F];
			  buffer_[6] = NUMS[data[offset + 6] & 0x0F];
			  buffer_[7] = '-';
			  buffer_[8] = NUMS[data[offset + 8] & 0x0F];
			  buffer_[9] = NUMS[data[offset + 9] & 0x0F];
			  buffer_[10] = ' '; // Was using 'T' but now try to match java.sql.Timestamp.valueOf() format.
			  buffer_[11] = NUMS[data[offset + 11] & 0x0F];
			  buffer_[12] = NUMS[data[offset + 12] & 0x0F];
			  buffer_[13] = ':';
			  buffer_[14] = NUMS[data[offset + 14] & 0x0F];
			  buffer_[15] = NUMS[data[offset + 15] & 0x0F];
			  buffer_[16] = ':';
			  buffer_[17] = NUMS[data[offset + 17] & 0x0F];
			  buffer_[18] = NUMS[data[offset + 18] & 0x0F];
			  buffer_[19] = '.';
			  buffer_[20] = NUMS[data[offset + 20] & 0x0F];
			  buffer_[21] = NUMS[data[offset + 21] & 0x0F];
			  buffer_[22] = NUMS[data[offset + 22] & 0x0F];
			  buffer_[23] = NUMS[data[offset + 23] & 0x0F];
			  buffer_[24] = NUMS[data[offset + 24] & 0x0F];
			  buffer_[25] = NUMS[data[offset + 25] & 0x0F];
			  string val = new string(buffer_, 0, 26);
			  if (useStringCache_)
			  {
				cache(data, offset, length_, val);
			  }
			  return val;
			case DB2Type.CLOB: // LOB
			  int totalLength = Conv.byteArrayToInt(data, offset);
			  switch (ccsid_)
			  {
				case 13488:
				case 1200:
				  return Conv.unicodeByteArrayToString(data, offset + 4, totalLength, buffer_);
				default:
				  return Conv.ebcdicByteArrayToString(data, offset + 4, totalLength, buffer_, ccsid_);
			  }
			case DB2Type.VARCHAR: // VARCHAR
			case DB2Type.LONGVARCHAR: // LONG VARCHAR
			case DB2Type.DATALINK:
			  int varlen = Conv.byteArrayToShort(data, offset);
			  switch (ccsid_)
			  {
				case 13488:
				case 1200:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset + 2, varlen);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.unicodeByteArrayToString(data, offset + 2, varlen, buffer_);
					cache(data, offset + 2, varlen, value);
					return value;
				  }
				  return Conv.unicodeByteArrayToString(data, offset + 2, varlen, buffer_);
				default:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset + 2, varlen);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.ebcdicByteArrayToString(data, offset + 2, varlen, buffer_, ccsid_);
					cache(data, offset + 2, varlen, value);
					return value;
				  }
				  return Conv.ebcdicByteArrayToString(data, offset + 2, varlen, buffer_, ccsid_);
			  }
			case DB2Type.CHAR: // CHAR
			  switch (ccsid_)
			  {
				case 13488:
				case 1200:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset, length_);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.unicodeByteArrayToString(data, offset, length_, buffer_);
					cache(data, offset, length_, value);
					return value;
				  }
				  return Conv.unicodeByteArrayToString(data, offset, length_, buffer_);
				default:
				  if (useStringCache_)
				  {
					string value = lookupString(data, offset, length_);
					if (!string.ReferenceEquals(value, null))
					{
						return value;
					}
					value = Conv.ebcdicByteArrayToString(data, offset, length_, buffer_);
					cache(data, offset, length_, value);
					return value;
				  }
				  return Conv.ebcdicByteArrayToString(data, offset, length_, buffer_, ccsid_);
			  }
			case DB2Type.VARGRAPHIC: // VARGRAPHIC
			case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
				if (ccsid_ == 13488 || ccsid_ == 1200)
				{
			  int varglen = Conv.byteArrayToShort(data, offset);
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset + 2, varglen * 2);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.unicodeByteArrayToString(data, offset + 2, varglen * 2, buffer_);
				cache(data, offset + 2, varglen * 2, value);
				return value;
			  }
			  return Conv.unicodeByteArrayToString(data, offset + 2, varglen * 2, buffer_);
				}
				else
				{
					JDBCError.throwSQLException(JDBCError.EXC_CHAR_CONVERSION_INVALID);
				}
				goto case DB2Type.GRAPHIC;
			case DB2Type.GRAPHIC: // GRAPHIC
				if (ccsid_ == 13488 || ccsid_ == 1200)
				{
					return Conv.unicodeByteArrayToString(data, offset, length_, buffer_);
				}
				else
				{
					JDBCError.throwSQLException(JDBCError.EXC_CHAR_CONVERSION_INVALID);
				}
			goto case DB2Type.FLOATINGPOINT;
		case DB2Type.FLOATINGPOINT: // floating point:
			if (length_ == 4)
			{
			return "" + Conv.byteArrayToFloat(data, offset);
			}
			else if (length_ == 8)
			{
			return "" + Conv.byteArrayToDouble(data, offset);
			}
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_ + " length= " + length_);
			case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			  return Conv.packedDecimalToString(data, offset, precision_, scale_, buffer_);
			case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			  return Conv.zonedDecimalToString(data, offset, precision_, scale_, buffer_);
			case DB2Type.BIGINT: // BIGINT
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset, 8);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.byteArrayToLong(data, offset).ToString();
				cache(data, offset, 8, value);
				return value;
			  }
			  return Conv.byteArrayToLong(data, offset).ToString();
			case DB2Type.INTEGER: // INTEGER
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset, 4);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.byteArrayToInt(data, offset).ToString();
				cache(data, offset, 4, value);
				return value;
			  }
			  return Conv.byteArrayToInt(data, offset).ToString();
			case DB2Type.SMALLINT: // SMALLINT
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset, 2);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.byteArrayToShort(data, offset).ToString();
				cache(data, offset, 2, value);
				return value;
			  }
			  return Conv.byteArrayToShort(data, offset).ToString();
			case DB2Type.VARBINARY: // VARBINARY
			  int varblen = Conv.byteArrayToShort(data, offset);
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset + 2, varblen);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.bytesToHexString(data, offset + 2, varblen);
				cache(data, offset + 2, varblen, value);
				return value;
			  }
			  return Conv.bytesToHexString(data, offset + 2, varblen);
			case DB2Type.BINARY: // BINARY
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset, length_);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.bytesToHexString(data, offset, length_);
				cache(data, offset, length_, value);
				return value;
			  }
			  return Conv.bytesToHexString(data, offset, length_);
			case DB2Type.DBCLOB:
				if (ccsid_ == 13488 || ccsid_ == 1200)
				{
			  int varglen = Conv.byteArrayToInt(data, offset);
			  if (useStringCache_)
			  {
				string value = lookupString(data, offset + 4, varglen * 2);
				if (!string.ReferenceEquals(value, null))
				{
					return value;
				}
				value = Conv.unicodeByteArrayToString(data, offset + 4, varglen * 2, buffer_);
				cache(data, offset + 2, varglen * 2, value);
				return value;
			  }
			  return Conv.unicodeByteArrayToString(data, offset + 4, varglen * 2, buffer_);
				}
				else
				{
					JDBCError.throwSQLException(JDBCError.EXC_CHAR_CONVERSION_INVALID);
					return null;
				}

			case DB2Type.BLOB_LOCATOR: //TODO BLOB locator
			case DB2Type.CLOB_LOCATOR: //TODO - CLOB locator
			case DB2Type.DBCLOB_LOCATOR: // - DBCLOB locator
			case DB2Type.XML_LOCATOR:
	//          System.out.println("CCSID! "+ccsid_);
	//          System.out.println(length_+", "+offset+", "+buffer_.length+", "+scale_+", "+precision_);
			  int locatorHandle = Conv.byteArrayToInt(data, offset);
	//          System.out.println("Handle: 0x"+Integer.toHexString(locatorHandle));
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unsupported database type: " + type_ + " length= " + length_);
			case DB2Type.DECFLOAT:
			  if (length_ == 8)
			  {
				return Conv.decfloat16ByteArrayToString(data, offset);
			  }
			  else if (length_ == 16)
			  {
				return Conv.decfloat34ByteArrayToString(data, offset);
			  }
			  else
			  {
				throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown DECFLOAT length= " + length_);
			  }
			default:
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_ + " length= " + length_);
		  }
		}
		catch (UnsupportedEncodingException uee)
		{
		  SQLException sql = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH,"Data conversion error");
		  sql.initCause(uee);
		  throw sql;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean convertToBoolean(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal bool convertToBoolean(sbyte[] data, int rowOffset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = rowOffset+offset_;
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.VARCHAR: // VARCHAR
		  case DB2Type.LONGVARCHAR: // LONG VARCHAR
		  case DB2Type.CHAR: // CHAR
		  case DB2Type.VARGRAPHIC: // VARGRAPHIC
		  case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
		  case DB2Type.GRAPHIC: // GRAPHIC
			string s = convertToString(data, rowOffset);
			s = s.Trim();
			return !s.Equals("0") && !s.Equals("false", StringComparison.OrdinalIgnoreCase) && !s.Equals("n", StringComparison.OrdinalIgnoreCase); //TODO
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			return Conv.packedDecimalToDouble(data, offset, precision_, scale_) != 0.0d;
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			return Conv.zonedDecimalToDouble(data, offset, precision_, scale_) != 0.0d;
		  case DB2Type.BIGINT: // BIGINT
			return convertToLong(data, rowOffset) != 0L;
		  case DB2Type.INTEGER: // INTEGER
			return convertToInt(data, rowOffset) != 0;
		  case DB2Type.SMALLINT: // SMALLINT
			return convertToShort(data, rowOffset) != 0;
		  case DB2Type.FLOATINGPOINT:
			return convertToDouble(data, rowOffset) != 0;
		  case DB2Type.VARBINARY: // VARBINARY
			int varblen = Conv.byteArrayToShort(data, offset);
			for (int i = offset + 2; i < offset + 2 + varblen; ++i)
			{
			  if (data[i] != 0)
			  {
				  return true;
			  }
			}
			return false;
		  case DB2Type.BINARY: // BINARY
			for (int i = offset; i < offset + length_; ++i)
			{
			  if (data[i] != 0)
			  {
				  return true;
			  }
			}
			return false;
		  case DB2Type.DECFLOAT:
		  {
			  string stringValue;
			if (length_ == 8)
			{
			  stringValue = Conv.decfloat16ByteArrayToString(data, offset);
			}
			else if (length_ == 16)
			{
			  stringValue = Conv.decfloat34ByteArrayToString(data, offset);
			}
			else
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown DECFLOAT length= " + length_);
			}
			double? d = double.Parse(stringValue);
			if (d.Value != 0.0)
			{
				return true;
			}
			return false;
		  }
		  case DB2Type.DATE: // DATE
		  case DB2Type.TIME: // TIME
		  case DB2Type.TIMESTAMP: // TIMESTAMP
		  case DB2Type.CLOB: // LOB
		  case DB2Type.BLOB_LOCATOR: //TODO BLOB locator
		  case DB2Type.CLOB_LOCATOR: //TODO - CLOB locator
		  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
		  case DB2Type.XML_LOCATOR:
			JDBCError.throwSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			return false;
		  default:
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_);
		}
	  }

	  // See http://download.oracle.com/javase/1.4.2/docs/guide/jdbc/getstart/mapping.html#1004791
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object convertToObject(final byte[] data, final int rowOffset) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal object convertToObject(sbyte[] data, int rowOffset)
	  {
		try
		{
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.DATE: // DATE
			return convertToDate(data, rowOffset, calendar_);
		  case DB2Type.TIME: // TIME
			return convertToTime(data, rowOffset, calendar_);
		  case DB2Type.TIMESTAMP: // TIMESTAMP
			return convertToTimestamp(data, rowOffset, calendar_);
	//TODO      case SQLType.CLOB: // LOB
		  case DB2Type.VARCHAR: // VARCHAR
		  case DB2Type.LONGVARCHAR: // LONG VARCHAR
		  case DB2Type.CHAR: // CHAR
		  case DB2Type.VARGRAPHIC: // VARGRAPHIC
		  case DB2Type.LONGVARGRAPHIC: // LONG VARGRAPHIC
		  case DB2Type.GRAPHIC: // GRAPHIC
			return convertToString(data, rowOffset);
		  case DB2Type.DECIMAL: // DECIMAL (packed decimal)
			return new decimal(convertToString(data, rowOffset));
		  case DB2Type.NUMERIC: // NUMERIC (zoned decimal)
			return new decimal(convertToString(data, rowOffset));
		  case DB2Type.BIGINT: // BIGINT
			return new long?(convertToLong(data, rowOffset));
		  case DB2Type.INTEGER: // INTEGER
			return new int?(convertToInt(data, rowOffset));
		  case DB2Type.SMALLINT: // SMALLINT
			return new int?(convertToShort(data, rowOffset));
		  case DB2Type.VARBINARY: // VARBINARY
		  {
			int offset = rowOffset + offset_;
			int varblen = Conv.byteArrayToShort(data, offset);
			sbyte[] vb = new sbyte[varblen];
			Array.Copy(data, offset + 2, vb, 0, varblen);
			return vb;
		  }
		  case DB2Type.BINARY: // BINARY
			sbyte[] b = new sbyte[length_];
			Array.Copy(data, rowOffset + offset_, b, 0, length_);
			return b;
		  case DB2Type.FLOATINGPOINT:
		  {
			int offset = rowOffset + offset_;
			if (length_ == 4)
			{
			  float floatValue = Conv.byteArrayToFloat(data, offset);
			  return new float?(floatValue);
			}
			else if (length_ == 8)
			{
			  double doubleValue = Conv.byteArrayToDouble(data, offset);
			  return new double?(doubleValue);
			}
		  }
			break;
		  case DB2Type.BLOB_LOCATOR: //TODO BLOB locator
		  case DB2Type.CLOB_LOCATOR: //TODO - CLOB locator
		  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
		  case DB2Type.XML_LOCATOR:
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Type " + type_ + " not fully supported");
		  case DB2Type.DECFLOAT:

			if (length_ == 8)
			{
			  return new decimal(Conv.decfloat16ByteArrayToString(data, rowOffset));
			}
			else if (length_ == 16)
			{
			  return new decimal(Conv.decfloat34ByteArrayToString(data, rowOffset));
			}
			else
			{
			  throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown DECFLOAT length= " + length_);
			}
		  case DB2Type.DATALINK:
		  {
			string dlValue = convertToString(data, rowOffset);
			try
			{
			  return new URL(dlValue);
			}
			catch (MalformedURLException e)
			{
			  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
			  sqlex.initCause(e);
			  throw sqlex;
			}

		  }
			  goto default;
		  default:
			throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: " + type_);
		}
		}
		catch (System.FormatException nfe)
		{
		  SQLException sqlex = JDBCError.getSQLException(JDBCError.EXC_DATA_TYPE_MISMATCH);
		  sqlex.initCause(nfe);
		  throw sqlex;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Blob convertToBlob(final byte[] data, final int rowOffset, JDBCConnection conn) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal Blob convertToBlob(sbyte[] data, int rowOffset, JDBCConnection conn)
	  {
		int offset = rowOffset + offset_;
		switch (type_ & 0xFFFE)
		{
		  case DB2Type.CLOB: // LOB
			int len = Conv.byteArrayToInt(data, offset);
			return new JDBCBlob(data, offset + 4, len);
		  case DB2Type.BLOB_LOCATOR: //TODO BLOB locator
		  case DB2Type.CLOB_LOCATOR: //TODO - CLOB locator
		  case DB2Type.DBCLOB_LOCATOR: // DBCLOB locator
		  case DB2Type.XML_LOCATOR:

			int locatorHandle = Conv.byteArrayToInt(data, offset);
			DatabaseRetrieveLOBDataAttributes a = new DatabaseRequestAttributes();
			a.LOBLocatorHandle = locatorHandle;
			JDBCBlobLocator locator = new JDBCBlobLocator(conn.DatabaseConnection, a);
			return locator;
		  default:
			return new JDBCBlob(data, offset, length_);
	//      default:
	//        throw JDBCError.getSQLException(JDBCError.EXC_INTERNAL,"Unknown database type: "+type_);
		}
	  }

	  //
	  // Make sure that the decimal number has the correct scale.
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String formatDecimal(String input, int precision, int scale) throws SQLException
	  internal string formatDecimal(string input, int precision, int scale)
	  {
		  if (scale == 0)
		  {
			  int dotIndex = input.IndexOf('.');
			  if (dotIndex > 0)
			  {
				  input = input.Substring(0,dotIndex);
			  }
			  int inputLength = input.Length;
			  if (input[0] == '-')
			  {
				  inputLength--;
			  }
			  if (inputLength > precision)
			  {
					throw new DataTruncation(index_, parameter_, false, input.Length, precision); // transferSize
			  }
		  }
		  else
		  {
			  int dotIndex = input.IndexOf('.');
			  if (dotIndex > 0)
			  {
				  int scaleDigits = input.Length - dotIndex - 1;
				  if (scaleDigits < scale)
				  {
					  StringBuilder sb = new StringBuilder(input);
					  while (scaleDigits < scale)
					  {
						  sb.Append('0');
						  scaleDigits++;
					  }
					  input = sb.ToString();
				  }
				  else if (scaleDigits > scale)
				  {
					  // Truncation warning should go here
					  input = input.Substring(0, dotIndex + 1 + scale);
				  }
			  }
			  else
			  {
				 // add the scale
				 StringBuilder sb = new StringBuilder(input);
				 sb.Append('.');
				 for (int i = 0; i < scale; i++)
				 {
					 sb.Append('0');
				 }
				 input = sb.ToString();
			  }
			  // check for truncation
			  int inputLength = input.Length;
			  if (input[0] == '-')
			  {
				  inputLength--;
			  }
			  if (inputLength > precision + 1)
			  {
					throw new DataTruncation(index_, parameter_, false, input.Length - 1, precision); // transferSize
			  }
		  }
		  return input;
	  }

	public int Nullable
	{
		get
		{
			if ((type_ & 0x1) != 0)
			{
				return ResultSetMetaData.columnNullable;
			}
			else
			{
				return ResultSetMetaData.columnNoNulls;
			}
		}
	}

	/*
	 * Returns an instance of a Gregorian calendar to be used to set
	 * Date values.   This is needed because the server uses the Gregorian calendar.
	 * For some locales, the calendar returned by Calendar.getInstance is not usable.
	 * For example, in the THAI local, a java.util.BuddhistCalendar is returned.
	 */
	public static DateTime GregorianInstance
	{
		get
		{
		  DateTime returnCalendar = new DateTime();
		  bool isGregorian = (returnCalendar is GregorianCalendar);
		  bool isBuddhist = false;
		  try
		  {
			  isBuddhist = isBuddhistCalendar(returnCalendar);
		  }
		  catch (Exception)
		  {
			// Just ignore if any exception occurs.
			// Possible exceptions (from Javadoc) are:
			// java.lang.NoClassDefFoundError
			// java.security.AccessControlException (if sun.util classes cannot be used)
		  }
    
		  if (isGregorian && (!isBuddhist))
		  {
			 // Calendar is gregorian, but not buddhist
			 return returnCalendar;
		  }
		  else
		  {
			// Create a new gregorianCalendar for the current timezone and locale
			DateTime gregorianCalendar = new GregorianCalendar();
			return gregorianCalendar;
		  }
		}
	}

	private static bool isBuddhistCalendar(DateTime calendar)
	{
	  try
	  {
		  Type c = Type.GetType("sun.util.BuddhistCalendar");
		  return c.IsInstanceOfType(calendar);

	  }
	catch (Exception)
	{
	  // Just ignore if any exception occurs.  @F2C
	  // Possible exceptions (from Javadoc) are:
	  // java.lang.NoClassDefFoundError
	  // java.security.AccessControlException (if sun.util classes cannot be used)
	}
	  return false;
	}



	/*  Get an instance of a calendar from the GMT timezone  */
	internal static TimeZone gmtTimeZone = null;
	public static DateTime GMTInstance
	{
		get
		{
		  if (gmtTimeZone == null)
		  {
			gmtTimeZone = TimeZone.getTimeZone("GMT");
		  }
		  return DateTime.getInstance(gmtTimeZone);
		}
	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Clob convertToClob(final byte[] data, final int rowOffset, JDBCConnection conn) throws SQLException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal Clob convertToClob(sbyte[] data, int rowOffset, JDBCConnection conn)
	  {
		int offset = rowOffset + offset_;
		switch (type_)
		{
		  case 404: // BLOB
		  case 405:
		  case 408: // LOB
		  case 409:
			int len = Conv.byteArrayToInt(data, offset);
			return new JDBCClob(data, offset + 4, len, ccsid_);
		  case 960: //TODO BLOB locator
		  case 961:
		  case 964: //TODO - CLOB locator
		  case 965:
			int locatorHandle = Conv.byteArrayToInt(data, offset);
			DatabaseRetrieveLOBDataAttributes a = new DatabaseRequestAttributes();
			a.LOBLocatorHandle = locatorHandle;
	//TODO        JDBCClobLocator locator = new JDBCClobLocator(conn.getDatabaseConnection(), a);
	//        return locator;
			return null;
		  default:
			return new JDBCClob(data, offset, length_, ccsid_);
	//      default:
	//        throw new SQLException("Unknown database type: "+type_);
		}
	  }

	}






}