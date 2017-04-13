using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;
using bytePassion.OnkoTePla.Server.Visualization.SampleDataGenerators;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage
{
	internal class PatientsPageViewModel : ViewModel, IPatientsPageViewModel
	{
		private readonly IPatientRepository patientRepository;
		private readonly ISharedState<Patient> selectedPatientVariable;
		private readonly PatientNameGenerator patientNameGenerator;		

		private bool isPatientSelected;
		private bool isPatientAlive;
		private string patientName;
		private string patientInternalId;
		private string patientExternalId;
		private string patientBirthday;
		private bool isPatientHidden;

		public PatientsPageViewModel(IPatientSelectorViewModel patientSelectorViewModel,
									 IPatientRepository patientRepository,
									 ISharedState<Patient> selectedPatientVariable,
									 PatientNameGenerator patientNameGenerator)
		{
			this.patientRepository = patientRepository;
			this.selectedPatientVariable = selectedPatientVariable;
			this.patientNameGenerator = patientNameGenerator;			
			PatientSelectorViewModel = patientSelectorViewModel;
			
			selectedPatientVariable.StateChanged += OnSelectedPatientChanged;
			OnSelectedPatientChanged(selectedPatientVariable.Value);

			Generate100RandomPatients = new Command(DoGeneratePatients);		
			
			SaveChanges = new Command(DoSaveChanges);
			DiscardChanges = new Command(DoDiscardChanges);	
		}

		private void DoDiscardChanges()
		{
			selectedPatientVariable.Value = null;
		}

		private async void DoSaveChanges()
		{
			if (!Date.IsValidDateString(PatientBirthday))
			{
				var dialog = new UserDialogBox("Error",
											   $"{PatientBirthday} ist kein gültiges Datum.\nFormat: dd.mm.yyyy",
											   MessageBoxButton.OK);

				await dialog.ShowMahAppsDialog();
				return;
			}			

			if (string.IsNullOrWhiteSpace(PatientName))
			{
				var dialog = new UserDialogBox("Error",
											   "Der Patientenname darf nicht leer bleiben",
											   MessageBoxButton.OK);

				await dialog.ShowMahAppsDialog();
				return;
			}

			patientRepository.UpdatePatient(selectedPatientVariable.Value.Id,
											PatientName,
											Date.Parse(PatientBirthday),
											IsPatientAlive,
											IsPatientHidden);
			selectedPatientVariable.Value = null;
		}

		private void DoGeneratePatients()
		{
			for (int i = 0; i < 100; i++)
			{
				var newPatient = patientNameGenerator.NewPatient();
				patientRepository.AddPatient(newPatient.Name, 
											 newPatient.Birthday, 
											 newPatient.Alive, 
											 newPatient.ExternalId);
			}

			MessageBox.Show("100 Patent was generated");
		}

		private void OnSelectedPatientChanged(Patient patient)
		{
			if (patient == null)
			{
				IsPatientSelected = false;
				IsPatientAlive    = false;
				PatientBirthday   = "";
				PatientName       = "";
				PatientExternalId = "";
				PatientInternalId = "";
				IsPatientHidden   = false;
			}
			else
			{
				IsPatientSelected = true;
				IsPatientAlive    = patient.Alive;
				PatientBirthday   = patient.Birthday.ToString();
				PatientName       = patient.Name;
				PatientExternalId = patient.ExternalId;
				PatientInternalId = patient.Id.ToString();
				IsPatientHidden   = patient.IsHidden;
			}
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand Generate100RandomPatients { get; }

		public ICommand SaveChanges { get; }
		public ICommand DiscardChanges { get; }

		public bool IsPatientSelected
		{
			get { return isPatientSelected; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isPatientSelected, value); }
		}
		public bool IsPatientAlive
		{
			get { return isPatientAlive; }
			set { PropertyChanged.ChangeAndNotify(this, ref isPatientAlive, value); }
		}
		public string PatientName
		{
			get { return patientName; }
			set { PropertyChanged.ChangeAndNotify(this, ref patientName, value); }
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

		public bool IsPatientHidden
		{
			get { return isPatientHidden; }
			set { PropertyChanged.ChangeAndNotify(this, ref isPatientHidden, value); }
		}

		public string PatientBirthday
		{
			get { return patientBirthday; }
			set { PropertyChanged.ChangeAndNotify(this, ref patientBirthday, value); }
		}

		protected override void CleanUp()
		{
			selectedPatientVariable.StateChanged -= OnSelectedPatientChanged;
		}
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
