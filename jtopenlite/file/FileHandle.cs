using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  FileHandle.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.file
{
	public sealed class FileHandle : IComparable<FileHandle>
	{
	  public const int OPEN_READ_ONLY = 1;
	  public const int OPEN_WRITE_ONLY = 2;
	  public const int OPEN_READ_WRITE = 3;

	  public const int SHARE_ALL = 0;
	  public const int SHARE_WRITE = 1;
	  public const int SHARE_READ = 2;
	  public const int SHARE_NONE = 3;

	  private string name_;
	  private int openType_;
	  private int share_;
	  private bool open_;
	  private int handle_;
	  private long id_;
	  private int dataCCSID_;
	  private long created_;
	  private long modified_;
	  private long accessed_;
	  private long size_;
	  private int version_;

	  private int lastStatus_;

	  private long currentOffset_ = 0;

	  private bool symlink_;
	  private bool directory_;
	  private string path_;

	  private FileHandle()
	  {
	  }

	  public static FileHandle createEmptyHandle()
	  {
		return new FileHandle();
	  }

	  public override bool Equals(object obj)
	  {
		if (obj != null && obj is FileHandle)
		{
		  FileHandle h = (FileHandle)obj;
		  return h.handle_ == this.handle_ && h.id_ == this.id_ && h.size_ == this.size_ && ((string.ReferenceEquals(h.Path, null) && string.ReferenceEquals(this.Path, null)) || (!string.ReferenceEquals(h.Path, null) && !string.ReferenceEquals(this.Path, null) && h.Path.Equals(this.Path)));
		}
		return false;
	  }

	  public override int GetHashCode()
	  {
		return string.ReferenceEquals(Path, null) ? base.GetHashCode() : Path.GetHashCode();
	  }

	  public int CompareTo(FileHandle h)
	  {
		string p1 = this.Path;
		string p2 = h.Path;
		if (string.ReferenceEquals(p1, null) && string.ReferenceEquals(p2, null))
		{
			return 0;
		}
		if (string.ReferenceEquals(p1, null))
		{
			return -1;
		}
		if (string.ReferenceEquals(p2, null))
		{
			return 1;
		}
		return p1.CompareTo(p2);
	  }

	  internal bool Symlink
	  {
		  set
		  {
			symlink_ = value;
		  }
		  get
		  {
			return symlink_;
		  }
	  }


	  internal bool Directory
	  {
		  set
		  {
			directory_ = value;
		  }
		  get
		  {
			return directory_;
		  }
	  }


	  internal string Path
	  {
		  set
		  {
			path_ = value;
		  }
		  get
		  {
			return string.ReferenceEquals(path_, null) ? name_ : path_;
		  }
	  }


	  /// <summary>
	  /// Returns the current byte offset in the file where the next read or write operation will take place.
	  /// 
	  /// </summary>
	  public long Offset
	  {
		  get
		  {
			return currentOffset_;
		  }
		  set
		  {
			currentOffset_ = value;
		  }
	  }


	  internal int ShareOption
	  {
		  set
		  {
			share_ = value;
		  }
		  get
		  {
			return share_;
		  }
	  }


	  internal int OpenType
	  {
		  set
		  {
			openType_ = value;
		  }
		  get
		  {
			return openType_;
		  }
	  }


	  internal bool Open
	  {
		  set
		  {
			open_ = value;
		  }
		  get
		  {
			return open_;
		  }
	  }


	  internal string Name
	  {
		  set
		  {
			name_ = value;
		  }
		  get
		  {
			return name_;
		  }
	  }


	  internal int Handle
	  {
		  set
		  {
			handle_ = value;
		  }
		  get
		  {
			return handle_;
		  }
	  }


	  internal long ID
	  {
		  set
		  {
			id_ = value;
		  }
		  get
		  {
			return id_;
		  }
	  }


	  internal int DataCCSID
	  {
		  set
		  {
			dataCCSID_ = value;
		  }
		  get
		  {
			return dataCCSID_;
		  }
	  }


	  internal long CreateDate
	  {
		  set
		  {
			created_ = value;
		  }
	  }

	  public long TimestampCreated
	  {
		  get
		  {
			return created_;
		  }
	  }

	  internal long ModifyDate
	  {
		  set
		  {
			modified_ = value;
		  }
	  }

	  public long TimestampModified
	  {
		  get
		  {
			return modified_;
		  }
	  }

	  internal long AccessDate
	  {
		  set
		  {
			accessed_ = value;
		  }
	  }

	  public long TimestampAccessed
	  {
		  get
		  {
			return accessed_;
		  }
	  }

	  internal long Size
	  {
		  set
		  {
			size_ = value;
		  }
		  get
		  {
			return size_;
		  }
	  }


	  internal int Version
	  {
		  set
		  {
			version_ = value;
		  }
		  get
		  {
			return version_;
		  }
	  }


	  public int LastStatus
	  {
		  get
		  {
			return lastStatus_;
		  }
		  set
		  {
			lastStatus_ = value;
		  }
	  }


	  public override string ToString()
	  {
		string path = Path;
		return string.ReferenceEquals(path, null) ? base.ToString() : path;
	  }
	}

}