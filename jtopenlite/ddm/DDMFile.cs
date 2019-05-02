///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMFile.java
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
	/// Represents a handle to a file.
	/// 
	/// </summary>
	public sealed class DDMFile
	{
	  public const int READ_ONLY = 0;
	  public const int WRITE_ONLY = 1;
	  public const int READ_WRITE = 2;

	  private readonly string library_;
	  private readonly string file_;
	  private readonly string member_;
	  private readonly sbyte[] recordFormatName_;
	  private readonly sbyte[] dclNam_;
	  private readonly int openType_;
	  private readonly int recordLength_;
	  private readonly int recordIncrement_;
	  private readonly int batchSize_;
	  private readonly int nullFieldByteMapOffset_;
	  private readonly DDMDataBuffer[] buffers_;
	  private readonly DDMCallbackEvent eventBuffer_;

	  internal DDMFile(string library, string file, string member, sbyte[] recordFormatName, sbyte[] dclName, int openType, int recLength, int recInc, int batchSize, int nullFieldOffset, int numBuffers)
	  {
		library_ = library;
		file_ = file;
		member_ = member;
		recordFormatName_ = recordFormatName;
		dclNam_ = dclName;
		openType_ = openType;
		recordLength_ = recLength;
		recordIncrement_ = recInc;
		batchSize_ = batchSize;
		nullFieldByteMapOffset_ = nullFieldOffset;
		eventBuffer_ = new DDMCallbackEvent(this);

		numBuffers = numBuffers <= 0 ? 1 : numBuffers;

		buffers_ = new DDMDataBuffer[numBuffers];
		for (int i = 0; i < numBuffers; ++i)
		{
		  buffers_[i] = new DDMDataBuffer(recordLength_, recordIncrement_ - recordLength_ + 2, recordIncrement_ - nullFieldOffset);
		}
	  }

	  internal sbyte[] DCLNAM
	  {
		  get
		  {
			return dclNam_;
		  }
	  }

	  internal sbyte[] RecordFormatName
	  {
		  get
		  {
			return recordFormatName_;
		  }
	  }

	  internal int RecordIncrement
	  {
		  get
		  {
			return recordIncrement_;
		  }
	  }

	  internal int BatchSize
	  {
		  get
		  {
			return batchSize_;
		  }
	  }

	  internal int NullFieldByteMapOffset
	  {
		  get
		  {
			return nullFieldByteMapOffset_;
		  }
	  }

	  private int bufferIndex_;

	  internal void nextBuffer()
	  {
		if (++bufferIndex_ == buffers_.Length)
		{
		  bufferIndex_ = 0;
		}
	  }

	  /// <summary>
	  /// Returns the number of data buffers used for reading records from this file.
	  /// 
	  /// </summary>
	  public int BufferCount
	  {
		  get
		  {
			return buffers_.Length;
		  }
	  }

	  internal int CurrentBufferIndex
	  {
		  get
		  {
			return bufferIndex_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMDataBuffer getDataBuffer(final int index)
	  internal DDMDataBuffer getDataBuffer(int index)
	  {
		return buffers_[index];
	  }

	  internal DDMDataBuffer NextDataBuffer
	  {
		  get
		  {
			int index = bufferIndex_ + 1;
			if (index == buffers_.Length)
			{
				index = 0;
			}
			return buffers_[index];
		  }
	  }

	  internal sbyte[] NullFieldMap
	  {
		  get
		  {
			return buffers_[bufferIndex_].NullFieldMap;
		  }
	  }

	  internal bool[] NullFieldValues
	  {
		  get
		  {
			return buffers_[bufferIndex_].NullFieldValues;
		  }
	  }

	  /// <summary>
	  /// Returns the record length in bytes of this file.
	  /// 
	  /// </summary>
	  public int RecordLength
	  {
		  get
		  {
			return recordLength_;
		  }
	  }

	  /// <summary>
	  /// Returns the current data buffer's record data buffer.
	  /// 
	  /// </summary>
	  public sbyte[] RecordDataBuffer
	  {
		  get
		  {
			return buffers_[bufferIndex_].RecordDataBuffer;
		  }
	  }

	  internal sbyte[] PacketBuffer
	  {
		  get
		  {
			return buffers_[bufferIndex_].PacketBuffer;
		  }
	  }

	  internal DDMCallbackEvent EventBuffer
	  {
		  get
		  {
			return eventBuffer_;
		  }
	  }

	  /// <summary>
	  /// Returns the read-write access type used to open this file.
	  /// 
	  /// </summary>
	  public int ReadWriteType
	  {
		  get
		  {
			return openType_;
		  }
	  }

	  /// <summary>
	  /// Returns the library in which this file resides.
	  /// 
	  /// </summary>
	  public string Library
	  {
		  get
		  {
			return library_;
		  }
	  }

	  /// <summary>
	  /// Returns the name of this file.
	  /// 
	  /// </summary>
	  public string File
	  {
		  get
		  {
			return file_;
		  }
	  }

	  /// <summary>
	  /// Returns the member name of this file.
	  /// 
	  /// </summary>
	  public string Member
	  {
		  get
		  {
			return member_;
		  }
	  }

	  public override string ToString()
	  {
		return library_ + file_ + member_;
	  }
	}


}