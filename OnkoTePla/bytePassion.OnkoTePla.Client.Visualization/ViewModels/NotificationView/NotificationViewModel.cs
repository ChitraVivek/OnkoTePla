using System;
using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.NotificationView
{
	public class NotificationViewModel : ViewModel, 
                                         INotificationViewModel
	{
		
		public NotificationViewModel(string message,
									 Guid notificationId,
									 IViewModelCommunication viewModelCommunication)
		{			
			Message = message;

			HideNotification = new Command(() =>
			{
				viewModelCommunication.Send(new HideNotification(notificationId));	                               
			});
		}

		public string   Message          { get; }
		public ICommand HideNotification { get; }

        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
	}
}
