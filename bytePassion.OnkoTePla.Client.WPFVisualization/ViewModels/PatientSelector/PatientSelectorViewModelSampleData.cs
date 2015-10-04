﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Patients;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.PatientSelector
{
	internal class PatientSelectorViewModelSampleData : IPatientSelectorViewModel 
	{
		public PatientSelectorViewModelSampleData()
		{			
			var patientList = new List<Patient>
			                  {
				                  new Patient("John Do", new Date(1,2,1945), true, new Guid(), ""),
								  new Patient("John Do", new Date(1,2,1945), true, new Guid(), ""),
								  new Patient("John Do", new Date(1,2,1945), true, new Guid(), ""),
								  new Patient("John Do", new Date(1,2,1945), true, new Guid(), "")								 
							  };

			Patients = new CollectionViewSource
			           {
				           Source = patientList
			           };

			SelectedPatient = null; //patientList[0];
			ListIsEmpty = false;
		}

		public string SearchFilter { get; set; }
		public Patient SelectedPatient { get; set; }
		public bool ListIsEmpty { get; }
		public CollectionViewSource Patients { get;  }

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}
