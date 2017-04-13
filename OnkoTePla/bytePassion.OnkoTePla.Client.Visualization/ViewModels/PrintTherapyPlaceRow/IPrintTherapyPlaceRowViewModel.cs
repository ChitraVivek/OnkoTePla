using System.Collections.ObjectModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow
{
	internal interface IPrintTherapyPlaceRowViewModel : IViewModel
	{
		ObservableCollection<IPrintAppointmentViewModel> AppointmentViewModels { get; }
		
		string TherapyPlaceName { get; }	

		Time TimeSlotBegin { get; }
		Time TimeSlotEnd { get; }
		double GridWidth { get; set; }
	}
}