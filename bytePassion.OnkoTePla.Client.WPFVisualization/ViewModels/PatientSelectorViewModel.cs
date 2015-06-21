﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Helper;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Interfaces;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels
{
	internal class PatientSelectorViewModel : IPatientSelectorViewModel, INotifyPropertyChanged
	{
	    private readonly IReadOnlyList<Appointment> appointments;
	    private string filterstring;
		private readonly IPatientReadRepository patients;
		private bool isListEmpty;
        private PatientListItem selectedPatient;

		public PatientSelectorViewModel(IPatientReadRepository patients)
		{
		    this.patients = patients;
			filterstring = "";

			IsListEmpty = Patients.Count == 0;
		}

		public IReadOnlyList<PatientListItem> Patients
		{
			get { return PatientListItem.ConvertPatientList(patients.GetAllPatients().ToList()); }
		}

	    public IReadOnlyList<Appointment> Appointments
	    {
            get
            {
                if (selectedPatient != null)
                    return appointments.Where(a => a.Patient.Id == SelectedPatient.Patient.Id).ToList();
                return null;
            }
	    }

	    private static bool IsMatch(string patientName, string filterString)
		{
			var seperatedPatientNames = patientName.Split(' ');

			return seperatedPatientNames.Any(namepart => namepart.ToLower().StartsWith(filterString));
		}

		public bool IsListEmpty
		{
            get { return isListEmpty; }
            private set { PropertyChanged.ChangeAndNotify(this, ref isListEmpty, value); }
			}

        public PatientListItem SelectedPatient
			{
            get { return selectedPatient; }
            set { PropertyChanged.ChangeAndNotify(this, ref selectedPatient, value); PropertyChanged.Notify(this, "ObservableAppointments");}
		}	

		public string FilterString
		{
			set
			{
				var trimmedValue = value.Trim();

				if (String.Equals(filterstring, trimmedValue, StringComparison.CurrentCultureIgnoreCase)) return;
				filterstring = value.ToLower();

				if (String.IsNullOrEmpty(filterstring))
				{
					foreach (var patientListItem in Patients)
						patientListItem.IsCurrentVisibleInList = true;
				}

				foreach (var patientListItem in Patients)
					patientListItem.IsCurrentVisibleInList = IsMatch(patientListItem.Patient.Name, filterstring);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}
