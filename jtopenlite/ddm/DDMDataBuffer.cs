///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMDataBuffer.java
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
	/// Represents a set of temporary data for a given record.
	/// Data buffers are reused internally by a DDMConnection when reading records from a file.
	/// When a data buffer is passed to <seealso cref="DDMReadCallback#newRecord DDMReadCallback.newRecord()"/>, its record data buffer, record number,
	/// and null field values will all be in sync. If a data buffer is referenced elsewhere, no
	/// guarantee is made as to what the actual values stored in the buffer will be, as data buffers
	/// are reused while records are read from a file.
	/// 
	/// </summary>
	public sealed class DDMDataBuffer
	{
	  private volatile bool processing_;
	  private readonly sbyte[] recordDataBuffer_;
	  private int recordNumber_;
	  private readonly sbyte[] nullFieldMap_;
	  private readonly bool[] nullFieldValues_;
	  private readonly sbyte[] packetBuffer_;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMDataBuffer(final int recordLength, final int packetLength, final int nullFieldLength)
	  internal DDMDataBuffer(int recordLength, int packetLength, int nullFieldLength)
	  {
		recordDataBuffer_ = new sbyte[recordLength];
		packetBuffer_ = new sbyte[packetLength];
		nullFieldMap_ = new sbyte[nullFieldLength];
		nullFieldValues_ = new bool[nullFieldLength];
	  }

	  internal bool Processing
	  {
		  get
		  {
			return processing_;
		  }
	  }

	  internal void startProcessing()
	  {
		processing_ = true;
	  }

	  internal void doneProcessing()
	  {
		processing_ = false;
	  }

	  /// <summary>
	  /// Returns the current record data stored in this buffer.
	  /// 
	  /// </summary>
	  public sbyte[] RecordDataBuffer
	  {
		  get
		  {
			return recordDataBuffer_;
		  }
	  }

	  internal sbyte[] PacketBuffer
	  {
		  get
		  {
			return packetBuffer_;
		  }
	  }

	  /// <summary>
	  /// Returns the current record number stored in this buffer.
	  /// 
	  /// </summary>
	  public int RecordNumber
	  {
		  get
		  {
			return recordNumber_;
		  }
		  set
		  {
			recordNumber_ = value;
		  }
	  }


	  internal sbyte[] NullFieldMap
	  {
		  get
		  {
			return nullFieldMap_;
		  }
	  }

	  /// <summary>
	  /// Returns the current null field values stored in this buffer.
	  /// 
	  /// </summary>
	  public bool[] NullFieldValues
	  {
		  get
		  {
			return nullFieldValues_;
		  }
	  }
	}


}