using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage
{
	public class HoursOfOpeningPageViewModel : ViewModel, 
												 IHoursOfOpeningPageViewModel
	{
		private readonly IDataCenter dataCenter;
		private readonly ISharedStateReadOnly<MainPage> selectedPageVariable;

		#region backing-field variables

		private bool isHoursOfOpeningSettingVisible;
		private bool isAnyPracticeAvailable;
		private MedPracticeDisplayData selectedMedicalPractice;
		private string openingMondayText;
		private string openingTuesdayText;
		private string openingWednesdayText;
		private string openingThursdayText;
		private string openingFridayText;
		private string openingSaturdayText;
		private string openingSundayText;
		private string closingMondayText;
		private string closingTuesdayText;
		private string closingWednesdayText;
		private string closingThursdayText;
		private string closingFridayText;
		private string closingSaturdayText;
		private string closingSundayText;
		private bool isOpenOnMonday;
		private bool isOpenOnTuesday;
		private bool isOpenOnWednesday;
		private bool isOpenOnThursday;
		private bool isOpenOnFriday;
		private bool isOpenOnSaturday;
		private bool isOpenOnSunday;
		private string additionOpenedDays;
		private string additionClosedDays;

		#endregion

		public HoursOfOpeningPageViewModel(IDataCenter dataCenter,
										   ISharedStateReadOnly<MainPage> selectedPageVariable)
		{
			this.dataCenter = dataCenter;
			this.selectedPageVariable = selectedPageVariable;

			AvailableMedicalPractices = new ObservableCollection<MedPracticeDisplayData>();

			ConfirmChanges = new Command(DoConfirmChanges);
			RejectChanges  = new Command(DoRejectChanges);

			IsHoursOfOpeningSettingVisible = false;
			IsAnyPracticeAvailable = false;

			selectedPageVariable.StateChanged += OnSelectedPageStateChanged;
		}

		private async void DoRejectChanges()
		{
			var dialog = new UserDialogBox("", "Änderungen verwerfen?", MessageBoxButton.OKCancel);
			var result = await dialog.ShowMahAppsDialog();

			if (result == MessageDialogResult.Affirmative)
			{
				SelectedMedicalPractice = null;
			}
		}

		private async void DoConfirmChanges()
		{			
			if (AreFieldsValid())
			{
				var currentMedicalPractice = dataCenter.GetMedicalPractice(SelectedMedicalPractice.Id);
				var updatedMedicalPractice = currentMedicalPractice.SetNewHoursOfOpening(EvaluateFields());
				dataCenter.UpdateMedicalPractice(updatedMedicalPractice);

				SelectedMedicalPractice = null;
			}
			else
			{
				var dialog = new UserDialogBox("", "Felder nicht korrekt ausgefüllt!", MessageBoxButton.OK);
				await dialog.ShowMahAppsDialog();
			}
		}		

		private void OnSelectedPageStateChanged(MainPage mainPage)
		{
			if (mainPage == MainPage.HoursOfOpening)
			{
				AvailableMedicalPractices.Clear();

				dataCenter.GetAllMedicalPractices()
						  .Select(medPractice => new MedPracticeDisplayData(medPractice.Name, medPractice.Id))
						  .Do(AvailableMedicalPractices.Add);

				IsAnyPracticeAvailable = AvailableMedicalPractices.Count > 0;
			}
		}

		public ICommand ConfirmChanges { get; }
		public ICommand RejectChanges { get; }

		public ObservableCollection<MedPracticeDisplayData> AvailableMedicalPractices { get; }

		public MedPracticeDisplayData SelectedMedicalPractice
		{
			get { return selectedMedicalPractice; }
			set
			{
				var fillFields = false;

				if (value != null && value != selectedMedicalPractice)
				{
					IsHoursOfOpeningSettingVisible = true;
					fillFields = true;
				}

				if (value == null)
				{
					IsHoursOfOpeningSettingVisible = false;
				}				

				PropertyChanged.ChangeAndNotify(this, ref selectedMedicalPractice, value);

				if (fillFields)
					FillFields();
			}
		}

		public bool IsHoursOfOpeningSettingVisible
		{
			get { return isHoursOfOpeningSettingVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isHoursOfOpeningSettingVisible, value); }
		}

		public bool IsAnyPracticeAvailable
		{
			get { return isAnyPracticeAvailable; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isAnyPracticeAvailable, value); }
		}

		#region editing fields

		public string OpeningMondayText
		{
			get { return openingMondayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingMondayText, value); }
		}

		public string OpeningTuesdayText
		{
			get { return openingTuesdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingTuesdayText, value); }
		}

		public string OpeningWednesdayText
		{
			get { return openingWednesdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingWednesdayText, value); }
		}

		public string OpeningThursdayText
		{
			get { return openingThursdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingThursdayText,value); }
		}

		public string OpeningFridayText
		{
			get { return openingFridayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingFridayText, value); }
		}

		public string OpeningSaturdayText
		{
			get { return openingSaturdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingSaturdayText, value); }
		}

		public string OpeningSundayText
		{
			get { return openingSundayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref openingSundayText, value); }
		}

		public string ClosingMondayText
		{
			get { return closingMondayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingMondayText, value); }
		}

		public string ClosingTuesdayText
		{
			get { return closingTuesdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingTuesdayText, value); }
		}

		public string ClosingWednesdayText
		{
			get { return closingWednesdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingWednesdayText, value); }
		}

		public string ClosingThursdayText
		{
			get { return closingThursdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingThursdayText, value); }
		}

		public string ClosingFridayText
		{
			get { return closingFridayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingFridayText, value); }
		}

		public string ClosingSaturdayText
		{
			get { return closingSaturdayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingSaturdayText, value); }
		}

		public string ClosingSundayText
		{
			get { return closingSundayText; }
			set { PropertyChanged.ChangeAndNotify(this, ref closingSundayText, value); }
		}

		public bool IsOpenOnMonday
		{
			get { return isOpenOnMonday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnMonday, value); }
		}

		public bool IsOpenOnTuesday
		{
			get { return isOpenOnTuesday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnTuesday, value); }
		}

		public bool IsOpenOnWednesday
		{
			get { return isOpenOnWednesday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnWednesday, value); }
		}

		public bool IsOpenOnThursday
		{
			get { return isOpenOnThursday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnThursday, value); }
		}

		public bool IsOpenOnFriday
		{
			get { return isOpenOnFriday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnFriday, value); }
		}

		public bool IsOpenOnSaturday
		{
			get { return isOpenOnSaturday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnSaturday, value); }
		}

		public bool IsOpenOnSunday
		{
			get { return isOpenOnSunday; }
			set { PropertyChanged.ChangeAndNotify(this, ref isOpenOnSunday, value); }
		}

		public string AdditionOpenedDays
		{
			get { return additionOpenedDays; }
			set { PropertyChanged.ChangeAndNotify(this, ref additionOpenedDays, value); }
		}

		public string AdditionClosedDays
		{
			get { return additionClosedDays; }
			set { PropertyChanged.ChangeAndNotify(this, ref additionClosedDays, value); }
		}

		#endregion

		#region field filling / validation / evaluation

		private void FillFields()
		{
			var currentMedicalPracticeVersion = dataCenter.GetMedicalPractice(SelectedMedicalPractice.Id);
			 
			OpeningMondayText    = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeMonday.ToStringMinutesAndHoursOnly();
			OpeningTuesdayText   = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeTuesday.ToStringMinutesAndHoursOnly();
			OpeningWednesdayText = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeWednesday.ToStringMinutesAndHoursOnly();
			OpeningThursdayText  = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeThursday.ToStringMinutesAndHoursOnly();
			OpeningFridayText    = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeFriday.ToStringMinutesAndHoursOnly();
			OpeningSaturdayText  = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeSaturday.ToStringMinutesAndHoursOnly();
			OpeningSundayText    = currentMedicalPracticeVersion.HoursOfOpening.OpeningTimeSunday.ToStringMinutesAndHoursOnly();

			ClosingMondayText    = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeMonday.ToStringMinutesAndHoursOnly();
			ClosingTuesdayText   = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeTuesday.ToStringMinutesAndHoursOnly();
			ClosingWednesdayText = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeWednesday.ToStringMinutesAndHoursOnly();
			ClosingThursdayText  = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeThursday.ToStringMinutesAndHoursOnly();
			ClosingFridayText    = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeFriday.ToStringMinutesAndHoursOnly();
			ClosingSaturdayText  = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeSaturday.ToStringMinutesAndHoursOnly();
			ClosingSundayText    = currentMedicalPracticeVersion.HoursOfOpening.ClosingTimeSunday.ToStringMinutesAndHoursOnly();

			IsOpenOnMonday    = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnMonday;
			IsOpenOnTuesday   = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnTuesday;
			IsOpenOnWednesday = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnWednesday;
			IsOpenOnThursday  = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnThursday;
			IsOpenOnFriday    = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnFriday;
			IsOpenOnSaturday  = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnSaturday;
			IsOpenOnSunday    = currentMedicalPracticeVersion.HoursOfOpening.IsOpenOnSunday;

			var additionsOpenedDayBuilder = new StringBuilder();
			foreach (var day in currentMedicalPracticeVersion.HoursOfOpening.AdditionalOpenedDays)
			{
				additionsOpenedDayBuilder.Append(day);
				additionsOpenedDayBuilder.Append('\n');
			}

			AdditionOpenedDays = additionsOpenedDayBuilder.ToString();

			var additionsClosedDayBuilder = new StringBuilder();
			foreach (var day in currentMedicalPracticeVersion.HoursOfOpening.AdditionalClosedDays)
			{
				additionsClosedDayBuilder.Append(day);
				additionsClosedDayBuilder.Append('\n');
			}

			AdditionClosedDays = additionsClosedDayBuilder.ToString();
		}

		private HoursOfOpening EvaluateFields()
		{
			return new HoursOfOpening(Time.Parse(OpeningMondayText),
									  Time.Parse(OpeningTuesdayText),
									  Time.Parse(OpeningWednesdayText),
									  Time.Parse(OpeningThursdayText),
									  Time.Parse(OpeningFridayText),
									  Time.Parse(OpeningSaturdayText),
									  Time.Parse(OpeningSundayText),
									  Time.Parse(ClosingMondayText),
									  Time.Parse(ClosingTuesdayText),
									  Time.Parse(ClosingWednesdayText),
									  Time.Parse(ClosingThursdayText),
									  Time.Parse(ClosingFridayText),
									  Time.Parse(ClosingSaturdayText),
									  Time.Parse(ClosingSundayText),
									  IsOpenOnMonday,
									  IsOpenOnTuesday,
									  IsOpenOnWednesday,
									  IsOpenOnThursday,
									  IsOpenOnFriday,
									  IsOpenOnSaturday,
									  IsOpenOnSunday,
									  AdditionClosedDays.Split('\n')
														.Where(element => !string.IsNullOrWhiteSpace(element))
														.Select(element => Date.Parse(element.Trim()))
														.ToList(),
									  AdditionOpenedDays.Split('\n')
														.Where(element => !string.IsNullOrWhiteSpace(element))
														.Select(element => Date.Parse(element.Trim()))
														.ToList()
									  );			
		}

		private bool AreFieldsValid ()
		{

			if (!Time.IsValidTimeString(OpeningMondayText)    ||
			    !Time.IsValidTimeString(OpeningTuesdayText)   ||
			    !Time.IsValidTimeString(OpeningWednesdayText) ||
			    !Time.IsValidTimeString(OpeningThursdayText)  ||
			    !Time.IsValidTimeString(OpeningFridayText)    ||
			    !Time.IsValidTimeString(OpeningSaturdayText)  ||
			    !Time.IsValidTimeString(OpeningSundayText))
			{
				return false;
			}

			if (!Time.IsValidTimeString(ClosingMondayText)    ||
				!Time.IsValidTimeString(ClosingTuesdayText)   ||
				!Time.IsValidTimeString(ClosingWednesdayText) ||
				!Time.IsValidTimeString(ClosingThursdayText)  ||
				!Time.IsValidTimeString(ClosingFridayText)    ||
				!Time.IsValidTimeString(ClosingSaturdayText)  ||
				!Time.IsValidTimeString(ClosingSundayText))
			{
				return false;
			}

			if (AdditionClosedDays.Split('\n')
								  .Where(element => !string.IsNullOrWhiteSpace(element))
								  .Any(element => !Date.IsValidDateString(element.Trim())))
			{
				return false;
			}
			
			if (AdditionOpenedDays.Split('\n')
								  .Where(element => !string.IsNullOrWhiteSpace(element))
								  .Any(element => !Date.IsValidDateString(element.Trim())))
			{
				return false;
			}

			return true;
		}

		#endregion

		protected override void CleanUp()
		{
			selectedPageVariable.StateChanged -= OnSelectedPageStateChanged;
		}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
