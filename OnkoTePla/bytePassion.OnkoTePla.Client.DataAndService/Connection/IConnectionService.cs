using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection
{
	public interface IConnectionService : IDisposable
	{
		event Action<ConnectionEvent> ConnectionEventInvoked;


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////                                                                                                                            ////////
		////////                                       events triggered by push notifications                                               ////////
		////////                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                                                                                                            ////////
		event Action<DomainEvent> NewDomainEventAvailable;                                                                                  ////////
		event Action<Patient> NewPatientAvailable;                                                                                          ////////
		event Action<Patient> UpdatedPatientAvailable;                                                                                      ////////
		event Action<TherapyPlaceType> NewTherapyPlaceTypeAvailable;                                                                        ////////
		event Action<TherapyPlaceType> UpdatedTherapyPlaceTypeAvailable;                                                                    ////////
		event Action<Label> NewLabelAvailable;                                                                                              ////////
		event Action<Label> UpdatedLabelAvailable;																							////////
																																			////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////                                                                                                                            ////////
		////////                                              connect and disconnect			                                            ////////
		////////                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		                                                                                                                                    ////////
		void TryConnect     (Address serverAddress, Address clientAddress, Action<string> errorCallback);                                   ////////
		void TryDebugConnect(Address serverAddress, Address clientAddress, Action<string> errorCallback);                                   ////////
		void TryDisconnect  (Action dissconnectionSuccessful, Action<string> errorCallback);                                                ////////
		                                                                                                                                    ////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////                                                                                                                            ////////
		////////                                                 login and logout		    	                                            ////////
		////////                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                                                                                                            ////////
		void TryLogin (Action loginSuccessfulCallback,  ClientUserData user, string password, Action<string> errorCallback);                ////////
		void TryLogout(Action logoutSuccessfulCallback, ClientUserData user,                  Action<string> errorCallback);                ////////
                                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////                                                                                                                            ////////
		////////                                                    data-requests		    	                                            ////////
		////////                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		                                                                                                                                    ////////
		void RequestUserList(Action<IReadOnlyList<ClientUserData>> dataReceivedCallback,                                                    ////////
							  Action<string> errorCallback);                                                                                ////////
                                                                                                                                            ////////
		void RequestPatientList(Action<IReadOnlyList<Patient>> dataReceivedCallback,                                                        ////////
								Action<string> errorCallback);                                                                              ////////
                                                                                                                                            ////////
		void RequestAppointmentsOfADay(Action<IReadOnlyList<AppointmentTransferData>, AggregateIdentifier, uint> dataReceivedCallback,      ////////
									   Date day, Guid medicalPracticeId,							                                        ////////
									   Action<string> errorCallback);                                                                       ////////
                                                                                                                                            ////////
		void RequestAppointmentsOfAPatient (Action<IReadOnlyList<AppointmentTransferData>> dataReceivedCallaback,                           ////////
										   Guid patientId,                                                                                  ////////
										   Action<string> errorCallback);                                                                   ////////
                                                                                                                                            ////////
		void RequestMedicalPractice(Action<ClientMedicalPracticeData> dataReceivedCallback,                                                 ////////
									Guid medicalPracticeId, uint medicalPracticeVersion,                                                    ////////
									Action<string> errorCallback);                                                                          ////////
                                                                                                                                            ////////
		void RequestPracticeVersionInfo(Action<uint> dataReceivedCallback,                                                                  ////////
										Guid medicalPracticeId, Date day,                                                                   ////////
										Action<string> errorCallback);                                                                      ////////
                                                                                                                                            ////////
		void RequestTherapyPlaceTypeList(Action<IReadOnlyList<TherapyPlaceType>> dataReceivedCallback,                                      ////////
										 Action<string> errorCallback);                                                                     ////////
                                                                                                                                            ////////
		void RequestLabelList (Action<IReadOnlyList<Label>> dataReceivedCallback,                                                           ////////
							  Action<string> errorCallback);																				////////
                                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		void AddPatientRequest(Patient newPatient, Action<string> errorCallback);


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////                                                                                                                            ////////
		////////                                                    other-requests		    	                                            ////////
		////////                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                                                                                                            ////////
		void TryAddEvents(Action<bool> resultCallback,   IReadOnlyList<DomainEvent> newEvents, Action<string> errorCallback);               ////////
		void TryToGetLock(Action<bool> resultCallback,   Guid medicalPracticeId, Date day,     Action<string> errorCallback);				////////
		void ReleaseLock (Action actionCompleteCallback, Guid medicalPracticeId, Date day,     Action<string> errorCallback);				////////
                                                                                                                                            ////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}