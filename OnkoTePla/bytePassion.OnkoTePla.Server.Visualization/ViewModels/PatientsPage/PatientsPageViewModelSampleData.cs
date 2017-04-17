using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage
{
	internal class PatientsPageViewModelSampleData : IPatientsPageViewModel
	{

		public PatientsPageViewModelSampleData()
		{
			PatientSelectorViewModel = new PatientSelectorViewModelSampleData();

			IsPatientSelected = true;
			IsPatientAlive = false;
			IsPatientHidden = true;

			PatientName = "John Doe";
			PatientBirthday = new Date(1,1,2000).ToString();
			PatientExternalId = "exampleExternal-ID";
			PatientInternalId = "exampleInternal-ID";
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand Generate100RandomPatients => null;

		public ICommand SaveChanges    => null;
		public ICommand DiscardChanges => null;

		public bool   IsPatientSelected { get; }
		public bool   IsPatientAlive    { get; set; }
		public string PatientName       { get; set; }		
		public string PatientInternalId { get; }
		public string PatientExternalId { get; set; }
		public bool IsPatientHidden     { get; set; }
		public string PatientBirthday   { get; set; }		

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}