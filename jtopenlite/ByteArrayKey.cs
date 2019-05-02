///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ByteArrayKey.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{
	/// <summary>
	/// Utility class for mapping byte array portions.
	/// 
	/// </summary>
	public sealed class ByteArrayKey
	{
	  private sbyte[] key_;
	  private int offset_;
	  private int length_;
	  private int hash_;

	  public ByteArrayKey()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public ByteArrayKey(final byte[] key)
	  public ByteArrayKey(sbyte[] key)
	  {
		key_ = key;
		offset_ = 0;
		length_ = key.Length;
		hash_ = computeHash(key, 0, key.Length);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final int computeHash(final byte[] key, final int offset, final int length)
	  private static int computeHash(sbyte[] key, int offset, int length)
	  {
	/*    int hash = 0;
	    for (int i=0; i<length_ && i<4; ++i)
	    {
	      hash = hash | key_[offset_+i];
	    }
	    if (length_ > 4)
	    {
	      for (int i=length_-4; i<length_; ++i)
	      {
	        hash = hash | key_[offset_+i];
	      }
	    }
	    hash_ = hash;
	*/
		return key[offset] + key[offset + length - 1] + key[offset + (length >> 1)];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setHashData(final byte[] data, final int offset, final int length)
	  public void setHashData(sbyte[] data, int offset, int length)
	  {
		key_ = data;
		offset_ = offset;
		length_ = length;
		hash_ = computeHash(data, offset, length);
	  }

	  public override int GetHashCode()
	  {
		return hash_;
	  }

	  public override bool Equals(object obj)
	  {
		if (obj != null && obj is ByteArrayKey)
		{
		  ByteArrayKey e = (ByteArrayKey)obj;
		  return this.matches(e.key_, e.offset_, e.length_);
		}
		return false;
	  }

	  public sbyte[] Key
	  {
		  get
		  {
			return key_;
		  }
	  }

	  public int Offset
	  {
		  get
		  {
			return offset_;
		  }
	  }

	  public int Length
	  {
		  get
		  {
			return length_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean matches(final byte[] data, final int offset, final int length)
	  public bool matches(sbyte[] data, int offset, int length)
	  {
		if (length_ != length)
		{
			return false;
		}
		for (int i = 0; i < length_; ++i)
		{
		  if (key_[offset_ + i] != data[offset + i])
		  {
			  return false;
		  }
		}
		return true;
	  }
	}


}