using System;
using System.ComponentModel;
using System.Windows.Media;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper
{
	public class RoomDisplayData : INotifyPropertyChanged
	{
		private Color displayedColor;
		private string name;

		public RoomDisplayData(string name, Color displayedColor, Guid roomId)
		{
			Name = name;
			DisplayedColor = displayedColor;
			RoomId = roomId;
		}

		public string Name
		{
			get { return name; }
			set { PropertyChanged.ChangeAndNotify(this, ref name, value); }
		}

		public Color DisplayedColor
		{
			get { return displayedColor; }
			set { PropertyChanged.ChangeAndNotify(this, ref displayedColor, value); }
		}

		public Guid RoomId { get; }
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
