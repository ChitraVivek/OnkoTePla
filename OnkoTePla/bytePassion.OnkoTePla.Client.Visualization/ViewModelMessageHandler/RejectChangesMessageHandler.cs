using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessageHandler
{
	internal class RejectChangesMessageHandler : IViewModelMessageHandler<RejectChanges>
	{
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly ISharedState<AppointmentModifications> appointmentModificationsVariable;

		public RejectChangesMessageHandler (IViewModelCommunication viewModelCommunication, 
                                            ISharedState<AppointmentModifications> appointmentModificationsVariable)
		{
		    this.viewModelCommunication = viewModelCommunication;
		    this.appointmentModificationsVariable = appointmentModificationsVariable;
		}

        public void Process(RejectChanges message)
		{
			ProcessMesageAsync();
		}

		private async void ProcessMesageAsync()
		{
			var currentMods = appointmentModificationsVariable.Value;
			var original = appointmentModificationsVariable.Value.OriginalAppointment;

			if (currentMods.BeginTime == original.StartTime &&
			    currentMods.EndTime == original.EndTime &&
			    currentMods.Description == original.Description &&
			    currentMods.CurrentLocation == currentMods.InitialLocation)
			{
				RejectChanges();
			}
			else
			{
				var dialog = new UserDialogBox("", "Wollen Sie alle Änderungen verwerfen?",
				                               MessageBoxButton.OKCancel);
				var result = await dialog.ShowMahAppsDialog();

				if (result == MessageDialogResult.Affirmative)
				{
					RejectChanges();
				}
			}
		}

		private void RejectChanges()
		{
			var isInitial = appointmentModificationsVariable.Value.IsInitialAdjustment;
            var originalAppointmentId = appointmentModificationsVariable.Value.OriginalAppointment.Id;

			appointmentModificationsVariable.Value.Dispose();
			appointmentModificationsVariable.Value = null;

			if (isInitial)
			{
				viewModelCommunication.SendTo(											//
					Constants.ViewModelCollections.AppointmentViewModelCollection,      // do nothing but
					originalAppointmentId,                                              // deleting the temporarly
					new Dispose()                                                       // created Appointment
				);                                                                      //
			}
			else
			{
				viewModelCommunication.SendTo(
					Constants.ViewModelCollections.AppointmentViewModelCollection,
					originalAppointmentId,
					new RestoreOriginalValues()
				);
			}		
		}
	}
}
