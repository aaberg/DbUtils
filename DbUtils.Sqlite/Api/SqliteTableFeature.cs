using System;
using DbUtils.Sqlite;
using DbUtils.Api;
using System.IO;

namespace DbUtils.Sqlite.Api
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

