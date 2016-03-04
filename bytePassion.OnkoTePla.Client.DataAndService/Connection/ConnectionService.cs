﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Client.DataAndService.Connection.RequestHandling;
using bytePassion.OnkoTePla.Client.DataAndService.Connection.RequestHandling.Handlers;
using bytePassion.OnkoTePla.Client.DataAndService.Connection.Threads;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Contracts.Types;
using NetMQ;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection
{
	public class ConnectionService : DisposingObject, 
									 IConnectionService
	{
		public event Action<ConnectionEvent> ConnectionEventInvoked;

		public event Action<DomainEvent> NewDomainEventAvailable;
		public event Action<Patient> NewPatientAvailable;
		public event Action<Patient> UpdatedPatientAvailable;
		public event Action<TherapyPlaceType> NewTherapyPlaceTypeAvailable;
		public event Action<TherapyPlaceType> UpdatedTherapyPlaceTypeAvailable;

		private readonly NetMQContext zmqContext;
		private readonly SharedState<ConnectionInfo> connectionInfoVariable;


		public ConnectionService()
		{
			connectionInfoVariable = new SharedState<ConnectionInfo>(new ConnectionInfo(null, null));
			zmqContext = NetMQContext.Create();
		}
		
		private Address ServerAddress { get; set; }
		private Address	ClientAddress { get; set; }
		 
		private bool ConnectionWasTerminated { get; set; }

		private HeartbeatThead heartbeatThread;
		private NotificationThread notificationThread;
		private UniversalRequestThread universalRequestThread;
		private TimeoutBlockingQueue<IRequestHandler> requestWorkQueue; 
		 
        public void TryConnect(Address serverAddress, Address clientAddress, 
							   Action<string> errorCallback)
        {
			ConnectionWasTerminated = false;

			ServerAddress = serverAddress;
			ClientAddress = clientAddress;
			
			ConnectionEventInvoked?.Invoke(ConnectionEvent.StartedTryConnect);
			
			requestWorkQueue = new TimeoutBlockingQueue<IRequestHandler>(1000);
			universalRequestThread = new UniversalRequestThread(zmqContext, ServerAddress, requestWorkQueue);
			new Thread(universalRequestThread.Run).Start();
			
			requestWorkQueue.Put(new BeginConnectionRequestHandler(ConnectionBeginResponeReceived,
																   ClientAddress.Identifier,
																   errorMsg =>
																   {																   		
																   		ConnectionEventInvoked?.Invoke(ConnectionEvent.ConAttemptUnsuccessful);
																   		errorCallback(errorMsg);
																   }));								
		}

		public void TryDebugConnect(Address serverAddress, Address clientAddress, 
									Action<string> errorCallback)
		{
			ConnectionWasTerminated = false;

			ServerAddress = serverAddress;
			ClientAddress = clientAddress;
			
			ConnectionEventInvoked?.Invoke(ConnectionEvent.StartedTryConnect);

			requestWorkQueue = new TimeoutBlockingQueue<IRequestHandler>(1000);
			universalRequestThread = new UniversalRequestThread(zmqContext, ServerAddress, requestWorkQueue);
			new Thread(universalRequestThread.Run).Start();

			requestWorkQueue.Put(new BeginDebugConnectionRequestHandler(DebugConnectionBeginResponeReceived,
																	    ClientAddress.Identifier,
																		errorMsg =>
																		{																			 
																			 ConnectionEventInvoked?.Invoke(ConnectionEvent.ConAttemptUnsuccessful);
																			 errorCallback(errorMsg);
																		}));
		}

		public void TryDisconnect(Action dissconnectionSuccessful, Action<string> errorCallback)
		{			
			ConnectionEventInvoked?.Invoke(ConnectionEvent.StartedTryDisconnect);

			requestWorkQueue.Put(new EndConnectionRequestHandler(() =>
																 {
																	ConnectionEndResponseReceived();
																	dissconnectionSuccessful?.Invoke();
																 },														
																 connectionInfoVariable,
																 errorCallback));
		}

		public void TryAddEvents(Action<bool> resultCallback, IReadOnlyList<DomainEvent> newEvents, Action<string> errorCallback)
		{
			requestWorkQueue.Put(new TryToAddNewEventsRequestHander(resultCallback,		
																	connectionInfoVariable, 
																	newEvents, 
																	errorCallback));
		}

		public void RequestUserList(Action<IReadOnlyList<ClientUserData>> dataReceivedCallback,
									Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetUserListRequestHandler(dataReceivedCallback,
															   connectionInfoVariable, 
															   errorCallback));
		}

		public void RequestPatientList(Action<IReadOnlyList<Patient>> dataReceivedCallback, 									  
									   Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetPatientListRequestHandler(dataReceivedCallback, 
																  connectionInfoVariable, 																 
																  errorCallback));
		}
		
		public void RequestAppointmentsOfADay(Action<IReadOnlyList<AppointmentTransferData>, AggregateIdentifier, uint> dataReceivedCallback, 
											  Date day, Guid medicalPracticeId, Action<string> errorCallback) 
		{
			requestWorkQueue.Put(new GetAppointmentsOfADayRequestHandler(dataReceivedCallback, 
																		 connectionInfoVariable, 
																		 day,
																		 medicalPracticeId, 																		
																		 errorCallback));
		}

		public void RequestAppointmentsOfAPatient(Action<IReadOnlyList<AppointmentTransferData>> dataReceivedCallaback, Guid patientId, Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetAppointmentsOfAPatientRequestHandler(dataReceivedCallaback, 
																			 connectionInfoVariable,
																			 patientId, 
																			 errorCallback));
		}

		public void RequestMedicalPractice(Action<ClientMedicalPracticeData> dataReceivedCallback, 
										   Guid medicalPracticeId, uint medicalPracticeVersion,
										   Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetMedicalPracticeRequestHandler(dataReceivedCallback, 
																	  connectionInfoVariable, 
																	  medicalPracticeId, 
																	  medicalPracticeVersion, 
																	  errorCallback));
		}

		public void RequestPracticeVersionInfo(Action<uint> dataReceivedCallback, Guid medicalPracticeId, Date day, Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetPracticeVersionInfoRequestHandler(dataReceivedCallback, 
																		  connectionInfoVariable, 
																		  medicalPracticeId, 
																		  day, 
																		  errorCallback));
		}

		public void RequestTherapyPlaceTypeList(Action<IReadOnlyList<TherapyPlaceType>> dataReceivedCallback, 
												Action<string> errorCallback)
		{
			requestWorkQueue.Put(new GetTherapyPlaceListRequestHandler(dataReceivedCallback, 
																	   connectionInfoVariable, 
																	   errorCallback));
		}


		public void TryLogin(Action loginSuccessfulCallback, ClientUserData user, string password, 
							 Action<string> errorCallback)
		{
			requestWorkQueue.Put(new LoginRequestHandler(loginSuccessfulCallback, 
														 user, 
														 password,
														 connectionInfoVariable, 
														 errorCallback));
		}

		public void TryLogout(Action logoutSuccessfulCallback, ClientUserData user, Action<string> errorCallback)
		{
			requestWorkQueue.Put(new LogoutRequestHandler(logoutSuccessfulCallback, 
														  user,
														  connectionInfoVariable, 
														  errorCallback));
		}

		private void ConnectionEndResponseReceived()
		{
			Application.Current.Dispatcher.Invoke(
				() =>
				{
					ConnectionWasTerminated = true;										
					ConnectionEventInvoked?.Invoke(ConnectionEvent.Disconnected);
					CleanUpAfterDisconnection();
				});
		}
		private void ConnectionBeginResponeReceived(ConnectionSessionId connectionSessionId)
		{
			Application.Current.Dispatcher.Invoke(
				() =>
				{
					connectionInfoVariable.Value = new ConnectionInfo(connectionSessionId, null);

					heartbeatThread = new HeartbeatThead(zmqContext, ClientAddress, connectionSessionId);
					heartbeatThread.ServerVanished += OnServerVanished;
					new Thread(heartbeatThread.Run).Start();

					notificationThread = new NotificationThread(zmqContext, ClientAddress, connectionSessionId);

					notificationThread.NewDomainEventAvailable          += OnNewDomainEventAvailable;
					notificationThread.NewPatientAvailable              += OnNewPatientAvailable;
					notificationThread.UpdatedPatientAvailable          += OnUpdatedPatientAvailable;
					notificationThread.NewTherapyPlaceTypeAvailable     += OnNewTherapyPlaceTypeAvailable;
					notificationThread.UpdatedTherapyPlaceTypeAvailable += OnUpdatedTherapyPlaceTypeAvailable;

					new Thread(notificationThread.Run).Start();
					
					ConnectionEventInvoked?.Invoke(ConnectionEvent.ConnectionEstablished);
				}
			);			
		}		
		private void DebugConnectionBeginResponeReceived (ConnectionSessionId connectionSessionId)
		{
			Application.Current.Dispatcher.Invoke(
				() =>
				{
					connectionInfoVariable.Value = new ConnectionInfo(connectionSessionId, null);


					notificationThread = new NotificationThread(zmqContext, ClientAddress, connectionSessionId);

					notificationThread.NewDomainEventAvailable          += OnNewDomainEventAvailable;
					notificationThread.NewPatientAvailable              += OnNewPatientAvailable;
					notificationThread.UpdatedPatientAvailable          += OnUpdatedPatientAvailable;
					notificationThread.NewTherapyPlaceTypeAvailable     += OnNewTherapyPlaceTypeAvailable;
					notificationThread.UpdatedTherapyPlaceTypeAvailable += OnUpdatedTherapyPlaceTypeAvailable;

					new Thread(notificationThread.Run).Start();
					
					ConnectionEventInvoked?.Invoke(ConnectionEvent.ConnectionEstablished);
				}
			);
		}
		private void OnServerVanished()
		{
			if (heartbeatThread != null)
				heartbeatThread.ServerVanished -= OnServerVanished;

			notificationThread.NewDomainEventAvailable          -= OnNewDomainEventAvailable;
			notificationThread.NewPatientAvailable              -= OnNewPatientAvailable;
			notificationThread.UpdatedPatientAvailable          -= OnUpdatedPatientAvailable;
			notificationThread.NewTherapyPlaceTypeAvailable     -= OnNewTherapyPlaceTypeAvailable;
			notificationThread.UpdatedTherapyPlaceTypeAvailable -= OnUpdatedTherapyPlaceTypeAvailable;

			if (!ConnectionWasTerminated)
			{				
				ConnectionEventInvoked?.Invoke(ConnectionEvent.ConnectionLost);

				CleanUpAfterDisconnection();
			}
		}

		private void OnUpdatedTherapyPlaceTypeAvailable (TherapyPlaceType therapyPlaceType) { UpdatedTherapyPlaceTypeAvailable?.Invoke(therapyPlaceType); }
		private void OnNewTherapyPlaceTypeAvailable     (TherapyPlaceType therapyPlaceType) { NewTherapyPlaceTypeAvailable?.Invoke(therapyPlaceType);     }
		private void OnUpdatedPatientAvailable          (Patient patient)                   { UpdatedPatientAvailable?.Invoke(patient);                   }
		private void OnNewPatientAvailable              (Patient patient)                   { NewPatientAvailable?.Invoke(patient);                       }
		private void OnNewDomainEventAvailable          (DomainEvent domainEvent)           { NewDomainEventAvailable?.Invoke(domainEvent);               }

		private void CleanUpAfterDisconnection()
		{
			heartbeatThread?.Stop();
			heartbeatThread = null;

			notificationThread?.Stop();
			notificationThread = null;

			universalRequestThread?.Stop();
			universalRequestThread = null;
			requestWorkQueue = null;
		}

		protected override void CleanUp()
		{
			CleanUpAfterDisconnection();
			zmqContext.Dispose();
		}
	}
}
