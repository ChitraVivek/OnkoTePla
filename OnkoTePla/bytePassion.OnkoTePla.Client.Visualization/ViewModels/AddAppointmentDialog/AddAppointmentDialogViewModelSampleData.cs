using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AddAppointmentDialog.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Patients;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AddAppointmentDialog
{
	internal class AddAppointmentDialogViewModelSampleData : IAddAppointmentDialogViewModel
	{
		public AddAppointmentDialogViewModelSampleData()
		{
			PatientSelectorViewModel = new PatientSelectorViewModelSampleData();

			SelectedPatient = new Patient("John Doh", new Date(12,4,1978), true, new Guid(), "externalID", false);

			DurationHours = 2;
			DurationMinutes = 15;

			CreationState = AppointmentCreationState.NoPatientSelected;

			AllAvailablesLabels = new ObservableCollection<Label>
			{
				new Label("label1", Colors.Aqua,       Guid.NewGuid()),
				new Label("label2", Colors.BurlyWood,  Guid.NewGuid()),
				new Label("label3", Colors.Chartreuse, Guid.NewGuid()),
				new Label("label4", Colors.Gold,       Guid.NewGuid())
			};

			SelectedLabel = AllAvailablesLabels[2];
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand CloseDialog        { get; } = null;
		public ICommand CreateAppointment  { get; } = null;
		
		public ICommand HourPlusOne        { get; } = null;
		public ICommand HourMinusOne       { get; } = null;
		public ICommand MinutePlusFifteen  { get; } = null;
		public ICommand MinuteMinusFifteen { get; } = null;

		public byte DurationHours   { get; }
		public byte DurationMinutes { get; }

	    public ObservableCollection<Label> AllAvailablesLabels { get; }
	    public Label SelectedLabel { get; set; }

	    public AppointmentCreationState CreationState { get; }

		public Patient SelectedPatient { get; }
		public string  Description     { set {} }

		public void Dispose() { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}