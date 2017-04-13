using System;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.EditDescriptionViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Views;
using bytePassion.OnkoTePla.Contracts.Appointments;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder
{
	internal class EditDescriptionWindowBuilder : IWindowBuilder<EditDescription>
    {
        private readonly Appointment appointmentToEdit;
		private readonly IClientLabelRepository labelRepository;
		private readonly ISharedState<AppointmentModifications> modificationsVar;
		private readonly Action<string> errorCallback;

		public EditDescriptionWindowBuilder(Appointment appointmentToEdit, 
											IClientLabelRepository labelRepository,
										    ISharedState<AppointmentModifications> modificationsVar,
											Action<string> errorCallback) 			
        {            
            this.modificationsVar = modificationsVar;
			this.errorCallback = errorCallback;
			this.appointmentToEdit = appointmentToEdit;
			this.labelRepository = labelRepository;
        }
		
        public EditDescription BuildWindow()
        {
	        var view = new EditDescription
	        {
		        DataContext = new EditDescriptionViewModel(appointmentToEdit, labelRepository, modificationsVar, errorCallback),
		        Owner = Application.Current.MainWindow
	        };
	        return view;
        }

        public void DisposeWindow(EditDescription buildedWindow)
        {
			throw new NotImplementedException();
		}
    }
}