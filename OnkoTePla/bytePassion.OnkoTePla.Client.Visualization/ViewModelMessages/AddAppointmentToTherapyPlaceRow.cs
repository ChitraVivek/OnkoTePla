using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class AddAppointmentToTherapyPlaceRow : ViewModelMessage
	{
		public AddAppointmentToTherapyPlaceRow(AppointmentViewModel appointmentViewModelToAdd)
		{
			AppointmentViewModelToAdd = appointmentViewModelToAdd;			
		}

		public AppointmentViewModel AppointmentViewModelToAdd { get; }		
	}
}
