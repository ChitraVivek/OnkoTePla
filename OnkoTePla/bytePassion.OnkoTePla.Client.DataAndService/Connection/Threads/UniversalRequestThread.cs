using System;
using bytePassion.Lib.ConcurrencyLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Client.DataAndService.Connection.RequestHandling;
using bytePassion.OnkoTePla.Resources;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection.Threads
{
	internal class UniversalRequestThread : IThread
	{
		private readonly Address serverAddress;
		private readonly TimeoutBlockingQueue<IRequestHandler> workQueue;


		private volatile bool stopRunning;

		public UniversalRequestThread(Address serverAddress,
									  TimeoutBlockingQueue<IRequestHandler> workQueue)
		{			
			this.serverAddress = serverAddress;
			this.workQueue = workQueue;

			IsRunning = false;
			stopRunning = false;
		}

		public void Run()
		{
			IsRunning = true;

			try
			{
				using (var socket = new RequestSocket())
				{
					socket.Options.Linger = TimeSpan.Zero;

					socket.Connect(serverAddress.ZmqAddress + ":" + GlobalConstants.TcpIpPort.Request);

					while (!stopRunning)
					{
						var workItem = workQueue.TimeoutTake();

						if (workItem == null)   // Timeout-case
							continue;

						workItem.HandleRequest(socket);
					}
				}
			}
			catch 
			{
				// ignored				
			}
			
			IsRunning = false;
		}

		public void Stop()
		{
			stopRunning = true;
		}

		public bool IsRunning { get; private set; }
	}
}