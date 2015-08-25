using System;
using Gtk;
using GtkTestProject.Api;
using DbUtils;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;

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


		private TreeView sqlGrid = null;
		protected void ExecuteCommand(){
			int rowCounter = 0;

			// cleanup old grid
			if (sqlGrid != null) sqlResultContainer.Remove (sqlGrid);

			// Open connection, if not already opened
			if (Connection.State == ConnectionState.Closed) {
				Connection.Open ();
			}

			String command;
			if (this.sqlArea.Buffer.HasSelection) {
				command = "";
			} else {
				command = this.sqlArea.Buffer.Text;
			}

			try {
				using (IDbCommand cmd = Connection.CreateCommand ()) {
					cmd.CommandText = command;

					if (command.ToLower().StartsWith("select")) {
						using (IDataReader reader = cmd.ExecuteReader ()) {
							DataTable meta = reader.GetSchemaTable ();
							ListStore ls = InitSqlResultGrid (meta);

							int numberOfCols = meta.Rows.Count;
							while (reader.Read ()) {
								object[] values = new object[numberOfCols];
								reader.GetValues (values);
								ls.AppendValues (	values);
								rowCounter++;
							}

						}

						outputView.Buffer.Text = string.Format ("Query OK{0}Rows returned: {1}", System.Environment.NewLine, rowCounter);
						resultNoteBook.Page = 0;
					} else {
						int res = cmd.ExecuteNonQuery();
						outputView.Buffer.Text = string.Format ("Command OK{0}Rows affected: {1}", System.Environment.NewLine, res);
						resultNoteBook.Page = 1;
					}
				}


			} catch (Exception e) {
				outputView.Buffer.Text = string.Format ("Query Error: {0}", e.Message);
				resultNoteBook.Page = 1;
			}



			this.sqlResultContainer.ShowAll ();
		}
			
		private ListStore InitSqlResultGrid(DataTable meta) {

			TreeView sqlResTreeView = new TreeView ();
			List<Type> types = new List<Type> ();

			for (int i = 0, maxCount = this.ServerConnection.GetColumnInfosFromMeta (meta).Count; i < maxCount; i++) {
				var columnInfo = this.ServerConnection.GetColumnInfosFromMeta (meta) [i];

				sqlResTreeView.AppendColumn (columnInfo.ColumnName, new CellRendererText (), "text", i);

				types.Add (typeof(string));
			}

			ListStore ls = new ListStore (types.ToArray ());

			sqlResTreeView.Model = ls;
			sqlResultContainer.Add (sqlResTreeView);

			sqlGrid = sqlResTreeView;

			return ls;
		}


		# region UI stuff
		 
			
		protected Gtk.VPaned vpaned;
		protected Gtk.Toolbar toolbar;
		protected Gtk.TextView sqlArea;
		protected Gtk.Notebook resultNoteBook;
		protected Gtk.ScrolledWindow sqlResultContainer;
		protected Gtk.TextView outputView;

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
			sqlArea.Margin = 10;
			vpaned.Add1 (sqlArea);

			// result tabs
			resultNoteBook = new Notebook ();
			sqlResultContainer = new Gtk.ScrolledWindow ();
			resultNoteBook.Add (sqlResultContainer);
			resultNoteBook.SetTabLabel (sqlResultContainer, new Label ("Results"));

			outputView = new TextView ();
			outputView.Editable = false;
			resultNoteBook.Add (outputView);
			resultNoteBook.SetTabLabel (outputView, new Label ("Output"));

			vpaned.Add2 (resultNoteBook);

			vpaned.Position = 300;
		}

		#endregion
	}
}

