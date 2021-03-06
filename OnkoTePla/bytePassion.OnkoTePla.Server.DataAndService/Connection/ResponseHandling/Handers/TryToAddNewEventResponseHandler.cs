using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;
using bytePassion.OnkoTePla.Communication.SendReceive;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.DataAndService.SessionRepository;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.ResponseHandling.Handers
{
	internal class TryToAddNewEventResponseHandler : ResponseHandlerBase<TryToAddNewEventsRequest>
	{
		private readonly IDataCenter dataCenter;

		public TryToAddNewEventResponseHandler (ICurrentSessionsInfo sessionRepository, 
												ResponseSocket socket,
												IDataCenter dataCenter) 
			: base(sessionRepository, socket)
		{
			this.dataCenter = dataCenter;
		}

		public override void Handle(TryToAddNewEventsRequest request)
		{
			foreach (var domainEvent in request.NewEvents)
			{
				if (!IsRequestValid(request.SessionId, request.UserId, domainEvent.AggregateId.MedicalPracticeId))
					return;
			}
			
			var addingSuccessful = dataCenter.AddEvents(request.NewEvents);			

			Socket.SendNetworkMsg(new TryToAddNewEventsResponse(addingSuccessful));
		}
	}
}