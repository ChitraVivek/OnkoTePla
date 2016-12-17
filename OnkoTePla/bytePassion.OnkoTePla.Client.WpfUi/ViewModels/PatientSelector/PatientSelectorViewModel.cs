using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.Commands.Updater;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository;
using bytePassion.OnkoTePla.Client.WpfUi.DialogServices;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.PatientSelector
{
	internal class PatientSelectorViewModel : ViewModel, 
                                              IPatientSelectorViewModel
    {
		private class PatientSorter : IComparer<Patient>
		{
			public int Compare (Patient p1, Patient p2)
			{
				return string.Compare(p1.Name, p2.Name, StringComparison.Ordinal);
			}
		}
		

		private readonly IClientPatientRepository patientRepository;
		private readonly ISharedState<Patient> selectedPatientSharedVariable;
		private readonly Action<string> errorCallback;
		private bool listIsEmpty;
        private string searchFilter;

        private Patient selectedPatient;
        private bool showDeceasedPatients;

	    private readonly ObservableCollection<Patient> observablePatientList;

        public PatientSelectorViewModel(IClientPatientRepository patientRepository, 
									    ISharedState<Patient> selectedPatientSharedVariable,
										Action<string> errorCallback)
        {
	        this.patientRepository = patientRepository;
	        this.selectedPatientSharedVariable = selectedPatientSharedVariable;
	        this.errorCallback = errorCallback;

			observablePatientList = new ObservableCollection<Patient>();

	        Patients = new CollectionViewSource
	        {
		        Source = observablePatientList
			};
	        Patients.Filter += Filter;			
			SearchFilter = "";						

			patientRepository.NewPatientAvailable     += OnNewPatientAvailable;
	        patientRepository.UpdatedPatientAvailable += OnUpdatedPatientAvailable;

			patientRepository.RequestAllPatientList(
				patientList =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						var sortedList = patientList.OrderBy(p => p, new PatientSorter());
						
						observablePatientList.Clear();
						sortedList.Do(observablePatientList.Add);						
						UpdateForNewInput();
					});					
				},
				errorCallback	
			);   
			
			CreatePatient = new Command(DoCreatePatient,
										() => !string.IsNullOrWhiteSpace(SearchFilter),
										new PropertyChangedCommandUpdater(this, nameof(SearchFilter)));                    			            
        }

	    private void DoCreatePatient()
	    {
		    var newPatient = AddPatientDialogService.GetNewPatient(SearchFilter);

		    if (newPatient != null)
		    {
			    OnNewPatientAvailable(newPatient);
			    SearchFilter = newPatient.Name;

			    if (SelectedPatient == null)
			    {
				    SelectedPatient = newPatient;
			    }

				patientRepository.AddPatient(newPatient, errorCallback);
		    }
	    }

	    private void OnUpdatedPatientAvailable(Patient patient)
	    {
		    Application.Current.Dispatcher.Invoke(() =>
		    {
			    var patientToRemove = observablePatientList.FirstOrDefault(p => p.Id == patient.Id);

				if (patientToRemove != null)
					observablePatientList.Remove(patientToRemove);

				observablePatientList.Add(patient);
				observablePatientList.Sort(new PatientSorter());
				UpdateForNewInput();
			});
	    }

	    private void OnNewPatientAvailable(Patient patient)
	    {
			if (observablePatientList.FirstOrDefault(p => p.Id == patient.Id) != null)
				return;

			Application.Current.Dispatcher.Invoke(() =>
			{				
				observablePatientList.Add(patient);
				observablePatientList.Sort(new PatientSorter());
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

	    public ICommand CreatePatient { get; }

	    public string SearchFilter
        {
            get { return searchFilter; }
            set
            {
                PropertyChanged.ChangeAndNotify(this, ref searchFilter, value);             
                UpdateForNewInput();
            }
        }

	    private void UpdateForNewInput()
	    {
		    if (Patients.View != null)
		    {
			    SelectedPatient = null;
			    Patients.View.Refresh();
			    CheckList();
		    }
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
				SelectedPatient = (Patient)((ListCollectionView)Patients.View).GetItemAt(0);

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

			patientRepository.NewPatientAvailable     -= OnNewPatientAvailable;
			patientRepository.UpdatedPatientAvailable -= OnUpdatedPatientAvailable;

			((Command)CreatePatient).Dispose();
		}
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}