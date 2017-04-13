using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage.Helper
{
	internal class ConnectedClientDisplayData : INotifyPropertyChanged
	{
		private string logginInUser;

		public ConnectedClientDisplayData(string sessionId, string connectionTime, 
										  string clientAddress, string logginInUser = "no UserLoggedIn")
		{
			SessionId = sessionId;
			ConnectionTime = connectionTime;
			ClientAddress = clientAddress;
			LogginInUser = logginInUser;
		}

		public string SessionId      { get; }
		public string ConnectionTime { get; }
		public string ClientAddress  { get; }

		public string LogginInUser
		{
			get { return logginInUser; }
			set { PropertyChanged.ChangeAndNotify(this, ref logginInUser, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}