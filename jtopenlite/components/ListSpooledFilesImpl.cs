///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListSpooledFilesImpl.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.openlist;
	using com.ibm.jtopenlite.command.program.print;

	internal class ListSpooledFilesImpl : OpenListOfSpooledFilesFormatOSPL0300Listener, OpenListOfSpooledFilesFilterOSPF0100Listener, SpooledFileInfoListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			spooledFileList_ = new OpenListOfSpooledFiles(spooledFileFormat_, 1000, 1, null, this, null, null, null);
			handler_ = new OpenListHandler(spooledFileList_, spooledFileFormat_, this);
		}

	  private readonly OpenListOfSpooledFilesFormatOSPL0300 spooledFileFormat_ = new OpenListOfSpooledFilesFormatOSPL0300();
	  private OpenListOfSpooledFiles spooledFileList_;
	  private OpenListHandler handler_;

	  private SpooledFileInfoListener sfiListener_;
	  private int counter_ = -1;
	  private SpooledFileInfo[] spooledFiles_;
	  // private final char[] charBuffer_ = new char[4096];

	  private string userName_;

	  public ListSpooledFilesImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public virtual SpooledFileInfoListener SpooledFileInfoListener
	  {
		  set
		  {
			sfiListener_ = value;
		  }
	  }

	  public virtual void openComplete()
	  {
	  }

	  public virtual void totalRecordsInList(int total)
	  {
		sfiListener_.totalRecords(total);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpooledFileInfo[] getSpooledFiles(final CommandConnection conn, String user) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual SpooledFileInfo[] getSpooledFiles(CommandConnection conn, string user)
	  {
		spooledFiles_ = null;
		counter_ = -1;
		userName_ = user;
		handler_.process(conn, 1000);
		return spooledFiles_;
	  }

	  public virtual bool stopProcessing()
	  {
		return false;
	  }

	  public virtual void totalRecords(int totalRecords)
	  {
		spooledFiles_ = new SpooledFileInfo[totalRecords];
	  }

	  public virtual void newSpooledFileInfo(SpooledFileInfo info, int index)
	  {
		spooledFiles_[index] = info;
	  }

	  // OSPL0300 listener.

	  public virtual void newSpooledFileEntry(string jobName, string jobUser, string jobNumber, string spooledFileName, int spooledFileNumber, int fileStatus, string dateOpened, string timeOpened, string spooledFileSchedule, string jobSystemName, string userData, string formType, string outputQueueName, string outputQueueLibrary, int auxiliaryStoragePool, long size, int totalPages, int copiesLeftToPrint, string priority, int internetPrintProtocolJobIdentifier)
	  {
		sfiListener_.newSpooledFileInfo(new SpooledFileInfo(jobName, jobUser, jobNumber, spooledFileName, spooledFileNumber, fileStatus, dateOpened, timeOpened, userData, formType, outputQueueName, outputQueueLibrary, auxiliaryStoragePool, size, totalPages, copiesLeftToPrint, priority), ++counter_);
	  }


	  // Filter listener.

	  public virtual int NumberOfUserNames
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual string getUserName(int index)
	  {
		return string.ReferenceEquals(userName_, null) ? "*CURRENT" : userName_;
	  }

	  public virtual int NumberOfOutputQueues
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual string getOutputQueueName(int index)
	  {
		return "*ALL";
	  }

	  public virtual string getOutputQueueLibrary(int index)
	  {
		return "";
	  }

	  public virtual string FormType
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string UserSpecifiedData
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual int NumberOfStatuses
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual string getStatus(int index)
	  {
		return "*ALL";
	  }

	  public virtual int NumberOfPrinterDevices
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public virtual string getPrinterDevice(int index)
	  {
		return "*ALL";
	  }
	}
}