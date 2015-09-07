using System;
using DbUtils.Core.Api;

namespace DbUtils.Core.State
{
	public interface IStatePersistanceProvider
	{
		void SaveSessions(IDbServerConnection[] sessions);
		IDbServerConnection[] RestoreSessions();
	}
}

