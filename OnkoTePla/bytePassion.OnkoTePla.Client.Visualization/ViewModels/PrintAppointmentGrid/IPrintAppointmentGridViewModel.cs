using System.Collections.ObjectModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid
{
	internal interface IPrintAppointmentGridViewModel : IViewModel
	{
		ObservableCollection<IPrintTherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; }

		ITimeGridViewModel TimeGridViewModel { get; }
	}
}