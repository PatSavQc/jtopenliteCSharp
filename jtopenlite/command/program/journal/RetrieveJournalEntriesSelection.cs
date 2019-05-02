using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  RetrieveJournalEntriesSelectionListener.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2014 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.journal
{

	/// <summary>
	/// Sample implementation of RetrieveJournalEntriesSelectionListener.
	/// 
	/// Note: This implementation does not check the validity of the input parameters. 
	/// Refer to the API documentation 
	/// (http://publib.boulder.ibm.com/infocenter/iseries/v5r4/index.jsp?topic=%2Fapis%2FQJORJRNE.htm) 
	/// for information about the parameters. 
	/// </summary>
	public class RetrieveJournalEntriesSelection : RetrieveJournalEntriesSelectionListener
	{
	   internal int allocatedSize;
	   internal int recordCount;
	   internal int[] keys;
	   internal sbyte[][] data;


	   public RetrieveJournalEntriesSelection()
	   {
		   allocatedSize = 10;
		   recordCount = 0;
		   keys = new int[allocatedSize];
		   data = new sbyte[allocatedSize][];
	   }

	   /// <summary>
	   /// check that entry can be safely added.  If not
	   /// expand the storage. 
	   /// </summary>
	   private void beforeAdd()
	   {
		 if (recordCount == allocatedSize)
		 {
			 int newAllocatedSize = allocatedSize + 10;
			 int[] newKeys = new int[newAllocatedSize];
			 sbyte[][] newData = new sbyte[newAllocatedSize][];

			 for (int i = 0 ; i < recordCount; i++)
			 {
				 newKeys[i] = keys[i];
				 newData[i] = data[i];
			 }
			 keys = newKeys;
			 data = newData;
			 allocatedSize = newAllocatedSize;

		 }
	   }

	   public virtual void addEntry(int key, int value)
	   {
		   lock (this)
		   {
			  beforeAdd();
			  keys[recordCount] = key;
			  data[recordCount] = Conv.intToByteArray(value);
			  recordCount++;
		   }
	   }

	   public virtual void addEntry(int key, string value)
	   {
		   lock (this)
		   {
			   beforeAdd();
				  keys[recordCount] = key;
				  data[recordCount] = Conv.stringToEBCDICByteArray37(value);
				  recordCount++;
        
		   }
	   }

	   public virtual void addEntry(int key, sbyte[] value)
	   {
		   lock (this)
		   {
			  beforeAdd();
			  keys[recordCount] = key;
			  data[recordCount] = value;
			  recordCount++;
		   }
	   }


	   public virtual int NumberOfVariableLengthRecords
	   {
		   get
		   {
			   lock (this)
			   {
				   return recordCount;
			   }
		   }
	   }
	   public virtual int getVariableLengthRecordKey(int index)
	   {
		   lock (this)
		   {
			   return keys[index];
		   }
	   }

	   public virtual int getVariableLengthRecordDataLength(int index)
	   {
		   lock (this)
		   {
			   return data[index].Length;
		   }
	   }

	   public virtual void setVariableLengthRecordData(int index, sbyte[] buffer, int offset)
	   {
		   lock (this)
		   {
			   sbyte[] fromData = data[index];
			   Array.Copy(fromData, 0, buffer, offset, fromData.Length);
		   }
	   }

	}

}