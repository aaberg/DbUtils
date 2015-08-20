using System;

namespace GtkTestProject.Api
{
	public interface IDbServerConnection
	{
		IFeature[] GetFeatures (IFeature parentFeature);
	}
}

