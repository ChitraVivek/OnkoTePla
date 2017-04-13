using System.Collections.ObjectModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.GridContainer
{
	internal interface IGridContainerViewModel : IViewModel,
												 IViewModelMessageHandler<AsureDayIsLoaded>
	{
		ObservableCollection<IAppointmentGridViewModel> LoadedAppointmentGrids { get; }
		
		int CurrentDisplayedAppointmentGridIndex { get; }

		Size ReportedGridSize { set; }
	}
}
