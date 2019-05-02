using System.Threading;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMReaderRunner.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{
	internal sealed class DDMReaderRunner : ThreadStart
	{
	  private bool done_;
	  private readonly DDMThreadedReader reader_;
	  private readonly DDMRecordFormat format_;
	  private readonly int resetIndex_;
	  private readonly int skipCount_;
	  private readonly int total_;

	  internal DDMReaderRunner(DDMThreadedReader reader, DDMRecordFormat format, int reset, int skip, int total)
	  {
		reader_ = reader;
		format_ = format.newCopy();
		resetIndex_ = reset;
		skipCount_ = skip;
		total_ = total;
	  }

	  public void done()
	  {
		done_ = true;
	  }

	  public void run()
	  {
		int currentIndex = resetIndex_;

		while (!done_ && !Thread.CurrentThread.Interrupted)
		{
		  while (!done_ && !Thread.CurrentThread.Interrupted && !reader_.getDataBuffer(currentIndex).Processing)
		  {
			currentIndex += skipCount_;
			if (currentIndex >= total_)
			{
			  currentIndex = resetIndex_;
			}
		  }
		  if (!done_ && !Thread.CurrentThread.Interrupted)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMDataBuffer buffer = reader_.getDataBuffer(currentIndex);
			DDMDataBuffer buffer = reader_.getDataBuffer(currentIndex);
			reader_.process(format_, buffer);
			buffer.doneProcessing();
		  }
		}
	  }
	}

}