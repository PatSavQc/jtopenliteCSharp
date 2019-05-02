using System;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CcsidConversion.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ccsidConversion
{

	/*
	 * This class provides conversion routine for CCSID typically not available in a JVM.
	 * A mobile implementation should only ship the conversion table that it believes are necessary.
	 */
	public class CcsidConversion
	{
		internal static object @lock = new object();
		internal static SingleByteConversion[] ccsidToSingleByteConversion = new SingleByteConversion[700];
		internal static StringBuilder sb = new StringBuilder();



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SingleByteConversion getSingleByteConversion(int ccsid) throws java.io.UnsupportedEncodingException
		public static SingleByteConversion getSingleByteConversion(int ccsid)
		{
			SingleByteConversion singleByteConversion;
			lock (@lock)
			{
				if (ccsid >= ccsidToSingleByteConversion.Length)
				{
					SingleByteConversion[] newCcsidToSingleByteConversion = new SingleByteConversion[ccsid + 1];
					for (int i = 0; i < ccsidToSingleByteConversion.Length; i++)
					{
						newCcsidToSingleByteConversion [i] = ccsidToSingleByteConversion [i];
					}
					ccsidToSingleByteConversion = newCcsidToSingleByteConversion;
				}
				singleByteConversion = ccsidToSingleByteConversion[ccsid];
				if (singleByteConversion == null)
				{
					singleByteConversion = acquireSingleByteConversion(ccsid);
				}
			}
			return singleByteConversion;
		}

		/* String in the specified CCSID */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String createString(byte[] data, int offset, int length, int ccsid) throws java.io.UnsupportedEncodingException
		public static string createString(sbyte[] data, int offset, int length, int ccsid)
		{
				SingleByteConversion singleByteConversion;

				singleByteConversion = getSingleByteConversion(ccsid);

				char[] toUnicodeTable = singleByteConversion.returnToUnicode();
				lock (@lock)
				{
				sb.Length = 0;

				for (int i = 0; i < length; i++)
				{
					int b = 0xFF & data[i + offset];
					if (b < toUnicodeTable.Length)
					{
						sb.Append(toUnicodeTable[b]);
					}
					else
					{
						sb.Append('\uFFFD');
					}
				}
				return sb.ToString();
				}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static SingleByteConversion acquireSingleByteConversion(int ccsid) throws java.io.UnsupportedEncodingException
		internal static SingleByteConversion acquireSingleByteConversion(int ccsid)
		{
			// Attempt to find the shipped table using reflection
			SingleByteConversion singleByteConversion = null;
			Type conversionClass = null;

			try
			{
				conversionClass = Type.GetType("com.ibm.jtopenlite.ccsidConversion.CCSID" + ccsid);

				Type[] emptyParameterTypes = new Type[0];
				System.Reflection.MethodInfo method = conversionClass.GetMethod("getInstance", emptyParameterTypes);
				object[] args = new object[0];
				singleByteConversion = (SingleByteConversion) method.invoke(null, args);

			}
			catch (ClassNotFoundException exceptionCause)
			{
				//
				// TODO:   Download tables from the server.
				//
				UnsupportedEncodingException ex = new UnsupportedEncodingException("CCSID=" + ccsid);
				ex.initCause(exceptionCause);
				throw ex;


			}
			catch (Exception exceptionCause)
			{
				UnsupportedEncodingException ex = new UnsupportedEncodingException("CCSID=" + ccsid);
				ex.initCause(exceptionCause);
				throw ex;

			}
			ccsidToSingleByteConversion[ccsid] = singleByteConversion;
			return singleByteConversion;

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static byte[] stringToEBCDICByteArray(String s, int ccsidToUse) throws java.io.UnsupportedEncodingException
		public static sbyte[] stringToEBCDICByteArray(string s, int ccsidToUse)
		{
			int sLen = s.Length;
			sbyte[] buffer = new sbyte[sLen * 2];
			int outLen = stringToEBCDICByteArray(s, sLen, buffer, 0, ccsidToUse);
			sbyte[] outArray = new sbyte[outLen];
			Array.Copy(buffer, 0, outArray, 0, outLen);
			return outArray;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int stringToEBCDICByteArray(String s, byte[] data, int offset, int ccsidToUse) throws java.io.UnsupportedEncodingException
		public static int stringToEBCDICByteArray(string s, sbyte[] data, int offset, int ccsidToUse)
		{
			return stringToEBCDICByteArray(s, s.Length, data, offset, ccsidToUse);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int stringToEBCDICByteArray(String s, int length, byte[] data, int offset, int ccsidToUse) throws java.io.UnsupportedEncodingException
		public static int stringToEBCDICByteArray(string s, int length, sbyte[] data, int offset, int ccsidToUse)
		{


			SingleByteConversion singleByteConversion;

			singleByteConversion = getSingleByteConversion(ccsidToUse);

			sbyte[] fromUnicodeTable = singleByteConversion.returnFromUnicode();

			for (int i = 0; i < length; i++)
			{
				int b = s[i];
				sbyte x;
				if (b < fromUnicodeTable.Length)
				{
					x = fromUnicodeTable[b];
				}
				else
				{
					x = 0x3f;
				}
				data[offset + i] = x;
			}

			return length;

		}

	}

}