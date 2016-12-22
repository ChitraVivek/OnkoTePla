using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.LoginViewModel;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.MainViewModel;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.ActionBar;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.LoginView;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.MainView;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.NotificationServiceContainer;
using Size = bytePassion.Lib.Types.SemanticTypes.Size;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.MainWindow
{
	internal class MainWindowViewModel : ViewModel, 
                                         IMainWindowViewModel
    {
        private readonly IMainViewModelBuilder  mainViewModelBuilder;
		private readonly ISession session;
		private readonly Action<string> errorCallback;

		private IMainViewModel mainViewModel;        
        private ILoginViewModel loginViewModel;

        private bool isMainViewVisible;
        private bool isLoginViewVisible;
        private bool isDisabledOverlayVisible;

		private Size lastGridSize;

        public MainWindowViewModel(IMainViewModelBuilder mainViewModelBuilder, 
                                   ILoginViewModelBuilder loginViewModelBuilder,
                                   INotificationServiceContainerViewModel notificationServiceContainerViewModel, 
                                   IActionBarViewModel actionBarViewModel,
								   ISession session,
								   Action<string> errorCallback)
        {
            this.mainViewModelBuilder = mainViewModelBuilder;
	        this.session = session;
	        this.errorCallback = errorCallback;

	        NotificationServiceContainerViewModel = notificationServiceContainerViewModel;
            ActionBarViewModel = actionBarViewModel;

			session.ApplicationStateChanged += OnApplicationStateChanged;
            
            LoginViewModel = loginViewModelBuilder.Build();

            IsMainViewVisible = false;
            IsLoginViewVisible = true;

	        CheckWindowClosing = true;
			CloseWindow = new Command(DoCloseWindow);
        }

		private void DoCloseWindow()
		{
			switch (session.CurrentApplicationState)
			{
				case ApplicationState.LoggedIn:				
				case ApplicationState.TryingToConnect:
				case ApplicationState.ConnectedButNotLoggedIn:
				{
					session.TryDisconnect(() =>
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								CheckWindowClosing = false;
								KillWindow();
							});
						},
						_ => KillWindow()
					);
					break;
				}								

				default:
				{
					CheckWindowClosing = false;
					KillWindow();
					break;
				}
			}
		}

		private static void KillWindow()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var windows = Application.Current.Windows
											 .OfType<WpfUi.MainWindow>()
											 .ToList();

				if (windows.Count == 1)
					windows[0].Close();
				else
					throw new Exception("inner error");
			});			
		}

		private void OnApplicationStateChanged(ApplicationState newApplicationState)
		{
			switch (newApplicationState)
			{
				case ApplicationState.LoggedIn:
				{					
					MainViewModel = mainViewModelBuilder.Build(errorCallback, lastGridSize);

					IsMainViewVisible = true;
					IsLoginViewVisible = false;

					break;
				}

				default:
				{
					if (MainViewModel != null)
					{
						lastGridSize = mainViewModelBuilder.GetCurrentGridSize();

						mainViewModelBuilder.DisposeViewModel(MainViewModel);
						MainViewModel = null;						

						IsMainViewVisible = false;
						IsLoginViewVisible = true;
					}

					break;
				}
			}
		}


		public IMainViewModel MainViewModel
        {
            get { return mainViewModel; }
            private set { PropertyChanged.ChangeAndNotify(this, ref mainViewModel, value); }
        }

        public bool IsMainViewVisible
        {
            get { return isMainViewVisible; }
            private set { PropertyChanged.ChangeAndNotify(this, ref isMainViewVisible, value); }
        }

        public bool IsLoginViewVisible
        {
            get { return isLoginViewVisible; }
            private set { PropertyChanged.ChangeAndNotify(this, ref isLoginViewVisible, value); }
        }

        public bool IsDisabledOverlayVisible
        {
            get { return isDisabledOverlayVisible; }
            private set { PropertyChanged.ChangeAndNotify(this, ref isDisabledOverlayVisible, value); }
        }

		public bool CheckWindowClosing { get; private set; }

		public ICommand CloseWindow { get; }

		public ILoginViewModel LoginViewModel
        {
            get { return loginViewModel; }
            private set { PropertyChanged.ChangeAndNotify(this, ref loginViewModel, value); }
        }

        public IActionBarViewModel ActionBarViewModel { get; }

        public INotificationServiceContainerViewModel NotificationServiceContainerViewModel { get; }

        public void Process(ShowDisabledOverlay message)
        {
            IsDisabledOverlayVisible = true;
        }

        public void Process(HideDisabledOverlay message)
        {
            IsDisabledOverlayVisible = false;
        }

        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;        
    }
}