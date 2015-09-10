using System;
using DbUtils.Core.State;
using DbUtils.Sqlite.Api;

namespace DbUtils.Sqlite
{
	public class SqliteSessionStateProvider : ISessionStateProvider
	{
		private SqliteDbServerConnection session;
		
		public SqliteSessionStateProvider (SqliteDbServerConnection session)
		{
			this.session = session;
		}

		public SqliteSessionStateProvider() {
			
		}

		#region ISessionStateProvider implementation

		public string getSerializedState ()
		{
			return session.Name + "$" + session.SqliteConnectionString;
		}

		public DbUtils.Core.Api.IDbServerConnection restoreSessionFromState (string state)
		{
			var strs = state.Split ('$');
			var name = strs [0];
			var constr = strs [1];
			return new SqliteDbServerConnection (name, constr);
		}

		#endregion
	}
}

