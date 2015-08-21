using System;
using System.IO;

namespace Gtk3TestApp
{
	public class Application
	{
		public Application ()
		{
			MainWindow w = new MainWindow ();
			w.SetSizeRequest (1024, 700);
//			w.Maximize ();
		}

		public static void Main(){
			

			Gtk.Application.Init();
			new Application ();
			Gtk.Application.Run ();
		}
	}
}

