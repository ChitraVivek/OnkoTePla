using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage.Helper
{
	internal class DisplayAppointmentData
	{
		public DisplayAppointmentData(AppointmentTransferData appointmentRawData,string medicalPracticeName)
		{
			Description = appointmentRawData.Description;
			Day = appointmentRawData.Day;			
			TimeSpan = $"({appointmentRawData.StartTime.ToStringMinutesAndHoursOnly()} - {appointmentRawData.EndTime.ToStringMinutesAndHoursOnly()})";
            MedicalPracticeName = medicalPracticeName;
			AppointmentRawData = appointmentRawData;
		}

		public AppointmentTransferData AppointmentRawData { get; }

		public string Description         { get; }
		public string TimeSpan            { get; }
		public Date   Day                 { get; }		
		public string MedicalPracticeName { get; }
	}
}
