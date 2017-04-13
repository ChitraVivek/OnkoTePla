using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.TherapyPlaceRowViewModel
{
	internal class TherapyPlaceRowViewModelBuilder : ITherapyPlaceRowViewModelBuilder
	{
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository;
		private readonly AdornerControl adornerControl;
		private readonly ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable;
		private readonly ISharedStateReadOnly<Size> appointmentGridSizeVariable;

		public TherapyPlaceRowViewModelBuilder(IViewModelCommunication viewModelCommunication,
											   IClientMedicalPracticeRepository medicalPracticeRepository, 
											   IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository,
											   AdornerControl adornerControl, 
											   ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable,
											   ISharedStateReadOnly<Size> appointmentGridSizeVariable)
		{
			this.viewModelCommunication = viewModelCommunication;
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.therapyPlaceTypeRepository = therapyPlaceTypeRepository;

			this.adornerControl = adornerControl;
			this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.appointmentGridSizeVariable = appointmentGridSizeVariable;
		}		
		
		public void RequestBuild(Action<IEnumerable<ITherapyPlaceRowViewModel>> viewModelsAvailable, 
								 AggregateIdentifier identifier, IEnumerable<Room> rooms, 
								 Action<string> errorCallback)
		{
			medicalPracticeRepository.RequestMedicalPractice(
				practice =>
				{
					viewModelsAvailable(
						(
							from room in rooms
							from therapyPlace in room.TherapyPlaces

							let location = new TherapyPlaceRowIdentifier(identifier, therapyPlace.Id)
							let hoursOfOpeing = practice.HoursOfOpening

							select new ViewModels.TherapyPlaceRowView.TherapyPlaceRowViewModel(
								viewModelCommunication,
								therapyPlaceTypeRepository,
								therapyPlace,
								room.DisplayedColor,
								location,
								adornerControl,
								hoursOfOpeing.GetOpeningTime(location.PlaceAndDate.Date),
								hoursOfOpeing.GetClosingTime(location.PlaceAndDate.Date),
								appointmentModificationsVariable,
								appointmentGridSizeVariable.Value.Width,
								errorCallback)
						).ToList()
					);
				},
				identifier.MedicalPracticeId,
				identifier.PracticeVersion,
				errorCallback	
			);
		}
	}
}