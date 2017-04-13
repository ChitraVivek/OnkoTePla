using System.Collections.ObjectModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationServiceContainer
{

	internal interface INotificationServiceContainerViewModel : IViewModel,
															  IViewModelMessageHandler<ShowNotification>,
															  IViewModelMessageHandler<HideNotification>
	{
		ObservableCollection<INotificationViewModel> CurrentVisibleNotifications { get; } 
	}
}