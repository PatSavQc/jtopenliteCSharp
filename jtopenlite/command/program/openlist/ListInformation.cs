///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: ListInformation.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.openlist
{
	public class ListInformation
	{
	  public const int TYPE_COMPLETE = 0;
	  public const int TYPE_INCOMPLETE = 1;
	  public const int TYPE_PARTIAL = 2;
	  public const int TYPE_UNKNOWN = 3;

	  public const int STATUS_PENDING = 4;
	  public const int STATUS_BUILDING = 5;
	  public const int STATUS_BUILT = 6;
	  public const int STATUS_ERROR = 7;
	  public const int STATUS_PRIMED = 8;
	  public const int STATUS_OVERFLOW = 9;
	  public const int STATUS_UNKNOWN = 10;

	  private int totalRecords_;
	  private int recordsReturned_;
	  private sbyte[] requestHandle_;
	  private int recordLength_;
	  private int infoComplete_;
	  private string created_;
	  private int status_;
	  private int lengthOfInfoReturned_;
	  private int firstRecord_;

	  public ListInformation(int total, int returned, sbyte[] handle, int length, int complete, string date, int status, int lengthOfInfo, int first)
	  {
		totalRecords_ = total;
		recordsReturned_ = returned;
		requestHandle_ = handle;
		recordLength_ = length;
		infoComplete_ = complete;
		created_ = date;
		status_ = status;
		lengthOfInfoReturned_ = lengthOfInfo;
		firstRecord_ = first;
	  }

	  public virtual int TotalRecords
	  {
		  get
		  {
			return totalRecords_;
		  }
	  }

	  public virtual int RecordsReturned
	  {
		  get
		  {
			return recordsReturned_;
		  }
	  }

	  public virtual sbyte[] RequestHandle
	  {
		  get
		  {
			return requestHandle_;
		  }
	  }

	  public virtual int RecordLength
	  {
		  get
		  {
			return recordLength_;
		  }
	  }

	  public virtual int CompleteType
	  {
		  get
		  {
			return infoComplete_;
		  }
	  }

	  public virtual string DateAndTimeCreated
	  {
		  get
		  {
			return created_;
		  }
	  }

	  public virtual int Status
	  {
		  get
		  {
			return status_;
		  }
	  }

	  public virtual int LengthOfInformationReturned
	  {
		  get
		  {
			return lengthOfInfoReturned_;
		  }
	  }

	  public virtual int FirstRecord
	  {
		  get
		  {
			return firstRecord_;
		  }
	  }
	}

}