using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListDiskStatusesImpl.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.components
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.print;
	using com.ibm.jtopenlite.command.program.openlist;
	using com.ibm.jtopenlite.ddm;

	internal class ListDiskStatusesImpl : OpenListOfSpooledFilesFormatOSPL0300Listener, OpenListOfSpooledFilesFilterOSPF0100Listener, DDMReadCallback
	{
		private bool done_ = false;
		private string outputQueueLibrary_ = null;

		internal ListDiskStatusesImpl()
		{
		}

		private string jobName_;
		private string jobUser_;
		private string jobNumber_;
		private string spooledFileName_;
		private int spooledFileNumber_;

		private string elapsedTime_; // TODO

		private readonly char[] charBuffer_ = new char[132];

		private int skip_ = 0;
		private bool theEnd_ = false;
	  private readonly List<DiskStatus> statuses_ = new List<DiskStatus>();

		public virtual void newRecord(DDMCallbackEvent @event, DDMDataBuffer dataBuffer)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] data = dataBuffer.getRecordDataBuffer();
			sbyte[] data = dataBuffer.RecordDataBuffer;
			string line = Conv.ebcdicByteArrayToString(data, 0, data.Length, charBuffer_);
			if (skip_ == 1)
			{
				int index = line.IndexOf(":", StringComparison.Ordinal);
				if (index > 0)
				{
					int end = line.IndexOf("System name", index, StringComparison.Ordinal);
					if (end > index)
					{
			  elapsedTime_ = line.Substring(index + 1, (end - index) - (index + 1)).Trim();
					}
				}
				++skip_;
			}
			else if (skip_ < 4)
			{
				++skip_;
			}
			else if (!theEnd_)
			{
				if (line.IndexOf("E N D  O F  L I S T I N G", StringComparison.Ordinal) >= 0)
				{
					theEnd_ = true;
				}
				else
				{
					StringTokenizer st = new StringTokenizer(line);
					string unit = nextToken(st);
					string type = nextToken(st);
					string sizeMB = nextToken(st);
					string percentUsed = nextToken(st);
					string ioRequests = nextToken(st);
					string requestSizeKB = nextToken(st);
					string readRequests = nextToken(st);
					string writeRequests = nextToken(st);
					string readKB = nextToken(st);
					string writeKB = nextToken(st);
					string percentBusy = nextToken(st);
					string asp = nextToken(st);
					string protectionType = nextToken(st);
					string protectionStatus = nextToken(st);
					string compression = nextToken(st);
					DiskStatus ds = new DiskStatus(unit, type, sizeMB, percentUsed, ioRequests, requestSizeKB, readRequests, writeRequests, readKB, writeKB, percentBusy, asp, protectionType, protectionStatus, compression);
					statuses_.Add(ds);
				}
			}
		}

		private string nextToken(StringTokenizer st)
		{
			return st.hasMoreTokens() ? st.nextToken() : "";
		}

		public virtual void recordNotFound(DDMCallbackEvent @event)
		{
			done_ = true;
		}

		public virtual void endOfFile(DDMCallbackEvent @event)
		{
			done_ = true;
		}

		private bool done()
		{
			return done_;
		}

	  public virtual void totalRecordsInList(int total)
	  {
	  }

	  public virtual void openComplete()
	  {
	  }

	  public virtual bool stopProcessing()
	  {
		return false;
	  }

		public virtual void newSpooledFileEntry(string jobName, string jobUser, string jobNumber, string spooledFileName, int spooledFileNumber, int fileStatus, string dateOpened, string timeOpened, string spooledFileSchedule, string jobSystemName, string userData, string formType, string outputQueueName, string outputQueueLibrary, int auxiliaryStoragePool, long size, int totalPages, int copiesLeftToPrint, string priority, int internetPrintProtocolJobIdentifier)
		{
			jobName_ = jobName;
			jobUser_ = jobUser;
			jobNumber_ = jobNumber;
			spooledFileName_ = spooledFileName;
			spooledFileNumber_ = spooledFileNumber;
		}

		public virtual int NumberOfUserNames
		{
			get
			{
				return 1;
			}
		}

		public virtual string getUserName(int index)
		{
			return "*ALL";
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
			return "DSKSTS";
		}

		public virtual string getOutputQueueLibrary(int index)
		{
			return outputQueueLibrary_;
		}

		public virtual string FormType
		{
			get
			{
				return "*ALL";
			}
		}

		public virtual string UserSpecifiedData
		{
			get
			{
				return "*ALL";
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

		public virtual string ElapsedTime
		{
			get
			{
				return elapsedTime_;
			}
		}

		/// <summary>
		/// NOTE: The workingLibrary will be deleted when this method is called.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiskStatus[] getDiskStatuses(final CommandConnection cc, final DDMConnection ddmConn, String workingLibrary, boolean reset) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual DiskStatus[] getDiskStatuses(CommandConnection cc, DDMConnection ddmConn, string workingLibrary, bool reset)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SystemInfo si1 = cc.getInfo();
			SystemInfo si1 = cc.Info;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SystemInfo si2 = ddmConn.getInfo();
			SystemInfo si2 = ddmConn.Info;
			if (!si1.System.Equals(si2.System) || si1.ServerLevel != si2.ServerLevel)
			{
		  throw new IOException("Command connection does not match DDM connection.");
			}

			skip_ = 0;
			theEnd_ = false;
			statuses_.Clear();
			elapsedTime_ = null;
			done_ = false;
			outputQueueLibrary_ = workingLibrary;

			// I really wish spooled files could go into QTEMP!
		// DDMConnection conn = DDMConnection.getConnection("rchasa12", "csmith",
		// "s1r4l0in");
			// CommandConnection conn = CommandConnection.getConnection("rchasa12",
			// "csmith", "s1r4l0in");
			// System.out.println(conn.getJobName());
			CommandResult result = cc.execute("CLROUTQ OUTQ(" + workingLibrary + "/DSKSTS)");
			if (!result.succeeded())
			{
		  IList<Message> messages = result.MessagesList;
		  if (messages.Count != 1 && !messages[0].ID.Equals("CPF3357"))
		  {
					throw new IOException("Error clearing output queue: " + result.ToString());
		  }
			}
			result = cc.execute("DLTLIB " + workingLibrary);
			if (!result.succeeded())
			{
		  IList<Message> messages = result.MessagesList;
		  if (messages.Count != 1 || !messages[0].ID.Equals("CPF2110")) // Library
																				  // not
																				  // found.
		  {
			throw new IOException("Error deleting library: " + result.ToString());
		  }
			}
			result = cc.execute("CRTLIB " + workingLibrary);
			if (!result.succeeded())
			{
		  throw new IOException("Error creating library: " + result.ToString());
			}
		result = cc.execute("CRTPF " + workingLibrary + "/DSKSTS RCDLEN(132) MAXMBRS(*NOMAX) SIZE(*NOMAX) LVLCHK(*NO)");
			if (!result.succeeded())
			{
				throw new IOException("Error creating physical file: " + result.ToString());
			}
			result = cc.execute("CRTOUTQ OUTQ(" + workingLibrary + "/DSKSTS)");
			if (!result.succeeded())
			{
		  throw new IOException("Error creating output queue: " + result.ToString());
			}
			result = cc.execute("CHGJOB OUTQ(" + workingLibrary + "/DSKSTS)");
			if (!result.succeeded())
			{
				throw new IOException("Error changing job: " + result.ToString());
			}
		result = cc.execute("WRKDSKSTS OUTPUT(*PRINT) RESET(" + (reset ? "*YES" : "*NO") + ")");
			if (!result.succeeded())
			{
		  throw new IOException("Error running WRKDSKSTS: " + result.ToString());
			}
		OpenListOfSpooledFilesFormatOSPL0300 format = new OpenListOfSpooledFilesFormatOSPL0300();
		OpenListOfSpooledFiles list = new OpenListOfSpooledFiles(format, 256, -1, null, this, null, null, null);
		list.FormatListener = this;
			result = cc.call(list);
			if (!result.succeeded())
			{
				throw new IOException("Error retrieving spooled file: " + result.ToString());
			}
			ListInformation info = list.ListInformation;
			CloseList close = new CloseList(info.RequestHandle);
			result = cc.call(close);
			if (!result.succeeded())
			{
				throw new IOException("Error closing spooled file list: " + result.ToString());
			}
			string jobID = jobNumber_.Trim() + "/" + jobUser_.Trim() + "/"
					+ jobName_.Trim();
		result = cc.execute("CPYSPLF FILE(" + spooledFileName_.Trim() + ") TOFILE(" + workingLibrary + "/DSKSTS) JOB(" + jobID + ") SPLNBR(" + spooledFileNumber_ + ") MBROPT(*REPLACE)");
			if (!result.succeeded())
			{
		  throw new IOException("Error copying spooled file: " + result.ToString());
			}

		DDMFile file = ddmConn.open(workingLibrary, "DSKSTS", "DSKSTS", "DSKSTS", DDMFile.READ_ONLY, false, 200, 1);
			while (!done())
			{
				ddmConn.readNext(file, this);
			}
			ddmConn.close(file);
		result = cc.execute("DLTLIB " + workingLibrary);

			DiskStatus[] arr = new DiskStatus[statuses_.Count];
		statuses_.toArray(arr);
		return arr;

			/*
			 * Class.forName("com.ibm.jtopenlite.database.jdbc.JDBCDriver");
			 * java.sql.Connection c =
			 * DriverManager.getConnection("jdbc:systemi://rchasa12", "csmith",
			 * "s1r4l0in"); Statement s = c.createStatement(); ResultSet rs =
			 * s.executeQuery("SELECT * FROM QZRDDSKSTS.DSKSTS"); int skip = 0;
			 * boolean theEnd = false; while (rs.next()) { String line =
			 * rs.getString(1); if (skip < 4) { ++skip; } else if (!theEnd) { if
			 * (line.indexOf("E N D  O F  L I S T I N G") >= 0) { theEnd = true; }
			 * else { StringTokenizer st = new StringTokenizer(line); String unit =
			 * st.nextToken(); String type = st.nextToken(); String sizeMB =
			 * st.nextToken(); String percentUsed = st.nextToken(); String
			 * ioRequests = st.nextToken(); String requestSizeKB = st.nextToken();
			 * String readRequests = st.nextToken(); String writeRequests =
			 * st.nextToken(); String readKB = st.nextToken(); String writeKB =
			 * st.nextToken(); String percentBusy = st.nextToken(); String asp =
			 * st.nextToken(); String protectionType = st.nextToken(); String
			 * protectionStatus = st.nextToken(); String compression =
			 * st.nextToken(); //TODO } } } rs.close(); s.close(); c.close();
			 */
		}
	}

}