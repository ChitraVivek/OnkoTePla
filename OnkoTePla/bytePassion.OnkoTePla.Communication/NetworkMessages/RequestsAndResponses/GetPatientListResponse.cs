using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class GetPatientListResponse : NetworkMessageBase
	{
		public GetPatientListResponse(IReadOnlyList<Patient> patients)
			: base(NetworkMessageType.GetPatientListResponse)
		{
			Patients = patients;
		}

		public IReadOnlyList<Patient> Patients { get; } 

		public override string AsString()
		{
			var sb = new StringBuilder();

			foreach (var patient in Patients)
			{
				sb.Append(patient.Name);       sb.Append(';');
				sb.Append(patient.Alive);      sb.Append(';');
				sb.Append(patient.Birthday);   sb.Append(';');
				sb.Append(patient.Id);		   sb.Append(';');
				sb.Append(patient.ExternalId); sb.Append(';');
				sb.Append(patient.IsHidden);

				sb.Append('|');
			}

			if (Patients.Count > 0)
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		public static GetPatientListResponse Parse(string s)
		{
			var patients = (
				from patientParts 

				in s.Split('|')
					.Where(part => !string.IsNullOrWhiteSpace(part))
					.Select(patientData => patientData.Split(';'))

				let name       =            patientParts[0]
				let alive      = bool.Parse(patientParts[1])
				let birthday   = Date.Parse(patientParts[2])
				let id         = Guid.Parse(patientParts[3])
				let externalId =            patientParts[4]
				let isHiddden  = bool.Parse(patientParts[5])

				select new Patient(name, birthday, alive, id, externalId, isHiddden)
			).ToList();

			return new GetPatientListResponse(patients);
		}
	}
}