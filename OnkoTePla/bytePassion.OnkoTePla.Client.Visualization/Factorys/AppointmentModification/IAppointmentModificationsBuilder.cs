using System;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.AppointmentModification
{
	internal interface IAppointmentModificationsBuilder
    {
        AppointmentModifications Build(Appointment originalAppointment, Guid medicalPracticeId, 
									   bool isInitialAdjustment, Action<string> errorCallback);
    }
}
