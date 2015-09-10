using System;
using DbUtils.Core.Api;
using DbUtils.Sqlite;
using System.IO;
using Mono.Data.Sqlite;
using DbUtils;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DbUtils.Core.State;

namespace DbUtils.Sqlite.Api
{
	public class SqliteDbServerConnection : IDbServerConnection
	{
		public SqliteDbServerConnection (string sqliteDatabaseFileName, String connectionString)
		{
			this.SqliteConnectionString = connectionString;
			this.Name = new FileInfo (sqliteDatabaseFileName).Name;
		}

		#region IDbServerConnection implementation

		public string Name {
			get;
			private set;
		}

		public string SqliteConnectionString  {
			get;
			private set;
		}

		public IFeature[] GetFeatures (IFeature parentFeature)
		{
			using (SqliteConnection con = new SqliteConnection (SqliteConnectionString)) {
				con.Open ();
			
				if (parentFeature == null) {
					// root
					return new SqliteFeature[] {
						new SqliteFeature ("filename", this.Name, String.Format ("Resources{0}Icons{0}database.png", Path.DirectorySeparatorChar))
					};
				} else if (((SqliteFeature)parentFeature).Key == "tables") {
					return loadTables (con);

				} else if (((SqliteFeature)parentFeature).Key == "table") {
					return new IFeature[] { 
						new SqliteColumnsFolderFeature (((SqliteTableFeature)parentFeature).Text)
					};
				} else if (((SqliteFeature)parentFeature).Key == "columns") {
					return loadColumns (((SqliteColumnsFolderFeature)parentFeature).TableName, con);
				} else if (((SqliteFeature)parentFeature).Key == "filename") {
					return new SqliteFeature[] {
						new SqliteFeature ("tables", "Tables"),
						new SqliteFeature ("views", "Views"),
						new SqliteFeature ("system", "System Catalogue")
					};
				} else {
					return new SqliteFeature[0];
				}
			}
		}

		public IDbConnection CreateConnection() {
			return new SqliteConnection (SqliteConnectionString);
		}

		public List<ColumnInfo> GetColumnInfosFromMeta(DataTable meta) {
			return meta.Select ().Select ((row) => new ColumnInfo (
				(string)row ["ColumnName"], 
				(string)row ["DataTypeName"],
				!(bool)row ["AllowDbNull"],
				(int)row ["ColumnSize"])
			).ToList ();

		}

		private ISessionStateProvider _sessionStateProvider = null;
		public DbUtils.Core.State.ISessionStateProvider SessionStateProvider {
			get {
				if (_sessionStateProvider == null) {
					_sessionStateProvider = new SqliteSessionStateProvider (this);
				}
				return _sessionStateProvider;
			}
		}

		#endregion

		private SqliteTableFeature[] loadTables(SqliteConnection con) {

			List<string> tables = new List<string> ();

			using (IDbCommand command = con.CreateCommand ()) {
				command.CommandText = "select  name from sqlite_master where type = 'table'";

				using (IDataReader reader = command.ExecuteReader ()) {
					while (reader.Read ()) {
						tables.Add( reader.GetString (0) );
					}
				}
			}
				
			var res = tables.Select (t => new SqliteTableFeature ("table", t));
			return res.ToArray ();
		}

		private SqliteColumnFeature[] loadColumns(String table, SqliteConnection con) {
			List<SqliteColumnFeature> columns = new List<SqliteColumnFeature> ();

			using (IDbCommand command = con.CreateCommand ()) {
				command.CommandText = "PRAGMA table_info(" + table + ")";
				using (IDataReader reader = command.ExecuteReader ()) {
					while (reader.Read ()) {
						string name = reader.GetString (reader.GetOrdinal ("name"));
						string type = reader.GetString (reader.GetOrdinal ("type"));
						bool notNull = reader.GetBoolean (reader.GetOrdinal ("notnull"));
						bool primaryKey = reader.GetBoolean (reader.GetOrdinal ("pk"));

						String text = string.Format ("{0} ({1} {2}, {3})",
							              name, primaryKey ? "PK, " : "", type, notNull ? "not null" : "");
						columns.Add(new SqliteColumnFeature("column", text, primaryKey));
					}
				}
			}
			return columns.ToArray ();
		}

		public override bool Equals (object obj)
		{
			if (obj.GetType () != this.GetType ())
				return false;

			SqliteDbServerConnection other = (SqliteDbServerConnection)obj;

			return 
				this.Name == other.Name && 
				this.SqliteConnectionString == other.SqliteConnectionString;
		}
	}
}

