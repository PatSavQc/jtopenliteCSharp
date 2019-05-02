///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMReadCallback.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{

	/// <summary>
	/// Used by DDMConnection to pass the output of a read operation to the user in a memory-conscious fashion.
	/// 
	/// </summary>
	public interface DDMReadCallback
	{
	  /// <summary>
	  /// Called by DDMConnection when a new record has been read.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void newRecord(DDMCallbackEvent event, DDMDataBuffer dataBuffer) throws IOException;
	  void newRecord(DDMCallbackEvent @event, DDMDataBuffer dataBuffer);

	  /// <summary>
	  /// Called by DDMConnection when a keyed read returned no matching records.
	  /// 
	  /// </summary>
	  void recordNotFound(DDMCallbackEvent @event);

	  /// <summary>
	  /// Called by DDMConnection when a read or position operation moved the cursor to before the first record or after the last record.
	  /// 
	  /// </summary>
	  void endOfFile(DDMCallbackEvent @event);
	}

}