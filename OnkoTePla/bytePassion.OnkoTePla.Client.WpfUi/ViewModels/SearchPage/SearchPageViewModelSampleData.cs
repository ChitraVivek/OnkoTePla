using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.SearchPage.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Appointments;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.SearchPage
{
	internal class SearchPageViewModelSampleData : ISearchPageViewModel
	{
		public SearchPageViewModelSampleData()
		{
			PatientSelectorViewModel = new PatientSelectorViewModelSampleData();

			DisplayedAppointments = new ObservableCollection<DisplayAppointmentData>
			{
				new DisplayAppointmentData(new AppointmentTransferData(Guid.Empty, "testApp1", new Date(21,10,2015), 
																	   new Time(10,30), new Time(12,45), Guid.Empty,
																	   Guid.Empty, Guid.Empty, Guid.Empty), "Fürth"),

				new DisplayAppointmentData(new AppointmentTransferData(Guid.Empty, "testApp2", new Date(22,10,2015),
																	   new Time(11,30), new Time(13,45), Guid.Empty,
																	   Guid.Empty, Guid.Empty, Guid.Empty), "Fürth"),

				new DisplayAppointmentData(new AppointmentTransferData(Guid.Empty, "testApp3", new Date(23,10,2015),
																	   new Time(12,30), new Time(14,45), Guid.Empty,
																	   Guid.Empty, Guid.Empty, Guid.Empty), "Fürth")
			};

			SelectedPatient = "John Doe";
			ShowPreviousAppointments = true;
			NoAppointmentsAvailable = true;
		}

		public ICommand DeleteAppointment { get; } = null;
		public ICommand ModifyAppointment { get; } = null;		

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public bool ShowPreviousAppointments { get; set; }

		public bool NoAppointmentsAvailable { get; }
		public string SelectedPatient { get; }

		public ObservableCollection<DisplayAppointmentData> DisplayedAppointments { get; }
		
		public void Dispose() { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
