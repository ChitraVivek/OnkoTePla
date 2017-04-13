using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Connection;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients
{
	public class PatientRepository : IPatientRepository
	{
		public event Action<Patient> NewPatientAvailable;
		public event Action<Patient> UpdatedPatientAvailable;

		private readonly IPersistenceService<IEnumerable<Patient>> persistenceService;
		private readonly IConnectionService connectionService;
		

		private IDictionary<Guid, Patient> patients;
		

		public PatientRepository(IPersistenceService<IEnumerable<Patient>> persistenceService, IConnectionService connectionService)
		{
			this.persistenceService = persistenceService;
			this.connectionService = connectionService;
			patients = new Dictionary<Guid, Patient>();
		}

		public Patient GetPatientById(Guid id)
		{
			return patients.ContainsKey(id) ? patients[id] : null;
		}

		public IEnumerable<Patient> GetAllPatients()
		{
			return patients.Values.ToList();
		}

		public void AddPatient(string name, Date birthday, bool alive, string externalId)
		{
			var newPatientId = Guid.NewGuid();
			var newPatient = new Patient(name, birthday, alive, newPatientId, externalId, false);
			patients.Add(newPatientId, newPatient);

			NewPatientAvailable?.Invoke(newPatient);
			connectionService.SendPatientAddedNotification(newPatient);
		}

		public bool AddPatient(Patient newPatient)
		{
			if (!patients.ContainsKey(newPatient.Id))
			{
				patients.Add(newPatient.Id, newPatient);

				NewPatientAvailable?.Invoke(newPatient);
				connectionService.SendPatientAddedNotification(newPatient);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void ModifyPatientAndRaiseEvent(Guid patientId, Func<Patient, Patient> modification)
		{
			if (!patients.ContainsKey(patientId))
				throw new InvalidOperationException("there is no patient with this id");

			var oldPatientData = patients[patientId];
			var newPatientData = modification(oldPatientData);

			patients[patientId] = newPatientData;

			UpdatedPatientAvailable?.Invoke(newPatientData);
			connectionService.SendPatientUpdatedNotification(newPatientData);
		}

		public void SetNewName(Guid patientId, string newName)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(newName, 
															  patient.Birthday,
															  patient.Alive, 
															  patient.Id,
															  patient.ExternalId,
															  patient.IsHidden));
		}

		public void SetNewBirthday(Guid patientId, Date newBirthday)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  newBirthday, 
															  patient.Alive, 
															  patient.Id,
															  patient.ExternalId,
															  patient.IsHidden));
		}

		public void SetLivingStatus(Guid patientId, bool newLivingStatus)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  patient.Birthday, 
															  newLivingStatus, 
															  patient.Id,
															  patient.ExternalId,
															  patient.IsHidden));
		}
		
		public void UpdatePatient(Guid patientId, string newName, Date newBirthday, bool newLivingStatus, bool newIsHiddenStatus)
		{
			ModifyPatientAndRaiseEvent(patientId,
									   patient => new Patient(newName,
															  newBirthday,
															  newLivingStatus,
															  patient.Id,
															  patient.ExternalId,
															  newIsHiddenStatus));
		}

		public void PersistRepository()
		{
			persistenceService.Persist(patients.Values.ToList());
		}

		public void LoadRepository()
		{
			patients = persistenceService.Load().ToDictionary(patient => patient.Id, 
															  patient => patient);
		}
		
		public void RequestAllPatientList(Action<IReadOnlyList<Patient>> patientListAvailableCallback, Action<string> errorCallback)
		{
			patientListAvailableCallback?.Invoke(patients.Values.ToList());
		}

		public void AddPatient(Patient newPatient, Action<string> errorCallback)
		{
			AddPatient(newPatient);
		}
	}
}
