using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.OnkoTePla.Contracts.Config;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView
{
	internal class LoginViewModelSampleData : ILoginViewModel
    {
	    public LoginViewModelSampleData()
	    {
		    AvailableUsers = new ObservableCollection<ClientUserData>
		    {
			    new ClientUserData("user1", Guid.NewGuid(), null),
				new ClientUserData("user2", Guid.NewGuid(), null)
			};

			ClientIpAddresses = new ObservableCollection<string>
			{
				"192.168.128.14",
				"192.168.128.13"
			};

		    SelectedUser = AvailableUsers.First();
		    ServerAddress = "192.168.128.12";
			ClientAddress = "192.168.128.13";

			AutoConnectOnNextStart = true;
		    AreConnectionSettingsVisible = true;
		    IsUserListAvailable = true;
	    }

	    public ICommand Login      => null;
	    public ICommand Connect    => null;		
		public ICommand Disconnect => null;

	    public ObservableCollection<ClientUserData> AvailableUsers { get; }
		public ObservableCollection<string>         ClientIpAddresses { get; }

		public ClientUserData SelectedUser  { get; set; }	   
	    public string         ServerAddress { get; set; }
		public string         ClientAddress { get; set; }

		public bool AreConnectionSettingsVisible { get; set; }
		public bool IsUserListAvailable          { get; }
		public bool AutoConnectOnNextStart       { get; set; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}