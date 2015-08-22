using System;
using GtkTestProject.Api;
using GtkTestProject.Sqlite;
using System.IO;
using Mono.Data.Sqlite;
using Gtk3TestProject;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GtkTestProject
{
	public class SqliteDbServerConnection : IDbServerConnection
	{

		private string sqliteConnectionString;
		private string sqliteFileName;
		private SqliteConnection con;


		public SqliteDbServerConnection (string sqliteDatabaseFileName, String connectionString)
		{
			sqliteConnectionString = connectionString;
			sqliteFileName = sqliteDatabaseFileName;

			con = new SqliteConnection (sqliteConnectionString);
			con.Open ();
		}

		#region IDbServerConnection implementation

		public IFeature[] GetFeatures (IFeature parentFeature)
		{
			if (parentFeature == null) {
				// root
				FileInfo dbFile = new FileInfo(sqliteFileName);
				return new SqliteFeature[] {
					new SqliteFeature ("filename", dbFile.Name, String.Format ("Resources{0}Icons{0}database.png", Path.DirectorySeparatorChar))
				};
			} else if (((SqliteFeature)parentFeature).Key == "tables") {
				return loadTables ();

			} else if (((SqliteFeature)parentFeature).Key == "table") {
				return new IFeature[] { 
					new SqliteColumnsFolderFeature (((SqliteTableFeature)parentFeature).Text)
				};
			} else if (((SqliteFeature)parentFeature).Key == "columns") {
				return loadColumns (((SqliteColumnsFolderFeature)parentFeature).TableName);
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

		#endregion

		private SqliteTableFeature[] loadTables() {

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

		private SqliteColumnFeature[] loadColumns(String table) {
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
	}
}

