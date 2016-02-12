﻿using System;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients
{
	public interface IPatientWriteRepository : IPersistable
	{
		void AddPatient(string name, Date birthday, bool alive, string externalId);

		void SetNewName(Guid patientId, string newName);
		void SetNewBirthday(Guid patientId, Date newBirthday);
		void SetLivingStatus(Guid patientId, bool newLivingStatus);
	}
}