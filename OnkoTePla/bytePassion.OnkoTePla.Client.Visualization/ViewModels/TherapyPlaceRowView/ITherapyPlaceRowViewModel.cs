using System.Collections.ObjectModel;
using System.Windows.Media;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView
{
	internal interface ITherapyPlaceRowViewModel : IViewModel,
												   IViewModelCollectionItem<TherapyPlaceRowIdentifier>,												   
												   IViewModelMessageHandler<NewSizeAvailable>,
												   IViewModelMessageHandler<AddAppointmentToTherapyPlaceRow>,
												   IViewModelMessageHandler<RemoveAppointmentFromTherapyPlaceRow>,
												   IViewModelMessageHandler<SetVisibility>
	{		
		ObservableCollection<IAppointmentViewModel> AppointmentViewModels { get; }				
		
		string      TherapyPlaceName { get; }
		Color       RoomColor        { get; }
		ImageSource PlaceTypeIcon    { get; }	
		
		Time TimeSlotBegin { get; }		
		Time TimeSlotEnd   { get; }	

		double                   GridWidth                        { get; }
		AppointmentModifications AppointmentModifications         { get; }
		AdornerControl           AdornerControl                   { get; }		
		
		bool IsVisible { get; }	
	}
}