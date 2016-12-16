using System;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Communication.NetworkMessages;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Server.DataAndService.Connection.ResponseHandling.Handers;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.DataAndService.SessionRepository;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Server.DataAndService.Connection.ResponseHandling
{
	internal class ResponseHandlerFactory : IResponseHandlerFactory
	{
		private readonly IDataCenter dataCenter;			
		private readonly ICurrentSessionsInfo sessionRespository;
		private readonly Action<AddressIdentifier, ConnectionSessionId> newConnectionEstablishedCallback;
		private readonly Action<AddressIdentifier, ConnectionSessionId> newDebugConnectionEstablishedCallback;
		private readonly Action<ConnectionSessionId> connectionEndedCallback;

		public ResponseHandlerFactory(IDataCenter dataCenter, 									  
									  ICurrentSessionsInfo sessionRespository,
									  Action<AddressIdentifier, ConnectionSessionId> newConnectionEstablishedCallback,
									  Action<AddressIdentifier, ConnectionSessionId> newDebugConnectionEstablishedCallback,
									  Action<ConnectionSessionId> connectionEndedCallback)
		{
			this.dataCenter = dataCenter;					
			this.sessionRespository = sessionRespository;
			this.newConnectionEstablishedCallback = newConnectionEstablishedCallback;
			this.newDebugConnectionEstablishedCallback = newDebugConnectionEstablishedCallback;
			this.connectionEndedCallback = connectionEndedCallback;
		}
		
		public IResponseHandler<TRequest> GetHandler<TRequest>(TRequest request, ResponseSocket socket) 
			where TRequest : NetworkMessageBase
		{
			switch (request.Type)
			{				
				case NetworkMessageType.LogoutRequest:                    return (IResponseHandler<TRequest>) new LogoutResponseHandler                    (sessionRespository, socket );
				case NetworkMessageType.GetLockRequest:					  return (IResponseHandler<TRequest>) new GetLockResponseHandler				   (sessionRespository, socket );
				case NetworkMessageType.ReleaseLockRequest:				  return (IResponseHandler<TRequest>) new ReleaseLockResponseHandler			   (sessionRespository, socket );
				case NetworkMessageType.BeginDebugConnectionRequest:      return (IResponseHandler<TRequest>) new BeginDebugConnectionResponseHandler      (sessionRespository, socket, newDebugConnectionEstablishedCallback);			
				case NetworkMessageType.BeginConnectionRequest:           return (IResponseHandler<TRequest>) new BeginConnectionResponseHandler           (sessionRespository, socket, newConnectionEstablishedCallback);				
				case NetworkMessageType.EndConnectionRequest:             return (IResponseHandler<TRequest>) new EndConnectionResponseHandler             (sessionRespository, socket, connectionEndedCallback);		
				case NetworkMessageType.GetAppointmentsOfADayRequest:     return (IResponseHandler<TRequest>) new GetAppointemntsOfADayResponseHandler     (sessionRespository, socket, dataCenter);
				case NetworkMessageType.GetAppointmentsOfAPatientRequest: return (IResponseHandler<TRequest>) new GetAppointemntsOfAPatientResponseHandler (sessionRespository, socket, dataCenter);							
				case NetworkMessageType.GetPatientListRequest:            return (IResponseHandler<TRequest>) new GetPatientListResponseHandler            (sessionRespository, socket, dataCenter);				
				case NetworkMessageType.GetMedicalPracticeRequest:        return (IResponseHandler<TRequest>) new GetMedicalPracticeResponseHandler        (sessionRespository, socket, dataCenter);
				case NetworkMessageType.GetTherapyPlacesTypeListRequest:  return (IResponseHandler<TRequest>) new GetTherapyPlaceTypeListResponseHandler   (sessionRespository, socket, dataCenter);
				case NetworkMessageType.GetUserListRequest:               return (IResponseHandler<TRequest>) new GetUserListResponseHandler               (sessionRespository, socket, dataCenter);
				case NetworkMessageType.GetLabelListRequest:			  return (IResponseHandler<TRequest>) new GetLabelListResponseHandler              (sessionRespository, socket, dataCenter);
				case NetworkMessageType.GetPracticeVersionInfoRequest:    return (IResponseHandler<TRequest>) new GetPracticeVersionInfoResponseHandler    (sessionRespository, socket, dataCenter);
				case NetworkMessageType.LoginRequest:                     return (IResponseHandler<TRequest>) new LoginResponseHandler                     (sessionRespository, socket, dataCenter);
				case NetworkMessageType.TryToAddNewEventsRequest:         return (IResponseHandler<TRequest>) new TryToAddNewEventResponseHandler          (sessionRespository, socket, dataCenter);
				case NetworkMessageType.TryToAddNewPatientRequest:        return (IResponseHandler<TRequest>) new TryToAddNewPatientResponseHandler        (sessionRespository, socket, dataCenter);

				default:
					throw new NotImplementedException();
			}
		}
	}
}
