using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;
using bytePassion.OnkoTePla.Communication.SendReceive;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.DataAndService.SessionRepository;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.ResponseHandling.Handers
{
	internal class TryToAddNewPatientResponseHandler : ResponseHandlerBase<TryToAddNewPatientRequest>
	{
		private readonly IDataCenter dataCenter;

		public TryToAddNewPatientResponseHandler (ICurrentSessionsInfo sessionRepository,
			ResponseSocket socket,
			IDataCenter dataCenter)
			: base(sessionRepository, socket)
		{
			this.dataCenter = dataCenter;
		}

		public override void Handle (TryToAddNewPatientRequest request)
		{
			if (!IsRequestValid(request.SessionId, request.UserId))
				return;

			
			var addingSuccessful = dataCenter.AddPatient(request.NewPatient);

			Socket.SendNetworkMsg(new TryToAddNewPatientResponse(addingSuccessful));
		}
	}
}