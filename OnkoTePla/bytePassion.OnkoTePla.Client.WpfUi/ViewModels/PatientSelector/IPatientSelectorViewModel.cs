using System.Windows.Data;
using System.Windows.Input;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.PatientSelector
{
	public interface IPatientSelectorViewModel : IViewModel
	{
		CollectionViewSource Patients { get; }

		ICommand CreatePatient { get; }

		string SearchFilter { set; }
        Patient SelectedPatient { set; }

		bool ListIsEmpty { get; }
		bool ShowDeceasedPatients { get; set; }
    }
}
