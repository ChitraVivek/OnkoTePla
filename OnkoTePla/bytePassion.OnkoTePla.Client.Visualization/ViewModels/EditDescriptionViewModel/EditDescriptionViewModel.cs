using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.Views;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.EditDescriptionViewModel
{
	internal class EditDescriptionViewModel : ViewModel, IEditDescriptionViewModel
	{        	
        private readonly ISharedState<AppointmentModifications> modificationsVar;

		private Label selectedLabel;
		private string description;

		public EditDescriptionViewModel(Appointment appointmentToEdit, 
										IClientLabelRepository labelRepository,										
										ISharedState<AppointmentModifications> modificationsVar,
										Action<string> errorCallback)
        {	        
            this.modificationsVar = modificationsVar;  
			          
            Description = appointmentToEdit.Description;
			AllAvailablesLabels = new ObservableCollection<Label>();

            Cancel = new Command(CloseWindow);
            Accept = new Command(SaveAndClose);

			labelRepository.RequestAllLabels(
				labelList =>
				{
					labelList.Do(AllAvailablesLabels.Add);
					SelectedLabel = labelList.First(label => label.Id == appointmentToEdit.Label.Id);
				},
				errorCallback	
			);
        }
		
        private void SaveAndClose()
        {
            modificationsVar.Value.SetNewDescription(Description);

			if (SelectedLabel != null)
				modificationsVar.Value.SetNewLabel(SelectedLabel);

            CloseWindow();

        }

        public ICommand Cancel { get; }
        public ICommand Accept { get; }

        public string Description
        {
            get { return description; }
            set { PropertyChanged.ChangeAndNotify(this, ref description, value); }
        }

		public ObservableCollection<Label> AllAvailablesLabels { get; }

		public Label SelectedLabel
		{
			get { return selectedLabel; }
			set { PropertyChanged.ChangeAndNotify(this, ref selectedLabel, value); }
		}

		private static void CloseWindow()
        {
            var windows = Application.Current.Windows
                                             .OfType<EditDescription>()
                                             .ToList();

            if (windows.Count == 1)
                windows[0].Close();
            else
                throw new Exception("inner error");
        }

		protected override void CleanUp() {}		
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
