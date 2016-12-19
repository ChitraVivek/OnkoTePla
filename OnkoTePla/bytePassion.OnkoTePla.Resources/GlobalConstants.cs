using System;

namespace bytePassion.OnkoTePla.Resources
{
	public static class GlobalConstants
    {		
		public static class TcpIpPort
		{						 
			public const uint Heartbeat    = 6656;
			public const uint Request      = 6657;
			public const uint Notification = 6658;			
		}

		public static readonly char[] ForbiddenCharacters = {'|',';','#',
															 '(','[','{','}',']',')' };

		public const uint StandardSendingTimeout			 =  2000;    // unit is milliseconds
		public const uint HeartbeatIntverval				 =  5000;    //			"
		public const uint ClientWaitTimeForHeartbeat		 = 10000;	 //			"
		public const uint ServerWaitTimeForHeartbeatResponse =  2000;    //			"

		private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		public static readonly string ClientBasePath = AppDataPath + @"\bytePassion\OnkoTePla\Client\";
		public static readonly string ServerBasePath = AppDataPath + @"\bytePassion\OnkoTePla\Server\";
		public static readonly string BackupBasePath = AppDataPath + @"\bytePassion\OnkoTePla\Backup\";

		private const string Extension = ".xml";

		public static readonly string PatientPersistenceFile             = ServerBasePath + "patients" + Extension;		
		public static readonly string ConfigPersistenceFile              = ServerBasePath + "config"   + Extension;				
		public static readonly string MetaDataPersistanceFile            = ServerBasePath + "metaData" + Extension;
		public static readonly string LocalServerSettingsPersistanceFile = ServerBasePath + "settings" + Extension;
		public static readonly string EventHistoryBasePath               = ServerBasePath + "EventHistory";
																         
		public static readonly string LocalSettingsPersistenceFile       = ClientBasePath + "settings" + Extension;
	}
}
