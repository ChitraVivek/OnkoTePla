using System.ComponentModel;
using bytePassion.Lib.TimeLib;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView
{
	internal class PrintAppointmentViewModelSampleData : IPrintAppointmentViewModel
	{
		public PrintAppointmentViewModelSampleData(Time beginTime, Time endTime, string patientDisplayName)
		{
			BeginTime = beginTime;
			EndTime = endTime;
			PatientDisplayName = patientDisplayName;
		}

		public PrintAppointmentViewModelSampleData()
		{
			BeginTime = new Time(8,30);
			EndTime = new Time(10,0);
			PatientDisplayName = "John Doe";
		}

		public Time BeginTime { get; }
		public Time EndTime   { get; }

		public string PatientDisplayName { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;				
	}
}