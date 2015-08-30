using System;
using DbUtils.Sqlite;

namespace DbUtils.Sqlite.Api
{
	public class SqliteColumnsFolderFeature : SqliteFeature
	{
		public SqliteColumnsFolderFeature (string tableName) : 
		base("columns", "Columns")
		{
			this.TableName = tableName;
		}

		public String TableName {
			get;
			private set;
		}
	}
}

