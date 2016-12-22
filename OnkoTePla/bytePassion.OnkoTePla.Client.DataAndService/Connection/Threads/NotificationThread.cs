using System;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Communication.NetworkMessages;
using bytePassion.OnkoTePla.Communication.NetworkMessages.Notifications;
using bytePassion.OnkoTePla.Communication.SendReceive;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Resources;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection.Threads
{
	internal class NotificationThread : IThread
	{
		public event Action<DomainEvent> NewDomainEventAvailable;
		public event Action<Patient> NewPatientAvailable;
		public event Action<Patient> UpdatedPatientAvailable;
		public event Action<TherapyPlaceType> NewTherapyPlaceTypeAvailable;
		public event Action<TherapyPlaceType> UpdatedTherapyPlaceTypeAvailable;
		public event Action<Label> NewLabelAvailable;
		public event Action<Label> UpdatedLabelAvailable;

		private readonly Address clientAddress;
		private readonly ConnectionSessionId sessionId;

		private volatile bool stopRunning;

		public NotificationThread(Address clientAddress,
								  ConnectionSessionId sessionId)
		{
			this.clientAddress = clientAddress;
			this.sessionId = sessionId;

			IsRunning = false;
			stopRunning = false;
		}

		public void Run()
		{
			IsRunning = true;

			try
			{
				using (var socket = new PullSocket())
				{
					socket.Options.Linger = TimeSpan.Zero;

					socket.Bind(clientAddress.ZmqAddress + ":" + GlobalConstants.TcpIpPort.Notification);
				
					while (!stopRunning)
					{
						var notification = socket.ReceiveNetworkMsg(TimeSpan.FromSeconds(1));

						if (notification == null)										
							continue;

						switch (notification.Type)
						{
							case NetworkMessageType.EventBusNotification:
							{
								var eventNotification = (EventBusNotification) notification;

								if (eventNotification.SessionId == sessionId)
									NewDomainEventAvailable(eventNotification.NewEvent);

								break;
							}
							case NetworkMessageType.PatientAddedNotification:
							{
								var patientAddedNotification = (PatientAddedNotification) notification;

								if (patientAddedNotification.SessionId == sessionId)
									NewPatientAvailable(patientAddedNotification.Patient);
													
								break;
							}
							case NetworkMessageType.PatientUpdatedNotification:
							{
								var patientUpdatedNotification = (PatientAddedNotification) notification;

								if (patientUpdatedNotification.SessionId == sessionId)
									UpdatedPatientAvailable(patientUpdatedNotification.Patient);

								break;
							}
							case NetworkMessageType.TherapyPlaceTypeAddedNotification:
							{
								var therpyPlaceTypeAddedNotification = (TherapyPlaceTypeAddedNotification) notification;

								if (therpyPlaceTypeAddedNotification.SessionId == sessionId)
									NewTherapyPlaceTypeAvailable(therpyPlaceTypeAddedNotification.TherapyPlaceType);
													
								break;
							}
							case NetworkMessageType.TherapyPlaceTypeUpdatedNotification:
							{
								var therpyPlaceTypeUpdatedNotification = (TherapyPlaceTypeUpdatedNotification) notification;

								if (therpyPlaceTypeUpdatedNotification.SessionId == sessionId)
									UpdatedTherapyPlaceTypeAvailable(therpyPlaceTypeUpdatedNotification.TherapyPlaceType);

								break;
							}
							case NetworkMessageType.LabelAddedNotification:
							{
								var labelAddedNotification = (LabelAddedNotification) notification;

								if (labelAddedNotification.SessionId == sessionId)
									NewLabelAvailable(labelAddedNotification.Label);

								break;
							}
							case NetworkMessageType.LabelUpdatedNotification:
							{
								var labelUpdatedNotification = (LabelUpdatedNotification) notification;

								if (labelUpdatedNotification.SessionId == sessionId)
									UpdatedLabelAvailable(labelUpdatedNotification.Label);

								break;
							}

							default:
								throw new ArgumentException();
						}
					}
				}
			}
			catch 
			{
				// Ignored				
			}

			IsRunning = false;
		}

		public void Stop ()
		{
			stopRunning = true;
		}

		public bool IsRunning { get; private set; }
	}
}