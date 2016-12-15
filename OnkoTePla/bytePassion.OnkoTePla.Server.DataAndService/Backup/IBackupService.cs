namespace bytePassion.OnkoTePla.Server.DataAndService.Backup
{
	public interface IBackupService
	{
		void Export(string filename);
		void Import(string filename);
	}
}
