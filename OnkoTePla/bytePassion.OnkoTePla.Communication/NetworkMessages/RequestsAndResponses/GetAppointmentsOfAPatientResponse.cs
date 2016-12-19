using System;
using System.Collections.Generic;
using System.Text;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class GetAppointmentsOfAPatientResponse : NetworkMessageBase
	{
		public GetAppointmentsOfAPatientResponse (IReadOnlyList<AppointmentTransferData> appointmentCollection)
			: base(NetworkMessageType.GetAppointmentsOfAPatientResponse)
		{			
			AppointmentList = appointmentCollection;			
		}
				
		public IReadOnlyList<AppointmentTransferData> AppointmentList  { get; }

		public override string AsString()
		{
			var sb = new StringBuilder();			

			foreach (var appointmentTransferData in AppointmentList)
			{
				sb.Append(appointmentTransferData.PatientId);         sb.Append("#");
				sb.Append(appointmentTransferData.Description);       sb.Append("#");
				sb.Append(appointmentTransferData.Day);               sb.Append("#");
				sb.Append(appointmentTransferData.StartTime);         sb.Append("#");
				sb.Append(appointmentTransferData.EndTime);           sb.Append("#");
				sb.Append(appointmentTransferData.TherapyPlaceId);    sb.Append("#");
				sb.Append(appointmentTransferData.LabelId);			  sb.Append("#");
				sb.Append(appointmentTransferData.Id);			      sb.Append("#");
				sb.Append(appointmentTransferData.MedicalPracticeId);

				sb.Append(";");
			}

			if (AppointmentList.Count > 0)
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		public static GetAppointmentsOfAPatientResponse Parse (string s)
		{					
			var appointmentList = new List<AppointmentTransferData>();

			if (!string.IsNullOrWhiteSpace(s))
			{
				var appointmentListParts = s.Split(';');

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

			return new GetAppointmentsOfAPatientResponse(appointmentList);
		}
	}
}