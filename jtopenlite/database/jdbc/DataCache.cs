using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DataCache.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{
	// Unnecessary include
	// import java.util.*;

	internal sealed class DataCache
	{
	  private int rowCount_;
	  private int columnCount_;
	  private int rowSize_;

	//  private int startingRow_; // Relative row number. In fact, should always be zero?
	//  private boolean startingRowSet_;
	  private sbyte[] data_;

	  private bool[] nullMap_;

	  private int currentRow_ = -1;

	  internal DataCache()
	  {
	  }

	  internal sbyte[] Data
	  {
		  get
		  {
			return data_;
		  }
	  }

	  internal int RowOffset
	  {
		  get
		  {
			return currentRow_ * rowSize_;
		  }
	  }

	  internal void init(int rowCount, int columnCount, int rowSize)
	  {
		rowCount_ = rowCount;
		columnCount_ = columnCount;
		rowSize_ = rowSize;
		int totalSize = rowCount * rowSize;
		if (data_ == null || data_.Length < totalSize)
		{
		  data_ = new sbyte[totalSize];
		}
		int totalNulls = rowCount * columnCount;
		if (nullMap_ == null || nullMap_.Length < totalNulls)
		{
		  nullMap_ = new bool[totalNulls];
		}
	//    startingRowSet_ = false;
		currentRow_ = -1;
	  }

	  internal void setNull(int row, int column, bool b)
	  {
	//    if (!startingRowSet_)
	//    {
	//      startingRow_ = row + (currentRow_ < 0 ? 0 : currentRow_);
	//      startingRowSet_ = true;
	//    }
		int offset = row * columnCount_;
		nullMap_[offset + column] = b;
	  }

	  //
	  // Is the column null.  The column number is 0 based
	  // 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isNull(int column) throws SQLException
	  internal bool isNull(int column)
	  {
		// Validate the column number
	   if (column < 0 || column >= columnCount_)
	   {
		   throw new SQLException("Descriptor index (" + column + "/" + columnCount_ + ")not valid.");
	   }
		int offset = currentRow_ * columnCount_;
		return nullMap_[offset + column];
	  }

	  internal void setRow(int row, sbyte[] tempData)
	  {
	//    if (!startingRowSet_)
	//    {
	//      startingRow_ = row + (currentRow_ < 0 ? 0 : currentRow_);
	//      startingRowSet_ = true;
	//    }
		int offset = row * rowSize_;
		Array.Copy(tempData, 0, data_, offset, rowSize_);
	//    System.out.println("setRow: "+row);
	  }

	//  int getStartingRow()
	//  {
	//    return startingRow_;
	//  }

	  internal int NumRows
	  {
		  get
		  {
			return rowCount_;
		  }
	  }

	  internal int CurrentRow
	  {
		  get
		  {
			return currentRow_;
		  }
	  }

	  internal int nextRow()
	  {
		return ++currentRow_;
	  }
	}


}