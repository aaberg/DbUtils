using System;
using System.IO;
using GtkTestProject.Api;
using System.Collections.Generic;

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

		#region static stuff

		private static List<IDbServerConnection> _connections = new List<IDbServerConnection>();
		public static List<IDbServerConnection> Connections {
			get{
				return _connections;
			}
		}

		#endregion
	}
}

