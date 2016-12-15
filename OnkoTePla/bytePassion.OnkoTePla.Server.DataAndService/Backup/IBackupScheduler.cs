using System;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.LocalSettings;

namespace bytePassion.OnkoTePla.Server.DataAndService.Backup
{
	public interface IBackupScheduler
	{
		void Start(ILocalSettingsRepository localSettingsRepository);
		void Stop();
	}
}