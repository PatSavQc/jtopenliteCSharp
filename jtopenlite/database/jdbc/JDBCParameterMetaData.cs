using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCParameterMetaData.java
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


	public class JDBCParameterMetaData : ParameterMetaData, DatabaseParameterMarkerCallback
	{
	  private Column[] columns_;
	  private int rowSize_;
	  private JDBCStatement statement_;

	  private readonly DateTime calendar_;

	  public JDBCParameterMetaData(DateTime calendarUsedForConversions)
	  {
		calendar_ = calendarUsedForConversions;
		statement_ = null;
	  }

	  internal virtual JDBCStatement Statement
	  {
		  set
		  {
			  statement_ = value;
		  }
	  }


	  public virtual void parameterMarkerDescription(int numFields, int recordSize)
	  {
		columns_ = new Column[numFields];
		for (int i = 0; i < numFields; ++i)
		{
		  columns_[i] = new Column(calendar_, i + 1, true);
		}
		rowSize_ = recordSize;
	  }

	  public virtual void parameterMarkerFieldDescription(int fieldIndex, int fieldType, int length, int scale, int precision, int ccsid, int parameterType, int joinRefPosition, int lobLocator, int lobMaxSize)
	  {
		columns_[fieldIndex].Type = fieldType;
		columns_[fieldIndex].Length = length;
		columns_[fieldIndex].Scale = scale;
		columns_[fieldIndex].Precision = precision;
		columns_[fieldIndex].CCSID = ccsid;
	  }

	  public virtual void parameterMarkerFieldName(int fieldIndex, string name)
	  {
		columns_[fieldIndex].Name = name;
	  }

	  public virtual void parameterMarkerUDTName(int fieldIndex, string name)
	  {
	  }

	  internal virtual int RowSize
	  {
		  get
		  {
			return rowSize_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Column getColumn(int fieldIndex) throws SQLException
	  internal virtual Column getColumn(int fieldIndex)
	  {
		if (columns_ == null || fieldIndex >= columns_.Length || fieldIndex < 0)
		{
			throw new SQLException("Descriptor index not valid.");
		}
		return columns_[fieldIndex];
	  }

	  internal virtual sbyte[] ExtendedSQLParameterMarkerDataFormat
	  {
		  get
		  {
			if (columns_ == null)
			{
				return null;
			}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int numFields = columns_.length;
			int numFields = columns_.Length;
			if (numFields == 0)
			{
				return null;
			}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int size = 16+(numFields*64);
			int size = 16 + (numFields * 64);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final byte[] data = new byte[size];
			sbyte[] data = new sbyte[size];
			Conv.intToByteArray(1, data, 0); // Consistency token.
			Conv.intToByteArray(numFields, data, 4);
			Conv.intToByteArray(rowSize_, data, 12);
			int offset = 16;
			for (int i = 0; i < numFields; ++i)
			{
			  const int recordSize = 64;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int type = columns_[i].getType();
			  int type = columns_[i].Type;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int length = columns_[i].getLength();
			  int length = columns_[i].Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int scale = columns_[i].getScale();
			  int scale = columns_[i].Scale;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int precision = columns_[i].getPrecision();
			  int precision = columns_[i].Precision;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int ccsid = columns_[i].getCCSID();
			  int ccsid = columns_[i].CCSID;
			  Conv.shortToByteArray(recordSize, data, offset);
			  Conv.shortToByteArray(type, data, offset + 2);
			  Conv.intToByteArray(length, data, offset + 4);
			  Conv.shortToByteArray(scale, data, offset + 8);
			  Conv.shortToByteArray(precision, data, offset + 10);
			  Conv.shortToByteArray(ccsid, data, offset + 12);
			  offset += 64;
			}
			return data;
		  }
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getParameterClassName(int param) throws SQLException
	  public virtual string getParameterClassName(int param)
	  {
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParameterCount() throws SQLException
	  public virtual int ParameterCount
	  {
		  get
		  {
			  checkRequest();
			return columns_ == null ? 0 : columns_.Length;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParameterMode(int param) throws SQLException
	  public virtual int getParameterMode(int param)
	  {
		  checkRequest();
		return ParameterMetaData.parameterModeUnknown;
	  }

	  /// 
	  /// 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParameterType(int param) throws SQLException
	  public virtual int getParameterType(int param)
	  {
		  checkRequest(param);
		return columns_[param - 1].SQLType;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getParameterTypeName(int param) throws SQLException
	  public virtual string getParameterTypeName(int param)
	  {
		  checkRequest(param);
		return columns_[param - 1].SQLTypeName;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getPrecision(int param) throws SQLException
	  public virtual int getPrecision(int param)
	  {
		  checkRequest(param);

		  return JDBCColumnMetaData.getPrecision(columns_[param - 1]);


	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getScale(int param) throws SQLException
	  public virtual int getScale(int param)
	  {
		checkRequest(param);
		return JDBCColumnMetaData.getScale(columns_[param - 1]);
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int isNullable(int param) throws SQLException
	  public virtual int isNullable(int param)
	  {
		  checkRequest(param);
		throw new NotImplementedException();
	  }

	  /// <summary>
	  /// Not implemented.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isSigned(int param) throws SQLException
	  public virtual bool isSigned(int param)
	  {
		  checkRequest(param);
		throw new NotImplementedException();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkRequest(int param) throws SQLException
	  private void checkRequest(int param)
	  {
		  checkRequest();

		  if (columns_ == null || (param < 1) || (param > columns_.Length))
		  {
				throw JDBCError.getSQLException(JDBCError.EXC_DESCRIPTOR_INDEX_INVALID);
		  }

	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkRequest() throws SQLException
	  private void checkRequest()
	  {
		  if (statement_.Closed)
		  {
		  throw JDBCError.getSQLException(JDBCError.EXC_FUNCTION_SEQUENCE);
		  }
	  }
	}

}