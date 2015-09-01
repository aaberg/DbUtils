using System;
using System.Data;
using DbUtils;
using System.Collections.Generic;

namespace DbUtils.Core.Api
{
	public interface IDbServerConnection
	{
		IFeature[] GetFeatures (IFeature parentFeature);

		String Name {get;}

		IDbConnection CreateConnection();

		List<ColumnInfo> GetColumnInfosFromMeta (DataTable meta);
	}
}

