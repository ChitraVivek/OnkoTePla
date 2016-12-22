using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.Threads;
using NetMQ;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.ThreadCollections
{
	internal class HeartbeatThreadCollection : DisposingObject, IHeartbeatThreadCollection
	{
		public event Action<ConnectionSessionId> ClientVanished;

		private readonly IDictionary<ConnectionSessionId, HeartbeatThread> heartbeatThreads;

		public HeartbeatThreadCollection()
		{
			heartbeatThreads = new ConcurrentDictionary<ConnectionSessionId, HeartbeatThread>();
		}		

		public void StopThread(ConnectionSessionId sessionId)
		{
			if (heartbeatThreads.ContainsKey(sessionId))
			{
				var heartbeatThread = heartbeatThreads[sessionId];
				heartbeatThread.ClientVanished -= HeartbeatOnClientVanished;
				heartbeatThread.Stop();
				heartbeatThreads.Remove(sessionId);
			}
		}

		public void AddThread(AddressIdentifier clientAddressIdentifier, ConnectionSessionId id)
		{
			var clientAddress = new Address(new TcpIpProtocol(), clientAddressIdentifier);
			var heartbeatThread = new HeartbeatThread(clientAddress, id);

			heartbeatThreads.Add(id, heartbeatThread);
			heartbeatThread.ClientVanished += HeartbeatOnClientVanished;
			new Thread(heartbeatThread.Run).Start();
		}

		private void HeartbeatOnClientVanished (ConnectionSessionId connectionSessionId)
		{							
			ClientVanished?.Invoke(connectionSessionId);					
		}

		protected override void CleanUp()
		{
			foreach (var heartbeatThread in heartbeatThreads.Values)
			{
				heartbeatThread.Stop();
				heartbeatThread.ClientVanished -= HeartbeatOnClientVanished;
			}
			heartbeatThreads.Clear();
		}
	}
}
