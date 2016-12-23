﻿using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientSelector;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.PatientsPage
{
	internal interface IPatientsPageViewModel : IViewModel
	{
		IPatientSelectorViewModel PatientSelectorViewModel { get; }

		ICommand Generate100RandomPatients   { get; }		

		ICommand SaveChanges { get; }
		ICommand DiscardChanges { get; }

		bool   IsPatientSelected { get; }		
		bool   IsPatientAlive    { get; set; }
		string PatientName       { get; set; }
		string PatientBirthday   { get; set; }
		string PatientInternalId { get; }
		string PatientExternalId { get; }	
		bool   IsPatientHidden   { get; set; }	
	}
}