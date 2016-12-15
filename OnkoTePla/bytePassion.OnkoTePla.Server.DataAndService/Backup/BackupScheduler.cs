using System;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.LocalSettings;

namespace bytePassion.OnkoTePla.Server.DataAndService.Backup
{
	public class BackupScheduler : IBackupScheduler
	{
		private readonly IBackupService backupService;

		public BackupScheduler(IBackupService backupService)
		{
			this.backupService = backupService;
		}
		

		private void StartDayly(string destinationFolder, Time backupTime)
		{
			
		}

		private void StartWeekly(string destinationFolder, Time backupTime, DayOfWeek backupDay)
		{
			
		}

		private void StartMonthly(string destinationFolder, Time backupTime, int backupDay)
		{
			
		}

		public void Start(ILocalSettingsRepository localSettingsRepository)
		{
			switch (localSettingsRepository.BackupInterval)
			{
				case BackupInterval.Dayly:
				{
					StartDayly(localSettingsRepository.BackupDirectory,
							   localSettingsRepository.BackupTime);
					break;
				}
				case BackupInterval.Weekly:
				{
					StartWeekly(localSettingsRepository.BackupDirectory,
								localSettingsRepository.BackupTime,
								localSettingsRepository.BackupWeekDay);
					break;
				}
				case BackupInterval.Monthly:
				{
					StartMonthly(localSettingsRepository.BackupDirectory,
								 localSettingsRepository.BackupTime,
								 localSettingsRepository.BackUpDay);
					break;
				}
			}
		}

		public void Stop()
		{
			
		}
	}
}