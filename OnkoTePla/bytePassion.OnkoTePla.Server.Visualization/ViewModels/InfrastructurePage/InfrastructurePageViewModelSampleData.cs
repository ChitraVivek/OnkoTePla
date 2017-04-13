using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage
{
	internal class InfrastructurePageViewModelSampleData : IInfrastructurePageViewModel
    {
	    public InfrastructurePageViewModelSampleData()
	    {
			const string iconBasePath = "pack://application:,,,/bytePassion.OnkoTePla.Resources;component/Icons/TherapyPlaceType/";

			MedicalPractices = new ObservableCollection<MedPracticeDisplayData>
			{
				new MedPracticeDisplayData("practice1", Guid.NewGuid()),
				new MedPracticeDisplayData("practice1", Guid.NewGuid()),
				new MedPracticeDisplayData("practice1", Guid.NewGuid())
			};

			Rooms = new ObservableCollection<RoomDisplayData>
			{
				new RoomDisplayData("room1", Colors.Aqua,      Guid.NewGuid()),
				new RoomDisplayData("room2", Colors.BurlyWood, Guid.NewGuid())
			};

		    TherapyPlaces = new ObservableCollection<TherapyPlaceDisplayData>
		    {
				new TherapyPlaceDisplayData("place1", "stuhl", ImageLoader.LoadImage(new Uri(iconBasePath + "chair01.png")), Guid.NewGuid(), Colors.Aqua),
				new TherapyPlaceDisplayData("place2", "stuhl", ImageLoader.LoadImage(new Uri(iconBasePath + "chair01.png")), Guid.NewGuid(), Colors.Aqua),
				new TherapyPlaceDisplayData("place3", "stuhl", ImageLoader.LoadImage(new Uri(iconBasePath + "chair01.png")), Guid.NewGuid(), Colors.Aqua),
				new TherapyPlaceDisplayData("place4", "bett",  ImageLoader.LoadImage(new Uri(iconBasePath + "bed01.png")),   Guid.NewGuid(), Colors.Aqua),
				new TherapyPlaceDisplayData("place5", "stuhl", ImageLoader.LoadImage(new Uri(iconBasePath + "chair01.png")), Guid.NewGuid(), Colors.Aqua),
				new TherapyPlaceDisplayData("place6", "stuhl", ImageLoader.LoadImage(new Uri(iconBasePath + "chair01.png")), Guid.NewGuid(), Colors.Aqua)
			};

			AvailableColors = new ObservableCollection<ColorDisplayData>
			{
				new ColorDisplayData(Colors.Aqua),
				new ColorDisplayData(Colors.Blue),
				new ColorDisplayData(Colors.Fuchsia)
			};
			
			AvailableTherapyPlaceTypes = new ObservableCollection<TherapyPlaceTypeDisplayData>
			{
				new TherapyPlaceTypeDisplayData("none" , ImageLoader.LoadImage(new Uri(iconBasePath + "none.png" )), Guid.NewGuid()),
				new TherapyPlaceTypeDisplayData("type1", ImageLoader.LoadImage(new Uri(iconBasePath + "bed02.png")), Guid.NewGuid()),
				new TherapyPlaceTypeDisplayData("type2", ImageLoader.LoadImage(new Uri(iconBasePath + "bed04.png")), Guid.NewGuid())
			};

		    SelectedMedicalPractice = MedicalPractices.First();
		    SelectedRoom            = Rooms.First();
		    SelectedTherapyPlace    = TherapyPlaces.First();

		    PracticeName     = SelectedMedicalPractice.Name;
		    RoomName         = SelectedRoom.Name;
		    TherapyPlaceName = SelectedTherapyPlace.Name;

		    RoomDisplayColor = AvailableColors.First();
		    TherapyPlaceType = AvailableTherapyPlaceTypes.First();

		    IsRoomListVisible            = true;
			IsTherapyPlaceListVisible    = true;
			IsMedPracticeSettingVisible  = true;
			IsRoomSettingVisible         = true;
			IsTherapyPlaceSettingVisible = true;

		}

	    public ObservableCollection<MedPracticeDisplayData>  MedicalPractices { get; }
	    public ObservableCollection<RoomDisplayData>         Rooms            { get; }
	    public ObservableCollection<TherapyPlaceDisplayData> TherapyPlaces    { get; }

	    public ICommand AddMedicalPractice           => null;
		public ICommand SaveMedicalPracticeChanges   => null;
		public ICommand DeleteMedicalPractice        => null;
		public ICommand GenerateAppointmentsForToday => null;
		public ICommand AddRoom                      => null;
		public ICommand SaveRoomChanges              => null;
		public ICommand DeleteRoom                   => null;
		public ICommand AddTherapyPlace              => null;
		public ICommand SaveTherapyPlaceChanges      => null;
		public ICommand DeleteTherapyPlace           => null;
		
		public MedPracticeDisplayData  SelectedMedicalPractice { get; set; }
	    public RoomDisplayData         SelectedRoom            { get; set; }
	    public TherapyPlaceDisplayData SelectedTherapyPlace    { get; set; }

		public string PracticeName     { get; set; }
		public string RoomName         { get; set; }
		public string TherapyPlaceName { get; set; }
		
		public ColorDisplayData            RoomDisplayColor { get; set; }
		public TherapyPlaceTypeDisplayData TherapyPlaceType { get; set; }

		public bool IsRoomListVisible            { get; }
		public bool IsTherapyPlaceListVisible    { get; }
		public bool IsMedPracticeSettingVisible  { get; }
		public bool IsRoomSettingVisible         { get; }
		public bool IsTherapyPlaceSettingVisible { get; }

		public ObservableCollection<ColorDisplayData>            AvailableColors            { get; }
		public ObservableCollection<TherapyPlaceTypeDisplayData> AvailableTherapyPlaceTypes { get; } 

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}