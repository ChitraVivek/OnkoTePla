using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Server.DataAndService.Backup;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.BackupPage
{
	internal class BackupPageViewModelSampleData : IBackupPageViewModel
	{
		public BackupPageViewModelSampleData()
		{
			AllBackupIntervals = Enum.GetValues(typeof(BackupInterval)).Cast<BackupInterval>().ToObservableCollection();			
			AllDaysOfWeek      = Enum.GetValues(typeof(DayOfWeek     )).Cast<DayOfWeek     >().ToObservableCollection();

			SelectedBackupInterval = AllBackupIntervals[2];
			BackupDestinationFolder = @"C:\test";
			BackupTime = "18:30";			
			SelectedDayOfWeek = AllDaysOfWeek[4];
			BackupDay = "10";

			IsBackupDayOfWeekVisible  = true;
			IsBackupDayVisible        = true;
			IsBackupFolderVisible     = true;
			IsBackupTimeVisible       = true;
			IsActivateButtonVisible   = true;
			IsDeactivateButtonVisible = false;

			IsBackupScheduleChangeable = false;
		}

		public ICommand ImportData               => null;
		public ICommand ExportData               => null;
		public ICommand SelectBackupFolder       => null;
		public ICommand ActivateBackupSchedule   => null;
		public ICommand DeactivateBackupSchedule => null;

		public ObservableCollection<BackupInterval> AllBackupIntervals { get; }
		public ObservableCollection<DayOfWeek> AllDaysOfWeek { get; }

		public BackupInterval SelectedBackupInterval  { get; set; }
		public string         BackupDestinationFolder { get; set; }		
		public string         BackupTime              { get; set; }		
		public DayOfWeek      SelectedDayOfWeek       { get; set; }
		public string         BackupDay               { get; set; }

		public bool IsBackupScheduleChangeable { get; }

		public bool IsBackupFolderVisible     { get; }
		public bool IsBackupTimeVisible       { get; }
		public bool IsBackupDayOfWeekVisible  { get; }
		public bool IsBackupDayVisible        { get; }
		public bool IsActivateButtonVisible   { get; }
		public bool IsDeactivateButtonVisible { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}