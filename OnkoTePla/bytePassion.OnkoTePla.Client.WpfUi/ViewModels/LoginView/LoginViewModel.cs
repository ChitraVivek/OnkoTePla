using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.Communication;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.ZmqUtils;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Resources.UserNotificationService;

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.LoginView
{
	internal class LoginViewModel : ViewModel, 
                                    ILoginViewModel
	{
		private static readonly Protocol Protocol = new TcpIpProtocol();

		private readonly ISession session;
		private readonly  ILocalSettingsRepository localSettingsRepository;

		private ClientUserData selectedUser;
		private string serverAddress;
		private string clientAddress;		

		private bool autoConnectOnNextStart;
		private bool areConnectionSettingsVisible;
		private bool isUserListAvailable;

		public LoginViewModel(ISession session,
							  ILocalSettingsRepository localSettingsRepository)
	    {
		    this.session = session;
			this.localSettingsRepository = localSettingsRepository;

			Login = new ParameterrizedCommand<PasswordBox>(DoLogin,
													       IsLoginPossible);

#if DEBUG
			Connect = new Command(DoDebugConnect, IsConnectPossible);
#else

			Connect = new Command(DoConnect, IsConnectPossible);
#endif

			DebugConnect = new Command(DoDebugConnect,
									   IsConnectPossible);

			Disconnect = new Command(DoDisconnect,
									 IsDisconnectPossible);

			AutoConnectOnNextStart = localSettingsRepository.IsAutoConnectionEnabled;

			AvailableUsers = new ObservableCollection<ClientUserData>();

			ClientIpAddresses = IpAddressCatcher.GetAllAvailableLocalIpAddresses()
												.Select(address => address.Identifier.ToString())
												.ToObservableCollection();

			ClientAddress = ClientIpAddresses.First();
			
			session.ApplicationStateChanged += OnApplicationStateChanged;
			OnApplicationStateChanged(session.CurrentApplicationState);

			AreConnectionSettingsVisible = !AutoConnectOnNextStart;

			if (AutoConnectOnNextStart)
			{
				var clientIpAddress = localSettingsRepository.AutoConnectionClientAddress.ToString();

				if (ClientIpAddresses.Contains(clientIpAddress))
				{
					ClientAddress = clientIpAddress;
					ServerAddress = localSettingsRepository.AutoConnectionServerAddress.ToString();
					Connect.Execute(null);
				}
			}
	    }		

		private void OnApplicationStateChanged(ApplicationState applicationState)
		{			
			if (applicationState == ApplicationState.DisconnectedFromServer)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					AvailableUsers.Clear();
					AreConnectionSettingsVisible = true;
					SelectedUser = null;
					IsUserListAvailable = false;
				});				
			}							

			if (applicationState == ApplicationState.ConnectedButNotLoggedIn)
			{								
				AreConnectionSettingsVisible = false;

				session.RequestUserList(
					userList => 
					{
						Application.Current.Dispatcher.Invoke(async () =>
						{
							AvailableUsers.Clear();
							userList.Do(userData => AvailableUsers.Add(userData));
							IsUserListAvailable = true;

							if (AvailableUsers.Count > 0)
							{
								SelectedUser = AvailableUsers.Any(user => user.Id == localSettingsRepository.LastLoggedInUserId)
													? AvailableUsers.First(user => user.Id == localSettingsRepository.LastLoggedInUserId)
													: AvailableUsers.First();
							}
							else
							{
								var dialog = new UserDialogBox("",
															   "Es sind keine verfügbaren User vorhanden\n" +														 
															   "Die Verbindung wird getrennt!",
															   MessageBoxButton.OK);
								await dialog.ShowMahAppsDialog();
								Disconnect.Execute(null);
							}
						});					
					},
					errorMessage =>
					{
						Application.Current.Dispatcher.Invoke(async () =>
						{
							var dialog = new UserDialogBox("", 
														  "Die Userliste kann nicht vom Server abgefragt werden:\n" +
														  $">> {errorMessage} <<\n" +
														  "Die Verbindung wird getrennt - versuchen Sie es erneut", 
														  MessageBoxButton.OK);
							await dialog.ShowMahAppsDialog();
							Disconnect.Execute(null);
						});
					}
				);
			}


			Application.Current.Dispatcher.Invoke(() =>
			{
				((ParameterrizedCommand<PasswordBox>) Login).RaiseCanExecuteChanged();
				((Command) Connect).RaiseCanExecuteChanged();
				((Command) DebugConnect).RaiseCanExecuteChanged();
				((Command) Disconnect).RaiseCanExecuteChanged();
			});
		}	
		
		
		private void DoDisconnect()
		{
			session.TryDisconnect(
				null,
				errorMessage =>
				{
					Application.Current.Dispatcher.Invoke(async () =>
					{
						var dialog = new UserDialogBox("",
													   "Die Trennung der Verbindung konnte nicht ordnungsgemäß durchgeführt werden\n" +
													   $">> {errorMessage} <<",
													   MessageBoxButton.OK);
						await dialog.ShowMahAppsDialog();					
					});
				}
			);
		}
		private bool IsDisconnectPossible ()
		{
			return session.CurrentApplicationState == ApplicationState.ConnectedButNotLoggedIn;
		}

		private async void DoDebugConnect ()
		{
			if (AddressIdentifier.IsIpAddressIdentifier(ServerAddress))
			{
				session.TryDebugConnect(new Address(Protocol, AddressIdentifier.GetIpAddressIdentifierFromString(ServerAddress)),
								        new Address(Protocol, AddressIdentifier.GetIpAddressIdentifierFromString(ClientAddress)),
										errorMessage =>
										{
											Application.Current.Dispatcher.Invoke(async () =>
											{
												var dialog = new UserDialogBox(
													"",
													$"Es kann keine Verbindung mit {ServerAddress} hergestellt werden",
													MessageBoxButton.OK
												);
												await dialog.ShowMahAppsDialog();
											});
										});
			}
			else
			{
				var dialog = new UserDialogBox("", $"{ServerAddress} ist keine gültige Ip-Adresse",
											   MessageBoxButton.OK);
				await dialog.ShowMahAppsDialog();
			}

			SetAutoConnectToSettings();
		}
		private async void DoConnect()
		{
			if (AddressIdentifier.IsIpAddressIdentifier(ServerAddress))
			{
				session.TryConnect(new Address(Protocol, AddressIdentifier.GetIpAddressIdentifierFromString(ServerAddress)),
								   new Address(Protocol, AddressIdentifier.GetIpAddressIdentifierFromString(ClientAddress)),
								   errorMessage =>
								   {
									   Application.Current.Dispatcher.Invoke(async () =>
									   {
										   var dialog = new UserDialogBox(
													"",
													$"Es kann keine Verbindung mit {ServerAddress} hergestellt werden",
													MessageBoxButton.OK
												);
										   await dialog.ShowMahAppsDialog();
									   });
								   });
			}
			else
			{
				var dialog = new UserDialogBox("", $"{ServerAddress} ist keine gültige Ip-Adresse",
											   MessageBoxButton.OK);
				await dialog.ShowMahAppsDialog();
			}

			SetAutoConnectToSettings();
		}
		private bool IsConnectPossible ()
		{
			if (ServerAddress == null)
				return false;

			return AddressIdentifier.IsIpAddressIdentifier(ServerAddress) &&
				   session.CurrentApplicationState == ApplicationState.DisconnectedFromServer;
		}
		
		private void SetAutoConnectToSettings()
		{
			if (AutoConnectOnNextStart)
			{
				localSettingsRepository.IsAutoConnectionEnabled = true;
				localSettingsRepository.AutoConnectionServerAddress = AddressIdentifier.GetIpAddressIdentifierFromString(ServerAddress);
				localSettingsRepository.AutoConnectionClientAddress = AddressIdentifier.GetIpAddressIdentifierFromString(ClientAddress);
			}
			else
			{
				localSettingsRepository.IsAutoConnectionEnabled = false;
				localSettingsRepository.AutoConnectionClientAddress = new IpV4AddressIdentifier(127, 0, 0, 1);
				localSettingsRepository.AutoConnectionServerAddress = new IpV4AddressIdentifier(127, 0, 0, 1);
			}
		}
		
		private void DoLogin(PasswordBox passwordBox)
		{
			localSettingsRepository.LastLoggedInUserId = selectedUser.Id;

			session.TryLogin(
				selectedUser, 
				passwordBox.Password,
				errorMessage =>
				{
					Application.Current.Dispatcher.Invoke(async () =>
					{
						var dialog = new UserDialogBox("",
													   $"Login nicht möglich:\n>> {errorMessage} <<",
													   MessageBoxButton.OK);
						await dialog.ShowMahAppsDialog();
					});
				});

			passwordBox.Password = "";
		}
		private bool IsLoginPossible (PasswordBox passwordBox)
		{			
			return session.CurrentApplicationState == ApplicationState.ConnectedButNotLoggedIn &&
				   SelectedUser != null;
		}

		public ICommand Login        { get; }
		public ICommand Connect      { get; }
		public ICommand DebugConnect { get; }
		public ICommand Disconnect   { get; }

		public ObservableCollection<ClientUserData> AvailableUsers    { get; }
		public ObservableCollection<string>         ClientIpAddresses { get; }
		
		public ClientUserData SelectedUser
		{
			get { return selectedUser; }
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref selectedUser, value);
				((ParameterrizedCommand<PasswordBox>)Login).RaiseCanExecuteChanged();
			}
		}		

		public string ServerAddress
		{
			get { return serverAddress; }
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref serverAddress, value);
				((Command)Connect).RaiseCanExecuteChanged();
				((Command)DebugConnect).RaiseCanExecuteChanged();
			}
		}

		public string ClientAddress
		{
			get { return clientAddress; }
			set { PropertyChanged.ChangeAndNotify(this, ref clientAddress, value); }
		}
		public bool AreConnectionSettingsVisible
		{
			get { return areConnectionSettingsVisible; }
			set { PropertyChanged.ChangeAndNotify(this, ref areConnectionSettingsVisible, value); }
		}
		public bool IsUserListAvailable
		{
			get { return isUserListAvailable; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isUserListAvailable, value); }
		}
		public bool AutoConnectOnNextStart
		{
			get { return autoConnectOnNextStart; }
			set { PropertyChanged.ChangeAndNotify(this, ref autoConnectOnNextStart, value); }
		}

		protected override void CleanUp()
		{			
			session.ApplicationStateChanged -= OnApplicationStateChanged;
		}
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
