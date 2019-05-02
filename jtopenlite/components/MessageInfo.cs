///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  MessageInfo.java
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
	/// Represents message information returned by the ListQSYSOPRMessages class.
	/// The toString() and toString2() methods will print
	/// the various fields in a format similar to what DSPMSG QSYSOPR does.
	/// 
	/// </summary>
	public class MessageInfo
	{
	  private int severity_;
	  private string identifier_;
	  private string type_;
	  private int key_;
	  private string date_;
	  private string time_;
	  private string microseconds_;

	  private string replyStatus_;
	  private string text_;

	  internal MessageInfo(int sev, string id, string type, int key, string date, string time, string micro)
	  {
		severity_ = sev;
		identifier_ = id;
		type_ = type;
		key_ = key;
		date_ = date;
		time_ = time;
		microseconds_ = micro;
	  }

	  public virtual int Severity
	  {
		  get
		  {
			return severity_;
		  }
	  }

	  public virtual string Identifier
	  {
		  get
		  {
			return identifier_;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  public virtual int Key
	  {
		  get
		  {
			return key_;
		  }
	  }

	  public virtual string Date
	  {
		  get
		  {
			return date_;
		  }
	  }

	  public virtual string Time
	  {
		  get
		  {
			return time_;
		  }
	  }

	  public virtual string Microseconds
	  {
		  get
		  {
			return microseconds_;
		  }
	  }

	  public virtual string ReplyStatus
	  {
		  set
		  {
			replyStatus_ = value;
		  }
		  get
		  {
			return replyStatus_;
		  }
	  }


	  public virtual string Text
	  {
		  set
		  {
			text_ = value;
		  }
		  get
		  {
			return text_;
		  }
	  }


	  public override string ToString()
	  {
		return identifier_ + ": " + text_;
	  }

	  private string TypeString
	  {
		  get
		  {
			char c0 = type_[0];
			char c1 = type_[1];
			switch (c0)
			{
			  case '0':
				switch (c1)
				{
				  case '1':
					  return "Completion";
				  case '2':
					  return "Diagnostic";
				  case '4':
					  return "Informational";
				  case '5':
					  return "Inquiry";
				  case '6':
					  return "Sender's copy";
				  case '8':
					  return "Request";
				}
				break;
			  case '1':
				switch (c1)
				{
				  case '0':
					  return "Request with prompting";
				  case '4':
					  return "Notify, exception already handled when API is called";
				  case '5':
					  return "Escape, exception already handled when API is called";
				  case '6':
					  return "Notify, exception not handled when API is called";
				  case '7':
					  return "Escape, exception not handled when API is called";
				}
				break;
			  case '2':
				switch (c1)
				{
				  case '1':
					  return "Reply, not checked for validity";
				  case '2':
					  return "Reply, checked for validity";
				  case '3':
					  return "Reply, message default used";
				  case '4':
					  return "Reply, system default used";
				  case '5':
					  return "Reply, from system reply list";
				  case '6':
					  return "Reply, from exit program";
				}
				break;
			}
			return "Unknown";
		  }
	  }

	  private string formatDate()
	  {
		string year = date_.Substring(1, 2);
		string month = date_.Substring(3, 2);
		string day = date_.Substring(5);
		return month + "/" + day + "/" + year;
	  }

	  private string formatTime()
	  {
		string hour = time_.Substring(0,2);
		string min = time_.Substring(2, 2);
		string sec = time_.Substring(4);
		return hour + ":" + min + ":" + sec;
	  }

	  public virtual string toString2()
	  {
		return "Message ID: " + identifier_ + "\t Severity: " + severity_ + "\n"+
			   "Message type: " + TypeString + "\n"+
			   "Date sent: " + formatDate() + "\t Time sent: " + formatTime() + "\n"+
			   "Message: " + text_ + "\n";
	  }
	}

}