using System;
using DbUtils.Core.Api;

namespace DbUtils.Core.Api
{
	public interface IConnectionLoader
	{

		IDbServerConnection getConnection(Gtk.Window parentWindow);
	}
}

