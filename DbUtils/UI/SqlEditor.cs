using System;
using Gtk;
using DbUtils.Api;
using DbUtils;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using Mono.TextEditor.Highlighting;
using System.Reflection;

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
			if (string.IsNullOrWhiteSpace(sqlArea.SelectedText)) {
				command = this.sqlArea.Text;
			} else {
				command = this.sqlArea.SelectedText;
			}

			try {
				using (IDbCommand cmd = Connection.CreateCommand ()) {
					cmd.CommandText = command.Trim(' ', '\r', '\n');

					if (cmd.CommandText.ToLower().StartsWith("select")) {
						using (IDataReader reader = cmd.ExecuteReader ()) {
							DataTable meta = reader.GetSchemaTable ();
							ListStore ls = InitSqlResultGrid (meta);

							int numberOfCols = meta.Rows.Count;
							while (reader.Read ()) {
								object[] values = new object[numberOfCols];
								reader.GetValues (values);
								ls.AppendValues ( values.Select((o) => o.ToString()).ToArray() );
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
			sqlResTreeView.RulesHint = true;
			List<Type> types = new List<Type> ();

			for (int i = 0, maxCount = this.ServerConnection.GetColumnInfosFromMeta (meta).Count; i < maxCount; i++) {
				var columnInfo = this.ServerConnection.GetColumnInfosFromMeta (meta) [i];

				TreeViewColumn tvc = new TreeViewColumn (columnInfo.ColumnName, new CellRendererText(), "text", i);
				tvc.Resizable = true;
				sqlResTreeView.AppendColumn (tvc);

				types.Add (typeof(string));
			}

			ListStore ls = new ListStore (types.ToArray ());

			sqlResTreeView.Model = ls;
			sqlResultContainer.Add (sqlResTreeView);

			sqlGrid = sqlResTreeView;

			return ls;
		}

		private SyntaxMode loadSyntaxMode() {
			var syntaxModeStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("DbUtils.UI.SyntaxModes.SqliteSyntaxMode.xml");
			return SyntaxMode.Read (syntaxModeStream);
		}


		# region UI stuff
		 
			
		protected Gtk.VPaned vpaned;
		protected Gtk.Toolbar toolbar;
		protected Mono.TextEditor.TextEditor sqlArea;
		protected Gtk.Notebook resultNoteBook;
		protected Gtk.ScrolledWindow sqlResultContainer;
		protected Gtk.TextView outputView;

		private void Build() {

			// toolbar
			toolbar = new Toolbar();
//			toolbar.MarginLeft = 10;
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
				
			// Sql area textbox wrapped in a scrolled window
			ScrolledWindow sqlAreaScroll = new ScrolledWindow();
			vpaned.Add1 (sqlAreaScroll);
			sqlArea = new Mono.TextEditor.TextEditor();
			sqlArea.Text = string.Format ("{0}{0}", System.Environment.NewLine);
			sqlArea.SetCaretTo (1, 1);
			sqlArea.CanFocus = true;
			sqlArea.IsFocus = true;

			var syntaxMode = loadSyntaxMode ();
			sqlArea.Document.SyntaxMode = syntaxMode;

			sqlAreaScroll.Add (sqlArea);

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

			vpaned.Position = 180;
		}

		#endregion
	}
}

