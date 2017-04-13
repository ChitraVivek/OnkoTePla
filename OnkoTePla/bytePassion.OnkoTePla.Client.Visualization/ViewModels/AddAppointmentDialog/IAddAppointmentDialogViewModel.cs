using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AddAppointmentDialog.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AddAppointmentDialog
{
	internal interface IAddAppointmentDialogViewModel : IViewModel
	{
		IPatientSelectorViewModel PatientSelectorViewModel { get; }

		ICommand CloseDialog { get; }
		ICommand CreateAppointment { get; }

		ICommand HourPlusOne        { get; }
		ICommand HourMinusOne       { get; }
		ICommand MinutePlusFifteen  { get; }
		ICommand MinuteMinusFifteen { get; }

		byte DurationHours   { get; }
		byte DurationMinutes { get; }

		ObservableCollection<Label> AllAvailablesLabels { get; }
		Label SelectedLabel { get; set; }

		AppointmentCreationState CreationState { get; }

		Patient SelectedPatient { get; }
		string Description { set; }
	}
}