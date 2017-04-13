using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Client.Visualization.Enums;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.ConnectionStatusView
{
	internal class ConnectionStatusViewModel : ViewModel, 
											   IConnectionStatusViewModel
    {
	    private readonly ISession session;
	    private ConnectionStatus connectionStatus;
		private string text;

		public ConnectionStatusViewModel(ISession session)
	    {
		    this.session = session;

			session.ApplicationStateChanged += OnApplicationStateChanged;
			OnApplicationStateChanged(session.CurrentApplicationState);
	    }

	    private void OnApplicationStateChanged(ApplicationState newApplicationState)
	    {
		    switch (newApplicationState)
		    {
				case ApplicationState.LoggedIn:
				case ApplicationState.ConnectedButNotLoggedIn:
			    {
				    ConnectionStatus = ConnectionStatus.Connected;
					Text="connected to server";
					break;
			    }
				case ApplicationState.DisconnectedFromServer:
			    {
				    ConnectionStatus = ConnectionStatus.Disconnected;
					Text="disconnected from server";
					break;
			    }				
				case ApplicationState.TryingToConnect:
			    {
				    ConnectionStatus = ConnectionStatus.TryToConnect;
					Text="trying to connect ...";
					break;
			    }
				case ApplicationState.TryingToDisconnect:
			    {
				    ConnectionStatus = ConnectionStatus.TryToDisconnect;
					Text="trying to disconnect ...";
					break;
			    }
		    }
	    }

	    public ConnectionStatus ConnectionStatus
	    {
		    get { return connectionStatus; }
		    private set { PropertyChanged.ChangeAndNotify(this, ref connectionStatus, value); }
	    }

		public string Text
		{
			get { return text; }
			private set { PropertyChanged.ChangeAndNotify(this, ref text, value); }
		}

		protected override void CleanUp()
        {
			session.ApplicationStateChanged -= OnApplicationStateChanged;
		}
        public override event PropertyChangedEventHandler PropertyChanged;
	    
    }

}
