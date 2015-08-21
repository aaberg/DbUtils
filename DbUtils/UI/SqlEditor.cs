using System;
using Gtk;

namespace Gtk3TestProject
{
	public class SqlEditor : Gtk.VPaned
	{
		public SqlEditor ()
		{
			this.Build ();
		}

		protected Gtk.TextView sqlArea;
		protected Gtk.Notebook resultNoteBook;
		protected Gtk.TreeView sqlResult;

		private void Build() {
			sqlArea = new TextView ();
			sqlArea.CanFocus = true;
			this.Add1 (sqlArea);

			resultNoteBook = new Notebook ();
			sqlResult = new TreeView ();
			resultNoteBook.Add (sqlResult);
			Label sqlResultLabel = new Label ("Results");
			resultNoteBook.SetTabLabel (sqlResult, sqlResultLabel);

			this.Add2 (resultNoteBook);

			this.Position = 300;
		}
	}
}

