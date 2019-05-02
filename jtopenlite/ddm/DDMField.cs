using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMField.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{
	using com.ibm.jtopenlite;

	/// <summary>
	/// Represents an individual field of a record format.
	/// 
	/// </summary>
	public sealed class DDMField
	{
	//  public static final char TYPE_LOB = '1';
	//  public static final char TYPE_DBCLOB = '3';
	//  public static final char TYPE_DATALINK = '4';

	  public const char TYPE_CHARACTER = 'A';
	  public const char TYPE_DBCS_EITHER = 'E';
	  public const char TYPE_DBCS_GRAPHIC = 'G';
	  public const char TYPE_DBCS_ONLY = 'J';
	  public const char TYPE_DBCS_OPEN = 'O';
	  public const char TYPE_BINARY = 'B';
	  public const char TYPE_FLOAT = 'F';
	  public const char TYPE_DECIMAL_FLOAT = '6';
	  public const char TYPE_HEXADECIMAL = 'H';
	  public const char TYPE_BINARY_CHARACTER = '5';
	  public const char TYPE_DATE = 'L';
	  public const char TYPE_PACKED_DECIMAL = 'P';
	  public const char TYPE_ZONED_DECIMAL = 'S';
	  public const char TYPE_TIME = 'T';
	  public const char TYPE_TIMESTAMP = 'Z';

	  private readonly int offset_; // The offset into the record.
	  private readonly string name_;
	  private readonly int length_;
	  private readonly int numDigits_;
	  private readonly int decimalPositions_;
	  private readonly string text_;
	  private readonly char type_;
	  private readonly string defaultValue_;
	  private readonly int ccsid_;
	  private readonly string variableLengthField_;
	  private readonly int allocatedLength_;
	  private readonly string allowNulls_;
	  private readonly string dateTimeFormat_;
	  private readonly string dateTimeSeparator_;
	  private readonly char[] buffer_;

	  private Dictionary<ByteArrayKey, string> cache_;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMField(final int offset, final String name, final int length, final int digits, final int decpos, final String text, final char type, final String defaultValue, final int ccsid, final String varlen, final int alloc, final String allowNull, final String dateFormat, final String dateSeparator)
	  internal DDMField(int offset, string name, int length, int digits, int decpos, string text, char type, string defaultValue, int ccsid, string varlen, int alloc, string allowNull, string dateFormat, string dateSeparator)
	  {
		offset_ = offset;
		name_ = name;
		length_ = length;
		numDigits_ = digits;
		decimalPositions_ = decpos;
		text_ = text;
		type_ = type;
		defaultValue_ = defaultValue;
		ccsid_ = ccsid;
		variableLengthField_ = varlen;
		allocatedLength_ = alloc;
		allowNulls_ = allowNull;
		dateTimeFormat_ = dateFormat;
		dateTimeSeparator_ = dateSeparator;
		buffer_ = new char[length_ * 2 + 2]; //TODO - Is this cool?
	  }

	  /// <summary>
	  /// Returns a new copy of this field, which is useful if multiple threads
	  /// need to operate on the same field without contention, as this class
	  /// is not threadsafe.
	  /// 
	  /// </summary>
	  public DDMField newCopy()
	  {
		DDMField f = new DDMField(offset_, name_, length_, numDigits_, decimalPositions_, text_, type_, defaultValue_, ccsid_, variableLengthField_, allocatedLength_, allowNulls_, dateTimeFormat_, dateTimeSeparator_);
		f.CacheStrings = CacheStrings;
		return f;
	  }

	  /// <summary>
	  /// Returns the name (WHFLDE) of this field.
	  /// 
	  /// </summary>
	  public string Name
	  {
		  get
		  {
			return name_;
		  }
	  }

	  /// <summary>
	  /// Returns the type (WHFLDT) of this field.
	  /// 
	  /// </summary>
	  public char Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  /// <summary>
	  /// Returns the length (WHFLDB) of this field.
	  /// 
	  /// </summary>
	  public int Length
	  {
		  get
		  {
			return length_;
		  }
	  }

	  /// <summary>
	  /// Returns the offset in the record data where this field begins.
	  /// 
	  /// </summary>
	  public int Offset
	  {
		  get
		  {
			return offset_;
		  }
	  }

	  /// <summary>
	  /// Returns the text description (WHFTXT) of this field.
	  /// 
	  /// </summary>
	  public string Text
	  {
		  get
		  {
			return text_;
		  }
	  }

	  /// <summary>
	  /// Returns the CCSID (WHCCSID) of this field.
	  /// 
	  /// </summary>
	  public int CCSID
	  {
		  get
		  {
			return ccsid_;
		  }
	  }

	  /// <summary>
	  /// Returns the default value (WHDFT) of this field.
	  /// 
	  /// </summary>
	  public string DefaultValue
	  {
		  get
		  {
			return defaultValue_;
		  }
	  }

	  /// <summary>
	  /// Indicates if this field is variable length (WHVARL).
	  /// 
	  /// </summary>
	  public bool VariableLength
	  {
		  get
		  {
			return variableLengthField_.Equals("Y");
		  }
	  }

	  /// <summary>
	  /// Indicates if this field allows null values (WHNULL).
	  /// 
	  /// </summary>
	  public bool NullAllowed
	  {
		  get
		  {
			return allowNulls_.Equals("Y");
		  }
	  }

	  /// <summary>
	  /// Returns the database allocated length (WHALLC) of this field.
	  /// 
	  /// </summary>
	  public int AllocatedLength
	  {
		  get
		  {
			return allocatedLength_;
		  }
	  }

	  /// <summary>
	  /// Returns the total number of digits (WHFLDO) of this field.
	  /// 
	  /// </summary>
	  public int NumberOfDigits
	  {
		  get
		  {
			return numDigits_;
		  }
	  }

	  /// <summary>
	  /// Returns the number of decimal positions (WHFLDP) of this field.
	  /// 
	  /// </summary>
	  public int DecimalPositions
	  {
		  get
		  {
			return decimalPositions_;
		  }
	  }

	  /// <summary>
	  /// Returns the date/time format (WHFMT) of this field.
	  /// 
	  /// </summary>
	  public string DateTimeFormat
	  {
		  get
		  {
			return dateTimeFormat_;
		  }
	  }

	  /// <summary>
	  /// Returns the date/time separator (WHSEP) of this field.
	  /// 
	  /// </summary>
	  public string DateTimeSeparator
	  {
		  get
		  {
			return dateTimeSeparator_;
		  }
	  }

	  /// <summary>
	  /// Indicates if <seealso cref="#getString getString()"/> will cache previously created String values
	  /// for memory conservation. This can be extremely helpful for fields that contain a finite
	  /// number of distinct values across all the records in the file. </summary>
	  /// <seealso cref= #setCacheStrings
	  ///  </seealso>
	  public bool CacheStrings
	  {
		  get
		  {
			return cache_ != null;
		  }
		  set
		  {
			cache_ = value ? new Dictionary<ByteArrayKey, string>() : null;
		  }
	  }


	  /// <summary>
	  /// Converts the specified record data at this field's offset into a long value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public long getLong(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return (long)Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return (long)Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).longValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).longValue();
			}
		  default:
			return long.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into a short value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short getShort(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public short getShort(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return (short)Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return (short)Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return (short)Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return (short)Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return (short)Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (short)(new decimal(value)).intValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (short)(new decimal(value)).intValue();
			}
		  default:
			return short.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into an int value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public int getInt(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return (int)Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return (int)Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return (int)Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).intValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).intValue();
			}
		  default:
			return int.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into a byte value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte getByte(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public sbyte getByte(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return (sbyte)Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return (sbyte)Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return (sbyte)Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return (sbyte)Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return (sbyte)Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (sbyte)(new decimal(value)).intValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (sbyte)(new decimal(value)).intValue();
			}
		  default:
			return sbyte.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Returns an array of bytes that is only this field's data, which is a subset of the specified record data.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public sbyte[] getBytes(sbyte[] recordData)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean varlen = isVariableLength();
		bool varlen = VariableLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = varlen ? length_-2 : length_;
		int length = varlen ? length_ - 2 : length_;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = varlen ? offset_+2 : offset_;
		int offset = varlen ? offset_ + 2 : offset_;
		sbyte[] b = new sbyte[length];
		Array.Copy(recordData, offset, b, 0, length);
		return b;
	  }

	  /// <summary>
	  /// Converts the specified byte array value into record data at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBytes(final byte[] value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setBytes(sbyte[] value, sbyte[] recordData)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean varlen = isVariableLength();
		bool varlen = VariableLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = varlen ? length_-2 : length_;
		int length = varlen ? length_ - 2 : length_;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = varlen ? offset_+2 : offset_;
		int offset = varlen ? offset_ + 2 : offset_;

		int len = value.Length;
		int limit = len < length ? len : length;
		Array.Copy(value, 0, recordData, offset, limit);
		if (varlen)
		{
		  Conv.shortToByteArray(limit, recordData, offset_);
		}
		else if (len < length)
		{
		  int left = length - len;
		  int count = offset + limit;
		  for (int i = 0; i < left; ++i)
		  {
			recordData[count++] = 0;
		  }
		}
	  }

	  private readonly ByteArrayKey key_ = new ByteArrayKey();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private String lookup(final byte[] recordData)
	  private string lookup(sbyte[] recordData)
	  {
		key_.setHashData(recordData, offset_, length_);
		return (string)cache_[key_];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void cache(final byte[] recordData, final String value)
	  private void cache(sbyte[] recordData, string value)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] key = new byte[length_];
		sbyte[] key = new sbyte[length_];
		Array.Copy(recordData, offset_, key, 0, length_);
		cache_[new ByteArrayKey(key)] = value;
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into a float value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public float getFloat(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public float getFloat(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return (float)Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return (float)Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return (float)Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return (float)Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).floatValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).floatValue();
			}
		  default:
			return float.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into a double value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public double getDouble(sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  return (double)Conv.byteArrayToShort(recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  return (double)Conv.byteArrayToInt(recordData, offset_);
			}
			else
			{
			  return (double)Conv.byteArrayToLong(recordData, offset_);
			}
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  return (double)Conv.byteArrayToFloat(recordData, offset_);
			}
			else
			{
			  return Conv.byteArrayToDouble(recordData, offset_);
			}
		  case TYPE_DECIMAL_FLOAT: // Decimal float
			if (length_ == 8)
			{
			  // DECFLOAT 16
			  string value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).doubleValue();
			}
			else
			{
			  // length must be 16
			  // DECFLOAT 34
			  string value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  return (new decimal(value)).doubleValue();
			}
		  default:
			return double.Parse(getString(recordData));
		}
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset directly into a String value, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public string getString(sbyte[] recordData)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean useCache = cache_ != null;
		bool useCache = cache_ != null;
		string value = useCache ? lookup(recordData) : null;
		if (string.ReferenceEquals(value, null))
		{
		  switch (type_)
		  {
			case '1': // BLOB or CLOB data - this should never occur, since DDM doesn't allow us to open a file containing LOB data.
			case '3': // DBCLOB data
			  throw new IOException("LOB data not allowed");
			case '4': // DATALINK data - this should never occur.
			  throw new IOException("DATALINK data not allowed");
			case TYPE_CHARACTER: // Character field
			case TYPE_DBCS_EITHER: // DBCS-Either field
			case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
			case TYPE_DBCS_ONLY: // DBCS-Only field
			case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean varlen = isVariableLength();
			  bool varlen = VariableLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = varlen ? (Conv.byteArrayToShort(recordData, offset_) * (type_ == TYPE_DBCS_GRAPHIC ? 2 : 1)) : length_;
			  int length = varlen ? (Conv.byteArrayToShort(recordData, offset_) * (type_ == TYPE_DBCS_GRAPHIC ? 2 : 1)) : length_;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = varlen ? offset_+2 : offset_;
			  int offset = varlen ? offset_ + 2 : offset_;
			  value = Conv.ebcdicByteArrayToString(recordData, offset, length, buffer_, ccsid_);
			  break;
	//        int defaultLen = WHDFTL;
	//        String defaultVal = WHDFT; // Could be *NULL or have apostrophes ' in it
			case TYPE_BINARY: // Binary field
			  //TODO Binary fields can have decimal positions, actually -- I think the decimal must get inserted after the number is converted to base-10.
			  if (numDigits_ < 5)
			  {
				value = Conv.byteArrayToShort(recordData, offset_).ToString();
			  }
			  else if (numDigits_ < 10)
			  {
				value = Conv.byteArrayToInt(recordData, offset_).ToString();
			  }
			  else
			  {
				value = Conv.byteArrayToLong(recordData, offset_).ToString();
			  }
			  break;
			case TYPE_FLOAT: // Float field
			  if (length_ == 4)
			  {
				value = Conv.byteArrayToFloat(recordData, offset_).ToString();
			  }
			  else
			  {
				value = Conv.byteArrayToDouble(recordData, offset_).ToString();
			  }
			  break;
			case TYPE_DECIMAL_FLOAT: // Decimal float
			  if (length_ == 8)
			  {
				// DECFLOAT 16
				value = Conv.decfloat16ByteArrayToString(recordData, offset_);
			  }
			  else
			  {
				// length must be 16
				// DECFLOAT 34
				value = Conv.decfloat34ByteArrayToString(recordData, offset_);
			  }
			  break;
			case TYPE_HEXADECIMAL: // Hex field
			case TYPE_BINARY_CHARACTER: // Binary character
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean varLen = isVariableLength();
			  bool varLen = VariableLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fLength = varLen ? Conv.byteArrayToShort(recordData, offset_) : length_;
			  int fLength = varLen ? Conv.byteArrayToShort(recordData, offset_) : length_;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fOffset = varLen ? offset_+2 : offset_;
			  int fOffset = varLen ? offset_ + 2 : offset_;
			  value = Conv.bytesToHexString(recordData, fOffset, fLength, buffer_);
			  break;
			case TYPE_DATE: // Date field
			  value = Conv.ebcdicByteArrayToString(recordData, offset_, length_, buffer_); //TODO - format?
			  break;
			case TYPE_PACKED_DECIMAL: // Packed decimal field
			  value = Conv.packedDecimalToString(recordData, offset_, numDigits_, decimalPositions_, buffer_);
			  break;
			case TYPE_ZONED_DECIMAL: // Zoned decimal field
			  value = Conv.zonedDecimalToString(recordData, offset_, numDigits_, decimalPositions_, buffer_);
			  break;
			case TYPE_TIME: // Time field
			  value = Conv.ebcdicByteArrayToString(recordData, offset_, length_, buffer_); //TODO - format?
			  break;
			case TYPE_TIMESTAMP: // Timestamp field
			  value = Conv.ebcdicByteArrayToString(recordData, offset_, length_, buffer_); //TODO - format?
			  break;
			default:
			  throw new IOException("Unhandled field type: '" + type_ + "'");

		  }
		  if (useCache)
		  {
			cache(recordData, value);
		  }
		}
		return value;
	  }

	  /// <summary>
	  /// Converts the specified record data at this field's offset into a date, time, or timestamp, if possible,
	  /// and sets the appropriate fields in the provided Calendar object. </summary>
	  /// <returns> true if the field data was converted and set into the Calendar object; false otherwise.
	  ///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getDate(final byte[] recordData, final Calendar cal) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public bool getDate(sbyte[] recordData, DateTime cal)
	  {
		switch (type_)
		{
		  case TYPE_DATE:
			if (!string.ReferenceEquals(dateTimeFormat_, null) && dateTimeFormat_.Equals("*ISO"))
			{
			  string value = getString(recordData);
			  int year = int.Parse(value.Substring(0,4));
			  int month = int.Parse(value.Substring(5, 2));
			  int day = int.Parse(value.Substring(8, 2));
			  cal.set(DateTime.YEAR, year);
			  cal.set(DateTime.MONTH, month - 1);
			  cal.set(DateTime.DAY_OF_MONTH, day);
			  return true;
			}
			break;
		  case TYPE_TIME:
			if (!string.ReferenceEquals(dateTimeFormat_, null) && dateTimeFormat_.Equals("*ISO"))
			{
			  string value = getString(recordData);
			  int hour = int.Parse(value.Substring(0,2));
			  int minute = int.Parse(value.Substring(3, 2));
			  int second = int.Parse(value.Substring(6, 2));
			  cal.set(DateTime.HOUR_OF_DAY, hour);
			  cal.set(DateTime.MINUTE, minute);
			  cal.set(DateTime.SECOND, second);
			  return true;
			}
			break;
		  case TYPE_TIMESTAMP:
			string value = getString(recordData);
			int year = int.Parse(value.Substring(0,4));
			int month = int.Parse(value.Substring(5, 2));
			int day = int.Parse(value.Substring(8, 2));
			int hour = int.Parse(value.Substring(11, 2));
			int minute = int.Parse(value.Substring(14, 2));
			int second = int.Parse(value.Substring(17, 2));
			int milli = int.Parse(value.Substring(20, 3));
			cal.set(DateTime.YEAR, year);
			cal.set(DateTime.MONTH, month - 1);
			cal.set(DateTime.DAY_OF_MONTH, day);
			cal.set(DateTime.HOUR_OF_DAY, hour);
			cal.set(DateTime.MINUTE, minute);
			cal.set(DateTime.SECOND, second);
			cal.set(DateTime.MILLISECOND, milli);
			return true;
		  default:
			break;
		}
		return false;
	  }

	  /// <summary>
	  /// Converts the specified Calendar value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDate(final Calendar cal, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setDate(DateTime cal, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_DATE:
			if (!string.ReferenceEquals(dateTimeFormat_, null) && dateTimeFormat_.Equals("*ISO"))
			{
			  int year = cal.Year;
			  int month = cal.Month + 1;
			  int day = cal.Day;
			  string value = year + "-" + get2(month) + "-" + get2(day);
			  Conv.stringToEBCDICByteArray37(value, recordData, offset_);
			}
			else
			{
			  throw new IOException("Unhandled date/time format: '" + dateTimeFormat_ + "'");
			}
			break;
		  case TYPE_TIME:
			if (!string.ReferenceEquals(dateTimeFormat_, null) && dateTimeFormat_.Equals("*ISO"))
			{
			  int hour = cal.Hour;
			  int minute = cal.Minute;
			  int second = cal.Second;
			  string value = get2(hour) + "." + get2(minute) + "." + get2(second);
			  Conv.stringToEBCDICByteArray37(value, recordData, offset_);
			}
			else
			{
			  throw new IOException("Unhandled date/time format: '" + dateTimeFormat_ + "'");
			}
			break;
		  case TYPE_TIMESTAMP:
			int year = cal.Year;
			int month = cal.Month + 1;
			int day = cal.Day;
			int hour = cal.Hour;
			int minute = cal.Minute;
			int second = cal.Second;
			int milli = cal.Millisecond;
			string value = year + "-" + get2(month) + "-" + get2(day) + "." + get2(hour) + "." + get2(minute) + "." + get2(second) + ".";
			if (milli < 100)
			{
				value += "0";
			}
			if (milli < 10)
			{
				value += "0";
			}
			value += milli;
			value += "000"; // Microseconds.
			Conv.stringToEBCDICByteArray37(value, recordData, offset_);
			break;
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  private readonly char[] buf2 = new char[2];

	  private string get2(int val)
	  {
		buf2[0] = (char)((val > 10) ? ((val / 10) + '0') : '0');
		buf2[1] = (char)((val % 10) + '0');
		return new string(buf2);
	  }


	  /// <summary>
	  /// Converts the specified String value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setString(String value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setString(string value, sbyte[] recordData)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean varlen = isVariableLength();
		bool varlen = VariableLength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = varlen ? offset_+2 : offset_;
		int offset = varlen ? offset_ + 2 : offset_;
		switch (type_)
		{
		  case '1': // BLOB or CLOB data - this should never occur, since DDM doesn't allow us to open a file containing LOB data.
		  case '3': // DBCLOB data
			throw new IOException("LOB data not allowed");
		  case '4': // DATALINK data - this should never occur.
			throw new IOException("DATALINK data not allowed");
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
			if (varlen)
			{
			  int num = Conv.stringToEBCDICByteArray(value, recordData, offset, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(value, recordData, offset, len, ccsid_);
			}
			break;
	//        int defaultLen = WHDFTL;
	//        String defaultVal = WHDFT; // Could be *NULL or have apostrophes ' in it
		  case TYPE_BINARY: // Binary field
			//TODO Binary fields can have decimal positions, actually -- I think the decimal must get inserted after the number is converted to base-10.
			if (numDigits_ < 5)
			{
			  short val = short.Parse(value);
			  Conv.shortToByteArray(val, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  int val = int.Parse(value);
			  Conv.intToByteArray(val, recordData, offset_);
			}
			else
			{
			  long val = long.Parse(value);
			  Conv.longToByteArray(val, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  float val = float.Parse(value);
			  Conv.floatToByteArray(val, recordData, offset_);
			}
			else
			{
			  double val = double.Parse(value);
			  Conv.doubleToByteArray(val, recordData, offset_);
			}
			break;
	//TODO        case TYPE_DECIMAL_FLOAT: // Decimal float
		  case TYPE_HEXADECIMAL: // Hex field
		  case TYPE_BINARY_CHARACTER: // Binary character
			int numBytes = Conv.hexStringToBytes(value, recordData, offset);
			if (varlen)
			{
			  Conv.shortToByteArray(numBytes, recordData, offset_);
			}
			break;
		  case TYPE_DATE: // Date field
		  case TYPE_TIME: // Time field
		  case TYPE_TIMESTAMP: // Timestamp field
			Conv.stringToEBCDICByteArray37(value, recordData, offset_);
			break;
		  case TYPE_PACKED_DECIMAL: // Packed decimal field
			Conv.stringToPackedDecimal(value, numDigits_, recordData, offset_);
			break;
		  case TYPE_ZONED_DECIMAL: // Zoned decimal field
			Conv.stringToZonedDecimal(value, numDigits_, recordData, offset_);
			break;
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified double value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDouble(final double value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setDouble(double value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray((short)value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray((int)value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray((long)value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray((float)value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray(value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified float value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFloat(final float value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setFloat(float value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray((short)value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray((int)value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray((long)value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray(value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray((double)value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified byte value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setByte(final byte value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setByte(sbyte value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray((short)value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray((int)value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray((long)value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray((float)value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray((double)value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified short value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setShort(final short value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setShort(short value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray(value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray((int)value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray((long)value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray((float)value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray((double)value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified int value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInt(final int value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setInt(int value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray((short)value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray(value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray((long)value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray((float)value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray((double)value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }

	  /// <summary>
	  /// Converts the specified long value into record data for this field's type at this field's offset, if possible.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLong(final long value, final byte[] recordData) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public void setLong(long value, sbyte[] recordData)
	  {
		switch (type_)
		{
		  case TYPE_CHARACTER: // Character field
		  case TYPE_DBCS_EITHER: // DBCS-Either field
		  case TYPE_DBCS_GRAPHIC: // DBCS-Graphic field
		  case TYPE_DBCS_ONLY: // DBCS-Only field
		  case TYPE_DBCS_OPEN: // DBCS-Open field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String s = String.valueOf(value);
			string s = value.ToString();
			if (VariableLength)
			{
			  int num = Conv.stringToEBCDICByteArray(s, recordData, offset_ + 2, ccsid_);
			  if (type_ == TYPE_DBCS_GRAPHIC)
			  {
				  num = num >> 1;
			  }
			  Conv.shortToByteArray(num, recordData, offset_);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = type_ == TYPE_DBCS_GRAPHIC ? length_*2 : length_;
			  int len = type_ == TYPE_DBCS_GRAPHIC ? length_ * 2 : length_;
			  Conv.stringToBlankPadEBCDICByteArray(s, recordData, offset_, len, ccsid_);
			}
			break;
		  case TYPE_BINARY:
			if (numDigits_ < 5)
			{
			  Conv.shortToByteArray((short)value, recordData, offset_);
			}
			else if (numDigits_ < 10)
			{
			  Conv.intToByteArray((int)value, recordData, offset_);
			}
			else
			{
			  Conv.longToByteArray(value, recordData, offset_);
			}
			break;
		  case TYPE_FLOAT: // Float field
			if (length_ == 4)
			{
			  Conv.floatToByteArray((float)value, recordData, offset_);
			}
			else
			{
			  Conv.doubleToByteArray((double)value, recordData, offset_);
			}
			break;
	//TODO      case TYPE_DECIMAL_FLOAT: // Decimal float
		  default:
			throw new IOException("Unhandled field type: '" + type_ + "'");
		}
	  }
	}

}