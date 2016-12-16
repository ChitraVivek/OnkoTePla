using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.Commands.Updater;

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddPatientDialog
{
	internal class AddPatientDialogViewModel : ViewModel, IAddPatientDialogViewModel
	{
		private string name;
		private string birthday;

		public AddPatientDialogViewModel(string initialName)
		{
			CreatePatient = new Command(DoCreatePatient,
										IsPatientCreationPossible,
										new PropertyChangedCommandUpdater(this, nameof(Name), nameof(Birthday)));
			Cancel = new Command(DoCancel);

			Name = initialName;
		}

		private bool IsPatientCreationPossible()
		{
			// TODO: Test name
			// TODO: Test birthday

			return true;
		}

		private void DoCancel()
		{
			Result = false;
			CloseDialog();
		}

		private void DoCreatePatient()
		{
			Result = true;
			CloseDialog();
		}

		private void CloseDialog()
		{
			Application.Current.Windows
							   .OfType<Views.AddPatientDialog>()
							   .FirstOrDefault(window => ReferenceEquals(window.DataContext, this))
							   ?.Close();
		}

		public ICommand CreatePatient { get; }
		public ICommand Cancel { get; }

		public string Name
		{
			get { return name; }
			set { PropertyChanged.ChangeAndNotify(this, ref name, value); }
		}

		public string Birthday
		{
			get { return birthday; }
			set { PropertyChanged.ChangeAndNotify(this, ref birthday, value); }
		}

		public bool Result { get; private set; }

		protected override void CleanUp()
		{
			((Command)CreatePatient).Dispose();	
		}

		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
