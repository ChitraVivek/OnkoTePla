using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage
{
	internal class HoursOfOpeningPageViewModelSampleData : IHoursOfOpeningPageViewModel
	{
		public HoursOfOpeningPageViewModelSampleData()
		{
			AvailableMedicalPractices = new ObservableCollection<MedPracticeDisplayData>
			{
				new MedPracticeDisplayData("practice1", Guid.NewGuid()),
				new MedPracticeDisplayData("practice2", Guid.NewGuid()),
				new MedPracticeDisplayData("practice3", Guid.NewGuid())
			};
			SelectedMedicalPractice = AvailableMedicalPractices.First();
			IsAnyPracticeAvailable = true;
			IsHoursOfOpeningSettingVisible = true;

			OpeningMondayText = OpeningTuesdayText = OpeningWednesdayText = OpeningThursdayText = 
				OpeningFridayText = OpeningSaturdayText = OpeningSundayText = "8:00";
			 
			ClosingMondayText = ClosingTuesdayText = ClosingWednesdayText = ClosingThursdayText = 
				ClosingFridayText = ClosingSaturdayText = ClosingSundayText = "16:00";

			IsOpenOnMonday = IsOpenOnTuesday = IsOpenOnWednesday = IsOpenOnThursday = IsOpenOnFriday = true;
			IsOpenOnSaturday = IsOpenOnSunday = false;

			AdditionOpenedDays = "12.03.2016\n" +
			                     "02.04.2016\n" +
			                     "30.05.2016";

			AdditionClosedDays = "22.03.2016\n" +								
								 "31.01.2016";
		}

		public ICommand ConfirmChanges => null;
		public ICommand RejectChanges  => null;

		public ObservableCollection<MedPracticeDisplayData> AvailableMedicalPractices { get; }

		public MedPracticeDisplayData SelectedMedicalPractice { get; set; }

		public bool IsHoursOfOpeningSettingVisible { get; }
		public bool IsAnyPracticeAvailable { get; }

		public string OpeningMondayText    { get; set; }
		public string OpeningTuesdayText   { get; set; }
		public string OpeningWednesdayText { get; set; }
		public string OpeningThursdayText  { get; set; }
		public string OpeningFridayText    { get; set; }
		public string OpeningSaturdayText  { get; set; }
		public string OpeningSundayText    { get; set; }
		
		public string ClosingMondayText    { get; set; }
		public string ClosingTuesdayText   { get; set; }
		public string ClosingWednesdayText { get; set; }
		public string ClosingThursdayText  { get; set; }
		public string ClosingFridayText    { get; set; }
		public string ClosingSaturdayText  { get; set; }
		public string ClosingSundayText    { get; set; }
		
		public bool IsOpenOnMonday    { get; set; }
		public bool IsOpenOnTuesday   { get; set; }
		public bool IsOpenOnWednesday { get; set; }
		public bool IsOpenOnThursday  { get; set; }
		public bool IsOpenOnFriday    { get; set; }
		public bool IsOpenOnSaturday  { get; set; }
		public bool IsOpenOnSunday    { get; set; }

		public string AdditionOpenedDays { get; set; }
		public string AdditionClosedDays { get; set; }

		public void Dispose() { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}