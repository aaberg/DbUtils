using System;

namespace DbUtils
{
	public class MainToolbar : Gtk.Toolbar
	{
		public MainToolbar ()
		{
			Build ();
		}

		global::Gtk.ToolButton connectBtn;
		global::Gtk.ToolButton disconnectBtn;

		private void Build(){

			connectBtn = new global::Gtk.ToolButton (Gtk.Stock.Connect);
			this.Add (connectBtn);

			disconnectBtn = new global::Gtk.ToolButton (Gtk.Stock.Disconnect);
			disconnectBtn.Sensitive = false;
			this.Add (disconnectBtn);

			this.ShowAll ();
		}
	}
}

