///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  SingleByteConversionTable.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ccsidConversion
{
	public class SingleByteConversionTable : SingleByteConversion
	{
		internal int ccsid_;
		internal char[] toUnicode_;
		internal sbyte[] fromUnicode_;


		public virtual int Ccsid
		{
			get
			{
				return ccsid_;
			}
		}

		/* returns the table mapping from ccsid value to unicode */
		public virtual char[] returnToUnicode()
		{
			return toUnicode_;
		}

		/* returns the table mapping from unicode to the ccsid value */
		public virtual sbyte[] returnFromUnicode()
		{
			return fromUnicode_;
		}



		public static sbyte[] generateFromUnicode(char[] toUnicode)
		{
			int maxUnicodeValue = 0;
			for (int i = 0; i < toUnicode.Length; i++)
			{
				if (toUnicode[i] > (char)maxUnicodeValue)
				{
					maxUnicodeValue = toUnicode[i];
				}
			}
			sbyte[] fromUnicode = new sbyte[maxUnicodeValue+1];
			for (int i = 0; i < maxUnicodeValue+1; i++)
			{
				fromUnicode[i] = 0x3f;
			}
			for (int i = 0; i < toUnicode.Length; i++)
			{
				fromUnicode[toUnicode[i]] = (sbyte) i;
			}

			return fromUnicode;
		}
	}


}