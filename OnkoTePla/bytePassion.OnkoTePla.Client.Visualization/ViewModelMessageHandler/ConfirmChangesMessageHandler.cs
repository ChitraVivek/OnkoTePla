using System;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessageHandler
{
	internal class ConfirmChangesMessageHandler : IViewModelMessageHandler<ConfirmChanges>
	{
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly ICommandService commandService;
		private readonly ISharedState<AppointmentModifications> appointmentModificationsVariable;
		private readonly Action<string> errorCallback;

		public ConfirmChangesMessageHandler (IViewModelCommunication viewModelCommunication, 
											 ICommandService commandService,
                                             ISharedState<AppointmentModifications> appointmentModificationsVariable,
											 Action<string> errorCallback)
		{
		    this.viewModelCommunication = viewModelCommunication;
			this.commandService = commandService;
			this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.errorCallback = errorCallback;
		}

	    public void Process(ConfirmChanges message)
		{			
			var currentAppointmentModification = appointmentModificationsVariable.Value;

			if (currentAppointmentModification.IsInitialAdjustment)
			{
				
				
				commandService.TryAddNewAppointment(
					operationSuccessful =>
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							if (operationSuccessful)
							{
								viewModelCommunication.SendTo(
									Constants.ViewModelCollections.AppointmentViewModelCollection,
									currentAppointmentModification.OriginalAppointment.Id,
									new Dispose()
								);

								currentAppointmentModification.Dispose();
								appointmentModificationsVariable.Value = null;
							}
							else
							{
								viewModelCommunication.Send(new ShowNotification("anlegen des Termins nicht möglich: versuch es noch einmal oder breche die operation ab", 5));	
							}

						});						
					},
					currentAppointmentModification.CurrentLocation.PlaceAndDate,
					currentAppointmentModification.OriginalAppointment.Patient.Id,
					currentAppointmentModification.Description,
					currentAppointmentModification.BeginTime,
					currentAppointmentModification.EndTime,
					currentAppointmentModification.CurrentLocation.TherapyPlaceId,
					currentAppointmentModification.Label.Id,
					Guid.NewGuid(),
					ActionTag.RegularAction,
					errorCallback
				);
			}
			else
			{
				if (appointmentModificationsVariable.Value == null)
					return;

				var originalAppointment = currentAppointmentModification.OriginalAppointment;

				if (originalAppointment.Description     == currentAppointmentModification.Description &&
					originalAppointment.StartTime       == currentAppointmentModification.BeginTime &&
					originalAppointment.EndTime         == currentAppointmentModification.EndTime &&
					originalAppointment.Day             == currentAppointmentModification.CurrentLocation.PlaceAndDate.Date &&
					originalAppointment.TherapyPlace.Id == currentAppointmentModification.CurrentLocation.TherapyPlaceId &&
					originalAppointment.Label.Id        == currentAppointmentModification.Label.Id)
				{

					currentAppointmentModification.Dispose();		//
					appointmentModificationsVariable.Value = null;  //
																	//	no changes to report
					return;											// 
				}

				commandService.TryReplaceAppointment(

					operationSuccessful =>
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							if (operationSuccessful)
							{
								currentAppointmentModification.Dispose();
								appointmentModificationsVariable.Value = null;
							}
							else
							{
								viewModelCommunication.Send(new ShowNotification("anlegen des Termins nicht möglich: versuch es noch einmal oder breche die operation ab", 5));

								viewModelCommunication.SendTo(
									Constants.ViewModelCollections.AppointmentViewModelCollection,
									originalAppointment.Id,
									new RestoreOriginalValues()
								);
							}
						});						
					},
					currentAppointmentModification.InitialLocation.PlaceAndDate, 
					currentAppointmentModification.CurrentLocation.PlaceAndDate,
					currentAppointmentModification.OriginalAppointment.Patient.Id,
					currentAppointmentModification.OriginalAppointment.Description,
					currentAppointmentModification.Description,
					currentAppointmentModification.InitialLocation.PlaceAndDate.Date,
					currentAppointmentModification.CurrentLocation.PlaceAndDate.Date,
					currentAppointmentModification.OriginalAppointment.StartTime,
					currentAppointmentModification.BeginTime,
					currentAppointmentModification.OriginalAppointment.EndTime,
					currentAppointmentModification.EndTime,
					currentAppointmentModification.InitialLocation.TherapyPlaceId,
					currentAppointmentModification.CurrentLocation.TherapyPlaceId,
					currentAppointmentModification.OriginalAppointment.Label.Id,
					currentAppointmentModification.Label.Id,
					currentAppointmentModification.OriginalAppointment.Id,													 
					ActionTag.RegularAction, 
					errorCallback
				);
			}
		}
	}
}
