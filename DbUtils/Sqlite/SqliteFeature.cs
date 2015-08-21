using System;
using GtkTestProject.Api;

namespace GtkTestProject.Sqlite
{
	public class SqliteFeature : IFeature
	{
		public SqliteFeature(string key, string text) :
		this(key, text, ""){
		}
		
		public SqliteFeature (string key, string text, string icon)
		{ 
			this.Text = text;
			this.Key = key;
			this.Icon = icon;
		}

		#region IFeature implementation

		public string Text { get; private set; }

		public string Icon { get; private set; }

		#endregion

		#region Properties

		public virtual string Key {
			get;
			private set;
		}

		#endregion
	}
}

