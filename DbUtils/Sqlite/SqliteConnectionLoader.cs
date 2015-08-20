using System;
using Gtk;

namespace GtkTestProject
{
	public class SqliteConnectionLoader : IConnectionLoader
	{
		public SqliteConnectionLoader ()
		{
		}

		#region IConnectionLoader implementation

		public GtkTestProject.Api.IDbServerConnection getConnection (Window parentWindow)
		{
			FileChooserDialog chooseSqliteDbChooserDialog = null;
			try {
				chooseSqliteDbChooserDialog = new FileChooserDialog (
					"Open Sqlite database", 
					parentWindow, 
					FileChooserAction.Open, 
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept
				);
				if (chooseSqliteDbChooserDialog.Run () == (int)ResponseType.Accept) {
					return new SqliteDbServerConnection (chooseSqliteDbChooserDialog.Filename, String.Format ("Data Source={0}; user=sa", "dbfile.db"));
				} else {
					return null; 
				}
			} finally {
				if (chooseSqliteDbChooserDialog != null)
					chooseSqliteDbChooserDialog.Destroy ();
			}

		}

		#endregion
	}
}

