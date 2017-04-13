using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class RemoveAppointmentFromTherapyPlaceRow : ViewModelMessage
	{
		public RemoveAppointmentFromTherapyPlaceRow (AppointmentViewModel appointmentViewModelToAdd)
		{
			AppointmentViewModelToRemove = appointmentViewModelToAdd;			
		}

		public AppointmentViewModel AppointmentViewModelToRemove { get; }		
	}
}
