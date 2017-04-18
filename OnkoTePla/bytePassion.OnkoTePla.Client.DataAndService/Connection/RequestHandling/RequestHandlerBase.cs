using System;
using bytePassion.OnkoTePla.Communication.NetworkMessages;
using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;
using bytePassion.OnkoTePla.Communication.SendReceive;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection.RequestHandling
{
	internal abstract class RequestHandlerBase : IRequestHandler
	{
		protected RequestHandlerBase(Action<string> errorCallback)
		{
			ErrorCallback = errorCallback;			
		}

		public abstract void HandleRequest(RequestSocket socket);
		
		protected Action<string> ErrorCallback { get; }		
		
		protected void HandleRequestHelper<TRequest, TResponse> (TRequest request, 
																 RequestSocket socket,
																 Action<TResponse> responseReceived)
			where TRequest  : NetworkMessageBase
			where TResponse : NetworkMessageBase
		{
			var sendingSuccessful = socket.SendNetworkMsg(request);

			if (!sendingSuccessful)
			{
				ErrorCallback("failed sending");
				return;
			}

#if DEBUG
			var response = socket.ReceiveNetworkMsg(TimeSpan.FromSeconds(50));
#else
			var response = socket.ReceiveNetworkMsg(TimeSpan.FromSeconds(5));
#endif

			if (response == null)
			{
				ErrorCallback("no response received");
				return;
			}

			var responseMsg = response as TResponse;
			if (responseMsg != null)
			{
				responseReceived(responseMsg);
				return;
			}

			if (response.Type == NetworkMessageType.ErrorResponse)
			{
				var errorResponse = (ErrorResponse) response;
				ErrorCallback(errorResponse.ErrorMessage);
				return;
			}

			throw new ArgumentException($"unexpected Message-Type: {response.Type}");
		}
	}
}