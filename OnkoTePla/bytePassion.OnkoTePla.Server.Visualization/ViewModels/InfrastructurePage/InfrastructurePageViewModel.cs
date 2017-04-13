using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Utils;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.SampleDataGenerators;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage
{
	public class InfrastructurePageViewModel : ViewModel, 
                                                 IInfrastructurePageViewModel
    {
		private readonly IDataCenter dataCenter;
		private readonly ISharedStateReadOnly<MainPage> selectedPageVariable;
		private readonly AppointmentGenerator appointmentGenerator;

		#region property backing fields

		private bool isRoomListVisible;
		private bool isTherapyPlaceListVisible;
		private bool isMedPracticeSettingVisible;
		private bool isRoomSettingVisible;
		private bool isTherapyPlaceSettingVisible;
		private MedPracticeDisplayData selectedMedicalPractice;
		private RoomDisplayData selectedRoom;
		private TherapyPlaceDisplayData selectedTherapyPlace;
		private string practiceName;
		private string roomName;
		private string therapyPlaceName;
		private ColorDisplayData roomDisplayColor;
		private TherapyPlaceTypeDisplayData therapyPlaceType;
		
		#endregion

		public InfrastructurePageViewModel(IDataCenter dataCenter,
										   ISharedStateReadOnly<MainPage> selectedPageVariable,
										   AppointmentGenerator appointmentGenerator)
		{
			this.dataCenter = dataCenter;
			this.selectedPageVariable = selectedPageVariable;
			this.appointmentGenerator = appointmentGenerator;

			AddMedicalPractice           = new Command(DoAddMedicalPractice);
			SaveMedicalPracticeChanges   = new Command(DoSaveMedicalPracticeChanges);
			DeleteMedicalPractice        = new Command(DoDeleteMedicalPractice);
			AddRoom                      = new Command(DoAddRoom);
			SaveRoomChanges              = new Command(DoSaveRoomChanges);
			DeleteRoom                   = new Command(DoDeleteRoom);
			AddTherapyPlace              = new Command(DoAddTherapyPlace);
			SaveTherapyPlaceChanges      = new Command(DoSaveTherapyPlaceChanges);
			DeleteTherapyPlace           = new Command(DoDeleteTherapyPlace);
			GenerateAppointmentsForToday = new Command(DoGenerateAppointments);

			MedicalPractices           = new ObservableCollection<MedPracticeDisplayData>();			
			Rooms                      = new ObservableCollection<RoomDisplayData>();
			TherapyPlaces              = new ObservableCollection<TherapyPlaceDisplayData>();
			AvailableTherapyPlaceTypes = new ObservableCollection<TherapyPlaceTypeDisplayData>();

			SelectedMedicalPractice = null;
			SelectedRoom            = null;
			SelectedTherapyPlace    = null;

			AvailableColors = typeof (Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)
											 .Select(p => (Color) p.GetValue(null, null))
											 .Select(color => new ColorDisplayData(color))
											 .ToObservableCollection();
			
			selectedPageVariable.StateChanged += OnSelectedPageStateChanged;
		}		

		private MedicalPractice SelectedMedicalPracticeObject { get; set; }
		private Room            SelectedRoomObject            { get; set; }
		private TherapyPlace    SelectedTherapyPlaceObject    { get; set; }

		public ObservableCollection<MedPracticeDisplayData>  MedicalPractices { get; }
	    public ObservableCollection<RoomDisplayData>         Rooms            { get; }
	    public ObservableCollection<TherapyPlaceDisplayData> TherapyPlaces    { get; }

		public ObservableCollection<ColorDisplayData>            AvailableColors            { get; }
		public ObservableCollection<TherapyPlaceTypeDisplayData> AvailableTherapyPlaceTypes { get; }		

		#region OnSelectedPageStateChanged

		private void OnSelectedPageStateChanged (MainPage mainPage)
		{
			if (mainPage == MainPage.Infrastructure)
			{
				AvailableTherapyPlaceTypes.Clear();

				dataCenter.GetAllTherapyPlaceTypesPlusDummy()
						  .Select(placeType => new TherapyPlaceTypeDisplayData(placeType.Name, 
																			   GetIconForTherapyPlaceType(placeType.IconType), 
																			   placeType.Id))
						  .Do(AvailableTherapyPlaceTypes.Add);

				MedicalPractices.Clear();

				dataCenter.GetAllMedicalPractices()
						  .Select(practice => new MedPracticeDisplayData(practice.Name, practice.Id))
						  .Do(MedicalPractices.Add);

				SelectedMedicalPractice = null;
			}
		}

		#endregion

		#region Update-Helper

		private void UpdateMedicalPractice(MedicalPractice updatedMedicalPractice)
		{			
			var practiceListItem = MedicalPractices.First(listItem => listItem.Id == updatedMedicalPractice.Id);
			practiceListItem.Name = updatedMedicalPractice.Name;

			dataCenter.UpdateMedicalPractice(updatedMedicalPractice);
			SelectedMedicalPracticeObject = updatedMedicalPractice;
		}

		private void UpdateRoom(Room updatedRoom)
		{
			var roomListItem = Rooms.First(listItem => listItem.RoomId == updatedRoom.Id);
			roomListItem.Name = updatedRoom.Name;
			roomListItem.DisplayedColor = updatedRoom.DisplayedColor;

			SelectedRoomObject = updatedRoom;

			var updatedPractice = SelectedMedicalPracticeObject.UpdateRoom(updatedRoom);
			UpdateMedicalPractice(updatedPractice);
		}

		private void UpdateTherapyPlace(TherapyPlace updatedTherapyPlace)
		{
			var placeListItem = TherapyPlaces.First(listItem => listItem.PlaceId == updatedTherapyPlace.Id);
			var placeType = dataCenter.GetTherapyPlaceType(updatedTherapyPlace.TypeId);
			placeListItem.Name = updatedTherapyPlace.Name;
			placeListItem.TypeIcon = GetIconForTherapyPlaceType(placeType.IconType);
			placeListItem.TypeName = placeType.Name;

			SelectedTherapyPlaceObject = updatedTherapyPlace;

			var updatedRoom = SelectedRoomObject.UpdateTherapyPlace(updatedTherapyPlace);
			UpdateRoom(updatedRoom);
		}

		#endregion

		#region command executing methods

		private void DoGenerateAppointments ()
		{
			appointmentGenerator.NewAppointments(SelectedMedicalPracticeObject.Id, TimeTools.Today());
			MessageBox.Show("appointment-generation finished");
		}

		private void DoAddMedicalPractice ()
		{
			var newPractice = MedicalPracticeCreateAndEditLogic.Create("noName");
			var newPracticeListItem = new MedPracticeDisplayData(newPractice.Name, newPractice.Id);

			MedicalPractices.Add(newPracticeListItem);
			dataCenter.AddNewMedicalPractice(newPractice);

			SelectedMedicalPractice = newPracticeListItem;
		}

		private void DoSaveMedicalPracticeChanges ()
		{
			if (!NameChecker.CheckName(PracticeName))
			{
				NameChecker.ShowCharacterError(PracticeName);
				return;
			}

			if (PracticeName != SelectedMedicalPractice.Name)
			{
				var updatedPractice = SelectedMedicalPracticeObject.SetNewName(PracticeName);
				UpdateMedicalPractice(updatedPractice);				
			}

			SelectedMedicalPractice = null;
		}

		private async void DoDeleteMedicalPractice()
		{
			var dialog = new UserDialogBox("",
										   $"Praxis [{SelectedMedicalPractice.Name}] wirklich löschen?",
                                           MessageBoxButton.OKCancel);

			var result = await dialog.ShowMahAppsDialog();

			if (result == MessageDialogResult.Affirmative)
			{
				var practiceToDelete = SelectedMedicalPracticeObject;

				dataCenter.RemoveMedicalPractice(practiceToDelete);
				MedicalPractices.Remove(SelectedMedicalPractice);

				foreach (var user in dataCenter.GetAllUsers())
				{
					if (user.ListOfAccessableMedicalPractices.Contains(practiceToDelete.Id))
					{
						var updatedUser = user.UpdateListOfAccessableMedicalPractices(
							user.ListOfAccessableMedicalPractices.Where(medPracId => medPracId != practiceToDelete.Id)
																 .ToList()
						);
						dataCenter.UpdateUser(updatedUser);
					}
				}

				SelectedMedicalPractice = null;
			}			
		}

		private void DoAddRoom ()
		{
			var newRoom = RoomCreateAndEditLogic.Create("noName");
			var newRoomListItem = new RoomDisplayData(newRoom.Name, newRoom.DisplayedColor, newRoom.Id);

			Rooms.Add(newRoomListItem);
			var updatedPractice = SelectedMedicalPracticeObject.AddRoom(newRoom);
			UpdateMedicalPractice(updatedPractice);

			SelectedRoom = newRoomListItem;
		}

		private void DoSaveRoomChanges ()
		{
			if (!NameChecker.CheckName(RoomName))
			{
				NameChecker.ShowCharacterError(RoomName);
				return;
			}

			if (RoomName != SelectedRoomObject.Name || RoomDisplayColor.Color != SelectedRoomObject.DisplayedColor)
			{
				var updatedRoom = SelectedRoomObject.SetNewName(RoomName)
											        .SetNewDisplayedColor(RoomDisplayColor.Color);
				UpdateRoom(updatedRoom);
			}

			SelectedRoom = null;
		}

		private async void DoDeleteRoom ()
		{
			var dialog = new UserDialogBox("",
										   $"Raum [{SelectedRoom.Name}] wirklich löschen?",
										   MessageBoxButton.OKCancel);

			var result = await dialog.ShowMahAppsDialog();

			if (result == MessageDialogResult.Affirmative)
			{
				var roomToDelete = SelectedRoomObject;

				var updatedPractice = SelectedMedicalPracticeObject.RemoveRoom(roomToDelete.Id);
				UpdateMedicalPractice(updatedPractice);
				Rooms.Remove(SelectedRoom);

				SelectedRoom = null;
			}
		}

		private void DoAddTherapyPlace ()
		{
			var newTherapyPlace = TherapyPlaceCreateAndEditLogic.Create("noName");
			var placeType = dataCenter.GetTherapyPlaceType(newTherapyPlace.TypeId);
			var newTherapyPlaceListItem = new TherapyPlaceDisplayData(newTherapyPlace.Name,
																	  placeType.ToString(),
																	  GetIconForTherapyPlaceType(placeType.IconType),
																	  newTherapyPlace.Id,
																	  SelectedRoom.DisplayedColor);

			TherapyPlaces.Add(newTherapyPlaceListItem);
			var updatedRoom = SelectedRoomObject.AddTherapyPlace(newTherapyPlace);
			UpdateRoom(updatedRoom);

			SelectedTherapyPlace = newTherapyPlaceListItem;
		}

		private void DoSaveTherapyPlaceChanges ()
		{
			if (!NameChecker.CheckName(TherapyPlaceName))
			{
				NameChecker.ShowCharacterError(TherapyPlaceName);
				return;
			}

			if (TherapyPlaceName != SelectedTherapyPlaceObject.Name || TherapyPlaceType.Id != SelectedTherapyPlaceObject.TypeId)
			{
				var updatedTherapyPlace = SelectedTherapyPlaceObject.SetNewName(TherapyPlaceName)
															        .SetNewType(TherapyPlaceType.Id);
				UpdateTherapyPlace(updatedTherapyPlace);
			}

			SelectedTherapyPlace = null;
		}

		private async void DoDeleteTherapyPlace()
		{
			var dialog = new UserDialogBox("",
										   $"Therapyplatz [{SelectedTherapyPlace.Name}] wirklich löschen?",
										   MessageBoxButton.OKCancel);

			var result = await dialog.ShowMahAppsDialog();

			if (result == MessageDialogResult.Affirmative)
			{
				var therapyPlaceToDelete = SelectedTherapyPlace;

				var updatedRoom = SelectedRoomObject.RemoveTherapyPlace(therapyPlaceToDelete.PlaceId);
				UpdateRoom(updatedRoom);
				TherapyPlaces.Remove(SelectedTherapyPlace);

				SelectedTherapyPlace = null;
			}
		}
		
		#endregion		

		#region commands

		public ICommand AddMedicalPractice           { get; }
		public ICommand SaveMedicalPracticeChanges   { get; }
		public ICommand DeleteMedicalPractice        { get; }
		public ICommand GenerateAppointmentsForToday { get; }

		public ICommand AddRoom         { get; }
		public ICommand SaveRoomChanges { get; }
		public ICommand DeleteRoom      { get; }

		public ICommand AddTherapyPlace         { get; }
		public ICommand SaveTherapyPlaceChanges { get; }
		public ICommand DeleteTherapyPlace      { get; }

		#endregion

		#region selected items

		public MedPracticeDisplayData SelectedMedicalPractice
		{
			get { return selectedMedicalPractice; }
			set
			{
				if (value != null && value != selectedMedicalPractice)
				{
					SelectedMedicalPracticeObject = dataCenter.GetMedicalPractice(value.Id);

					IsRoomListVisible           = true;
					IsMedPracticeSettingVisible = true;

					Rooms.Clear();

					SelectedMedicalPracticeObject.Rooms
												 .Select(room => new RoomDisplayData(room.Name, room.DisplayedColor, room.Id))
												 .Do(Rooms.Add);
					
					PracticeName = value.Name;
				}

				if (value == null)
				{
					SelectedRoom = null;					

					IsRoomListVisible            = false;					
					IsMedPracticeSettingVisible  = false;										
				}

				PropertyChanged.ChangeAndNotify(this, ref selectedMedicalPractice, value);
			}
		}

		public RoomDisplayData SelectedRoom
		{
			get { return selectedRoom; }
			set
			{
				if (value != null && value != selectedRoom)
				{
					SelectedRoomObject = SelectedMedicalPracticeObject.GetRoomById(value.RoomId);

					IsTherapyPlaceListVisible = true;
					IsRoomSettingVisible      = true;

					TherapyPlaces.Clear();					

					SelectedRoomObject.TherapyPlaces
									  .Select(therapyPlace => new TherapyPlaceDisplayData(therapyPlace.Name,
																						  dataCenter.GetTherapyPlaceType(therapyPlace.TypeId).ToString(),
																						  GetIconForTherapyPlaceType(dataCenter.GetTherapyPlaceType(therapyPlace.TypeId).IconType),
																						  therapyPlace.Id,
																						  value.DisplayedColor))
									  .Do(TherapyPlaces.Add);
					
					RoomName         = value.Name;					
					RoomDisplayColor = AvailableColors.First(colorData => colorData.Color == SelectedRoomObject.DisplayedColor);
				}

				if (value == null)
				{
					SelectedTherapyPlace = null;
								
					IsTherapyPlaceListVisible    = false;					
					IsRoomSettingVisible         = false;					
				}

				PropertyChanged.ChangeAndNotify(this, ref selectedRoom, value);
			}
		}

		public TherapyPlaceDisplayData SelectedTherapyPlace
		{
			get { return selectedTherapyPlace; }
			set
			{
				if (value != null && value != selectedTherapyPlace)
				{
					SelectedTherapyPlaceObject = SelectedRoomObject.TherapyPlaces.First(therapyPlace => therapyPlace.Id == value.PlaceId);
								
					IsTherapyPlaceSettingVisible = true;

					TherapyPlaceName = value.Name;
					TherapyPlaceType = AvailableTherapyPlaceTypes.First(type => type.Id == SelectedTherapyPlaceObject.TypeId);
				}

				if (value == null)
				{					
					IsTherapyPlaceSettingVisible = false;
				}

				PropertyChanged.ChangeAndNotify(this, ref selectedTherapyPlace, value);
			}
		}

		#endregion

		#region editable fields

		public string PracticeName
		{
			get { return practiceName; }
			set { PropertyChanged.ChangeAndNotify(this, ref practiceName, value); }
		}

		public string RoomName
		{
			get { return roomName; }
			set { PropertyChanged.ChangeAndNotify(this, ref roomName, value); }
		}

		public string TherapyPlaceName
		{
			get { return therapyPlaceName; }
			set { PropertyChanged.ChangeAndNotify(this, ref therapyPlaceName, value); }
		}

		public ColorDisplayData RoomDisplayColor
		{
			get { return roomDisplayColor; }
			set { PropertyChanged.ChangeAndNotify(this, ref roomDisplayColor, value); }
		}

		public TherapyPlaceTypeDisplayData TherapyPlaceType
		{
			get { return therapyPlaceType; }
			set { PropertyChanged.ChangeAndNotify(this, ref therapyPlaceType, value); }
		}

		#endregion

		#region visibility-variables

		public bool IsRoomListVisible
		{
			get { return isRoomListVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isRoomListVisible, value); }
		}

		public bool IsTherapyPlaceListVisible
		{
			get { return isTherapyPlaceListVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isTherapyPlaceListVisible, value); }
		}

		public bool IsMedPracticeSettingVisible
		{
			get { return isMedPracticeSettingVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isMedPracticeSettingVisible, value); }
		}

		public bool IsRoomSettingVisible
		{
			get { return isRoomSettingVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isRoomSettingVisible, value); }
		}

		public bool IsTherapyPlaceSettingVisible
		{
			get { return isTherapyPlaceSettingVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isTherapyPlaceSettingVisible, value); }
		}

		#endregion
		

		private static ImageSource GetIconForTherapyPlaceType(TherapyPlaceTypeIcon iconType)
		{
			const string basePath = "pack://application:,,,/bytePassion.OnkoTePla.Resources;component/Icons/TherapyPlaceType/";

			switch (iconType)
			{
				case TherapyPlaceTypeIcon.BedType1:   return ImageLoader.LoadImage(new Uri(basePath + "bed01.png"));
				case TherapyPlaceTypeIcon.BedType2:   return ImageLoader.LoadImage(new Uri(basePath + "bed02.png"));
				case TherapyPlaceTypeIcon.BedType3:   return ImageLoader.LoadImage(new Uri(basePath + "bed03.png"));
				case TherapyPlaceTypeIcon.BedType4:   return ImageLoader.LoadImage(new Uri(basePath + "bed04.png"));
				case TherapyPlaceTypeIcon.BedType5:   return ImageLoader.LoadImage(new Uri(basePath + "bed05.png"));
				case TherapyPlaceTypeIcon.ChairType1: return ImageLoader.LoadImage(new Uri(basePath + "chair01.png"));
				case TherapyPlaceTypeIcon.ChairType2: return ImageLoader.LoadImage(new Uri(basePath + "chair02.png"));
				case TherapyPlaceTypeIcon.ChairType3: return ImageLoader.LoadImage(new Uri(basePath + "chair03.png"));
				case TherapyPlaceTypeIcon.ChairType4: return ImageLoader.LoadImage(new Uri(basePath + "chair04.png"));
				case TherapyPlaceTypeIcon.ChairType5: return ImageLoader.LoadImage(new Uri(basePath + "chair05.png"));
				case TherapyPlaceTypeIcon.None:       return ImageLoader.LoadImage(new Uri(basePath + "none.png"));

				default:
					throw new ArgumentException();
			}			
		}

		protected override void CleanUp()
		{
			selectedPageVariable.StateChanged -= OnSelectedPageStateChanged;
		}

		public override event PropertyChangedEventHandler PropertyChanged;
    }
}
