using System.Windows.Input;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.ActionBar;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainWindow
{
	internal interface IMainWindowViewModel : IViewModel, 
                                              IViewModelMessageHandler<ShowDisabledOverlay>,
                                              IViewModelMessageHandler<HideDisabledOverlay>
    {
		IMainViewModel MainViewModel { get; }

        bool IsMainViewVisible { get; }
        bool IsLoginViewVisible { get; }

        bool IsDisabledOverlayVisible { get; }
       
		bool CheckWindowClosing { get; }
		ICommand CloseWindow { get; }

        ILoginViewModel LoginViewModel { get; }
        IActionBarViewModel ActionBarViewModel { get; }
        INotificationServiceContainerViewModel NotificationServiceContainerViewModel { get; }        
    }
}
