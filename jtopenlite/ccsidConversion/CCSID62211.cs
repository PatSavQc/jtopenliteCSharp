﻿///////////////////////////////////////////////////////////////////////////////
//
// JTOpen (IBM Toolbox for Java - OSS version)
//
// Filename:  CCSID62211.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ccsidConversion
{
	public class CCSID62211 : SingleByteConversion
	{
	  internal static CCSID62211 singleton = new CCSID62211();

	  public static SingleByteConversion Instance
	  {
		  get
		  {
			return singleton;
		  }
	  }

	  public virtual int Ccsid
	  {
		  get
		  {
			return 62211;
		  }
	  }

	  public virtual sbyte[] returnFromUnicode()
	  {
		return fromUnicode_;
	  }

	  public virtual char[] returnToUnicode()
	  {
		return toUnicode_;
	  }
	  private static readonly char[] toUnicode_ = new char[] {'\u0000', '\u0001', '\u0002', '\u0003', '\u009C', '\u0009', '\u0086', '\u007F', '\u0097', '\u008D', '\u008E', '\u000B', '\u000C', (char)0xD, '\u000E', '\u000F', '\u0010', '\u0011', '\u0012', '\u0013', '\u009D', '\u0085', '\u0008', '\u0087', '\u0018', '\u0019', '\u0092', '\u008F', '\u001C', '\u001D', '\u001E', '\u001F', '\u0080', '\u0081', '\u0082', '\u0083', '\u0084', (char)0xA, '\u0017', '\u001B', '\u0088', '\u0089', '\u008A', '\u008B', '\u008C', '\u0005', '\u0006', '\u0007', '\u0090', '\u0091', '\u0016', '\u0093', '\u0094', '\u0095', '\u0096', '\u0004', '\u0098', '\u0099', '\u009A', '\u009B', '\u0014', '\u0015', '\u009E', '\u001A', '\u0020', '\u05D0', '\u05D1', '\u05D2', '\u05D3', '\u05D4', '\u05D5', '\u05D6', '\u05D7', '\u05D8', '\u00A2', '\u002E', '\u003C', '\u0028', '\u002B', '\u007C', '\u0026', '\u05D9', '\u05DA', '\u05DB', '\u05DC', '\u05DD', '\u05DE', '\u05DF', '\u05E0', '\u05E1', '\u0021', '\u0024', '\u002A', '\u0029', '\u003B', '\u00AC', '\u002D', '\u002F', '\u05E2', '\u05E3', '\u05E4', '\u05E5', '\u05E6', '\u05E7', '\u05E8', '\u05E9', '\u00A6', '\u002C', '\u0025', '\u005F', '\u003E', '\u003F', '\u001A', '\u05EA', '\u001A', '\u001A', '\u00A0', '\u001A', '\u001A', '\u001A', '\u2017', '\u0060', '\u003A', '\u0023', '\u0040', (char)0x27, '\u003D', '\u0022', '\u001A', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', '\u0066', '\u0067', '\u0068', '\u0069', '\u00AB', '\u00BB', '\u001A', '\u001A', '\u001A', '\u00B1', '\u00B0', '\u006A', '\u006B', '\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071', '\u0072', '\u001A', '\u001A', '\u20AC', '\u00B8', '\u20AA', '\u00A4', '\u00B5', '\u007E', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', '\u0078', '\u0079', '\u007A', '\u001A', '\u001A', '\u001A', '\u001A', '\u001A', '\u00AE', '\u005E', '\u00A3', '\u00A5', '\u2022', '\u00A9', '\u00A7', '\u00B6', '\u00BC', '\u00BD', '\u00BE', '\u005B', '\u005D', '\u203E', '\u00A8', '\u00B4', '\u00D7', '\u007B', '\u0041', '\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', '\u0048', '\u0049', '\u00AD', '\u001A', '\u001A', '\u001A', '\u001A', '\u001A', '\u007D', '\u004A', '\u004B', '\u004C', '\u004D', '\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u00B9', '\u202D', '\u202E', '\u202C', '\u001A', '\u001A', (char)0x5C, '\u00F7', '\u0053', '\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', '\u005A', '\u00B2', '\u001A', '\u001A', '\u001A', '\u001A', '\u001A', '\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', '\u0036', '\u0037', '\u0038', '\u0039', '\u00B3', '\u202A', '\u202B', '\u200E', '\u200F', '\u009F'};


	  private static sbyte[] fromUnicode_ = null;
	  /* dynamically generate the inverse table */
	  static CCSID62211()
	  {
		  fromUnicode_ = SingleByteConversionTable.generateFromUnicode(toUnicode_);
	  }

	}

}