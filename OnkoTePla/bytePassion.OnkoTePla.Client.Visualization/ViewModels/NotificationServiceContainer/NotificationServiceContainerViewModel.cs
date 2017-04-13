using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer
{
	internal class NotificationServiceContainerViewModel : ViewModel, 
                                                         INotificationServiceContainerViewModel
	{

		private readonly IViewModelCommunication viewModelCommunication;
		private readonly IDictionary<Guid, INotificationViewModel> notifications;
				

		public NotificationServiceContainerViewModel(IViewModelCommunication viewModelCommunication)
		{
			this.viewModelCommunication = viewModelCommunication;

			CurrentVisibleNotifications = new ObservableCollection<INotificationViewModel>();
			notifications = new Dictionary<Guid, INotificationViewModel>();	
			
			viewModelCommunication.RegisterViewModelMessageHandler<ShowNotification>(this);
			viewModelCommunication.RegisterViewModelMessageHandler<HideNotification>(this);
		}

		public void Process(ShowNotification message)
		{
			var notificationId = Guid.NewGuid();
			var notificationViewModel = new NotificationViewModel(message.NotificationMessage, 
																  notificationId, 
																  viewModelCommunication);

			if (message.SecondsToShow > 0)
			{		
				Application.Current.Dispatcher.DelayInvoke(
					() => viewModelCommunication.Send(new HideNotification(notificationId)), 
					TimeSpan.FromSeconds(message.SecondsToShow)
				);		
			}
			
			notifications.Add(notificationId, notificationViewModel);
			CurrentVisibleNotifications.Add(notificationViewModel);
		}


		public void Process(HideNotification message)
		{
			if (notifications.ContainsKey(message.NotificationId))
			{
				var notificationViewModelToHide = notifications[message.NotificationId];
				CurrentVisibleNotifications.Remove(notificationViewModelToHide);
			}
		}

		public ObservableCollection<INotificationViewModel> CurrentVisibleNotifications { get; }
		
	    protected override void CleanUp()
	    {
            viewModelCommunication.DeregisterViewModelMessageHandler<ShowNotification>(this);
            viewModelCommunication.DeregisterViewModelMessageHandler<HideNotification>(this);
        }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
