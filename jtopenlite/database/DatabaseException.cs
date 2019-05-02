///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DatabaseException.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database
{
	using com.ibm.jtopenlite;

	public class DatabaseException : DataStreamException
	{
	  /// 
		private const long serialVersionUID = -8481404420909977054L;
	private int class_;
	  private int returnCode_;

	  internal DatabaseException(string dataStreamName, int rcClass, int returnCode) : base(BAD_RETURN_CODE, dataStreamName, (rcClass << 16) | (returnCode & 0x00FFFF))
	  {
		class_ = rcClass;
		returnCode_ = returnCode;
	  }

	  public virtual int ReturnCode
	  {
		  get
		  {
			return returnCode_;
		  }
	  }

	  public virtual int ReturnCodeClass
	  {
		  get
		  {
			return class_;
		  }
	  }

	  public static string getReturnCodeString(int rcClass, int returnCode)
	  {
		if (rcClass == 2)
		{
		  int i = returnCode & 0x00FFFF;
		  switch (i)
		  {
			case 0xFDA5:
				return "Descriptor handle expected";
			case 0xFDA6:
				return "Parameter marker data expected";
			case 0xFDA7:
				return "Descriptor and parameter marker data do not match";
			case 0xFF37:
				return "Statement name not valid";
			case 0xFF99:
				return "Cursor name required for operation";
			case 0xFF9B:
				return "Statement name required for operation";
		  }
		}
		else if (rcClass == 7)
		{
		  int i = returnCode & 0x00FFFF;
		  switch (i)
		  {
			case 0xFD42:
				return "Cannot find the descriptor for the specified handle";
			case 0xFE6A:
				return "RPB to create, already exists";
			case 0xFE6D:
				return "Cannot do this function on the default RPB";
			case 0xFE6F:
				return "Specified RPB not found";
			case 0xFF9A:
				return "Unexpected error, processing can continue";
			case 0xFF9B:
				return "Unexpected error, no more processing can continue";
		  }
		}
		else if (rcClass == 8)
		{
		  int i = returnCode & 0x00FFFF;
		  switch (i)
		  {
			case 0xFF9B:
				return "User exit program rejected request";
		  }
		}
		return "Unknown error";
	  }

	  public virtual string ReturnCodeString
	  {
		  get
		  {
			return getReturnCodeString(class_, returnCode_);
		  }
	  }

	  public override string ToString()
	  {
		return base.ToString() + ": " + ReturnCodeString;
	  }
	}

}