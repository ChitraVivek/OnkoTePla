using System;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.AppointmentViewModel;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddAppointmentDialog;
using bytePassion.OnkoTePla.Client.WpfUi.Views;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Client.WpfUi.Factorys.WindowBuilder
{
	internal class AddAppointmentDialogWindowBuilder : IWindowBuilder<AddAppointmentDialog>
	{
		private readonly IClientPatientRepository patientRepository;
		private readonly IClientReadModelRepository readModelRepository;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientLabelRepository labelRepository;
		private readonly ISharedStateReadOnly<Guid> selectedMedicalPractiveVariable;       
        private readonly ISharedStateReadOnly<Date> selectedDateVariable;	    	   
	    private readonly IAppointmentViewModelBuilder appointmentViewModelBuilder;
		private readonly Action<string> errorCallback;

		public AddAppointmentDialogWindowBuilder(IClientPatientRepository patientRepository,
												 IClientReadModelRepository readModelRepository,
												 IClientMedicalPracticeRepository medicalPracticeRepository,
												 IClientLabelRepository labelRepository,
												 ISharedStateReadOnly<Guid> selectedMedicalPractiveVariable,                                                 
                                                 ISharedStateReadOnly<Date> selectedDateVariable, 												
												 IAppointmentViewModelBuilder appointmentViewModelBuilder,
												 Action<string> errorCallback)
		{
		    this.patientRepository = patientRepository;
		    this.readModelRepository = readModelRepository;
		    this.medicalPracticeRepository = medicalPracticeRepository;
			this.labelRepository = labelRepository;
			this.selectedMedicalPractiveVariable = selectedMedicalPractiveVariable;            
            this.selectedDateVariable = selectedDateVariable;	        
		    this.appointmentViewModelBuilder = appointmentViewModelBuilder;
			this.errorCallback = errorCallback;
		}

		public AddAppointmentDialog BuildWindow()
		{

            var selectedPatientVariable = new SharedState<Patient>();
		    						
			IPatientSelectorViewModel patientSelectorViewModel = new PatientSelectorViewModel(patientRepository, 
																							  selectedPatientVariable, 
																							  errorCallback);

			return new AddAppointmentDialog
			       {
						Owner = Application.Current.MainWindow,
						DataContext = new AddAppointmentDialogViewModel(medicalPracticeRepository,
																		readModelRepository,
																		labelRepository,
                                                                        patientSelectorViewModel, 																		
                                                                        selectedPatientVariable,
                                                                        selectedDateVariable.Value,                                                                         																		
																		selectedMedicalPractiveVariable.Value,
																		appointmentViewModelBuilder,
																		errorCallback)
			       };
		}

		public void DisposeWindow(AddAppointmentDialog buildedWindow)
		{
			throw new NotImplementedException();
		}
	}
}
