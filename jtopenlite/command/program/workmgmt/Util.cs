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
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	using com.ibm.jtopenlite;

	internal sealed class Util
	{
	  private Util()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static void readKeyData(final byte[] data, int numRead, final int key, final int lengthOfData, final boolean isBinary, JobKeyDataListener listener, final char[] c)
	  internal static void readKeyData(sbyte[] data, int numRead, int key, int lengthOfData, bool isBinary, JobKeyDataListener listener, char[] c)
	  {
		switch (lengthOfData)
		{
		  case 1:
			string s1 = Conv.ebcdicByteArrayToString(data, numRead, 1, c);
			listener.newKeyData(key, s1, data, numRead);
			break;
		  case 2:
			string s2 = Conv.ebcdicByteArrayToString(data, numRead, 2, c);
			listener.newKeyData(key, s2, data, numRead);
			break;
		  case 4:
			if (isBinary)
			{
			  int value = Conv.byteArrayToInt(data, numRead);
			  listener.newKeyData(key, value);
			}
			else
			{
			  string s4 = Conv.ebcdicByteArrayToString(data, numRead, 4, c);
			  listener.newKeyData(key, s4, data, numRead);
			}
			break;
	/*      case 8:
	        if (isBinary)
	        {
	          long data = in.readLong();
	          listener.newKeyData(key, data);
	        }
	        else
	        {
	          byte[] b = new byte[8];
	          in.readFully(b);
	          listener.newKeyData(key, new String(b, "Cp037"), b);
	        }
	        break;
	*/
		  case 10:
			string s10 = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
			listener.newKeyData(key, s10, data, numRead);
			break;
		  case 20:
			string s20 = Conv.ebcdicByteArrayToString(data, numRead, 20, c);
			listener.newKeyData(key, s20, data, numRead);
			break;
		  default:
			string s = (c.Length >= lengthOfData) ? Conv.ebcdicByteArrayToString(data, numRead, lengthOfData, c) : Conv.ebcdicByteArrayToString(data, numRead, lengthOfData);
			listener.newKeyData(key, s, data, numRead);
			break;
		}
	  }
	}

}