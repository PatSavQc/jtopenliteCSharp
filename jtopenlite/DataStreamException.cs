using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DataStreamException.java
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
	/// An IOException that represents an error processing a System i Host Server datastream reply.
	/// 
	/// </summary>
	public class DataStreamException : IOException
	{
	  /// <summary>
	  /// Id for serialization
	  /// </summary>
		private const long serialVersionUID = -3804731342532950986L;

	/// <summary>
	/// Constant representing a datastream reply type that has an unsuccessful return code.
	///  
	/// </summary>
	  public const string BAD_RETURN_CODE = "Bad return code";

	  /// <summary>
	  /// Constant representing a datastream reply type that has an unexpected datastream length (LL).
	  /// 
	  /// </summary>
	  public const string BAD_LENGTH = "Bad length";

	  /// <summary>
	  /// Constant representing a datastream reply type that has an unexpected reply.
	  /// 
	  /// </summary>
	  public const string BAD_REPLY = "Bad reply";

	  /// <summary>
	  /// Constant representing a datastream reply type that contains an error message.
	  /// 
	  /// </summary>
	  public const string ERROR_MESSAGE = "Error message";

	  private string type_;
	  private string dataStreamName_;
	  private int value_;
	  private List<Message> messages_ = new List<Message>();

	  protected internal DataStreamException(string type, string dataStreamName, int value) : base(type + " on " + dataStreamName + ": " + (BAD_RETURN_CODE.Equals(type) || BAD_REPLY.Equals(type) ? ("0x" + value.ToString("x")) : "" + value))
	  {
		/* BAD_RETURN_CODE is a string so .equals must be used */ 
		type_ = type;
		dataStreamName_ = dataStreamName;
		value_ = value;
	  }

	  private DataStreamException(string dataStreamName, Message message) : base(ERROR_MESSAGE + " on " + dataStreamName + ": " + message)
	  {
		type_ = ERROR_MESSAGE;
		dataStreamName_ = dataStreamName;
		value_ = message.Severity;
		messages_.Add(message);
	  }

	  /// <summary>
	  /// Returns the datastream reply type.
	  /// 
	  /// </summary>
	  public virtual string Type
	  {
		  get
		  {
			return type_;
		  }
	  }

	  /// <summary>
	  /// Returns the name of the datastream that caused this exception.
	  /// 
	  /// </summary>
	  public virtual string DataStreamName
	  {
		  get
		  {
			return dataStreamName_;
		  }
	  }

	  /// <summary>
	  /// Returns the value associated with this exception, if any.
	  /// 
	  /// </summary>
	  public virtual int Value
	  {
		  get
		  {
			return value_;
		  }
	  }

	  /// <summary>
	  /// Returns the primary error message associated with this exception, if any.
	  /// 
	  /// </summary>
	  public virtual Message ErrorMessage
	  {
		  get
		  {
			return messages_.Count == 0 ? null : messages_[0];
		  }
	  }

	  /// <summary>
	  /// Associates a message with this exception.
	  /// 
	  /// </summary>
	  public virtual void addMessage(Message message)
	  {
		messages_.Add(message);
	  }

	  /// <summary>
	  /// Returns the array of error messages associated with this exception, if any.
	  /// 
	  /// </summary>
	  public virtual Message[] ErrorMessages
	  {
		  get
		  {
			Message[] arr = new Message[messages_.Count];
			messages_.toArray(arr);
			return arr;
		  }
	  }

	  /// <summary>
	  /// Factory method for constructing a datastream exception with the provided bad return code value.
	  /// 
	  /// </summary>
	  public static DataStreamException badReturnCode(string dataStreamName, int value)
	  {
		return new DataStreamException(BAD_RETURN_CODE, dataStreamName, value);
	  }

	  /// <summary>
	  /// Factory method for constructing a datastream exception with the provided bad length value.
	  /// 
	  /// </summary>
	  public static DataStreamException badLength(string dataStreamName, int value)
	  {
		return new DataStreamException(BAD_LENGTH, dataStreamName, value);
	  }

	  /// <summary>
	  /// Factory method for constructing a datastream exception with the provided bad reply value.
	  /// 
	  /// </summary>
	  public static DataStreamException badReply(string dataStreamName, int codepoint)
	  {
		return new DataStreamException(BAD_REPLY, dataStreamName, codepoint);
	  }

	  /// <summary>
	  /// Factory method for constructing a datastream exception with the provided error message.
	  /// 
	  /// </summary>
	  public static DataStreamException errorMessage(string dataStreamName, Message message)
	  {
		return new DataStreamException(dataStreamName, message);
	  }
	}

}