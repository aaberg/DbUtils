using System;
using System.Collections.ObjectModel;
using GtkTestProject.Api;

namespace DbUtils
{
	public class ApplicationState
	{
		public ApplicationState ()
		{
			_instance = this;
		}

		private ObservableCollection<IDbServerConnection> _connections = new ObservableCollection<IDbServerConnection> ();
		public  ObservableCollection<IDbServerConnection> Connections {
			get{
				return _connections;
			}
		}

		public event DbServerConnectionEventHandler CurrentConnectionChanged;

		private IDbServerConnection _currentConnection = null;
		public IDbServerConnection CurrentConnection {
			get { return _currentConnection; }
			set{ 
				_currentConnection = value; 
				if (CurrentConnectionChanged != null) {
					CurrentConnectionChanged (null, new DbServerConnectionEventArgs (value));
				}
			}
		}




		#region static stuff

		private static ApplicationState _instance;
		public static ApplicationState Instance {
			get{return _instance; }
		}

		#endregion
	}
}

