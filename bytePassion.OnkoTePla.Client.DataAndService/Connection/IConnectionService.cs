﻿using System;
using System.Collections.Generic;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection
{
	internal interface IConnectionService : IDisposable
	{
		event Action<ConnectionEvent> ConnectionEventInvoked;
		
		Address          ServerAddress    { get; }
		Address          ClientAddress    { get; }
		ConnectionStatus ConnectionStatus { get; }
		
		void TryConnect     (Address serverAddress, Address clientAddress, Action<string> errorCallback);
		void TryDebugConnect(Address serverAddress, Address clientAddress, Action<string> errorCallback);
		void TryDisconnect  (Action<string> errorCallback);
		
		void RequestUserList (Action<IReadOnlyList<ClientUserData>> dataReceivedCallback,
							  Action<string> errorCallback);

		void TryLogin (Action loginSuccessfulCallback,
					   ClientUserData user, string password,
					   Action<string> errorCallback);
		void TryLogout(Action logoutSuccessfulCallback,
					   ClientUserData user,
					   Action<string> errorCallback); 
	}
}