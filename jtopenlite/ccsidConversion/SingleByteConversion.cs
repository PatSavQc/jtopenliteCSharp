///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  SingleByteConversion.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ccsidConversion
{
	public interface SingleByteConversion
	{



		int Ccsid {get;}

		/* returns the table mapping from ccsid value to unicode */
		char[] returnToUnicode();

		/* returns the table mapping from unicode to the ccsid value */
		sbyte[] returnFromUnicode();

	}


}