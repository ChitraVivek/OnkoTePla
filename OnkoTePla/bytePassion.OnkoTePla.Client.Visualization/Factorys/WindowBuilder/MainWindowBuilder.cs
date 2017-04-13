using System;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.PatientRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.LoginViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.MainViewModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.ActionBar;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.ConnectionStatusView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainWindow;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder
{
	public class MainWindowBuilder : IWindowBuilder<MainWindow>
	{
		private readonly ILocalSettingsRepository localSettingsRepository;
		private readonly IClientPatientRepository patientRepository;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;		
		private readonly IClientReadModelRepository readModelRepository;
		private readonly IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository;
		private readonly IClientLabelRepository labelRepository;
		private readonly ICommandService commandService;
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly ISession session;			    	
        private readonly Action<string> errorCallback;

		public MainWindowBuilder(ILocalSettingsRepository localSettingsRepository,
								 IClientPatientRepository patientRepository,
								 IClientMedicalPracticeRepository medicalPracticeRepository,								 
								 IClientReadModelRepository readModelRepository,	
								 IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository,	
								 IClientLabelRepository labelRepository,						 
								 ICommandService commandService,
                                 IViewModelCommunication viewModelCommunication,
								 ISession session,								
                                Action<string> errorCallback)
		{
	        this.localSettingsRepository = localSettingsRepository;
	        this.patientRepository = patientRepository;
	        this.medicalPracticeRepository = medicalPracticeRepository;	        
	        this.readModelRepository = readModelRepository;
	        this.therapyPlaceTypeRepository = therapyPlaceTypeRepository;
			this.labelRepository = labelRepository;
			this.commandService = commandService;
	        this.viewModelCommunication = viewModelCommunication;
	        this.session = session;	        
            this.errorCallback = errorCallback;
		} 

		public MainWindow BuildWindow()
		{
            // build modules

            var adornerControl = new AdornerControl();			

            // build viewModels

            var mainViewModelBuilder = new MainViewModelBuilder(medicalPracticeRepository,
																readModelRepository,
																patientRepository,
																therapyPlaceTypeRepository,	
																labelRepository,															
																commandService, 
																localSettingsRepository, 
                                                                viewModelCommunication,
																session,                                                                
                                                                adornerControl);

            var loginViewModelBuilder = new LoginViewModelBuilder(session, 
																  localSettingsRepository);

            var notificationServiceContainerViewModel = new NotificationServiceContainerViewModel(viewModelCommunication);

		    var connectionStatusViewModel = new ConnectionStatusViewModel(session);

		    var dialogBuilder = new AboutDialogWindowBuilder();

		    var actionBarViewModel = new ActionBarViewModel(session,
															connectionStatusViewModel,
                                                            viewModelCommunication,
                                                            dialogBuilder);

		    var mainWindowViewModel = new MainWindowViewModel(mainViewModelBuilder,
                                                              loginViewModelBuilder,
                                                              notificationServiceContainerViewModel,
                                                              actionBarViewModel,
															  session, 
															  errorCallback);

            // build mainWindow

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

            // set GridContainer as ReferenceElement of AdornerControl

		    adornerControl.ReferenceElement = mainWindow.MainView.OverviewPage.GridContainer;

            viewModelCommunication.RegisterViewModelMessageHandler<ShowDisabledOverlay>(mainWindowViewModel);
            viewModelCommunication.RegisterViewModelMessageHandler<HideDisabledOverlay>(mainWindowViewModel);

            return mainWindow;
		}

		public void DisposeWindow(MainWindow buildedWindow)
		{
			throw new NotImplementedException();
		}
	}
}
