using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Utils;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Client.WpfUi.Enums;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.ConnectionStatusView;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Resources.UserNotificationService;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.ActionBar
{
	internal class ActionBarViewModel : ViewModel, IActionBarViewModel
    {
		private readonly ISession session;
		private readonly IViewModelCommunication viewModelCommunication;
        private readonly IWindowBuilder<Views.AboutDialog> dialogBuilder;		
		private bool navigationAndLogoutButtonVisibility;

		public ActionBarViewModel(ISession session,
								  IConnectionStatusViewModel connectionStatusViewModel,
                                  IViewModelCommunication viewModelCommunication,
                                  IWindowBuilder<Views.AboutDialog> dialogBuilder)
        {
	        this.session = session;
	        this.viewModelCommunication = viewModelCommunication;
            this.dialogBuilder = dialogBuilder;			
			ConnectionStatusViewModel = connectionStatusViewModel;

	        var titleBuilder = new StringBuilder();

#if DEBUG
	        titleBuilder.Append(">>> DEBUG <<<    ");
#endif
	        titleBuilder.Append("OnkoTePla - Version ");
	        titleBuilder.Append(ApplicationInfo.ClientVersion);

#if DEBUG
			titleBuilder.Append("    >>> DEBUG <<<");
#endif

	        Title = titleBuilder.ToString();

			ShowOverview = new Command(() => viewModelCommunication.Send(new ShowPage(MainPage.Overview)));
            ShowSearch   = new Command(() => viewModelCommunication.Send(new ShowPage(MainPage.Search)));
            ShowOptions  = new Command(() => viewModelCommunication.Send(new ShowPage(MainPage.Options)));

            ShowAbout = new Command(ShowAboutDialog);
			Logout    = new Command(DoLogOut);

			session.ApplicationStateChanged += OnApplicationStateChanged;
			OnApplicationStateChanged(session.CurrentApplicationState);
        }

		private void OnApplicationStateChanged(ApplicationState newApplicationState)
		{
			NavigationAndLogoutButtonVisibility = newApplicationState == ApplicationState.LoggedIn;
		}

		private void DoLogOut()
	    {
			session.Logout(
				() => { },
				errorMessage =>
				{
					Application.Current.Dispatcher.Invoke(async () =>
					{
						var dialog = new UserDialogBox("",
													   "Logout nicht erfolgreich:\n" +
													   $">> {errorMessage} <<",													  
													   MessageBoxButton.OK);
						await dialog.ShowMahAppsDialog();						
					});
				}	
			);
	    }

	    private void ShowAboutDialog()
        {
            viewModelCommunication.Send(new ShowDisabledOverlay());

            var dialogWindow = dialogBuilder.BuildWindow();
            dialogWindow.ShowDialog();
			
            viewModelCommunication.Send(new HideDisabledOverlay());
        }

	    public string Title { get; }

	    public ICommand ShowOverview { get; }
        public ICommand ShowSearch   { get; }
        public ICommand ShowOptions  { get; }
        public ICommand Logout       { get; }
        public ICommand ShowAbout    { get; }

		public bool NavigationAndLogoutButtonVisibility
		{
			get { return navigationAndLogoutButtonVisibility; }
			private set { PropertyChanged.ChangeAndNotify(this, ref navigationAndLogoutButtonVisibility, value); }
		}

		public IConnectionStatusViewModel ConnectionStatusViewModel { get; }

        protected override void CleanUp()
        {
			session.ApplicationStateChanged -= OnApplicationStateChanged;
		}
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
