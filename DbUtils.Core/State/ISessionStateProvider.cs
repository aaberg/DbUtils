using System;
using DbUtils.Core.Api;

namespace DbUtils.Core.State
{
	public interface ISessionStateProvider
	{
		string getSerializedState();
		IDbServerConnection restoreSessionFromState(string state);
	}
}

