using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel
{
	public class PatientSelectorViewModelSampleData : IPatientSelectorViewModel 
	{
		public PatientSelectorViewModelSampleData()
		{			
			var patientList = new List<Patient>
			                  {
				                  new Patient("John Do", new Date(1,2,1945), true , new Guid(), "", false),
								  new Patient("John Do", new Date(1,2,1945), false, new Guid(), "", false),
								  new Patient("John Do", new Date(1,2,1945), false, new Guid(), "", false),
								  new Patient("John Do", new Date(1,2,1945), true , new Guid(), "", false)								 
							  };

			Patients = new CollectionViewSource
			           {
				           Source = patientList
			           };

			SelectedPatient = patientList[0];
			ListIsEmpty = false;
			ShowDeceasedPatients = true;
			SearchFilter = "John Do";
		}

		public ICommand CreatePatient => null;

		public string  SearchFilter         { get; set; }
		public Patient SelectedPatient      { get; set; }
		public bool    ListIsEmpty          { get; }
		public bool    ShowDeceasedPatients { get; set; }

		public CollectionViewSource Patients { get; }
		
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
