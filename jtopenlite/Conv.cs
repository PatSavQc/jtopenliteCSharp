using System;
using System.Collections;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  Conv.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	using CcsidConversion = com.ibm.jtopenlite.ccsidConversion.CcsidConversion;

	/// <summary>
	/// Utility class for converting data from one format to another.
	/// 
	/// </summary>
	public sealed class Conv
	{

	  private static Hashtable localeNlvMap_;

	  private static readonly char[] NUM = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
	  private static readonly sbyte[] CHAR_HIGH = new sbyte[10];
	  private static readonly sbyte[] CHAR_LOW = new sbyte[10];
	  static Conv()
	  {
		for (int i = 0; i <= 9; ++i)
		{
		  int val = i;
		  CHAR_HIGH[i] = (sbyte)(val << 4);
		  CHAR_LOW[i] = (sbyte)val;
		}
		Array.Copy(INIT_TO_37, 0, CONV_TO_37, 0, INIT_TO_37.Length);
		for (int i = INIT_TO_37.Length; i < CONV_TO_37.Length; ++i)
		{
		  CONV_TO_37[i] = 0x3F;
		}
		char[] buffer = new char[1];
		sbyte[] data = new sbyte[1];
		for (int i = 0; i < CACHE_FROM_37.Length; ++i)
		{
		  data[0] = (sbyte)i;
		  CACHE_FROM_37[i] = ebcdicByteArrayToString(data, 0, 1, buffer);
		}
		cacheFrom37Init_ = true;
		// 137+ possible Java encodings. 13 have unknown CCSIDs.
		// We have 128 known in this table.
		encodingCcsid_["ASCII"] = "367"; // ANSI X.34 ASCI.
		encodingCcsid_["Cp1252"] = "1252";
		encodingCcsid_["ISO8859_1"] = "819";
		encodingCcsid_["Unicode"] = "13488";
		encodingCcsid_["UnicodeBig"] = "13488"; // BOM is 0xFEFF.
		// encodingCcsid_.put("UnicodeBigUnmarked", 13488);
		encodingCcsid_["UnicodeLittle"] = "1202"; // BOM is 0xFFFE.
		// encodingCcsid_.put("UnicodeLittleUnmarked", 13488);
		encodingCcsid_["UTF8"] = "1208";
		encodingCcsid_["UTF-8"] = "1208";
		encodingCcsid_["UTF-16BE"] = "1200";

		encodingCcsid_["Big5"] = "950";
		// encodingCcsid_.put("Big5 HKSCS", ???); // Big5 with Hong Kong extensions.
		encodingCcsid_["CNS11643"] = "964";
		encodingCcsid_["Cp037"] = "37";
		encodingCcsid_["Cp256"] = "256";
		encodingCcsid_["Cp273"] = "273";
		encodingCcsid_["Cp277"] = "277";
		encodingCcsid_["Cp278"] = "278";
		encodingCcsid_["Cp280"] = "280";
		encodingCcsid_["Cp284"] = "284";
		encodingCcsid_["Cp285"] = "285";
		encodingCcsid_["Cp290"] = "290";
		encodingCcsid_["Cp297"] = "297";
		encodingCcsid_["Cp420"] = "420";
		encodingCcsid_["Cp423"] = "423";
		encodingCcsid_["Cp424"] = "424";
		encodingCcsid_["Cp437"] = "437";
		encodingCcsid_["Cp500"] = "500";
		encodingCcsid_["Cp737"] = "737";
		encodingCcsid_["Cp775"] = "775";
		encodingCcsid_["Cp833"] = "833";
		encodingCcsid_["Cp838"] = "838";
		encodingCcsid_["Cp850"] = "850";
		encodingCcsid_["Cp852"] = "852";
		encodingCcsid_["Cp855"] = "855";
		encodingCcsid_["Cp856"] = "856";
		encodingCcsid_["Cp857"] = "857";
		encodingCcsid_["Cp858"] = "858";
		encodingCcsid_["Cp860"] = "860";
		encodingCcsid_["Cp861"] = "861";
		encodingCcsid_["Cp862"] = "862";
		encodingCcsid_["Cp863"] = "863";
		encodingCcsid_["Cp864"] = "864";
		encodingCcsid_["Cp865"] = "865";
		encodingCcsid_["Cp866"] = "866";
		encodingCcsid_["Cp868"] = "868";
		encodingCcsid_["Cp869"] = "869";
		encodingCcsid_["Cp870"] = "870";
		encodingCcsid_["Cp871"] = "871";
		encodingCcsid_["Cp874"] = "874";
		encodingCcsid_["Cp875"] = "875";
		encodingCcsid_["Cp880"] = "880";
		encodingCcsid_["Cp905"] = "905";
		encodingCcsid_["Cp918"] = "918";
		encodingCcsid_["Cp921"] = "921";
		encodingCcsid_["Cp922"] = "922";
		encodingCcsid_["Cp923"] = "923"; // IBM Latin-9.
		encodingCcsid_["Cp924"] = "924";
		encodingCcsid_["Cp930"] = "930";
		encodingCcsid_["Cp933"] = "933";
		encodingCcsid_["Cp935"] = "935";
		encodingCcsid_["Cp937"] = "937";
		encodingCcsid_["Cp939"] = "939";
		encodingCcsid_["Cp942"] = "942";
		// encodingCcsid_.put("Cp942C",    ???);  // Don't know the CCSID - unclear what the 'C' means.
		encodingCcsid_["Cp943"] = "943";
		// encodingCcsid_.put("Cp943C",    ???); // Don't know the CCSID - unclear what the 'C' means.
		encodingCcsid_["Cp948"] = "948";
		encodingCcsid_["Cp949"] = "949";
		// encodingCcsid_.put("Cp949C",    ???); // Don't know the CCSID - unclear what the 'C' means.
		encodingCcsid_["Cp950"] = "950";
		encodingCcsid_["Cp964"] = "964";
		encodingCcsid_["Cp970"] = "970";
		encodingCcsid_["Cp1006"] = "1006";
		encodingCcsid_["Cp1025"] = "1025";
		encodingCcsid_["Cp1026"] = "1026";
		encodingCcsid_["Cp1027"] = "1027";
		encodingCcsid_["Cp1046"] = "1046";
		encodingCcsid_["Cp1097"] = "1097";
		encodingCcsid_["Cp1098"] = "1098";
		encodingCcsid_["Cp1112"] = "1112";
		encodingCcsid_["Cp1122"] = "1122";
		encodingCcsid_["Cp1123"] = "1123";
		encodingCcsid_["Cp1124"] = "1124";
		encodingCcsid_["Cp1130"] = "1130";
		encodingCcsid_["Cp1132"] = "1132";
		encodingCcsid_["Cp1137"] = "1137";
		encodingCcsid_["Cp1140"] = "1140";
		encodingCcsid_["Cp1141"] = "1141";
		encodingCcsid_["Cp1142"] = "1142";
		encodingCcsid_["Cp1143"] = "1143";
		encodingCcsid_["Cp1144"] = "1144";
		encodingCcsid_["Cp1145"] = "1145";
		encodingCcsid_["Cp1146"] = "1146";
		encodingCcsid_["Cp1147"] = "1147";
		encodingCcsid_["Cp1148"] = "1148";
		encodingCcsid_["Cp1149"] = "1149";
		encodingCcsid_["Cp1153"] = "1153";
		encodingCcsid_["Cp1154"] = "1154";
		encodingCcsid_["Cp1155"] = "1155";
		encodingCcsid_["Cp1156"] = "1156";
		encodingCcsid_["Cp1157"] = "1157";
		encodingCcsid_["Cp1158"] = "1158";
		encodingCcsid_["Cp1160"] = "1160";
		encodingCcsid_["Cp1164"] = "1164";
		encodingCcsid_["Cp1250"] = "1250";
		encodingCcsid_["Cp1251"] = "1251";
		encodingCcsid_["Cp1253"] = "1253";
		encodingCcsid_["Cp1254"] = "1254";
		encodingCcsid_["Cp1255"] = "1255";
		encodingCcsid_["Cp1256"] = "1256";
		encodingCcsid_["Cp1257"] = "1257";
		encodingCcsid_["Cp1258"] = "1258";
		encodingCcsid_["Cp1364"] = "1364";
		encodingCcsid_["Cp1381"] = "1381";
		encodingCcsid_["Cp1383"] = "1383";
		encodingCcsid_["Cp1388"] = "1388";
		encodingCcsid_["Cp1399"] = "1399";
		encodingCcsid_["Cp4971"] = "4971";
		encodingCcsid_["Cp5123"] = "5123";
		encodingCcsid_["Cp9030"] = "9030";
		encodingCcsid_["Cp13121"] = "13121";
		encodingCcsid_["Cp13124"] = "13124";
		encodingCcsid_["Cp28709"] = "28709";
		encodingCcsid_["Cp33722"] = "33722";

		// The Toolbox does not directly support EUC at this time, Java will do the conversion.
		encodingCcsid_["EUC_CN"] = "1383"; // Superset of 5479.
		encodingCcsid_["EUC_JP"] = "33722";
		encodingCcsid_["EUC_KR"] = "970"; // Superset of 5066.
		encodingCcsid_["EUC_TW"] = "964"; // Superset of 5060.

		encodingCcsid_["GB2312"] = "1381";
		encodingCcsid_["GB18030"] = "1392"; //1392 is mixed 4-byte; the individual component CCSIDs are not supported.
		encodingCcsid_["GBK"] = "1386";

		// encodingCcsid_.put("ISCII91", ???); // Indic scripts.

		// The Toolbox does not directly support ISO2022.
		// encodingCcsid_.put("ISO2022CN",     ???);  // Not sure of the CCSID, possibly 9575?
		// encodingCcsid_.put("ISO2022CN_CNS", "965");  // Java doesn't support this one?
		// encodingCcsid_.put("ISO2022CN_GB",  "9575");  // Java doesn't support this one?

		encodingCcsid_["ISO2022JP"] = "5054"; // Could be 956 also, but the IBM i JVM uses 5054.
		encodingCcsid_["ISO2022KR"] = "25546"; // Could be 17354 also, but the IBM i JVM uses 25546.

		encodingCcsid_["ISO8859_2"] = "912";
		encodingCcsid_["ISO8859_3"] = "913";
		encodingCcsid_["ISO8859_4"] = "914";
		encodingCcsid_["ISO8859_5"] = "915";
		encodingCcsid_["ISO8859_6"] = "1089";
		encodingCcsid_["ISO8859_7"] = "813";
		encodingCcsid_["ISO8859_8"] = "916";
		encodingCcsid_["ISO8859_9"] = "920";
		// encodingCcsid_.put("ISO8859_13", ???);  // Latin alphabet No. 7.
		// encodingCcsid_.put("ISO8859_15_FDIS", ???); // Don't know the CCSID; FYI, this codepage is ISO 28605.

		// The Toolbox does not directly support JIS.
		encodingCcsid_["JIS0201"] = "897"; // Could be 895, but the IBM i JVM uses 897.
		encodingCcsid_["JIS0208"] = "952";
		encodingCcsid_["JIS0212"] = "953";
		// encodingCcsid_.put("JISAutoDetect", ???); // Can't do this one. Would need to look at the bytes to determine the CCSID.

		encodingCcsid_["Johab"] = "1363";
		encodingCcsid_["KOI8_R"] = "878";
		encodingCcsid_["KSC5601"] = "949";

		encodingCcsid_["MS874"] = "874";
		encodingCcsid_["MS932"] = "943";
		encodingCcsid_["MS936"] = "1386";
		encodingCcsid_["MS949"] = "949";
		encodingCcsid_["MS950"] = "950";

		// encodingCcsid_.put("MacArabic", ???); // Don't know.
		encodingCcsid_["MacCentralEurope"] = "1282";
		encodingCcsid_["MacCroatian"] = "1284";
		encodingCcsid_["MacCyrillic"] = "1283";
		// encodingCcsid_.put("MacDingbat", ???); // Don't know.
		encodingCcsid_["MacGreek"] = "1280";
		// encodingCcsid_.put("MacHebrew", ???); // Don't know.
		encodingCcsid_["MacIceland"] = "1286";
		encodingCcsid_["MacRoman"] = "1275";
		encodingCcsid_["MacRomania"] = "1285";
		// encodingCcsid_.put("MacSymbol", ???); // Don't know.
		// encodingCcsid_.put("MacThai", ???); // Don't know.
		encodingCcsid_["MacTurkish"] = "1281";
		// encodingCcsid_.put("MacUkraine", ???); // Don't know.

		encodingCcsid_["SJIS"] = "932"; // Could be 943, but the IBM i JVM uses 932.
		encodingCcsid_["TIS620"] = "874"; // IBM i JVM uses 874.
		// Build the CCSID to encoding map.
		System.Collections.IEnumerator it = encodingCcsid_.Keys.GetEnumerator();
		while (it.MoveNext())
		{
		  object key = it.Current;
		  ccsidEncoding_[encodingCcsid_[key]] = key;
		}

		ccsidEncoding_["13488"] = "UTF-16BE";
		ccsidEncoding_["61952"] = "UTF-16BE";
		ccsidEncoding_["17584"] = "UTF-16BE"; // IBM i doesn't support this, but other people use it.

		it = ccsidEncoding_.Keys.GetEnumerator();
		while (it.MoveNext())
		{
		  string ccsid = (string)it.Current;
		  string encoding = (string)ccsidEncoding_[ccsid];
		  int i = (Convert.ToInt32(ccsid));
		  encodings_[i] = encoding;
		}
	  }

	  // The array offset is the Unicode character value, the array value is the EBCDIC 37 value.
	  // e.g. CONV_TO_37['0'] == 0xF0  and  CONV_TO_37[' '] == 0x40
	  private static readonly sbyte[] CONV_TO_37 = new sbyte[65536];

	  private static readonly sbyte[] INIT_TO_37 = new sbyte[] {0x00, 0x01, 0x02, 0x03, 0x37, 0x2D, 0x2E, 0x2F, 0x16, 0x05, 0x25, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x3C, 0x3D, 0x32, 0x26, 0x18, 0x19, 0x3F, 0x27, 0x1C, 0x1D, 0x1E, 0x1F, 0x40, 0x5A, 0x7F, 0x7B, 0x5B, 0x6C, 0x50, 0x7D, 0x4D, 0x5D, 0x5C, 0x4E, 0x6B, 0x60, 0x4B, 0x61, unchecked((sbyte)0xF0), unchecked((sbyte)0xF1), unchecked((sbyte)0xF2), unchecked((sbyte)0xF3), unchecked((sbyte)0xF4), unchecked((sbyte)0xF5), unchecked((sbyte)0xF6), unchecked((sbyte)0xF7), unchecked((sbyte)0xF8), unchecked((sbyte)0xF9), 0x7A, 0x5E, 0x4C, 0x7E, 0x6E, 0x6F, 0x7C, unchecked((sbyte)0xC1), unchecked((sbyte)0xC2), unchecked((sbyte)0xC3), unchecked((sbyte)0xC4), unchecked((sbyte)0xC5), unchecked((sbyte)0xC6), unchecked((sbyte)0xC7), unchecked((sbyte)0xC8), unchecked((sbyte)0xC9), unchecked((sbyte)0xD1), unchecked((sbyte)0xD2), unchecked((sbyte)0xD3), unchecked((sbyte)0xD4), unchecked((sbyte)0xD5), unchecked((sbyte)0xD6), unchecked((sbyte)0xD7), unchecked((sbyte)0xD8), unchecked((sbyte)0xD9), unchecked((sbyte)0xE2), unchecked((sbyte)0xE3), unchecked((sbyte)0xE4), unchecked((sbyte)0xE5), unchecked((sbyte)0xE6), unchecked((sbyte)0xE7), unchecked((sbyte)0xE8), unchecked((sbyte)0xE9), unchecked((sbyte)0xBA), unchecked((sbyte)0xE0), unchecked((sbyte)0xBB), unchecked((sbyte)0xB0), 0x6D, 0x79, unchecked((sbyte)0x81), unchecked((sbyte)0x82), unchecked((sbyte)0x83), unchecked((sbyte)0x84), unchecked((sbyte)0x85), unchecked((sbyte)0x86), unchecked((sbyte)0x87), unchecked((sbyte)0x88), unchecked((sbyte)0x89), unchecked((sbyte)0x91), unchecked((sbyte)0x92), unchecked((sbyte)0x93), unchecked((sbyte)0x94), unchecked((sbyte)0x95), unchecked((sbyte)0x96), unchecked((sbyte)0x97), unchecked((sbyte)0x98), unchecked((sbyte)0x99), unchecked((sbyte)0xA2), unchecked((sbyte)0xA3), unchecked((sbyte)0xA4), unchecked((sbyte)0xA5), unchecked((sbyte)0xA6), unchecked((sbyte)0xA7), unchecked((sbyte)0xA8), unchecked((sbyte)0xA9), unchecked((sbyte)0xC0), 0x4F, unchecked((sbyte)0xD0), unchecked((sbyte)0xA1), 0x07, 0x20, 0x21, 0x22, 0x23, 0x24, 0x15, 0x06, 0x17, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x09, 0x0A, 0x1B, 0x30, 0x31, 0x1A, 0x33, 0x34, 0x35, 0x36, 0x08, 0x38, 0x39, 0x3A, 0x3B, 0x04, 0x14, 0x3E, unchecked((sbyte)0xFF), 0x41, unchecked((sbyte)0xAA), 0x4A, unchecked((sbyte)0xB1), unchecked((sbyte)0x9F), unchecked((sbyte)0xB2), 0x6A, unchecked((sbyte)0xB5), unchecked((sbyte)0xBD), unchecked((sbyte)0xB4), unchecked((sbyte)0x9A), unchecked((sbyte)0x8A), 0x5F, unchecked((sbyte)0xCA), unchecked((sbyte)0xAF), unchecked((sbyte)0xBC), unchecked((sbyte)0x90), unchecked((sbyte)0x8F), unchecked((sbyte)0xEA), unchecked((sbyte)0xFA), unchecked((sbyte)0xBE), unchecked((sbyte)0xA0), unchecked((sbyte)0xB6), unchecked((sbyte)0xB3), unchecked((sbyte)0x9D), unchecked((sbyte)0xDA), unchecked((sbyte)0x9B), unchecked((sbyte)0x8B), unchecked((sbyte)0xB7), unchecked((sbyte)0xB8), unchecked((sbyte)0xB9), unchecked((sbyte)0xAB), 0x64, 0x65, 0x62, 0x66, 0x63, 0x67, unchecked((sbyte)0x9E), 0x68, 0x74, 0x71, 0x72, 0x73, 0x78, 0x75, 0x76, 0x77, unchecked((sbyte)0xAC), 0x69, unchecked((sbyte)0xED), unchecked((sbyte)0xEE), unchecked((sbyte)0xEB), unchecked((sbyte)0xEF), unchecked((sbyte)0xEC), unchecked((sbyte)0xBF), unchecked((sbyte)0x80), unchecked((sbyte)0xFD), unchecked((sbyte)0xFE), unchecked((sbyte)0xFB), unchecked((sbyte)0xFC), unchecked((sbyte)0xAD), unchecked((sbyte)0xAE), 0x59, 0x44, 0x45, 0x42, 0x46, 0x43, 0x47, unchecked((sbyte)0x9C), 0x48, 0x54, 0x51, 0x52, 0x53, 0x58, 0x55, 0x56, 0x57, unchecked((sbyte)0x8C), 0x49, unchecked((sbyte)0xCD), unchecked((sbyte)0xCE), unchecked((sbyte)0xCB), unchecked((sbyte)0xCF), unchecked((sbyte)0xCC), unchecked((sbyte)0xE1), 0x70, unchecked((sbyte)0xDD), unchecked((sbyte)0xDE), unchecked((sbyte)0xDB), unchecked((sbyte)0xDC), unchecked((sbyte)0x8D), unchecked((sbyte)0x8E), unchecked((sbyte)0xDF)};

	  // The array offset is the EBCDIC 37 character value, the array value is the Unicode value.
	  // e.g. CONV_FROM_37[0xF0] == '0'  and  CONV_FROM_37[0x40] == ' '
	  private static readonly char[] CONV_FROM_37 = new char[] {(char)0x0000, (char)0x0001, (char)0x0002, (char)0x0003, (char)0x009C, (char)0x0009, (char)0x0086, (char)0x007F, (char)0x0097, (char)0x008D, (char)0x008E, (char)0x000B, (char)0x000C, (char)0x000D, (char)0x000E, (char)0x000F, (char)0x0010, (char)0x0011, (char)0x0012, (char)0x0013, (char)0x009D, (char)0x0085, (char)0x0008, (char)0x0087, (char)0x0018, (char)0x0019, (char)0x0092, (char)0x008F, (char)0x001C, (char)0x001D, (char)0x001E, (char)0x001F, (char)0x0080, (char)0x0081, (char)0x0082, (char)0x0083, (char)0x0084, (char)0x000A, (char)0x0017, (char)0x001B, (char)0x0088, (char)0x0089, (char)0x008A, (char)0x008B, (char)0x008C, (char)0x0005, (char)0x0006, (char)0x0007, (char)0x0090, (char)0x0091, (char)0x0016, (char)0x0093, (char)0x0094, (char)0x0095, (char)0x0096, (char)0x0004, (char)0x0098, (char)0x0099, (char)0x009A, (char)0x009B, (char)0x0014, (char)0x0015, (char)0x009E, (char)0x001A, (char)0x0020, (char)0x00A0, (char)0x00E2, (char)0x00E4, (char)0x00E0, (char)0x00E1, (char)0x00E3, (char)0x00E5, (char)0x00E7, (char)0x00F1, (char)0x00A2, (char)0x002E, (char)0x003C, (char)0x0028, (char)0x002B, (char)0x007C, (char)0x0026, (char)0x00E9, (char)0x00EA, (char)0x00EB, (char)0x00E8, (char)0x00ED, (char)0x00EE, (char)0x00EF, (char)0x00EC, (char)0x00DF, (char)0x0021, (char)0x0024, (char)0x002A, (char)0x0029, (char)0x003B, (char)0x00AC, (char)0x002D, (char)0x002F, (char)0x00C2, (char)0x00C4, (char)0x00C0, (char)0x00C1, (char)0x00C3, (char)0x00C5, (char)0x00C7, (char)0x00D1, (char)0x00A6, (char)0x002C, (char)0x0025, (char)0x005F, (char)0x003E, (char)0x003F, (char)0x00F8, (char)0x00C9, (char)0x00CA, (char)0x00CB, (char)0x00C8, (char)0x00CD, (char)0x00CE, (char)0x00CF, (char)0x00CC, (char)0x0060, (char)0x003A, (char)0x0023, (char)0x0040, (char)0x0027, (char)0x003D, (char)0x0022, (char)0x00D8, (char)0x0061, (char)0x0062, (char)0x0063, (char)0x0064, (char)0x0065, (char)0x0066, (char)0x0067, (char)0x0068, (char)0x0069, (char)0x00AB, (char)0x00BB, (char)0x00F0, (char)0x00FD, (char)0x00FE, (char)0x00B1, (char)0x00B0, (char)0x006A, (char)0x006B, (char)0x006C, (char)0x006D, (char)0x006E, (char)0x006F, (char)0x0070, (char)0x0071, (char)0x0072, (char)0x00AA, (char)0x00BA, (char)0x00E6, (char)0x00B8, (char)0x00C6, (char)0x00A4, (char)0x00B5, (char)0x007E, (char)0x0073, (char)0x0074, (char)0x0075, (char)0x0076, (char)0x0077, (char)0x0078, (char)0x0079, (char)0x007A, (char)0x00A1, (char)0x00BF, (char)0x00D0, (char)0x00DD, (char)0x00DE, (char)0x00AE, (char)0x005E, (char)0x00A3, (char)0x00A5, (char)0x00B7, (char)0x00A9, (char)0x00A7, (char)0x00B6, (char)0x00BC, (char)0x00BD, (char)0x00BE, (char)0x005B, (char)0x005D, (char)0x00AF, (char)0x00A8, (char)0x00B4, (char)0x00D7, (char)0x007B, (char)0x0041, (char)0x0042, (char)0x0043, (char)0x0044, (char)0x0045, (char)0x0046, (char)0x0047, (char)0x0048, (char)0x0049, (char)0x00AD, (char)0x00F4, (char)0x00F6, (char)0x00F2, (char)0x00F3, (char)0x00F5, (char)0x007D, (char)0x004A, (char)0x004B, (char)0x004C, (char)0x004D, (char)0x004E, (char)0x004F, (char)0x0050, (char)0x0051, (char)0x0052, (char)0x00B9, (char)0x00FB, (char)0x00FC, (char)0x00F9, (char)0x00FA, (char)0x00FF, (char)0x005C, (char)0x00F7, (char)0x0053, (char)0x0054, (char)0x0055, (char)0x0056, (char)0x0057, (char)0x0058, (char)0x0059, (char)0x005A, (char)0x00B2, (char)0x00D4, (char)0x00D6, (char)0x00D2, (char)0x00D3, (char)0x00D5, (char)0x0030, (char)0x0031, (char)0x0032, (char)0x0033, (char)0x0034, (char)0x0035, (char)0x0036, (char)0x0037, (char)0x0038, (char)0x0039, (char)0x00B3, (char)0x00DB, (char)0x00DC, (char)0x00D9, (char)0x00DA, (char)0x009F};

	  private static readonly string[] CACHE_FROM_37 = new string[256];
	  private static readonly bool cacheFrom37Init_;


	  private Conv()
	  {
	  }


	  /// <summary>
	  /// Converts the specified bytes into their hexadecimal String representation.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String bytesToHexString(final byte[] data, final int offset, final int length)
	  public static string bytesToHexString(sbyte[] data, int offset, int length)
	  {
		return bytesToHexString(data, offset, length, new char[length * 2]);
	  }

	  /// <summary>
	  /// Converts the specified bytes into their hexadecimal String representation.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String bytesToHexString(final byte[] data, final int offset, final int length, final char[] buffer)
	  public static string bytesToHexString(sbyte[] data, int offset, int length, char[] buffer)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numChars = length*2;
		int numChars = length * 2;
		int count = numChars;
		for (int i = offset + length - 1; i >= offset; --i)
		{
		  int low = data[i] & 0x000F;
		  int high = (data[i] >> 4) & 0x000F;
		  buffer[--count] = NUM[low];
		  buffer[--count] = NUM[high];
		}
		return new string(buffer, 0, numChars);
	  }

	  /// <summary>
	  /// Converts the specified hexadecimal String into its constituent byte values.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] hexStringToBytes(final String value)
	  public static sbyte[] hexStringToBytes(string value)
	  {
		int len = value.Length;
		/* this comparison works with negative numbers */
		if (len % 2 != 0)
		{
			++len;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] data = new byte[len>>1];
		sbyte[] data = new sbyte[len >> 1];
		hexStringToBytes(value, data, 0);
		return data;
	  }

	  /// <summary>
	  /// Converts the specified hexadecimal String into its constituent byte values.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int hexStringToBytes(final String value, final byte[] data, final int offset)
	  public static int hexStringToBytes(string value, sbyte[] data, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = value.length();
		int len = value.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int odd = len % 2;
		int odd = len % 2;
		if (odd == 1)
		{
		  data[offset] = 0;
		}
		for (int i = 0; i < len; ++i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char c = value.charAt(i);
		  char c = value[i];
		  int val = 0;
		  if (c >= '0' && c <= '9')
		  {
			val = c - '0';
		  }
		  else if (c >= 'A' && c <= 'F')
		  {
			val = c - 'A' + 10;
		  }
		  else if (c >= 'a' && c <= 'f')
		  {
			val = c - 'a' + 10;
		  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int arrOff = offset + ((i+odd)>>1);
		  int arrOff = offset + ((i + odd) >> 1);
		  data[arrOff] = i % 2 == odd ? (sbyte)(val << 4) : (sbyte)(data[arrOff] | val);
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int num = len >> 1;
		int num = len >> 1;
		return odd == 0 ? num : num + 1;
	  }


	  /// <summary>
	  /// Converts the specified String into CCSID 37 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] stringToEBCDICByteArray37(final String s)
	  public static sbyte[] stringToEBCDICByteArray37(string s)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] b = new byte[s.length()];
		sbyte[] b = new sbyte[s.Length];
		stringToEBCDICByteArray37(s, b, 0);
		return b;
	  }

	  /// <summary>
	  /// Converts the specified String into CCSID 37 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int stringToEBCDICByteArray37(final String s, final byte[] data, final int offset)
	  public static int stringToEBCDICByteArray37(string s, sbyte[] data, int offset)
	  {
		  return stringToEBCDICByteArray37(s, s.Length, data, offset);
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int stringToEBCDICByteArray37(final String s, int length, final byte[] data, final int offset)
	  public static int stringToEBCDICByteArray37(string s, int length, sbyte[] data, int offset)
	  {
		int sLength = s.Length;
		if (sLength < length)
		{
			length = sLength;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = offset+length;
		int stop = offset + length;
		for (int i = offset; i < stop; ++i)
		{
		  data[i] = CONV_TO_37[s[i - offset]];
		}
		return length;
	  }


	  /// <summary>
	  /// Converts the specified String into the appropriate byte values for the specified CCSID. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final byte[] stringToEBCDICByteArray(final String s, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static sbyte[] stringToEBCDICByteArray(string s, int ccsid)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
			return stringToEBCDICByteArray37(s);
		}
		string encoding = encodings_[ccsidToUse];
		if (!string.ReferenceEquals(encoding, null))
		{
			try
			{
				return s.GetBytes(encoding);
			}
			catch (UnsupportedEncodingException)
			{
				encodings_[ccsidToUse] = null;
			}
		}
		return CcsidConversion.stringToEBCDICByteArray(s, ccsidToUse);

	  }

	  /// <summary>
	  /// Converts the specified String into the appropriate byte values for the specified CCSID. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final int stringToEBCDICByteArray(final String s, final byte[] data, final int offset, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static int stringToEBCDICByteArray(string s, sbyte[] data, int offset, int ccsid)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
			return stringToEBCDICByteArray37(s, data, offset);
		}
		string encoding = encodings_[ccsidToUse];
		if (!string.ReferenceEquals(encoding, null))
		{
			try
			{
		  // BOOOO!
		  sbyte[] b = s.GetBytes(encoding);
		  Array.Copy(b, 0, data, offset, b.Length);
		  return b.Length;
			}
			catch (UnsupportedEncodingException)
			{
				encodings_[ccsidToUse] = null;
			}
		}
		return CcsidConversion.stringToEBCDICByteArray(s, data, offset, ccsidToUse);

	  }

	  /// <summary>
	  /// Converts the specified String into the appropriate byte values for the specified CCSID. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final int stringToEBCDICByteArray(final String s, int length, final byte[] data, final int offset, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static int stringToEBCDICByteArray(string s, int length, sbyte[] data, int offset, int ccsid)
	  {
		  int sLength = s.Length;
		  if (length > sLength)
		  {
			  length = sLength;
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
			return stringToEBCDICByteArray37(s, length, data, offset);
		}
		string encoding = encodings_[ccsidToUse];
		if (!string.ReferenceEquals(encoding, null))
		{
			try
			{
				  // BOOOO!

		  sbyte[] b = s.Substring(0,length).GetBytes(encoding);
		  Array.Copy(b, 0, data, offset, b.Length);
		  return b.Length;
			}
			catch (UnsupportedEncodingException)
			{
				encodings_[ccsidToUse] = null;
			}
		}
		return CcsidConversion.stringToEBCDICByteArray(s, length, data, offset, ccsidToUse);

	  }


	  /// <summary>
	  /// Converts the specified String into Unicode bytes.
	  /// returns the number of bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] stringToUnicodeByteArray(final String s)
	  public static sbyte[] stringToUnicodeByteArray(string s)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] b = new byte[s.length()*2];
		sbyte[] b = new sbyte[s.Length * 2];
		stringToUnicodeByteArray(s, b, 0);
		return b;
	  }

	  /// <summary>
	  /// Converts the specified String into Unicode bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int stringToUnicodeByteArray(final String s, final byte[] data, final int offset)
	  public static int stringToUnicodeByteArray(string s, sbyte[] data, int offset)
	  {
		  return stringToUnicodeByteArray(s, s.Length, data, offset);
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int stringToUnicodeByteArray(final String s, int length, final byte[] data, final int offset)
	  public static int stringToUnicodeByteArray(string s, int length, sbyte[] data, int offset)
	  {
		for (int i = 0; i < length; ++i)
		{
		  char c = s[i];
		  sbyte high = (sbyte)(c >> 8);
		  sbyte low = (sbyte)c;
		  data[offset + (i * 2)] = high;
		  data[offset + (i * 2) + 1] = low;
		}
		return length * 2;
	  }

	  /* Version that pads with spaces */
	  public static void stringToUnicodeByteArray(string s, sbyte[] data, int offset, int byteLength)
	  {
		int sLength = s.Length;

		for (int i = 0; i < byteLength / 2; ++i)
		{
		  char c;
		  if (i < sLength)
		  {
		   c = s[i];
		  }
		  else
		  {
			c = ' ';
		  }
		  sbyte high = (sbyte)(c >> 8);
		  sbyte low = (sbyte)c;
		  data[offset + (i * 2)] = high;
		  data[offset + (i * 2) + 1] = low;
		}

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int stringToUtf8ByteArray(final String s, int length, final byte[] data, final int offset)
	  public static int stringToUtf8ByteArray(string s, int length, sbyte[] data, int offset)
	  {
		  int sLength = s.Length;
		  if (length > sLength)
		  {
			  length = sLength;
		  }

				  // BOOOO!

		  sbyte[] b;
		  try
		  {
		  b = s.Substring(0,length).GetBytes(Encoding.UTF8);
		  Array.Copy(b, 0, data, offset, b.Length);
		  return b.Length;
		  }
		  catch (UnsupportedEncodingException)
		  {
			  // should never happen
			  return 0;
		  }


	  }



	  /// <summary>
	  /// Converts the specified String into Unicode bytes, padding the byte array with Unicode
	  /// spaces (0x0020) up to <i>length</i> bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void stringToBlankPadUnicodeByteArray(final String s, final byte[] data, final int offset, final int length)
	  public static void stringToBlankPadUnicodeByteArray(string s, sbyte[] data, int offset, int length)
	  {
		int counter = 0;
		if (!string.ReferenceEquals(s, null))
		{
		  for (int i = 0; i < s.Length && (counter + 2) <= length; ++i)
		  {
			sbyte high = (sbyte)(s[i] >> 8);
			sbyte low = (sbyte)s[i];
			data[offset + counter] = high;
			++counter;
			data[offset + counter] = low;
			++counter;
		  }
		}
		while ((counter + 2) <= length)
		{
		  data[offset + counter] = 0x00;
		  ++counter;
		  data[offset + counter] = 0x20;
		  ++counter;
		}
	  }

	  /// <summary>
	  /// Converts the specified Unicode bytes into a String.
	  /// The length is in bytes, and should be twice the length of the returned String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String unicodeByteArrayToString(final byte[] data, final int offset, final int length)
	  public static string unicodeByteArrayToString(sbyte[] data, int offset, int length)
	  {
		char[] buf = new char[length];
		return unicodeByteArrayToString(data, offset, length, buf);
	  }

	  /// <summary>
	  /// Converts the specified Unicode bytes into a String.
	  /// The length is in bytes, and should be twice the length of the returned String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String unicodeByteArrayToString(final byte[] data, final int offset, final int length, final char[] buffer)
	  public static string unicodeByteArrayToString(sbyte[] data, int offset, int length, char[] buffer)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numChars = length/2;
		int numChars = length / 2;
		int count = numChars;
		for (int i = offset + length - 1; i >= offset; i -= 2)
		{
		  int low = data[i] & 0x00FF;
		  int high = data[i - 1] & 0x00FF;
		  char c = (char)((high << 8) | low);
		  buffer[--count] = c;
		}
		return new string(buffer, 0, numChars);
	  }

	  /// <summary>
	  /// Converts the specified String into CCSID 37 bytes, padding the byte array with EBCDIC spaces (0x40) up to <i>length</i> bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void stringToBlankPadEBCDICByteArray(final String s, final byte[] data, final int offset, final int length)
	  public static void stringToBlankPadEBCDICByteArray(string s, sbyte[] data, int offset, int length)
	  {
		for (int i = 0; i < s.Length && i < length; ++i)
		{
		  data[offset + i] = CONV_TO_37[s[i]];
		}
		for (int i = s.Length; i < length; ++i)
		{
		  data[offset + i] = 0x40;
		}
	  }

	  /// <summary>
	  /// Converts the specified String into bytes for the specified CCSID, padding the byte array with spaces up to <i>length</i> bytes. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final void stringToBlankPadEBCDICByteArray(final String s, final byte[] data, final int offset, final int length, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static void stringToBlankPadEBCDICByteArray(string s, sbyte[] data, int offset, int length, int ccsid)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
		  stringToBlankPadEBCDICByteArray(s, data, offset, length);
		}
		else
		{
		  string encoding = encodings_[ccsidToUse];
		  if (!string.ReferenceEquals(encoding, null))
		  {
			// BOOOO!
			sbyte[] b = s.GetBytes(encoding);
			int len = b.Length;
			int total = len < length ? len : length;
			Array.Copy(b, 0, data, offset, total);
			int rem = length - len;
			if (rem > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] blank = " ".getBytes(encoding);
			  sbyte[] blank = " ".GetBytes(encoding);
			  while (rem > 0)
			  {
				Array.Copy(blank, 0, data, offset + total, blank.Length);
				total += blank.Length;
				rem = rem - blank.Length;
			  }
			}
		  }
		  else
		  {
			throw new UnsupportedEncodingException("CCSID " + ccsidToUse);
		  }
		}
	  }

	  /// <summary>
	  /// Converts the specified CCSID 37 byte into a Unicode char without creating any intermediate objects.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final char ebcdicByteToChar(final byte b)
	  public static char ebcdicByteToChar(sbyte b)
	  {
		return CONV_FROM_37[b & 0x00FF];
	  }

	  /// <summary>
	  /// Converts the specified CCSID 37 bytes into a String.
	  /// Note: You might as well just use new String(data,"Cp037") to avoid the extra char array this method needs to create.
	  /// Note: You cannot use new String(data,"Cp037" because this is not supported on all JVMS
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String ebcdicByteArrayToString(final byte[] data, final int offset, final int length)
	  public static string ebcdicByteArrayToString(sbyte[] data, int offset, int length)
	  {
		if (length == 1 && cacheFrom37Init_)
		{
			return CACHE_FROM_37[data[offset] & 0x00FF];
		}
		return ebcdicByteArrayToString(data, offset, length, new char[length]);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String ebcdicByteArrayToString(final byte[] data, final char[] buffer)
	  public static string ebcdicByteArrayToString(sbyte[] data, char[] buffer)
	  {
		int offset = 0;
		int length = data.Length;
		return ebcdicByteArrayToString(data, offset, length, buffer);
	  }

	  // @csmith: Perf testing:
	  // This uses the same amount of memory as new String(data, "Cp037")
	  // but is about 2x faster on IBM 1.5 Windows 32-bit and about 3x faster on Sun 1.4 Windows 32-bit.
	  /// <summary>
	  /// Converts the specified CCSID 37 bytes into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String ebcdicByteArrayToString(final byte[] data, final int offset, final int length, final char[] buffer)
	  public static string ebcdicByteArrayToString(sbyte[] data, int offset, int length, char[] buffer)
	  {
		if (length == 1 && cacheFrom37Init_)
		{
			return CACHE_FROM_37[data[offset] & 0x00FF];
		}
		int counter = length;
		for (int i = offset + length - 1; i >= offset; --i)
		{
		  buffer[--counter] = CONV_FROM_37[data[i] & 0x00FF];
		}
		return new string(buffer, 0, length);
	  }

	  // Conversion maps copied from Toolbox/JTOpen.
	  private static readonly Hashtable encodingCcsid_ = new Hashtable();
	  private static readonly Hashtable ccsidEncoding_ = new Hashtable();

	  private static readonly string[] encodings_ = new string[65536];



	  /// <summary>
	  /// Converts the specific CCSID bytes into a String. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final String ebcdicByteArrayToString(final byte[] data, final int offset, final int length, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static string ebcdicByteArrayToString(sbyte[] data, int offset, int length, int ccsid)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
			return ebcdicByteArrayToString(data, offset, length);
		}
		string encoding = encodings_[ccsidToUse];
		if (!string.ReferenceEquals(encoding, null))
		{
		  return StringHelper.NewString(data, offset, length, encoding);
		}
		throw new UnsupportedEncodingException("CCSID " + ccsidToUse);
	  }

	  /// <summary>
	  /// Converts the specific CCSID bytes into a String. </summary>
	  /// <exception cref="UnsupportedEncodingException"> Thrown if conversion to or from the specified CCSID is not supported.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final String ebcdicByteArrayToString(final byte[] data, final int offset, final int length, final char[] buffer, final int ccsid) throws UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public static string ebcdicByteArrayToString(sbyte[] data, int offset, int length, char[] buffer, int ccsid)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ccsidToUse = ccsid & 0x00FFFF;
		int ccsidToUse = ccsid & 0x00FFFF; // So we don't overflow our encodings_ table.
		if (ccsidToUse == 37)
		{
			return ebcdicByteArrayToString(data, offset, length, buffer);
		}
		string encoding = encodings_[ccsidToUse];
		if (!string.ReferenceEquals(encoding, null))
		{
		  try
		  {
			 return StringHelper.NewString(data, offset, length, encoding);
		  }
		  catch (UnsupportedEncodingException)
		  {
			  // Mark as unsupported
			  encodings_[ccsidToUse] = null;
			  // Fall through and convert
			  encoding = null;

		  }
		}
	   return CcsidConversion.createString(data, offset, length, ccsidToUse);
	  }

	  /// <summary>
	  /// Returns true if the conversion to or from the specific CCSID is supported by the methods on this class.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean isSupported(final int ccsid)
	  public static bool isSupported(int ccsid)
	  {
		if (ccsid < 0 || ccsid > 65535)
		{
			return false;
		}
		return ccsid == 37 || !string.ReferenceEquals(encodings_[ccsid], null);
	  }

	  /// <summary>
	  /// Converts the specified bytes into a long value.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final long byteArrayToLong(final byte[] data, final int offset)
	  public static long byteArrayToLong(sbyte[] data, int offset)
	  {
		int p0 = (0x00FF & data[offset]) << 24;
		int p1 = (0x00FF & data[offset + 1]) << 16;
		int p2 = (0x00FF & data[offset + 2]) << 8;
		int p3 = 0x00FF & data[offset + 3];
		int p4 = (0x00FF & data[offset + 4]) << 24;
		int p5 = (0x00FF & data[offset + 5]) << 16;
		int p6 = (0x00FF & data[offset + 6]) << 8;
		int p7 = (0x00FF & data[offset + 7]);
		long l1 = (long)(p0 | p1 | p2 | p3);
		long l2 = (long)(p4 | p5 | p6 | p7);
		return (l1 << 32) | (l2 & 0x00FFFFFFFFL);
	  }

	  /// <summary>
	  /// Converts the specified bytes into an int value.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int byteArrayToInt(final byte[] data, final int offset)
	  public static int byteArrayToInt(sbyte[] data, int offset)
	  {
		int p0 = (0x00FF & data[offset]) << 24;
		int p1 = (0x00FF & data[offset + 1]) << 16;
		int p2 = (0x00FF & data[offset + 2]) << 8;
		int p3 = 0x00FF & data[offset + 3];
		return p0 | p1 | p2 | p3;
	  }

	  /// <summary>
	  /// Converts the specified bytes into a short value.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final short byteArrayToShort(final byte[] data, final int offset)
	  public static short byteArrayToShort(sbyte[] data, int offset)
	  {
		short p0 = (short)((0x00FF & data[offset]) << 8);
		short p1 = (short)(0x00FF & data[offset + 1]);
		return (short)(p0 | p1);
	  }

	  /// <summary>
	  /// Converts the specified short value into 2 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void shortToByteArray(final int value, final byte[] data, final int offset)
	  public static void shortToByteArray(int value, sbyte[] data, int offset)
	  {
		data[offset] = (sbyte)(value >> 8);
		data[offset + 1] = (sbyte)value;
	  }

	  /// <summary>
	  /// Converts the specified int value into 4 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] intToByteArray(final int value)
	  public static sbyte[] intToByteArray(int value)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] val = new byte[4];
		sbyte[] val = new sbyte[4];
		intToByteArray(value, val, 0);
		return val;
	  }

	  /// <summary>
	  /// Converts the specified int value into 4 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void intToByteArray(final int value, final byte[] data, final int offset)
	  public static void intToByteArray(int value, sbyte[] data, int offset)
	  {
		data[offset] = (sbyte)(value >> 24);
		data[offset + 1] = (sbyte)(value >> 16);
		data[offset + 2] = (sbyte)(value >> 8);
		data[offset + 3] = (sbyte)value;
	  }

	  /// <summary>
	  /// Converts the specified long value into 8 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] longToByteArray(final long longValue)
	  public static sbyte[] longToByteArray(long longValue)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] val = new byte[8];
		sbyte[] val = new sbyte[8];
		longToByteArray(longValue, val, 0);
		return val;
	  }

	  // @csmith: Perf testing:
	  // Confirmed breaking the long into two int's is much faster.
	  // Using sign extension >> rather than unsigned shift >>> is faster on IBM JRE but not on Sun (Windows 32-bit).
	  /// <summary>
	  /// Converts the specified long value into 8 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void longToByteArray(final long longValue, final byte[] data, final int offset)
	  public static void longToByteArray(long longValue, sbyte[] data, int offset)
	  {
		// Do in two parts to avoid long temps.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int high = (int)(longValue >> 32);
		int high = (int)(longValue >> 32);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int low = (int)longValue;
		int low = (int)longValue;

		data[offset] = (sbyte)(high >> 24);
		data[offset + 1] = (sbyte)(high >> 16);
		data[offset + 2] = (sbyte)(high >> 8);
		data[offset + 3] = (sbyte)high;

		data[offset + 4] = (sbyte)(low >> 24);
		data[offset + 5] = (sbyte)(low >> 16);
		data[offset + 6] = (sbyte)(low >> 8);
		data[offset + 7] = (sbyte)low;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static final void writeStringToUnicodeBytes(final String s, final HostServerConnection.HostOutputStream out) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal static void writeStringToUnicodeBytes(string s, HostServerConnection.HostOutputStream @out)
	  {
		for (int i = 0; i < s.Length; ++i)
		{
		  @out.writeShort(s[i]);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static final void writePadEBCDIC(final String s, final int length, final HostServerConnection.HostOutputStream out) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal static void writePadEBCDIC(string s, int length, HostServerConnection.HostOutputStream @out)
	  {
		for (int i = 0; i < length; ++i)
		{
		  if (string.ReferenceEquals(s, null) || s.Length <= i)
		  {
			@out.write(0x40);
		  }
		  else
		  {
			@out.write(CONV_TO_37[s[i]]);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static final void writePadEBCDIC10(final String s, final HostServerConnection.HostOutputStream out) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  internal static void writePadEBCDIC10(string s, HostServerConnection.HostOutputStream @out)
	  {
		writePadEBCDIC(s, 10, @out);
	  }

	  //TODO - Replace calls to this method with calls to the one above.
	  /// <summary>
	  /// Converts the specified String into a total of ten CCSID 37 bytes, blank padding with EBCDIC 0x40 as necessary.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] blankPadEBCDIC10(final String s)
	  public static sbyte[] blankPadEBCDIC10(string s) // throws IOException
	  {
		sbyte[] blank37 = new sbyte[] {0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40};
		if (!string.ReferenceEquals(s, null))
		{
		  for (int i = 0; i < s.Length && i < 10; ++i)
		  {
			blank37[i] = CONV_TO_37[s[i]];
		  }
		}
		return blank37;
	  }

	  /// <summary>
	  /// Converts the specified bytes into a float value.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final float byteArrayToFloat(final byte[] data, final int offset)
	  public static float byteArrayToFloat(sbyte[] data, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = byteArrayToInt(data, offset);
		int i = byteArrayToInt(data, offset);
		return Float.intBitsToFloat(i);
	  }

	  /// <summary>
	  /// Converts the specified float value into 4 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void floatToByteArray(final float f, final byte[] data, final int offset)
	  public static void floatToByteArray(float f, sbyte[] data, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = Float.floatToIntBits(f);
		int i = Float.floatToIntBits(f);
		intToByteArray(i, data, offset);
	  }

	  /// <summary>
	  /// Converts the specified float value into 4 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] floatToByteArray(final float f)
	  public static sbyte[] floatToByteArray(float f)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = Float.floatToIntBits(f);
		int i = Float.floatToIntBits(f);
		return intToByteArray(i);
	  }

	  /// <summary>
	  /// Converts the specified bytes into a double value.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final double byteArrayToDouble(final byte[] data, final int offset)
	  public static double byteArrayToDouble(sbyte[] data, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long l = byteArrayToLong(data, offset);
		long l = byteArrayToLong(data, offset);
		return Double.longBitsToDouble(l);
	  }

	  /// <summary>
	  /// Converts the specified double value into 8 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void doubleToByteArray(final double d, final byte[] data, final int offset)
	  public static void doubleToByteArray(double d, sbyte[] data, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long l = Double.doubleToLongBits(d);
		long l = System.BitConverter.DoubleToInt64Bits(d);
		longToByteArray(l, data, offset);
	  }

	  /// <summary>
	  /// Converts the specified double value into 8 bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] doubleToByteArray(final double d)
	  public static sbyte[] doubleToByteArray(double d)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long l = Double.doubleToLongBits(d);
		long l = System.BitConverter.DoubleToInt64Bits(d);
		return longToByteArray(l);
	  }

	  // Copied from JTOpen.
	  private const int DEC_FLOAT_16_BIAS = 398;
	  private const long DEC_FLOAT_16_SIGNAL_MASK = 0x0200000000000000L; // 1 bit (7th bit from left) //@snan
	  private const long DEC_FLOAT_16_SIGN_MASK = unchecked((long)0x8000000000000000L); // 1 bits
	  private const long DEC_FLOAT_16_COMBINATION_MASK = 0x7c00000000000000L; // 5 bits
	  private const long DEC_FLOAT_16_EXPONENT_CONTINUATION_MASK = 0x03fc000000000000L; // 8 bits
	  private const long DEC_FLOAT_16_COEFFICIENT_CONTINUATION_MASK = 0x0003ffffffffffffL; // 50 bits

	  private const int DEC_FLOAT_34_BIAS = 6176;
	  private const long DEC_FLOAT_34_SIGNAL_MASK = 0x0200000000000000L; // 1 bit (7th bit from left) //@snan
	  private const long DEC_FLOAT_34_SIGN_MASK = unchecked((long)0x8000000000000000L); // 1 bits
	  private const long DEC_FLOAT_34_COMBINATION_MASK = 0x7c00000000000000L; // 5 bits
	  private const long DEC_FLOAT_34_EXPONENT_CONTINUATION_MASK = 0x03ffc00000000000L; // 12 bits
	  // private final static long DEC_FLOAT_34_COEFFICIENT_CONTINUATION_MASK = 0x00003fffffffffffL; // 46 bits + 64 bits = 110 bits

	  private static readonly int[][] TEN_RADIX_MAGNITUDE = new int[][]
	  {
		  new int[] {0x3b9aca00},
		  new int[] {0x0de0b6b3, unchecked((int)0xa7640000)},
		  new int[] {0x033b2e3c, unchecked((int)0x9fd0803c), unchecked((int)0xe8000000)}
	  };

	  // Copied from JTOpen. TODO - Needs optimization.
	  /// <summary>
	  /// Converts the specified 8 bytes in decfloat16 format into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String decfloat16ByteArrayToString(final byte[] data, final int offset)
	  public static string decfloat16ByteArrayToString(sbyte[] data, int offset)
	  {
		long decFloat16Bits = byteArrayToLong(data, offset);
		long combination = (decFloat16Bits & DEC_FLOAT_16_COMBINATION_MASK) >> 58;

		//compute sign here so we can get -+Infinity values
		int sign = ((decFloat16Bits & DEC_FLOAT_16_SIGN_MASK) == DEC_FLOAT_16_SIGN_MASK) ? -1 : 1;

		// deal with special numbers. (not a number and infinity)
		if ((combination == 0x1fL) && (sign == 1))
		{
		  long nanSignal = (decFloat16Bits & DEC_FLOAT_16_SIGNAL_MASK) >> 57; //shift first 7 bits to get signal bit out  //@snan
		  return nanSignal == 1 ? "SNaN" : "NaN";
		}
		else if ((combination == 0x1fL) && (sign == -1))
		{
		  long nanSignal = (decFloat16Bits & DEC_FLOAT_16_SIGNAL_MASK) >> 57; //shift first 7 bits to get signal bit out  //@snan
		  return nanSignal == 1 ? "-SNaN" : "-NaN";
		}
		else if ((combination == 0x1eL) && (sign == 1))
		{
		  return "Infinity";
		}
		else if ((combination == 0x1eL) && (sign == -1))
		{
		  return "-Infinity";
		}

		// compute the exponent MSD and the coefficient MSD.
		int exponentMSD;
		long coefficientMSD;
		if ((combination & 0x18L) == 0x18L)
		{
		  // format of 11xxx:
		  exponentMSD = (int)((combination & 0x06L) >> 1);
		  coefficientMSD = 8 + (combination & 0x01L);
		}
		else
		{
		  // format of xxxxx:
		  exponentMSD = (int)((combination & 0x18L) >> 3);
		  coefficientMSD = (combination & 0x07L);
		}

		// compute the exponent.
		int exponent = (int)((decFloat16Bits & DEC_FLOAT_16_EXPONENT_CONTINUATION_MASK) >> 50);
		exponent |= (exponentMSD << 8);
		exponent -= DEC_FLOAT_16_BIAS;

		// compute the coefficient.
		long coefficientContinuation = decFloat16Bits & DEC_FLOAT_16_COEFFICIENT_CONTINUATION_MASK;
		int coefficientLo = decFloatBitsToDigits((int)(coefficientContinuation & 0x3fffffff)); // low 30 bits (9 digits)
		int coefficientHi = decFloatBitsToDigits((int)((coefficientContinuation >> 30) & 0xfffff)); // high 20 bits (6 digits)
		coefficientHi += (int)(coefficientMSD * 1000000L);

		// compute the int array of coefficient.
		int[] value = computeMagnitude(new int[] {coefficientHi, coefficientLo});

		// convert value to a byte array of coefficient.
		sbyte[] magnitude = new sbyte[8];
		magnitude[0] = (sbyte)((int)((uint)value[0] >> 24));
		magnitude[1] = (sbyte)((int)((uint)value[0] >> 16));
		magnitude[2] = (sbyte)((int)((uint)value[0] >> 8));
		magnitude[3] = (sbyte)(value[0]);
		magnitude[4] = (sbyte)((int)((uint)value[1] >> 24));
		magnitude[5] = (sbyte)((int)((uint)value[1] >> 16));
		magnitude[6] = (sbyte)((int)((uint)value[1] >> 8));
		magnitude[7] = (sbyte)(value[1]);

		System.Numerics.BigInteger bigInt = new System.Numerics.BigInteger(sign, magnitude);
		decimal bigDec = new decimal(bigInt, -exponent);
		return bigDec.ToString();
	  }

	  // Copied from JTOpen. TODO - Needs optimization.
	  /// <summary>
	  /// Converts the specified 16 bytes in decfloat34 format into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String decfloat34ByteArrayToString(final byte[] data, final int offset)
	  public static string decfloat34ByteArrayToString(sbyte[] data, int offset)
	  {
		long decFloat34BitsHi = byteArrayToLong(data, offset);
		long decFloat34BitsLo = byteArrayToLong(data, offset + 8);
		long combination = (decFloat34BitsHi & DEC_FLOAT_34_COMBINATION_MASK) >> 58;

		//compute sign.
		int sign = ((decFloat34BitsHi & DEC_FLOAT_34_SIGN_MASK) == DEC_FLOAT_34_SIGN_MASK) ? -1 : 1;

		// deal with special numbers.
		if ((combination == 0x1fL) && (sign == 1))
		{
		  long nanSignal = (decFloat34BitsHi & DEC_FLOAT_34_SIGNAL_MASK) >> 57; //shift first 7 bits to get signal bit out  //@snan
		  return nanSignal == 1 ? "SNaN" : "NaN";
		}
		else if ((combination == 0x1fL) && (sign == -1))
		{
		  long nanSignal = (decFloat34BitsHi & DEC_FLOAT_34_SIGNAL_MASK) >> 57; //shift first 7 bits to get signal bit out  //@snan
		  return nanSignal == 1 ? "-SNaN" : "-NaN";
		}
		else if ((combination == 0x1eL) && (sign == 1))
		{
		  return "Infinity";
		}
		else if ((combination == 0x1eL) && (sign == -1))
		{
		  return "-Infinity";
		}

		// compute the exponent MSD and the coefficient MSD.
		int exponentMSD;
		long coefficientMSD;
		if ((combination & 0x18L) == 0x18L)
		{
		  // format of 11xxx:
		  exponentMSD = (int)((combination & 0x06L) >> 1);
		  coefficientMSD = 8 + (combination & 0x01L);
		}
		else
		{
		  // format of xxxxx:
		  exponentMSD = (int)((combination & 0x18L) >> 3);
		  coefficientMSD = (combination & 0x07L);
		}

		// compute the exponent.
		int exponent = (int)((decFloat34BitsHi & DEC_FLOAT_34_EXPONENT_CONTINUATION_MASK) >> 46);
		exponent |= (exponentMSD << 12);
		exponent -= DEC_FLOAT_34_BIAS;

		// compute the coefficient.
		int coefficientLo = decFloatBitsToDigits((int)(decFloat34BitsLo & 0x3fffffff)); // last 30 bits (9 digits)
		// another 30 bits (9 digits)
		int coefficientMeLo = decFloatBitsToDigits((int)((decFloat34BitsLo >> 30) & 0x3fffffff));
		// another 30 bits (9 digits). 26 bits from hi and 4 bits from lo.
		int coefficientMeHi = decFloatBitsToDigits((int)(((decFloat34BitsHi & 0x3ffffff) << 4) | ((decFloat34BitsLo >> 60) & 0xf)));
		int coefficientHi = decFloatBitsToDigits((int)((decFloat34BitsHi >> 26) & 0xfffff)); // high 20 bits (6 digits)
		coefficientHi += (int)(coefficientMSD * 1000000L);

		// compute the int array of coefficient.
		int[] value = computeMagnitude(new int[] {coefficientHi, coefficientMeHi, coefficientMeLo, coefficientLo});

		// convert value to a byte array of coefficient.
		sbyte[] magnitude = new sbyte[16];
		magnitude[0] = (sbyte)((int)((uint)value[0] >> 24));
		magnitude[1] = (sbyte)((int)((uint)value[0] >> 16));
		magnitude[2] = (sbyte)((int)((uint)value[0] >> 8));
		magnitude[3] = (sbyte)(value[0]);
		magnitude[4] = (sbyte)((int)((uint)value[1] >> 24));
		magnitude[5] = (sbyte)((int)((uint)value[1] >> 16));
		magnitude[6] = (sbyte)((int)((uint)value[1] >> 8));
		magnitude[7] = (sbyte)(value[1]);
		magnitude[8] = (sbyte)((int)((uint)value[2] >> 24));
		magnitude[9] = (sbyte)((int)((uint)value[2] >> 16));
		magnitude[10] = (sbyte)((int)((uint)value[2] >> 8));
		magnitude[11] = (sbyte)(value[2]);
		magnitude[12] = (sbyte)((int)((uint)value[3] >> 24));
		magnitude[13] = (sbyte)((int)((uint)value[3] >> 16));
		magnitude[14] = (sbyte)((int)((uint)value[3] >> 8));
		magnitude[15] = (sbyte)(value[3]);

		System.Numerics.BigInteger bigInt = new System.Numerics.BigInteger(sign, magnitude);
		decimal bigDec = new decimal(bigInt, -exponent);
		return bigDec.ToString();
	  }

	  // Copied from JTOpen. TODO - Needs optimization.
	  // Compute the int array of magnitude from input value segments.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final int[] computeMagnitude(final int[] input)
	  private static int[] computeMagnitude(int[] input)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = input.length;
		int length = input.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] mag = new int[length];
		int[] mag = new int[length];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = length-1;
		int stop = length - 1;
		mag[stop] = input[stop];
		for (int i = 0; i < stop; ++i)
		{
		  int carry = 0;
		  int j = TEN_RADIX_MAGNITUDE[i].Length - 1;
		  int k = length - 1;
		  for (; j >= 0; --j, --k)
		  {
			long product = (input[length - 2 - i] & 0xFFFFFFFFL) * (TEN_RADIX_MAGNITUDE[i][j] & 0xFFFFFFFFL) + (mag[k] & 0xFFFFFFFFL) + (carry & 0xFFFFFFFFL); // add carry
			carry = (int)((long)((ulong)product >> 32));
			mag[k] = unchecked((int)(product & 0xFFFFFFFFL));
		  }
		  mag[k] = (int) carry;
		}
		return mag;
	  }

	  // Copied from JTOpen. TODO - Needs optimization.
	  // Convert 30 binary bits coefficient to 9 decimal digits.
	  private static int decFloatBitsToDigits(int bits)
	  {
		int @decimal = 0;
		for (int i = 2; i >= 0; --i)
		{
		  @decimal *= 1000;
		  @decimal += unpackDenselyPackedDecimal((int)((bits >> (i * 10)) & 0x03ffL));
		}
		return @decimal;
	  }

	  // Copied from JTOpen. TODO - Needs optimization.
	  // Internal declet decoding helper method.
	  private static int unpackDenselyPackedDecimal(int bits)
	  {
		//Declet is the three bit encoding of one decimal digit.  The Decfloat is made up of declets to represent
		//the decfloat 16 or 34 digits
		int combination;
		if ((bits & 14) == 14)
		{
		  combination = ((bits & 96) >> 5) | 4;
		}
		else
		{
		  combination = ((bits & 8) == 8) ? (((~bits) & 6) >> 1) : 0;
		}
		int decoded = 0;
		switch (combination)
		{
		  case 0: // bit 6 is 0
			decoded = ((bits & 896) << 1) | (bits & 119);
			break;
		  case 1: // bits 6,7,8 are 1-1-0
			decoded = ((bits & 128) << 1) | (bits & 113) | ((bits & 768) >> 7) | 2048;
			break;
		  case 2: // bits 6,7,8 are 1-0-1
			decoded = ((bits & 896) << 1) | (bits & 17) | ((bits & 96) >> 4) | 128;
			break;
		  case 3: // bits 6,7,8 are 1-0-0
			decoded = ((bits & 896) << 1) | (bits & 113) | 8;
			break;
		  case 4: // bits 6,7,8 are 1-1-1, bits 3,4 are 0-0
			decoded = ((bits & 128) << 1) | (bits & 17) | ((bits & 768) >> 7) | 2176;
			break;
		  case 5: // bits 6,7,8 are 1-1-1, bits 3,4 are 0-1
			decoded = ((bits & 128) << 1) | (bits & 17) | ((bits & 768) >> 3) | 2056;
			break;
		  case 6: // bits 6,7,8 are 1-1-1, bits 3,4 are 1-0
			decoded = ((bits & 896) << 1) | (bits & 17) | 136;
			break;
		  case 7: // bits 6,7,8 are 1-1-1, bits 3,4 are 1-1
			// NB: we ignore values of bits 0,1 in this case
			decoded = ((bits & 128) << 1) | (bits & 17) | 2184;
			break;
		}
		return ((decoded & 3840) >> 8) * 100 + ((decoded & 240) >> 4) * 10 + (decoded & 15);
	  }

	  /// <summary>
	  /// Converts the specified packed decimal bytes into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String packedDecimalToString(final byte[] data, final int offset, final int numDigits, final int scale)
	  public static string packedDecimalToString(sbyte[] data, int offset, int numDigits, int scale)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = numDigits/2+1;
		int len = numDigits / 2 + 1;
		int sign = data[offset + len - 1] & 0x0F;
		bool isNegative = sign == 0x0B || sign == 0x0D;
		char[] buf = new char[numDigits + (scale > 0 ? 1 : 0) + (isNegative ? 1 : 0)];
		return packedDecimalToString(data, offset, numDigits, scale, buf);
	  }

	  /// <summary>
	  /// Converts the specified packed decimal bytes into a String.
	  /// The number of bytes used from <i>data</i> is equal to <i>numDigits</i>/2+1.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String packedDecimalToString(final byte[] data, int offset, int numDigits, final int scale, final char[] buffer)
	  public static string packedDecimalToString(sbyte[] data, int offset, int numDigits, int scale, char[] buffer)
	  {

		// even number of digits will have a leading zero
		if (numDigits % 2 == 0)
		{
			++numDigits;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = numDigits/2+1;
		int len = numDigits / 2 + 1;

		int sign = data[offset + len - 1] & 0x0F;
		bool isNegative = sign == 0x0B || sign == 0x0D;
		int count = 0;
		if (isNegative)
		{
		  buffer[count++] = '-';
		}
		// boolean doHigh = numDigits % 2 == 1;
		// FindBugs says:  The code uses x % 2 == 1 to check to see if a value is odd, but this won't work for negative numbers (e.g., (-5) % 2 == -1).
		// If this code is intending to check for oddness, consider using x & 1 == 1, or x % 2 != 0.
		bool doHigh = numDigits % 2 != 0;

		int digitsBeforeDecimal = numDigits - scale;
		bool foundNonZero = false;
		for (int i = 0; i < digitsBeforeDecimal; ++i)
		{
		  int nibble = (doHigh ? (data[offset] >> 4) : data[offset]) & 0x0F;
		  if (foundNonZero || nibble != 0)
		  {
			buffer[count++] = NUM[nibble];
			foundNonZero = true;
		  }
		  if (!doHigh)
		  {
			doHigh = true;
			++offset;
		  }
		  else
		  {
			doHigh = false;
		  }
		}
		if (count == 0 || (isNegative && count == 1))
		{
		  buffer[count++] = '0';
		}
		if (scale > 0)
		{
		  buffer[count++] = '.';
		}
		for (int i = digitsBeforeDecimal; i < numDigits; ++i)
		{
		  int nibble = (doHigh ? (data[offset] >> 4) : data[offset]) & 0x0F;
		  buffer[count++] = NUM[nibble];
		  if (!doHigh)
		  {
			doHigh = true;
			++offset;
		  }
		  else
		  {
			doHigh = false;
		  }
		}
		return new string(buffer, 0, count);
	  }

	  // Copied from JTOpen AS400PackedDecimal.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final double packedDecimalToDouble(final byte[] data, final int offset, final int numDigits, final int scale)
	  public static double packedDecimalToDouble(sbyte[] data, int offset, int numDigits, int scale)
	  {
		// Compute the value.
		double doubleValue = 0;
		double multiplier = Math.Pow(10, -scale);
		int rightMostOffset = offset + numDigits / 2;
		bool nibble = true; // true for left nibble, false for right nibble.
		for (int i = rightMostOffset; i >= offset;)
		{
		  if (nibble)
		  {
			doubleValue += (sbyte)((data[i] & 0x00F0) >> 4) * multiplier;
			--i;
		  }
		  else
		  {
			doubleValue += ((sbyte)(data[i] & 0x000F)) * multiplier;
		  }

		  multiplier *= 10;
		  nibble = !nibble;
		}

		// Determine the sign.
		switch (data[rightMostOffset] & 0x000F)
		{
		  case 0x000B:
		  case 0x000D:
			// Negative.
			doubleValue *= -1;
			break;
		  case 0x000A:
		  case 0x000C:
		  case 0x000E:
		  case 0x000F:
			// Positive.
			break;
		  default:
			throw new System.FormatException("Byte sequence not valid for packed decimal (" + rightMostOffset + ": " + (data[rightMostOffset] & 0x000F) + ").");
		}

		return doubleValue;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] doubleToPackedDecimal(final double d, final int numDigits, final int scale)
	  public static sbyte[] doubleToPackedDecimal(double d, int numDigits, int scale)
	  {
		sbyte[] data = new sbyte[numDigits / 2 + 1];
		doubleToPackedDecimal(d, data, 0, numDigits, scale);
		return data;
	  }

	  // Copied from JTOpen AS400PackedDecimal.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void doubleToPackedDecimal(final double d, final byte[] data, final int offset, final int numDigits, final int scale)
	  public static void doubleToPackedDecimal(double d, sbyte[] data, int offset, int numDigits, int scale)
	  {
		// GOAL:  For performance reasons, we need to do this conversion
		//        without creating any Java objects (e.g., BigDecimals,
		//        Strings).

		// If the number is too big, we can't do anything with it.
		double absValue = Math.Abs(d);
		if (absValue > long.MaxValue)
		{
		  throw new System.FormatException("Double value is too big: " + d);
		}

		// Extract the normalized value.  This is the value represented by
		// two longs (one for each side of the decimal point).  Using longs
		// here improves the quality of the algorithm as well as the
		// performance of arithmetic operations.  We may need to use an
		// "effective" scale due to the lack of precision representable
		// by a long.
		long leftSide = (long)absValue;
		int effectiveScale = (scale > 15) ? 15 : scale;
		long rightSide = (long)(long)Math.Round((absValue - (double)leftSide) * Math.Pow(10, effectiveScale), MidpointRounding.AwayFromZero);

		// Ok, now we are done with any double arithmetic!
		int length = numDigits / 2;
		int b = offset + length;
		bool nibble = true; // true for left nibble, false for right nibble.

		// If the effective scale is different than the actual scale,
		// then pad with zeros.
		int scaleDifference = scale - effectiveScale;
		for (int i = 1; i <= scaleDifference; ++i)
		{
		  if (nibble)
		  {
			data[b] &= (sbyte)(0x000F);
			--b;
		  }
		  else
		  {
			data[b] &= unchecked((sbyte)(0x00F0));
		  }
		  nibble = !nibble;
		}

		// Compute the bytes for the right side of the decimal point.
		int nextDigit;
		for (int i = 1; i <= effectiveScale; ++i)
		{
		  nextDigit = (int)(rightSide % 10);
		  if (nibble)
		  {
			data[b] &= (sbyte)(0x000F);
			data[b] |= (sbyte)((sbyte)nextDigit << 4);
			--b;
		  }
		  else
		  {
			data[b] &= unchecked((sbyte)(0x00F0));
			data[b] |= (sbyte)nextDigit;
		  }
		  nibble = !nibble;
		  rightSide /= 10;
		}

		// Compute the bytes for the left side of the decimal point.
		int leftSideDigits = numDigits - scale;
		for (int i = 1; i <= leftSideDigits; ++i)
		{
		  nextDigit = (int)(leftSide % 10);
		  if (nibble)
		  {
			data[b] &= (sbyte)(0x000F);
			data[b] |= (sbyte)((sbyte)nextDigit << 4);
			--b;
		  }
		  else
		  {
			data[b] &= unchecked((sbyte)(0x00F0));
			data[b] |= (sbyte)nextDigit;
		  }
		  nibble = !nibble;
		  leftSide /= 10;
		}

		// Zero out the left part of the value, if needed.
		while (b >= offset)
		{
		  if (nibble)
		  {
			data[b] &= (sbyte)(0x000F);
			--b;
		  }
		  else
		  {
			data[b] &= unchecked((sbyte)(0x00F0));
		  }
		  nibble = !nibble;
		}

		// Fix the sign.
		b = offset + length;
		data[b] &= unchecked((sbyte)(0x00F0));
		data[b] |= (sbyte)((d >= 0) ? 0x000F : 0x000D);

		// If left side still has digits, then the value was too big
		// to fit.
		if (leftSide > 0)
		{
		  throw new System.FormatException("Double value " + d + " too big for output array.");
		}
	  }

	  /// <summary>
	  /// Converts the specified String (number) into packed decimal bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] stringToPackedDecimal(final String s, final int numDigits)
	  public static sbyte[] stringToPackedDecimal(string s, int numDigits)
	  {
		sbyte[] b = new sbyte[numDigits / 2 + 1];
		stringToPackedDecimal(s, numDigits, b, 0);
		return b;
	  }

	  /// <summary>
	  /// Converts the specified String (number) into packed decimal bytes.
	  /// The string must have the correct number of decimal digits for
	  /// the conversion to be correct.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void stringToPackedDecimal(final String s, final int numDigits, final byte[] buffer, final int offset)
	  public static void stringToPackedDecimal(string s, int numDigits, sbyte[] buffer, int offset)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = numDigits/2+1;
		int len = numDigits / 2 + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isNegative = s != null && s.length() > 0 && s.charAt(0) == '-';
		bool isNegative = !string.ReferenceEquals(s, null) && s.Length > 0 && s[0] == '-';
		int counter = offset + len - 1;
		buffer[counter] = isNegative ? (sbyte)0x0D : (sbyte)0x0F;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = isNegative ? 1 : 0;
		int stop = isNegative ? 1 : 0;
		bool doHigh = true;
		if (!string.ReferenceEquals(s, null))
		{

		for (int i = s.Length - 1; i >= stop; --i)
		{
		  char c = s[i];
		  if (c != '.')
		  {
			  int index = (int)c - '0';
			  if (index < 0 || index > 9)
			  {
				  throw new System.FormatException("Invalid character " + c);
			  }
			if (doHigh)
			{
			  buffer[counter--] |= CHAR_HIGH[index];
			  doHigh = false;
			}
			else
			{
			  buffer[counter] = CHAR_LOW[index];
			  doHigh = true;
			}
		  }
		}
		}
	  }

	  /// <summary>
	  /// Converts the specified zoned decimal bytes into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String zonedDecimalToString(final byte[] data, final int offset, final int numDigits, final int scale)
	  public static string zonedDecimalToString(sbyte[] data, int offset, int numDigits, int scale)
	  {
		int sign = (data[offset + numDigits - 1] >> 4) & 0x0F;
		bool isNegative = sign == 0x0B || sign == 0x0D;
		char[] buf = new char[numDigits + (scale > 0 ? 1 : 0) + (isNegative ? 1 : 0)];
		return zonedDecimalToString(data, offset, numDigits, scale, buf);
	  }

	  /// <summary>
	  /// Converts the specified zoned decimal bytes into a String.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final String zonedDecimalToString(final byte[] data, int offset, final int numDigits, final int scale, final char[] buffer)
	  public static string zonedDecimalToString(sbyte[] data, int offset, int numDigits, int scale, char[] buffer)
	  {
		int sign = (data[offset + numDigits - 1] >> 4) & 0x0F;
		bool isNegative = sign == 0x0B || sign == 0x0D;
		int count = 0;
		bool foundNonZero = false;
		if (isNegative)
		{
		  buffer[count++] = '-';
		}
		int digitsBeforeDecimal = numDigits - scale;
		for (int i = 0; i < digitsBeforeDecimal; ++i)
		{
		  int nibble = data[offset++] & 0x0F;
		  if (foundNonZero || nibble != 0)
		  {
			buffer[count++] = NUM[nibble];
			foundNonZero = true;
		  }
		}
		if (count == 0 || (isNegative && count == 1))
		{
		  buffer[count++] = '0';
		}
		if (scale > 0)
		{
		  buffer[count++] = '.';
		}
		for (int i = digitsBeforeDecimal; i < numDigits; ++i)
		{
		  int nibble = data[offset++] & 0x0F;
		  buffer[count++] = NUM[nibble];
		}
		return new string(buffer, 0, count);
	  }

	  /// <summary>
	  /// The scale is 0, and 0 &lt; numDigits &lt;= 20.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final long zonedDecimalToLong(final byte[] data, final int offset, final int numDigits)
	  public static long zonedDecimalToLong(sbyte[] data, int offset, int numDigits)
	  {
		long longValue = 0;
		if (numDigits <= 0)
		{
			return longValue;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rightMostOffset = offset + numDigits - 1;
		int rightMostOffset = offset + numDigits - 1;
		for (int i = offset; i <= rightMostOffset; ++i)
		{
		  longValue = longValue * 10 + (data[i] & 0x000F);
		}
		// Determine the sign.
		switch (data[rightMostOffset] & 0x00F0)
		{
		  case 0x00B0:
		  case 0x00D0:
			// Negative.
			longValue = -longValue;
			break;
		  case 0x00A0:
		  case 0x00C0:
		  case 0x00E0:
		  case 0x00F0:
			// Positive.
			break;
		  default:
			throw new System.FormatException("Byte sequence not valid for zoned decimal (" + rightMostOffset + ": " + (data[rightMostOffset] & 0x00FF) + ").");
		}
		return longValue;
	  }

	  /// <summary>
	  /// The scale is 0, and 0 &lt; numDigits &lt;= 10.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final int zonedDecimalToInt(final byte[] data, final int offset, final int numDigits)
	  public static int zonedDecimalToInt(sbyte[] data, int offset, int numDigits)
	  {
		int value = 0;
		if (numDigits <= 0)
		{
			return value;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rightMostOffset = offset + numDigits - 1;
		int rightMostOffset = offset + numDigits - 1;
		for (int i = offset; i <= rightMostOffset; ++i)
		{
		  value = value * 10 + (data[i] & 0x000F);
		}
		// Determine the sign.
		switch (data[rightMostOffset] & 0x00F0)
		{
		  case 0x00B0:
		  case 0x00D0:
			// Negative.
			value = -value;
			break;
		  case 0x00A0:
		  case 0x00C0:
		  case 0x00E0:
		  case 0x00F0:
			// Positive.
			break;
		  default:
			throw new System.FormatException("Byte sequence not valid for zoned decimal (" + rightMostOffset + ": " + (data[rightMostOffset] & 0x00FF) + ").");
		}
		return value;
	  }

	  // Copied from JTOpen AS400ZonedDecimal.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final double zonedDecimalToDouble(final byte[] data, final int offset, final int numDigits, final int scale)
	  public static double zonedDecimalToDouble(sbyte[] data, int offset, int numDigits, int scale)
	  {
		/*
		 * This old code had a bug in that it can produce
		 * inexact answers. For example
		 * 10.10105 is turned into -10.101049999999999
	
		// Compute the value.
		double doubleValue = 0;
		double multiplier = Math.pow(10, numDigits - scale - 1);
		int rightMostOffset = offset + numDigits - 1;
		for (int i = offset; i <= rightMostOffset; ++i)
		{
		  doubleValue += ((byte)(data[i] & 0x000F)) * multiplier;
		  multiplier /= 10;
		}
		*/

		/*
		 * Instead we gather the digits using a long, then / by the scale.
		 * Note:  Using a multiple by Math.pow(10, -scale) gives a worse answer.
		 * Math.pow(10,-scale) is a less accurate number than Math.pow(10,scale)
		 *
		 * A number which exposes this issue is 10.10105
		 */

		long longValue = 0;
		double doubleValue = 0;
		double divisor = Math.Pow(10, scale);
		int rightMostOffset = offset + numDigits - 1;
		for (int i = offset; i <= rightMostOffset; ++i)
		{
			longValue = longValue * 10 + (sbyte)(data[i] & 0x000F);
		}
		doubleValue = longValue / divisor;



		// Determine the sign.
		switch (data[rightMostOffset] & 0x00F0)
		{
		  case 0x00B0:
		  case 0x00D0:
			// Negative.
			doubleValue *= -1;
			break;
		  case 0x00A0:
		  case 0x00C0:
		  case 0x00E0:
		  case 0x00F0:
			// Positive.
			break;
		  default:
			throw new System.FormatException("Byte sequence not valid for zoned decimal (" + rightMostOffset + ": " + (data[rightMostOffset] & 0x00FF) + ").");
		}

		return doubleValue;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] doubleToZonedDecimal(final double d, final int numDigits, final int scale)
	  public static sbyte[] doubleToZonedDecimal(double d, int numDigits, int scale)
	  {
		sbyte[] data = new sbyte[numDigits];
		doubleToZonedDecimal(d, data, 0, numDigits, scale);
		return data;
	  }

	  // Copied from JTOpen AS400ZonedDecimal.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void doubleToZonedDecimal(final double d, final byte[] data, final int offset, final int numDigits, final int scale)
	  public static void doubleToZonedDecimal(double d, sbyte[] data, int offset, int numDigits, int scale)
	  {
		// GOAL:  For performance reasons, we need to do this conversion
		//        without creating any Java objects (e.g., BigDecimals,
		//        Strings).

		// If the number is too big, we can't do anything with it.
		double absValue = Math.Abs(d);
		if (absValue > long.MaxValue)
		{
		  throw new System.FormatException("Double value is too big: " + d);
		}

		// Extract the normalized value.  This is the value represented by
		// two longs (one for each side of the decimal point).  Using longs
		// here improves the quality of the algorithm as well as the
		// performance of arithmetic operations.  We may need to use an
		// "effective" scale due to the lack of precision representable
		// by a long.
		long leftSide = (long)absValue;
		int effectiveScale = (scale > 15) ? 15 : scale;
		long rightSide = (long)(long)Math.Round((absValue - (double)leftSide) * Math.Pow(10, effectiveScale), MidpointRounding.AwayFromZero);

		// Ok, now we are done with any double arithmetic!

		// If the effective scale is different than the actual scale,
		// then pad with zeros.
		int rightmostOffset = offset + numDigits - 1;
		int padOffset = rightmostOffset - (scale - effectiveScale);
		for (int i = rightmostOffset; i > padOffset; --i)
		{
		  data[i] = unchecked((sbyte)0x00F0);
		}

		// Compute the bytes for the right side of the decimal point.
		int decimalOffset = rightmostOffset - scale;
		int nextDigit;
		for (int i = padOffset; i > decimalOffset; --i)
		{
		  nextDigit = (int)(rightSide % 10);
		  data[i] = unchecked((sbyte)(0x00F0 | nextDigit));
		  rightSide /= 10;
		}

		// Compute the bytes for the left side of the decimal point.
		for (int i = decimalOffset; i >= offset; --i)
		{
		  nextDigit = (int)(leftSide % 10);
		  data[i] = unchecked((sbyte)(0x00F0 | nextDigit));
		  leftSide /= 10;
		}

		// Fix the sign, if negative.
		if (d < 0)
		{
		  data[rightmostOffset] = unchecked((sbyte)(data[rightmostOffset] & 0x00DF));
		}

		// If left side still has digits, then the value was too big
		// to fit.
		if (leftSide > 0)
		{
		  throw new System.FormatException("Double value " + d + " too big for output array.");
		}
	  }

	  /// <summary>
	  /// The scale is 0, and 0 &lt; numDigits &lt;= 20.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void longToZonedDecimal(long l, final byte[] data, final int offset, final int numDigits)
	  public static void longToZonedDecimal(long l, sbyte[] data, int offset, int numDigits)
	  {
		int rightmostOffset = offset + numDigits - 1;
		bool isNegative = (l < 0);
		// Compute the bytes for the left side of the decimal point.
		for (int i = rightmostOffset; i >= offset; --i)
		{
		  int nextDigit = (int)(l % 10);
		  data[i] = unchecked((sbyte)(0x00F0 | nextDigit));
		  l /= 10;
		}

		if (isNegative)
		{
			data[rightmostOffset] = unchecked((sbyte)(data[rightmostOffset] & 0x00DF));
		}

		if (l != 0)
		{
		  throw new System.FormatException("Long value too big for ZONED(" + numDigits + ",0).");
		}
	  }

	  /// <summary>
	  /// Converts the specified String (number) into zoned decimal bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final byte[] stringToZonedDecimal(final String s, final int numDigits)
	  public static sbyte[] stringToZonedDecimal(string s, int numDigits)
	  {
		sbyte[] b = new sbyte[numDigits];
		stringToZonedDecimal(s, numDigits, b, 0);
		return b;
	  }

	  /// <summary>
	  /// Converts the specified String (number) into zoned decimal bytes.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static final void stringToZonedDecimal(final String s, final int numDigits, final byte[] buffer, final int offset)
	  public static void stringToZonedDecimal(string s, int numDigits, sbyte[] buffer, int offset)
	  {
		int counter = offset + numDigits - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isNegative = s != null && s.length() > 0 && s.charAt(0) == '-';
		bool isNegative = !string.ReferenceEquals(s, null) && s.Length > 0 && s[0] == '-';
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = isNegative ? 1 : 0;
		int stop = isNegative ? 1 : 0;
		if (!string.ReferenceEquals(s, null))
		{
		for (int i = s.Length - 1; i >= stop; --i)
		{
		  char c = s[i];
		  if (c != '.')
		  {
			int index = c - '0';
			if (index < 0 || index > 9)
			{
				throw new System.FormatException("Invalid character " + c);
			}
			buffer[counter--] = unchecked((sbyte)(CHAR_LOW[index] | 0xF0));
		  }
		}
		}
		if (isNegative)
		{
		  buffer[offset + numDigits - 1] = (sbyte)((buffer[offset + numDigits - 1] & ((sbyte) 0x0F)) | unchecked((sbyte) 0xD0));
		}
	  }


	  /// <summary>
	  /// Return the default NLV for the client corresponding to the Java locale. </summary>
	  /// <returns> the default client NLV </returns>
	  public static string DefaultNLV
	  {
		  get
		  {
			  if (localeNlvMap_ == null)
			  {
				  localeNlvMap_ = new Hashtable(100); // 74 actual entries.
					// 74 entries.
					localeNlvMap_["ar"] = "2954";
					localeNlvMap_["ar_SA"] = "2954";
					localeNlvMap_["be"] = "2979";
					localeNlvMap_["bg"] = "2974";
					localeNlvMap_["ca"] = "2931";
					localeNlvMap_["cs"] = "2975";
					localeNlvMap_["da"] = "2926";
					localeNlvMap_["de"] = "2929";
					localeNlvMap_["de_CH"] = "2939";
					localeNlvMap_["de_DE"] = "2929";
					localeNlvMap_["el"] = "2957";
					localeNlvMap_["en"] = "2924";
					localeNlvMap_["en_BE"] = "2909";
					localeNlvMap_["en_CN"] = "2984";
					localeNlvMap_["en_JP"] = "2938";
					localeNlvMap_["en_KR"] = "2984";
					localeNlvMap_["en_SG"] = "2984";
					localeNlvMap_["en_TW"] = "2984";
					localeNlvMap_["es"] = "2931";
					localeNlvMap_["es_ES"] = "2931";
					localeNlvMap_["et"] = "2902";
					localeNlvMap_["fa"] = "2998";
					localeNlvMap_["fi"] = "2925";
					localeNlvMap_["fr"] = "2928";
					localeNlvMap_["fr_BE"] = "2966";
					localeNlvMap_["fr_CA"] = "2981";
					localeNlvMap_["fr_CH"] = "2940";
					localeNlvMap_["fr_FR"] = "2928";
					localeNlvMap_["hr"] = "2912";
					localeNlvMap_["hu"] = "2976";
					localeNlvMap_["is"] = "2958";
					localeNlvMap_["it"] = "2932";
					localeNlvMap_["it_CH"] = "2942";
					localeNlvMap_["iw"] = "2961";
					localeNlvMap_["ja"] = "2962";
					localeNlvMap_["ji"] = "2961";
					localeNlvMap_["ka"] = "2979";
					localeNlvMap_["kk"] = "2979";
					localeNlvMap_["ko"] = "2986";
					localeNlvMap_["ko_KR"] = "2986";
					localeNlvMap_["lo"] = "2906";
					localeNlvMap_["lt"] = "2903";
					localeNlvMap_["lv"] = "2904";
					localeNlvMap_["mk"] = "2913";
					localeNlvMap_["nl"] = "2923";
					localeNlvMap_["nl_BE"] = "2963";
					localeNlvMap_["nl_NL"] = "2923";
					localeNlvMap_["no"] = "2933";
					localeNlvMap_["pl"] = "2978";
					localeNlvMap_["pt"] = "2996";
					localeNlvMap_["pt_BR"] = "2980";
					localeNlvMap_["pt_PT"] = "2922";
					localeNlvMap_["ro"] = "2992";
					localeNlvMap_["ru"] = "2979";
					localeNlvMap_["sh"] = "2912";
					localeNlvMap_["sk"] = "2994";
					localeNlvMap_["sl"] = "2911";
					localeNlvMap_["sq"] = "2995";
					localeNlvMap_["sr"] = "2914";
					localeNlvMap_["sv"] = "2937";
					localeNlvMap_["sv_SE"] = "2937";
					localeNlvMap_["th"] = "2972";
					localeNlvMap_["th_TH"] = "2972";
					localeNlvMap_["tr"] = "2956";
					localeNlvMap_["uk"] = "2979";
					localeNlvMap_["uz"] = "2979";
					localeNlvMap_["vi"] = "2905";
					localeNlvMap_["zh"] = "2989";
					localeNlvMap_["zh_CN"] = "2989";
					localeNlvMap_["zh_HK"] = "2987";
					localeNlvMap_["zh_SG"] = "2989";
					localeNlvMap_["zh_TW"] = "2987";
					localeNlvMap_["cht"] = "2987"; // Chinese/Taiwan
					localeNlvMap_["cht_CN"] = "2987"; // Chinese/Taiwan
			  }
			  string defaultNLV = null;
			  try
			  {
				  Locale locale = Locale.Default;
				  if (locale != null)
				  {
					string localeString = locale.ToString();
					defaultNLV = (string) localeNlvMap_[localeString];
					if (string.ReferenceEquals(defaultNLV, null))
					{
						// Check for underscore index in locale
						int underscoreIndex = localeString.IndexOf('_');
						if (underscoreIndex > 0)
						{
							localeString = localeString.Substring(0,underscoreIndex);
							defaultNLV = (string) localeNlvMap_[localeString];
						}
					}
				  }
			  }
			  catch (Exception)
			  {
				  // Ignore any errors 
			  }
			  if (string.ReferenceEquals(defaultNLV, null))
			  {
				  defaultNLV = "2924";
			  }
			  return defaultNLV;
    
		  }
	  }



	}


}