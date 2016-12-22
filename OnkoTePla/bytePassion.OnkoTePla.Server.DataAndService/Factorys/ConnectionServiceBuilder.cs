using bytePassion.OnkoTePla.Server.DataAndService.Connection;
using bytePassion.OnkoTePla.Server.DataAndService.Data;

namespace bytePassion.OnkoTePla.Server.DataAndService.Factorys
{
	public class ConnectionServiceBuilder : IConnectionServiceBuilder
	{
		private readonly DataCenterContainer dataCenterContainer;				

		public ConnectionServiceBuilder(DataCenterContainer dataCenterContainer)
		{
			this.dataCenterContainer = dataCenterContainer;			
		}
		
		public IConnectionService Build()
		{			
			return new ConnectionService(dataCenterContainer); 
		}
		
		public void DisposeConnectionService(IConnectionService connectionService)
		{
			connectionService.Dispose();
		}
	}
}
