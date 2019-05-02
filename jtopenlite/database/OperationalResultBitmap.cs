///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  OperationalResultBitmap
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.database
{
	/// <summary>
	/// The attributes should be OR-ed together when talking to the database host server.
	/// 
	/// </summary>
	internal interface OperationalResultBitmap
	{
	}

	public static class OperationalResultBitmap_Fields
	{
	  public const int SEND_REPLY_IMMED = unchecked((int)0x80000000);
	  public const int MESSAGE_ID = 0x40000000;
	  public const int FIRST_LEVEL_TEXT = 0x20000000;
	  public const int SECOND_LEVEL_TEXT = 0x10000000;
	  public const int DATA_FORMAT = 0x08000000;
	  public const int RESULT_DATA = 0x04000000;
	  public const int SQLCA = 0x02000000;
	  public const int SERVER_ATTRIBUTES = 0x01000000;
	  public const int PARAMETER_MARKER_FORMAT = 0x00800000;
	  public const int TRANSLATION_TABLES = 0x00400000;
	  public const int DATA_SOURCE_INFORMATION = 0x00200000;
	  public const int PACKAGE_INFORMATION = 0x00100000;
	  public const int REQUST_RLE_COMPRESSED = 0x00080000;
	  public const int REPLY_RLE_COMPRESSED = 0x00040000;
	  public const int EXTENDED_COLUMN_DESCRIPTORS = 0x00020000;
	  public const int REPLY_VARLEN_COLUMN_COMPRESSED = 0x00010000;
	  public const int RETURN_RESULT_SET_ATTRIBUTES = 0x00008000;
	}


}