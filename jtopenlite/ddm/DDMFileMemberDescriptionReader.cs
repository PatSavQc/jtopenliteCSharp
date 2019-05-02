using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMFileMemberDescriptionReader.java
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

	internal sealed class DDMFileMemberDescriptionReader : DDMReadCallback
	{
	  private readonly int serverCCSID_;

	  private bool eof_ = false;

	  private readonly List<DDMFileMemberDescription> memberDescriptions_ = new List<DDMFileMemberDescription>();

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMFileMemberDescriptionReader(final int serverCCSID)
	  internal DDMFileMemberDescriptionReader(int serverCCSID)
	  {
		serverCCSID_ = serverCCSID;
	  }

	  internal IList<DDMFileMemberDescription> MemberDescriptions
	  {
		  get
		  {
			return memberDescriptions_;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempData = dataBuffer.getRecordDataBuffer();
		sbyte[] tempData = dataBuffer.RecordDataBuffer;
		//final int recordNumber = dataBuffer.getRecordNumber();

		string fileName = Conv.ebcdicByteArrayToString(tempData, 13, 10).Trim(); // MBFILE
		string libName = Conv.ebcdicByteArrayToString(tempData, 23, 10).Trim(); // MBLIB
		string fileType = Conv.ebcdicByteArrayToString(tempData, 33, 1); // MBFTYP
		string fileAttrib = Conv.ebcdicByteArrayToString(tempData, 41, 6).Trim(); // MBFATR
		string systemName = Conv.ebcdicByteArrayToString(tempData, 47, 8).Trim(); // MBSYSN
		//int aspID = Integer.valueOf(Conv.packedDecimalToString(tempData, 55, 3, 0)); // MBASP
		string dataType = Conv.ebcdicByteArrayToString(tempData, 61, 1); // MBDTAT
		//int maxFileWaitTime = Integer.valueOf(Conv.packedDecimalToString(tempData, 62, 5, 0)); //MBWAIT
		//int maxRecordWaitTime = Integer.valueOf(Conv.packedDecimalToString(tempData, 65, 5, 0)); // MBWATR
		//String rfLevelCheck = Conv.ebcdicByteArrayToString(tempData, 69, 1); // MBLVLC
		string description = Conv.ebcdicByteArrayToString(tempData, 70, 50).Trim(); // MBTXT
		//int numRecordFormats = Integer.valueOf(Conv.packedDecimalToString(tempData, 120, 5, 0)); // MBNOFM
		string memberName = Conv.ebcdicByteArrayToString(tempData, 163, 10).Trim(); // MBNAME
		string memberDescription = Conv.ebcdicByteArrayToString(tempData, 193, 50).Trim(); // MBMTXT
		long recordCapacity = Convert.ToInt64(Conv.packedDecimalToString(tempData, 337, 10, 0)); // MBRCDC
		long currentRecords = Convert.ToInt64(Conv.packedDecimalToString(tempData, 343, 10, 0)); // MBNRCD
		long deletedRecords = Convert.ToInt64(Conv.packedDecimalToString(tempData, 349, 10, 0)); // MBNDTR

		memberDescriptions_.Add(new DDMFileMemberDescription(fileName, libName, fileType, fileAttrib, systemName, dataType, description, memberName, memberDescription, recordCapacity, currentRecords, deletedRecords));
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