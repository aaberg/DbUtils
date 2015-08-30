using System;
using System.IO;
using DbUtils.Api;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DbUtils.UI;

namespace DbUtils
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
		
			new ApplicationState ();

			Gtk.Application.Init();
			new Application ();
			Gtk.Application.Run ();
		}
	}
}

