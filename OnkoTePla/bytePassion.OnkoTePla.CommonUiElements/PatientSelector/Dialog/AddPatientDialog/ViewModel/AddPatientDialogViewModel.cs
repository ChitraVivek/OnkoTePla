using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.Commands.Updater;

namespace bytePassion.OnkoTePla.CommonUiElements.PatientSelector.Dialog.AddPatientDialog.ViewModel
{
	internal class AddPatientDialogViewModel : Lib.WpfLib.ViewModelBase.ViewModel, IAddPatientDialogViewModel
	{
		private string name;
		private string birthday;
		private string id;

		public AddPatientDialogViewModel(string initialName)
		{
			CreatePatient = new Command(DoCreatePatient,
										IsPatientCreationPossible,
										new PropertyChangedCommandUpdater(this, nameof(Name), nameof(Birthday)));
			Cancel = new Command(DoCancel);

			Name = initialName;
			Birthday = string.Empty;
		}

		private bool IsPatientCreationPossible()
		{
			return !string.IsNullOrWhiteSpace(Name) && 
				   Date.IsValidDateString(Birthday);
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
							   .OfType<AddPatientDialog>()
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

		public string Id
		{
			get { return id; }
			set { PropertyChanged.ChangeAndNotify(this, ref id, value); }
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
