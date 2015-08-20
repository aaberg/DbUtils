using System;
using Gtk;
using GtkTestProject;
using GtkTestProject.Api;

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

			CellRendererText textRendere = new CellRendererText ();
			obCol.PackStart (textRendere, true);

			objectBrowserTreeView.AppendColumn (obCol);

			obCol.AddAttribute (textRendere, "Text", 0);
		}

		protected void OnNewConnection (object sender, EventArgs e) {
			var conncetion = new SqliteConnectionLoader ().getConnection (this);
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
				if (parentIter.HasValue) {
					Console.WriteLine (feature.Text);
					iter = objectBrowserTreeStore.AppendValues (parentIter.Value, feature);
				} else {
					iter = objectBrowserTreeStore.AppendValues (feature);
				}
				loadConnectionRecursive (con, iter, feature);
			}

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
		protected Gtk.MenuItem exitMenuItem;
		protected Gtk.MenuItem edit;
		protected Gtk.Menu editMenu;


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

			exitMenuItem = new MenuItem ("Exit");
			exitMenuItem.Activated += OnExit;
			fileMenu.Append (exitMenuItem);
			mainMenu.Append (file);

			// setup edit menu
			edit = new MenuItem("Edit");
			editMenu = new Menu ();
			edit.Submenu = editMenu;

			mainMenu.Append (edit);
		


			// left paned
			leftPaned = new Gtk.HPaned();
			leftPaned.Position = 300;
			leftPaned.CanFocus = true;
			mainVbox.Add (leftPaned);
			var objectBrowserScrolledWindow = new Gtk.ScrolledWindow ();
			objectBrowserScrolledWindow.ShadowType = ShadowType.EtchedIn;
			leftPaned.Add1 (objectBrowserScrolledWindow);

			objectBrowserTreeView = new TreeView ();
			objectBrowserScrolledWindow.Add (objectBrowserTreeView);

			objectBrowserTreeStore = new TreeStore (typeof(string));
			objectBrowserTreeView.Model = objectBrowserTreeStore;

			dummyButton = new Gtk.Button ("Dummy");
			leftPaned.Add2 (dummyButton);


			// status bar
			statusBar = new Statusbar();
			mainVbox.Add (statusBar);
			var statusBarBox = (Gtk.Box.BoxChild)mainVbox [statusBar];
			statusBarBox.Position = 2;
			statusBarBox.Expand = false;
			statusBarBox.Fill = false;

			this.ShowAll ();

			this.DeleteEvent += OnExit;
		}

		#endregion
	}
}

