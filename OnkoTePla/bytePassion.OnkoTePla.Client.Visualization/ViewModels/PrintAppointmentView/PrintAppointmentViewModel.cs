using System.ComponentModel;
using bytePassion.Lib.TimeLib;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView
{
	internal class PrintAppointmentViewModel : ViewModel, IPrintAppointmentViewModel
	{
		public PrintAppointmentViewModel(Time beginTime, Time endTime, string patientDisplayName)
		{
			BeginTime = beginTime;
			EndTime = endTime;
			PatientDisplayName = patientDisplayName;
		}

		public Time BeginTime { get; }
		public Time EndTime   { get; }

		public string PatientDisplayName { get; }

		protected override void CleanUp() {}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
