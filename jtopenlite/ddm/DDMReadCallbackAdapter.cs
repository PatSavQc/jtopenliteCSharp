///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMReadCallbackAdapter.java
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
	/// Helper class for implementing a <seealso cref="DDMReadCallback DDMReadCallback"/>.
	/// 
	/// </summary>
	public abstract class DDMReadCallbackAdapter : DDMReadCallback
	{
	  private bool done_;

	  public DDMReadCallbackAdapter()
	  {
	  }

	  /// <summary>
	  /// Returns true after an operation calls <seealso cref="#endOfFile endOfFile()"/> or <seealso cref="#recordNotFound recordNotFound()"/>.
	  /// To reset the state, call <seealso cref="#reset reset()"/>.
	  /// 
	  /// </summary>
	  public virtual bool Done
	  {
		  get
		  {
			return done_;
		  }
	  }

	  /// <summary>
	  /// Called by the other newRecord().
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void newRecord(int recordNumber, byte[] recordData, boolean[] nullFields) throws IOException;
	  public abstract void newRecord(int recordNumber, sbyte[] recordData, bool[] nullFields);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void newRecord(DDMCallbackEvent event, DDMDataBuffer dataBuffer) throws IOException
	  public virtual void newRecord(DDMCallbackEvent @event, DDMDataBuffer dataBuffer)
	  {
		newRecord(dataBuffer.RecordNumber, dataBuffer.RecordDataBuffer, dataBuffer.NullFieldValues);
	  }

	  public virtual void recordNotFound(DDMCallbackEvent @event)
	  {
		done_ = true;
	  }

	  public virtual void endOfFile(DDMCallbackEvent @event)
	  {
		done_ = true;
	  }

	  /// <summary>
	  /// Resets the state of this callback adapter. </summary>
	  /// <seealso cref= #isDone
	  ///  </seealso>
	  public virtual void reset()
	  {
		done_ = false;
	  }
	}



}