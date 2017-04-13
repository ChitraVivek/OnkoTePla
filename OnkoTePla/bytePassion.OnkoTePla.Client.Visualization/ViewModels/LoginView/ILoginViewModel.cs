using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView
{
	internal interface ILoginViewModel : IViewModel
    {
        ICommand Login        { get; }
		ICommand Connect      { get; }		
		ICommand Disconnect   { get; }

		ObservableCollection<ClientUserData> AvailableUsers    { get; }
		ObservableCollection<string>         ClientIpAddresses { get; }

		ClientUserData SelectedUser { get; set; }
				
		string ServerAddress { get; set; }
		string ClientAddress { get; set; }

		bool AreConnectionSettingsVisible { get; set; }
		bool IsUserListAvailable          { get; }
		bool AutoConnectOnNextStart       { get; set; }
    }
}