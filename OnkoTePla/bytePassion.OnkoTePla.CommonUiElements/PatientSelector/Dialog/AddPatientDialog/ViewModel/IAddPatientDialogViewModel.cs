using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;

namespace bytePassion.OnkoTePla.CommonUiElements.PatientSelector.Dialog.AddPatientDialog.ViewModel
{
	internal interface IAddPatientDialogViewModel : IViewModel
	{
		ICommand CreatePatient { get; }
		ICommand Cancel        { get; }

		string Name     { get; set; }
		string Id       { get; set; }
		string Birthday { get; set; }

		bool Result { get; }
	}
}