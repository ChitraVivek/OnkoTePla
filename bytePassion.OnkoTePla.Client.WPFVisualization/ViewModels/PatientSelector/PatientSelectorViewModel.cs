﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Client.WPFVisualization.Model;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.PatientSelector.Helper;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.SearchPage;
using bytePassion.OnkoTePla.Contracts.Appointments;


namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.PatientSelector
{
    internal class PatientSelectorViewModel :ISearchPageViewModel
    {
        //private readonly IReadOnlyList<Appointment> appointments;
        //private readonly IPatientReadRepository dataCenter;
        private PatientListItem selectedPatient;
        private string searchFilter;

        public PatientSelectorViewModel(IDataCenter dataCenter)
        {
            Patients = new CollectionViewSource();
            Patients.Filter += Filter;
            Patients.Source = PatientListItem.ConvertPatientList(dataCenter.Patients.GetAllPatients().ToList());
        }

        public CollectionViewSource Patients { get; set; }

        public string SearchFilter
        {
            get { return searchFilter; }
            set
            {
                searchFilter = value;


                Patients.View.Refresh();
            }
        }

        public IReadOnlyList<Appointment> Appointments
        {
            get { return null; }
        }

        public PatientListItem SelectedPatient
        {
            get { return selectedPatient; }
            set
            {
                PropertyChanged.ChangeAndNotify(this, ref selectedPatient, value);
                PropertyChanged.Notify(this, "ObservableAppointments");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Filter(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as PatientListItem;
            if (string.IsNullOrEmpty(SearchFilter))
                e.Accepted = true;
            else if (!src.Patient.Name.ToLower().Contains(SearchFilter))
                e.Accepted = false;
        }
    }
}