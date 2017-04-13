using System;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.TherapyPlaceRowViewModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Contracts.Domain;
using Size = bytePassion.Lib.Types.SemanticTypes.Size;


namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentGridViewModel
{
	internal class AppointmentGridViewModelBuilder : IAppointmentGridViewModelBuilder 
	{				
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientReadModelRepository readModelRepository;		
		private readonly IViewModelCommunication viewModelCommunication;				
		private readonly ISharedStateReadOnly<Size> gridSizeVariable;
		private readonly ISharedStateReadOnly<Guid?> roomFilterVariable;
		private readonly ISharedStateReadOnly<Guid> displayedMedicalPracticeVariable;
		private readonly ISharedState<AppointmentModifications> appointmentModificationsVariable;
		private readonly IAppointmentViewModelBuilder appointmentViewModelBuilder;
		private readonly ITherapyPlaceRowViewModelBuilder therapyPlaceRowViewModelBuilder;

		public AppointmentGridViewModelBuilder(IClientMedicalPracticeRepository medicalPracticeRepository,
											   IClientReadModelRepository readModelRepository,
											   IViewModelCommunication viewModelCommunication,
											   ISharedStateReadOnly<Size> gridSizeVariable, 
											   ISharedStateReadOnly<Guid?> roomFilterVariable,
											   ISharedStateReadOnly<Guid> displayedMedicalPracticeVariable,
											   ISharedState<AppointmentModifications> appointmentModificationsVariable,
											   IAppointmentViewModelBuilder appointmentViewModelBuilder, 
											   ITherapyPlaceRowViewModelBuilder therapyPlaceRowViewModelBuilder)
											   
		{						
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.readModelRepository = readModelRepository;			
			this.viewModelCommunication = viewModelCommunication;						
			this.gridSizeVariable = gridSizeVariable;
			this.roomFilterVariable = roomFilterVariable;
			this.displayedMedicalPracticeVariable = displayedMedicalPracticeVariable;
			this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.appointmentViewModelBuilder = appointmentViewModelBuilder;
			this.therapyPlaceRowViewModelBuilder = therapyPlaceRowViewModelBuilder;
		}


		public void RequestBuild(Action<IAppointmentGridViewModel> viewModelAvailableCallback, AggregateIdentifier identifier, Action<string> errorCallback)
		{
			medicalPracticeRepository.RequestMedicalPractice(
				medicalPractice =>
				{
					if (medicalPractice.HoursOfOpening.IsOpen(identifier.Date))
					{
						therapyPlaceRowViewModelBuilder.RequestBuild(
							therapyPlaceRowViewModels =>
							{
								readModelRepository.RequestAppointmentsOfADayReadModel(
									readModel =>
									{
										Application.Current.Dispatcher.Invoke(() =>
										{
											foreach (var appointment in readModel.Appointments)
											{
												appointmentViewModelBuilder.Build(appointment, identifier, errorCallback);
											}

											viewModelAvailableCallback(new ViewModels.AppointmentGrid.AppointmentGridViewModel(identifier,
																															   medicalPractice,
																															   viewModelCommunication,
																															   gridSizeVariable,
																															   roomFilterVariable,
																															   displayedMedicalPracticeVariable,
																															   appointmentModificationsVariable,
																															   appointmentViewModelBuilder,
																															   readModel,
																															   therapyPlaceRowViewModels,
																															   errorCallback));
										});										
									},
									identifier,
									errorCallback
								);							
							},
							identifier,
							medicalPractice.Rooms,
							errorCallback
						);						
					}
					else
					{
						viewModelAvailableCallback(new ClosedDayGridViewModel(identifier,
																			  medicalPractice,
																			  viewModelCommunication,
																			  gridSizeVariable));
					}
				},
				identifier.MedicalPracticeId,
				identifier.Date,
				errorCallback
			);
		}
	}
}
