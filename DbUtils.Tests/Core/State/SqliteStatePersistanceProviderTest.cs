using System;
using NUnit.Framework;
using DbUtils.Core.State;
using DbUtils.Sqlite;
using DbUtils.Core.Api;
using DbUtils.Sqlite.Api;
using Mono.Data.Sqlite;

namespace DbUtils.Tests.Core.State
{
	[TestFixture()]
	public class SqliteStatePersistanceProviderTest
	{
		public SqliteStatePersistanceProviderTest ()
		{
		}

		[Test()]
		public void TestSqliteSessionStateProvider() {
			SqliteConnectionStringBuilder conStrBuilder = new SqliteConnectionStringBuilder ();
			SqliteDbServerConnection serverCon = new SqliteDbServerConnection ("test", conStrBuilder.ConnectionString);
			ISessionStateProvider sessionStateProvider = new SqliteSessionStateProvider(serverCon);

			String state = sessionStateProvider.getSerializedState (); 

			IDbServerConnection restoredCon = sessionStateProvider.restoreSessionFromState (state);
			Assert.AreEqual (restoredCon, serverCon);
		}

		[Test()]
		public void TestSqliteStatePersistanceProvider() {
			IDbServerConnection session = new SqliteDbServerConnection ("testdb", "someconnectionstring");
			SqliteStatePersistanceProvider prov = new SqliteStatePersistanceProvider ();

			SqliteConnectionStringBuilder conStrBuilder = new SqliteConnectionStringBuilder ();
			conStrBuilder.DataSource = ":memory:";



			using (SqliteConnection con = new SqliteConnection (conStrBuilder.ConnectionString)) {
				con.Open ();

				// Start calling RestoreSessions method even though no session is saved. This triggers the initialization logic
				// in the SqliteStatePersistanceProvider.
				var providersShouldBeEmpty = prov.RestoreSessions (con);
				Assert.AreEqual (0, providersShouldBeEmpty.Length);

				// Save session
				prov.SaveSessions (new IDbServerConnection[] { session }, con);

				// restore saved session
				IDbServerConnection[] restoredSessions = prov.RestoreSessions (con);

				Assert.AreEqual (1, restoredSessions.Length);
				Assert.AreEqual (session, restoredSessions [0]);
			}


		}
	}
}

