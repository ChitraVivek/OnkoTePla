using System;
using System.Text;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Contracts.Types;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class TryToAddNewPatientRequest : NetworkMessageBase
	{
		public TryToAddNewPatientRequest (ConnectionSessionId sessionId, Guid userId, Patient newPatient)
			: base(NetworkMessageType.TryToAddNewPatientRequest)
		{
			SessionId = sessionId;
			UserId = userId;
			NewPatient = newPatient;
		}

		public ConnectionSessionId SessionId { get; }
		public Guid UserId { get; }
		public Patient NewPatient { get; }

		public override string AsString ()
		{
			var sb = new StringBuilder();

			sb.Append(SessionId);             sb.Append(';');
			sb.Append(UserId);                sb.Append(';');
			sb.Append(NewPatient.Name);       sb.Append(';');
			sb.Append(NewPatient.Alive);      sb.Append(';');
			sb.Append(NewPatient.Birthday);   sb.Append(';');
			sb.Append(NewPatient.Id);         sb.Append(';');
			sb.Append(NewPatient.ExternalId);

			return sb.ToString();
		}

		public static TryToAddNewPatientRequest Parse (string s)
		{
			var parts = s.Split(';');

			var sessionId = new ConnectionSessionId(Guid.Parse(parts[0]));
			var userId    =                         Guid.Parse(parts[1]);

			var name       =            parts[2];
			var alive      = bool.Parse(parts[3]);
			var birthday   = Date.Parse(parts[4]);
			var id         = Guid.Parse(parts[5]);
			var externalId =            parts[6];

			return new TryToAddNewPatientRequest(sessionId, userId, new Patient(name, birthday, alive, id,externalId));
		}
	}
}