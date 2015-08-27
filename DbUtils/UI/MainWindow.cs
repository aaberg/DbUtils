using System;
using Gtk;
using GtkTestProject;
using GtkTestProject.Api;
using System.IO;
using DbUtils;
using System.Collections.Generic;

namespace DbUtils.UI
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
			ApplicationState.Instance.Connections.Add (con);
			ApplicationState.Instance.CurrentConnection = con;
		}

		private void loadConnectionRecursive(IDbServerConnection con, Nullable<TreeIter> parentIter, IFeature parentFeature) {
			IFeature[] features = con.GetFeatures (parentFeature);

			foreach (IFeature feature in features) {
				TreeIter iter;
				Gdk.Pixbuf icon = string.IsNullOrEmpty( feature.Icon ) ? new Gdk.Pixbuf(string.Format("Resources{0}Icons{0}folder.png", System.IO.Path.DirectorySeparatorChar)) : new Gdk.Pixbuf (feature.Icon);
				if (parentIter.HasValue) {
					iter = objectBrowserTreeStore.AppendValues (parentIter.Value, icon, feature.Text);
				} else {
					iter = objectBrowserTreeStore.AppendValues (icon, feature.Text);
				}
				loadConnectionRecursive (con, iter, feature);
			}

			objectBrowserTreeView.ExpandRow (new TreePath ("0"), false);
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
			var con = ApplicationState.Instance.Connections [0];
			SqlEditor sqlEditor = new SqlEditor (con);
//			tabbedArea.Add (sqlEditor);
//			tabbedArea.SetTabLabelText (sqlEditor, "Sql editor");

			var tabLabel = new TabLabel ("Sql window - " + con.Name);
			tabbedArea.AppendPage (sqlEditor, tabLabel);

			tabLabel.CloseClicked += (closedSender, closedEventArgs) => {
				tabbedArea.RemovePage(tabbedArea.PageNum(sqlEditor));
			};


//			HBox tabLabelBox = new HBox ();
//			tabLabelBox.Add (new Label ("Sql editor"));
//			Button btn = new Button ();
//			btn.Label = "close";
//			tabLabelBox.Add (btn);
////			tabbedArea.setla
//			tabbedArea.SetTabLabel (sqlEditor, new Label("test"));
//
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
		protected MainMenu mainMenu;


		protected Gtk.Notebook tabbedArea;


		private void Build(){

			this.Title = "Sqlite browser";

			mainVbox = new Gtk.VBox ();
			mainVbox.Spacing = 4;

			this.Add (mainVbox);

			// Main menu
			mainMenu = new MainMenu ();

			mainVbox.Add (mainMenu);

			var mainMenuBox = (Gtk.Box.BoxChild)mainVbox [mainMenu];

			mainMenuBox.Position = 0;
			mainMenuBox.Expand = false;
			mainMenuBox.Fill = false;

			mainMenu.NewSqliteDbActivated += OnNewSqliteDb;
			mainMenu.OpenSqliteDbActivated += OnOpenSqliteDb;
			mainMenu.ExitActivated += OnExit;
			mainMenu.NewSqlEditorWindowActivated += OnNewSqlTab;


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

