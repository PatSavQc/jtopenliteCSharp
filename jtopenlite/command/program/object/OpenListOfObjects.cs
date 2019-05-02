///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  OpenListOfObjects.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.@object
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;
	using com.ibm.jtopenlite.command.program.openlist;


	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qgyolobj.htm">QGYOLOBJ</a>
	/// 
	/// </summary>
	public class OpenListOfObjects : OpenListProgram<OpenListOfObjectsFormat, OpenListOfObjectsFormatListener>
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  private OpenListOfObjectsFormat formatter_;
	  private OpenListOfObjectsFormatListener formatListener_;
	  private int inputLength_;
	  private int numberOfRecordsToReturn_;
	  private SortListener sortListener_;
	  private string objectName_;
	  private string objectLibrary_;
	  private string objectType_;
	  private OpenListOfObjectsAuthorityListener authorityListener_;
	  private OpenListOfObjectsSelectionListener selectionListener_;
	  private int[] keys_;
	  private ListInformation info_;

	  private sbyte[] tempData_;

	  public OpenListOfObjects()
	  {
	  }

	  public OpenListOfObjects(OpenListOfObjectsFormat format, int lengthOfReceiverVariable, int numberOfRecordsToReturn, SortListener sortInformation, string objectName, string libraryName, string objectType, OpenListOfObjectsAuthorityListener authorityControl, OpenListOfObjectsSelectionListener selectionControl, int[] keysToReturn)
	  {
		formatter_ = format;
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		numberOfRecordsToReturn_ = numberOfRecordsToReturn;
		sortListener_ = sortInformation;
		objectName_ = objectName;
		objectLibrary_ = libraryName;
		objectType_ = objectType;
		authorityListener_ = authorityControl;
		selectionListener_ = selectionControl;
		keys_ = keysToReturn;
	  }

	  public sbyte[] TempDataBuffer
	  {
		  get
		  {
			int maxSize = 0;
			for (int i = 0; i < NumberOfParameters; ++i)
			{
			  int len = getParameterOutputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			  len = getParameterInputLength(i);
			  if (len > maxSize)
			  {
				  maxSize = len;
			  }
			}
			if (tempData_ == null || tempData_.Length < maxSize)
			{
			  tempData_ = new sbyte[maxSize];
			}
			return tempData_;
		  }
	  }

	  public virtual OpenListOfObjectsFormat Formatter
	  {
		  get
		  {
			return formatter_;
		  }
		  set
		  {
			formatter_ = value;
		  }
	  }


	  public virtual OpenListOfObjectsFormatListener FormatListener
	  {
		  get
		  {
			return formatListener_;
		  }
		  set
		  {
			formatListener_ = value;
		  }
	  }


	  public virtual string ProgramName
	  {
		  get
		  {
			return "QGYOLOBJ";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 12;
		  }
	  }

	  public virtual void newCall()
	  {
		info_ = null;
	  }

	  public virtual ListInformation ListInformation
	  {
		  get
		  {
			return info_;
		  }
	  }

	  public virtual int LengthOfReceiverVariable
	  {
		  get
		  {
			return inputLength_;
		  }
		  set
		  {
			inputLength_ = value;
		  }
	  }


	  public virtual int NumberOfRecordsToReturn
	  {
		  get
		  {
			return numberOfRecordsToReturn_;
		  }
		  set
		  {
			numberOfRecordsToReturn_ = value;
		  }
	  }


	  public virtual SortListener SortListener
	  {
		  get
		  {
			return sortListener_;
		  }
		  set
		  {
			sortListener_ = value;
		  }
	  }


	  public virtual string ObjectName
	  {
		  get
		  {
			return objectName_;
		  }
		  set
		  {
			objectName_ = value;
		  }
	  }


	  public virtual string ObjectLibrary
	  {
		  get
		  {
			return objectLibrary_;
		  }
		  set
		  {
			objectLibrary_ = value;
		  }
	  }


	  public virtual string ObjectType
	  {
		  get
		  {
			return objectType_;
		  }
		  set
		  {
			objectType_ = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterInputLength(final int parmIndex)
	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 0;
		  case 1:
			  return 4;
		  case 2:
			  return 0;
		  case 3:
			  return 4;
		  case 4:
			return sortListener_ == null ? 4 : 4 + (sortListener_.NumberOfSortKeys * 12);
		  case 5:
			  return 20;
		  case 6:
			  return 10;
		  case 7:
			return authorityListener_ == null ? 28 : 28 + (authorityListener_.NumberOfObjectAuthorities * 10) + (authorityListener_.NumberOfLibraryAuthorities * 10);
		  case 8:
			return selectionListener_ == null ? 21 : 20 + (selectionListener_.NumberOfStatuses);
		  case 9:
			  return 4;
		  case 10:
			return keys_ == null ? 0 : keys_.Length * 4;
		  case 11:
			  return 4;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterOutputLength(final int parmIndex)
	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return inputLength_;
		  case 2:
			  return 80;
		  case 11:
			  return 4;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
		  case 2:
			return Parameter.TYPE_OUTPUT;
		  case 11:
			return Parameter.TYPE_INPUT_OUTPUT;
		}
		return Parameter.TYPE_INPUT;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public byte[] getParameterInputData(final int parmIndex)
	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] tempData = getTempDataBuffer();
		sbyte[] tempData = TempDataBuffer;
		switch (parmIndex)
		{
		  case 1:
			  Conv.intToByteArray(inputLength_, tempData, 0);
			  return tempData;
		  case 3:
			  Conv.intToByteArray(numberOfRecordsToReturn_, tempData, 0);
			  return tempData;
		  case 4: // Sort information.
			if (sortListener_ == null)
			{
			  Conv.intToByteArray(0, tempData, 0);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numberOfKeys = sortListener_.getNumberOfSortKeys();
			  int numberOfKeys = sortListener_.NumberOfSortKeys;
			  Conv.intToByteArray(numberOfKeys, tempData, 0);
			  int offset = 4;
			  for (int i = 0; i < numberOfKeys; ++i)
			  {
				Conv.intToByteArray(sortListener_.getSortKeyFieldStartingPosition(i), tempData, offset);
				Conv.intToByteArray(sortListener_.getSortKeyFieldLength(i), tempData, offset + 4);
				Conv.shortToByteArray(sortListener_.getSortKeyFieldDataType(i), tempData, offset + 8);
				tempData[offset + 10] = sortListener_.isAscending(i) ? unchecked((sbyte)0xF1) : unchecked((sbyte)0xF2);
				tempData[offset + 11] = 0;
				offset += 12;
			  }
			}
			return tempData;
		  case 5:
			Conv.stringToBlankPadEBCDICByteArray(objectName_, tempData, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(objectLibrary_, tempData, 10, 10);
			return tempData;
		  case 6:
			Conv.stringToBlankPadEBCDICByteArray(objectType_, tempData, 0, 10);
			return tempData;
		  case 7: // Authority control
			if (authorityListener_ == null)
			{
			  Conv.intToByteArray(28, tempData, 0);
			  for (int i = 4; i < 28; ++i)
			  {
				  tempData[i] = 0;
			  }
			}
			else
			{
			  Conv.intToByteArray(getParameterInputLength(7), tempData, 0);
			  Conv.intToByteArray(authorityListener_.CallLevel, tempData, 4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numObjectAuthorities = authorityListener_.getNumberOfObjectAuthorities();
			  int numObjectAuthorities = authorityListener_.NumberOfObjectAuthorities;
			  int displacementToObjectAuthorities = numObjectAuthorities == 0 ? 0 : 28;
			  Conv.intToByteArray(displacementToObjectAuthorities, tempData, 8);
			  Conv.intToByteArray(numObjectAuthorities, tempData, 12);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numLibraryAuthorities = authorityListener_.getNumberOfLibraryAuthorities();
			  int numLibraryAuthorities = authorityListener_.NumberOfLibraryAuthorities;
			  int displacementToLibraryAuthorities = numLibraryAuthorities == 0 ? 0 : (28 + (numObjectAuthorities * 10));
			  Conv.intToByteArray(displacementToLibraryAuthorities, tempData, 16);
			  Conv.intToByteArray(numLibraryAuthorities, tempData, 20);
			  Conv.intToByteArray(0, tempData, 24); // Reserved.
			  int offset = 28;
			  for (int i = 0; i < numObjectAuthorities; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(authorityListener_.getObjectAuthority(i), tempData, offset, 10);
				offset += 10;
			  }
			  for (int i = 0; i < numLibraryAuthorities; ++i)
			  {
				Conv.stringToBlankPadEBCDICByteArray(authorityListener_.getLibraryAuthority(i), tempData, offset, 10);
				offset += 10;
			  }
			}
			return tempData;
		  case 8: // Selection control
			if (selectionListener_ == null)
			{
			  Conv.intToByteArray(21, tempData, 0);
			  Conv.intToByteArray(0, tempData, 4);
			  Conv.intToByteArray(20, tempData, 8);
			  Conv.intToByteArray(1, tempData, 12);
			  Conv.intToByteArray(0, tempData, 16);
			  Conv.stringToEBCDICByteArray37("*", tempData, 20);
			}
			else
			{
			  Conv.intToByteArray(getParameterInputLength(8), tempData, 0);
			  Conv.intToByteArray(selectionListener_.Selected ? 0 : 1, tempData, 4);
			  Conv.intToByteArray(20, tempData, 8);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numStatuses = selectionListener_.getNumberOfStatuses();
			  int numStatuses = selectionListener_.NumberOfStatuses;
			  Conv.intToByteArray(numStatuses, tempData, 12);
			  Conv.intToByteArray(0, tempData, 16); // Reserved.
			  int offset = 20;
			  for (int i = 0; i < numStatuses; ++i)
			  {
				Conv.stringToEBCDICByteArray37(selectionListener_.getStatus(i), tempData, offset++);
			  }
			}
			return tempData;
		  case 9:
			Conv.intToByteArray(keys_ == null ? 0 : keys_.Length, tempData, 0);
			return tempData;
		  case 10:
			if (keys_ != null)
			{
			  int offset = 0;
			  for (int i = 0; i < keys_.Length; ++i)
			  {
				Conv.intToByteArray(keys_[i], tempData, offset);
				offset += 4;
			  }
			}
			return tempData;
		  case 11:
			  return ZERO;
		}
		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 0:
			if (formatter_ != null)
			{
			  formatter_.format(data, maxLength, 0, formatListener_);
			}
			break;
		  case 2:
			if (maxLength < 12)
			{
			  info_ = null;
			}
			else
			{
			  info_ = Util.readOpenListInformationParameter(data, maxLength);
			}
			break;
		  default:
			break;
		}
	  }
	}


}