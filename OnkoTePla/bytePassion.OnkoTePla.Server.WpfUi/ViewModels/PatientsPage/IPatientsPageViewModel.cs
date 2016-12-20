using System.Windows.Input;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientSelector;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientsPage
{
	internal interface IPatientsPageViewModel : IViewModel
	{
		IPatientSelectorViewModel PatientSelectorViewModel { get; }

		ICommand Generate100RandomPatients   { get; }		

		bool   IsPatientSelected { get; }		
		bool   IsPatientAlive    { get; }
		string PatientName       { get; }		
		string PatientInternalId { get; }
		string PatientExternalId { get; }
		Date   PatientBirthday   { get; }				
	}
}