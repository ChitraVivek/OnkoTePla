﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Server.WpfUi.ViewModels.InfrastructurePage.Helper;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.InfrastructurePage
{
	internal interface IInfrastructurePageViewModel : IViewModel
    {
	    ObservableCollection<MedPracticeDisplayData>     MedicalPractices { get; }
		ObservableCollection<RoomDisplayData>         Rooms            { get; } 
		ObservableCollection<TherapyPlaceDisplayData> TherapyPlaces    { get; } 
		 
		ICommand AddMedicalPractice         { get; }
		ICommand SaveMedicalPracticeChanges { get; }
		ICommand DeleteMedicalPractice      { get; }

		ICommand AddRoom         { get; }
		ICommand SaveRoomChanges { get; }
		ICommand DeleteRoom      { get; }

		ICommand AddTherapyPlace         { get; }
		ICommand SaveTherapyPlaceChanges { get; }
		ICommand DeleteTherapyPlace      { get; }

		MedPracticeDisplayData     SelectedMedicalPractice { get; set; }
		RoomDisplayData         SelectedRoom            { get; set; }
		TherapyPlaceDisplayData SelectedTherapyPlace    { get; set; }
		
		string PracticeName     { get; set; }
		string RoomName         { get; set; }
		string TherapyPlaceName { get; set; }

		ColorDisplayData            RoomDisplayColor { get; set; }
		TherapyPlaceTypeDisplayData TherapyPlaceType { get; set; }

		bool IsRoomListVisible            { get; }
		bool IsTherapyPlaceListVisible    { get; }
		bool IsMedPracticeSettingVisible  { get; }
		bool IsRoomSettingVisible         { get; }
		bool IsTherapyPlaceSettingVisible { get; }

		ObservableCollection<ColorDisplayData>            AvailableColors            { get; }
		ObservableCollection<TherapyPlaceTypeDisplayData> AvailableTherapyPlaceTypes { get; } 
		
    }
}