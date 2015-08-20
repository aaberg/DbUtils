using System;
using GtkTestProject.Api;

namespace GtkTestProject
{
	public interface IConnectionLoader
	{

		IDbServerConnection getConnection(Gtk.Window parentWindow);
	}
}

