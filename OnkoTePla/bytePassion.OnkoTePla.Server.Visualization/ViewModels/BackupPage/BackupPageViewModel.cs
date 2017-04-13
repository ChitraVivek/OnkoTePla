using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.DataAndService.Backup;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.LocalSettings;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.BackupPage
{
	internal class BackupPageViewModel : ViewModel, IBackupPageViewModel
	{
		private readonly IBackupService backupService;
		private readonly IBackupScheduler backupScheduler;
		private readonly ILocalSettingsRepository localSettingsRepository;

		private BackupInterval selectedBackupInterval;
		private string backupDestinationFolder;
		private string backupTime;
		private DayOfWeek selectedDayOfWeek;
		private string backupDay;
		private bool isBackupFolderVisible;
		private bool isBackupTimeVisible;
		private bool isBackupDayOfWeekVisible;
		private bool isBackupDayVisible;
		private bool isActivateButtonVisible;
		private bool isDeactivateButtonVisible;
		private bool isBackupScheduleChangeable;

		public BackupPageViewModel(IBackupService backupService,
								   IBackupScheduler backupScheduler,
								   ILocalSettingsRepository localSettingsRepository)
		{
			this.backupService = backupService;
			this.backupScheduler = backupScheduler;
			this.localSettingsRepository = localSettingsRepository;			

			AllBackupIntervals = Enum.GetValues(typeof(BackupInterval)).Cast<BackupInterval>().ToObservableCollection();			
			AllDaysOfWeek      = Enum.GetValues(typeof(DayOfWeek     )).Cast<DayOfWeek     >().ToObservableCollection();

			ImportData               = new Command(DoImportData);
			ExportData               = new Command(DoExportData);
			SelectBackupFolder       = new Command(DoSelectBackupFolder);
			ActivateBackupSchedule   = new Command(DoActivateBackupSchedule);
			DeactivateBackupSchedule = new Command(DoDeactivateBackupScheldule);

			SelectedBackupInterval  = localSettingsRepository.BackupInterval;
			BackupDestinationFolder = localSettingsRepository.BackupDirectory;
			BackupTime              = localSettingsRepository.BackupTime.ToStringMinutesAndHoursOnly();
			SelectedDayOfWeek       = localSettingsRepository.BackupWeekDay;
			BackupDay               = localSettingsRepository.BackUpDay.ToString();

			IsActivateButtonVisible = false;
			IsDeactivateButtonVisible = localSettingsRepository.BackupInterval != BackupInterval.None;
			IsBackupScheduleChangeable = localSettingsRepository.BackupInterval == BackupInterval.None;
		}
		
		public ICommand ImportData               { get; }
		public ICommand ExportData               { get; }
		public ICommand SelectBackupFolder       { get; }
		public ICommand ActivateBackupSchedule   { get; }
		public ICommand DeactivateBackupSchedule { get; }

		public ObservableCollection<BackupInterval> AllBackupIntervals { get; }
		public ObservableCollection<DayOfWeek>      AllDaysOfWeek      { get; }

		public BackupInterval SelectedBackupInterval
		{
			get { return selectedBackupInterval; }
			set
			{
				IsBackupFolderVisible    = value != BackupInterval.None;
				IsBackupTimeVisible      = value != BackupInterval.None;
				IsBackupDayOfWeekVisible = value == BackupInterval.Weekly;
				IsBackupDayVisible       = value == BackupInterval.Monthly;			

				if (localSettingsRepository.BackupInterval == BackupInterval.None &&
				    value != BackupInterval.None)
				{
					IsActivateButtonVisible = true;
				}

				PropertyChanged.ChangeAndNotify(this, ref selectedBackupInterval, value);
			}
		}

		public string BackupDestinationFolder
		{
			get { return backupDestinationFolder; }
			set { PropertyChanged.ChangeAndNotify(this, ref backupDestinationFolder, value); }
		}
		public string BackupTime
		{
			get { return backupTime; }
			set { PropertyChanged.ChangeAndNotify(this, ref backupTime, value); }
		}
		public DayOfWeek SelectedDayOfWeek
		{
			get { return selectedDayOfWeek; }
			set { PropertyChanged.ChangeAndNotify(this, ref selectedDayOfWeek, value); }
		}
		public string BackupDay
		{
			get { return backupDay; }
			set { PropertyChanged.ChangeAndNotify(this, ref backupDay, value); }
		}
		public bool IsBackupScheduleChangeable
		{
			get { return isBackupScheduleChangeable; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isBackupScheduleChangeable, value); }
		}
		public bool IsBackupFolderVisible
		{
			get { return isBackupFolderVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isBackupFolderVisible, value); }
		}
		public bool IsBackupTimeVisible
		{
			get { return isBackupTimeVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isBackupTimeVisible, value); }
		}
		public bool IsBackupDayOfWeekVisible
		{
			get { return isBackupDayOfWeekVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isBackupDayOfWeekVisible, value); }
		}
		public bool IsBackupDayVisible
		{
			get { return isBackupDayVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isBackupDayVisible, value); }
		}
		public bool IsActivateButtonVisible
		{
			get { return isActivateButtonVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isActivateButtonVisible, value); }
		}
		public bool IsDeactivateButtonVisible
		{
			get { return isDeactivateButtonVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isDeactivateButtonVisible, value); }
		}

		private void DoImportData()
		{			
			var openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".obf",
				Filter = "OnkoTePla Backup Files (*.obf)|*.obf"
			};

			var dialogResult = openFileDialog.ShowDialog();			
			if (dialogResult == true)
			{								
				backupService.Import(openFileDialog.FileName);			
			}
		}

		private void DoExportData ()
		{
			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".obf",
				Filter = "OnkoTePla Backup Files (*.obf)|*.obf"
			};

			var dialogResult = saveFileDialog.ShowDialog();
			if (dialogResult == true)
			{
				backupService.Export(saveFileDialog.FileName);
			}			
		}

		private void DoSelectBackupFolder ()
		{
			var dialog = new VistaFolderBrowserDialog
			{
				Description = @"Bitte Ordner Auswählen",
				UseDescriptionForTitle = true
			};

			var showDialogResult = dialog.ShowDialog(null);
			if (showDialogResult != null && (bool)showDialogResult)
				BackupDestinationFolder = dialog.SelectedPath;
		}

		private void DoDeactivateBackupScheldule ()
		{
			localSettingsRepository.BackupInterval = BackupInterval.None;
			localSettingsRepository.PersistRepository();

			backupScheduler.Stop();

			IsDeactivateButtonVisible = false;
			IsActivateButtonVisible = true;
			IsBackupScheduleChangeable = true;
		}

		private void DoActivateBackupSchedule ()
		{
			localSettingsRepository.BackupInterval  = SelectedBackupInterval;
			localSettingsRepository.BackUpDay       = int.Parse(BackupDay);
			localSettingsRepository.BackupDirectory = BackupDestinationFolder;
			localSettingsRepository.BackupTime      = Time.Parse(BackupTime);
			localSettingsRepository.BackupWeekDay   = SelectedDayOfWeek;

			localSettingsRepository.PersistRepository();

			backupScheduler.Start(localSettingsRepository);
			
			IsActivateButtonVisible = false;
			IsDeactivateButtonVisible = true;
			IsBackupScheduleChangeable = false;
		}

		protected override void CleanUp () { }
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
