﻿using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Base;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModelMessages
{
	public class AddAppointmentToTherapyPlaceRow : ViewModelMessage
	{
		public AddAppointmentToTherapyPlaceRow(AppointmentViewModel appointmentViewModelToAdd)
		{
			AppointmentViewModelToAdd = appointmentViewModelToAdd;			
		}

		public AppointmentViewModel AppointmentViewModelToAdd { get; }		
	}
}