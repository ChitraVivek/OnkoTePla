using System;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.AppointmentModification;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentViewModel
{
	internal class AppointmentViewModelBuilder : IAppointmentViewModelBuilder 
	{
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly IClientLabelRepository labelRepository;
		private readonly ICommandService commandService;
		private readonly ISharedState<ViewModels.AppointmentView.Helper.AppointmentModifications> appointmentModificationsVariable;
		private readonly ISharedState<Date> selectedDateVariable;		
		private readonly AdornerControl adornerControl;
	    private readonly IAppointmentModificationsBuilder appointmentModificationsBuilder;

	    public AppointmentViewModelBuilder(IViewModelCommunication viewModelCommunication, 	
										   IClientLabelRepository labelRepository,
										   ICommandService commandService,									   
										   ISharedState<ViewModels.AppointmentView.Helper.AppointmentModifications> appointmentModificationsVariable, 
										   ISharedState<Date> selectedDateVariable, 
										   AdornerControl adornerControl,
                                           IAppointmentModificationsBuilder appointmentModificationsBuilder)
		{
			this.viewModelCommunication = viewModelCommunication;
		    this.labelRepository = labelRepository;
		    this.commandService = commandService;
		    this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.selectedDateVariable = selectedDateVariable;			
			this.adornerControl = adornerControl;
	        this.appointmentModificationsBuilder = appointmentModificationsBuilder;
		}

		public IAppointmentViewModel Build (Appointment appointment, AggregateIdentifier location, Action<string> errorCallback)
		{			
            var editDescriptionWindowBuilder = new EditDescriptionWindowBuilder(appointment, 
																				labelRepository,																				
																				appointmentModificationsVariable,
																				errorCallback);
			
			return new ViewModels.AppointmentView.AppointmentViewModel(appointment,
																	   commandService,
																	   viewModelCommunication,
																	   new TherapyPlaceRowIdentifier(location, appointment.TherapyPlace.Id),
																	   appointmentModificationsVariable,
																	   selectedDateVariable,
																	   appointmentModificationsBuilder,
																	   editDescriptionWindowBuilder,
																	   adornerControl,
																	   errorCallback);
		}
	}
}
