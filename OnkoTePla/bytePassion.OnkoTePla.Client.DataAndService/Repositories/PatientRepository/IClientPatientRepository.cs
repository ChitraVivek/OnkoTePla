using System;
using System.Collections.Generic;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository
{
	public interface IClientPatientRepository : IDisposable
	{
		event Action<Patient> NewPatientAvailable;
		event Action<Patient> UpdatedPatientAvailable;
					
		void RequestPatient(Action<Patient> patientAvailableCallback, Guid patientId, Action<string> errorCallback);
		void RequestAllPatientList(Action<IReadOnlyList<Patient>> patientListAvailableCallback, Action<string> errorCallback);

		void AddPatient(Patient newPatient, Action<string> errorCallback);
	}
}
