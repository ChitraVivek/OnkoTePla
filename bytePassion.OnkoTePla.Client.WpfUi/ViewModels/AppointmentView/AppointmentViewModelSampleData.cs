﻿using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.WpfUi.Adorner;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AppointmentView.Helper;
using System;
using System.ComponentModel;
using System.Windows.Input;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AppointmentView
{
    public class AppointmentViewModelSampleData : IAppointmentViewModel
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

			OperatingMode = OperatingMode.Edit;
			ShowDisabledOverlay = false;
			Identifier = new Guid();
		}

		public ICommand DeleteAppointment => null;
		public ICommand SwitchToEditMode  => null;

		public Time   BeginTime     { get; }
		public Time   EndTime       { get; }		

		public string PatientDisplayName { get; }
		public string TimeSpan           { get; }
		public string AppointmentDate    { get; }
		public string Description        { get; }
		public string Room               { get; }

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
