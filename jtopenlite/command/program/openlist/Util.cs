using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: Util.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{
	using com.ibm.jtopenlite;


	/// <summary>
	/// Internal use.
	/// 
	/// </summary>
	public sealed class Util
	{
	  private Util()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static ListInformation readOpenListInformationParameter(final byte[] data, final int maxLength)
	  public static ListInformation readOpenListInformationParameter(sbyte[] data, int maxLength) //throws IOException
	  {
		ListInformation info = null;
		int numRead = 0;
		if (maxLength >= 12)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int totalRecords = Conv.byteArrayToInt(data, numRead);
		  int totalRecords = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int recordsReturned = Conv.byteArrayToInt(data, numRead);
		  int recordsReturned = Conv.byteArrayToInt(data, numRead);
		  numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] handle = new byte[4];
		  sbyte[] handle = new sbyte[4];
		  //in.readFully(handle);
		  Array.Copy(data, numRead, handle, 0, 4);
		  numRead += 4;

		  int recordLength = 0;
		  int infoCompleteType = ListInformation.TYPE_UNKNOWN;
		  string creationDate = null;
		  int status = ListInformation.STATUS_UNKNOWN;
		  int lengthOfInfoReturned = 0;
		  int firstRecord = 0;

		  if (maxLength >= 31)
		  {
			recordLength = Conv.byteArrayToInt(data, numRead);
			numRead += 4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int infoComplete = data[numRead++] & 0x00FF;
			int infoComplete = data[numRead++] & 0x00FF;
			switch (infoComplete)
			{
			  case 0x00C3:
				  infoCompleteType = ListInformation.TYPE_COMPLETE;
				  break;
			  case 0x00C9:
				  infoCompleteType = ListInformation.TYPE_INCOMPLETE;
				  break;
			  case 0x00D7:
				  infoCompleteType = ListInformation.TYPE_PARTIAL;
				  break;
			}
	//        final byte[] created = new byte[13];
	//        in.readFully(created);
	//        creationDate = new String(created, "Cp037");
			creationDate = Conv.ebcdicByteArrayToString(data, numRead, 13);
			numRead += 13;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int listStatus = data[numRead++] & 0x00FF;
			int listStatus = data[numRead++] & 0x00FF;
			switch (listStatus)
			{
			  case 0x00F0:
				  status = ListInformation.STATUS_PENDING;
				  break;
			  case 0x00F1:
				  status = ListInformation.STATUS_BUILDING;
				  break;
			  case 0x00F2:
				  status = ListInformation.STATUS_BUILT;
				  break;
			  case 0x00F3:
				  status = ListInformation.STATUS_ERROR;
				  break;
			  case 0x00F4:
				  status = ListInformation.STATUS_PRIMED;
				  break;
			  case 0x00F5:
				  status = ListInformation.STATUS_OVERFLOW;
				  break;
			}
			numRead += 19;
			if (maxLength >= 40)
			{
			  //in.read();
			  numRead++;
			  lengthOfInfoReturned = Conv.byteArrayToInt(data, numRead);
			  numRead += 4;
			  firstRecord = Conv.byteArrayToInt(data, numRead);
			  numRead += 4;
			}
		  }
		  info = new ListInformation(totalRecords, recordsReturned, handle, recordLength, infoCompleteType, creationDate, status, lengthOfInfoReturned, firstRecord);
		}
	//    in.skipBytes(maxLength-numRead);
		return info;
	  }
	}


}