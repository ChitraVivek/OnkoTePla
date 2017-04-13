using System;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentGridViewModel
{
	internal interface IAppointmentGridViewModelBuilder
	{
		void RequestBuild(Action<IAppointmentGridViewModel> viewModelAvailableCallback, AggregateIdentifier identifier, Action<string> errorCallback);
	}
}