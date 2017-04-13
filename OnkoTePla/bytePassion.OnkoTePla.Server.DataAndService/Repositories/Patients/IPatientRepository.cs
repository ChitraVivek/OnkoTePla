using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients
{
	public interface IPatientRepository : IPatientDataBaseInteraction, IPersistable
	{		
		Patient GetPatientById(Guid id);
		IEnumerable<Patient> GetAllPatients();

		void AddPatient (string name, Date birthday, bool alive, string externalId);
		bool AddPatient (Patient newPatient);

		void SetNewName (Guid patientId, string newName);
		void SetNewBirthday (Guid patientId, Date newBirthday);
		void SetLivingStatus (Guid patientId, bool newLivingStatus);
		void UpdatePatient(Guid patientId, string newName, Date newBirthDay, bool newLivingStatus, bool newIsHiddenStatus);
	}
	
}
