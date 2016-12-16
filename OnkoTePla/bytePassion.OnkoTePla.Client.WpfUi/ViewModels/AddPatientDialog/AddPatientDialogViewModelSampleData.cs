using System.ComponentModel;
using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddPatientDialog
{
	internal class AddPatientDialogViewModelSampleData : IAddPatientDialogViewModel
	{
		public AddPatientDialogViewModelSampleData()
		{
			Name = "blubb blubb";
			Birthday = "1.2.16";
			Result = false;
		}

		public ICommand CreatePatient => null;
		public ICommand Cancel        => null;

		public string Name     { get; set; }
		public string Birthday { get; set; }

		public bool Result { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;				
	}
}