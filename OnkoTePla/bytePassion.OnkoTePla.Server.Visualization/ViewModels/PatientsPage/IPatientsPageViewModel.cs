using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage
{
	public interface IPatientsPageViewModel : IViewModel
	{
		IPatientSelectorViewModel PatientSelectorViewModel { get; }

		ICommand Generate100RandomPatients   { get; }		

		ICommand SaveChanges { get; }
		ICommand DiscardChanges { get; }

		bool   IsPatientSelected { get; }		
		bool   IsPatientAlive    { get; set; }
		string PatientName       { get; set; }
		string PatientBirthday   { get; set; }
		string PatientInternalId { get; }
		string PatientExternalId { get; }	
		bool   IsPatientHidden   { get; set; }	
	}
}