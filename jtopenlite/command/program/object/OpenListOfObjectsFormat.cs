using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  OpenListOfObjectsFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.@object
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command.program.openlist;

	public class OpenListOfObjectsFormat : ListEntryFormat<OpenListOfObjectsFormatListener>
	{
	  private readonly char[] charBuffer_ = new char[10];

	  public OpenListOfObjectsFormat()
	  {
	  }

	  private readonly sbyte[] lastLibraryBytes_ = new sbyte[10];
	  private string lastLibrary_ = "          ";
	  private readonly sbyte[] lastTypeBytes_ = new sbyte[10];
	  private string lastType_ = "          ";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static boolean matches(final byte[] data, final int offset, final byte[] data2)
	  private static bool matches(sbyte[] data, int offset, sbyte[] data2)
	  {
		for (int i = 0; i < data2.Length; ++i)
		{
		  if (data[offset + i] != data2[i])
		  {
			  return false;
		  }
		}
		return true;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getLibrary(final byte[] data, final int numRead)
	  private void getLibrary(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastLibraryBytes_))
		{
		  Array.Copy(data, numRead, lastLibraryBytes_, 0, 10);
		  lastLibrary_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void getType(final byte[] data, final int numRead)
	  private void getType(sbyte[] data, int numRead)
	  {
		if (!matches(data, numRead, lastTypeBytes_))
		{
		  Array.Copy(data, numRead, lastTypeBytes_, 0, 10);
		  lastType_ = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void format(final byte[] data, final int maxLength, final int recordLength, OpenListOfObjectsFormatListener listener)
	  public virtual void format(sbyte[] data, int maxLength, int recordLength, OpenListOfObjectsFormatListener listener)
	  {
		if (listener == null)
		{
		  return;
		}

		int numRead = 0;
		while (numRead + 36 <= maxLength)
		{
		  string objectName = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  numRead += 10;
	//      String objectLibrary = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getLibrary(data, numRead);
		  string objectLibrary = lastLibrary_;
		  numRead += 10;
	//      String objectType = Conv.ebcdicByteArrayToString(data, numRead, 10, charBuffer_);
		  getType(data, numRead);
		  string objectType = lastType_;
		  numRead += 10;
		  string informationStatus = Conv.ebcdicByteArrayToString(data, numRead, 1, charBuffer_);
		  numRead += 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numFields = Conv.byteArrayToInt(data, numRead);
		  int numFields = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
		  listener.newObjectEntry(objectName, objectLibrary, objectType, informationStatus, numFields);
		  if (!informationStatus.Equals("A") && !informationStatus.Equals("L"))
		  {
			for (int i = 0; i < numFields; ++i)
			{
			  int lengthOfFieldInfo = Conv.byteArrayToInt(data, numRead);
			  int keyField = Conv.byteArrayToInt(data, numRead + 4);
			  string typeOfData = Conv.ebcdicByteArrayToString(data, numRead + 8, 1, charBuffer_);
			  int lengthOfData = Conv.byteArrayToInt(data, numRead + 12);
			  listener.newObjectFieldData(lengthOfFieldInfo, keyField, typeOfData, lengthOfData, numRead + 16, data);
			  numRead += lengthOfFieldInfo;
			}
		  }
		  else
		  {
			numRead += (recordLength - 36);
		  }
		}
	  }
	}

}