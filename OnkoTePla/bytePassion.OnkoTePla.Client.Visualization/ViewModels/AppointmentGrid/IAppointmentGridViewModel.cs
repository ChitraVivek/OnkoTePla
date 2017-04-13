using System.Collections.ObjectModel;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid
{
	internal interface IAppointmentGridViewModel : IViewModel,												 
												   IViewModelCollectionItem<AggregateIdentifier>,
												   IViewModelMessageHandler<Activate>,
												   IViewModelMessageHandler<Deactivate>												 											  
	{						
		ObservableCollection<ITherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; } 	
		
		ITimeGridViewModel 	TimeGridViewModel { get; }

		bool PracticeIsClosedAtThisDay { get; }
		bool IsActive { get; }
	}
}
