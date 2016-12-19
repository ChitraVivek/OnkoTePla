using System;
using bytePassion.OnkoTePla.Contracts.Types;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class LogoutRequest : NetworkMessageBase
	{
		public LogoutRequest (ConnectionSessionId sessionId, Guid userId) 
			: base(NetworkMessageType.LogoutRequest)
		{
			SessionId = sessionId;
			UserId = userId;			
		}

		public ConnectionSessionId SessionId { get; }
		public Guid                UserId    { get; }		

		public override string AsString()
		{
			return $"{SessionId};{UserId}";
		}

		public static LogoutRequest Parse (string s)
		{
			var parts = s.Split(';');

			var sessionId = new ConnectionSessionId(Guid.Parse(parts[0]));
			var userId    = Guid.Parse(parts[1]);			
			
			return new LogoutRequest(sessionId, userId);
		}
	}
}
