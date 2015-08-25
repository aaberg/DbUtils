using System;
using Gtk;

namespace DbUtils.UI
{
	public class MainMenu : Gtk.MenuBar
	{
		public MainMenu ()
		{
			Build ();
		}

		protected Gtk.MenuItem file;
		protected Gtk.Menu fileMenu;
		protected Gtk.MenuItem openMenuItem;
		protected Gtk.MenuItem newSqlEditorMenuItem;
		protected Gtk.MenuItem exitMenuItem;
		protected Gtk.MenuItem edit;
		protected Gtk.Menu editMenu;
		protected Gtk.MenuItem database;
		protected Gtk.Menu databaseMenu;
		protected Gtk.MenuItem newSqliteDbMenuItem;
		protected Gtk.MenuItem openSqliteDbMenuItem;
		protected Gtk.Notebook tabbedArea;

		public event EventHandler NewSqliteDbActivated;
		public event EventHandler OpenSqliteDbActivated;
		public event EventHandler ExitActivated;
		public event EventHandler NewSqlEditorWindowActivated;

		protected virtual void OnNewSqliteDbActivated(object sender, EventArgs e) {
			if (NewSqliteDbActivated != null) {
				NewSqliteDbActivated (this, e);
			}
		}

		protected virtual void OnOpenSqliteDbActivated(object sender, EventArgs e) {
			if (OpenSqliteDbActivated != null) {
				OpenSqliteDbActivated (this, e);
			}
		}

		protected virtual void OnExitActivated(object sender,EventArgs e) {
			if (ExitActivated != null) {
				ExitActivated (this, e);
			}
		}

		protected virtual void OnNewSqlEditorWindowActivated(object sender, EventArgs e) {
			if (NewSqlEditorWindowActivated != null) {
				NewSqlEditorWindowActivated (sender, e);
			}
		}

		public void Build() {

			// file menu
			file = new MenuItem("File");
			fileMenu = new Menu();
			file.Submenu = fileMenu;

			openMenuItem = new MenuItem ("Open Sqlite database");
			openMenuItem.Activated += OnNewSqliteDbActivated;
			fileMenu.Append (openMenuItem);

			newSqlEditorMenuItem = new MenuItem ("New Sql Editor Tab");
			newSqlEditorMenuItem.Sensitive = false;
			newSqlEditorMenuItem.Activated += OnNewSqlEditorWindowActivated;
			fileMenu.Append (newSqlEditorMenuItem);

			exitMenuItem = new MenuItem ("Exit");
			exitMenuItem.Activated += OnExitActivated;
			fileMenu.Append (exitMenuItem);

			this.Append (file);

			// setup edit menu
			edit = new MenuItem("Edit");
			editMenu = new Menu ();
			edit.Submenu = editMenu;

			this.Append (edit);

			// setup database menu
			database = new MenuItem("Database");
			databaseMenu = new Menu ();
			database.Submenu = databaseMenu;

			newSqliteDbMenuItem = new MenuItem ("New Sqlite DB");
			newSqliteDbMenuItem.Activated += OnNewSqliteDbActivated;
			databaseMenu.Append (newSqliteDbMenuItem);
			openSqliteDbMenuItem = new MenuItem ("Open Sqlite DB");
			openSqliteDbMenuItem.Activated += OnOpenSqliteDbActivated;
			databaseMenu.Append (openSqliteDbMenuItem);
			this.Append (database);


			// initialize event handlers


			ApplicationState.Instance.CurrentConnectionChanged += (sender, e) => newSqlEditorMenuItem.Sensitive = e.DbServerConnection != null;
		}
	}
}

