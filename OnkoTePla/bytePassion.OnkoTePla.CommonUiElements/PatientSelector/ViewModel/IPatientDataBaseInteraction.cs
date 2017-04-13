using System;
using System.Collections.Generic;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel
{
	public interface IPatientDataBaseInteraction
	{
		event Action<Patient> NewPatientAvailable;
		event Action<Patient> UpdatedPatientAvailable;
		
		void RequestAllPatientList (Action<IReadOnlyList<Patient>> patientListAvailableCallback, Action<string> errorCallback);

		void AddPatient (Patient newPatient, Action<string> errorCallback);
	}
}