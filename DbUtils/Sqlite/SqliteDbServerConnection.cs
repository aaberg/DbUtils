using System;
using GtkTestProject.Api;
using GtkTestProject.Sqlite;
using System.IO;

namespace GtkTestProject
{
	public class SqliteDbServerConnection : IDbServerConnection
	{

		private string sqliteConnectionString;
		private string sqliteFileName;

		public SqliteDbServerConnection (string sqliteDatabaseFileName, String connectionString)
		{
//			sqliteConnectionString = string.Format ("Data Source={0};user=sa", sqliteDatabaseFileName);
			sqliteConnectionString = connectionString;
			sqliteFileName = sqliteDatabaseFileName;
		}

		#region IDbServerConnection implementation

		public IFeature[] GetFeatures (IFeature parentFeature)
		{
			if (parentFeature == null) {
				// root
				return new SqliteFeature[] {
					new SqliteFeature ("filename", sqliteFileName)
				};
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
	}
}

