﻿using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Client.Core.Repositories.Patients
{
	public class PatientRepository : IPatientReadRepository, IPatientWriteRepository, IPersistablility
	{
		public event Action<Patient> PatientAdded;
		public event Action<Patient> PatientModified;

		private IDictionary<Guid, Patient> patients;
		private readonly IPersistenceService<IEnumerable<Patient>> persistenceService; 

		public PatientRepository(IPersistenceService<IEnumerable<Patient>> persistenceService)
		{
			this.persistenceService = persistenceService;
			patients = new Dictionary<Guid, Patient>();
		}

		public Patient GetPatientById(Guid id)
		{
			return patients.ContainsKey(id) ? patients[id] : null;
		}

		public void AddPatient(string name, Date birthday, bool alive)
		{
			var newPatientId = Guid.NewGuid();
			var newPatient = new Patient(name, birthday, alive, newPatientId);
			patients.Add(newPatientId, newPatient);

			RaisePatientAdded(newPatient);
		}

		private void RaisePatientAdded(Patient p)
		{
			var handler = PatientAdded;
			if (handler != null)
				handler(p);
		}

		private void ModifyPatientAndRaiseEvent(Guid patientId, Func<Patient, Patient> modification)
		{
			if (!patients.ContainsKey(patientId))
				throw new InvalidOperationException("there is no patient with this id");

			var oldPatientData = patients[patientId];
			var newPatientData = modification(oldPatientData);

			patients[patientId] = newPatientData;

			var handler = PatientModified;
			if (handler != null)
				handler(newPatientData);
		}

		public void SetNewName(Guid patientId, string newName)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(newName, 
															  patient.Birthday,
															  patient.Alive, 
															  patient.Id));
		}

		public void SetNewBirthday(Guid patientId, Date newBirthday)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  newBirthday, 
															  patient.Alive, 
															  patient.Id));
		}

		public void SetLivingStatus(Guid patientId, bool newLivingStatus)
		{
			ModifyPatientAndRaiseEvent(patientId, 
									   patient => new Patient(patient.Name, 
															  patient.Birthday, 
															  newLivingStatus, 
															  patient.Id));
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