using System;
using System.Text;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Contracts.Types;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.Notifications
{
	public class PatientAddedNotification : NetworkMessageBase
	{
		public PatientAddedNotification(Patient patient, ConnectionSessionId sessionId)
			: base(NetworkMessageType.PatientAddedNotification)
		{
			Patient = patient;
			SessionId = sessionId;
		}

		public Patient             Patient   { get; }
		public ConnectionSessionId SessionId { get; }

		public override string AsString()
		{
			var sb = new StringBuilder();

			sb.Append(SessionId);

			sb.Append('|');

			sb.Append(Patient.Name);       sb.Append(';');
			sb.Append(Patient.Alive);      sb.Append(';');
			sb.Append(Patient.Birthday);   sb.Append(';');
			sb.Append(Patient.Id);         sb.Append(';');
			sb.Append(Patient.ExternalId); sb.Append(';');
			sb.Append(Patient.IsHidden);

			return sb.ToString();
		}

		public static PatientAddedNotification Parse(string s)
		{
			var index = s.IndexOf("|", StringComparison.Ordinal);

			var sessionId = new ConnectionSessionId(Guid.Parse(s.Substring(0, index)));

			var patientData = s.Substring(index + 1, s.Length - index - 1)
							   .Split(';');

			var name       =            patientData[0];
			var alive      = bool.Parse(patientData[1]);
			var birthday   = Date.Parse(patientData[2]);
			var id         = Guid.Parse(patientData[3]);
			var externalId =            patientData[4];
			var isHidden   = bool.Parse(patientData[5]);

			return new PatientAddedNotification(new Patient(name, birthday, alive, id, externalId, isHidden), sessionId);
		}
	}
}
