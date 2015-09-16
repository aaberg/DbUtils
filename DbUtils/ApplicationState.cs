using System;
using System.Collections.ObjectModel;
using DbUtils.Core.Api;
using DbUtils.Core.State;

namespace DbUtils
{
	public class ApplicationState
	{
		public ApplicationState ()
		{
			this.StatePersistanceProvider = new SqliteStatePersistanceProvider ();

			_instance = this;

			// Make sure the first connection that is loaded is set as default connection.
			this.Connections.CollectionChanged += (sender, e) => {
				if (this.CurrentConnection == null && this.Connections.Count > 0) 
					this.CurrentConnection = this.Connections[0];
			};
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


		public IStatePersistanceProvider StatePersistanceProvider {
			get;
			private set;
		}




		#region static stuff

		private static ApplicationState _instance;
		public static ApplicationState Instance {
			get{return _instance; }
		}

		#endregion
	}
}

