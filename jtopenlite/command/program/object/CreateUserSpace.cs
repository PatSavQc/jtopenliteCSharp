///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  CreateUserSpace.java
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

	/// <summary>
	/// <para> Call the QUSCRTUS API, 
	/// <a href="http://publib.boulder.ibm.com/infocenter/iseries/v5r4/topic/apis/quscrtus.htm">QUSCRTUS</a>
	/// 
	/// </para>
	/// <para>This class is used with a CommandConnection to create a user space
	/// 
	/// </para>
	/// <para> Sample code
	/// <pre>
	/// public static void main(String[] args) {
	///   try { 
	///     CommandConnection connection = CommandConnection.getConnection(args[0],  args[1],  args[2]);
	///     CreateUserSpace createUserSpace = new CreateUserSpace(
	///                       args[4], // userSpaceName 
	///                       args[3], // userSpaceLibrary 
	///                       CreateUserSpace.EXTENDED_ATTRIBUTE_NONE,      // extendedAttribute 
	///                       100,     // initialSize       
	///                       CreateUserSpace.INITIAL_VALUE_BEST_PERFORMANCE, // initialValue 
	///                       CreateUserSpace.PUBLIC_AUTHORITY_USE, // publicAuthority 
	///                       "", //textDescription 
	///                       CreateUserSpace.REPLACE_NO,  //replace 
	///                       CreateUserSpace.DOMAIN_DEFAULT, //domain 
	///                       CreateUserSpace.TRANSFER_SIZE_REQUEST_DEFAULT, //transferSizeRequest 
	///                       CreateUserSpace.OPTIMUM_SPACE_ALIGNMENT_YES // optimumSpaceAlignment 
	///                    );
	/// 
	///     CommandResult result = connection.call(createUserSpace); 
	///     System.out.println("Command completed with "+result); 
	///   } catch (Exception e) {
	///     e.printStackTrace(System.out); 
	///   }
	/// }
	/// 
	/// </pre> 
	/// 
	/// </para>
	/// </summary>
	public class CreateUserSpace : Program
	{
	  private static readonly sbyte[] ZERO = new sbyte[4];

	  public const string EXTENDED_ATTRIBUTE_NONE = "";

	  public const int INITIAL_SIZE_MAX = 16776704;

	  public const sbyte INITIAL_VALUE_BEST_PERFORMANCE = 0;

	  public const string PUBLIC_AUTHORITY_ALL = "*ALL";
	  public const string PUBLIC_AUTHORITY_CHANGE = "*CHANGE";
	  public const string PUBLIC_AUTHORITY_EXCLUDE = "*EXCLUDE";
	  public const string PUBLIC_AUTHORITY_LIBCRTAUT = "*LIBCRTAUT";
	  public const string PUBLIC_AUTHORITY_USE = "*USE";

	  public const string REPLACE_YES = "*YES";
	  public const string REPLACE_NO = "*NO";

	  public const string DOMAIN_DEFAULT = "*DEFAULT";
	  public const string DOMAIN_SYSTEM = "*SYSTEM";
	  public const string DOMAIN_USER = "*USER";

	  public const int TRANSFER_SIZE_REQUEST_DEFAULT = 0;

	  public const string OPTIMUM_SPACE_ALIGNMENT_YES = "1";
	  public const string OPTIMUM_SPACE_ALIGNMENT_NO = "0";

	  private readonly sbyte[] tempData_ = new sbyte[50];

	  private string userSpaceName_;
	  private string userSpaceLibrary_;
	  private string extendedAttribute_;
	  private int initialSize_;
	  private sbyte initialValue_;
	  private string publicAuthority_;
	  private string textDescription_;
	  private string replace_;
	  private string domain_;
	  private int transferSizeRequest_;
	  private string optimumSpaceAlignment_;

	  public CreateUserSpace(string userSpaceName, string userSpaceLibrary, string extendedAttribute, int initialSize, sbyte initialValue, string publicAuthority, string textDescription, string replace, string domain, int transferSizeRequest, string optimumSpaceAlignment)
	  {
		userSpaceName_ = userSpaceName;
		userSpaceLibrary_ = userSpaceLibrary;
		extendedAttribute_ = extendedAttribute;
		initialSize_ = initialSize;
		initialValue_ = initialValue;
		publicAuthority_ = publicAuthority;
		textDescription_ = textDescription;
		replace_ = replace;
		domain_ = domain;
		transferSizeRequest_ = transferSizeRequest;
		optimumSpaceAlignment_ = optimumSpaceAlignment;
	  }

	  public virtual string UserSpaceName
	  {
		  get
		  {
			return userSpaceName_;
		  }
		  set
		  {
			userSpaceName_ = value;
		  }
	  }


	  public virtual string UserSpaceLibrary
	  {
		  get
		  {
			return userSpaceLibrary_;
		  }
		  set
		  {
			userSpaceLibrary_ = value;
		  }
	  }


	  public virtual int InitialSize
	  {
		  get
		  {
			return initialSize_;
		  }
		  set
		  {
			initialSize_ = value;
		  }
	  }


	  public virtual sbyte InitialValue
	  {
		  get
		  {
			return initialValue_;
		  }
		  set
		  {
			initialValue_ = value;
		  }
	  }


	  public virtual string PublicAuthority
	  {
		  get
		  {
			return publicAuthority_;
		  }
		  set
		  {
			publicAuthority_ = value;
		  }
	  }


	  public virtual string TextDescription
	  {
		  get
		  {
			return textDescription_;
		  }
		  set
		  {
			textDescription_ = value;
		  }
	  }


	  public virtual string Replace
	  {
		  get
		  {
			return replace_;
		  }
		  set
		  {
			replace_ = value;
		  }
	  }


	  public virtual string Domain
	  {
		  get
		  {
			return domain_;
		  }
		  set
		  {
			domain_ = value;
		  }
	  }


	  public virtual int TransferSizeRequest
	  {
		  get
		  {
			return transferSizeRequest_;
		  }
		  set
		  {
			transferSizeRequest_ = value;
		  }
	  }


	  public virtual string OptimumSpaceAlignment
	  {
		  get
		  {
			return optimumSpaceAlignment_;
		  }
		  set
		  {
			optimumSpaceAlignment_ = value;
		  }
	  }


	  public virtual void newCall()
	  {
	  }

	  public virtual int NumberOfParameters
	  {
		  get
		  {
			return 11;
		  }
	  }

	  public virtual int getParameterInputLength(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			  return 20;
		  case 1:
			  return 10;
		  case 2:
			  return 4;
		  case 3:
			  return 1;
		  case 4:
			  return 10;
		  case 5:
			  return 50;
		  case 6:
			  return 10;
		  case 7:
			  return 4;
		  case 8:
			  return 10;
		  case 9:
			  return 4;
		  case 10:
			  return 1;
		}
		return 0;
	  }

	  public virtual int getParameterOutputLength(int parmIndex)
	  {
		return (parmIndex == 7 ? 4 : 0);
	  }

	  public virtual int getParameterType(int parmIndex)
	  {
		return (parmIndex == 7 ? Parameter.TYPE_INPUT_OUTPUT : Parameter.TYPE_INPUT);
	  }

	  public virtual sbyte[] getParameterInputData(int parmIndex)
	  {
		switch (parmIndex)
		{
		  case 0:
			Conv.stringToBlankPadEBCDICByteArray(userSpaceName_, tempData_, 0, 10);
			Conv.stringToBlankPadEBCDICByteArray(userSpaceLibrary_, tempData_, 10, 10);
			break;
		  case 1:
			Conv.stringToBlankPadEBCDICByteArray(extendedAttribute_, tempData_, 0, 10);
			break;
		  case 2:
			Conv.intToByteArray(initialSize_, tempData_, 0);
			break;
		  case 3:
			tempData_[0] = initialValue_;
			break;
		  case 4:
			Conv.stringToBlankPadEBCDICByteArray(publicAuthority_, tempData_, 0, 10);
			break;
		  case 5:
			Conv.stringToBlankPadEBCDICByteArray(textDescription_, tempData_, 0, 50);
			break;
		  case 6:
			Conv.stringToBlankPadEBCDICByteArray(replace_, tempData_, 0, 10);
			break;
		  case 7:
			return ZERO;
		  case 8:
			Conv.stringToBlankPadEBCDICByteArray(domain_, tempData_, 0, 10);
			break;
		  case 9:
			Conv.intToByteArray(transferSizeRequest_, tempData_, 0);
			break;
		  case 10:
			Conv.stringToBlankPadEBCDICByteArray(optimumSpaceAlignment_, tempData_, 0, 1);
			break;
		}
		return tempData_;
	  }


	  public virtual sbyte[] TempDataBuffer
	  {
		  get
		  {
			return tempData_;
		  }
	  }

	  public virtual void setParameterOutputData(int parmIndex, sbyte[] tempData, int maxLength)
	  {
	  }

	  public virtual string ProgramName
	  {
		  get
		  {
			return "QUSCRTUS";
		  }
	  }

	  public virtual string ProgramLibrary
	  {
		  get
		  {
			return "QSYS";
		  }
	  }
	}


}