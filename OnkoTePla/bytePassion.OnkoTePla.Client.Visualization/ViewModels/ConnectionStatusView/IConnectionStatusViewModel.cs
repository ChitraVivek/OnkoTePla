using bytePassion.OnkoTePla.Client.Visualization.Enums;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.ConnectionStatusView
{
	internal interface IConnectionStatusViewModel : IViewModel
    {
        ConnectionStatus ConnectionStatus { get; }
		string Text { get; }
    }
}