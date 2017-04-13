using System.IO;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Server.DataAndService.Backup;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.DataAndService.Factorys;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Config;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.EventStore;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.StreamMetaData;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.StreamPersistance;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.XmlDataStores;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.SampleDataGenerators;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.BackupPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LicencePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.MainWindow;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage;

namespace bytePassion.OnkoTePla.Server.Visualization
{
	public partial class App
    {		
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

			AssureAppDataDirectoriesExist();

			///////////////////////////////////////////////////////////////////////////////////////////////////
			////////                                                                                 //////////
			////////                            Composition Root and Setup                           //////////
			////////                                                                                 //////////
			///////////////////////////////////////////////////////////////////////////////////////////////////

			var dataCenterContainer = new DataCenterContainer();

			// ConnectionService

			var connectionServiceBuilder = new ConnectionServiceBuilder(dataCenterContainer);
			var connectionService = connectionServiceBuilder.Build();

			// Patient-Repository

			var patientPersistenceService = new XmlPatientDataStore(GlobalConstants.PatientPersistenceFile);
			var patientRepository = new PatientRepository(patientPersistenceService, connectionService);
			patientRepository.LoadRepository();


			// Config-Repository

			var configPersistenceService = new XmlConfigurationDataStore(GlobalConstants.ConfigPersistenceFile);
			var configRepository = new ConfigurationRepository(configPersistenceService);
			configRepository.LoadRepository();

			
			// LocalSettings-Repository

			var settingsPersistenceService = new LocalSettingsXmlPersistenceService(GlobalConstants.LocalServerSettingsPersistanceFile);
			var localSettingsRepository = new LocalSettingsRepository(settingsPersistenceService);
			localSettingsRepository.LoadRepository();

			// Event-Store

			var eventStreamPersistenceService = new XmlEventStreamPersistanceService();
			var streamPersistenceService = new StreamPersistenceService(eventStreamPersistenceService, configRepository, GlobalConstants.EventHistoryBasePath, 500);
			var metaDataPersistenceService = new XmlPracticeMetaDataPersistanceService(GlobalConstants.MetaDataPersistanceFile);
			var metaDataService = new StreamMetaDataService(metaDataPersistenceService);

			var eventStore = new EventStore(streamPersistenceService, metaDataService, connectionService);
			eventStore.LoadRepository();

			// DataAndService

			var dataCenter = new DataCenter(configRepository, patientRepository, eventStore);			
	        dataCenterContainer.DataCenter = dataCenter;

	        var backUpService = new BackupService(patientRepository, configRepository, eventStore, connectionService);
	        var backupScheduler = new BackupScheduler(backUpService);
	        backupScheduler.Start(localSettingsRepository);

			// ViewModel-Variables

			var selectedPageVariable = new SharedState<MainPage>(MainPage.Overview);


			// sampleData-generators

			var patientNameGenerator = new PatientNameGenerator();
			var appointmentGenerator = new AppointmentGenerator(configRepository, patientRepository, eventStore);


			// ViewModels

			var selectedPatientVariable = new SharedState<Patient>(null);
			var patientSelectorViewModel = new PatientSelectorViewModel(patientRepository, selectedPatientVariable, null);

            var overviewPageViewModel          = new OverviewPageViewModel(connectionService);
            var connectionsPageViewModel       = new ConnectionsPageViewModel(dataCenter, connectionService, selectedPageVariable);
            var userPageViewModel              = new UserPageViewModel(dataCenter, selectedPageVariable);
            var licencePageViewModel           = new LicencePageViewModel();
            var infrastructurePageViewModel    = new InfrastructurePageViewModel(dataCenter, selectedPageVariable, appointmentGenerator);
			var hoursOfOpeningPageViewModel    = new HoursOfOpeningPageViewModel(dataCenter, selectedPageVariable);
			var therapyPlaceTypesPageViewModel = new TherapyPlaceTypesPageViewModel(dataCenter, selectedPageVariable, connectionService);
			var labelPageViewModel			   = new LabelPageViewModel(dataCenter, selectedPageVariable, connectionService);
			var patientsPageViewModel          = new PatientsPageViewModel(patientSelectorViewModel, patientRepository, selectedPatientVariable, patientNameGenerator);
			var backupPageViewModel			   = new BackupPageViewModel(backUpService, backupScheduler, localSettingsRepository);
			var optionsPageViewModel           = new OptionsPageViewModel();
            var aboutPageViewModel             = new AboutPageViewModel(); 
	        
	        var mainWindowViewModel = new MainWindowViewModel(overviewPageViewModel,
                                                              connectionsPageViewModel,
                                                              userPageViewModel,
                                                              licencePageViewModel,
                                                              infrastructurePageViewModel, 
															  hoursOfOpeningPageViewModel,  
															  therapyPlaceTypesPageViewModel,  
															  labelPageViewModel,
															  patientsPageViewModel,
															  backupPageViewModel,                                                         
                                                              optionsPageViewModel,
                                                              aboutPageViewModel,
															  selectedPageVariable);
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
            
			backupScheduler.Stop();

			configRepository.PersistRepository();
			patientRepository.PersistRepository();
			eventStore.PersistRepository();			
			localSettingsRepository.PersistRepository();

			connectionServiceBuilder.DisposeConnectionService(connectionService);
        }

		private void AssureAppDataDirectoriesExist()
		{
			if (!Directory.Exists(GlobalConstants.ServerBasePath))
			{
				Directory.CreateDirectory(GlobalConstants.ServerBasePath);
				Directory.CreateDirectory(GlobalConstants.BackupBasePath);
				Directory.CreateDirectory(GlobalConstants.EventHistoryBasePath);
			}
		}
    }
}
