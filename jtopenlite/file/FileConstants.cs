///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  FileConstants.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.file
{
	public sealed class FileConstants
	{
	  private FileConstants()
	  {
	  }

	  public const int RC_FILE_NOT_FOUND = 2;
	  public const int RC_PATH_NOT_FOUND = 3;
	  public const int RC_ACCESS_DENIED = 13;
	  public const int RC_NO_MORE_FILES = 18;

	  public static string returnCodeToString(int rc)
	  {
		switch (rc)
		{
		  case RC_FILE_NOT_FOUND:
			  return "File not found";
		  case RC_PATH_NOT_FOUND:
			  return "Path not found";
		  case RC_ACCESS_DENIED:
			  return "Access denied";
		  case RC_NO_MORE_FILES:
			  return "No more files";
		}
		return "Unknown return code";
	  }
	}

}