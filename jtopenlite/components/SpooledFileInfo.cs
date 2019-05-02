///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  SpooledFileInfo.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.components
{
	/// <summary>
	/// Represents spooled file information returned by the ListSpooledFiles class.
	/// The various toString() methods will print
	/// the various fields in a format similar to what WRKSPLF does.
	/// 
	/// </summary>
	public class SpooledFileInfo
	{
	  private readonly string jobName_;
	  private readonly string jobUser_;
	  private readonly string jobNumber_;
	  private readonly string spooledFileName_;
	  private readonly int spooledFileNumber_;
	  private readonly int fileStatus_;
	  private readonly string dateOpened_;
	  private readonly string timeOpened_;
	  private readonly string userData_;
	  private readonly string formType_;
	  private readonly string outputQueueName_;
	  private readonly string outputQueueLibrary_;
	  private readonly int asp_;
	  private readonly long size_;
	  private readonly int pages_;
	  private readonly int copiesLeft_;
	  private readonly string priority_;

	  internal SpooledFileInfo(string jobName, string jobUser, string jobNumber, string spooledFileName, int spooledFileNumber, int fileStatus, string dateOpened, string timeOpened, string userData, string formType, string outputQueueName, string outputQueueLibrary, int asp, long size, int pages, int copiesLeft, string priority)
	  {
		jobName_ = jobName;
		jobUser_ = jobUser;
		jobNumber_ = jobNumber;
		spooledFileName_ = spooledFileName;
		spooledFileNumber_ = spooledFileNumber;
		fileStatus_ = fileStatus;
		dateOpened_ = dateOpened;
		timeOpened_ = timeOpened;
		userData_ = userData;
		formType_ = formType;
		outputQueueName_ = outputQueueName;
		outputQueueLibrary_ = outputQueueLibrary;
		asp_ = asp;
		size_ = size;
		pages_ = pages;
		copiesLeft_ = copiesLeft;
		priority_ = priority;
	  }

	  public virtual string JobName
	  {
		  get
		  {
			return jobName_;
		  }
	  }

	  public virtual string JobUser
	  {
		  get
		  {
			return jobUser_;
		  }
	  }

	  public virtual string JobNumber
	  {
		  get
		  {
			return jobNumber_;
		  }
	  }

	  public virtual string SpooledFileName
	  {
		  get
		  {
			return spooledFileName_;
		  }
	  }

	  public virtual int SpooledFileNumber
	  {
		  get
		  {
			return spooledFileNumber_;
		  }
	  }

	  public virtual int FileStatus
	  {
		  get
		  {
			return fileStatus_;
		  }
	  }

	  public virtual string DateOpened
	  {
		  get
		  {
			return dateOpened_;
		  }
	  }

	  public virtual string TimeOpened
	  {
		  get
		  {
			return timeOpened_;
		  }
	  }

	  public virtual string UserData
	  {
		  get
		  {
			return userData_;
		  }
	  }

	  public virtual string FormType
	  {
		  get
		  {
			return formType_;
		  }
	  }

	  public virtual string OutputQueueName
	  {
		  get
		  {
			return outputQueueName_;
		  }
	  }

	  public virtual string OutputQueueLibrary
	  {
		  get
		  {
			return outputQueueLibrary_;
		  }
	  }

	  public virtual int ASP
	  {
		  get
		  {
			return asp_;
		  }
	  }

	  public virtual long Size
	  {
		  get
		  {
			return size_;
		  }
	  }

	  public virtual int PageCount
	  {
		  get
		  {
			return pages_;
		  }
	  }

	  public virtual int CopiesLeftToPrint
	  {
		  get
		  {
			return copiesLeft_;
		  }
	  }

	  public virtual string Priority
	  {
		  get
		  {
			return priority_;
		  }
	  }

	  public virtual string FileStatusString
	  {
		  get
		  {
			switch (fileStatus_)
			{
			  case 1:
				  return "RDY";
			  case 2:
				  return "OPN";
			  case 3:
				  return "CLO";
			  case 4:
				  return "SAV";
			  case 5:
				  return "WTR";
			  case 6:
				  return "HLD";
			  case 7:
				  return "MSGW";
			  case 8:
				  return "PND";
			  case 9:
				  return "PRT";
			  case 10:
				  return "FIN";
			  case 11:
				  return "SND";
			  case 12:
				  return "DFR";
			}
			return "";
		  }
	  }

	  public override string ToString()
	  {
		string pageSpaces = "";
		if (pages_ < 10000)
		{
			pageSpaces += " ";
		}
		if (pages_ < 1000)
		{
			pageSpaces += " ";
		}
		if (pages_ < 100)
		{
			pageSpaces += " ";
		}
		if (pages_ < 10)
		{
			pageSpaces += " ";
		}
		return spooledFileName_ + "  " + jobUser_ + "  " + outputQueueName_ + "  " + userData_ + "  " + FileStatusString + "  " + pageSpaces + pages_ + "  " + copiesLeft_;
	  }

	  public virtual string toString2()
	  {
		return spooledFileName_ + "  " + jobUser_ + "  " + formType_ + "  " + priority_ + "  " + dateOpened_ + "  " + timeOpened_;
	  }

	  public virtual string toString3()
	  {
		string numSpaces = "";
		if (spooledFileNumber_ < 10000)
		{
			numSpaces += " ";
		}
		if (spooledFileNumber_ < 1000)
		{
			numSpaces += " ";
		}
		if (spooledFileNumber_ < 100)
		{
			numSpaces += " ";
		}
		if (spooledFileNumber_ < 10)
		{
			numSpaces += " ";
		}
		return spooledFileName_ + "  " + numSpaces + spooledFileNumber_ + "  " + jobName_ + "  " + jobUser_ + "  " + jobNumber_;
	  }

	  public virtual string toString4()
	  {
		string aspSpaces = "";
		if (asp_ < 100)
		{
			aspSpaces += " ";
		}
		if (asp_ < 10)
		{
			aspSpaces += " ";
		}
		return spooledFileName_ + "  " + outputQueueName_ + "  " + outputQueueLibrary_ + "  " + aspSpaces + asp_ + "  " + size_;
	  }

	  public virtual string Timestamp
	  {
		  get
		  {
			char[] t = new char[19];
			t[0] = '2';
			t[1] = '0';
			t[2] = dateOpened_[1];
			t[3] = dateOpened_[2];
			t[4] = '-';
			t[5] = dateOpened_[3];
			t[6] = dateOpened_[4];
			t[7] = '-';
			t[8] = dateOpened_[5];
			t[9] = dateOpened_[6];
			t[10] = ' ';
			t[11] = timeOpened_[0];
			t[12] = timeOpened_[1];
			t[13] = ':';
			t[14] = timeOpened_[2];
			t[15] = timeOpened_[3];
			t[16] = ':';
			t[17] = timeOpened_[4];
			t[18] = timeOpened_[5];
			return new string(t);
		  }
	  }

	/*  private String formatDate()
	  {
	    String year = date_.substring(1,3);
	    String month = date_.substring(3,5);
	    String day = date_.substring(5);
	    return month+"/"+day+"/"+year;
	  }
	
	  private String formatTime()
	  {
	    String hour = time_.substring(0,2);
	    String min = time_.substring(2,4);
	    String sec = time_.substring(4);
	    return hour+":"+min+":"+sec;
	  }
	*/
	}

}