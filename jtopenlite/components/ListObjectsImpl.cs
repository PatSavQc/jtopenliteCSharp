///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  ListObjectsImpl.java
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
	using com.ibm.jtopenlite.command.program.@object;
	using OpenListHandler = com.ibm.jtopenlite.command.program.openlist.OpenListHandler;

	internal class ListObjectsImpl : OpenListOfObjectsFormatListener, OpenListOfObjectsSelectionListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			objectList_ = new OpenListOfObjects(format_, 8, 1, null, null, null, null, null, this, KEYS);
			handler_ = new OpenListHandler(objectList_, format_, this);
		}

	  private static readonly int[] KEYS = new int[] {202, 203}; // Extended object attribute, text description
	  private readonly OpenListOfObjectsFormat format_ = new OpenListOfObjectsFormat();
	  private OpenListOfObjects objectList_;
	  private OpenListHandler handler_;

	  private ObjectInfo[] objects_;
	  private int counter_ = -1;

	  private readonly char[] charBuffer_ = new char[50];

	  public ListObjectsImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }
	  public virtual void openComplete()
	  {
	  }

	  public virtual void totalRecordsInList(int total)
	  {
		objects_ = new ObjectInfo[total];
		counter_ = -1;
	  }

	  public virtual bool stopProcessing()
	  {
		return false;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObjectInfo[] getObjects(final CommandConnection conn, String name, String library, String type) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual ObjectInfo[] getObjects(CommandConnection conn, string name, string library, string type)
	  {
		objectList_.ObjectName = name;
		objectList_.ObjectLibrary = library;
		objectList_.ObjectType = type;
		objects_ = null;
		  counter_ = -1;
		handler_.process(conn, 2800);
		  return objects_;
	  }

	  ////////////////////////////////////////
	  //
	  // Selection methods.
	  //
	  ////////////////////////////////////////

	  public virtual bool Selected
	  {
		  get
		  {
			return false;
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
		return "A"; // Omit objects we do not have authority to.
	  }

	/*  public int getCallLevel()
	  {
	    return 0;
	  }
	
	  public int getNumberOfObjectAuthorities()
	  {
	    return 1;
	  }
	
	  public int getNumberOfLibraryAuthorities()
	  {
	    return 0;
	  }
	
	  public String getObjectAuthority(int index)
	  {
	    return "*OBJEXIST";
	  }
	
	  public String getLibraryAuthority(int index)
	  {
	    return null;
	  }
	*/
	  ////////////////////////////////////////
	  //
	  // List entry format methods.
	  //
	  ////////////////////////////////////////

	  public virtual void newObjectEntry(string objectName, string objectLibrary, string objectType, string informationStatus, int numFields)
	  {
		objects_[++counter_] = new ObjectInfo(objectName, objectLibrary, objectType, informationStatus);
	  }

	  public virtual void newObjectFieldData(int lengthOfFieldInfo, int key, string type, int dataLength, int dataOffset, sbyte[] data)
	  {
		switch (key)
		{
		  case 202:
			string attribute = isBlank(data, dataOffset) ? blankAttribute_ : Conv.ebcdicByteArrayToString(data, dataOffset, dataLength, charBuffer_);
			objects_[counter_].Attribute = attribute;
			break;
		  case 203:
			string description = Conv.ebcdicByteArrayToString(data, dataOffset, dataLength, charBuffer_);
			objects_[counter_].TextDescription = description;
			break;
		}
	  }

	  private const string blankAttribute_ = "          ";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private boolean isBlank(final byte[] data, final int numRead)
	  private bool isBlank(sbyte[] data, int numRead)
	  {
		int stop = numRead + 10;
		for (int i = numRead; i < stop; ++i)
		{
		  if (data[i] != 0x40)
		  {
			  return false;
		  }
		}
		return true;
	  }
	}

}