using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage
{
	public interface IConnectionsPageViewModel : IViewModel
    {
		ICommand UpdateAvailableAddresses { get; }
		
		ICommand ActivateConnection   { get; }
		ICommand DeactivateConnection { get; }
		
		bool IsConnectionActive   { get; }
		bool IsActivationPossible { get; }

		string SelectedIpAddress { get; set; }
	    string ActiveConnection  { get; }

		ObservableCollection<string> AvailableIpAddresses { get; }
		ObservableCollection<ConnectedClientDisplayData> ConnectedClients { get; }
    }
}