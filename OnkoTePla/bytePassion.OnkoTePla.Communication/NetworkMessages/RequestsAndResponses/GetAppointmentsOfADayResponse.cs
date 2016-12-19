using System;
using System.Collections.Generic;
using System.Text;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class GetAppointmentsOfADayResponse : NetworkMessageBase
	{
		public GetAppointmentsOfADayResponse(Guid medicalPracticeId, uint medicalPracticeVersion, 
											 uint aggregateVersion, IReadOnlyList<AppointmentTransferData> appointmentCollection)
			: base(NetworkMessageType.GetAppointmentsOfADayResponse)
		{
			MedicalPracticeId = medicalPracticeId;
			MedicalPracticeVersion = medicalPracticeVersion;
			AggregateVersion = aggregateVersion;
			AppointmentList = appointmentCollection;			
		}

		public Guid	MedicalPracticeId      { get; }
		public uint	MedicalPracticeVersion { get; }
		public uint	AggregateVersion       { get; }
		
		public IReadOnlyList<AppointmentTransferData> AppointmentList  { get; }

		public override string AsString()
		{
			var sb = new StringBuilder();

			sb.Append(MedicalPracticeId);      sb.Append("|");
			sb.Append(MedicalPracticeVersion); sb.Append("|");
			sb.Append(AggregateVersion);       sb.Append("|");

			foreach (var appointmentTransferData in AppointmentList)
			{
				sb.Append(appointmentTransferData.PatientId);      sb.Append("#");
				sb.Append(appointmentTransferData.Description);    sb.Append("#");
				sb.Append(appointmentTransferData.Day);            sb.Append("#");
				sb.Append(appointmentTransferData.StartTime);      sb.Append("#");
				sb.Append(appointmentTransferData.EndTime);        sb.Append("#");
				sb.Append(appointmentTransferData.TherapyPlaceId); sb.Append("#");
				sb.Append(appointmentTransferData.LabelId);		   sb.Append("#");
				sb.Append(appointmentTransferData.Id);			   sb.Append("#");
				sb.Append(MedicalPracticeId);

				sb.Append(";");
			}

			if (AppointmentList.Count > 0)
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		public static GetAppointmentsOfADayResponse Parse(string s)
		{
			var parts = s.Split('|');

			var medicalPracticeId      = Guid.Parse(parts[0]);
			var medicalPracticeVersion = uint.Parse(parts[1]);
			var aggregateVersion       = uint.Parse(parts[2]);
			var appointmentListData    =            parts[3];
						
			var appointmentList = new List<AppointmentTransferData>();

			if (!string.IsNullOrWhiteSpace(appointmentListData))
			{
				var appointmentListParts = appointmentListData.Split(';');

				foreach (var appointmentData in appointmentListParts)
				{
					var appointmentParts = appointmentData.Split('#');

					var patientId      = Guid.Parse(appointmentParts[0]);
					var description    =            appointmentParts[1];
					var day            = Date.Parse(appointmentParts[2]);
					var startTime      = Time.Parse(appointmentParts[3]);
					var endTime        = Time.Parse(appointmentParts[4]);
					var therapyPlaceId = Guid.Parse(appointmentParts[5]);
					var labelId		   = Guid.Parse(appointmentParts[6]);
					var id             = Guid.Parse(appointmentParts[7]);
					var medPracticeId  = Guid.Parse(appointmentParts[8]);

					appointmentList.Add(new AppointmentTransferData(patientId, description, day,
																	startTime, endTime, therapyPlaceId, 
																	id, medPracticeId, labelId));
				}
			}
			return new GetAppointmentsOfADayResponse(medicalPracticeId, medicalPracticeVersion, 
													aggregateVersion, appointmentList);
		}
	}
}