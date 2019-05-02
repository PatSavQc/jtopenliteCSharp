///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename: RetrieveAuthorizedUsers.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////
namespace com.ibm.jtopenlite.command.program.security
{
	using com.ibm.jtopenlite;
	using com.ibm.jtopenlite.command;

	/// <summary>
	/// Use the 
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/qsyrautu.htm">
	/// QSYRAUTU</a>
	/// API to retrieve authorized users. 
	/// 
	/// To utilize the output of the API, the user of this class must implement a 
	/// RetrieveAuthorizedUsersListener to process the API output. 
	/// 
	/// </summary>
	public class RetrieveAuthorizedUsers : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const int FORMAT_AUTU0100 = 0;
	  public const int FORMAT_AUTU0150 = 1;
	  public const int FORMAT_AUTU0200 = 2;
	  public const int FORMAT_AUTU0250 = 3;

	  public const string SELECTION_ALL = "*ALL";
	  public const string SELECTION_USER = "*USER";
	  public const string SELECTION_GROUP = "*GROUP";
	  public const string SELECTION_MEMBER = "*MEMBER";

	  public const string STARTING_PROFILE_FIRST = "*FIRST";

	  public const string GROUP_NONE = "*NONE";
	  public const string GROUP_NO_GROUP = "*NOGROUP";

	  public const string ENDING_PROFILE_LAST = "*LAST";

	  private int inputFormat_;
	  private int inputLength_;
	  private string inputSelection_;
	  private string inputStart_;
	  private bool inputIncludeStart_;
	  private string inputGroup_;
	  private string inputEnd_;

	  private int bytesReturned_;
	  private int bytesAvailable_;
	  private int numberOfProfileNames_;

	  private RetrieveAuthorizedUsersListener listener_;

	  private sbyte[] tempData_;

