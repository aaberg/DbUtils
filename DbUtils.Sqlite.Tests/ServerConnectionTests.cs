using System;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using DbUtils.Core.Api;
using DbUtils.Sqlite.Api;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

namespace DbUtils.Sqlite.Tests
{
	[TestFixture()]
	public class TestServerConnection
	{
		[Test()]
		public void TestGetColumnInfoFromMeta() {
			String path = Environment.CurrentDirectory + string.Format ("{0}Resources{0}test.db", Path.DirectorySeparatorChar);
			SqliteConnectionStringBuilder constrBuilder = new SqliteConnectionStringBuilder ();
			constrBuilder.DataSource = path;
			IDbServerConnection serverCon = new SqliteDbServerConnection ("test.db", constrBuilder.ConnectionString);

			var con = serverCon.CreateConnection ();

			List<ColumnInfo> colInfos;

			try {
				con.Open();
				using (IDbCommand cmd = con.CreateCommand ()) {
					cmd.CommandText = "select * from testtable";
					using (var reader = cmd.ExecuteReader ()) {
						colInfos = serverCon.GetColumnInfosFromMeta( reader.GetSchemaTable () );
					}
				}
			} finally {
				if (con.State == ConnectionState.Open) 
					con.Close ();
			}

			Assert.Greater (colInfos.Count, 1);

		}
	}
}

