using System;
using System.Collections.ObjectModel;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TimeGrid.Helper;
using bytePassion.OnkoTePla.Contracts.Domain;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TimeGrid
{

	internal interface ITimeGridViewModel : IViewModelCollectionItem<AggregateIdentifier>,
										  IDisposable,
										  IViewModelMessageHandler<NewSizeAvailable>,
										  IViewModelMessageHandler<Dispose>
	{
		ObservableCollection<TimeSlotLabel> TimeSlotLabels { get; }
		ObservableCollection<TimeSlotLine>  TimeSlotLines  { get; }
	}
}