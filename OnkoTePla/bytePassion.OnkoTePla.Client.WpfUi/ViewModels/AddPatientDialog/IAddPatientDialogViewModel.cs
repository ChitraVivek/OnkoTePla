using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddPatientDialog
{
	internal interface IAddPatientDialogViewModel : IViewModel
	{
		ICommand CreatePatient { get; }
		ICommand Cancel        { get; }

		string Name     { get; set; }
		string Birthday { get; set; }

		bool Result { get; }
	}
}