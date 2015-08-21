using System;
using GtkTestProject.Sqlite;
using GtkTestProject.Api;
using System.IO;

namespace Gtk3TestProject
{
	public class SqliteTableFeature : SqliteFeature
	{
		public SqliteTableFeature (string key, string tablename) : 
		base(key, tablename, string.Format("Resources{0}Icons{0}table.png", Path.DirectorySeparatorChar))
		{
			
		}

		public override string Key {
			get {
				return base.Key;
			}
		}

	}
}

