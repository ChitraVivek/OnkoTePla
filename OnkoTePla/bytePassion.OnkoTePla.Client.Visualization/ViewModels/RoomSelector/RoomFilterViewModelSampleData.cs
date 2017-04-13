using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector
{
	class RoomFilterViewModelSampleData : IRoomFilterViewModel
	{
		public RoomFilterViewModelSampleData()
		{
			AvailableRoomFilters = new ObservableCollection<RoomSelectorData>
			{
				new RoomSelectorData("room 01",    null, Colors.Aqua),
				new RoomSelectorData("room 02",    null, Colors.Coral),
				new RoomSelectorData("alle Räume", null, Colors.DarkGreen)
			};

			SelectedRoomFilter = AvailableRoomFilters[0];
		}

		public ObservableCollection<RoomSelectorData> AvailableRoomFilters { get; }

		public RoomSelectorData SelectedRoomFilter { get; set;  }		
		
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
