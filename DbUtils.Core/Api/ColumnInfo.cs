using System;

namespace DbUtils
{
	public class ColumnInfo
	{
		public ColumnInfo (string columnName)
		{
			this.ColumnName = columnName;
		}

		public string ColumnName {
			get;
			private set;
		}
	}
}

