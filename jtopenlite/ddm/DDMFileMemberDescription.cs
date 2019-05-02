///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMFileMemberDescription.java
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
	/// Represents information about a file member.
	/// 
	/// </summary>
	public class DDMFileMemberDescription
	{
	  private readonly string library_;
	  private readonly string file_;
	  private readonly string member_;
	  private readonly string fileType_;
	  private readonly string fileAttrib_;
	  private readonly string dataType_;
	  private readonly string fileText_;
	  private readonly string memberText_;
	  private readonly string systemName_;
	  private readonly long recordCapacity_;
	  private readonly long currentRecords_;
	  private readonly long deletedRecords_;

	  internal DDMFileMemberDescription(string fileName, string libName, string fileType, string fileAttrib, string systemName, string dataType, string description, string memberName, string memberDescription, long recordCapacity, long currentRecords, long deletedRecords)
	  {
		library_ = libName;
		file_ = fileName;
		member_ = memberName;
		fileType_ = fileType;
		fileAttrib_ = fileAttrib;
		dataType_ = dataType;
		fileText_ = description;
		memberText_ = memberDescription;
		systemName_ = systemName;
		recordCapacity_ = recordCapacity;
		currentRecords_ = currentRecords;
		deletedRecords_ = deletedRecords;
	  }

	  /// <summary>
	  /// Returns the name (MBNAME) of the member.
	  /// 
	  /// </summary>
	  public virtual string Member
	  {
		  get
		  {
			return member_;
		  }
	  }

	  /// <summary>
	  /// Returns the library (MBLIB) in which the file resides.
	  /// 
	  /// </summary>
	  public virtual string Library
	  {
		  get
		  {
			return library_;
		  }
	  }

	  /// <summary>
	  /// Returns the name of the file (MBFILE).
	  /// 
	  /// </summary>
	  public virtual string File
	  {
		  get
		  {
			return file_;
		  }
	  }

	  /// <summary>
	  /// Returns the file type (MBFTYP).
	  /// 
	  /// </summary>
	  public virtual string FileType
	  {
		  get
		  {
			return fileType_;
		  }
	  }

	  /// <summary>
	  /// Returns the file attribute (MBFATR).
	  /// 
	  /// </summary>
	  public virtual string FileAttribute
	  {
		  get
		  {
			return fileAttrib_;
		  }
	  }

	  /// <summary>
	  /// Returns the data type (MBDTAT).
	  /// 
	  /// </summary>
	  public virtual string DataType
	  {
		  get
		  {
			return dataType_;
		  }
	  }

	  /// <summary>
	  /// Returns the source system name (MBSYSN).
	  /// 
	  /// </summary>
	  public virtual string SystemName
	  {
		  get
		  {
			return systemName_;
		  }
	  }

	  /// <summary>
	  /// Returns the text description (MBTEXT) of the file.
	  /// 
	  /// </summary>
	  public virtual string FileText
	  {
		  get
		  {
			return fileText_;
		  }
	  }

	  /// <summary>
	  /// Returns the text description (MBMTXT) of the member.
	  /// 
	  /// </summary>
	  public virtual string MemberText
	  {
		  get
		  {
			return memberText_;
		  }
	  }

	  /// <summary>
	  /// Returns the record capacity (MBRCDC) of the member.
	  /// 
	  /// </summary>
	  public virtual long RecordCapacity
	  {
		  get
		  {
			return recordCapacity_;
		  }
	  }

	  /// <summary>
	  /// Returns the current number of records (MBNRCD) in the member.
	  /// 
	  /// </summary>
	  public virtual long RecordCount
	  {
		  get
		  {
			return currentRecords_;
		  }
	  }

	  /// <summary>
	  /// Returns the number of deleted records (MBNDTR) in the member.
	  /// 
	  /// </summary>
	  public virtual long DeletedRecordCount
	  {
		  get
		  {
			return deletedRecords_;
		  }
	  }
	}

}