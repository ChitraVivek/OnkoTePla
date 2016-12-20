using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientSelector;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientsPage
{
	internal class PatientsPageViewModelSampleData : IPatientsPageViewModel
	{

		public PatientsPageViewModelSampleData()
		{
			PatientSelectorViewModel = new PatientSelectorViewModelSampleData();

			IsPatientSelected = true;
			IsPatientAlive = false;

			PatientName = "John Doe";
			PatientBirthday = new Date(1,1,2000);
			PatientExternalId = "exampleExternal-ID";
			PatientInternalId = "exampleInternal-ID";
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand Generate100RandomPatients   => null;		

		public bool   IsPatientSelected { get; }
		public bool   IsPatientAlive    { get; }
		public string PatientName       { get; }		
		public string PatientInternalId { get; }
		public string PatientExternalId { get; }
		public Date   PatientBirthday   { get; }		

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}