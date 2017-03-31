using System;
using System.Windows;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddPatientDialog;
using bytePassion.OnkoTePla.Client.WpfUi.Views;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.WpfUi.DialogServices
{
	internal static class AddPatientDialogService
	{
		public static Patient GetNewPatient(string initialName)
		{
			var addPatientDialogViewModel = new AddPatientDialogViewModel(initialName);

			var dialog = new AddPatientDialog
			{
				Owner = Application.Current.MainWindow,
				DataContext = addPatientDialogViewModel
			};

			dialog.ShowDialog();

			if (addPatientDialogViewModel.Result)
			{
				return new Patient(addPatientDialogViewModel.Name,
								   Date.Parse(addPatientDialogViewModel.Birthday.Trim()),
								   true,
								   Guid.NewGuid(),
								   addPatientDialogViewModel.Id,
								   false);
			}
			else
			{
				return null;
			}			
		}
	}
}
