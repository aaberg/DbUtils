using System;
using GtkTestProject.Api;

namespace GtkTestProject.Sqlite
{
	public class SqliteFeature : IFeature
	{
		public SqliteFeature (string key, string text)
		{ 
			this.Text = text;
			this.Key = key;
		}

		#region IFeature implementation

		public string Text { get; private set; }

		#endregion

		#region Properties

		public string Key {
			get;
			private set;
		}

		#endregion
	}
}

