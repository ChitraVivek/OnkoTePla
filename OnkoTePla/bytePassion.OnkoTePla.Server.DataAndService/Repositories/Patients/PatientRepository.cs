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
		private readonly IPersistenceService<IEnumerable<Patient>> persistenceService;
		private readonly IConnectionService connectionService;

		public event Action<Patient> PatientAdded;
		public event Action<Patient> PatientModified;

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
			var newPatient = new Patient(name, birthday, alive, newPatientId, externalId);
			patients.Add(newPatientId, newPatient);

			PatientAdded?.Invoke(newPatient);
			connectionService.SendPatientAddedNotification(newPatient);
		}

		public bool AddPatient(Patient newPatient)
		{
			if (!patients.ContainsKey(newPatient.Id))
			{
				patients.Add(newPatient.Id, newPatient);

				PatientAdded?.Invoke(newPatient);
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

			PatientModified?.Invoke(newPatientData);
			connectionService.SendPatientUpdatedNotification(newPatientData);
		}

		public void SetNewName(Guid patientId, string newName)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(newName, 
															  patient.Birthday,
															  patient.Alive, 
															  patient.Id,
															  patient.ExternalId));
		}

		public void SetNewBirthday(Guid patientId, Date newBirthday)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  newBirthday, 
															  patient.Alive, 
															  patient.Id,
															  patient.ExternalId));
		}

		public void SetLivingStatus(Guid patientId, bool newLivingStatus)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  patient.Birthday, 
															  newLivingStatus, 
															  patient.Id,
															  patient.ExternalId));
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
	}
}
