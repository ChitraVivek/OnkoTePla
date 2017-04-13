using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.BackupPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LicencePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.MainWindow
{
	internal class MainWindowViewModel : ViewModel, IMainWindowViewModel
    {
		private readonly ISharedState<MainPage> selectedPageVariable;

		private MainPage selectedPage;		

		public MainWindowViewModel(IOverviewPageViewModel overviewPageViewModel, 
                                   IConnectionsPageViewModel connectionsPageViewModel,
                                   IUserPageViewModel userPageViewModel,
                                   ILicencePageViewModel licencePageViewModel, 
                                   IInfrastructurePageViewModel infrastructurePageViewModel,
								   IHoursOfOpeningPageViewModel hoursOfOpeningPageViewModel,
								   ITherapyPlaceTypesPageViewModel therapyPlaceTypesPageViewModel,
								   ILabelPageViewModel labelPageViewModel,
								   IPatientsPageViewModel patientsPageViewModel,
								   IBackupPageViewModel backupPageViewModel,
								   IOptionsPageViewModel optionsPageViewModel, 
                                   IAboutPageViewModel aboutPageViewModel,
								   ISharedState<MainPage> selectedPageVariable)
        {
	        this.selectedPageVariable     = selectedPageVariable;
			LabelPageViewModel = labelPageViewModel;
			BackupPageViewModel = backupPageViewModel;
			
			selectedPageVariable.StateChanged += OnSelectedPageVariableChanged;

	        PatientsPageViewModel          = patientsPageViewModel;
	        OverviewPageViewModel          = overviewPageViewModel;
            ConnectionsPageViewModel       = connectionsPageViewModel;
            UserPageViewModel              = userPageViewModel;
            LicencePageViewModel           = licencePageViewModel;
            InfrastructurePageViewModel    = infrastructurePageViewModel;
	        HoursOfOpeningPageViewModel    = hoursOfOpeningPageViewModel;
            OptionsPageViewModel           = optionsPageViewModel;
            AboutPageViewModel             = aboutPageViewModel;
	        TherapyPlaceTypesPageViewModel = therapyPlaceTypesPageViewModel;

	        SwitchToPage = new ParameterrizedCommand<MainPage>(page => SelectedPage = page);
			CloseWindow = new Command(DoCloseApplication);

			CheckWindowClosing = true;
        }

		private void OnSelectedPageVariableChanged(MainPage newPage)
		{
			SelectedPage = newPage;
		}

		private void DoCloseApplication()
		{
			CheckWindowClosing = false;

			if (ConnectionsPageViewModel.IsConnectionActive)
			{
				ConnectionsPageViewModel.DeactivateConnection.Execute(null);
				Application.Current.Dispatcher.DelayInvoke(KillWindow, TimeSpan.FromSeconds(3));
			}
			else
			{				
				KillWindow();
			}
		}

		private static void KillWindow()
		{
			var windows = Application.Current.Windows
											 .OfType<Visualization.MainWindow>()
											 .ToList();
			if (windows.Count == 1)
				windows[0].Close();
			else
				throw new Exception("inner error");
		}

		public ICommand SwitchToPage { get; }

        public MainPage SelectedPage
        {
            get { return selectedPage; }
	        private set
	        {
		        PropertyChanged.ChangeAndNotify(this, ref selectedPage, value);
		        selectedPageVariable.Value = value;
	        }
        }

		public bool CheckWindowClosing { get; private set; }
		public ICommand CloseWindow { get; }
	

		public IOverviewPageViewModel          OverviewPageViewModel          { get; }
        public IConnectionsPageViewModel       ConnectionsPageViewModel       { get; }
        public IUserPageViewModel              UserPageViewModel              { get; }
        public ILicencePageViewModel           LicencePageViewModel           { get; }
        public IInfrastructurePageViewModel    InfrastructurePageViewModel    { get; }
		public IHoursOfOpeningPageViewModel    HoursOfOpeningPageViewModel    { get; }
		public ITherapyPlaceTypesPageViewModel TherapyPlaceTypesPageViewModel { get; }
		public ILabelPageViewModel			   LabelPageViewModel			  { get; }
		public IPatientsPageViewModel		   PatientsPageViewModel          { get; }
		public IBackupPageViewModel			   BackupPageViewModel			  { get; }
		public IOptionsPageViewModel           OptionsPageViewModel           { get; }
        public IAboutPageViewModel             AboutPageViewModel             { get; }

        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;        
    }
}
