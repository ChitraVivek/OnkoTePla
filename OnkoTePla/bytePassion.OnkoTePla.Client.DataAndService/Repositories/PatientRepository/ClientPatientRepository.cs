using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.DataAndService.Connection;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository
{
	public class ClientPatientRepository : DisposingObject, IClientPatientRepository
	{
		public event Action<Patient> NewPatientAvailable;
		public event Action<Patient> UpdatedPatientAvailable;

		private readonly IConnectionService connectionService;
		private  IDictionary<Guid, Patient> cachedPatients; 

		public ClientPatientRepository(IConnectionService connectionService)
		{
			this.connectionService = connectionService;		
			
			connectionService.NewPatientAvailable     += OnNewPatientAvailable;	
			connectionService.UpdatedPatientAvailable += OnUpdatedPatientAvailable;
		}

		private void OnUpdatedPatientAvailable(Patient patient)
		{
			cachedPatients.Remove(patient.Id);
			cachedPatients.Add(patient.Id, patient);

			UpdatedPatientAvailable?.Invoke(patient);
		}

		private void OnNewPatientAvailable(Patient patient)
		{
			cachedPatients.Add(patient.Id, patient);

			NewPatientAvailable?.Invoke(patient);
		}		

		public void RequestPatient(Action<Patient> patientAvailableCallback, Guid patientId, Action<string> errorCallback)
		{
			if (cachedPatients == null)
			{
				connectionService.RequestPatientList(
					patientList =>
					{
						cachedPatients = new ConcurrentDictionary<Guid, Patient>();
						foreach (var patient in patientList)
						{
							cachedPatients.Add(patient.Id, patient);
						}

						patientAvailableCallback(GetPatient(patientId, errorCallback));
					},
					errorCallback
				);
			}
			else
			{
				patientAvailableCallback(GetPatient(patientId, errorCallback));
			}
		}
		
		public void RequestAllPatientList(Action<IReadOnlyList<Patient>> patientListAvailableCallback, Action<string> errorCallback)
		{
			if (cachedPatients == null)
			{
				connectionService.RequestPatientList(
					patientList =>
					{
						cachedPatients = new Dictionary<Guid, Patient>();
						foreach (var patient in patientList)
						{
							cachedPatients.Add(patient.Id, patient);
						}

						patientListAvailableCallback(cachedPatients.Values.ToList());
					},
					errorCallback
				);
			}
			else
			{
				patientListAvailableCallback(cachedPatients.Values.ToList());
			}
		}

		public void AddPatient(Patient newPatient, Action<string> errorCallback)
		{
			connectionService.AddPatientRequest(newPatient, errorCallback);
		}

		private Patient GetPatient (Guid id, Action<string> errorCallback)
		{			
			if (cachedPatients.ContainsKey(id))
				return cachedPatients[id];

			errorCallback("patient not found");
			return null;
		}

		protected override void CleanUp()
		{
			connectionService.NewPatientAvailable     -= OnNewPatientAvailable;
			connectionService.UpdatedPatientAvailable -= OnUpdatedPatientAvailable;
		}
	}
}