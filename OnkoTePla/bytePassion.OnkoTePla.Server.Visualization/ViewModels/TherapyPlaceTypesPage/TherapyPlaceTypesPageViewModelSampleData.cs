using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage
{
	internal class TherapyPlaceTypesPageViewModelSampleData : ITherapyPlaceTypesPageViewModel
	{
		private const string BasePath = "pack://application:,,,/bytePassion.OnkoTePla.Resources;component/Icons/TherapyPlaceType/";
			 

		public TherapyPlaceTypesPageViewModelSampleData()
		{
			AllIcons = new ObservableCollection<IconDisplayData>
			{
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "bed01.png")),   TherapyPlaceTypeIcon.BedType1,   "Bed1"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "bed02.png")),   TherapyPlaceTypeIcon.BedType2,   "Bed2"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "bed03.png")),   TherapyPlaceTypeIcon.BedType3,   "Bed3"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "bed04.png")),   TherapyPlaceTypeIcon.BedType4,   "Bed4"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "bed05.png")),   TherapyPlaceTypeIcon.BedType5,   "Bed5"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "chair01.png")), TherapyPlaceTypeIcon.ChairType1, "Chair1"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "chair02.png")), TherapyPlaceTypeIcon.ChairType2, "Chair2"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "chair03.png")), TherapyPlaceTypeIcon.ChairType3, "Chair3"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "chair04.png")), TherapyPlaceTypeIcon.ChairType4, "Chair4"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "chair05.png")), TherapyPlaceTypeIcon.ChairType5, "Chair5"),
				new IconDisplayData(ImageLoader.LoadImage(new Uri(BasePath + "none.png")),    TherapyPlaceTypeIcon.None,       "None"),
			};

			TherapyPlaceTypes = new ObservableCollection<TherapyPlaceType>
			{
				new TherapyPlaceType("example1", TherapyPlaceTypeIcon.BedType2,   Guid.NewGuid()),
				new TherapyPlaceType("example2", TherapyPlaceTypeIcon.ChairType1, Guid.NewGuid())
			};

			SelectedTherapyPlaceType = TherapyPlaceTypes.First();

			Name     = SelectedTherapyPlaceType.Name;
			IconType = AllIcons[1];

			ShowModificationView = true;			
		}

		public ICommand AddTherapyPlaceType => null;
		public ICommand SaveChanges         => null;
		public ICommand DiscardChanges      => null;

		public ObservableCollection<TherapyPlaceType> TherapyPlaceTypes { get; }

		public TherapyPlaceType SelectedTherapyPlaceType { get; set; }

		public bool ShowModificationView { get; }

		public string          Name     { get; set; }
		public IconDisplayData IconType { get; set; }

		public ObservableCollection<IconDisplayData> AllIcons { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}