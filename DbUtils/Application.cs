using System;

namespace Gtk3TestApp
{
	public class Application
	{
		public Application ()
		{
			MainWindow w = new MainWindow ();
			w.SetSizeRequest (800, 600);
		}

		public static void Main(){
			Gtk.Application.Init();
			new Application ();
			Gtk.Application.Run ();
		}
	}
}

