using System.IO;
using System.IO.Compression;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Server.DataAndService.Connection;

namespace bytePassion.OnkoTePla.Server.DataAndService.Backup
{
	public class BackupService : IBackupService
	{
		private readonly IPersistable patientRepo;
		private readonly IPersistable configRepo;
		private readonly IPersistable eventstore;
		private readonly IConnectionService connectionService;

		public BackupService(IPersistable patientRepo, 
							 IPersistable configRepo, 
							 IPersistable eventstore, 
							 IConnectionService connectionService)
		{
			this.patientRepo = patientRepo;
			this.configRepo = configRepo;
			this.eventstore = eventstore;
			this.connectionService = connectionService;
		}

		public void Export(string filename)
		{			
			patientRepo.PersistRepository();
			configRepo.PersistRepository();
			eventstore.PersistRepository();

			ZipFile.CreateFromDirectory(GlobalConstants.ServerBasePath, filename, CompressionLevel.Fastest, false);
		}

		public void Import(string filename)
		{			
			connectionService.StopCommunication();

			var serverDirectory = new DirectoryInfo(GlobalConstants.ServerBasePath);

			foreach (var file in serverDirectory.GetFiles())      { file.Delete(); }
			foreach (var dir in serverDirectory.GetDirectories()) { dir.Delete(true); }

			ZipFile.ExtractToDirectory(filename, GlobalConstants.ServerBasePath);	
			
			patientRepo.LoadRepository();
			configRepo.LoadRepository();
			eventstore.LoadRepository();			
		}
	}
}