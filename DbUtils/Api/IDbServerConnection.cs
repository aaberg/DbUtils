using System;
using System.Data;

namespace GtkTestProject.Api
{
	public interface IDbServerConnection
	{
		IFeature[] GetFeatures (IFeature parentFeature);

		String Name {get;}

		IDbConnection CreateConnection();
	}
}

