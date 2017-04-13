using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.Communication;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.Commands.Updater;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.DataAndService.Connection;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage
{
	internal class ConnectionsPageViewModel : ViewModel, IConnectionsPageViewModel
	{
		private const string NoConnection = "- keine Verbindung momentan -";

	    private readonly IDataCenter dataCenter;
		private readonly IConnectionService connectionService;
		private readonly ISharedStateReadOnly<MainPage> selectedPageVariable;

		private string selectedIpAddress;
	    private string activeConnection;		
		private bool isActivationPossible;

		private bool connectionActivationLocked;
		private DispatcherTimer lockedTimer;
		private bool isConnectionActive;

		public ConnectionsPageViewModel(IDataCenter dataCenter, 
										IConnectionService connectionService,
										ISharedStateReadOnly<MainPage> selectedPageVariable)
	    {
		    this.dataCenter = dataCenter;
		    this.connectionService = connectionService;
			this.selectedPageVariable = selectedPageVariable;

			selectedPageVariable.StateChanged += SelectedPageVariableOnStateChanged;

			UpdateAvailableAddresses = new Command(UpdateAddresses);

			ActivateConnection = new Command(DoActivateConnection,
											 () => !IsConnectionActive && IsActivationPossible,
											 new PropertyChangedCommandUpdater(this, nameof(IsConnectionActive), nameof(IsActivationPossible)));
			DeactivateConnection = new Command(DoDeactivateConnection, 
											   () => IsConnectionActive,
											   new PropertyChangedCommandUpdater(this, nameof(IsConnectionActive)));

			AvailableIpAddresses = new ObservableCollection<string>();
			ConnectedClients = new ObservableCollection<ConnectedClientDisplayData>();

		    UpdateAddresses();

			ActiveConnection = NoConnection;			
			connectionActivationLocked = false;
		    SelectedIpAddress = AvailableIpAddresses.First();

			connectionService.NewSessionStarted += OnNewSessionStarted;
			connectionService.SessionTerminated += OnSessionTerminated;
			connectionService.LoggedInUserUpdated += OnLoggedInUserUpdated;

			connectionService.ConnectionStatusChanged += OnConnectionStatusChanged;

			IsConnectionActive = connectionService.IsConnectionActive;

			CheckIfActicationIsPossible();

//			if (ActivateConnection.CanExecute(null))
//				ActivateConnection.Execute(null);	 // TODO: just for testing
	    }

		private void OnConnectionStatusChanged()
		{
			IsConnectionActive = connectionService.IsConnectionActive;
		}

		private void SelectedPageVariableOnStateChanged(MainPage mainPage)
		{
			if (mainPage == MainPage.Connections)
			{
				CheckIfActicationIsPossible();
			}
		}

		private void OnLoggedInUserUpdated(SessionInfo sessionInfo)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var displayData = ConnectedClients.First(dd => dd.SessionId == sessionInfo.SessionId.ToString());
				displayData.LogginInUser = sessionInfo.LoggedInUser == null
												? "no User logged in"
												: sessionInfo.LoggedInUser.Name;
			});			
		}

		private void OnSessionTerminated(SessionInfo sessionInfo)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var displayData = ConnectedClients.First(dd => dd.SessionId == sessionInfo.SessionId.ToString());
				ConnectedClients.Remove(displayData);
			});			
		}

		private void OnNewSessionStarted(SessionInfo sessionInfo)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ConnectedClients.Add(new ConnectedClientDisplayData(sessionInfo.SessionId.ToString(),
																	sessionInfo.CreationTime.ToString(),
																	sessionInfo.ClientAddress.ToString()));
			});			
		}

		private void UpdateAddresses()
	    {
			AvailableIpAddresses.Clear();

		    dataCenter.GetAllAvailableAddresses()
					  .Select(address => address.Identifier.ToString())
					  .Do(AvailableIpAddresses.Add);
	    }

	    public ICommand UpdateAvailableAddresses { get; }
		public ICommand ActivateConnection       { get; }
		public ICommand DeactivateConnection     { get; }		

		private void DoDeactivateConnection ()
		{
			connectionActivationLocked = true;
			CheckIfActicationIsPossible();
			lockedTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(3),
				IsEnabled = true
			};
			lockedTimer.Tick += OnLockedTimerTick;

			ActiveConnection = NoConnection;
			connectionService.StopCommunication();
		}

		private void DoActivateConnection ()
		{
			ActiveConnection = SelectedIpAddress;
			connectionService.InitiateCommunication(new Address(new TcpIpProtocol(),
																AddressIdentifier.GetIpAddressIdentifierFromString(SelectedIpAddress)));
		}

		private void OnLockedTimerTick(object sender, EventArgs eventArgs)
		{
			lockedTimer.Tick -= OnLockedTimerTick;
			lockedTimer.IsEnabled = false;			

			connectionActivationLocked = false;
			CheckIfActicationIsPossible();
		}

		public bool IsConnectionActive
		{
			get { return isConnectionActive; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isConnectionActive, value); }
		}

		public bool IsActivationPossible
		{
			get { return isActivationPossible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isActivationPossible, value); }
		}

		public string SelectedIpAddress
	    {
		    get { return selectedIpAddress; }
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref selectedIpAddress, value);
				CheckIfActicationIsPossible();
			}
	    }

	    public string ActiveConnection
	    {
		    get { return activeConnection; }
		    private set { PropertyChanged.ChangeAndNotify(this, ref activeConnection, value); }
	    }

	    public ObservableCollection<string> AvailableIpAddresses { get; }
	    public ObservableCollection<ConnectedClientDisplayData> ConnectedClients { get; }


		private void CheckIfActicationIsPossible()
		{
			IsActivationPossible = !connectionActivationLocked && 
								   !string.IsNullOrWhiteSpace(SelectedIpAddress) &&
								   dataCenter.GetAllUsers().Count(user => !user.IsHidden && user.ListOfAccessableMedicalPractices.Any()) > 0 &&
								   dataCenter.GetAllMedicalPractices().Any();								  
		}

		protected override void CleanUp()
		{
			selectedPageVariable.StateChanged -= SelectedPageVariableOnStateChanged;

			connectionService.NewSessionStarted       -= OnNewSessionStarted;
			connectionService.SessionTerminated       -= OnSessionTerminated;
			connectionService.LoggedInUserUpdated     -= OnLoggedInUserUpdated;
			connectionService.ConnectionStatusChanged -= OnConnectionStatusChanged;
		}

		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
