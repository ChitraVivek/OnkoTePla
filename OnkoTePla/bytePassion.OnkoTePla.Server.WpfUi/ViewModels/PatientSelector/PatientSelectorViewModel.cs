using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientSelector
{
	internal class PatientSelectorViewModel : ViewModel, 
                                              IPatientSelectorViewModel
    {
		private readonly IPatientRepository patientRepository;
		private readonly ISharedState<Patient> selectedPatientSharedVariable;

        private bool listIsEmpty;
        private string searchFilter;
        private Patient selectedPatient;
        private bool showDeceasedPatients;

        public PatientSelectorViewModel(IPatientRepository patientRepository, 
									    ISharedState<Patient> selectedPatientSharedVariable)
        {
	        this.patientRepository = patientRepository;
	        this.selectedPatientSharedVariable = selectedPatientSharedVariable;

			selectedPatientSharedVariable.StateChanged += OnSelectedPatientSharedVariableChanged;

			patientRepository.PatientAdded    += PatientRepositoryUpdated;
	        patientRepository.PatientModified += PatientRepositoryUpdated;

			Patients = new CollectionViewSource();
			Patients.Filter += Filter;			
	        Patients.Source = patientRepository.GetAllPatients();

			SearchFilter = "";
		}

	    private void OnSelectedPatientSharedVariableChanged(Patient patient)
	    {
		    SelectedPatient = patient;
	    }

	    private void PatientRepositoryUpdated(Patient patient)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				Patients.Source = patientRepository.GetAllPatients();
				SearchFilter = "";
				UpdateForNewInput();
			});			
		}

		public bool ShowDeceasedPatients
        {
            get { return showDeceasedPatients; }
            set
            {
                PropertyChanged.ChangeAndNotify(this, ref showDeceasedPatients, value);
				UpdateForNewInput();
			}
        }

        public CollectionViewSource Patients { get; }

        public string SearchFilter
        {
            get { return searchFilter; }
            set
            {
                searchFilter = value;                
                UpdateForNewInput();
            }
        }

	    private void UpdateForNewInput()
	    {
			SelectedPatient = null;
			Patients.View.Refresh();
			CheckList();
		}


        public Patient SelectedPatient
        {
            get { return selectedPatient; }
            set
            {
                selectedPatientSharedVariable.Value = value;
                PropertyChanged.ChangeAndNotify(this, ref selectedPatient, value);
            }
        }

        public bool ListIsEmpty
        {
            get { return listIsEmpty; }
            private set { PropertyChanged.ChangeAndNotify(this, ref listIsEmpty, value); }
        }
       
        private void CheckList()
        {
            var count = ((ListCollectionView) Patients.View).Count;

            if (count == 1)
                SelectedPatient = (Patient) ((ListCollectionView) Patients.View).GetItemAt(0);

            ListIsEmpty = count == 0;
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            var patientToCheck = e.Item as Patient;

            var nameCheck = IsPatientNameWithinFilter(patientToCheck, SearchFilter);

	        e.Accepted = nameCheck && (ShowDeceasedPatients || patientToCheck.Alive);
        }

        private static bool IsPatientNameWithinFilter(Patient p, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            if (p.Name.ToLower().Contains(filter.ToLower()))
            {
				return true;
			}

	        return false;
        }

        protected override void CleanUp()
        {
            Patients.Filter -= Filter;

			patientRepository.PatientAdded    += PatientRepositoryUpdated;
			patientRepository.PatientModified += PatientRepositoryUpdated;
		}
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}