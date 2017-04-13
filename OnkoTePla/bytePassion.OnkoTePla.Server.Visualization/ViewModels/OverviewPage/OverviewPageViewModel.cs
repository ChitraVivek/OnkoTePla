using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.DataAndService.Connection;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.OverviewPage
{
	public class OverviewPageViewModel : ViewModel, IOverviewPageViewModel
    {
		private readonly IConnectionService connectionService;

		private string connectionStatusText;		
		private bool isConnectionActive;

		public OverviewPageViewModel(IConnectionService connectionService)
		{
			this.connectionService = connectionService;

			CurrentlyLoggedInUser = new ObservableCollection<string>();

			connectionService.ConnectionStatusChanged += OnConnectionStatusChanged;
			connectionService.LoggedInUserUpdated     += OnLoggedInUserUpdated;
			OnConnectionStatusChanged();
		}

		private void OnLoggedInUserUpdated(SessionInfo sessionInfo)
		{
			UpdateUserList();
		}

		private void OnConnectionStatusChanged()
		{
			IsConnectionActive = connectionService.IsConnectionActive;

			ConnectionStatusText = connectionService.IsConnectionActive 
										? "active" 
										: "inactive";

			if (!IsConnectionActive)
			{
				CurrentlyLoggedInUser.Clear();
				CurrentlyLoggedInUser.Add("momentan ist kein Benutzer eingeloggt");
			}
			else
			{
				UpdateUserList();
			}
		}

		private void UpdateUserList()
		{			
			var userList = connectionService.GetAllCurrentlyLoggedInUser();

			Application.Current.Dispatcher.Invoke(() =>
			{
				CurrentlyLoggedInUser.Clear();
				userList.Do(user => CurrentlyLoggedInUser.Add(user.Name));

				if (CurrentlyLoggedInUser.Count == 0)
				{
					CurrentlyLoggedInUser.Add("momentan ist kein Benutzer eingeloggt");
				}
			});			
		}

		public string ConnectionStatusText
		{
			get { return connectionStatusText; }
			private set { PropertyChanged.ChangeAndNotify(this, ref connectionStatusText, value); }
		}

		public bool IsConnectionActive
		{
			get { return isConnectionActive; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isConnectionActive, value); }
		}

		public ObservableCollection<string> CurrentlyLoggedInUser { get; }

		protected override void CleanUp()
		{
			connectionService.ConnectionStatusChanged -= OnConnectionStatusChanged;
		}
        public override event PropertyChangedEventHandler PropertyChanged;		
    }
}
