using System.Diagnostics;
using System.IO;
using System.Windows;
using bytePassion.Lib.Communication.MessageBus;
using bytePassion.Lib.Communication.MessageBus.HandlerCollection;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.DataAndService.Connection;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandHandler;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSystem;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.EventBus;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.UndoRedo.UserActionFactory;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder;
using bytePassion.OnkoTePla.CommonUiElements.DebugOutput;
using bytePassion.OnkoTePla.CommonUiElements.DebugOutput.ViewModel;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Utils;

namespace bytePassion.OnkoTePla.Client.Application
{
	internal class ApplicationLifeCycle : IApplicationLifeCycle
	{
		private ILocalSettingsRepository localSettingsRepository;
		private IConnectionService connectionService;
		
		public void BuildAndStart (StartupEventArgs startupEventArgs)
		{


#if DEBUG
			var listener = new OnkoTePlaDebugListener();

			Debug.Listeners.Add(listener);
#endif

			AssureAppDataDirectoriesExist();

			connectionService = new ConnectionService();
			var eventBus          = new ClientEventBus(connectionService);

			var commandHandlerCollection = new SingleHandlerCollection<DomainCommand>();
			var commandMessageBus = new LocalMessageBus<DomainCommand>(commandHandlerCollection);
			var commandBus = new CommandBus(commandMessageBus);

			var persistenceService = new LocalSettingsXMLPersistenceService(GlobalConstants.LocalSettingsPersistenceFile);
			
			localSettingsRepository = new LocalSettingsRepository(persistenceService);
			localSettingsRepository.LoadRepository();

			var clientMedicalPracticeRepository  = new ClientMedicalPracticeRepository(connectionService);
			var clientPatientRepository          = new ClientPatientRepository(connectionService);
			var clienttherapyPlaceTypeRepository = new ClientTherapyPlaceTypeRepository(connectionService);
			var clientLabelRepository            = new ClientLabelRepository(connectionService);
			var clientReadmodelRepository        = new ClientReadModelRepository(eventBus, clientPatientRepository, clientMedicalPracticeRepository, clientLabelRepository, connectionService);


			var workFlow = new ClientWorkflow();
			var session  = new Session(connectionService, workFlow);

			var fatalErrorHandler = new FatalErrorHandler(session);

			var commandService = new CommandService(session, clientReadmodelRepository, commandBus);


			var userActionBuilder = new UserActionBuilder(commandService);

			// CommandHandler

			var     addAppointmentCommandHandler = new     AddAppointmentCommandHandler(connectionService, session, clientPatientRepository,                                  userActionBuilder, fatalErrorHandler.HandleFatalError);
			var  deleteAppointmentCommandHandler = new  DeleteAppointmentCommandHandler(connectionService, session, clientPatientRepository,                                  userActionBuilder, fatalErrorHandler.HandleFatalError);
			var replaceAppointmentCommandHandler = new ReplaceAppointmentCommandHandler(connectionService, session, clientPatientRepository, clientMedicalPracticeRepository, userActionBuilder, fatalErrorHandler.HandleFatalError);

			commandBus.RegisterCommandHandler(addAppointmentCommandHandler);
			commandBus.RegisterCommandHandler(deleteAppointmentCommandHandler);
			commandBus.RegisterCommandHandler(replaceAppointmentCommandHandler);


			// initiate ViewModelCommunication			

			var handlerCollection = new MultiHandlerCollection<ViewModelMessage>();
			IMessageBus<ViewModelMessage> viewModelMessageBus = new LocalMessageBus<ViewModelMessage>(handlerCollection);
			IViewModelCollectionList viewModelCollections = new ViewModelCollectionList();

			IViewModelCommunication viewModelCommunication = new ViewModelCommunication(viewModelMessageBus,
																						viewModelCollections);


			var mainWindowBuilder = new MainWindowBuilder(localSettingsRepository,
														  clientPatientRepository,
														  clientMedicalPracticeRepository,
														  clientReadmodelRepository,
														  clienttherapyPlaceTypeRepository,
														  clientLabelRepository,
														  commandService,
														  viewModelCommunication,
														  session,
														  fatalErrorHandler.HandleFatalError);

			var mainWindow = mainWindowBuilder.BuildWindow();

			mainWindow.Show();

#if DEBUG
			

			var debugOutputWindowViewModel = new DebugOutputWindowViewModel(listener);

			var debugWindow = new DebugOutputWindow
			{
				Owner = mainWindow,
				DataContext = debugOutputWindowViewModel
			};

			debugWindow.Show();
#endif

		}

		private static void AssureAppDataDirectoriesExist ()
		{
			if (!Directory.Exists(GlobalConstants.ClientBasePath))
			{
				Directory.CreateDirectory(GlobalConstants.ClientBasePath);
			}
		}

		public void CleanUp (ExitEventArgs exitEventArgs)
		{
			localSettingsRepository.PersistRepository();

			connectionService.Dispose();
		}
	}
}
