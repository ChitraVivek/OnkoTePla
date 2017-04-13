using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer
{

	internal class NotificationServiceContainerViewModelSampleData : INotificationServiceContainerViewModel
	{
		public NotificationServiceContainerViewModelSampleData()
		{
			CurrentVisibleNotifications = new ObservableCollection<INotificationViewModel>
			                              {
				                              new NotificationViewModelSampleData(),
				                              new NotificationViewModelSampleData(),
				                              new NotificationViewModelSampleData()
			                              };
		}

		public void Process(ShowNotification message) {}
		public void Process(HideNotification message) {}

		public ObservableCollection<INotificationViewModel> CurrentVisibleNotifications { get; }

        public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;	    
	}
}