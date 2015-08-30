using System;
using Gtk;
using DbUtils.Api;
using Mono.Data.Sqlite;

namespace DbUtils.Sqlite.Api
{
	public class SqliteConnectionLoader : IConnectionLoader
	{
		private bool openNew;
		public SqliteConnectionLoader (bool openNew)
		{
			this.openNew = openNew;
		}

		#region IConnectionLoader implementation

		public DbUtils.Api.IDbServerConnection getConnection (Window parentWindow)
		{
			
			FileChooserDialog chooseFileDialog = new FileChooserDialog (
				                                     openNew ? "New SQLite database" : "Open existing SQLite database", 
				                                     parentWindow, 
				                                     openNew ? FileChooserAction.Save : FileChooserAction.Open,
				                                     "Cancel", ResponseType.Cancel,
				                                     openNew ? "Create" : "Open", ResponseType.Accept
			                                     );
			try {
				if (chooseFileDialog.Run () != (int)ResponseType.Accept)
					return null;

				SqliteConnectionStringBuilder conStrBuilder = new SqliteConnectionStringBuilder ();
				conStrBuilder.DataSource = chooseFileDialog.Filename;
				conStrBuilder.FailIfMissing = !openNew; 

				return new SqliteDbServerConnection (chooseFileDialog.Filename, conStrBuilder.ConnectionString);
			} finally {
				chooseFileDialog.Destroy ();
			}


		}

		#endregion
	}
}

