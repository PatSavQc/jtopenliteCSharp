using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMRecordFormatReader.java
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

	internal sealed class DDMRecordFormatReader : DDMReadCallback
	{
	  private readonly int serverCCSID_;

	  private bool eof_ = false;

	  private string library_;
	  private string file_;
	  private string name_;
	  private string type_;
	  private string text_;
	  private DDMField[] fields_;

	  private int totalLength_ = 0;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMRecordFormatReader(final int serverCCSID)
	  internal DDMRecordFormatReader(int serverCCSID)
	  {
		serverCCSID_ = serverCCSID;
	  }

	  internal string Library
	  {
		  get
		  {
			return library_;
		  }
	  }

	  internal string File
	  {
		  get
		  {
			return file_;
		  }
	  }

	  internal string Name
	  {
		  get
		  {
			return name_;
		  }
	  }

	  internal string Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  internal string Text
	  {
		  get
		  {
			return text_;
		  }
	  }

	  internal DDMField[] Fields
	  {
		  get
		  {
			return fields_;
		  }
	  }

	  internal int Length
	  {
		  get
		  {
			return totalLength_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void newRecord(final DDMCallbackEvent event, final DDMDataBuffer dataBuffer)
	  public void newRecord(DDMCallbackEvent @event, DDMDataBuffer dataBuffer)
	  {
		if (eof_)
		{
			return;
		}
	//    int rfCount = Integer.valueOf(Conv.zonedDecimalToString(tempData, 28, 5, 0)); // WHCNT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempData = dataBuffer.getRecordDataBuffer();
		sbyte[] tempData = dataBuffer.RecordDataBuffer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int recordNumber = dataBuffer.getRecordNumber();
		int recordNumber = dataBuffer.RecordNumber;
		if (fields_ == null)
		{
		  int numFields = Convert.ToInt32(Conv.zonedDecimalToString(tempData, 361, 5, 0)); // WHNFLD
		  fields_ = new DDMField[numFields];
		  name_ = Conv.ebcdicByteArrayToString(tempData, 46, 10); // WHNAME
		  library_ = Conv.ebcdicByteArrayToString(tempData, 10, 10); // WHLIB
		  file_ = Conv.ebcdicByteArrayToString(tempData, 0, 10); // WHFILE
		  type_ = Conv.ebcdicByteArrayToString(tempData, 27, 1); // WHFTYP
		  text_ = Conv.ebcdicByteArrayToString(tempData, 69, 50); // WHTEXT
		  if (numFields == 0)
		  {
			  return; // Save files have no fields, for example.
		  }
		}
		else if (recordNumber > fields_.Length)
		{
		  //TODO - More than one record format, we'll support that later.
		  eof_ = true;
		  return;
		}

		string fieldName = Conv.ebcdicByteArrayToString(tempData, 139, 10).Trim(); // WHFLDE;
		int fieldByteLength = Convert.ToInt32(Conv.zonedDecimalToString(tempData, 159, 5, 0)); // WHFLDB
		int numDigits = Convert.ToInt32(Conv.zonedDecimalToString(tempData, 164, 2, 0)); // WHFLDO
		int decimalPositions = Convert.ToInt32(Conv.zonedDecimalToString(tempData, 166, 2, 0)); // WHFLDP
		string fieldText = Conv.ebcdicByteArrayToString(tempData, 168, 50); // WHFTXT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char fieldType = Conv.ebcdicByteArrayToString(tempData, 321, 1).charAt(0);
		char fieldType = Conv.ebcdicByteArrayToString(tempData, 321, 1)[0]; // WHFLDT
		int defaultValueLength = Convert.ToInt32(Conv.zonedDecimalToString(tempData, 402, 2, 0)); // WHDFTL
		string defaultValue = Conv.ebcdicByteArrayToString(tempData, 404, defaultValueLength > 30 ? 30 : defaultValueLength); // WHDFT
		int ccsid = Convert.ToInt32(Conv.packedDecimalToString(tempData, 491, 5, 0)); // WHCCSID
		string dateTimeFormat = Conv.ebcdicByteArrayToString(tempData, 494, 4); // WHFMT
		string dateTimeSeparator = Conv.ebcdicByteArrayToString(tempData, 498, 1); // WHSEP
		string variableLengthField = Conv.ebcdicByteArrayToString(tempData, 499, 1); // WHVARL
		int allocatedLength = Convert.ToInt32(Conv.packedDecimalToString(tempData, 500, 5, 0)); // WHALLC
		string allowNulls = Conv.ebcdicByteArrayToString(tempData, 503, 1); // WHNULL;

		if (ccsid == 65535)
		{
			ccsid = serverCCSID_;
		}

		fields_[recordNumber - 1] = new DDMField(totalLength_, fieldName, fieldByteLength, numDigits, decimalPositions, fieldText, fieldType, defaultValue, ccsid, variableLengthField, allocatedLength, allowNulls, dateTimeFormat, dateTimeSeparator);
		totalLength_ += fieldByteLength;
	//    String alias = Conv.ebcdicByteArrayToString(tempData, 370,30); // WHALIS
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void recordNotFound(final DDMCallbackEvent event)
	  public void recordNotFound(DDMCallbackEvent @event)
	  {
		eof_ = true;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void endOfFile(final DDMCallbackEvent event)
	  public void endOfFile(DDMCallbackEvent @event)
	  {
		eof_ = true;
	  }

	  internal bool eof()
	  {
		return eof_;
	  }
	}

}