using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage
{
	internal interface ISearchPageViewModel : IViewModel
	{
		ICommand DeleteAppointment { get; }
		ICommand ModifyAppointment { get; }		
		
		IPatientSelectorViewModel PatientSelectorViewModel { get; }

		bool ShowPreviousAppointments { get; set; }
		bool NoAppointmentsAvailable { get; }
		string SelectedPatient { get; }
		
		ObservableCollection<DisplayAppointmentData> DisplayedAppointments { get; }
	}
}
