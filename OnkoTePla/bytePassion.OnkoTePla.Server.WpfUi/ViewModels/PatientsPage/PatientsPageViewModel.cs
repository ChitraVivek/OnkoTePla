using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;
using bytePassion.OnkoTePla.Server.WpfUi.SampleDataGenerators;
using bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientSelector;


namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientsPage
{
	internal class PatientsPageViewModel : ViewModel, IPatientsPageViewModel
	{
		private readonly IPatientRepository patientRepository;
		private readonly ISharedStateReadOnly<Patient> selectedPatientVariable;
		private readonly PatientNameGenerator patientNameGenerator;		

		private bool isPatientSelected;
		private bool isPatientAlive;
		private string patientName;
		private string patientInternalId;
		private string patientExternalId;
		private Date patientBirthday;

		public PatientsPageViewModel(IPatientSelectorViewModel patientSelectorViewModel,
									 IPatientRepository patientRepository,
									 ISharedStateReadOnly<Patient> selectedPatientVariable,
									 PatientNameGenerator patientNameGenerator)
		{
			this.patientRepository = patientRepository;
			this.selectedPatientVariable = selectedPatientVariable;
			this.patientNameGenerator = patientNameGenerator;			
			PatientSelectorViewModel = patientSelectorViewModel;
			
			selectedPatientVariable.StateChanged += OnSelectedPatientChanged;
			OnSelectedPatientChanged(selectedPatientVariable.Value);

			Generate1000RandomPatients = new Command(DoGeneratePatients);			
		}
		
		private void DoGeneratePatients()
		{
			for (int i = 0; i < 1; i++)
			{
				var newPatient = patientNameGenerator.NewPatient();
				patientRepository.AddPatient(newPatient.Name, 
											 newPatient.Birthday, 
											 newPatient.Alive, 
											 newPatient.ExternalId);
			}

			MessageBox.Show("1 Patent was generated");
		}

		private void OnSelectedPatientChanged(Patient patient)
		{
			if (patient == null)
			{
				IsPatientSelected = false;
				IsPatientAlive = false;
				PatientBirthday = new Date(1,1,2000);
				PatientName = "";
				PatientExternalId = "";
				PatientInternalId = "";
			}
			else
			{
				IsPatientSelected = true;
				IsPatientAlive    = patient.Alive;
				PatientBirthday   = patient.Birthday;
				PatientName       = patient.Name;
				PatientExternalId = patient.ExternalId;
				PatientInternalId = patient.Id.ToString();
			}
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand Generate1000RandomPatients { get; }		

		public bool IsPatientSelected
		{
			get { return isPatientSelected; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isPatientSelected, value); }
		}
		public bool IsPatientAlive
		{
			get { return isPatientAlive; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isPatientAlive, value); }
		}
		public string PatientName
		{
			get { return patientName; }
			private set { PropertyChanged.ChangeAndNotify(this, ref patientName, value); }
		}
		public string PatientInternalId
		{
			get { return patientInternalId; }
			private set { PropertyChanged.ChangeAndNotify(this, ref patientInternalId, value); }
		}
		public string PatientExternalId
		{
			get { return patientExternalId; }
			private set { PropertyChanged.ChangeAndNotify(this, ref patientExternalId, value); }
		}
		public Date PatientBirthday
		{
			get { return patientBirthday; }
			private set { PropertyChanged.ChangeAndNotify(this, ref patientBirthday, value); }
		}

		protected override void CleanUp()
		{
			selectedPatientVariable.StateChanged -= OnSelectedPatientChanged;
		}
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
