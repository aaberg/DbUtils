using System;
using GtkTestProject.Api;
using DbUtils;

namespace DbUtils
{
	public class DbServerConnectionEventArgs : EventArgs
	{
		public DbServerConnectionEventArgs (IDbServerConnection dbServerConnection)
		{
			this.DbServerConnection = dbServerConnection;
		}

		public IDbServerConnection DbServerConnection {
			get;
			private set;
		}
	}



	public delegate void DbServerConnectionEventHandler(object sender, DbServerConnectionEventArgs e);
}