	  public RetrieveAuthorizedUsers(int format, int lengthOfReceiverVariable, string selectionCriteria, string startingProfileName, bool includeStartingProfile, string groupProfileName, string endingProfileName)
	  {
		inputFormat_ = format;
		inputLength_ = lengthOfReceiverVariable <= 0 ? 1 : lengthOfReceiverVariable;
		inputSelection_ = string.ReferenceEquals(selectionCriteria, null) ? SELECTION_ALL : selectionCriteria;
		inputStart_ = string.ReferenceEquals(startingProfileName, null) ? STARTING_PROFILE_FIRST : startingProfileName;
		inputIncludeStart_ = includeStartingProfile;
		inputGroup_ = string.ReferenceEquals(groupProfileName, null) ? GROUP_NONE : groupProfileName;
		inputEnd_ = endingProfileName;
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
			return "QSYRAUTU";
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
			return (string.ReferenceEquals(inputEnd_, null) ? 9 : 10);
		  }
	  }

	  public virtual void newCall()
	  {
		bytesReturned_ = 0;
		bytesAvailable_ = 0;
		numberOfProfileNames_ = 0;
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


	  public virtual int BytesReturned
	  {
		  get
		  {
			return bytesReturned_;
		  }
	  }

	  public virtual int BytesAvailable
	  {
		  get
		  {
			return bytesAvailable_;
		  }
	  }

	  public virtual int NumberOfProfileNames
	  {
		  get
		  {
			return numberOfProfileNames_;
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
			  return 8;
		  case 4:
			  return 10;
		  case 5:
			  return 10;
		  case 6:
			  return 1;
		  case 7:
			  return 10;
		  case 8:
			  return 4;
		  case 9:
			  return 10;
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
			  return 16;
		  case 8:
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
			  case FORMAT_AUTU0100:
				  return "AUTU0100";
			  case FORMAT_AUTU0150:
				  return "AUTU0150";
			  case FORMAT_AUTU0200:
				  return "AUTU0200";
			  case FORMAT_AUTU0250:
				  return "AUTU0250";
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
		  case 2:
			  return Parameter.TYPE_OUTPUT;
		  case 8:
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
			  Conv.stringToEBCDICByteArray37(FormatName, tempData, 0);
			  return tempData;
		  case 4:
			  Conv.stringToBlankPadEBCDICByteArray(inputSelection_, tempData, 0, 10);
			  return tempData;
		  case 5:
			  Conv.stringToBlankPadEBCDICByteArray(inputStart_, tempData, 0, 10);
			  return tempData;
		  case 6:
			  tempData[0] = inputIncludeStart_ ? unchecked((sbyte)0xF1) : unchecked((sbyte)0xF0);
			  return tempData;
		  case 7:
			  Conv.stringToBlankPadEBCDICByteArray(inputGroup_, tempData, 0, 10);
			  return tempData;
		  case 8:
			  return ZERO;
		  case 9:
			  Conv.stringToBlankPadEBCDICByteArray(inputEnd_, tempData, 0, 10);
			  return tempData;
		}
		return null;
	  }

	  public virtual RetrieveAuthorizedUsersListener Listener
	  {
		  set
		  {
			listener_ = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setParameterOutputData(final int parmIndex, final byte[] data, final int maxLength)
	  public virtual void setParameterOutputData(int parmIndex, sbyte[] data, int maxLength)
	  {
		switch (parmIndex)
		{
		  case 2:
			bytesReturned_ = Conv.byteArrayToInt(data, 0);
			bytesAvailable_ = Conv.byteArrayToInt(data, 4);
			numberOfProfileNames_ = Conv.byteArrayToInt(data, 8);
			// int entryLength = Conv.byteArrayToInt(data, 12);
			break;
		  case 0:
			if (listener_ == null)
			{
			  return;
			}
			int numRead = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] c = new char[50];
			char[] c = new char[50];
			while (numRead < maxLength)
			{
			  int remaining = maxLength - numRead;
			  switch (inputFormat_)
			  {
				case FORMAT_AUTU0100:
				  if (remaining >= 12)
				  {
					string profileName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					int userOrGroupIndicator = data[numRead++] & 0x00FF;
					int groupMembersIndicator = data[numRead++] & 0x00FF;
					listener_.newEntry(profileName, userOrGroupIndicator == 0x00F1, groupMembersIndicator == 0x00F1, null, null);
				  }
				  else
				  {
					numRead += remaining;
				  }
				  break;
				case FORMAT_AUTU0150:
				  if (remaining >= 62)
				  {
					string profileName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					int userOrGroupIndicator = data[numRead++] & 0x00FF;
					int groupMembersIndicator = data[numRead++] & 0x00FF;
					string textDescription = Conv.ebcdicByteArrayToString(data, numRead, 50, c);
					numRead += 50;
					listener_.newEntry(profileName, userOrGroupIndicator == 0x00F1, groupMembersIndicator == 0x00F1, textDescription, null);
				  }
				  else
				  {
					numRead += remaining;
				  }
				  break;
				case FORMAT_AUTU0200:
				  if (remaining >= 176)
				  {
					string profileName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					int userOrGroupIndicator = data[numRead++] & 0x00FF;
					int groupMembersIndicator = data[numRead++] & 0x00FF;
					int numberOfGroupProfiles = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					string[] groupProfiles = new string[numberOfGroupProfiles];
					if (numberOfGroupProfiles > 0)
					{
					  for (int i = 0; i < numberOfGroupProfiles; ++i)
					  {
						groupProfiles[i] = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
						numRead += 10;
					  }
					  numRead += 10 * (16 - numberOfGroupProfiles);
					}
					else
					{
					  numRead += 160;
					}
					listener_.newEntry(profileName, userOrGroupIndicator == 0x00F1, groupMembersIndicator == 0x00F1, null, groupProfiles);
				  }
				  else
				  {
					numRead += remaining;
				  }
				  break;
				case FORMAT_AUTU0250:
				  if (remaining >= 228)
				  {
					string profileName = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
					numRead += 10;
					int userOrGroupIndicator = data[numRead++] & 0x00FF;
					int groupMembersIndicator = data[numRead++] & 0x00FF;
					string textDescription = Conv.ebcdicByteArrayToString(data, numRead, 50, c);
					numRead += 50;
					numRead += 2;
					int numberOfGroupProfiles = Conv.byteArrayToInt(data, numRead);
					numRead += 4;
					string[] groupProfiles = new string[numberOfGroupProfiles];
					if (numberOfGroupProfiles > 0)
					{
					  groupProfiles = new string[numberOfGroupProfiles];
					  for (int i = 0; i < numberOfGroupProfiles; ++i)
					  {
						groupProfiles[i] = Conv.ebcdicByteArrayToString(data, numRead, 10, c);
						numRead += 10;
					  }
					  numRead += 10 * (16 - numberOfGroupProfiles);
					}
					else
					{
					  numRead += 160;
					}
					listener_.newEntry(profileName, userOrGroupIndicator == 0x00F1, groupMembersIndicator == 0x00F1, textDescription, groupProfiles);
				  }
				  else
				  {
					numRead += remaining;
				  }
				  break;
				default:
				  numRead += remaining;
				  break;
			  }
			}
			break;
		  default:
			break;
		}
	  }
	}


}