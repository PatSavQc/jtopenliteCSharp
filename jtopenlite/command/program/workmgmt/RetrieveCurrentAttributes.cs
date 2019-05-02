///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveCurrentAttributes.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.workmgmt
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qwcrtvca.htm">QWCRTVCA</a>
	/// 
	/// </summary>
	public class RetrieveCurrentAttributes : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const int FORMAT_RTVC0100 = 0;
	  public const int FORMAT_RTVC0200 = 1;
	  public const int FORMAT_RTVC0300 = 2;

	  private int inputFormat_;
	  private int inputLength_;
	  private int[] attributesToReturn_;

	  private int numberOfAttributesReturned_;

	  private int bytesReturned_;
	  private int bytesAvailable_;

	  private int numberOfLibrariesInSYSLIBL_;
	  private int numberOfProductLibraries_;
	  private bool currentLibraryExistence_;
	  private int numberOfLibrariesInUSRLIBL_;

	  private int numberOfASPGroups_;

	  private JobKeyDataListener keyDataListener_;
	  private RetrieveCurrentAttributesLibraryListener libraryListener_;
	  private RetrieveCurrentAttributesASPGroupListener aspGroupListener_;

	  private sbyte[] tempData_;

	  public RetrieveCurrentAttributes(int format, int lengthOfReceiverVariable, int[] attributesToReturn)
	  {
		inputFormat_ = format;
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		attributesToReturn_ = attributesToReturn == null ? new int[0] : attributesToReturn;
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

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QWCRTVCA";
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
			return 6;
		  }
	  }

	  public virtual void newCall()
	  {
		numberOfAttributesReturned_ = 0;
		bytesReturned_ = 0;
		bytesAvailable_ = 0;
		numberOfLibrariesInSYSLIBL_ = 0;
		numberOfProductLibraries_ = 0;
		currentLibraryExistence_ = false;
		numberOfLibrariesInUSRLIBL_ = 0;
		numberOfASPGroups_ = 0;
	  }

	  public virtual int Format
	  {
		  set
		  {
			inputFormat_ = value;
		  }
		  get
		  {
			return inputFormat_;
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
			inputLength_ = value <= 0 ? 1 : value;
		  }
	  }


	  public virtual int[] AttributesToReturn
	  {
		  get
		  {
			return attributesToReturn_;
		  }
		  set
		  {
			attributesToReturn_ = value == null ? new int[0] : value;
		  }
	  }


	  public virtual int NumberOfAttributesReturned
	  {
		  get
		  {
			return numberOfAttributesReturned_;
		  }
	  }

	  public virtual int BytesAvailable
	  {
		  get
		  {
			return bytesAvailable_;
		  }
	  }

	  public virtual int BytesReturned
	  {
		  get
		  {
			return bytesReturned_;
		  }
	  }

	  public virtual int NumberOfSystemLibraries
	  {
		  get
		  {
			return numberOfLibrariesInSYSLIBL_;
		  }
	  }

	  public virtual int NumberOfProductLibraries
	  {
		  get
		  {
			return numberOfProductLibraries_;
		  }
	  }

	  public virtual bool hasCurrentLibrary()
	  {
		return currentLibraryExistence_;
	  }

	  public virtual int NumberOfUserLibraries
	  {
		  get
		  {
			return numberOfLibrariesInUSRLIBL_;
		  }
	  }

	  public virtual int NumberOfASPGroups
	  {
		  get
		  {
			return numberOfASPGroups_;
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
			  return 8;
		  case 3:
			  return 4;
		  case 4:
			  return 4 * attributesToReturn_.Length;
		  case 5:
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
		  case 5:
			  return 4;
		}
		return 0;
	  }

	  private string FormatName
	  {
		  get
		  {
			switch (inputFormat_)
			{
			  case FORMAT_RTVC0100:
				  return "RTVC0100";
			  case FORMAT_RTVC0200:
				  return "RTVC0200";
			  case FORMAT_RTVC0300:
				  return "RTVC0300";
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParameterType(final int parmIndex)
	  public virtual int getParameterType(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return Parameter.TYPE_OUTPUT;
		  case 5:
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
		  case 2:
			  Conv.stringToEBCDICByteArray37(FormatName, tempData, 0);
			  return tempData;
		  case 3:
			  Conv.intToByteArray(attributesToReturn_.Length, tempData, 0);
			  return tempData;
		  case 4:
			for (int i = 0; i < attributesToReturn_.Length; ++i)
			{
			  Conv.intToByteArray(attributesToReturn_[i], tempData, i * 4);
			}
			return tempData;
		  case 5:
			  return ZERO;
		}
		return null;
	  }

	  public virtual JobKeyDataListener KeyDataListener
	  {
		  set
		  {
			keyDataListener_ = value;
		  }
	  }

	  public virtual RetrieveCurrentAttributesLibraryListener LibraryListener
	  {
		  set
		  {
			libraryListener_ = value;
		  }
	  }

	  public virtual RetrieveCurrentAttributesASPGroupListener ASPGroupListener
	  {
		  set
		  {
			aspGroupListener_ = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] c = new char[20];
		char[] c = new char[20];
		switch (parmIndex)
		{
		  case 0:
			int numRead = 0;
			switch (inputFormat_)
			{
			  case FORMAT_RTVC0100:
				if (maxLength >= 4)
				{
				  numberOfAttributesReturned_ = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				}
				if (keyDataListener_ != null)
				{
				  for (int i = 0; i < numberOfAttributesReturned_ && numRead + 16 <= maxLength; ++i)
				  {
					int lengthOfAttributeInfoReturned = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int key = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					int typeOfData = data[numRead] & 0x00FF;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isBinary = typeOfData == 0x00C2;
					bool isBinary = typeOfData == 0x00C2;
					numRead += 4;
					int lengthOfData = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					if (numRead + lengthOfData <= maxLength)
					{
					  Util.readKeyData(data, numRead, key, lengthOfData, isBinary, keyDataListener_, c);
					  numRead += lengthOfData;
					  int skip = lengthOfAttributeInfoReturned - 16 - lengthOfData;
					  if (numRead + skip <= maxLength)
					  {
						numRead += skip;
					  }
					  else
					  {
						numRead = maxLength;
					  }
					}
					else
					{
					  numRead = maxLength;
					}
				  }
				}
				break;

			  case FORMAT_RTVC0200:
				if (maxLength >= 8)
				{
				  bytesReturned_ = Conv.byteArrayToInt(data, 0);
				  bytesAvailable_ = Conv.byteArrayToInt(data, 4);
				  numRead += 8;
				}
				if (maxLength >= 24)
				{
				  numberOfLibrariesInSYSLIBL_ = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				  numberOfProductLibraries_ = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				  currentLibraryExistence_ = Conv.byteArrayToInt(data, numRead) == 1;
				  numRead += 4;
				  numberOfLibrariesInUSRLIBL_ = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				  if (libraryListener_ != null)
				  {
					for (int i = 0; i < numberOfLibrariesInSYSLIBL_ && numRead + 11 <= maxLength; ++i)
					{
					  string lib = Conv.ebcdicByteArrayToString(data, numRead, 11, c);
					  numRead += 11;
					  libraryListener_.newSystemLibrary(lib);
					}
					for (int i = 0; i < numberOfProductLibraries_ && numRead + 11 <= maxLength; ++i)
					{
					  string lib = Conv.ebcdicByteArrayToString(data, numRead, 11, c);
					  numRead += 11;
					  libraryListener_.newProductLibrary(lib);
					}
					if (currentLibraryExistence_ && numRead + 11 <= maxLength)
					{
					  string lib = Conv.ebcdicByteArrayToString(data, numRead, 11, c);
					  numRead += 11;
					  libraryListener_.currentLibrary(lib);
					}
					for (int i = 0; i < numberOfLibrariesInUSRLIBL_ && numRead + 11 <= maxLength; ++i)
					{
					  string lib = Conv.ebcdicByteArrayToString(data, numRead, 11, c);
					  numRead += 11;
					  libraryListener_.newUserLibrary(lib);
					}
				  }
				}
				break;

			  case FORMAT_RTVC0300:
				if (maxLength >= 8)
				{
				  bytesReturned_ = Conv.byteArrayToInt(data, 0);
				  bytesAvailable_ = Conv.byteArrayToInt(data, 4);
				  numRead += 8;
				}
				if (maxLength >= 20)
				{
				  int offsetToASPGroupInformation = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				  numberOfASPGroups_ = Conv.byteArrayToInt(data, numRead);
				  numRead += 4;
				  int lengthOfASPGroupEntry = Conv.byteArrayToInt(data, numRead);
				  numRead += 8;
				  if (aspGroupListener_ != null)
				  {
					int skip = offsetToASPGroupInformation - 20;
					if (numRead + skip <= maxLength)
					{
					  numRead += skip;
					  for (int i = 0; i < numberOfASPGroups_ && numRead + 10 <= maxLength; ++i)
					  {
						string aspGroupName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
						aspGroupListener_.newASPGroup(aspGroupName);
						skip = lengthOfASPGroupEntry - 10;
						if (numRead + skip <= maxLength)
						{
						  numRead += skip;
						}
						else
						{
						  numRead = maxLength;
						}
					  }
					}
				  }
				}
				break;
			}
			break;

		  default:
			break;
		}
	  }
	}


}