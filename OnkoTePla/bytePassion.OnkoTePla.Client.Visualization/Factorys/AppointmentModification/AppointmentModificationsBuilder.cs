using System;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.AppointmentModification
{
	internal class AppointmentModificationsBuilder : IAppointmentModificationsBuilder
    {        
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientReadModelRepository readModelRepository;
		private readonly IViewModelCommunication viewModelCommunication;
        private readonly ISharedState<Date> selectedDateVariable;
        private readonly ISharedStateReadOnly<Size> gridSizeVariable;

        public AppointmentModificationsBuilder(IClientMedicalPracticeRepository medicalPracticeRepository,
											   IClientReadModelRepository readModelRepository,
                                               IViewModelCommunication viewModelCommunication,
                                               ISharedState<Date> selectedDateVariable,
                                               ISharedStateReadOnly<Size> gridSizeVariable)
        {            
	        this.medicalPracticeRepository = medicalPracticeRepository;
	        this.readModelRepository = readModelRepository;
	        this.viewModelCommunication = viewModelCommunication;
            this.selectedDateVariable = selectedDateVariable;
            this.gridSizeVariable = gridSizeVariable;
        }

        public AppointmentModifications Build(Appointment originalAppointment, Guid medicalPracticeId, 
											  bool isInitialAdjustment, Action<string> errorCallback)
        {
            return new AppointmentModifications(originalAppointment,
                                                medicalPracticeId,
                                                medicalPracticeRepository,
												readModelRepository,
                                                viewModelCommunication,
                                                selectedDateVariable,
                                                gridSizeVariable,
                                                isInitialAdjustment,
												errorCallback);
        }
    }
}