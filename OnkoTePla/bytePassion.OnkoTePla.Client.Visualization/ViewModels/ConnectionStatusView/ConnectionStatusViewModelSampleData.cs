using System.ComponentModel;
using bytePassion.OnkoTePla.Client.Visualization.Enums;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.ConnectionStatusView
{
	internal class ConnectionStatusViewModelSampleData : IConnectionStatusViewModel
    {
        public ConnectionStatusViewModelSampleData()
        {
            ConnectionStatus = ConnectionStatus.Connected;
	        Text = "connected to server";
        }
		
		public ConnectionStatus ConnectionStatus { get; }
		public string Text { get; }

		public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;	    
    }
}