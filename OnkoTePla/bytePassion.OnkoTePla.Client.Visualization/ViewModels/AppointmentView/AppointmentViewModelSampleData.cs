using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView
{
	internal class AppointmentViewModelSampleData : IAppointmentViewModel
	{
		public AppointmentViewModelSampleData()
			: this(new Time(9,0), new Time(11,0))
		{			
		}

		public AppointmentViewModelSampleData(Time beginTime, Time endTime)
		{
			PatientDisplayName = "Jerry Black";
			TimeSpan = "10.00-13.00";
			AppointmentDate = "2.November 2015";
			Description = "test";
			Room = "A12";

			BeginTime = beginTime;
			EndTime = endTime;

			LabelColor = Colors.SteelBlue;
			LabelName = "testLabel";

			OperatingMode = OperatingMode.Edit;
			ShowDisabledOverlay = false;
			Identifier = new Guid();
		}

		public ICommand DeleteAppointment => null;
		public ICommand SwitchToEditMode  => null;
        public ICommand EditDescription   => null;
	    public ICommand ConfirmChanges    => null;
		public ICommand RejectChanges     => null;

		public Time   BeginTime     { get; }
		public Time   EndTime       { get; }

		public Color LabelColor { get; }

		public string PatientDisplayName { get; }
		public string TimeSpan           { get; }
		public string AppointmentDate    { get; }
		public string Description        { get; }
		public string Room               { get; }
		public string LabelName          { get; }

		public AppointmentModifications CurrentAppointmentModifications { get; } = null;
		public AdornerControl AdornerControl { get; } = null;

		public OperatingMode OperatingMode  { get; set; }
		public bool ShowDisabledOverlay { get; }

		public Guid Identifier { get; }

		public void Process (Dispose               message) { }		
		public void Process (RestoreOriginalValues message) { }
		public void Process (ShowDisabledOverlay   message) { }
		public void Process (HideDisabledOverlay   message) { }
		public void Process (SwitchToEditMode      message) { }

		public void Dispose() {}

		public IViewModelCommunication ViewModelCommunication { get; } = null;

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}
