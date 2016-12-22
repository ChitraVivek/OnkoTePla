using System;
using System.Windows;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Communication.NetworkMessages;
using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;
using bytePassion.OnkoTePla.Communication.SendReceive;
using bytePassion.OnkoTePla.Contracts.Types;
using bytePassion.OnkoTePla.Resources;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection.Threads
{
	internal class HeartbeatThead : IThread
	{
		public event Action ServerVanished;

		private readonly Address clientAddress;
		private readonly ConnectionSessionId sessionId;

		private volatile bool stopRunning;

		public HeartbeatThead(Address clientAddress,
							  ConnectionSessionId sessionId)
		{
			this.clientAddress = clientAddress;
			this.sessionId = sessionId;

			stopRunning = false;
			IsRunning = false;
		}

		public void Run()
		{
			IsRunning = true;

			try
			{
				using (var socket = new ResponseSocket())
				{
					socket.Options.Linger = TimeSpan.Zero;

					socket.Bind(clientAddress.ZmqAddress + ":" + GlobalConstants.TcpIpPort.Heartbeat);

					var timoutCounter = 0;

					while (!stopRunning)
					{
						var request = socket.ReceiveNetworkMsg(TimeSpan.FromSeconds(1));

						if (request == null)
						{
							timoutCounter++;

							if (timoutCounter == 10)
							{
								Application.Current?.Dispatcher.Invoke(() => ServerVanished?.Invoke());
								break;
							}
						}
						else if (request.Type == NetworkMessageType.HeartbeatRequest)
						{
							timoutCounter = 0;
							var heartbeatRequest = (HeartbeatRequest) request;
							socket.SendNetworkMsg(new HeartbeatResponse(heartbeatRequest.SessionId));
						}
					}
				}
			}
			catch 
			{
				// Ignored				
			}			

			IsRunning = false;
		}

		public void Stop ()
		{			
			stopRunning = true;
		}

		public bool IsRunning { get; private set; }
	}
}