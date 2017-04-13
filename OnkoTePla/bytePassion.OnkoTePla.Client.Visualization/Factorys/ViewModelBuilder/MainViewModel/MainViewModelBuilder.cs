using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.AppointmentModification;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentGridViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.TherapyPlaceRowViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessageHandler;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateDisplay;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.GridContainer;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.UndoRedoView;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.MainViewModel
{
	internal class MainViewModelBuilder : IMainViewModelBuilder
    {
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientReadModelRepository readModelRepository;
		private readonly IClientPatientRepository patientRepository;
		private readonly IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository;
		private readonly IClientLabelRepository labelRepository;
		private readonly ICommandService commandService;
		private readonly ILocalSettingsRepository localSettingsRepository;
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly ISession session;		
        private readonly AdornerControl adornerControl;

		private ConfirmChangesMessageHandler confirmChangesMessageHandler;
		private RejectChangesMessageHandler rejectChangesMessageHandler;

		private SharedState<Size> gridSizeVariable; 

		public MainViewModelBuilder(IClientMedicalPracticeRepository medicalPracticeRepository,
									IClientReadModelRepository readModelRepository,
									IClientPatientRepository patientRepository,		
									IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository,
									IClientLabelRepository labelRepository,							
									ICommandService commandService,
									ILocalSettingsRepository localSettingsRepository,
                                    IViewModelCommunication viewModelCommunication,
									ISession session,                                   
                                    AdornerControl adornerControl)
        {
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.readModelRepository = readModelRepository;
			this.patientRepository = patientRepository;
			this.therapyPlaceTypeRepository = therapyPlaceTypeRepository;
			this.labelRepository = labelRepository;
			this.commandService = commandService;
			this.localSettingsRepository = localSettingsRepository;
			this.viewModelCommunication = viewModelCommunication;
	        this.session = session;	        
            this.adornerControl = adornerControl;
        }



        public IMainViewModel Build(Action<string> errorCallback, Size initialSize = null) 
        {            
            // Register Global ViewModelVariables

	        var firstDispayedDate = TimeTools.Today();	// TODO find last open

	        var lastUsedMedicalPracticeId = localSettingsRepository.LastUsedMedicalPracticeId;

	        if (lastUsedMedicalPracticeId == Guid.Empty)
	        {
		        lastUsedMedicalPracticeId = session.LoggedInUser.ListOfAccessablePractices.First();
	        }
	       			    
                gridSizeVariable                  = new SharedState<Size>(initialSize ?? new Size(new Width(400), new Height(400)));
            var selectedDateVariable              = new SharedState<Date>(firstDispayedDate);    
            var selectedMedicalPracticeIdVariable = new SharedState<Guid>(lastUsedMedicalPracticeId);
            var roomFilterVariable                = new SharedState<Guid?>();
            var appointmentModificationsVariable  = new SharedState<AppointmentModifications>();

            var selectedPatientForAppointmentSearchVariable = new SharedState<Patient>();


            // Create ViewModelCollection

            viewModelCommunication.CreateViewModelCollection<ITherapyPlaceRowViewModel, TherapyPlaceRowIdentifier>(
                Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection
            );

            viewModelCommunication.CreateViewModelCollection<IAppointmentGridViewModel, AggregateIdentifier>(
                Constants.ViewModelCollections.AppointmentGridViewModelCollection
            );

            viewModelCommunication.CreateViewModelCollection<ITimeGridViewModel, AggregateIdentifier>(
                Constants.ViewModelCollections.TimeGridViewModelCollection
            );

            viewModelCommunication.CreateViewModelCollection<IAppointmentViewModel, Guid>(
                Constants.ViewModelCollections.AppointmentViewModelCollection
            );            

            // build factorys

            var appointmentModificationsBuilder = new AppointmentModificationsBuilder(medicalPracticeRepository, 
																					  readModelRepository,
                                                                                      viewModelCommunication,
                                                                                      selectedDateVariable,
                                                                                      gridSizeVariable);

            var appointmentViewModelBuilder = new AppointmentViewModelBuilder(viewModelCommunication,
																			  labelRepository,
																			  commandService,
                                                                              appointmentModificationsVariable,
                                                                              selectedDateVariable,
                                                                              adornerControl,
                                                                              appointmentModificationsBuilder);

            var therapyPlaceRowViewModelBuilder = new TherapyPlaceRowViewModelBuilder(viewModelCommunication,																					  
                                                                                      medicalPracticeRepository,
																					  therapyPlaceTypeRepository,
																					  adornerControl,
                                                                                      appointmentModificationsVariable,
																					  gridSizeVariable);

            var appointmentGridViewModelBuilder = new AppointmentGridViewModelBuilder(medicalPracticeRepository,
																					  readModelRepository,
                                                                                      viewModelCommunication,
                                                                                      gridSizeVariable,
                                                                                      roomFilterVariable,
																					  selectedMedicalPracticeIdVariable,
																					  appointmentModificationsVariable,
																					  appointmentViewModelBuilder,
                                                                                      therapyPlaceRowViewModelBuilder);
			
            var addAppointmentDialogWindowBuilder = new AddAppointmentDialogWindowBuilder(patientRepository,
																						  readModelRepository,
																						  medicalPracticeRepository,
																						  labelRepository,
                                                                                          selectedMedicalPracticeIdVariable,
                                                                                          selectedDateVariable,
                                                                                          appointmentViewModelBuilder,
																						  errorCallback);

			var printDialogWindowBuilder = new PrintDialogWindowBuilder(medicalPracticeRepository,
																		readModelRepository, 
																		errorCallback);

            // build stand-alone viewModelMessageHandler

	        confirmChangesMessageHandler = new ConfirmChangesMessageHandler(viewModelCommunication,
																			commandService,
																		    appointmentModificationsVariable,
																			errorCallback);

	        rejectChangesMessageHandler = new RejectChangesMessageHandler(viewModelCommunication,
																		  appointmentModificationsVariable);
			

            // build factories



            // create permanent ViewModels

            var dateDisplayViewModel = new DateDisplayViewModel(selectedDateVariable);

            var medicalPracticeSelectorViewModel = new MedicalPracticeSelectorViewModel(session, 
																						medicalPracticeRepository,
																						localSettingsRepository,
                                                                                        selectedMedicalPracticeIdVariable,
                                                                                        appointmentModificationsVariable, 
																						errorCallback);
			
            var roomSelectorViewModel = new RoomFilterViewModel(medicalPracticeRepository,
                                                                roomFilterVariable,
                                                                selectedDateVariable,
                                                                selectedMedicalPracticeIdVariable,
																appointmentModificationsVariable,
																errorCallback);

            var dateSelectorViewModel = new DateSelectorViewModel(selectedDateVariable);

            var gridContainerViewModel = new GridContainerViewModel(viewModelCommunication,
																	medicalPracticeRepository,
                                                                    selectedDateVariable,
                                                                    selectedMedicalPracticeIdVariable,
                                                                    gridSizeVariable,
                                                                    new List<AggregateIdentifier>(),
                                                                    50,
                                                                    appointmentGridViewModelBuilder,
																	errorCallback);
            
            var undoRedoViewModel = new UndoRedoViewModel(viewModelCommunication,
                                                          appointmentModificationsVariable,
                                                          session,
														  errorCallback);

            var overviewPageViewModel = new OverviewPageViewModel(viewModelCommunication,
                                                                  dateDisplayViewModel,
                                                                  medicalPracticeSelectorViewModel,
                                                                  roomSelectorViewModel,
                                                                  dateSelectorViewModel,
                                                                  gridContainerViewModel,                                                                
                                                                  undoRedoViewModel,
                                                                  addAppointmentDialogWindowBuilder,
																  printDialogWindowBuilder,
                                                                  appointmentModificationsVariable,
																  selectedMedicalPracticeIdVariable,
																  selectedDateVariable,
																  medicalPracticeRepository,
																  errorCallback);

            var patientSelectorViewModel = new PatientSelectorViewModel(patientRepository,
                                                                        selectedPatientForAppointmentSearchVariable,
																		errorCallback);

            var searchPageViewModel = new SearchPageViewModel(patientSelectorViewModel,
                                                              selectedPatientForAppointmentSearchVariable,
                                                              selectedDateVariable,                                                              
                                                              viewModelCommunication,
															  commandService,
                                                              readModelRepository,	
															  medicalPracticeRepository,														 
															  errorCallback);

            var optionsPageViewModel = new OptionsPageViewModel();            

            var mainViewModel = new ViewModels.MainView.MainViewModel(viewModelCommunication,
																	  overviewPageViewModel,
                                                                      searchPageViewModel,
                                                                      optionsPageViewModel,
																	  appointmentModificationsVariable);
			
			viewModelCommunication.RegisterViewModelMessageHandler<AsureDayIsLoaded>(gridContainerViewModel);
            viewModelCommunication.RegisterViewModelMessageHandler<ShowPage>(mainViewModel);
			viewModelCommunication.RegisterViewModelMessageHandler(confirmChangesMessageHandler);
			viewModelCommunication.RegisterViewModelMessageHandler(rejectChangesMessageHandler);

			return mainViewModel;
        }

        public void DisposeViewModel(IMainViewModel viewModelToDispose)
        {
			viewModelCommunication.RemoveViewModelCollection(Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection);
			viewModelCommunication.RemoveViewModelCollection(Constants.ViewModelCollections.AppointmentGridViewModelCollection);
			viewModelCommunication.RemoveViewModelCollection(Constants.ViewModelCollections.TimeGridViewModelCollection);
			viewModelCommunication.RemoveViewModelCollection(Constants.ViewModelCollections.AppointmentViewModelCollection);
			
			var optionsPageViewModel             = viewModelToDispose.OptionsPageViewModel;
	        var searchPageViewModel              = viewModelToDispose.SearchPageViewModel;
	        var overviewPageViewModel            = viewModelToDispose.OverviewPageViewModel;
	        var patientSelectorViewModel         = searchPageViewModel.PatientSelectorViewModel;
	        var dateDisplayViewModel             = overviewPageViewModel.DateDisplayViewModel;
	        var medicalPracticeSelectorViewModel = overviewPageViewModel.MedicalPracticeSelectorViewModel;
	        var roomSelectorViewModel            = overviewPageViewModel.RoomFilterViewModel;
	        var dateSelectorViewModel            = overviewPageViewModel.DateSelectorViewModel;
	        var gridContainerViewModel           = overviewPageViewModel.GridContainerViewModel;	        
	        var undoRedoViewModel                = overviewPageViewModel.UndoRedoViewModel;

			viewModelCommunication.DeregisterViewModelMessageHandler<AsureDayIsLoaded>(gridContainerViewModel);
			viewModelCommunication.DeregisterViewModelMessageHandler<ShowPage>(viewModelToDispose);
			viewModelCommunication.DeregisterViewModelMessageHandler(confirmChangesMessageHandler);
			viewModelCommunication.DeregisterViewModelMessageHandler(rejectChangesMessageHandler);

			optionsPageViewModel.Dispose();
			searchPageViewModel.Dispose();
			overviewPageViewModel.Dispose();
			patientSelectorViewModel.Dispose();
			dateDisplayViewModel.Dispose();
			medicalPracticeSelectorViewModel.Dispose();
			roomSelectorViewModel.Dispose();
			dateSelectorViewModel.Dispose();
			gridContainerViewModel.Dispose();			
			undoRedoViewModel.Dispose();
		}

		public Size GetCurrentGridSize()
		{
			return gridSizeVariable?.Value;
		}
    }
}
