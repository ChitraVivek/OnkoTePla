using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.NotificationObjects;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.Threads;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.ThreadCollections
{
	internal class NotificationThreadCollection : DisposingObject, INotificationThreadCollection
	{
		private readonly IDictionary<ConnectionSessionId, NotificationThread> notificationThreads;
		private readonly IDictionary<ConnectionSessionId, TimeoutBlockingQueue<NotificationObject>> notificationQueues; 

		public NotificationThreadCollection ()
		{
			notificationThreads = new ConcurrentDictionary<ConnectionSessionId, NotificationThread>();
			notificationQueues = new ConcurrentDictionary<ConnectionSessionId, TimeoutBlockingQueue<NotificationObject>>();
		}

		public void StopThread (ConnectionSessionId sessionId)
		{
			if (notificationThreads.ContainsKey(sessionId))
			{
				var notificationThread = notificationThreads[sessionId];				
				notificationThread.Stop();

				notificationThreads.Remove(sessionId);
				notificationQueues.Remove(sessionId);
			}
		}

		public void AddThread (AddressIdentifier clientAddressIdentifier, ConnectionSessionId id)
		{
			var clientAddress = new Address(new TcpIpProtocol(), clientAddressIdentifier);
			var notificationQueue = new TimeoutBlockingQueue<NotificationObject>(1000);
			var notificationThread = new NotificationThread(clientAddress, id, notificationQueue);
			
			notificationThreads.Add(id, notificationThread);
			notificationQueues.Add(id, notificationQueue);
						
			new Thread(notificationThread.Run).Start();
		}

		public void SendNotification(NotificationObject notificationObject)
		{
			foreach (var blockingQueue in notificationQueues.Values)
			{
				blockingQueue.Put(notificationObject);
			}
		}


		protected override void CleanUp ()
		{
			foreach (var notificationThread in notificationThreads.Values)
			{
				notificationThread.Stop();				
			}

			// Not nice but easy: 
			// disposing of the timeoutBlockingQueues is done by everyThread

			notificationThreads.Clear();
		}
	}
}