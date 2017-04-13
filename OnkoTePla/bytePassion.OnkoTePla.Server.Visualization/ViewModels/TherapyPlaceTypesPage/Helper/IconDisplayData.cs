using System.ComponentModel;
using System.Windows.Media;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage.Helper
{
	internal class IconDisplayData : INotifyPropertyChanged
	{
		public IconDisplayData(ImageSource iconImage, TherapyPlaceTypeIcon iconType, string iconName)
		{
			IconImage = iconImage;
			IconType = iconType;
			IconName = iconName;
		}

		public ImageSource          IconImage { get; }
		public TherapyPlaceTypeIcon IconType  { get; }
		public string               IconName  { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
