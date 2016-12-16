using System;
using bytePassion.Lib.Communication.State;
using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;
using bytePassion.OnkoTePla.Contracts.Patients;
using NetMQ.Sockets;

namespace bytePassion.OnkoTePla.Client.DataAndService.Connection.RequestHandling.Handlers
{
	internal class TryToAddNewPatientRequestHander : RequestHandlerBase
	{
		private readonly Patient newPatient;
		private readonly ISharedStateReadOnly<ConnectionInfo> connectionInfoVariable;


		public TryToAddNewPatientRequestHander (Patient newPatient,
			                                    ISharedStateReadOnly<ConnectionInfo> connectionInfoVariable,
			                                    Action<string> errorCallback)
			: base(errorCallback)
		{
			this.newPatient = newPatient;
			this.connectionInfoVariable = connectionInfoVariable;
		}

		public override void HandleRequest (RequestSocket socket)
		{
			HandleRequestHelper<TryToAddNewPatientRequest, TryToAddNewPatientResponse>(
				new TryToAddNewPatientRequest(connectionInfoVariable.Value.SessionId,
					                          connectionInfoVariable.Value.LoggedInUser.Id,
					                          newPatient), 
				socket,
				response =>
				{
					if (!response.AddingWasSuccessful)
						ErrorCallback("adding patient failed");
				}
			);
		}
	}
}