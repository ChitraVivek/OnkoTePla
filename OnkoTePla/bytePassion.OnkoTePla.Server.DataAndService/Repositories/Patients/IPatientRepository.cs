using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients
{
	public interface IPatientRepository : IPersistable
	{
		event Action<Patient> PatientAdded;
		event Action<Patient> PatientModified;

		Patient GetPatientById(Guid id);
		IEnumerable<Patient> GetAllPatients();


		void AddPatient (string name, Date birthday, bool alive, string externalId);
		bool AddPatient (Patient newPatient);

		void SetNewName (Guid patientId, string newName);
		void SetNewBirthday (Guid patientId, Date newBirthday);
		void SetLivingStatus (Guid patientId, bool newLivingStatus);
	}
	
}
