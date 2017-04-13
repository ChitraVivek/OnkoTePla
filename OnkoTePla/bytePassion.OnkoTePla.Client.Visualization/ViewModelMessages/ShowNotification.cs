using bytePassion.Lib.Communication.ViewModel.Messages;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class ShowNotification : ViewModelMessage
	{
		public ShowNotification(string notificationMessage,
								int secondsToShow)
		{
			NotificationMessage = notificationMessage;
			SecondsToShow = secondsToShow;
		}

		public string NotificationMessage { get; }
		public int    SecondsToShow       { get; }
	}
}
