using System;
using Gtk;
using GtkTestProject.Api;
using DbUtils;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;

namespace DbUtils.UI
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

		private IDbConnection _con = null;
		protected IDbConnection Connection {
			get {
				if (_con == null) {
					_con = this.ServerConnection.CreateConnection ();
				}
				return _con;
			}
		}


		protected void ExecuteCommand(){
			if (Connection.State == ConnectionState.Closed) {
				Connection.Open ();
			}

			String command;
			if (this.sqlArea.Buffer.HasSelection) {
				command = "";
			} else {
				command = this.sqlArea.Buffer.Text;
			}

			using (IDbCommand cmd = Connection.CreateCommand ()) {
				cmd.CommandText = command;
				using (IDataReader reader = cmd.ExecuteReader ()) {
					while (reader.Read ()) {
						
					}
				}
			}
		}


		# region UI stuff
		 
			
		protected Gtk.VPaned vpaned;
		protected Gtk.Toolbar toolbar;
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
			Gtk.Label l = new Gtk.Label(string.Format("Current database: {0}", this.ServerConnection.Name));
			ToolItem lTi = new ToolItem ();
			lTi.Add (l);
			toolbar.Insert (lTi, 0);

			// toolbar exec button
			ToolButton execBtn = new ToolButton (Gtk.Stock.MediaPlay);
			execBtn.Clicked += (sender, e) => this.ExecuteCommand();
			toolbar.Insert (execBtn, 1);

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

		#endregion
	}
}

