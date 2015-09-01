using System;

namespace DbUtils.Core.Api
{
	public class ColumnInfo
	{
		public ColumnInfo (string columnName, string datatype, Boolean notnull, int size)
		{
			this.ColumnName = columnName;
			this.DataType = datatype;
			this.NotNull = notnull;
			this.Size = size;
		}

		public string ColumnName {
			get;
			private set;
		}

		public string DataType {
			get;
			private set;
		}

		public bool NotNull {
			get;
			private set;
		}

		public int Size {
			get;
			private set;
		}
	}
}

