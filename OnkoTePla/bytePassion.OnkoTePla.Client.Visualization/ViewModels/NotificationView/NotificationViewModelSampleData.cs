using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView
{
	public class NotificationViewModelSampleData : INotificationViewModel
	{
		public NotificationViewModelSampleData()
		{
			Message = "this is a very important notification";
		}

		public string Message { get; }

		public ICommand HideNotification { get; } = null;
	}
}
