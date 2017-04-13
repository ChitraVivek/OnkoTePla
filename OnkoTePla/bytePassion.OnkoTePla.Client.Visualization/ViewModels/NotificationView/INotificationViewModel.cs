using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView
{
	public interface INotificationViewModel
	{
		string Message { get; }

		ICommand HideNotification { get; }
	}
}