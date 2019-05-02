using System.Text;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DiskStatus.java
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
	/// Represents disk status information returned by the ListDiskStatuses class.
	/// The toString() method will print various fields in a format similar to
	/// what WRKDSKSTS does.
	/// 
	/// </summary>
	public sealed class DiskStatus
	{
	  private readonly string unit_;
	  private readonly string type_;
	  private readonly string size_;
	  private readonly string percentUsed_;
	  private readonly string ioRequests_;
	  private readonly string requestSize_;
	  private readonly string readRequests_;
	  private readonly string writeRequests_;
	  private readonly string read_;
	  private readonly string write_;
	  private readonly string percentBusy_;
	  private readonly string asp_;
	  private readonly string protectionType_;
	  private readonly string protectionStatus_;
	  private readonly string compression_;

	  internal DiskStatus(string unit, string type, string sizeMB, string percentUsed, string ioRequests, string requestSizeKB, string readRequests, string writeRequests, string readKB, string writeKB, string percentBusy, string asp, string protectionType, string protectionStatus, string compression)
	  {
		unit_ = unit;
		type_ = type;
		size_ = sizeMB;
		percentUsed_ = percentUsed;
		ioRequests_ = ioRequests;
		requestSize_ = requestSizeKB;
		readRequests_ = readRequests;
		writeRequests_ = writeRequests;
		read_ = readKB;
		write_ = writeKB;
		percentBusy_ = percentBusy;
		asp_ = asp;
		protectionType_ = protectionType;
		protectionStatus_ = protectionStatus;
		compression_ = compression;
	  }

	  public string Unit
	  {
		  get
		  {
			return unit_;
		  }
	  }

	  public string Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  public string Size
	  {
		  get
		  {
			return size_;
		  }
	  }

	  public string PercentUsed
	  {
		  get
		  {
			return percentUsed_;
		  }
	  }

	  public string IORequests
	  {
		  get
		  {
			return ioRequests_;
		  }
	  }

	  public string RequestSize
	  {
		  get
		  {
			return requestSize_;
		  }
	  }

	  public string ReadRequests
	  {
		  get
		  {
			return readRequests_;
		  }
	  }

	  public string WriteRequests
	  {
		  get
		  {
			return writeRequests_;
		  }
	  }

	  public string ReadKB
	  {
		  get
		  {
			return read_;
		  }
	  }

	  public string WriteKB
	  {
		  get
		  {
			return write_;
		  }
	  }

	  public string PercentBusy
	  {
		  get
		  {
			return percentBusy_;
		  }
	  }

	  public string ASP
	  {
		  get
		  {
			return asp_;
		  }
	  }

	  public string ProtectionType
	  {
		  get
		  {
			return protectionType_;
		  }
	  }

	  public string ProtectionStatus
	  {
		  get
		  {
			return protectionStatus_;
		  }
	  }

	  public string Compression
	  {
		  get
		  {
			return compression_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private final DiskStatus print(final StringBuffer buf, final String value)
	  private DiskStatus print(StringBuilder buf, string value)
	  {
		buf.Append(" ");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int num = 5-value.length();
		int num = 5 - value.Length;
		for (int i = 0; i < num; ++i)
		{
		  buf.Append(" ");
		}
		buf.Append(value);
		return this;
	  }

	  public override string ToString()
	  {
		StringBuilder buf = new StringBuilder();
		int num = 2 - unit_.Length;
		buf.Append(" ");
		for (int i = 0; i < num; ++i)
		{
		  buf.Append(" ");
		}
		buf.Append(unit_).Append("  ").Append(type_).Append("  ").Append(size_);
		print(buf, percentUsed_).print(buf, ioRequests_).print(buf, requestSize_);
		print(buf, readRequests_).print(buf, writeRequests_);
		print(buf, read_).print(buf, write_);
		buf.Append(" ");
		num = 3 - percentBusy_.Length;
		for (int i = 0; i < num; ++i)
		{
		  buf.Append(" ");
		}
		buf.Append(percentBusy_);
		buf.Append("  ").Append(asp_);
		print(buf, protectionType_).print(buf, protectionStatus_);
		if (!string.ReferenceEquals(compression_, null))
		{
		  buf.Append(" ");
		  buf.Append(compression_);
		}
		return buf.ToString();
	  }
	}


}