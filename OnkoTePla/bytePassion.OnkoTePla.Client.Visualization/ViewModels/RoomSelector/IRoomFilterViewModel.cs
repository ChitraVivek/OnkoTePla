using System.Collections.ObjectModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector
{
	public interface IRoomFilterViewModel : IViewModel
	{
		ObservableCollection<RoomSelectorData> AvailableRoomFilters { get; }

		RoomSelectorData SelectedRoomFilter { get; set; }


	}
}
