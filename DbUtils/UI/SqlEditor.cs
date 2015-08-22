using System;
using Gtk;
using GtkTestProject.Api;
using Gtk3TestApp;
using System.Linq;

namespace Gtk3TestProject
{
	public class SqlEditor : Gtk.VBox
	{
		public SqlEditor (IDbServerConnection serverConnection)
		{
			this.ServerConnection = serverConnection;
			this.Build ();
		}

		public IDbServerConnection ServerConnection {
			get;
			private set;
		}
			
		protected Gtk.VPaned vpaned;
		protected Gtk.Toolbar toolbar;
		protected Gtk.ComboBox currentDatabaseDropDown;
		protected Gtk.TextView sqlArea;
		protected Gtk.Notebook resultNoteBook;
		protected Gtk.TreeView sqlResult;

		private void Build() {

			// toolbar
			toolbar = new Toolbar();
			toolbar.MarginLeft = 10;
			this.Add (toolbar);
			var toolbarBox = (Gtk.Box.BoxChild)this [toolbar];
			toolbarBox.Fill = false;
			toolbarBox.Expand = false;
			toolbarBox.Position = 0;

			// toolbar choose database dropdown
			Gtk.Label l = new Gtk.Label("Current database");
			ToolItem lTi = new ToolItem ();
			lTi.Add (l);
			toolbar.Insert (lTi, 0);

			currentDatabaseDropDown = new ComboBox (Gtk3TestApp.Application.Connections.Select(c => c.Name).ToArray());
			currentDatabaseDropDown.WidthRequest = 120;
			ToolItem dropdownWrapper = new ToolItem ();
			dropdownWrapper.Add (currentDatabaseDropDown);
			toolbar.Insert (dropdownWrapper, 1);

			// toolbar exec button
			ToolButton execBtn = new ToolButton (Gtk.Stock.MediaPlay);
			toolbar.Insert (execBtn, 2);

			// vpaned
			vpaned = new VPaned();
			this.Add (vpaned);

			// Sql area textbox
			sqlArea = new TextView ();
			sqlArea.CanFocus = true;
			vpaned.Add1 (sqlArea);

			// result tabs
			resultNoteBook = new Notebook ();
			sqlResult = new TreeView ();
			resultNoteBook.Add (sqlResult);
			Label sqlResultLabel = new Label ("Results");
			resultNoteBook.SetTabLabel (sqlResult, sqlResultLabel);

			vpaned.Add2 (resultNoteBook);

			vpaned.Position = 300;
		}
	}
}

