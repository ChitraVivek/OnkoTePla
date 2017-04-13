using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage
{
	public interface IHoursOfOpeningPageViewModel : IViewModel
	{		
		ICommand ConfirmChanges { get; }
		ICommand RejectChanges  { get; }

		ObservableCollection<MedPracticeDisplayData> AvailableMedicalPractices { get; } 

		MedPracticeDisplayData SelectedMedicalPractice { get; set; }

		bool IsHoursOfOpeningSettingVisible { get; }
		bool IsAnyPracticeAvailable { get; }

		string OpeningMondayText    { get; set; }
		string OpeningTuesdayText   { get; set; }
		string OpeningWednesdayText { get; set; }
		string OpeningThursdayText  { get; set; }
		string OpeningFridayText    { get; set; }
		string OpeningSaturdayText  { get; set; }
		string OpeningSundayText    { get; set; }

		string ClosingMondayText    { get; set; }
		string ClosingTuesdayText   { get; set; }
		string ClosingWednesdayText { get; set; }
		string ClosingThursdayText  { get; set; }
		string ClosingFridayText    { get; set; }
		string ClosingSaturdayText  { get; set; }
		string ClosingSundayText    { get; set; }

		bool IsOpenOnMonday    { get; set; }
		bool IsOpenOnTuesday   { get; set; }
		bool IsOpenOnWednesday { get; set; }
		bool IsOpenOnThursday  { get; set; }
		bool IsOpenOnFriday    { get; set; }
		bool IsOpenOnSaturday  { get; set; }
		bool IsOpenOnSunday    { get; set; }

		string AdditionOpenedDays { get; set; }
		string AdditionClosedDays { get; set; }
	}
}