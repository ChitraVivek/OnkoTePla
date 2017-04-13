using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.DataAndService.Backup;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.BackupPage
{
	public interface IBackupPageViewModel : IViewModel
	{
		ICommand ImportData               { get; }
		ICommand ExportData               { get; }
		ICommand SelectBackupFolder       { get; }
		ICommand ActivateBackupSchedule   { get; }
		ICommand DeactivateBackupSchedule { get; }

		ObservableCollection<BackupInterval> AllBackupIntervals { get; }
		ObservableCollection<DayOfWeek>      AllDaysOfWeek      { get; }

		BackupInterval SelectedBackupInterval  { get; set; }
		string         BackupDestinationFolder { get; set; }		
		string         BackupTime              { get; set; }		
		DayOfWeek      SelectedDayOfWeek       { get; set; }
		string         BackupDay               { get; set; }

		bool IsBackupScheduleChangeable { get; }

		bool IsBackupFolderVisible     { get; }
		bool IsBackupTimeVisible       { get; }
		bool IsBackupDayOfWeekVisible  { get; }
		bool IsBackupDayVisible        { get; }
		bool IsActivateButtonVisible   { get; }
		bool IsDeactivateButtonVisible { get; }
	}
}