using System;
using System.Text;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Types;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.Notifications
{
	public class TherapyPlaceTypeUpdatedNotification : NetworkMessageBase
	{
		public TherapyPlaceTypeUpdatedNotification (TherapyPlaceType therapyPlaceType, ConnectionSessionId sessionId)
			: base(NetworkMessageType.TherapyPlaceTypeUpdatedNotification)
		{
			TherapyPlaceType = therapyPlaceType;
			SessionId = sessionId;
		}

		public TherapyPlaceType    TherapyPlaceType { get; }
		public ConnectionSessionId SessionId        { get; }

		public override string AsString ()
		{
			var sb = new StringBuilder();

			sb.Append(SessionId);

			sb.Append('|');

			sb.Append(TherapyPlaceType.Name);     sb.Append(';');
			sb.Append(TherapyPlaceType.IconType); sb.Append(';');
			sb.Append(TherapyPlaceType.Id);

			return sb.ToString();
		}

		public static TherapyPlaceTypeUpdatedNotification Parse (string s)
		{
			var index = s.IndexOf("|", StringComparison.Ordinal);

			var sessionId = new ConnectionSessionId(Guid.Parse(s.Substring(0, index)));

			var typeParts = s.Substring(index + 1, s.Length - index - 1)
							 .Split(';');

			var name     = typeParts[0];
			var iconType = (TherapyPlaceTypeIcon) Enum.Parse(typeof (TherapyPlaceTypeIcon), typeParts[1]);
			var id       = Guid.Parse(typeParts[2]);

			return new TherapyPlaceTypeUpdatedNotification(new TherapyPlaceType(name, iconType, id), sessionId);
		}
	}
}