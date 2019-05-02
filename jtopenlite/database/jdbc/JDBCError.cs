using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  JDBCError.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{
	/// <summary>
	/// Utility class to report common SQL errors
	/// </summary>
	public class JDBCError
	{

	  // Constants for SQL states.
	  internal const string EXC_ACCESS_MISMATCH = "42505";
	  internal const string EXC_ATTRIBUTE_VALUE_INVALID = "HY024";
	  internal const string EXC_BUFFER_LENGTH_INVALID = "HY090";
	  internal const string EXC_CHAR_CONVERSION_INVALID = "22524";
	  internal const string EXC_CCSID_INVALID = "22522";
	  internal const string EXC_COLUMN_NOT_FOUND = "42703";
	  internal const string EXC_CONCURRENCY_INVALID = "HY108";
	  internal const string EXC_CONNECTION_NONE = "08003";
	  internal const string EXC_CONNECTION_REJECTED = "08004";
	  internal const string EXC_CONNECTION_UNABLE = "08001";
	  internal const string EXC_COMMUNICATION_LINK_FAILURE = "08S01";
	  internal const string EXC_CURSOR_NAME_AMBIGUOUS = "3C000";
	  internal const string EXC_CURSOR_NAME_INVALID = "34000";
	  internal const string EXC_CURSOR_POSITION_INVALID = "HY109";
	  internal const string EXC_CURSOR_STATE_INVALID = "24000";
	  internal const string EXC_DATA_TYPE_INVALID = "HY004";
	  internal const string EXC_DATA_TYPE_MISMATCH = "07006";
	  internal const string EXC_DESCRIPTOR_INDEX_INVALID = "07009";
	  internal const string EXC_FUNCTION_NOT_SUPPORTED = "IM001";
	  internal const string EXC_FUNCTION_SEQUENCE = "HY010";
	  internal const string EXC_INTERNAL = "HY000";
	  internal const string EXC_MAX_STATEMENTS_EXCEEDED = "HY014";
	  internal const string EXC_OPERATION_CANCELLED = "HY008";
	  internal const string EXC_PARAMETER_COUNT_MISMATCH = "07001";
	  internal const string EXC_PARAMETER_TYPE_INVALID = "HY105";
	  internal const string EXC_SCALE_INVALID = "HY094";
	  internal const string EXC_SERVER_ERROR = "HY001";
	  internal const string EXC_SYNTAX_BLANK = "43617";
	  internal const string EXC_SYNTAX_ERROR = "42601";
	  internal const string EXC_TXN_STATE_INVALID = "25000";
	  internal const string EXC_SQL_STATEMENT_TOO_LONG = "54001";



	  private static readonly object[][] errors = new object[][]
	  {
		  new object[] {EXC_PARAMETER_COUNT_MISMATCH, "The number of parameter values set or registered does not match the number of parameters."},
		  new object[] {EXC_DATA_TYPE_MISMATCH, "Data type mismatch."},
		  new object[] {EXC_DESCRIPTOR_INDEX_INVALID, "Descriptor index not valid."},
		  new object[] {EXC_CONNECTION_NONE, "The connection does not exist."},
		  new object[] {EXC_CONNECTION_REJECTED, "The connection was rejected."},
		  new object[] {EXC_COMMUNICATION_LINK_FAILURE, "Communication link failure."},
		  new object[] {EXC_CCSID_INVALID, "CCSID value is not valid."},
		  new object[] {EXC_CHAR_CONVERSION_INVALID, "Character conversion resulted in truncation."},
		  new object[] {EXC_CURSOR_STATE_INVALID, "Cursor state not valid."},
		  new object[] {EXC_TXN_STATE_INVALID, "Transaction state not valid."},
		  new object[] {EXC_CURSOR_NAME_INVALID, "Cursor name not valid."},
		  new object[] {EXC_CURSOR_NAME_AMBIGUOUS, "Cursor name is ambiguous."},
		  new object[] {EXC_ACCESS_MISMATCH, "Connection authorization failure occurred."},
		  new object[] {EXC_SYNTAX_ERROR, "A character, token, or clause is not valid or is missing."},
		  new object[] {EXC_COLUMN_NOT_FOUND, "An undefined column name was detected."},
		  new object[] {EXC_SYNTAX_BLANK, "A string parameter value with zero length was detected."},
		  new object[] {EXC_INTERNAL, "Internal driver error."},
		  new object[] {EXC_SERVER_ERROR, "Internal server error."},
		  new object[] {EXC_DATA_TYPE_INVALID, "Data type not valid."},
		  new object[] {EXC_OPERATION_CANCELLED, "Operation cancelled."},
		  new object[] {EXC_FUNCTION_SEQUENCE, "Function sequence error."},
		  new object[] {EXC_MAX_STATEMENTS_EXCEEDED, "Limit on number of statements exceeded."},
		  new object[] {EXC_ATTRIBUTE_VALUE_INVALID, "Attribute value not valid."},
		  new object[] {EXC_BUFFER_LENGTH_INVALID, "String or buffer length not valid."},
		  new object[] {EXC_SCALE_INVALID, "Scale not valid."},
		  new object[] {EXC_PARAMETER_TYPE_INVALID, "Parameter type not valid."},
		  new object[] {EXC_CONCURRENCY_INVALID, "Concurrency or type option not valid."},
		  new object[] {EXC_CURSOR_POSITION_INVALID, "Cursor position not valid."},
		  new object[] {EXC_FUNCTION_NOT_SUPPORTED, "The driver does not support this function."},
		  new object[] {EXC_SQL_STATEMENT_TOO_LONG, "SQL statement too long or complex."}
	  };


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static java.util.Hashtable reasonHashtable = null;
	   internal static Hashtable reasonHashtable = null;



	/// <summary>
	/// Throws an SQL exception based on an error in the
	/// error table.
	/// </summary>
	/// <param name="sqlState">    The SQL State.
	/// </param>
	/// <exception cref="SQLException">    Always.
	///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void throwSQLException(String sqlState) throws SQLException
	  public static void throwSQLException(string sqlState)
	  {
		throw getSQLException(sqlState);
	  }

	  public static SQLException getSQLException(string sqlState)
	  {
		string reason = getReason(sqlState);
		SQLException e = new SQLException(reason, sqlState, -99999);
		return e;
	  }


	  public static SQLException getSQLException(string sqlState, string extra)
	  {
		string reason = getReason(sqlState);
		SQLException e;
		if (!string.ReferenceEquals(extra, null))
		{
		e = new SQLException(reason + " : " + extra, sqlState, -99999);
		}
		else
		{
		e = new SQLException(reason, sqlState, -99999);
		}
		return e;
	  }

	/// <summary>
	/// Returns the reason text based on a SQL state.
	/// </summary>
	/// <param name="sqlState">    the SQL State. </param>
	/// <returns>             Reason - error description.
	///  </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static final String getReason(String sqlState)
	  internal static string getReason(string sqlState)
	  {
			lock (errors)
			{
				if (reasonHashtable == null)
				{
					reasonHashtable = new Hashtable();
					for (int i = 0; i < errors.Length; i++)
					{
						reasonHashtable[errors[i][0]] = errors[i][1];
					}
				}
			}
			string reason = (string) reasonHashtable[sqlState];
			if (string.ReferenceEquals(reason, null))
			{
				reason = "No message for " + sqlState;
			}
			return reason;
	  }

	}

}