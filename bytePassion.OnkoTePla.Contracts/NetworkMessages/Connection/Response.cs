﻿using System;
using bytePassion.OnkoTePla.Contracts.Types;

namespace bytePassion.OnkoTePla.Contracts.NetworkMessages.Connection
{
	public class Response
	{
		public Response(ConnectionSessionId sessionId)
		{
			SessionId = sessionId;
		}

		public ConnectionSessionId SessionId { get; }

		public string AsString()
		{
			return $"Confirmed;{SessionId}";
		}

		public static Response Parse(string s)
		{
			var parts = s.Split(';');

			if (parts.Length != 2)
				throw new ArgumentException($"{s} is not a ConnectionResponse");

			if (parts[0] != "Confirmed")
				throw new ArgumentException($"{s} is not a ConnectionResponse");

			var sessionId = new ConnectionSessionId(Guid.Parse(parts[1]));

			return new Response(sessionId);
		}
	}
}
