using System.Threading;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMThreadedReader.java
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
	/// A special kind of <seealso cref="DDMReadCallback DDMReadCallback"/> you can use when you want multiple
	/// threads to simultaneously process data being read out of the same file and connection.  The data is read by the main thread,
	/// but the conversion is done by one or more processing threads.  Subclass this class and implement the
	/// <seealso cref="#process process()"/> method to read record data off-thread from the main I/O thread.
	/// This gives the performance advantage of streaming data from the server in parallel with processing said data.
	/// It is important to note that using more than one thread will likely cause the records to be processed out-of-order.
	/// 
	/// </summary>
	public abstract class DDMThreadedReader : DDMReadCallback
	{
	  private readonly DDMFile file_;
	  private readonly DDMReaderRunner[] runners_;
	  private readonly Thread[] threads_;
	  private bool done_;

	  private long sequence_;

	  /// <summary>
	  /// Constructs a multi-threaded reader to process data being read from the specified file
	  /// using the specified record format. </summary>
	  /// <param name="format"> The record format to copy and give to each thread for it to pass to <seealso cref="#process process()"/>. </param>
	  /// <param name="file"> The file being read. </param>
	  /// <param name="numThreads"> The number of threads to use. This number is capped by the number of buffers in the file object, so
	  /// that each thread always has at least one buffer to process, to avoid contention. Having more than one buffer per thread is fine.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DDMThreadedReader(final DDMRecordFormat format, final DDMFile file, int numThreads)
	  public DDMThreadedReader(DDMRecordFormat format, DDMFile file, int numThreads)
	  {
		file_ = file;
		done_ = false;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numBuffers = file.getBufferCount();
		int numBuffers = file.BufferCount;
		if (numThreads > numBuffers)
		{
			numThreads = numBuffers;
		}
		runners_ = new DDMReaderRunner[numThreads];
		threads_ = new Thread[numThreads];
		for (int i = 0; i < numThreads; ++i)
		{
		  runners_[i] = new DDMReaderRunner(this, format, i, numThreads, numBuffers);
		  threads_[i] = new Thread(runners_[i], "DDMThreadedReader-" + i);
		  threads_[i].Start();
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: final DDMDataBuffer getDataBuffer(final int index)
	  internal DDMDataBuffer getDataBuffer(int index)
	  {
		return file_.getDataBuffer(index);
	  }

	  /// <summary>
	  /// Do not call this method directly; it is implemented for DDMConnection to call.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public final void newRecord(final DDMCallbackEvent event, final DDMDataBuffer buffer)
	  public void newRecord(DDMCallbackEvent @event, DDMDataBuffer buffer)
	  {
		buffer.startProcessing();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMFile file = event.getFile();
		DDMFile file = @event.File;
		if (file == file_)
		{
		  DDMDataBuffer nextBuffer = file.NextDataBuffer;
		  // Wait to use the next buffer until our background thread is done with it.
		  while (nextBuffer.Processing) // Uses volatile variable, faster than synchronization, at the cost of a CPU loop here.
		  {
			file.nextBuffer(); // Advance.
			nextBuffer = file.NextDataBuffer; // Check the next one.
		  }
		}
	  }

	  /// <summary>
	  /// Do not call this method directly; it is implemented for DDMConnection to call.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public final void recordNotFound(final DDMCallbackEvent event)
	  public void recordNotFound(DDMCallbackEvent @event)
	  {
		finish();
	  }

	  /// <summary>
	  /// Do not call this method directly; it is implemented for DDMConnection to call.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public final void endOfFile(final DDMCallbackEvent event)
	  public void endOfFile(DDMCallbackEvent @event)
	  {
		finish();
	  }

	  /// <summary>
	  /// Indicates if end-of-file has been reached and our threads have been shutdown.
	  /// 
	  /// </summary>
	  public bool Done
	  {
		  get
		  {
			return done_;
		  }
	  }

	  private void finish()
	  {
		for (int i = 0; i < file_.BufferCount; ++i)
		{
		  while (file_.getDataBuffer(i).Processing)
		  {
				  ;
		  }
		}

		for (int i = 0; i < runners_.Length; ++i)
		{
		  runners_[i].done();
		}
		for (int i = 0; i < threads_.Length; ++i)
		{
		  try
		  {
			threads_[i].Join();
		  }
		  catch (InterruptedException)
		  {
		  }
		}
		done_ = true;
	  }

	  /// <summary>
	  /// Override this method with your own record processing logic.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public abstract void process(final DDMRecordFormat format, final DDMDataBuffer dataBuffer);
	  public abstract void process(DDMRecordFormat format, DDMDataBuffer dataBuffer);
	}

}