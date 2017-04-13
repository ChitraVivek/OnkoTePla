using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.ConnectionStatusView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.ActionBar
{
	internal interface IActionBarViewModel : IViewModel
    {
		string Title { get; }

        ICommand ShowOverview { get; }
        ICommand ShowSearch   { get; }
        ICommand ShowOptions  { get; }
        ICommand Logout       { get; }
        ICommand ShowAbout    { get; }

		bool NavigationAndLogoutButtonVisibility { get; }

        IConnectionStatusViewModel ConnectionStatusViewModel { get; }
    }
}