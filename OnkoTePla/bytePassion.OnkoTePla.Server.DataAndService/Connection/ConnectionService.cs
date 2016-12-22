using System;
using System.Collections.Generic;
using System.Threading;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.NotificationObjects;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.ResponseHandling;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.ThreadCollections;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.Threads;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.DataAndService.SessionRepository;
using NetMQ;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection
{
	public class ConnectionService : DisposingObject, 
								     IConnectionService
	{
		public event Action ConnectionStatusChanged;

		public event Action<SessionInfo> NewSessionStarted
		{
			add    { sessionRepository.NewSessionStarted += value; }
			remove { sessionRepository.NewSessionStarted -= value; }
		}
		public event Action<SessionInfo> SessionTerminated
		{
			add    { sessionRepository.SessionTerminated += value; }
			remove { sessionRepository.SessionTerminated -= value; }
		}
		public event Action<SessionInfo> LoggedInUserUpdated
		{
			add    { sessionRepository.LoggedInUserUpdated += value; }
			remove { sessionRepository.LoggedInUserUpdated -= value; }
		}
		 
		public SessionInfo GetSessionInfo (ConnectionSessionId id)
		{
			return sessionRepository.GetSessionInfo(id);
		}

		public IEnumerable<User> GetAllCurrentlyLoggedInUser()
		{
			return sessionRepository.GetAllLoggedInUsers();
		}

		public bool IsConnectionActive
		{
			get { return isConnectionActive; }
			private set
			{
				if (IsConnectionActive != value)
				{
					isConnectionActive = value;
					ConnectionStatusChanged?.Invoke();
				}
			}
		}

		private          IDataCenter dataCenter;		

		private readonly ICurrentSessionsInfo          sessionRepository;		
		private          IHeartbeatThreadCollection    heartbeatThreadCollection;
		private			 INotificationThreadCollection notificationThreadCollection;

		private UniversalResponseThread   universalResponseThread;
		private bool isConnectionActive;


		internal ConnectionService (DataCenterContainer dataCenterContainer) 								   
		{
			sessionRepository = new CurrentSessionsInfo();

			dataCenterContainer.DataCenterAvailable += OnDataCenterAvailable;
			IsConnectionActive = false;
		}

		private void OnDataCenterAvailable(DataCenterContainer dataCenterContainer, IDataCenter data)
		{
			dataCenter = data;
			dataCenterContainer.DataCenterAvailable -= OnDataCenterAvailable;
		}

		public void InitiateCommunication(Address serverAddress)
		{			
			heartbeatThreadCollection = new HeartbeatThreadCollection();
			heartbeatThreadCollection.ClientVanished += OnClientVanished;

			notificationThreadCollection = new NotificationThreadCollection();

			var responseHandlerFactory = new ResponseHandlerFactory(dataCenter, 																	
																	sessionRepository, 
																	NewConnectionEstablished,
																	NewDebugConnectionEstablishedCallback,  
																	ConnectionEnded);

			universalResponseThread = new UniversalResponseThread(serverAddress, responseHandlerFactory);
			new Thread(universalResponseThread.Run).Start();

			IsConnectionActive = true;
		}

		

		private void OnClientVanished(ConnectionSessionId connectionSessionId)
		{
			if (sessionRepository.DoesSessionExist(connectionSessionId))
			{                                                               //	the session does not exist if it was
				sessionRepository.RemoveSession(connectionSessionId);       //  ended corretly by connectionEndMessage
			}

			ConnectionEnded(connectionSessionId);
		}

		private void ConnectionEnded(ConnectionSessionId connectionSessionId)
		{
			heartbeatThreadCollection.StopThread(connectionSessionId);
			notificationThreadCollection.StopThread(connectionSessionId);
		}

		private void NewConnectionEstablished(AddressIdentifier clientAddress, ConnectionSessionId newSessionId)
		{
			sessionRepository.AddSession(newSessionId, TimeTools.GetCurrentTimeStamp().Item2, clientAddress, false);

			heartbeatThreadCollection.AddThread(clientAddress, newSessionId);
			notificationThreadCollection.AddThread(clientAddress, newSessionId);
		}
		
		private void NewDebugConnectionEstablishedCallback (AddressIdentifier clientAddress, ConnectionSessionId newSessionId)
		{
			sessionRepository.AddSession(newSessionId, TimeTools.GetCurrentTimeStamp().Item2, clientAddress, true);

			notificationThreadCollection.AddThread(clientAddress, newSessionId);
		}

		public void StopCommunication()
		{	
			sessionRepository.ClearRepository();

			if (heartbeatThreadCollection != null)
			{
				heartbeatThreadCollection.Dispose();
				heartbeatThreadCollection.ClientVanished -= OnClientVanished;
				heartbeatThreadCollection = null;
			}			

			notificationThreadCollection?.Dispose();
			notificationThreadCollection = null;

			universalResponseThread?.Stop();
			universalResponseThread = null;

			IsConnectionActive = false;
		}

		public void SendEventNotification                  (DomainEvent      domainEvent)             { notificationThreadCollection?.SendNotification(new EventNotificationObject                 (domainEvent));             }
		public void SendPatientAddedNotification           (Patient          newPatient)              { notificationThreadCollection?.SendNotification(new PatientAddedNotificationObject          (newPatient));              }
		public void SendPatientUpdatedNotification         (Patient          updatedPatient)          { notificationThreadCollection?.SendNotification(new PatientUpdatedNotificationObject        (updatedPatient));          }
		public void SendTherapyPlaceTypeAddedNotification  (TherapyPlaceType newTherapyPlaceType)     { notificationThreadCollection?.SendNotification(new TherpyPlaceTypeAddedNotificationObject  (newTherapyPlaceType));     }
		public void SendTherapyPlaceTypeUpdatedNotification(TherapyPlaceType updatedTherapyPlaceType) { notificationThreadCollection?.SendNotification(new TherpyPlaceTypeUpdatedNotificationObject(updatedTherapyPlaceType)); }
		public void SendLabelAddedNotification             (Label            newLabel)                { notificationThreadCollection?.SendNotification(new LabelAddedNotificationObject            (newLabel));                }
		public void SendLabelUpdatedNotification           (Label            updatedLabel)            { notificationThreadCollection?.SendNotification(new LabelUpdatedNotificationObject          (updatedLabel));            }

		protected override void CleanUp()
		{	
			StopCommunication();		
		}
	}
}