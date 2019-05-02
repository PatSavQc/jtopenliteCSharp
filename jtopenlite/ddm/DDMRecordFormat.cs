///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  DDMRecordFormat.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite.ddm
{

	/// <summary>
	/// Represents the record format and field information for a file.
	/// 
	/// </summary>
	public class DDMRecordFormat
	{
	  private readonly string library_;
	  private readonly string file_;
	  private readonly string name_;
	  private readonly string type_;
	  private readonly string text_;
	  private readonly DDMField[] fields_;
	  private readonly int totalLength_;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: DDMRecordFormat(final String library, final String file, final String name, final String type, final String text, final DDMField[] fields, final int totalLength)
	  internal DDMRecordFormat(string library, string file, string name, string type, string text, DDMField[] fields, int totalLength)
	  {
		library_ = library;
		file_ = file;
		name_ = name;
		type_ = type;
		text_ = text;
		fields_ = fields;
		totalLength_ = totalLength;
	  }

	  /// <summary>
	  /// Returns a new copy of this record format, which includes a new copy of each DDMField.
	  /// This is useful if multiple threads need to do field conversions on the same record format definition,
	  /// since the DDMRecordFormat and DDMField classes are not thread-safe, each thread can be given its
	  /// own copy of the record format, rather than using synchronization to share a single record format.
	  /// 
	  /// </summary>
	  public virtual DDMRecordFormat newCopy()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DDMField[] fields = new DDMField[this.fields_.length];
		DDMField[] fields = new DDMField[this.fields_.Length];
		for (int i = 0; i < this.fields_.Length; ++i)
		{
		  fields[i] = this.fields_[i].newCopy();
		}
		return new DDMRecordFormat(this.library_, this.file_, this.name_, this.type_, this.text_, fields, this.totalLength_);
	  }

	  /// <summary>
	  /// Returns the name (WHNAME) of this record format.
	  /// 
	  /// </summary>
	  public virtual string Name
	  {
		  get
		  {
			return name_;
		  }
	  }

	  /// <summary>
	  /// Returns the library (WHLIB) in which the file resides.
	  /// 
	  /// </summary>
	  public virtual string Library
	  {
		  get
		  {
			return library_;
		  }
	  }

	  /// <summary>
	  /// Returns the name of the file (WHFILE) for this record format.
	  /// 
	  /// </summary>
	  public virtual string File
	  {
		  get
		  {
			return file_;
		  }
	  }

	  /// <summary>
	  /// Returns the file type (WHFTYP) of record format.
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
	  /// Returns the text description (WHTEXT) of this record format.
	  /// 
	  /// </summary>
	  public virtual string Text
	  {
		  get
		  {
			return text_;
		  }
	  }

	  /// <summary>
	  /// Returns the total length in bytes of this record format.
	  /// 
	  /// </summary>
	  public virtual int Length
	  {
		  get
		  {
			return totalLength_;
		  }
	  }

	  /// <summary>
	  /// Returns the recommended batch size to use for reading or writing records with this record format.
	  /// 
	  /// </summary>
	  public virtual int RecommendedBatchSize
	  {
		  get
		  {
			int div = totalLength_ + 16;
			int num = (32768 / div) - 1; //TODO - Something's wrong with out DDM data stream if we use an exact number, subtract 1 for now.
			if (num <= 0)
			{
				return 1;
			}
			return num;
		  }
	  }

	  /// <summary>
	  /// Returns the number of fields in this record format.
	  /// 
	  /// </summary>
	  public virtual int FieldCount
	  {
		  get
		  {
			return fields_.Length;
		  }
	  }

	  /// <summary>
	  /// Returns the field at the specified index, or null if the index is not valid.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DDMField getField(final int index)
	  public virtual DDMField getField(int index)
	  {
		return (index >= 0 && index < fields_.Length) ? fields_[index] : null;
	  }

	  /// <summary>
	  /// Returns the field with the specified name, or null if no such field exists in this record format.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DDMField getField(final String fieldName)
	  public virtual DDMField getField(string fieldName)
	  {
		return getField(getIndex(fieldName));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private int getIndex(final String fieldName)
	  private int getIndex(string fieldName)
	  {
		for (int i = 0; i < fields_.Length; ++i)
		{
		  if (fields_[i].Name.Equals(fieldName))
		  {
			  return i;
		  }
		}
		return -1;
	  }
	}

}