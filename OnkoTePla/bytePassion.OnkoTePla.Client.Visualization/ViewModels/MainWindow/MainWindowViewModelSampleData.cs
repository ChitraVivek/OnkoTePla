using System.ComponentModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.ActionBar;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainWindow
{
	internal class MainWindowViewModelSampleData : IMainWindowViewModel
	{
		public MainWindowViewModelSampleData()
		{
			MainViewModel = new MainViewModelSampleData();

		    IsMainViewVisible = true;
		    IsLoginViewVisible = true;
		    IsDisabledOverlayVisible = false;

			CheckWindowClosing = false;

            LoginViewModel = new LoginViewModelSampleData();
            ActionBarViewModel = new ActionBarViewModelSampleData();
            NotificationServiceContainerViewModel = new NotificationServiceContainerViewModelSampleData();
		}
					            
        public IMainViewModel MainViewModel { get; }
        public bool IsMainViewVisible { get; }
        public bool IsLoginViewVisible { get; }
        public bool IsDisabledOverlayVisible { get; }

	    public bool CheckWindowClosing { get; }
	    public ICommand CloseWindow => null;	    

        public ILoginViewModel LoginViewModel { get; }
        public IActionBarViewModel ActionBarViewModel { get; }
        public INotificationServiceContainerViewModel NotificationServiceContainerViewModel { get; }

        public void Process(ShowDisabledOverlay message) { }
        public void Process(HideDisabledOverlay message) { }

        public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;        
	}
}
