using System;
using System.IO;
using Mono.Data.Sqlite;
using System.Linq;
using DbUtils.Core.State;
using DbUtils.Core.Api;

namespace DbUtils.Core.State
{
	public class SqliteStatePersistanceProvider : IStatePersistanceProvider
	{
		public SqliteStatePersistanceProvider ()
		{
		}

		#region IStatePersistanceProvider implementation

		public void SaveSessions (DbUtils.Core.Api.IDbServerConnection[] sessions)
		{
			SqliteConnectionStringBuilder sbuilder = new SqliteConnectionStringBuilder ();
			sbuilder.DataSource = StateDbDataSource;

			using (SqliteConnection con = new SqliteConnection (sbuilder.ConnectionString)) {
				con.Open ();
				SaveSessions (sessions, con);
			}
		}

		public void SaveSessions(IDbServerConnection[] sessions, SqliteConnection connection) {
			

			using (SqliteCommand cmd = connection.CreateCommand ()) {
				cmd.CommandText = "delete from dbstate";
				cmd.ExecuteNonQuery ();
			}

			using (SqliteCommand cmd = connection.CreateCommand()) {

				cmd.CommandText = "insert into dbstate(provider, state) values (:provider, :state)";

				sessions.ToList ().ForEach ((s) => {
					cmd.Parameters.Add("provider", System.Data.DbType.String).Value = s.GetType().FullName;
					cmd.Parameters.Add("state", System.Data.DbType.String).Value = s.SessionStateProvider.getSerializedState();
					cmd.ExecuteNonQuery();
				});
			}
		}

		public DbUtils.Core.Api.IDbServerConnection[] RestoreSessions ()
		{
			InitializeStateDb ();
			return new IDbServerConnection[]{ };
		}

		#endregion

		public string StateDbDataSource {
			get {
				var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				dataPath += string.Format ("{0}dbutils{0}", Path.DirectorySeparatorChar);
				if (!Directory.Exists (dataPath)) {
					Directory.CreateDirectory (dataPath);
				}
				String dbFileName = string.Format ("{0}{1}{2}", dataPath, Path.DirectorySeparatorChar, dbname);

				return dbFileName;
			}
		}

		private const int dbversion = 1;
		private const string dbname = "DbUtilsState.db";
		private void InitializeStateDb() {
			SqliteConnectionStringBuilder constrBuilder = new SqliteConnectionStringBuilder ();
			constrBuilder.DataSource = StateDbDataSource;
			constrBuilder.FailIfMissing = false;

			int ver;
			using (SqliteConnection con = new SqliteConnection (constrBuilder.ConnectionString)) {
				con.Open ();
				ver = GetDbVersion (con);
			}

			if (ver == dbversion) {
				return;
			} else if (ver == 0) {
				setupNewDb (constrBuilder);
			} else {
				// todo should upgrade. For now, just delete the old file and create a new one.
				File.Delete( StateDbDataSource );
				setupNewDb (constrBuilder);
			}

		}

		private int GetDbVersion(SqliteConnection con) {
			using (SqliteCommand cmd = con.CreateCommand ()) {
				cmd.CommandText = "select * from sqlite_master where name = :tname";
				cmd.Parameters.Add ("tname", System.Data.DbType.String).Value = "dbversion";

				using (SqliteDataReader reader = cmd.ExecuteReader()) {
					//todo this is not right, not reading dbversion table!!
					if (!reader.Read ()) {
						return 0;
					} 
				}
			}

			using (SqliteCommand cmd = con.CreateCommand ()) {
				cmd.CommandText = "select version from dbversion";
				object verObj = cmd.ExecuteScalar ();
				return Convert.ToInt32 (verObj);
			}
		}

		private void setupNewDb(SqliteConnectionStringBuilder conStrBuilder) {
			using (SqliteConnection con = new SqliteConnection (conStrBuilder.ConnectionString)) {
				con.Open ();
				using (SqliteCommand cmd = con.CreateCommand ()) {
					cmd.CommandText = "create table dbversion(version integer)";
					cmd.ExecuteNonQuery ();
				}

				using (SqliteCommand cmd = con.CreateCommand ()) {
					cmd.CommandText = "insert into dbversion(version) values (:ver)";
					cmd.Parameters.Add ("ver", System.Data.DbType.Int32).Value = dbversion;
					cmd.ExecuteNonQuery ();
				}

				using (SqliteCommand cmd = con.CreateCommand ()) {
					cmd.CommandText = "create table dbstate(id integer primary key, provider varchar, state varchar)";
					cmd.ExecuteNonQuery ();
				}
			}
		}
	}
}

