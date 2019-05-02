using System;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpen (IBM Toolbox for Java - OSS version)
//
// Filename: AS400JDBCDatabaseMetaData.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 1997-2010 International Business Machines Corporation and
// others. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.database.jdbc
{





	/// <summary>
	/// The JDBCDatabaseMetaData class provides information
	/// about the database as a whole.
	/// 
	/// <para>Some of the methods in this class take arguments that are
	/// pattern strings.  Such arguments are suffixed with "Pattern".
	/// Within a pattern string, "%" means match any substring of zero
	/// or more characters, and "_" means match exactly one character.
	/// Only entries matching the pattern string are returned.
	/// 
	/// </para>
	/// <para>For example, if the schemaPattern argument for getTables()
	/// is "H%WO_LD", then the following schemas might match
	/// the pattern, provided they exist on the system:
	/// <pre>
	/// HELLOWORLD
	/// HIWORLD
	/// HWORLD
	/// HELLOWOULD
	/// HIWOULD
	/// </pre>
	/// 
	/// </para>
	/// <para>Many of the methods here return lists of information in
	/// result sets.  You can use the normal ResultSet methods to
	/// retrieve data from these result sets.  The format of the
	/// result sets are described in the JDBC interface specification.
	/// 
	/// </para>
	/// <para>Schema and table names that are passed as input to methods
	/// in this class are implicitly uppercased unless enclosed in
	/// double-quotes.
	/// 
	/// </para>
	/// </summary>

	//-----------------------------------------------------------
	// Using nulls and empty strings for catalog functions
	//
	//   When the parameter is NOT search pattern capable and:
	//     null is specified for:
	//             catalog (system) - parameter is ignored
	//             schema (library) - use default SQL schema
	//                                The default SQL schema can be
	//                                set in the URL. If not
	//                                specified in URL, the first
	//                                library specified in the library
	//                                properties is used as the
	//                                default SQL schema.
	//                                If no default SQL schema exists,
	//                                QGPL is used.
	//             table (file)     - empty result set is returned
	//             column (field)   - empty result set is returned
	//     empty string is specified for:
	//             catalog (system) - empty result set is returned
	//             schema (library) - empty result set is returned
	//             table (file)     - empty result set is returned
	//             column (field)   - empty result set is returned
	//
	//
	//   When the parameter is search pattern capable and:
	//     null is specified for:
	//             schemaPattern (library) - no value sent to system.
	//					 System default of
	//                                       *USRLIBL is used.
	//             tablePattern (file)     - no value sent to system
	//                                       system default of *ALL used
	//     empty string is specified for:
	//             schemaPattern (library) - empty result set is returned
	//             tablePattern (file)     - empty result set is returned
	//
	//
	//----------------------------------------------------------

	public class JDBCDatabaseMetaData : DatabaseMetaData
	{
	  internal const string copyright = "Copyright (C) 1997-2010 International Business Machines Corporation and others.";

		// Private data.
		private JDBCConnection connection_;

		// misc constants for sysibm stored procedures
		internal const int SQL_NO_NULLS = 0;
		internal const int SQL_NULLABLE = 1;
		internal const int SQL_NULLABLE_UNKNOWN = 2;
		internal const int SQL_BEST_ROWID = 1;
		internal const int SQL_ROWVER = 2;
		internal const string EMPTY_STRING = "";
		internal const string MATCH_ALL = "%";


		private const string VIEW = "VIEW";
		private const string TABLE = "TABLE";
		private const string SYSTEM_TABLE = "SYSTEM TABLE";
		private const string ALIAS = "ALIAS";
		private const string MQT = "MATERIALIZED QUERY TABLE";
		private const string SYNONYM = "SYNONYM";
		private const string FAKE_VALUE = "QCUJOFAKE";
		private const int SQL_ALL_TYPES = 0;

		// the DB2 SQL reference says this should be 2147483647 but we return 1 less to allow for NOT NULL columns
		internal const int MAX_LOB_LENGTH = 2147483646;


		internal static int javaVersion = 0;
		static JDBCDatabaseMetaData()
		{
			string javaVersionString = System.getProperty("java.version");
			if (!string.ReferenceEquals(javaVersionString, null))
			{
				int dotIndex = javaVersionString.IndexOf('.');
				if (dotIndex > 0)
				{
				  int secondDotIndex = javaVersionString.IndexOf('.', dotIndex + 1);
				  if (secondDotIndex > 0)
				  {
					string firstDigit = javaVersionString.Substring(0,dotIndex);
					string secondDigit = javaVersionString.Substring(dotIndex + 1, secondDotIndex - (dotIndex + 1));
					javaVersion = int.Parse(firstDigit) * 10 + int.Parse(secondDigit);
				  }
				}
			}
		}

		/// <summary>
		/// Constructs an JDBCDatabaseMetaData object.
		/// </summary>
		/// <param name="connection">  The connection to the system.
		///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: JDBCDatabaseMetaData(JDBCConnection connection) throws java.sql.SQLException
		internal JDBCDatabaseMetaData(JDBCConnection connection)
		{
			connection_ = connection;
		}



		/// <summary>
		/// Indicates if all of the procedures returned by getProcedures() can be
		/// called by the current user.
		/// </summary>
		/// <returns>     Always false.   This driver cannot determine if all of the procedures
		///                            can be called by the current user. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean allProceduresAreCallable() throws java.sql.SQLException
		public virtual bool allProceduresAreCallable()
		{
			return false;
		}



		/// <summary>
		/// Indicates if all of the tables returned by getTables() can be
		/// SELECTed by the current user.
		/// </summary>
		/// <returns>     Always false. This driver cannot determine if all of the tables
		///            returned by getTables() can be selected by the current user. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean allTablesAreSelectable() throws java.sql.SQLException
		public virtual bool allTablesAreSelectable()
		{
			return false;
		}



		/// <summary>
		/// Indicates if a data definition statement within a transaction
		/// can force the transaction to commit.
		/// </summary>
		/// <returns>     Always false. A data definition statement within a transaction
		///            does not force the transaction to commit. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean dataDefinitionCausesTransactionCommit() throws java.sql.SQLException
		public virtual bool dataDefinitionCausesTransactionCommit()
		{
			return false;
		}



		/// <summary>
		/// Indicates if a data definition statement within a transaction is
		/// ignored.
		/// </summary>
		/// <returns>     Always false. A data definition statement within a
		///            transaction is not ignored. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean dataDefinitionIgnoredInTransactions() throws java.sql.SQLException
		public virtual bool dataDefinitionIgnoredInTransactions()
		{
			return false;
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if visible deletes to a result set of the specified type
		/// can be detected by calling ResultSet.rowDeleted().  If visible
		/// deletes cannot be detected, then rows are removed from the
		/// result set as they are deleted.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     Always false.  Deletes can not be detected
		///                            by calling ResultSet.rowDeleted().
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean deletesAreDetected(int resultSetType) throws java.sql.SQLException
		public virtual bool deletesAreDetected(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return false;
		}



		/// <summary>
		/// Indicates if getMaxRowSize() includes blobs when computing the
		/// maximum length of a single row.
		/// </summary>
		/// <returns>     Always true. getMaxRowSize() does include blobs when
		///            computing the maximum length of a single row. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean doesMaxRowSizeIncludeBlobs() throws java.sql.SQLException
		public virtual bool doesMaxRowSizeIncludeBlobs()
		{
			return true;
		}




		/// <summary>
		/// Returns a ResultSet containing a description of type attributes available in a
		/// specified catalog.
		/// 
		/// This method only applies to the attributes of a
		/// structured type.  Distinct types are stored in the datatypes
		/// catalog, not the attributes catalog. Since DB2 for IBM i does not support
		/// structured types at this time, an empty ResultSet will always be returned
		/// for calls to this method.
		/// </summary>
		/// <param name="catalog">              The catalog name. </param>
		/// <param name="schemaPattern">        The schema name pattern. </param>
		/// <param name="typeNamePattern">      The type name pattern. </param>
		/// <param name="attributeNamePattern"> The attribute name pattern.
		/// </param>
		/// <returns>      The empty ResultSet
		/// </returns>
		/// <exception cref="SQLException">  This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getAttributes(String catalog, String schemaPattern, String typeNamePattern, String attributeNamePattern) throws java.sql.SQLException
		public virtual ResultSet getAttributes(string catalog, string schemaPattern, string typeNamePattern, string attributeNamePattern)
		{
			// We return an empty result set because this is not supported by our driver
			Statement statement = connection_.createStatement();


			ResultSet rs = statement.executeQuery("SELECT VARCHAR('1', 128) AS TYPE_CAT, " + "VARCHAR('2', 128)  AS TYPE_SCHEM, " + "VARCHAR('3', 128)  AS TYPE_NAME, " + "VARCHAR('4', 128)  AS ATTR_NAME, " + "SMALLINT(5)        AS DATA_TYPE, " + "VARCHAR('6', 128)  AS ATTR_TYPE_NAME, " + "INT(7)             AS ATTR_SIZE, " + "INT(8)             AS DECIMAL_DIGITS, " + "INT(9)             AS NUM_PREC_RADIX, " + "INT(10)            AS NULLABLE, " + "VARCHAR('11', 128) AS REMARKS, " + "VARCHAR('12', 128) AS ATTR_DEF, " + "INT(13)            AS SQL_DATA_TYPE, " + "INT(14)            AS SQL_DATETIME_SUB, " + "INT(15)            AS CHAR_OCTET_LENGTH, " + "INT(16)            AS ORDINAL_POSITION, " + "VARCHAR('17', 128) AS IS_NULLABLE, " + "VARCHAR('18', 128) AS SCOPE_CATALOG, " + "VARCHAR('19', 128) AS SCOPE_SCHEMA, " + "VARCHAR('20', 128) AS SCOPE_TABLE, " + "SMALLINT(21)       AS SOURCE_DATA_TYPE " + "FROM QSYS2" + CatalogSeparator + "SYSTYPES WHERE 1 = 2 FOR FETCH ONLY ");

			return rs;
		}



		/// <summary>
		/// Returns a description of a table's optimal set of columns
		/// that uniquely identifies a row.
		/// 
		/// </summary>
		/// <param name="catalog">        The catalog name. If null is specified, this parameter
		///                   is ignored.  If empty string is specified,
		///                   an empty result set is returned. </param>
		/// <param name="schema">         The schema name. If null is specified, the
		///                   default SQL schema specified in the URL is used.
		///                   If null is specified and a default SQL schema was not
		///                   specified in the URL, the first library specified
		///                   in the libraries properties file is used.
		///                   If null is specified and a default SQL schema was
		///                   not specified in the URL and a library was not
		///                   specified in the libraries properties file,
		///                   QGPL is used.
		///                   If empty string is specified, an empty result set will
		///                   be returned. </param>
		/// <param name="table">          The table name. If null or empty string is specified,
		///                   an empty result set is returned. </param>
		/// <param name="scope">          The scope of interest. Valid values are:
		///                   bestRowTemporary and bestRowTransaction.
		///                   bestRowSession is not allowed because
		///                   it cannot be guaranteed that
		///                   the row will remain valid for the session.
		///                   If bestRowSession is specified, an empty result
		///                   set is returned.
		///                   If bestRowTransaction is specified,
		///                   autocommit is false, and transaction is set to repeatable read,
		///                   then results is returned; otherwise, an empty result set
		///                   is returned. </param>
		/// <param name="nullable">       The value indicating if columns that are nullable should be included. </param>
		/// <returns>                The ResultSet containing a table's optimal
		///                   set of columns that uniquely identify a row.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                        or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getBestRowIdentifier(String catalog, String schema, String table, int scope, boolean nullable) throws java.sql.SQLException
		public virtual ResultSet getBestRowIdentifier(string catalog, string schema, string table, int scope, bool nullable)
		{

				CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLSPECIALCOLUMNS(?,?,?,?,?,?,?)");

				cstmt.setShort(1, (short)SQL_BEST_ROWID);
				cstmt.setString(2, normalize(catalog));
				cstmt.setString(3, normalize(schema));
				cstmt.setString(4, normalize(table));
				cstmt.setShort(5, (short) scope);
				if (nullable)
				{
					cstmt.setShort(6, (short) SQL_NULLABLE);
				}
				else
				{
					cstmt.setShort(6, (short) SQL_NO_NULLS);
				}
				cstmt.setString(7, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cstmt.execute();

				ResultSet rs = cstmt.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cstmt.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}

				return rs;

		}



		/// <summary>
		/// Returns the catalog name available in this database.  This
		/// will return a ResultSet with a single row, whose value is
		/// the IBM i system name.
		/// </summary>
		/// <returns>      The ResultSet containing the IBM i system name.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getCatalogs() throws java.sql.SQLException
		public virtual ResultSet Catalogs
		{
			get
			{
    
    
					CallableStatement cstmt = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLTABLES(?,?,?,?,?)");
    
					cstmt.setString(1, "%");
					cstmt.setString(2, "%");
					cstmt.setString(3, "%");
					cstmt.setString(4, "%");
					cstmt.setString(5, "DATATYPE='JDBC';GETCATALOGS=1;CURSORHOLD=1");
					cstmt.execute();
					ResultSet rs = cstmt.ResultSet;
					if (rs != null)
					{
						((JDBCResultSet)rs).isMetadataResultSet_ = true;
					}
					else
					{
						cstmt.close();
						// It is an error not to have received a result set
						JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
					}
    
					return rs;
			}
		}



		/// <summary>
		/// Returns the naming convention used when referring to tables.
		/// This depends on the naming convention specified in the connection
		/// properties.
		/// </summary>
		/// <returns>     If using SQL naming convention, "." is returned. If
		///            using system naming convention, "/" is returned.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCatalogSeparator() throws java.sql.SQLException
		public virtual string CatalogSeparator
		{
			get
			{
				string catalogSeparator;
			catalogSeparator = ".";
				return catalogSeparator;
			}
		}



		/// <summary>
		/// Returns the DB2 for IBM i SQL term for "catalog".
		/// </summary>
		/// <returns>     The term "Database".
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getCatalogTerm() throws java.sql.SQLException
		public virtual string CatalogTerm
		{
			get
			{
				return "Database";
			}
		}



		/// <summary>
		/// Returns a description of the access rights for a table's columns.
		/// </summary>
		/// <param name="catalog">         The catalog name. If null is specified, this parameter
		///                        is ignored.  If empty string is specified,
		///                        an empty result set is returned. </param>
		/// <param name="schema">          The schema name. If null is specified, the
		///                        default SQL schema specified in the URL is used.
		///                        If null is specified and a default SQL schema was not
		///                        specified in the URL, the first library specified
		///                        in the libraries properties file is used.
		///                        If null is specified and a default SQL schema was
		///                        not specified in the URL and a library was not
		///                        specified in the libraries properties file,
		///                        QGPL is used.
		///                        If empty string is specified, an empty result set will
		///                        be returned. </param>
		/// <param name="table">           The table name. If null or empty string is specified,
		///                        an empty result set is returned. </param>
		/// <param name="columnPattern">   The column name pattern.  If null is specified,
		///                        no value is sent to the system and the system
		///                        default of *all is used.  If empty string
		///                        is specified, an empty result set is returned.
		/// </param>
		/// <returns>                 The ResultSet containing access rights for a
		///                        table's columns.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getColumnPrivileges(String catalog, String schema, String table, String columnPattern) throws java.sql.SQLException
		public virtual ResultSet getColumnPrivileges(string catalog, string schema, string table, string columnPattern)
		{

				// Set the column name and search pattern
				// If null, do not set parameter. The system default
				// value of *ALL is used.

				CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLCOLPRIVILEGES (?, ?, ?, ?, ?)");

				cstmt.setString(1, normalize(catalog));
				cstmt.setString(2, normalize(schema));
				cstmt.setString(3, normalize(table));
				cstmt.setString(4, normalize(columnPattern));
				cstmt.setObject(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				ResultSet rs = cstmt.executeQuery();
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cstmt.close();
				}

				return rs;

		}




		/// <summary>
		/// Returns a description of the table's columns available in a
		/// catalog.
		/// </summary>
		/// <param name="catalog">         The catalog name. If null is specified, this parameter
		///                        is ignored.  If empty string is specified,
		///                        an empty result set is returned. </param>
		/// <param name="schemaPattern">   The schema name pattern.
		///                        If the "metadata source" connection property is set to 0
		///                        and null is specified, no value is sent to the system and
		///                        the default of *USRLIBL is used.
		///                        If the "metadata source" connection property is set to 1
		///                        and null is specified, then information from all schemas
		///                        will be returned.
		///                        If an empty string
		///                        is specified, an empty result set is returned. </param>
		/// <param name="tablePattern">    The table name pattern. If null is specified,
		///                        no value is sent to the system and the system
		///                        default of *ALL is used.  If empty string
		///                        is specified, an empty result set is returned. </param>
		/// <param name="columnPattern">   The column name pattern.  If null is specified,
		///                        no value is sent to the system and the system
		///                        default of *ALL is used.  If empty string
		///                        is specified, an empty result set is returned.
		/// </param>
		/// <returns>                 The ResultSet containing the table's columns available
		///                        in a catalog.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getColumns(String catalog, String schemaPattern, String tablePattern, String columnPattern) throws java.sql.SQLException
		public virtual ResultSet getColumns(string catalog, string schemaPattern, string tablePattern, string columnPattern)
		{





		CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLCOLUMNS(?,?,?,?,?)");

		cs.setString(1, normalize(catalog));
		cs.setString(2, normalize(schemaPattern));
		cs.setString(3, normalize(tablePattern));
		cs.setString(4, normalize(columnPattern));

		if (javaVersion > 16)
		{
			cs.setString(5, "DATATYPE='JDBC';JDBCVER='4.1';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
		}
		else if (javaVersion > 15)
		{
			cs.setString(5, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
		}
		else
		{
			cs.setString(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
		}
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{

					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
				}

				return rs;

		} // End of getColumns



		/// <summary>
		/// Returns the connection for this metadata.
		/// </summary>
		/// <returns> The connection for this metadata.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Connection getConnection() throws java.sql.SQLException
		public virtual Connection Connection
		{
			get
			{
				return connection_;
			}
		}




		/// <summary>
		/// Returns a description of the foreign key columns in the
		/// foreign key table that references the primary key columns
		/// of the primary key table.  This is a description of how
		/// the primary table imports the foreign table's key.
		/// </summary>
		/// <param name="primaryCatalog">   The catalog name. If null is specified,
		///                         this parameter is ignored.  If
		///                         empty string is specified, an empty
		///                         result set is returned. </param>
		/// <param name="primarySchema">    The name of the schema where the primary table
		///                         is located.
		///                         If null is specified, the
		///                         default SQL schema specified in the URL is used.
		///                         If null is specified and a default SQL schema was not
		///                         specified in the URL, the first library specified
		///                         in the libraries properties file is used.
		///                         If null is specified,a default SQL schema was
		///                         not specified in the URL, and a library was not
		///                         specified in the libraries properties file,
		///                         QGPL is used.
		///                         If empty string is specified, an empty result
		///                         set is returned. </param>
		/// <param name="primaryTable">     The primary table name. If null or empty string
		///                         is specified, an empty result set is returned. </param>
		/// <param name="foreignCatalog">   The catalog name. If null is specified,
		///                         this parameter is ignored.  If
		///                         empty string is specified, an empty
		///                         result set is returned. </param>
		/// <param name="foreignSchema">    The name of the schema where the primary table
		///                         is located. If null is specified, the
		///                         default SQL schema specified in the URL is used.
		///                         If null is specified and a default SQL schema was not
		///                         specified in the URL, the first library specified
		///                         in the libraries properties file is used.
		///                         If null is specified, a default SQL schema was
		///                         not specified in the URL, and a library was not
		///                         specified in the libraries properties file,
		///                         QGPL is used.
		///                         If empty string is specified,
		///                         an empty result set is returned. </param>
		/// <param name="foreignTable">     The foreign table name. If null or empty string
		///                         is specified, an empty result set is returned. </param>
		/// <returns>                  The ResultSet containing the description of the
		///                         foreign key columns in the foreign key table that
		///                         references the primary key columns of the primary
		///                         key table.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>

		//-------------------------------------------------
		//   The system returns the following:
		//   0 = cascade
		//   1 = No action or restrict
		//   2 = set null or set default
		//
		//   JDBC has 5 possible values:
		//     importedKeyNoAction
		//     importedKeyCascade
		//     importedKeySetNull
		//     importedKeySetDefault
		//     importedKeyRestrict
		//
		//   Since the system groups together
		//   some of the values, all of the
		//   possible JDBC values can not be returned.
		//
		//   For Update Rule, the only values
		//   supported by the system are
		//   no action and restrict.  Since
		//   the value of 1 is returned for
		//   both no action and restrict,
		//   the value of importKeyRestrict
		//   will always be returned for the
		//   update rule.
		//
		//   For Delete Rule
		//   the following is returned.  It is
		//   consistent with the ODBC implementation.
		//    if 0 from system = importedKeyCascade
		//    if 1 from system = importedKeyRestrict
		//    if 2 from system = importedKeySetNull
		//
		//
		//    importedKeyNoAction and importedKeySetDefault
		//    will not be returned.
		//-------------------------------------------------//

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getCrossReference(String primaryCatalog, String primarySchema, String primaryTable, String foreignCatalog, String foreignSchema, String foreignTable) throws java.sql.SQLException
		public virtual ResultSet getCrossReference(string primaryCatalog, string primarySchema, string primaryTable, string foreignCatalog, string foreignSchema, string foreignTable)
		{


				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLFOREIGNKEYS(?,?,?,?,?,?,?)");

				cs.setString(1, normalize(primaryCatalog));
				cs.setString(2, normalize(primarySchema));
				cs.setString(3, normalize(primaryTable));
				cs.setString(4, normalize(foreignCatalog));
				cs.setString(5, normalize(foreignSchema));
				cs.setString(6, normalize(foreignTable));
				cs.setString(7, "DATATYPE='JDBC';EXPORTEDKEY=0;IMPORTEDKEY=0;DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;

		} // End of getCrossReference



		/// <summary>
		/// Returns the major version number of the database.
		/// </summary>
		/// <returns>     The major version number.
		/// @since Modification 5
		///  </returns>
		public virtual int DatabaseMajorVersion
		{
			get
			{
    
				int defaultVersion = 0; //since we do not want to change signature to throw exception, have default as 0
				try
				{
					string v = DatabaseProductVersion;
					int dotIndex = v.IndexOf('.');
					if (dotIndex > 0)
					{
						v = v.Substring(0,dotIndex);
						defaultVersion = int.Parse(v);
					}
				}
				catch (Exception)
				{
					//should not happen
				}
    
				return defaultVersion;
			}
		}


		/// <summary>
		/// Returns the minor version number of the database.
		/// </summary>
		/// <returns>     The minor version number.
		/// @since Modification 5
		///  </returns>
		public virtual int DatabaseMinorVersion
		{
			get
			{
				int defaultVersion = 0;
				try
				{
					string v = DatabaseProductVersion;
					int dotIndex = v.IndexOf('.');
					if (dotIndex > 0)
					{
						v = v.Substring(dotIndex + 1);
						dotIndex = v.IndexOf('.');
						if (dotIndex > 0)
						{
							v = v.Substring(0,dotIndex);
						}
    
						defaultVersion = int.Parse(v);
    
					}
				}
				catch (Exception)
				{
					//should not happen
				}
				return defaultVersion;
    
			}
		}


		/// <summary>
		/// Returns the name of this database product.
		/// </summary>
		/// <returns>     The database product name.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDatabaseProductName() throws java.sql.SQLException
		public virtual string DatabaseProductName
		{
			get
			{
				return JDBCDriver.DATABASE_PRODUCT_NAME_; // @D2C
			}
		}



		/// <summary>
		/// Returns the version of this database product.
		/// </summary>
		/// <returns>     The product version.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDatabaseProductVersion() throws java.sql.SQLException
		public virtual string DatabaseProductVersion
		{
			get
			{
				// The ODBC specification suggests the
				// format "vv.rr.mmmm product-specific".
				// Although the JDBC specification does not
				// mention this format, I will adopt it anyway.
				//
				// The format of the product-specific part is
				// "VxRxmx".  I am not sure why the "m" is lowercase,
				// but somebody somewhere said this is normal, and
				// its a nit anyway.
				int v, r, m;
				int vrm = connection_.ServerVersion;
				v = (long)((ulong)(vrm & 0xffff0000) >> 16); // @D1C
				r = (int)((uint)(vrm & 0x0000ff00) >> 8); // @D1C
				m = (vrm & 0x000000ff); // @D1C
    
				StringBuilder buffer = new StringBuilder();
				if (v < 10)
				{
					buffer.Append("0");
				}
				buffer.Append(v);
				buffer.Append(".");
				if (r < 10)
				{
					buffer.Append("0");
				}
				buffer.Append(r);
				buffer.Append(".");
				if (m < 1000)
				{
					buffer.Append("0");
				}
				if (m < 100)
				{
					buffer.Append("0");
				}
				if (m < 10)
				{
					buffer.Append("0");
				}
				buffer.Append(m);
				buffer.Append(" V");
				buffer.Append(v);
				buffer.Append("R");
				buffer.Append(r);
				buffer.Append("m");
				buffer.Append(m);
				return buffer.ToString();
			}
		}



		/// <summary>
		/// Returns the default transaction isolation level.
		/// </summary>
		/// <returns>     The default transaction isolation level.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDefaultTransactionIsolation() throws java.sql.SQLException
		public virtual int DefaultTransactionIsolation
		{
			get
			{
				// Default value for other drivers.
				return Connection.TRANSACTION_READ_UNCOMMITTED;
			}
		}



		/// <summary>
		/// Returns the major version number for this JDBC driver.
		/// </summary>
		/// <returns>     The major version number.
		///  </returns>
		public virtual int DriverMajorVersion
		{
			get
			{
				return JDBCDriver.MAJOR_VERSION_;
			}
		}



		/// <summary>
		/// Returns the minor version number for this JDBC driver.
		/// </summary>
		/// <returns>     The minor version number.
		///  </returns>
		public virtual int DriverMinorVersion
		{
			get
			{
				return JDBCDriver.MINOR_VERSION_;
			}
		}



		/// <summary>
		/// Returns the name of this JDBC driver.
		/// </summary>
		/// <returns>     The driver name.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDriverName() throws java.sql.SQLException
		public virtual string DriverName
		{
			get
			{
				return JDBCDriver.DRIVER_NAME_; // @D2C
			}
		}



		/// <summary>
		/// Returns the version of this JDBC driver.
		/// </summary>
		/// <returns>     The driver version.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDriverVersion() throws java.sql.SQLException
		public virtual string DriverVersion
		{
			get
			{
				return JDBCDriver.MAJOR_VERSION_ + "." + JDBCDriver.MINOR_VERSION_;
			}
		}



		/// <summary>
		/// Returns a description of the foreign key columns that
		/// reference a table's primary key columns.  This is the
		/// foreign keys exported by a table.
		/// </summary>
		/// <param name="catalog">        The catalog name. If null is specified, this parameter
		///                       is ignored.  If empty string is specified,
		///                       an empty result set is returned. </param>
		/// <param name="schema">         The schema name. If null is specified, the
		///                       default SQL schema specified in the URL is used.
		///                       If null is specified and a default SQL schema was not
		///                       specified in the URL, the first library specified
		///                       in the libraries properties file is used.
		///                       If null is specified, a default SQL schema was
		///                       not specified in the URL, and a library was not
		///                       specified in the libraries properties file,
		///                       QGPL is used.
		///                       If empty string is specified, an empty result set will
		///                       be returned. </param>
		/// <param name="table">          The table name. If null or empty string is specified,
		///                       an empty result set is returned.
		/// </param>
		/// <returns>                The ResultSet containing the description of the
		///                       foreign key columns that reference a table's
		///                       primary key columns.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getExportedKeys(String catalog, String schema, String table) throws java.sql.SQLException
		public virtual ResultSet getExportedKeys(string catalog, string schema, string table)
		{



				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLFOREIGNKEYS(?,?,?,?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schema));
				cs.setString(3, normalize(table));
				cs.setString(4, normalize(catalog));
				cs.setString(5, EMPTY_STRING);
				cs.setString(6, EMPTY_STRING);
				cs.setString(7, "DATATYPE='JDBC';EXPORTEDKEY=1; CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;
		}



		/// <summary>
		/// Returns all of the "extra" characters that can be used in
		/// unquoted identifier names (those beyond a-z, A-Z, 0-9,
		/// and _).
		/// </summary>
		/// <returns>     The String containing the "extra" characters.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getExtraNameCharacters() throws java.sql.SQLException
		public virtual string ExtraNameCharacters
		{
			get
			{
				return "$@#";
			}
		}



		/// <summary>
		/// Returns the string used to quote SQL identifiers.
		/// </summary>
		/// <returns>     The quote string.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getIdentifierQuoteString() throws java.sql.SQLException
		public virtual string IdentifierQuoteString
		{
			get
			{
				return "\"";
			}
		}



		/// <summary>
		/// Returns a description of the primary key columns that are
		/// referenced by a table's foreign key columns.  This is the
		/// primary keys imported by a table.
		/// </summary>
		/// <param name="catalog">        The catalog name. If null is specified, this parameter
		///                       is ignored.  If empty string is specified,
		///                       an empty result set is returned. </param>
		/// <param name="schema">         The schema name. If null is specified, the
		///                       default SQL schema specified in the URL is used.
		///                       If null is specified and a default SQL schema was not
		///                       specified in the URL, the first library specified
		///                       in the libraries properties file is used.
		///                       If null is specified, a default SQL schema was
		///                       not specified in the URL, and a library was not
		///                       specified in the libraries properties file,
		///                       QGPL is used.
		///                       If empty string is specified, an empty result set will
		///                       be returned. </param>
		/// <param name="table">          The table name. If null or empty string is specified,
		///                       an empty result set is returned.
		/// </param>
		/// <returns>                The ResultSets containing the description of the primary
		///                       key columns that are referenced by a table's foreign
		///                       key columns.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getImportedKeys(String catalog, String schema, String table) throws java.sql.SQLException
		public virtual ResultSet getImportedKeys(string catalog, string schema, string table)
		{




				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLFOREIGNKEYS(?,?,?,?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, null);
				cs.setString(3, null);
				cs.setString(4, normalize(catalog));
				cs.setString(5, normalize(schema));
				cs.setString(6, normalize(table));
				cs.setString(7, "DATATYPE='JDBC';IMPORTEDKEY=1; CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;

		}



		/// <summary>
		/// Returns a description of a table's indexes and statistics.
		/// </summary>
		/// <param name="catalog">      The catalog name. If null is specified, this parameter
		///                     is ignored.  If empty string is specified,
		///                     an empty result set is returned. </param>
		/// <param name="schema">       The schema name. If null is specified, the
		///                     default SQL schema specified in the URL is used.
		///                     If null is specified and a default SQL schema was not
		///                     specified in the URL, the first library specified
		///                     in the libraries properties file is used.
		///                     If null is specified, a default SQL schema was
		///                     not specified in the URL, and a library was not
		///                     specified in the libraries properties file,
		///                     QGPL is used.
		///                     If empty string is specified, an empty result set will
		///                     be returned. </param>
		/// <param name="table">        The table name. If null or empty string is specified,
		///                     an empty result set is returned. </param>
		/// <param name="unique">       The value indicating if unique indexes should be returned.
		///                     If true, only indexes for unique values is returned.
		///                     If false, all indexes is returned. </param>
		/// <param name="approximate">  The value indicating if the result is allowed to reflect
		///                     approximate or out-of-date values.  This value is ignored. </param>
		/// <returns>              The ResultSet containing the description of a table's indexes
		///                     and statistics.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getIndexInfo(String catalog, String schema, String table, boolean unique, boolean approximate) throws java.sql.SQLException
		public virtual ResultSet getIndexInfo(string catalog, string schema, string table, bool unique, bool approximate)
		{



				short iUnique;
				short reserved = 0;

				if (unique)
				{
					iUnique = 0;
				}
				else
				{
					iUnique = 1;
				}

				//Set the library name
				if (!string.ReferenceEquals(schema, null))
				{
					schema = normalize(schema);
				}

				// Set the table name
				if (!string.ReferenceEquals(table, null))
				{
					table = normalize(table);
				}

				/*
				  sysibm.SQLStatistics(
				   CatalogName     varchar(128),
				   SchemaName      varchar(128),
				   TableName       varchar(128),
				   Unique          Smallint,
				   Reserved        Smallint,
				   Options         varchar(4000))
				 */
				CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLSTATISTICS(?,?,?,?,?,?)");

				cstmt.setString(1, normalize(catalog));
				cstmt.setString(2, normalize(schema));
				cstmt.setString(3, normalize(table));
				cstmt.setShort(4, iUnique);
				cstmt.setShort(5, reserved);
				cstmt.setString(6, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cstmt.execute();

				ResultSet rs = cstmt.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cstmt.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;


		} // End of getIndexInfo



		/// <summary>
		/// Returns the JDBC major version number.
		/// </summary>
		/// <returns>     The JDBC major version number.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getJDBCMajorVersion() throws java.sql.SQLException
		public virtual int JDBCMajorVersion
		{
			get
			{
				if (javaVersion >= 16)
				{
					return 4;
				}
				else
				{
					return 3;
				}
			}
		}



		/// <summary>
		/// Returns the JDBC minor version number.
		/// </summary>
		/// <returns>     The JDBC minor version number.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getJDBCMinorVersion() throws java.sql.SQLException
		public virtual int JDBCMinorVersion
		{
			get
			{
				if (javaVersion >= 17)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}




		/// <summary>
		/// Returns the maximum length for an inline binary literal.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxBinaryLiteralLength() throws java.sql.SQLException
		public virtual int MaxBinaryLiteralLength
		{
			get
			{
				return 32739;
			}
		}



		/// <summary>
		/// Returns the maximum length for a catalog name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxCatalogNameLength() throws java.sql.SQLException
		public virtual int MaxCatalogNameLength
		{
			get
			{
				return 10;
			}
		}



		/// <summary>
		/// Returns the maximum length for a character literal.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxCharLiteralLength() throws java.sql.SQLException
		public virtual int MaxCharLiteralLength
		{
			get
			{
				return 32739;
			}
		}



		/// <summary>
		/// Returns the maximum length for a column name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnNameLength() throws java.sql.SQLException
		public virtual int MaxColumnNameLength
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_540)
				{
					return 128;
				}
				else
				{
					return 30;
				}
			}
		}



		/// <summary>
		/// Returns the maximum number of columns in a GROUP BY clause.
		/// </summary>
		/// <returns>     The maximum number of columns.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnsInGroupBy() throws java.sql.SQLException
		public virtual int MaxColumnsInGroupBy
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_610)
				{
					return 8000;
				}
				else
				{
					return 120;
				}
			}
		}



		/// <summary>
		/// Returns the maximum number of columns allowed in an index.
		/// </summary>
		/// <returns>     The maximum number of columns.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnsInIndex() throws java.sql.SQLException
		public virtual int MaxColumnsInIndex
		{
			get
			{
				return 120;
			}
		}



		/// <summary>
		/// Returns the maximum number of columns in an ORDER BY clause.
		/// </summary>
		/// <returns>     The maximum number of columns.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnsInOrderBy() throws java.sql.SQLException
		public virtual int MaxColumnsInOrderBy
		{
			get
			{
				return 10000;
			}
		}



		/// <summary>
		/// Returns the maximum number of columns in a SELECT list.
		/// </summary>
		/// <returns>     The maximum number of columns.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnsInSelect() throws java.sql.SQLException
		public virtual int MaxColumnsInSelect
		{
			get
			{
				return 8000;
			}
		}



		/// <summary>
		/// Returns the maximum number of columns in a table.
		/// </summary>
		/// <returns>     The maximum number of columns.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxColumnsInTable() throws java.sql.SQLException
		public virtual int MaxColumnsInTable
		{
			get
			{
				return 8000;
			}
		}



		/// <summary>
		/// Returns the number of active connections you can have at a time
		/// to this database.
		/// </summary>
		/// <returns>     The maximum number of connections or 0
		///            if no limit.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxConnections() throws java.sql.SQLException
		public virtual int MaxConnections
		{
			get
			{
				// There is no limit.  The specification does
				// not come right out and say "0 means no limit",
				// but that is how ODBC and many other parts
				// of JDBC work.
				return 0;
			}
		}



		/// <summary>
		/// Returns the maximum cursor name length.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxCursorNameLength() throws java.sql.SQLException
		public virtual int MaxCursorNameLength
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_610)
				{
					return 128;
				}
				else
				{
					return 18;
				}
			}
		}



		/// <summary>
		/// Returns the maximum length of an index.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxIndexLength() throws java.sql.SQLException
		public virtual int MaxIndexLength
		{
			get
			{
				return 2000;
			}
		}



		/// <summary>
		/// Returns the maximum length of a procedure name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxProcedureNameLength() throws java.sql.SQLException
		public virtual int MaxProcedureNameLength
		{
			get
			{
				return 128;
			}
		}



		/// <summary>
		/// Returns the maximum length of a single row.
		/// </summary>
		/// <returns>     The maximum length (in bytes). </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxRowSize() throws java.sql.SQLException
		public virtual int MaxRowSize
		{
			get
			{
				return 32766;
			}
		}



		/// <summary>
		/// Returns the maximum length allowed for a schema name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxSchemaNameLength() throws java.sql.SQLException
		public virtual int MaxSchemaNameLength
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_710)
				{
					return 128;
				}
				else
				{
					return 10;
				}
			}
		}



		/// <summary>
		/// Returns the maximum length of an SQL statement.
		/// </summary>
		/// <returns>     The maximum length.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxStatementLength() throws java.sql.SQLException
		public virtual int MaxStatementLength
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_540)
				{
					return 1048576;
				}
				else
				{
					return 32767;
				}
			}
		}



		/// <summary>
		/// Returns the number of active statements you can have open at one
		/// time.
		/// </summary>
		/// <returns>     The maximum number of statements or 0
		///            if no limit.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxStatements() throws java.sql.SQLException
		public virtual int MaxStatements
		{
			get
			{
				return 9999;
			}
		}



		/// <summary>
		/// Returns the maximum length of a table name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxTableNameLength() throws java.sql.SQLException
		public virtual int MaxTableNameLength
		{
			get
			{
				return 128;
			}
		}



		/// <summary>
		/// Returns the maximum number of tables in a SELECT.
		/// </summary>
		/// <returns>     The maximum number of tables.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxTablesInSelect() throws java.sql.SQLException
		public virtual int MaxTablesInSelect
		{
			get
			{
				if (connection_.ServerVersion >= SystemInfo.VERSION_540)
				{
					return 1000;
				}
				else
				{
					return 32;
				}
			}
		}



		/// <summary>
		/// Returns the maximum length of a user name.
		/// </summary>
		/// <returns>     The maximum length (in bytes).
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaxUserNameLength() throws java.sql.SQLException
		public virtual int MaxUserNameLength
		{
			get
			{
				return 10;
			}
		}



		/// <summary>
		/// Returns the list of supported math functions.
		/// </summary>
		/// <returns>     The list of supported math functions, separated by commas.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNumericFunctions() throws java.sql.SQLException
		public virtual string NumericFunctions
		{
			get
			{
					return "abs,acos,asin,atan,atan2,ceiling,cos,cot,degrees,exp,floor,log,log10,mod,pi,power,radians,rand,round,sin,sign,sqrt,tan,truncate";
			}
		}



		/// <summary>
		/// Returns a description of the primary key columns. </summary>
		/// <param name="catalog">      The catalog name. If null is specified, this parameter
		///                     is ignored.  If empty string is specified,
		///                     an empty result set is returned. </param>
		/// <param name="schema">       The schema name. If null is specified, the
		///                     default SQL schema specified in the URL is used.
		///                     If null is specified and a default SQL schema was not
		///                     specified in the URL, the first library specified
		///                     in the libraries properties file is used.
		///                     If null is specified, a default SQL schema was
		///                     not specified in the URL, and a library was not
		///                     specified in the libraries properties file,
		///                     QGPL is used.
		///                     If empty string is specified, an empty result set will
		///                     be returned. </param>
		/// <param name="table">        The table name. If null or empty string is specified,
		///                     an empty result set is returned.
		/// </param>
		/// <returns>              The ResultSet containing the description of the primary
		///                     key columns.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getPrimaryKeys(String catalog, String schema, String table) throws java.sql.SQLException
		public virtual ResultSet getPrimaryKeys(string catalog, string schema, string table)
		{


				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLPRIMARYKEYS(?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schema));
				cs.setString(3, normalize(table));
				cs.setString(4, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;
		}



		/// <summary>
		/// Returns a description of a catalog's stored procedure
		/// parameters and result columns.
		/// <para> Note:  For this function to work with procedure names
		/// longer than 10 characters, the metadata source=1 property must be used on the connection.
		/// This is the default when connecting to a V7R1 or later system.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog">            The catalog name. If null is specified, this parameter
		///                           is ignored.  If empty string is specified,
		///                           an empty result set is returned. </param>
		/// <param name="schemaPattern">      The schema name pattern.   If null is specified,
		///                           it will not be included in the selection
		///                           criteria. If empty string
		///                           is specified, an empty result set is returned. </param>
		/// <param name="procedurePattern">   The procedure name pattern. If null is specified,
		///                           it will not be included in the selection criteria.
		///                           If empty string
		///                           is specified, an empty result set is returned. </param>
		/// <param name="columnPattern">      The column name pattern.  If null is specified,
		///                           it will not be included in the selection criteria.
		///                           If empty string
		///                           is specified, an empty result set is returned.
		/// </param>
		/// <returns>                    The ResultSet containing the description of the
		///                           catalog's stored procedure parameters and result
		///                           columns.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getProcedureColumns(String catalog, String schemaPattern, String procedurePattern, String columnPattern) throws java.sql.SQLException
		public virtual ResultSet getProcedureColumns(string catalog, string schemaPattern, string procedurePattern, string columnPattern)
		{

				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLPROCEDURECOLS(?,?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schemaPattern));
				cs.setString(3, normalize(procedurePattern));
				cs.setString(4, normalize(columnPattern));
				if (javaVersion >= 16)
				{
					cs.setString(5, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				}
				else
				{
					cs.setString(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				}
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;


		} // End of getProcedureColumns



		/// <summary>
		/// Returns the description of the stored procedures available
		/// in a catalog.
		/// </summary>
		/// <param name="catalog">            The catalog name. If null is specified, this parameter
		///                           is ignored.  If empty string is specified,
		///                           an empty result set is returned. </param>
		/// <param name="schemaPattern">      The schema name pattern.  If null is specified,
		///                           it will not be included in the selection
		///                           criteria.  If empty string
		///                           is specified, an empty result set is returned. </param>
		/// <param name="procedurePattern">   The procedure name pattern. If null is specified,
		///                           it will not be included in the selection
		///                           criteria.  If empty string
		///                           is specified, an empty result set is returned.
		/// </param>
		/// <returns>                    The ResultSet containing the description of the
		///                           stored procedures available in the catalog.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getProcedures(String catalog, String schemaPattern, String procedurePattern) throws java.sql.SQLException
		public virtual ResultSet getProcedures(string catalog, string schemaPattern, string procedurePattern)
		{

				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLPROCEDURES(?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schemaPattern));
				cs.setString(3, normalize(procedurePattern));
				cs.setString(4, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;

		}



		/// <summary>
		/// Returns the DB2 for IBM i SQL term for "procedure".
		/// </summary>
		/// <returns>     The term for "procedure".
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getProcedureTerm() throws java.sql.SQLException
		public virtual string ProcedureTerm
		{
			get
			{
				return "Procedure";
			}
		}




		/// <summary>
		/// Retrieves the default holdability of this ResultSet object.  Holdability is
		/// whether ResultSet objects are kept open when the statement is committed.
		/// </summary>
		/// <returns>     Always ResultSet.HOLD_CURSORS_OVER_COMMIT.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// 
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getResultSetHoldability() throws java.sql.SQLException
		public virtual int ResultSetHoldability
		{
			get
			{
				return JDBCResultSet.HOLD_CURSORS_OVER_COMMIT;
			}
		}



		/// <summary>
		/// Returns the schema names available in this database.
		/// This will return a ResultSet with a list of all the
		/// libraries.
		/// </summary>
		/// <returns>             The ResultSet containing the list of all the libraries.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getSchemas() throws java.sql.SQLException
		public virtual ResultSet Schemas
		{
			get
			{
    
					CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLTABLES(?,?,?,?,?)");
    
					cs.setString(1, "%");
					cs.setString(2, "%");
					cs.setString(3, "%");
					cs.setString(4, "%");
					cs.setString(5, "DATATYPE='JDBC';GETSCHEMAS=1;CURSORHOLD=1");
					cs.execute();
					ResultSet rs = cs.ResultSet;
					if (rs != null)
					{
						((JDBCResultSet)rs).isMetadataResultSet_ = true;
					}
					else
					{
						cs.close();
						// It is an error not to have received a result set
						JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
					}
					return rs;
			}
		}



		/// <summary>
		/// Returns the DB2 for IBM i SQL term for "schema".
		/// </summary>
		/// <returns>     The term for schema.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSchemaTerm() throws java.sql.SQLException
		public virtual string SchemaTerm
		{
			get
			{
				return "Library";
			}
		}



		/// <summary>
		/// Returns the string used to escape wildcard characters.
		/// This is the string that can be used to escape '_' or '%'
		/// in string patterns.
		/// </summary>
		/// <returns>     The escape string.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSearchStringEscape() throws java.sql.SQLException
		public virtual string SearchStringEscape
		{
			get
			{
				return "\\";
			}
		}



		/// <summary>
		/// Returns the list of all of the database's SQL keywords
		/// that are not also SQL92 keywords.
		/// </summary>
		/// <returns>     The list of SQL keywords, separated by commas.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSQLKeywords() throws java.sql.SQLException
		public virtual string SQLKeywords
		{
			get
			{
				return "AFTER,ALIAS,ALLOW,APPLICATION,ASSOCIATE,ASUTIME,AUDIT," + // @J2M
					   "AUX,AUXILIARY,BEFORE,BINARY," + // @J2A
					   "BUFFERPOOL,CACHE,CALL,CALLED,CAPTURE,CARDINALITY,CCSID,CLUSTER," + // @J2A
					   "COLLECTION,COLLID,COMMENT,CONCAT,CONDITION,CONTAINS,COUNT_BIG," + // @J2A
					   "CURRENT_LC_CTYPE," + // @J2A
					   "CURRENT_PATH,CURRENT_SERVER,CURRENT_TIMEZONE,CYCLE,DATA," + // @J2A
					   "DATABASE,DAYS," + // @J2A
					   "DB2GENERAL,DB2GENRL,DB2SQL,DBINFO,DEFAULTS,DEFINITION," + // @J2A
					   "DETERMINISTIC," + // @J2A
					   "DISALLOW,DO,DSNHATTR,DSSIZE,DYNAMIC,EACH,EDITPROC,ELSEIF," + // @J2A
					   "ENCODING,END-EXEC1," + // @J2A
					   "ERASE,EXCLUDING,EXIT,FENCED,FIELDPROC,FILE,FINAL,FREE,FUNCTION," + // @J2A
					   "GENERAL," + // @J2A
					   "GENERATED,GRAPHIC,HANDLER,HOLD,HOURS,IF,INCLUDING,INCREMENT," + // @J2A
					   "INDEX," + // @J2A
					   "INHERIT,INOUT,INTEGRITY,ISOBID,ITERATE,JAR,JAVA,LABEL,LC_CTYPE," + // @J2A
					   "LEAVE," + // @J2A
					   "LINKTYPE,LOCALE,LOCATOR,LOCATORS,LOCK,LOCKMAX,LOCKSIZE,LONG,LOOP," + // @J2A
					   "MAXVALUE,MICROSECOND,MICROSECONDS,MINUTES,MINVALUE,MODE,MODIFIES," + // @J2A
					   "MONTHS," + // @J2A
					   "NEW,NEW_TABLE,NOCACHE,NOCYCLE,NODENAME,NODENUMBER,NOMAXVALUE," + // @J2A
					   "NOMINVALUE," + // @J2A
					   "NOORDER,NULLS,NUMPARTS,OBID,OLD,OLD_TABLE,OPTIMIZATION,OPTIMIZE," + // @J2A
					   "OUT,OVERRIDING,PACKAGE,PARAMETER,PART,PARTITION,PATH,PIECESIZE," + // @J2A
					   "PLAN," + // @J2A
					   "PRIQTY,PROGRAM,PSID,QUERYNO,READS,RECOVERY,REFERENCING,RELEASE," + // @J2A
					   "RENAME,REPEAT,RESET,RESIGNAL,RESTART,RESULT,RESULT_SET_LOCATOR," + // @J2A
					   "RETURN," + // @J2A
					   "RETURNS,ROUTINE,ROW,RRN,RUN,SAVEPOINT,SCRATCHPAD,SECONDS,SECQTY," + // @J2A
					   "SECURITY,SENSITIVE,SIGNAL,SIMPLE,SOURCE,SPECIFIC,SQLID,STANDARD," + // @J2A
					   "START,STATIC,STAY,STOGROUP,STORES,STYLE,SUBPAGES,SYNONYM,SYSFUN," + // @J2A
					   "SYSIBM," + // @J2A
					   "SYSPROC,SYSTEM,TABLESPACE,TRIGGER,TYPE,UNDO,UNTIL,VALIDPROC," + // @J2A
					   "VARIABLE," + // @J2A
					   "VARIANT,VCAT,VOLUMES,WHILE,WLM,YEARS"; // @J2A
			}
		}




		/// <summary>
		/// Indicates whether the SQLSTATEs returned by SQLException.getSQLState is X/Open SQL CLI or
		/// SQL99.
		/// </summary>
		/// <returns>     Always sqlStateSQL99.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSQLStateType() throws java.sql.SQLException
		public virtual int SQLStateType
		{
			get
			{
				return JDBCDatabaseMetaData.sqlStateSQL99;
			}
		}



		/// <summary>
		/// Returns the list of supported string functions.
		/// </summary>
		/// <returns>     The list of supported string functions, separated by commas.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getStringFunctions() throws java.sql.SQLException
		public virtual string StringFunctions
		{
			get
			{
				return "char,concat,difference,insert,lcase,left,length,locate,ltrim,repeat,replace,right,rtrim,soundex,space,substring,ucase";
			}
		}




		/// <summary>
		/// Returns a ResultSet containing descriptions of the table hierarchies
		/// in a schema.
		/// 
		/// This method only applies to the attributes of a
		/// structured type.  Distinct types are stored in the datatypes
		/// catalog, not the attributes catalog.  Since DB2 for IBM i does not support
		/// structured types at this time, an empty ResultSet will always be returned
		/// for calls to this method.
		/// </summary>
		/// <param name="catalog">         The catalog name. </param>
		/// <param name="schemaPattern">   The schema name pattern. </param>
		/// <param name="typeNamePattern"> The type name pattern.
		/// </param>
		/// <returns>      The empty ResultSet
		/// </returns>
		/// <exception cref="SQLException">  This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getSuperTables(String catalog, String schemaPattern, String typeNamePattern) throws java.sql.SQLException
		public virtual ResultSet getSuperTables(string catalog, string schemaPattern, string typeNamePattern)
		{
			// We return an empty result set because this is not supported by our driver
			Statement statement = connection_.createStatement();
			return statement.executeQuery("SELECT VARCHAR('1', 128) AS TYPE_CAT, " + "VARCHAR('2', 128) AS TYPE_SCHEM, " + "VARCHAR('3', 128) AS TYPE_NAME, " + "VARCHAR('4', 128) AS SUPERTYPE_NAME " + "FROM QSYS2" + CatalogSeparator + "SYSTYPES WHERE 1 = 2 FOR FETCH ONLY ");
		}




		/// <summary>
		/// Returns a ResultSet containing descriptions of user-defined type hierarchies
		/// in a schema.
		/// 
		/// This method only applies to the attributes of a
		/// structured type.  Distinct types are stored in the datatypes
		/// catalog, not the attributes catalog. Since DB2 for IBM i does not support
		/// structured types at this time, an empty ResultSet will always be returned
		/// for calls to this method.
		/// </summary>
		/// <param name="catalog">         The catalog name. </param>
		/// <param name="schemaPattern">   The schema name pattern. </param>
		/// <param name="typeNamePattern"> The type name pattern.
		/// </param>
		/// <returns>      The empty result set
		/// </returns>
		/// <exception cref="SQLException">  This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getSuperTypes(String catalog, String schemaPattern, String typeNamePattern) throws java.sql.SQLException
		public virtual ResultSet getSuperTypes(string catalog, string schemaPattern, string typeNamePattern)
		{
			// We return an empty result set because this is not supported by our driver
			Statement statement = connection_.createStatement();
			return statement.executeQuery("SELECT VARCHAR('1', 128) AS TYPE_CAT, " + "VARCHAR('2', 128) AS TYPE_SCHEM, " + "VARCHAR('3', 128) AS TYPE_NAME, " + "VARCHAR('4', 128) AS SUPERTYPE_CAT, " + "VARCHAR('5', 128) AS SUPERTYPE_SCHEM, " + "VARCHAR('6', 128) AS SUPERTYPE_NAME " + "FROM QSYS2" + CatalogSeparator + "SYSTYPES WHERE 1 = 2 FOR FETCH ONLY ");
		}



		/// <summary>
		/// Returns the list of supported system functions.
		/// </summary>
		/// <returns>     The list of supported system functions, separated by commas.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSystemFunctions() throws java.sql.SQLException
		public virtual string SystemFunctions
		{
			get
			{
				return "database,ifnull,user";
			}
		}



		/// <summary>
		/// Returns the description of the access rights for each table
		/// available in a catalog.  Note that a table privilege applies
		/// to one or more columns in a table.
		/// </summary>
		/// <param name="catalog">             The catalog name. If null is specified, this parameter
		///                            is ignored.  If empty string is specified,
		///                            an empty result set is returned. </param>
		/// <param name="schemaPattern">       The schema name pattern.
		///                            If the "metadata source" connection property is set to 0
		///                            and null is specified, no value is sent to the system and
		///                            the default of *USRLIBL is used.
		///                            If the "metadata source" connection property is set to 1
		///                            and null is specified, then information from all schemas
		///                            will be returned.
		/// 
		///                            If empty string is specified, an empty
		///                            result set is returned. </param>
		/// <param name="tablePattern">        The table name. If null is specified,
		///                            no value is sent to the system and the system
		///                            default of *ALL is used.  If empty string
		///                            is specified, an empty result set is returned. </param>
		/// <returns>                     The ResultSet containing the description of the
		///                            access rights for each table available in the
		///                            catalog.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getTablePrivileges(String catalog, String schemaPattern, String tablePattern) throws java.sql.SQLException
		public virtual ResultSet getTablePrivileges(string catalog, string schemaPattern, string tablePattern)
		{

				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLTABLEPRIVILEGES(?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schemaPattern));
				cs.setString(3, normalize(tablePattern));
				cs.setString(4, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;
		}



		/// <summary>
		/// Returns the description of the tables available in a catalog.
		/// </summary>
		/// <param name="catalog">        The catalog name. If null is specified, this parameter
		///                       is ignored.  If empty string is specified,
		///                       an empty result set is returned. </param>
		/// <param name="schemaPattern">  The schema name pattern.
		///                       If the "metadata source" connection property is set to 0
		///                       and null is specified, no value is sent to the system and
		///                       the default of *USRLIBL is used.
		///                       If the "metadata source" connection property is set to 1
		///                       and null is specified, then information from all schemas
		///                       will be returned.  If an empty string
		///                       is specified, an empty result set is returned. </param>
		/// <param name="tablePattern">   The table name pattern. If null is specified,
		///                       no value is sent to the system and the system
		///                       default of *ALL is used.  If empty string
		///                       is specified, an empty result set is returned. </param>
		/// <param name="tableTypes">     The list of table types to include, or null to
		///                       include all table types. Valid types are:
		///                       TABLE, VIEW, SYSTEM TABLE, MATERIALIZED QUERY TABLE, and ALIAS. </param>
		/// <returns>                The ResultSet containing the description of the
		///                       tables available in the catalog.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getTables(String catalog, String schemaPattern, String tablePattern, String tableTypes[]) throws java.sql.SQLException
		public virtual ResultSet getTables(string catalog, string schemaPattern, string tablePattern, string[] tableTypes)
		{


		   // Verify that a connection
			// is available for use. Exception
			// is thrown if not available

				// Handle processing the array of table types.
				//bite the bullet and follow Native JDBC logic
				bool rsEmpty = false;
				string typeString = EMPTY_STRING;
				if (!rsEmpty)
				{
					int i;
					int stringsInList = 0;

					if (tableTypes != null)
					{
						for (i = 0; i < tableTypes.Length; i++)
						{
							string check = tableTypes[i];

							if ((check.Equals(VIEW, StringComparison.OrdinalIgnoreCase)) || (check.Equals(TABLE, StringComparison.OrdinalIgnoreCase)) || (check.Equals(SYSTEM_TABLE, StringComparison.OrdinalIgnoreCase)) || (check.Equals(ALIAS, StringComparison.OrdinalIgnoreCase)) || (check.Equals(SYNONYM, StringComparison.OrdinalIgnoreCase)) || (check.Equals(MQT, StringComparison.OrdinalIgnoreCase)))
							{

								if (check.Equals(SYNONYM, StringComparison.OrdinalIgnoreCase))
								{
									check = ALIAS;
								}
								stringsInList++;
								if (stringsInList > 1)
								{
									typeString = typeString + ",";
								}
								typeString = typeString + check;
							}
						}

						// If there were no valid types, ensure an empty result set.
						if (stringsInList == 0)
						{
							rsEmpty = true;
						}
					}
				}

				// If an empty result set is to be generated, produce the values to
				// do so here.
				if (rsEmpty)
				{
					schemaPattern = FAKE_VALUE;
					tablePattern = FAKE_VALUE;
					typeString = typeString + TABLE;
				}


				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLTABLES(?,?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schemaPattern));
				cs.setString(3, normalize(tablePattern));
				cs.setString(4, normalize(typeString));
				cs.setString(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();
				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;

		}


		/// <summary>
		/// Returns the table types available in this database.
		/// </summary>
		/// <returns>     The ResultSet containing the table types available in
		///            this database.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getTableTypes() throws java.sql.SQLException
		public virtual ResultSet TableTypes
		{
			get
			{
    
    
				  CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLTABLES(?,?,?,?,?)");
    
				  cs.setString(1, "%");
				  cs.setString(2, "%");
				  cs.setString(3, "%");
				  cs.setString(4, "%");
				  cs.setString(5, "DATATYPE='JDBC';GETTABLETYPES=1;CURSORHOLD=1");
				  cs.execute();
				  ResultSet rs = cs.ResultSet;
				  if (rs != null)
				  {
					  ((JDBCResultSet)rs).isMetadataResultSet_ = true;
				  }
				  else
				  {
					  cs.close();
					  // It is an error not to have received a result set
					  JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				  }
				  return rs;
    
			}
		}



		/// <summary>
		/// Returns the list of supported time and date functions.
		/// </summary>
		/// <returns>     The list of supported time and data functions,
		///            separated by commas.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getTimeDateFunctions() throws java.sql.SQLException
		public virtual string TimeDateFunctions
		{
			get
			{
				return "curdate,curtime,dayname,dayofmonth,dayofweek,dayofyear,hour,minute,month,monthname,now,quarter,second,timestampdiff,week,year";
			}
		}



		/// <summary>
		/// Returns a description of all of the standard SQL types
		/// supported by this database.
		/// </summary>
		/// <returns>    The ResultSet containing the description of all
		///           the standard SQL types supported by this
		///           database.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getTypeInfo() throws java.sql.SQLException
		public virtual ResultSet TypeInfo
		{
			get
			{
    
    
					PreparedStatement cs = connection_.prepareStatement("CALL SYSIBM" + CatalogSeparator + "SQLGETTYPEINFO(?,?)");
    
					cs.setShort(1, (short) SQL_ALL_TYPES);
					if (javaVersion >= 16)
					{
					cs.setString(2, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
					}
					else
					{
					cs.setString(2, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
					}
					cs.execute();
					ResultSet rs = cs.ResultSet;
					if (rs != null)
					{
						((JDBCResultSet)rs).isMetadataResultSet_ = true;
					}
					else
					{
						cs.close();
					}
    
					return rs;
    
    
			}
		}



		// JDBC 2.0
		/// <summary>
		/// Returns the description of the user-defined types
		/// available in a catalog.
		/// </summary>
		/// <param name="catalog">         The catalog name. If null is specified, this parameter
		///                        is ignored.  If empty string is specified,
		///                        an empty result set is returned. </param>
		/// <param name="schemaPattern">   The schema name pattern.
		///                        If the "metadata source" connection property is set to 0
		///                        and null is specified, no value is sent to the system and
		///                        the default of *USRLIBL is used.
		///                        If the "metadata source" connection property is set to 1
		///                        and null is specified, then information from all schemas
		///                        will be returned.
		///                        If an empty string
		///                        is specified, an empty result set is returned. </param>
		/// <param name="typeNamePattern"> The type name pattern. If null is specified,
		///                        no value is sent to the system and the system
		///                        default of *ALL is used.  If empty string
		///                        is specified, an empty result set is returned. </param>
		/// <param name="types">           The list of user-defined types to include, or null to
		///                        include all user-defined types. Valid types are:
		///                        JAVA_OBJECT, STRUCT, and DISTINCT. </param>
		/// <returns>                 The ResultSet containing the description of the
		///                        user-defined available in the catalog.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
		//
		// Implementation note:
		//
		// 1. I was worried about cases where one distinct type is created
		//    based on another distinct type.  This would cause problems
		//    because the source type would no longer identify a system
		//    predefined type and we would have to follow the chain until
		//    we found a system predefined type.
		//
		//    It turns out that this is not an issue.  In the "JDBC Tutorial and
		//    Reference", section 3.5.5 "Creating a DISTINCT type", it says:
		//    "A DISTINCT type is always based on another data type, which must
		//    be a predefined type.  ... a DISTINCT type cannot be based
		//    on a UDT."  ("UDT" means user-defined type.)
		//    So we can make the assumption that the source type
		//    always identifies a system predefined type.
		//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getUDTs(String catalog, String schemaPattern, String typeNamePattern, int[] types) throws java.sql.SQLException
		public virtual ResultSet getUDTs(string catalog, string schemaPattern, string typeNamePattern, int[] types)
		{





				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLUDTS(?,?,?,?,?)");

				cs.setString(1, normalize(catalog));
				cs.setString(2, normalize(schemaPattern));
				cs.setString(3, normalize(typeNamePattern));
				StringBuilder typesStringBuffer = new StringBuilder();
				int stringsInList = 0;

				if (types != null)
				{
					for (int i = 0; i < types.Length; i++)
					{
						if (stringsInList > 0)
						{
							typesStringBuffer.Append(",");
						}
						typesStringBuffer.Append(types[i]);
						stringsInList++;
					}
				}

				cs.setString(4, typesStringBuffer.ToString());
				if (javaVersion >= 16)
				{
					cs.setString(5, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				}
				else
				{
					cs.setString(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				}
				cs.execute();
				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;
		}



		/// <summary>
		/// Returns the URL for this database.
		/// </summary>
		/// <returns>     The URL for this database.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getURL() throws java.sql.SQLException
		public virtual string URL
		{
			get
			{
    
				return connection_.URL;
			}
		}



		/// <summary>
		/// Returns the current user name as known to the database.
		/// </summary>
		/// <returns>     The current user name as known to the database.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getUserName() throws java.sql.SQLException
		public virtual string UserName
		{
			get
			{
    
				return connection_.UserName;
			}
		}



		/// <summary>
		/// Returns a description of a table's columns that are automatically
		/// updated when any value in a row is updated. </summary>
		/// <param name="catalog">        The catalog name. If null is specified, this parameter
		///                       is ignored.  If empty string is specified,
		///                       an empty result set is returned. </param>
		/// <param name="schema">         The schema name. If null is specified, the
		///                       default SQL schema specified in the URL is used.
		///                       If null is specified and a default SQL schema was not
		///                       specified in the URL, the first library specified
		///                       in the libraries properties file is used.
		///                       If null is specified, a default SQL schema was
		///                       not specified in the URL, and a library was not
		///                       specified in the libraries properties file,
		///                       QGPL is used.
		///                       If empty string is specified, an empty result set will
		///                       be returned. </param>
		/// <param name="table">          The table name. If null or empty string is specified,
		///                       an empty result set is returned.
		/// </param>
		/// <returns>                The ResultSet containing the description of the
		///                       table's columns that are automatically updated
		///                       when any value in a row is updated.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getVersionColumns(String catalog, String schema, String table) throws java.sql.SQLException
		public virtual ResultSet getVersionColumns(string catalog, string schema, string table)
		{




				CallableStatement cs = connection_.prepareCall("CALL SYSIBM" + CatalogSeparator + "SQLSPECIALCOLUMNS(?,?,?,?, ?,?,?)");

				cs.setShort(1, (short) SQL_ROWVER);
				cs.setString(2, normalize(catalog));
				cs.setString(3, normalize(schema));
				cs.setString(4, normalize(table));
				cs.setShort(5, (short) 0);
				cs.setShort(6, (short) 1);
				cs.setString(7, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
				cs.execute();

				ResultSet rs = cs.ResultSet;
				if (rs != null)
				{
					((JDBCResultSet)rs).isMetadataResultSet_ = true;
				}
				else
				{
					cs.close();
					// It is an error not to have received a result set
					JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
				}
				return rs;
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if visible inserts to a result set of the specified type
		/// can be detected by calling ResultSet.rowInserted().
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     Always false.  Inserts can not be detected
		///                            by calling ResultSet.rowInserted().
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean insertsAreDetected(int resultSetType) throws java.sql.SQLException
		public virtual bool insertsAreDetected(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return false;
		}



		/// <summary>
		/// Indicates if a catalog appears at the start or the end of
		/// a qualified name.
		/// </summary>
		/// <returns>     Always true. A catalog appears at the start of a
		///            qualified name.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isCatalogAtStart() throws java.sql.SQLException
		public virtual bool CatalogAtStart
		{
			get
			{
				return true;
			}
		}






		/// <summary>
		/// Indicates if the database is in read-only mode.
		/// </summary>
		/// <returns>     true if in read-only mode; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the connection is not open
		///                            or an error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReadOnly() throws java.sql.SQLException
		public virtual bool ReadOnly
		{
			get
			{
    
				return connection_.ReadOnly;
			}
		}



		/// <summary>
		/// Indicates if updateable LOB methods update a copy of the LOB or if updates
		/// are made directly to the LOB.  True is returned if updateable lob methods
		/// update a copy of the LOB, false is returned if updates are made directly
		/// to the LOB.
		/// </summary>
		/// <returns>     Always true.    Updateable lob methods update a copy of the LOB.
		/// ResultSet.updateRow() must be called to update the LOB in the DB2 for IBM i database.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean locatorsUpdateCopy() throws java.sql.SQLException
		public virtual bool locatorsUpdateCopy()
		{
			return true;
		}



		// @E4A
		// The database is not case-sensitive except when names are quoted with double
		// quotes.  The host server flows are case-sensitive, so I will uppercase
		// everything to save the caller from having to do so.
		private string normalize(string mixedCaseName)
		{
			if (string.ReferenceEquals(mixedCaseName, null))
			{
				return null;
			}

			return mixedCaseName.ToUpper();
		}



		/// <summary>
		/// Indicates if concatenations between null and non-null values
		/// are null.
		/// 
		/// </summary>
		/// <returns>     Always true. Concatenations between null and non-null
		///            values are null.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nullPlusNonNullIsNull() throws java.sql.SQLException
		public virtual bool nullPlusNonNullIsNull()
		{
			return true;
		}



		/// <summary>
		/// Indicates if null values are sorted at the end regardless of sort
		/// order.
		/// </summary>
		/// <returns>     Always false. Null values are not sorted at the end
		///            regardless of sort order.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nullsAreSortedAtEnd() throws java.sql.SQLException
		public virtual bool nullsAreSortedAtEnd()
		{
			return false;
		}



		/// <summary>
		/// Indicates if null values are sorted at the start regardless of sort
		/// order.
		/// </summary>
		/// <returns>     Always false. Null values are not sorted at the start
		///            regardless of sort order.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nullsAreSortedAtStart() throws java.sql.SQLException
		public virtual bool nullsAreSortedAtStart()
		{
			return false;
		}



		/// <summary>
		/// Indicates if null values are sorted high.
		/// </summary>
		/// <returns>     Always true. Null values are sorted high.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nullsAreSortedHigh() throws java.sql.SQLException
		public virtual bool nullsAreSortedHigh()
		{
			return true;
		}



		/// <summary>
		/// Indicates if null values are sorted low.
		/// </summary>
		/// <returns>     Always false. Null values are not sorted low.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nullsAreSortedLow() throws java.sql.SQLException
		public virtual bool nullsAreSortedLow()
		{
			return false;
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if deletes made by others are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if deletes made by others
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean othersDeletesAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool othersDeletesAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if inserts made by others are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if inserts made by others
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean othersInsertsAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool othersInsertsAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if updates made by others are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if updates made by others
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean othersUpdatesAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool othersUpdatesAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if a result set's own deletes are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if the result set's own deletes
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ownDeletesAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool ownDeletesAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if a result set's own inserts are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if the result set's own inserts
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ownInsertsAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool ownInsertsAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if a result set's own updates are visible.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true if the result set's own updates
		///                            are visible; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ownUpdatesAreVisible(int resultSetType) throws java.sql.SQLException
		public virtual bool ownUpdatesAreVisible(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return (resultSetType == ResultSet.TYPE_SCROLL_SENSITIVE);
		}



		/// <summary>
		/// Indicates if the database treats mixed case, unquoted SQL identifiers
		/// as case insensitive and stores them in lowercase.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case,
		///            unquoted SQL identifiers as case insensitive and store
		///            them in lowercase.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesLowerCaseIdentifiers() throws java.sql.SQLException
		public virtual bool storesLowerCaseIdentifiers()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, quoted SQL identifiers
		/// as case insensitive and stores them in lowercase.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case, quoted
		///            SQL identifiers as case insensitive and store them in
		///            lowercase.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesLowerCaseQuotedIdentifiers() throws java.sql.SQLException
		public virtual bool storesLowerCaseQuotedIdentifiers()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, unquoted SQL identifiers
		/// as case insensitive and stores them in mixed case.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case, unquoted
		///            SQL identifiers as case insensitive and store them in
		///            mixed case.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesMixedCaseIdentifiers() throws java.sql.SQLException
		public virtual bool storesMixedCaseIdentifiers()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, quoted SQL identifiers
		/// as case insensitive and stores them in mixed case.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case, quoted
		///            SQL identifiers as case insensitive and store them in
		///            mixed case.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesMixedCaseQuotedIdentifiers() throws java.sql.SQLException
		public virtual bool storesMixedCaseQuotedIdentifiers()
		{
			// @A2C changed from false to true
			return false;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, unquoted SQL identifiers
		/// as case insensitive and stores them in uppercase.
		/// </summary>
		/// <returns>     Always true. The database does treat mixed case, unquoted
		///            SQL identifiers as case insensitive and store them
		///            in uppercase.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesUpperCaseIdentifiers() throws java.sql.SQLException
		public virtual bool storesUpperCaseIdentifiers()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, quoted SQL identifiers
		/// as case insensitive and stores them in uppercase.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case, quoted
		///            SQL identifiers as case insensitive and store them
		///            in uppercase.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean storesUpperCaseQuotedIdentifiers() throws java.sql.SQLException
		public virtual bool storesUpperCaseQuotedIdentifiers()
		{
			return false;
		}



		/// <summary>
		/// Indicates if ALTER TABLE with ADD COLUMN is supported.
		/// </summary>
		/// <returns>     Always true.   ALTER TABLE with ADD COLUMN is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsAlterTableWithAddColumn() throws java.sql.SQLException
		public virtual bool supportsAlterTableWithAddColumn()
		{
			// @A2C Changed from false to true
			return true;
		}



		/// <summary>
		/// Indicates if ALTER TABLE with DROP COLUMN is supported.
		/// </summary>
		/// <returns>     Always true. ALTER TABLE with DROP COLUMN is not supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsAlterTableWithDropColumn() throws java.sql.SQLException
		public virtual bool supportsAlterTableWithDropColumn()
		{
			// @A2C Changed from false to true
			return true;
		}



		/// <summary>
		/// Indicates if the ANSI92 entry-level SQL grammar is supported.
		/// </summary>
		/// <returns>     Always true. The ANSI92 entry-level SQL grammar is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsANSI92EntryLevelSQL() throws java.sql.SQLException
		public virtual bool supportsANSI92EntryLevelSQL()
		{
			// ANSI92EntryLevelSQL is supported for V4R2 and beyond
			// true is always returned since it is checked for compliance.
			return true;

		}



		/// <summary>
		/// Indicates if the ANSI92, full SQL grammar is supported.
		/// </summary>
		/// <returns>     Always false. ANSI92, full SQL grammar is not supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsANSI92FullSQL() throws java.sql.SQLException
		public virtual bool supportsANSI92FullSQL()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the ANSI92 intermediate-level SQL grammar is supported.
		/// </summary>
		/// <returns>     Always false. ANSI92 intermediate-level SQL grammar is not supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsANSI92IntermediateSQL() throws java.sql.SQLException
		public virtual bool supportsANSI92IntermediateSQL()
		{
			return false;
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if the batch updates are supported.
		/// </summary>
		/// <returns>     Always true. Batch updates are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsBatchUpdates() throws java.sql.SQLException
		public virtual bool supportsBatchUpdates()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a catalog name can be used in a data manipulation
		/// statement.
		/// </summary>
		/// <returns>     Always false. A catalog name can not be used in a data manipulation
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCatalogsInDataManipulation() throws java.sql.SQLException
		public virtual bool supportsCatalogsInDataManipulation()
		{
			// @A2 Changed from true to false.
			return false;
		}



		/// <summary>
		/// Indicates if a catalog name can be used in an index definition
		/// statement.
		/// </summary>
		/// <returns>     Always false. A catalog name can not be used in an index definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCatalogsInIndexDefinitions() throws java.sql.SQLException
		public virtual bool supportsCatalogsInIndexDefinitions()
		{
			// @A2C Changed from true to false
			return false;
		}



		/// <summary>
		/// Indicates if a catalog name can be used in a privilege definition
		/// statement.
		/// </summary>
		/// <returns>     Always false. A catalog name can not be used in a privilege definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCatalogsInPrivilegeDefinitions() throws java.sql.SQLException
		public virtual bool supportsCatalogsInPrivilegeDefinitions()
		{
			// @A2C Changed from true to false
			return false;
		}



		/// <summary>
		/// Indicates if a catalog name can be used in a procedure call
		/// statement.
		/// </summary>
		/// <returns>     Always false. A catalog name can not be used in a procedure call
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCatalogsInProcedureCalls() throws java.sql.SQLException
		public virtual bool supportsCatalogsInProcedureCalls()
		{
			// @A2C Changed from true to false
			return false;
		}



		/// <summary>
		/// Indicates if a catalog name can be used in a table definition
		/// statement.
		/// </summary>
		/// <returns>     Always false. A catalog name can not be used in a table definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCatalogsInTableDefinitions() throws java.sql.SQLException
		public virtual bool supportsCatalogsInTableDefinitions()
		{
			// @A2C Changed from true to false
			return false;
		}



		/// <summary>
		/// Indicates if column aliasing is supported.  Column aliasing means
		/// that the SQL AS clause can be used to provide names for
		/// computed columns or to provide alias names for column
		/// as required.
		/// </summary>
		/// <returns>     Always true. Column aliasing is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsColumnAliasing() throws java.sql.SQLException
		public virtual bool supportsColumnAliasing()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the CONVERT function between SQL types is supported.
		/// </summary>
		/// <returns>     true if the CONVERT function between SQL types is supported;
		///            false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsConvert() throws java.sql.SQLException
		public virtual bool supportsConvert()
		{
			return false;
		}



		/// <summary>
		/// Indicates if CONVERT between the given SQL types is supported.
		/// </summary>
		/// <param name="fromType">        The SQL type code defined in java.sql.Types. </param>
		/// <param name="toType">          The SQL type code defined in java.sql.Types. </param>
		/// <returns>     true if CONVERT between the given SQL types is supported;
		///            false otherwise.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsConvert(int fromType, int toType) throws java.sql.SQLException
		public virtual bool supportsConvert(int fromType, int toType)
		{
			return false;
		}



		/// <summary>
		/// Indicates if the ODBC Core SQL grammar is supported.
		/// </summary>
		/// <returns>     Always true. The ODBC Core SQL grammar is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCoreSQLGrammar() throws java.sql.SQLException
		public virtual bool supportsCoreSQLGrammar()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the correlated subqueries are supported.
		/// </summary>
		/// <returns>     Always true. Correlated subqueries are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsCorrelatedSubqueries() throws java.sql.SQLException
		public virtual bool supportsCorrelatedSubqueries()
		{
			return true;
		}



		/// <summary>
		/// Indicates if both data definition and data manipulation statements
		/// are supported within a transaction.
		/// </summary>
		/// <returns>     Always true. Data definition and data manipulation statements
		///            are both supported within a transaction.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsDataDefinitionAndDataManipulationTransactions() throws java.sql.SQLException
		public virtual bool supportsDataDefinitionAndDataManipulationTransactions()
		{
			return true;
		}



		/// <summary>
		/// Indicates if data manipulation statements are supported within a transaction.
		/// </summary>
		/// <returns>     Always false.  Data manipulation statements are not supported within
		///            a transaction.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsDataManipulationTransactionsOnly() throws java.sql.SQLException
		public virtual bool supportsDataManipulationTransactionsOnly()
		{
			// @A2C Changed from true to false
			return false;
		}



		/// <summary>
		/// Indicates if table correlation names are supported, and if so, are they
		/// restricted to be different from the names of the tables.
		/// </summary>
		/// <returns>     Always false.  Table correlation names are not restricted
		///            to be different from the names of the tables.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsDifferentTableCorrelationNames() throws java.sql.SQLException
		public virtual bool supportsDifferentTableCorrelationNames()
		{
			return false;
		}



		/// <summary>
		/// Indicates if expressions in ORDER BY lists are supported.
		/// </summary>
		/// <returns>     Always true. Expression in ORDER BY lists are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsExpressionsInOrderBy() throws java.sql.SQLException
		public virtual bool supportsExpressionsInOrderBy()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the ODBC Extended SQL grammar is supported.
		/// </summary>
		/// <returns>     Always false. The ODBC Extended SQL grammar is not supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsExtendedSQLGrammar() throws java.sql.SQLException
		public virtual bool supportsExtendedSQLGrammar()
		{
			return false;
		}



		/// <summary>
		/// Indicates if full nested outer joins are supported.
		/// </summary>
		/// <returns>     Always false. Full nested outer joins are not supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsFullOuterJoins() throws java.sql.SQLException
		public virtual bool supportsFullOuterJoins()
		{
			if (connection_.ServerVersion >= SystemInfo.VERSION_610)
			{
				return true;
			}
			else
			{
				return false;
			}
		}




		/// <summary>
		/// Indicates if, after a statement is executed, auto-generated keys can be retrieved
		/// using the method Statement.getGeneratedKeys().
		/// </summary>
		/// <returns>     True if the user is connecting to a system running OS/400 V5R2
		/// or IBM i, otherwise false.  Auto-generated keys are supported
		/// only if connecting to a system running OS/400 V5R2 or IBM i.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsGetGeneratedKeys() throws java.sql.SQLException
		public virtual bool supportsGetGeneratedKeys()
		{
			return true;
		}



		/// <summary>
		/// Indicates if some form of the GROUP BY clause is supported.
		/// </summary>
		/// <returns>     Always true. Some form of GROUP BY clause is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsGroupBy() throws java.sql.SQLException
		public virtual bool supportsGroupBy()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a GROUP BY clause can add columns not in the SELECT
		/// provided it specifies all of the columns in the SELECT.
		/// </summary>
		/// <returns>     Always true. A GROUP BY clause can add columns not in the SELECT
		///            provided it specifies all of the columns in the SELECT.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsGroupByBeyondSelect() throws java.sql.SQLException
		public virtual bool supportsGroupByBeyondSelect()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a GROUP BY clause can use columns not in the SELECT.
		/// </summary>
		/// <returns>     Always true.  A GROUP BY clause can use columns not in the SELECT.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsGroupByUnrelated() throws java.sql.SQLException
		public virtual bool supportsGroupByUnrelated()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the SQL Integrity Enhancement Facility is supported.
		/// </summary>
		/// <returns>     Always false. The SQL Integrity Enhancement Facility is not
		///            supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsIntegrityEnhancementFacility() throws java.sql.SQLException
		public virtual bool supportsIntegrityEnhancementFacility()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the escape character in LIKE clauses is supported.
		/// </summary>
		/// <returns>     Always true. The escape character in LIKE clauses is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsLikeEscapeClause() throws java.sql.SQLException
		public virtual bool supportsLikeEscapeClause()
		{
			return true;
		}



		/// <summary>
		/// Indicates if there is limited support for outer joins.
		/// </summary>
		/// <returns>     Always true. There is limited support for outer joins.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsLimitedOuterJoins() throws java.sql.SQLException
		public virtual bool supportsLimitedOuterJoins()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the ODBC Minimum SQL grammar is supported.
		/// </summary>
		/// <returns>     Always true. The ODBC Minimum SQL grammar is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMinimumSQLGrammar() throws java.sql.SQLException
		public virtual bool supportsMinimumSQLGrammar()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, unquoted SQL
		/// identifiers as case sensitive and stores
		/// them in mixed case.
		/// </summary>
		/// <returns>     Always false. The database does not treat mixed case,
		///            unquoted SQL identifiers as case sensitive and as
		///            a result store them in mixed case.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMixedCaseIdentifiers() throws java.sql.SQLException
		public virtual bool supportsMixedCaseIdentifiers()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the database treats mixed case, quoted SQL
		/// identifiers as case sensitive and as a result stores
		/// them in mixed case.
		/// </summary>
		/// <returns>     Always true. The database does treat mixed case, quoted SQL
		///            identifiers as case sensitive and stores
		///            them in mixed case.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMixedCaseQuotedIdentifiers() throws java.sql.SQLException
		public virtual bool supportsMixedCaseQuotedIdentifiers()
		{
			return true;
		}




		/// <summary>
		/// Indicates if multiple result sets can be returned from a
		/// CallableStatement simultaneously.
		/// </summary>
		/// <returns>     Always false.  Multiple open result sets from a single execute
		///            are not supported by the Toolbox driver.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMultipleOpenResults() throws java.sql.SQLException
		public virtual bool supportsMultipleOpenResults()
		{
			return false;
		}



		/// <summary>
		/// Indicates if multiple result sets from a single execute are
		/// supported.
		/// </summary>
		/// <returns>     Always true. Multiple result sets from a single execute
		///            are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMultipleResultSets() throws java.sql.SQLException
		public virtual bool supportsMultipleResultSets()
		{
			return true;
		}



		/// <summary>
		/// Indicates if multiple transactions can be open at once (on
		/// different connections).
		/// </summary>
		/// <returns>     Always true. Multiple transactions can be open at
		///            once on different connections.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMultipleTransactions() throws java.sql.SQLException
		public virtual bool supportsMultipleTransactions()
		{
			return true;
		}




		/// <summary>
		/// Indicates if using parameter names to specify parameters on
		/// callable statements are supported.
		/// </summary>
		/// <returns>     Always true.  An application can use parameter names
		/// to specify parameters on callable statements.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsNamedParameters() throws java.sql.SQLException
		public virtual bool supportsNamedParameters()
		{
			return true;
		}



		/// <summary>
		/// Indicates if columns can be defined as non-nullable.
		/// </summary>
		/// <returns>     Always true. Columns can be defined as non-nullable.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsNonNullableColumns() throws java.sql.SQLException
		public virtual bool supportsNonNullableColumns()
		{
			return true;
		}



		/// <summary>
		/// Indicates if cursors can remain open across commits.
		/// </summary>
		/// <returns>     Always true. Cursors can remain open across commits.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOpenCursorsAcrossCommit() throws java.sql.SQLException
		public virtual bool supportsOpenCursorsAcrossCommit()
		{
			return true;
		}



		/// <summary>
		/// Indicates if cursors can remain open across rollback.
		/// </summary>
		/// <returns>     Always true. Cursors can remain open across rollback.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOpenCursorsAcrossRollback() throws java.sql.SQLException
		public virtual bool supportsOpenCursorsAcrossRollback()
		{
			return true;
		}



		/// <summary>
		/// Indicates if statements can remain open across commits.
		/// </summary>
		/// <returns>     Always true. Statements can remain open across commits.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOpenStatementsAcrossCommit() throws java.sql.SQLException
		public virtual bool supportsOpenStatementsAcrossCommit()
		{
			return true;
		}



		/// <summary>
		/// Indicates if statements can remain open across rollback.
		/// </summary>
		/// <returns>     Always true. Statements can remain open across rollback.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOpenStatementsAcrossRollback() throws java.sql.SQLException
		public virtual bool supportsOpenStatementsAcrossRollback()
		{
			return true;
		}



		/// <summary>
		/// Indicates if an ORDER BY clause can use columns not in the SELECT.
		/// </summary>
		/// <returns>     Always false. ORDER BY cannot use columns not in the SELECT.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOrderByUnrelated() throws java.sql.SQLException
		public virtual bool supportsOrderByUnrelated()
		{
			return false;
		}



		/// <summary>
		/// Indicates if some form of outer join is supported.
		/// </summary>
		/// <returns>     Always true. Some form of outer join is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsOuterJoins() throws java.sql.SQLException
		public virtual bool supportsOuterJoins()
		{
			return true;
		}



		/// <summary>
		/// Indicates if positioned DELETE is supported.
		/// </summary>
		/// <returns>     Always true.  Positioned DELETE is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsPositionedDelete() throws java.sql.SQLException
		public virtual bool supportsPositionedDelete()
		{
			return true;
		}



		/// <summary>
		/// Indicates if positioned UPDATE is supported.
		/// </summary>
		/// <returns>     Always true. Positioned UPDATE is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsPositionedUpdate() throws java.sql.SQLException
		public virtual bool supportsPositionedUpdate()
		{
			return true;
		}



		// JDBC 2.0  // @C0C
		/// <summary>
		/// Indicates if the specified result set concurrency is supported
		/// for the specified result set type.
		/// 
		/// <para>This chart describes the combinations of result set concurrency
		/// and type that this driver supports:
		/// <br> <br>
		/// <table border=1 summary="">
		/// <tr><th><br></th><th>CONCUR_READ_ONLY</th><th>CONCUR_UPDATABLE</th></tr>
		/// <tr><td>TYPE_FORWARD_ONLY</td><td>Yes</td><td>Yes</td></tr>
		/// <tr><td>TYPE_SCROLL_INSENSITIVE</td><td>Yes</td><td>No</td></tr>
		/// <tr><td>TYPE_SCROLL_SENSITIVE</td><td>Yes</td><td>Yes</td></tr>
		/// </table>
		/// <br>
		/// 
		/// </para>
		/// </summary>
		/// <param name="resultSetType">            The result set type.  Valid values are:
		///                                <ul>
		///                                  <li>ResultSet.TYPE_FORWARD_ONLY
		///                                  <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                                  <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                                </ul> </param>
		/// <param name="resultSetConcurrency">     The result set concurrency.  Valid values are:
		///                                <ul>
		///                                  <li>ResultSet.CONCUR_READ_ONLY
		///                                  <li>ResultSet.CONCUR_UPDATABLE
		///                                </ul> </param>
		/// <returns>                         true if the specified result set
		///                                concurrency is supported for the specified
		///                                result set type; false otherwise.
		/// </returns>
		/// <exception cref="SQLException">        If the result set type or result set
		///                                concurrency is not valid.
		///  </exception>
		//
		// Implementation note:
		//
		// The unsupported combinations are dictated by the DB2
		// cursor support.
		//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsResultSetConcurrency(int resultSetType, int resultSetConcurrency) throws java.sql.SQLException
		public virtual bool supportsResultSetConcurrency(int resultSetType, int resultSetConcurrency)
		{
			// Validate the result set type.
			if (supportsResultSetType(resultSetType))
			{
				// jtopenlite only supports READONLY cursors
				if ((resultSetConcurrency == ResultSet.CONCUR_READ_ONLY))
				{
					return true;
				}
				if (resultSetConcurrency == ResultSet.CONCUR_UPDATABLE)
				{
					return false;
				}
				else
				{
					JDBCError.throwSQLException(JDBCError.EXC_CONCURRENCY_INVALID);
					return false;
				}
			}
			else
			{
				return false;
			}
		}




		/// <summary>
		/// Indicates if a type of result set holdability is supported.  The two
		/// types are ResultSet.HOLD_CURSORS_OVER_COMMIT and ResultSet.CLOSE_CURSORS_AT_COMMIT.
		/// </summary>
		/// <returns>     True if the user is connecting to a system running OS/400
		/// V5R2 or IBM i, otherwise false.  Both types of result set
		/// holidability are supported if connecting to OS/400 V5R2 or IBM i.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsResultSetHoldability(int resultSetHoldability) throws java.sql.SQLException
		public virtual bool supportsResultSetHoldability(int resultSetHoldability)
		{
				return true;
		}




		/// <summary>
		/// Indicates if savepoints are supported.
		/// </summary>
		/// <returns>     False.  The toolboxlite driver does not support savepoints. </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		/// @since Modification 5
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSavepoints() throws java.sql.SQLException
		public virtual bool supportsSavepoints()
		{
			// Note we check only the system level.  We don't need to
			// check JDBC/JDK level because if running prior to JDBC 3.0
			// the app cannot call this method (it does not exist
			// in the interface).
			return false;
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if the specified result set type is supported.
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Valid values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     true for ResultSet.TYPE_FORWARD_ONLY
		///                            ResultSet.TYPE_SCROLL_SENSITIVE. and
		///                            ResultSet.TYPE_SCROLL_INSENSITIVE.
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsResultSetType(int resultSetType) throws java.sql.SQLException
		public virtual bool supportsResultSetType(int resultSetType)
		{
			switch (resultSetType)
			{
				case ResultSet.TYPE_FORWARD_ONLY:
					return true;
				case ResultSet.TYPE_SCROLL_SENSITIVE:
				case ResultSet.TYPE_SCROLL_INSENSITIVE:
					return false;
				default:
					throw new SQLException("resultSetType invalid: " + resultSetType);
			}
		}



		/// <summary>
		/// Indicates if a schema name can be used in a data manipulation
		/// statement.
		/// </summary>
		/// <returns>     Always true. A schema name can be used in a data
		///            manipulation statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSchemasInDataManipulation() throws java.sql.SQLException
		public virtual bool supportsSchemasInDataManipulation()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a schema name can be used in an index definition
		/// statement.
		/// </summary>
		/// <returns>     Always true. A schema name can be used in an index definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSchemasInIndexDefinitions() throws java.sql.SQLException
		public virtual bool supportsSchemasInIndexDefinitions()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a schema name be can used in a privilege definition
		/// statement.
		/// </summary>
		/// <returns>     Always true. A schema name can be used in a privilege definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSchemasInPrivilegeDefinitions() throws java.sql.SQLException
		public virtual bool supportsSchemasInPrivilegeDefinitions()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a schema name be can used in a procedure call
		/// statement.
		/// </summary>
		/// <returns>     Always true. A schema name can be used in a procedure call
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSchemasInProcedureCalls() throws java.sql.SQLException
		public virtual bool supportsSchemasInProcedureCalls()
		{
			return true;
		}



		/// <summary>
		/// Indicates if a schema name can be used in a table definition
		/// statement.
		/// </summary>
		/// <returns>     Always true. A schema name can be used in a table definition
		///            statement.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSchemasInTableDefinitions() throws java.sql.SQLException
		public virtual bool supportsSchemasInTableDefinitions()
		{
			return true;
		}



		/// <summary>
		/// Indicates if SELECT for UPDATE is supported.
		/// </summary>
		/// <returns>     Always true. SELECT for UPDATE is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSelectForUpdate() throws java.sql.SQLException
		public virtual bool supportsSelectForUpdate()
		{
			return true;
		}




		/// <summary>
		/// Indicates if statement pooling is supported. </summary>
		/// <returns> Always false. Statement pooling is not supported at this time.
		///  </returns>
		public virtual bool supportsStatementPooling()
		{
			return false;
		}


		/// <summary>
		/// Indicates if stored procedure calls using the stored procedure
		/// escape syntax are supported.
		/// </summary>
		/// <returns>     Always true. Stored procedure calls using the stored
		///            procedure escape syntax are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsStoredProcedures() throws java.sql.SQLException
		public virtual bool supportsStoredProcedures()
		{
			return true;
		}




		/// <summary>
		/// Indicates if subqueries in comparisons are supported.
		/// </summary>
		/// <returns>     Always true. Subqueries in comparisons are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSubqueriesInComparisons() throws java.sql.SQLException
		public virtual bool supportsSubqueriesInComparisons()
		{
			return true;
		}



		/// <summary>
		/// Indicates if subqueries in EXISTS expressions are supported.
		/// </summary>
		/// <returns>     Always true. Subqueries in EXISTS expressions are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSubqueriesInExists() throws java.sql.SQLException
		public virtual bool supportsSubqueriesInExists()
		{
			return true;
		}



		/// <summary>
		/// Indicates if subqueries in IN expressions are supported.
		/// </summary>
		/// <returns>     Always true. Subqueries in IN expressions are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSubqueriesInIns() throws java.sql.SQLException
		public virtual bool supportsSubqueriesInIns()
		{
			return true;
		}



		/// <summary>
		/// Indicates if subqueries in quantified expressions are supported.
		/// </summary>
		/// <returns>     Always true. Subqueries in quantified expressions are
		///            supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsSubqueriesInQuantifieds() throws java.sql.SQLException
		public virtual bool supportsSubqueriesInQuantifieds()
		{
			return true;
		}



		/// <summary>
		/// Indicates if table correlation names are supported.
		/// </summary>
		/// <returns>     Always true. Table correlation names are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsTableCorrelationNames() throws java.sql.SQLException
		public virtual bool supportsTableCorrelationNames()
		{
			return true;
		}



		/// <summary>
		/// Indicates if the database supports the given transaction
		/// isolation level.
		/// </summary>
		/// <param name="transactionIsolationLevel">   One of the Connection.TRANSACTION_*
		///                                        values. </param>
		/// <returns>                                 Always true.  All transaction isolation
		///                                        levels are supported.
		/// </returns>
		/// <exception cref="SQLException">  If the transaction isolation level is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsTransactionIsolationLevel(int transactionIsolationLevel) throws java.sql.SQLException
		public virtual bool supportsTransactionIsolationLevel(int transactionIsolationLevel)
		{
			// Validate the transaction isolation level.
			if ((transactionIsolationLevel != Connection.TRANSACTION_NONE) && (transactionIsolationLevel != Connection.TRANSACTION_READ_UNCOMMITTED) && (transactionIsolationLevel != Connection.TRANSACTION_READ_COMMITTED) && (transactionIsolationLevel != Connection.TRANSACTION_REPEATABLE_READ) && (transactionIsolationLevel != Connection.TRANSACTION_SERIALIZABLE))
			{
				throw new SQLException("transactionIsolationLevel invalid : " + transactionIsolationLevel);
			}

			return true;
		}



		/// <summary>
		/// Indicates if transactions are supported.
		/// </summary>
		/// <returns>     Always true. Transactions are supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsTransactions() throws java.sql.SQLException
		public virtual bool supportsTransactions()
		{
			return true;
		}



		/// <summary>
		/// Indicates if SQL UNION is supported.
		/// </summary>
		/// <returns>     Always true. SQL UNION is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsUnion() throws java.sql.SQLException
		public virtual bool supportsUnion()
		{
			return true;
		}



		/// <summary>
		/// Indicates if SQL UNION ALL is supported.
		/// </summary>
		/// <returns>     Always true. SQL UNION ALL is supported.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsUnionAll() throws java.sql.SQLException
		public virtual bool supportsUnionAll()
		{
			return true;
		}



		/// <summary>
		/// Returns the name of the catalog.
		/// </summary>
		/// <returns>        The name of the catalog.
		///  </returns>
		public override string ToString()
		{
			try
			{
				return connection_.Catalog;
			}
			catch (SQLException)
			{
				return base.ToString();
			}
		}



		// JDBC 2.0
		/// <summary>
		/// Indicates if visible updates to a result set of the specified type
		/// can be detected by calling ResultSet.rowUpdated().
		/// </summary>
		/// <param name="resultSetType">        The result set type.  Value values are:
		///                            <ul>
		///                              <li>ResultSet.TYPE_FORWARD_ONLY
		///                              <li>ResultSet.TYPE_SCROLL_INSENSITIVE
		///                              <li>ResultSet.TYPE_SCROLL_SENSITIVE
		///                            </ul> </param>
		/// <returns>                     Always false.  Updates can not be detected
		///                            by calling ResultSet.rowUpdated().
		/// </returns>
		/// <exception cref="SQLException">    If the result set type is not valid.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean updatesAreDetected(int resultSetType) throws java.sql.SQLException
		public virtual bool updatesAreDetected(int resultSetType)
		{
			// Validate the result set type.
			supportsResultSetType(resultSetType);

			return false;
		}



		/// <summary>
		/// Indicates if the database uses a file for each table.
		/// </summary>
		/// <returns>     Always false. The database does not use a file for each
		///            table.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean usesLocalFilePerTable() throws java.sql.SQLException
		public virtual bool usesLocalFilePerTable()
		{
			return false;
		}



		/// <summary>
		/// Indicates if the database stores tables in a local file.
		/// </summary>
		/// <returns>     Always false. The database does not store tables in a local
		///            file.
		/// </returns>
		/// <exception cref="SQLException">    This exception is never thrown.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean usesLocalFiles() throws java.sql.SQLException
		public virtual bool usesLocalFiles()
		{
			return false;
		}



		/// <summary>
		/// Retrieves whether a <code>SQLException</code> thrown while autoCommit is <code>true</code> indicates
		/// that all open ResultSets are closed, even ones that are holdable.  When a <code>SQLException</code> occurs while
		/// autocommit is <code>true</code>, it is vendor specific whether the JDBC driver responds with a commit operation, a
		/// rollback operation, or by doing neither a commit nor a rollback.  A potential result of this difference
		/// is in whether or not holdable ResultSets are closed.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean autoCommitFailureClosesAllResultSets() throws java.sql.SQLException
		public virtual bool autoCommitFailureClosesAllResultSets()
		{
			return false; //toolbox returns false based on current behavoir
		}

		/// <summary>
		/// Retrieves a list of the client info properties
		/// that the driver supports.  The result set contains the following columns
		/// <para>
		/// <ol>
		/// <li><b>NAME</b> String=&gt; The name of the client info property<br>
		/// <li><b>MAX_LEN</b> int=&gt; The maximum length of the value for the property<br>
		/// <li><b>DEFAULT_VALUE</b> String=&gt; The default value of the property<br>
		/// <li><b>DESCRIPTION</b> String=&gt; A description of the property.  This will typically
		///                      contain information as to where this property is
		///                      stored in the database.
		/// </ol>
		/// </para>
		/// <para>
		/// The <code>ResultSet</code> is sorted by the NAME column in ascending order
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <returns>  A <code>ResultSet</code> object; each row is a supported client info
		/// property
		/// <para>
		/// </para>
		/// </returns>
		///  <exception cref="SQLException"> if a database access error occurs
		/// <para> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getClientInfoProperties() throws java.sql.SQLException
		public virtual ResultSet ClientInfoProperties
		{
			get
			{
				// Not supported in toolbox lite driver
				throw new NotImplementedException();
			}
		}



		/// <summary>
		/// Retrieves the schema names available in this database.  The results
		/// are ordered by schema name.
		/// 
		/// <P>The schema column is:
		///  <OL>
		///  <LI><B>TABLE_SCHEM</B> String =&gt; schema name
		///  <LI><B>TABLE_CATALOG</B> String =&gt; catalog name (may be <code>null</code>)
		///  </OL>
		/// 
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it is stored
		/// in the database;"" retrieves those without a catalog; null means catalog
		/// name should not be used to narrow down the search. </param>
		/// <param name="schemaPattern"> a schema name; must match the schema name as it is
		/// stored in the database; null means
		/// schema name should not be used to narrow down the search. </param>
		/// <returns> a <code>ResultSet</code> object in which each row is a
		///         schema decription </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getSchemas(String catalog, String schemaPattern) throws java.sql.SQLException
		public virtual ResultSet getSchemas(string catalog, string schemaPattern)
		{

			CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLTABLES  (?, ?, ?, ?, ?)");

			cstmt.setString(1, normalize(catalog));
			cstmt.setString(2, normalize(schemaPattern));
			cstmt.setString(3, "%");
			cstmt.setString(4, "%");
			cstmt.setObject(5, "DATATYPE='JDBC';GETSCHEMAS=2;CURSORHOLD=1");
			cstmt.execute();
			ResultSet rs = cstmt.ResultSet;
			if (rs != null)
			{
				((JDBCResultSet)rs).isMetadataResultSet_ = true;
			}
			else
			{
				cstmt.close();
				// It is an error not to have received a result set
				JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
			}
			return rs;
		}


		/// <summary>
		/// Retrieves whether this database supports invoking user-defined or vendor functions
		/// using the stored procedure escape syntax.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsStoredFunctionsUsingCallSyntax() throws java.sql.SQLException
		public virtual bool supportsStoredFunctionsUsingCallSyntax()
		{
			// toolbox does not support this
			return false;
		}


		/// <summary>
		/// Retrieves a description of the user functions available in the given
		/// catalog.
		/// <P>
		/// Only system and user function descriptions matching the schema and
		/// function name criteria are returned.  They are ordered by
		/// <code>FUNCTION_CAT</code>, <code>FUNCTION_SCHEM</code>,
		/// <code>FUNCTION_NAME</code> and
		/// <code>SPECIFIC_ NAME</code>.
		/// 
		/// <P>Each function description has the the following columns:
		///  <OL>
		///  <LI><B>FUNCTION_CAT</B> String =&gt; function catalog (may be <code>null</code>)
		///  <LI><B>FUNCTION_SCHEM</B> String =&gt; function schema (may be <code>null</code>)
		///  <LI><B>FUNCTION_NAME</B> String =&gt; function name.  This is the name
		/// used to invoke the function
		///  <LI><B>REMARKS</B> String =&gt; explanatory comment on the function
		/// <LI><B>FUNCTION_TYPE</B> short =&gt; kind of function:
		///      <UL>
		///      <LI>functionResultUnknown - Cannot determine if a return value
		///       or table will be returned
		///      <LI> functionNoTable- Does not return a table
		///      <LI> functionReturnsTable - Returns a table
		///      </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  =&gt; the name which uniquely identifies
		///  this function within its schema.  This is a user specified, or DBMS
		/// generated, name that may be different then the <code>FUNCTION_NAME</code>
		/// for example with overload functions
		///  </OL>
		/// <para>
		/// A user may not have permissions to execute any of the functions that are
		/// returned by <code>getFunctions</code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="functionNamePattern"> a function name pattern; must match the
		///        function name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a function description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getFunctions(String catalog, String schemaPattern, String functionNamePattern) throws java.sql.SQLException
		public virtual ResultSet getFunctions(string catalog, string schemaPattern, string functionNamePattern)
		{

			CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLFUNCTIONS  ( ?, ?, ?, ?)");

			cstmt.setString(1, normalize(catalog));
			cstmt.setString(2, normalize(schemaPattern));
			cstmt.setString(3, normalize(functionNamePattern));
			cstmt.setObject(4, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
			cstmt.execute();
			ResultSet rs = cstmt.ResultSet;
			if (rs != null)
			{
				((JDBCResultSet)rs).isMetadataResultSet_ = true;
			}
			else
			{
				cstmt.close();
				// It is an error not to have received a result set
				JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
			}
			return rs;
		}


		/// <summary>
		/// Retrieves a description of the given catalog's system or user
		/// function parameters and return type.
		/// 
		/// <P>Only descriptions matching the schema,  function and
		/// parameter name criteria are returned. They are ordered by
		/// <code>FUNCTION_CAT</code>, <code>FUNCTION_SCHEM</code>,
		/// <code>FUNCTION_NAME</code> and
		/// <code>SPECIFIC_ NAME</code>. Within this, the return value,
		/// if any, is first. Next are the parameter descriptions in call
		/// order. The column descriptions follow in column number order.
		/// 
		/// <P>Each row in the <code>ResultSet</code>
		/// is a parameter description, column description or
		/// return type description with the following fields:
		///  <OL>
		///  <LI><B>FUNCTION_CAT</B> String =&gt; function catalog (may be <code>null</code>)
		///  <LI><B>FUNCTION_SCHEM</B> String =&gt; function schema (may be <code>null</code>)
		///  <LI><B>FUNCTION_NAME</B> String =&gt; function name.  This is the name
		/// used to invoke the function
		///  <LI><B>COLUMN_NAME</B> String =&gt; column/parameter name
		///  <LI><B>COLUMN_TYPE</B> Short =&gt; kind of column/parameter:
		///      <UL>
		///      <LI> functionColumnUnknown - nobody knows
		///      <LI> functionColumnIn - IN parameter
		///      <LI> functionColumnInOut - INOUT parameter
		///      <LI> functionColumnOut - OUT parameter
		///      <LI> functionColumnReturn - function return value
		///      <LI> functionColumnResult - Indicates that the parameter or column
		///  is a column in the <code>ResultSet</code>
		///      </UL>
		///  <LI><B>DATA_TYPE</B> int =&gt; SQL type from java.sql.Types
		///  <LI><B>TYPE_NAME</B> String =&gt; SQL type name, for a UDT type the
		///  type name is fully qualified
		///  <LI><B>PRECISION</B> int =&gt; precision
		///  <LI><B>LENGTH</B> int =&gt; length in bytes of data
		///  <LI><B>SCALE</B> short =&gt; scale -  null is returned for data types where
		/// SCALE is not applicable.
		///  <LI><B>RADIX</B> short =&gt; radix
		///  <LI><B>NULLABLE</B> short =&gt; can it contain NULL.
		///      <UL>
		///      <LI> functionNoNulls - does not allow NULL values
		///      <LI> functionNullable - allows NULL values
		///      <LI> functionNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>REMARKS</B> String =&gt; comment describing column/parameter
		///  <LI><B>CHAR_OCTET_LENGTH</B> int  =&gt; the maximum length of binary
		/// and character based parameters or columns.  For any other datatype the returned value
		/// is a NULL
		///  <LI><B>ORDINAL_POSITION</B> int  =&gt; the ordinal position, starting
		/// from 1, for the input and output parameters. A value of 0
		/// is returned if this row describes the function's return value.
		/// For result set columns, it is the
		/// ordinal position of the column in the result set starting from 1.
		///  <LI><B>IS_NULLABLE</B> String  =&gt; ISO rules are used to determine
		/// the nullability for a parameter or column.
		///       <UL>
		///       <LI> YES           --- if the parameter or column can include NULLs
		///       <LI> NO            --- if the parameter or column  cannot include NULLs
		///       <LI> empty string  --- if the nullability for the
		/// parameter  or column is unknown
		///       </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  =&gt; the name which uniquely identifies
		/// this function within its schema.  This is a user specified, or DBMS
		/// generated, name that may be different then the <code>FUNCTION_NAME</code>
		/// for example with overload functions
		///  </OL>
		/// 
		/// <para>The PRECISION column represents the specified column size for the given
		/// parameter or column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="functionNamePattern"> a procedure name pattern; must match the
		///        function name as it is stored in the database </param>
		/// <param name="columnNamePattern"> a parameter name pattern; must match the
		/// parameter or column name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row describes a
		/// user function parameter, column  or return type
		/// </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getFunctionColumns(String catalog, String schemaPattern, String functionNamePattern, String columnNamePattern) throws java.sql.SQLException
		public virtual ResultSet getFunctionColumns(string catalog, string schemaPattern, string functionNamePattern, string columnNamePattern)
		{

			CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLFUNCTIONCOLS  ( ?, ?, ?, ?, ?)");

			cstmt.setString(1, normalize(catalog));
			cstmt.setString(2, normalize(schemaPattern));
			cstmt.setString(3, normalize(functionNamePattern));
			cstmt.setString(4, normalize(columnNamePattern));
			if (javaVersion > 16)
			{
			cstmt.setObject(5, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
			}
			else
			{
				cstmt.setObject(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
			}
			cstmt.execute();
			ResultSet rs = cstmt.ResultSet;
			if (rs != null)
			{
				((JDBCResultSet)rs).isMetadataResultSet_ = true;
			}
			else
			{
				cstmt.close();
				// It is an error not to have received a result set
				JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
			}
			return rs;
		}


		// JDBC 4.1
		/// <summary>
		/// Retrieves whether a generated key will always be returned if the column name(s) or index(es)
		/// specified for the auto generated key column(s) are valid and the statement succeeds. </summary>
		/// <returns> true if so; false otherwise </returns>
		/// <exception cref="SQLException"> - if a database access error occurs </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean generatedKeyAlwaysReturned() throws java.sql.SQLException
		public virtual bool generatedKeyAlwaysReturned()
		{
		  return false;
		}



		// JDBC 4.1
		/// <summary>
		/// Retrieves a description of the pseudo or hidden columns available in a given table within the specified
		/// catalog and schema. Pseudo or hidden columns may not always be stored within a table and are not
		/// visible in a ResultSet unless they are specified in the query's outermost SELECT list. Pseudo or hidden
		/// columns may not necessarily be able to be modified. If there are no pseudo or hidden columns, an empty
		/// ResultSet is returned.
		/// <para>Only column descriptions matching the catalog, schema, table and column name criteria are returned.
		/// They are ordered by TABLE_CAT,TABLE_SCHEM, TABLE_NAME and COLUMN_NAME.
		/// </para>
		/// <para>Each column description has the following columns:
		/// <ol>
		/// <li>TABLE_CAT String =&gt; table catalog (may be null)
		/// <li>TABLE_SCHEM String =&gt; table schema (may be null)
		/// <li>TABLE_NAME String =&gt; table name
		/// <li>COLUMN_NAME String =&gt; column name
		/// <li>DATA_TYPE int =&gt; SQL type from java.sql.Types
		/// <li>COLUMN_SIZE int =&gt; column size.
		/// <li>DECIMAL_DIGITS int =&gt; the number of fractional digits. Null is returned for data types where DECIMAL_DIGITS is not applicable.
		/// <li>NUM_PREC_RADIX int =&gt; Radix (typically either 10 or 2)
		/// <li>COLUMN_USAGE String =&gt; The allowed usage for the column. The value returned will correspond to the enum name returned by PseudoColumnUsage.name()
		/// <li>REMARKS String =&gt; comment describing column (may be null)
		/// <li>CHAR_OCTET_LENGTH int =&gt; for char types the maximum number of bytes in the column
		/// <li>IS_NULLABLE String =&gt; ISO rules are used to determine the nullability for a column.
		/// <ul>
		/// <li>YES --- if the column can include NULLs
		/// <li>NO --- if the column cannot include NULLs
		/// <li>empty string --- if the nullability for the column is unknown
		/// </ul>
		/// </ol>
		/// </para>
		/// <para> The COLUMN_SIZE column specifies the column size for the given column. For numeric data, this is the
		/// maximum precision. For character data, this is the length in characters. For datetime datatypes,
		/// this is the length in characters of the String representation (assuming the maximum allowed precision of the
		/// fractional seconds component). For binary data, this is the length in bytes. For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the column size is not applicable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> - a catalog name; must match the catalog name as it is stored in the database;
		/// "" retrieves those without a catalog; null means that the catalog name should not be used to narrow the search </param>
		/// <param name="schemaPattern"> - a schema name pattern; must match the schema name as it is stored in the database;
		/// "" retrieves those without a schema; null means that the schema name should not be used to narrow the search </param>
		/// <param name="tableNamePattern"> - a table name pattern; must match the table name as it is stored in the database </param>
		/// <param name="columnNamePattern"> - a column name pattern; must match the column name as it is stored in the database </param>
		/// <returns>  ResultSet - each row is a column description </returns>
		/// <exception cref="SQLException"> - if a database access error occurs </exception>
		/// <seealso cref= java.sql.PseudoColumnUsage </seealso>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.ResultSet getPseudoColumns(String catalog, String schemaPattern, String tableNamePattern, String columnNamePattern) throws java.sql.SQLException
		public virtual ResultSet getPseudoColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{


		  CallableStatement cstmt = connection_.prepareCall("call SYSIBM" + CatalogSeparator + "SQLPSEUDOCOLUMNS  ( ?, ?, ?, ?, ?)");

		  cstmt.setString(1, normalize(catalog));
		  cstmt.setString(2, normalize(schemaPattern));
		  cstmt.setString(3, normalize(tableNamePattern));
		  cstmt.setString(4, normalize(columnNamePattern));
		  if (javaVersion >= 16)
		  {
			 cstmt.setObject(5, "DATATYPE='JDBC';JDBCVER='4.0';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
		  }
		  else
		  {
			  cstmt.setObject(5, "DATATYPE='JDBC';DYNAMIC=0;REPORTPUBLICPRIVILEGES=1;CURSORHOLD=1");
		  }
		  cstmt.execute();
		  ResultSet rs = cstmt.ResultSet;
		  if (rs != null)
		  {
			  ((JDBCResultSet)rs).isMetadataResultSet_ = true;
		  }
		  else
		  {
			  cstmt.close();
			  // It is an error not to have received a result set
			  JDBCError.throwSQLException(JDBCError.EXC_INTERNAL);
		  }
		  return rs;
		}








	}

}