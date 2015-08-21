using System;
using Gtk;
using GtkTestProject;
using GtkTestProject.Api;
using System.IO;
using Gtk3TestProject;

namespace Gtk3TestApp
{
	public class MainWindow : Gtk.Window
	{
		public MainWindow () :
		base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			InitObjectBrowser ();
		}

		protected void OnExit(object sender, EventArgs e) {
			Gtk.Application.Quit ();
		}

		private void InitObjectBrowser() {

			TreeViewColumn obCol = new TreeViewColumn ();
			obCol.Title = "Object browser";

			CellRendererPixbuf iconRendere = new CellRendererPixbuf ();
			CellRendererText textRendere = new CellRendererText ();

			obCol.PackStart (iconRendere, false);
			obCol.AddAttribute (iconRendere, "pixbuf", 0);
			obCol.PackStart (textRendere, true);		
			obCol.AddAttribute (textRendere, "text", 1);

			objectBrowserTreeView.AppendColumn (obCol);


		}

		protected void OnNewConnection (object sender, EventArgs e) {
			var conncetion = new SqliteConnectionLoader (false).getConnection (this);
			if (conncetion == null)
				return;

			loadConnection (conncetion);

		}

		private void loadConnection(IDbServerConnection con) {
			loadConnectionRecursive (con, new Nullable<TreeIter>(), (IFeature)null);
		}

		private void loadConnectionRecursive(IDbServerConnection con, Nullable<TreeIter> parentIter, IFeature parentFeature) {
			IFeature[] features = con.GetFeatures (parentFeature);

			foreach (IFeature feature in features) {
				TreeIter iter;
				Gdk.Pixbuf icon = string.IsNullOrEmpty( feature.Icon ) ? new Gdk.Pixbuf(string.Format("Resources{0}Icons{0}folder.png", System.IO.Path.DirectorySeparatorChar)) : new Gdk.Pixbuf (feature.Icon);
				icon.Save ("/home/lars/temp/test.png", "png");
				if (parentIter.HasValue) {
					Console.WriteLine (feature.Text);
					iter = objectBrowserTreeStore.AppendValues (parentIter.Value, icon, feature.Text);
				} else {
					iter = objectBrowserTreeStore.AppendValues (icon, feature.Text);
				}
				loadConnectionRecursive (con, iter, feature);
			}
		}

		protected void OnOpenSqliteDb(object sender, EventArgs e) {
			SqliteConnectionLoader loader = new SqliteConnectionLoader (false);
			IDbServerConnection connection = loader.getConnection (this);
			if (connection != null)
				loadConnection (connection);
		}

		protected void OnNewSqliteDb(object sende, EventArgs e) {
			SqliteConnectionLoader loader = new SqliteConnectionLoader (true);
			IDbServerConnection connection = loader.getConnection (this);
			if (connection != null)
				loadConnection (connection);
		}

		protected void OnNewSqlTab(object sender, EventArgs e) {
			SqlEditor sqlEditor = new SqlEditor ();
			tabbedArea.Add (sqlEditor);
			tabbedArea.SetTabLabelText (sqlEditor, "Sql editor");
			tabbedArea.ShowAll ();
		}


		#region UI stuff

		protected Gtk.VBox mainVbox;
		protected Gtk.HPaned leftPaned;
		protected Gtk.Statusbar statusBar;
		protected Gtk.TreeView objectBrowserTreeView;
		protected Gtk.TreeStore objectBrowserTreeStore;

		protected Gtk.Button dummyButton;


		// menu
		protected Gtk.MenuBar mainMenu;
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


		private void Build(){

			this.Title = "Sqlite browser";

			mainVbox = new Gtk.VBox ();
			mainVbox.Spacing = 4;

			this.Add (mainVbox);

			mainMenu = new Gtk.MenuBar ();

			mainVbox.Add (mainMenu);

			var mainMenuBox = (Gtk.Box.BoxChild)mainVbox [mainMenu];

			mainMenuBox.Position = 0;
			mainMenuBox.Expand = false;
			mainMenuBox.Fill = false;

			// setup file menu
			file = new MenuItem("File");
			fileMenu = new Menu();
			file.Submenu = fileMenu;

			openMenuItem = new MenuItem ("Open Sqlite database");
			openMenuItem.Activated += OnNewConnection;
			fileMenu.Append (openMenuItem);

			newSqlEditorMenuItem = new MenuItem ("New Sql Editor Tab");
			newSqlEditorMenuItem.Activated += OnNewSqlTab;
			fileMenu.Append (newSqlEditorMenuItem);

			exitMenuItem = new MenuItem ("Exit");
			exitMenuItem.Activated += OnExit;
			fileMenu.Append (exitMenuItem);

			mainMenu.Append (file);

			// setup edit menu
			edit = new MenuItem("Edit");
			editMenu = new Menu ();
			edit.Submenu = editMenu;

			mainMenu.Append (edit);

			// setup database menu
			database = new MenuItem("Database");
			databaseMenu = new Menu ();
			database.Submenu = databaseMenu;
		
			newSqliteDbMenuItem = new MenuItem ("New Sqlite DB");
			newSqliteDbMenuItem.Activated += OnNewSqliteDb;
			databaseMenu.Append (newSqliteDbMenuItem);
			openSqliteDbMenuItem = new MenuItem ("Open Sqlite DB");
			openSqliteDbMenuItem.Activated += OnOpenSqliteDb;
			databaseMenu.Append (openSqliteDbMenuItem);
			mainMenu.Append (database);


			// left paned
			leftPaned = new Gtk.HPaned();
			leftPaned.CanFocus = true;
			mainVbox.Add (leftPaned);
			var objectBrowserScrolledWindow = new Gtk.ScrolledWindow ();
			objectBrowserScrolledWindow.ShadowType = ShadowType.EtchedIn;
			leftPaned.Add1 (objectBrowserScrolledWindow);

			objectBrowserTreeView = new TreeView ();
			objectBrowserScrolledWindow.Add (objectBrowserTreeView);

			objectBrowserTreeStore = new TreeStore (typeof(Gdk.Pixbuf), typeof(string));
			objectBrowserTreeView.Model = objectBrowserTreeStore;

			// main tabbed area
			tabbedArea = new Notebook();
			leftPaned.Add2 (tabbedArea);

			// status bar
			statusBar = new Statusbar();
			mainVbox.Add (statusBar);
			var statusBarBox = (Gtk.Box.BoxChild)mainVbox [statusBar];
			statusBarBox.Position = 2;
			statusBarBox.Expand = false;
			statusBarBox.Fill = false;

			this.ShowAll (	);

			leftPaned.Position = 400;

			this.DeleteEvent += OnExit;
		}

		#endregion
	}
}

