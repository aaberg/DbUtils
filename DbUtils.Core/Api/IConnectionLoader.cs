using System;
using DbUtils.Api;

namespace DbUtils
{
	public interface IConnectionLoader
	{

		IDbServerConnection getConnection(Gtk.Window parentWindow);
	}
}

