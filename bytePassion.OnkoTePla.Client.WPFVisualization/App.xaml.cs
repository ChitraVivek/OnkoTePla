﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using bytePassion.Lib.Communication.MessageBus;
using bytePassion.Lib.Communication.MessageBus.HandlerCollection;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Core.CommandSystem;
using bytePassion.OnkoTePla.Client.Core.Domain;
using bytePassion.OnkoTePla.Client.Core.Domain.CommandHandler;
using bytePassion.OnkoTePla.Client.Core.Eventsystem;
using bytePassion.OnkoTePla.Client.Core.Repositories;
using bytePassion.OnkoTePla.Client.Core.Repositories.Aggregate;
using bytePassion.OnkoTePla.Client.Core.Repositories.Config;
using bytePassion.OnkoTePla.Client.Core.Repositories.EventStore;
using bytePassion.OnkoTePla.Client.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Client.Core.Repositories.Readmodel;
using bytePassion.OnkoTePla.Client.Resources;
using bytePassion.OnkoTePla.Client.WPFVisualization.Global;
using bytePassion.OnkoTePla.Client.WPFVisualization.SessionInfo;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AddAppointmentTestViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AppointmentGridViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AppointmentOverViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Base;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.DateSelectorViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.MainWindowViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.MedicalPracticeSelectorViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.PatientSelectorViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.RoomSelectorViewModel;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Client.WPFVisualization
{

	public partial class App : Application
	{
		protected override void OnStartup (StartupEventArgs e)
		{
			base.OnStartup(e);

			///////////////////////////////////////////////////////////////////////////////////////////////
			////////                                                                             //////////
			////////                          Composition Root and Setup                         //////////
			////////                                                                             //////////
			///////////////////////////////////////////////////////////////////////////////////////////////


			// Patient-Repository

			IPersistenceService<IEnumerable<Patient>> patientPersistenceService = new JSonPatientDataStore(GlobalConstants.PatientJsonPersistenceFile);
			IPatientReadRepository patientRepository = new PatientRepository(patientPersistenceService);
			patientRepository.LoadRepository();


			// Config-Repository

			IPersistenceService<Configuration> configPersistenceService = new XmlConfigurationDataStore(GlobalConstants.ConfigPersistenceFile);
			IConfigurationReadRepository configReadRepository = new ConfigurationRepository(configPersistenceService);
			configReadRepository.LoadRepository();


			// EventStore

			IPersistenceService<IEnumerable<EventStream<AggregateIdentifier>>> eventStorePersistenceService = new XmlEventStreamDataStore(GlobalConstants.EventHistoryPersistenceFile);
			IEventStore eventStore = new EventStore(eventStorePersistenceService, configReadRepository);
			eventStore.LoadRepository(); 


			// Event- and CommandBus

			IHandlerCollection<DomainEvent>   eventHandlerCollection   = new MultiHandlerCollection <DomainEvent>();
			IHandlerCollection<DomainCommand> commandHandlerCollection = new SingleHandlerCollection<DomainCommand>();

			IMessageBus<DomainEvent>   eventMessageBus   = new LocalMessageBus<DomainEvent>  (eventHandlerCollection);			
			IMessageBus<DomainCommand> commandMessageBus = new LocalMessageBus<DomainCommand>(commandHandlerCollection);

			IEventBus   eventBus   = new EventBus(eventMessageBus);
			ICommandBus commandBus = new CommandBus(commandMessageBus);


			// Aggregate- and Readmodel-Repositories

			IAggregateRepository aggregateRepository = new AggregateRepository(eventBus, eventStore, patientRepository, configReadRepository);
			IReadModelRepository readModelRepository = new ReadModelRepository(eventBus, eventStore, patientRepository, configReadRepository);


			// Register CommandHandler

			commandBus.RegisterCommandHandler(new AddAppointmentCommandHandler(aggregateRepository));
			commandBus.RegisterCommandHandler(new DeleteAppointmentCommandHandler(aggregateRepository));


			// SessionInformation

			var sessionInformation = new SessionInformation
			{
				LoggedInUser = configReadRepository.GetAllUsers().First()
			};

			// GlobalStates registration

			IHandlerCollection<ViewModelMessageBase> handlerCollection = new MultiHandlerCollection<ViewModelMessageBase>();
			IMessageBus<ViewModelMessageBase> viewModelMessageBus = new LocalMessageBus<ViewModelMessageBase>(handlerCollection);
			IStateEngine viewModelStateEngine = new StateEngine();

			GlobalAccess.ViewModelCommunication = new ViewModelCommunication<ViewModelMessageBase>(viewModelMessageBus,
																								   viewModelStateEngine);
			GlobalVariables.RegisterAllGlobalVariables();













			IStateEngine stateEngine = new StateEngine();

			var initialMedicalPractice = configReadRepository.GetAllMedicalPractices().First();  // TODO set last usage
			
			var        selectedDateInitialValue      = initialMedicalPractice.HoursOfOpening.GetLastOpenDayFromToday();
			var        displayedPracticeInitialValue = new Tuple<Guid, uint>(initialMedicalPractice.Id, initialMedicalPractice.Version); // TODO kann gefährlich sein ,wenn der letzte tag zu einer anderen config gehört
			Guid?      selectedRoomInitialValue      = null;
			const bool sideBarStateInitialValue      = true;

			stateEngine.RegisterState(GlobalConstants.GlobalStateMainGridSelectedDate,      selectedDateInitialValue);			
			stateEngine.RegisterState(GlobalConstants.GlobalStateMainGridDisplayedPractice, displayedPracticeInitialValue);
			stateEngine.RegisterState(GlobalConstants.GlobalStateMainGridSelectedRoom,      selectedRoomInitialValue);	// when selectedRoomID == null --> all rooms are selected
			stateEngine.RegisterState(GlobalConstants.GlobalStateSideBarState,              sideBarStateInitialValue);	// true --> full width; false --> minimized
			// stateEngine.RegisterState<CurrentEditingDomain> ..... // IDEE !!! <<<<


			// create permanent ViewModels

			var addAppointmentTestViewModel      = new AddAppointmentTestViewModel(configReadRepository, patientRepository, readModelRepository, commandBus);
			var appointmentOverViewModel         = new AppointmentOverViewModel(readModelRepository, configReadRepository);
            var patientsViewModel                = new PatientSelectorViewModel(patientRepository);
			var appointmentGridViewModel         = new AppointmentGridViewModel(readModelRepository, configReadRepository, commandBus, 
											     							    sessionInformation, 
											     							    stateEngine.GetState<Date>(GlobalConstants.GlobalStateMainGridSelectedDate),
																				stateEngine.GetState<Tuple<Guid, uint>>(GlobalConstants.GlobalStateMainGridDisplayedPractice),
																			    stateEngine.GetState<Guid?>(GlobalConstants.GlobalStateMainGridSelectedRoom));
			var dateSelectorViewModel            = new DateSelectorViewModel(stateEngine.GetState<Date>(GlobalConstants.GlobalStateMainGridSelectedDate),
																			 stateEngine.GetState<Tuple<Guid, uint>>(GlobalConstants.GlobalStateMainGridDisplayedPractice), 
																			 configReadRepository);
			var medicalPracticeSelectorViewModel = new MedicalPracticeSelectorViewModel(configReadRepository,
																						stateEngine.GetState<Tuple<Guid, uint>>(GlobalConstants.GlobalStateMainGridDisplayedPractice));
			var roomSelectorViewModel            = new RoomSelectorViewModel(stateEngine.GetState<Guid?>(GlobalConstants.GlobalStateMainGridSelectedRoom),
																			 stateEngine.GetState<Tuple<Guid, uint>>(GlobalConstants.GlobalStateMainGridDisplayedPractice),
																			 configReadRepository);

			var mainWindowViewModel = new MainWindowViewModel(patientsViewModel, 
															  addAppointmentTestViewModel, 
															  appointmentOverViewModel, 
															  appointmentGridViewModel,
															  dateSelectorViewModel,
															  medicalPracticeSelectorViewModel,
															  roomSelectorViewModel,
															  sessionInformation);

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};			

			mainWindow.ShowDialog();


			///////////////////////////////////////////////////////////////////////////////////////////////
			////////                                                                             //////////
			////////             Clean Up and store data after main Window was closed            //////////
			////////                                                                             //////////
			///////////////////////////////////////////////////////////////////////////////////////////////


			eventStore.PersistRepository();
		}		
	}
}
