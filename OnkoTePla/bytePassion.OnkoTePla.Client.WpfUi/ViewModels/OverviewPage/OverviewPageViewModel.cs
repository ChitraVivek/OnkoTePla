using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Utils;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.WindowBuilder;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.DateDisplay;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.DateSelector;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.GridContainer;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.MedicalPracticeSelector;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.RoomSelector;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.UndoRedoView;
using bytePassion.OnkoTePla.Contracts.Domain;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.OverviewPage
{
	internal class OverviewPageViewModel : ViewModel, IOverviewPageViewModel
	{
        private readonly ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable;
		private readonly ISharedStateReadOnly<Guid> selectedMedicalPracticeIdVariable;
		private readonly ISharedStateReadOnly<Date> selectedDayVariable;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly Action<string> errorCallback;
		 
	    private bool changeConfirmationVisible;
		private bool addAppointmentPossible;		

		public OverviewPageViewModel(IViewModelCommunication viewModelCommunication,
                                     IDateDisplayViewModel dateDisplayViewModel,
									 IMedicalPracticeSelectorViewModel medicalPracticeSelectorViewModel, 
									 IRoomFilterViewModel roomFilterViewModel, 
									 IDateSelectorViewModel dateSelectorViewModel, 
									 IGridContainerViewModel gridContainerViewModel, 									
									 IUndoRedoViewModel undoRedoViewModel,									 
                                     IWindowBuilder<Views.AddAppointmentDialog> addAppointmentDialogBuilder,
									 PrintDialogWindowBuilder printDialogBuilder,
									 ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable,
									 ISharedStateReadOnly<Guid> selectedMedicalPracticeIdVariable,
									 ISharedStateReadOnly<Date> selectedDayVariable,
									 IClientMedicalPracticeRepository medicalPracticeRepository,
									 Action<string> errorCallback)
		{
		    this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.selectedMedicalPracticeIdVariable = selectedMedicalPracticeIdVariable;
			this.selectedDayVariable = selectedDayVariable;
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.errorCallback = errorCallback;
			DateDisplayViewModel = dateDisplayViewModel;
			MedicalPracticeSelectorViewModel = medicalPracticeSelectorViewModel;
			RoomFilterViewModel = roomFilterViewModel;
			DateSelectorViewModel = dateSelectorViewModel;
			GridContainerViewModel = gridContainerViewModel;		
			UndoRedoViewModel = undoRedoViewModel;

			ChangeConfirmationVisible = false;
			AddAppointmentPossible = false;			

			
            appointmentModificationsVariable.StateChanged  += OnCurrentModifiedAppointmentVariableChanged;
			selectedMedicalPracticeIdVariable.StateChanged += OnSelectedMedicalPracticeIdVariableChanged;
			selectedDayVariable.StateChanged               += OnSelectedDayVariablChanged;			

			ShowAddAppointmentDialog = new Command(() =>
			{
				viewModelCommunication.Send(new ShowDisabledOverlay());

				var dialogWindow = addAppointmentDialogBuilder.BuildWindow();
				dialogWindow.ShowDialog();
				//addAppointmentDialogBuilder.DisposeWindow(dialogWindow);

                viewModelCommunication.Send(new HideDisabledOverlay());                
            });

			ShowPrintDialog = new Command(() =>
			{
				viewModelCommunication.Send(new ShowDisabledOverlay());

				var dialogWindow = printDialogBuilder.BuildWindow(new AggregateIdentifier(selectedDayVariable.Value, 
																						  selectedMedicalPracticeIdVariable.Value));
				dialogWindow.ShowDialog();
				//printDialogBuilder.DisposeWindow(dialogWindow);
				
				viewModelCommunication.Send(new HideDisabledOverlay());
			});

			UpdateAddAppointmentPossible();
		}

		private void OnSelectedDayVariablChanged(Date date)
		{
			UpdateAddAppointmentPossible();
		}

		private void OnSelectedMedicalPracticeIdVariableChanged(Guid guid)
		{
			UpdateAddAppointmentPossible();
		}

		private void OnCurrentModifiedAppointmentVariableChanged(AppointmentModifications appointmentModifications)
		{
			ChangeConfirmationVisible = appointmentModifications != null;

			UpdateAddAppointmentPossible();
		}

		private void UpdateAddAppointmentPossible()
		{
			if (appointmentModificationsVariable.Value != null)
			{
				AddAppointmentPossible = false;
				return;
			}

			var day = selectedDayVariable.Value;

			medicalPracticeRepository.RequestMedicalPractice(
				practice =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						AddAppointmentPossible = practice.HoursOfOpening.IsOpen(day);
					});					
				},
				selectedMedicalPracticeIdVariable.Value,
				day,
				errorCallback	
			);
		}

		public IDateDisplayViewModel             DateDisplayViewModel             { get; }
		public IMedicalPracticeSelectorViewModel MedicalPracticeSelectorViewModel { get; }
		public IRoomFilterViewModel              RoomFilterViewModel              { get; }
		public IDateSelectorViewModel            DateSelectorViewModel            { get; }
		public IGridContainerViewModel           GridContainerViewModel           { get; }		
		public IUndoRedoViewModel                UndoRedoViewModel                { get; }

		public ICommand ShowAddAppointmentDialog { get; }
		public ICommand ShowPrintDialog { get; }

		public bool ChangeConfirmationVisible
		{
			get { return changeConfirmationVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref changeConfirmationVisible, value); }
		}

		public bool AddAppointmentPossible
		{
			get { return addAppointmentPossible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref addAppointmentPossible, value); }
		}		
		
        protected override void CleanUp()
        {
            appointmentModificationsVariable.StateChanged  -= OnCurrentModifiedAppointmentVariableChanged;
			selectedMedicalPracticeIdVariable.StateChanged -= OnSelectedMedicalPracticeIdVariableChanged;
			selectedDayVariable.StateChanged               -= OnSelectedDayVariablChanged;
		}
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
