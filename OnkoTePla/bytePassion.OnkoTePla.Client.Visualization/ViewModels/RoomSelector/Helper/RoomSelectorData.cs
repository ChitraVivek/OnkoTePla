using System;
using System.Windows.Media;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector.Helper
{
	public class RoomSelectorData
	{				
		public RoomSelectorData(string roomName, Guid? roomId, Color displayedColor)
		{
			RoomName       = roomName;
			RoomId         = roomId;
			DisplayedColor = displayedColor;
		}

		public string RoomName       { get; }
		public Guid?  RoomId         { get; }
		public Color  DisplayedColor { get; }
	}
}