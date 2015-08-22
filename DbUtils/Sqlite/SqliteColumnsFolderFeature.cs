using System;
using GtkTestProject.Sqlite;

namespace Gtk3TestProject
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

