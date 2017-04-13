using System;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository
{
	public interface IClientPatientRepository : IPatientDataBaseInteraction, IDisposable
	{
		void RequestPatient(Action<Patient> patientAvailableCallback, Guid patientId, Action<string> errorCallback);		
	}
}
