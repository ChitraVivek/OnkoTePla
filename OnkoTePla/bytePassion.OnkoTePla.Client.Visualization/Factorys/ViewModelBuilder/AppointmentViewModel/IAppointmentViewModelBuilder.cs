using System;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentViewModel
{
	internal interface IAppointmentViewModelBuilder
	{
		IAppointmentViewModel Build (Appointment appointment, AggregateIdentifier location, Action<string> errorCallback);
	}
}