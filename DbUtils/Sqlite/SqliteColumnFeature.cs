using System;
using GtkTestProject.Sqlite;
using System.IO;

namespace Gtk3TestProject
{
	public class SqliteColumnFeature : SqliteFeature
	{
		public SqliteColumnFeature (string key, string columnName, bool primaryKey) :
		base(key, columnName, string.Format("Resources{0}Icons{0}{1}.png", Path.DirectorySeparatorChar, primaryKey ? "key" : "column"))
		{
		}
	}
}

