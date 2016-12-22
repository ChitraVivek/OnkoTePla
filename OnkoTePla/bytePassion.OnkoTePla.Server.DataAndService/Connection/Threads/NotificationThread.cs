using System;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Communication.NetworkMessages;
using bytePassion.OnkoTePla.Communication.NetworkMessages.Notifications;
using bytePassion.OnkoTePla.Communication.SendReceive;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.NotificationObjects;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.Threads
{
	internal class NotificationThread : IThread
	{		
		private readonly Address clientAddress;
		private readonly ConnectionSessionId sessionId;
		private readonly TimeoutBlockingQueue<NotificationObject> notificationQueue;

		private volatile bool stopRunning;

		public NotificationThread(Address clientAddress, 
								  ConnectionSessionId sessionId, 
								  TimeoutBlockingQueue<NotificationObject> notificationQueue)
		{
			this.clientAddress = clientAddress;
			this.sessionId = sessionId;
			this.notificationQueue = notificationQueue;

			stopRunning = false;
			IsRunning = false;
		}

		public void Run()
		{
			IsRunning = true;			
			using (var socket = new PushSocket())
			{
				socket.Options.Linger = TimeSpan.Zero;

				socket.Connect(clientAddress.ZmqAddress + ":" + GlobalConstants.TcpIpPort.Notification);

				while (!stopRunning)
				{
					var nextNotification = notificationQueue.TimeoutTake();

					if (nextNotification == null)
						continue;

					NetworkMessageBase outMsg;

					switch (nextNotification.NotificationType)
					{
						case NetworkMessageType.EventBusNotification:
						{
							var eventNotificationObject = (EventNotificationObject)nextNotification;
							outMsg = new EventBusNotification(eventNotificationObject.DomainEvent, sessionId);							
							break;
						}
						case NetworkMessageType.PatientAddedNotification:
						{
							var patientAddedNotificationObject = (PatientAddedNotificationObject) nextNotification;
							outMsg = new PatientAddedNotification(patientAddedNotificationObject.Patient, sessionId);
							break;
						}
						case NetworkMessageType.PatientUpdatedNotification:
						{
							var patientUpdatedNotificationObject = (PatientUpdatedNotificationObject) nextNotification;
							outMsg = new PatientAddedNotification(patientUpdatedNotificationObject.Patient, sessionId);
							break;
						}
						case NetworkMessageType.TherapyPlaceTypeAddedNotification:
						{
							var therapyPlaceTypeAddedNotificationObject = (TherpyPlaceTypeAddedNotificationObject) nextNotification;
							outMsg = new TherapyPlaceTypeAddedNotification(therapyPlaceTypeAddedNotificationObject.TherapyPlaceType, sessionId);
							break;
						}
						case NetworkMessageType.TherapyPlaceTypeUpdatedNotification:
						{
							var therapyPlaceTypeUpdatedNotificationObject = (TherpyPlaceTypeUpdatedNotificationObject) nextNotification;
							outMsg = new TherapyPlaceTypeUpdatedNotification(therapyPlaceTypeUpdatedNotificationObject.TherapyPlaceType, sessionId);
							break;
						}

						default:
							throw new ArgumentException();
					}

					socket.SendNetworkMsg(outMsg);

				}
			}

			notificationQueue.Dispose();    // It's easier to do this here than in the ThreadCollection			
			IsRunning = false;
		}

		public void Stop()
		{			
			stopRunning = true;
		}

		public bool IsRunning { get; private set; }
	}
}