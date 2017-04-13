using System;
using System.ComponentModel;
using System.Windows.Media;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper
{
	public class TherapyPlaceDisplayData : INotifyPropertyChanged
	{
		private string name;
		private string typeName;
		private ImageSource typeIcon;

		public TherapyPlaceDisplayData(string name, string typeName, ImageSource typeIcon, Guid placeId, Color roomColor)
		{
			Name = name;
			TypeName = typeName;
			TypeIcon = typeIcon;
			PlaceId = placeId;
			RoomColor = roomColor;
		}
		
		public string Name
		{
			get { return name; }
			set { PropertyChanged.ChangeAndNotify(this, ref name, value); }
		}

		public string TypeName
		{
			get { return typeName; }
			set { PropertyChanged.ChangeAndNotify(this, ref typeName, value); }
		}

		public Color RoomColor { get; }

		public ImageSource TypeIcon
		{
			get { return typeIcon; }
			set { PropertyChanged.ChangeAndNotify(this, ref typeIcon, value); }
		}

		public Guid PlaceId { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
