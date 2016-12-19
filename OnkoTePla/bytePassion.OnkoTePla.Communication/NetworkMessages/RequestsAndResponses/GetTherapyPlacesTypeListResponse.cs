using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class GetTherapyPlacesTypeListResponse : NetworkMessageBase
	{
		public GetTherapyPlacesTypeListResponse (IReadOnlyList<TherapyPlaceType> therapyPlaceTypes)
			: base(NetworkMessageType.GetTherapyPlacesTypeListResponse)
		{
			TherapyPlaceTypes = therapyPlaceTypes;
		}

		public IReadOnlyList<TherapyPlaceType> TherapyPlaceTypes { get; } 

		public override string AsString()
		{
			var sb = new StringBuilder();

			foreach (var therapyPlaceType in TherapyPlaceTypes)
			{
				sb.Append(therapyPlaceType.Name);		sb.Append('#');
				sb.Append(therapyPlaceType.IconType);	sb.Append('#');
				sb.Append(therapyPlaceType.Id);
				sb.Append('|');
			}

			if (TherapyPlaceTypes.Count > 0)
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		public static GetTherapyPlacesTypeListResponse Parse (string s)
		{
			var therapyPlaceTypeList = new List<TherapyPlaceType>();

			var typeListParts = s.Split('|')
						         .Select(listPart => listPart.Split('#'));

			foreach (var typeParts in typeListParts)
			{
				var name     = typeParts[0];
				var iconType = (TherapyPlaceTypeIcon) Enum.Parse(typeof (TherapyPlaceTypeIcon), typeParts[1]);
				var id       = Guid.Parse(typeParts[2]);

				therapyPlaceTypeList.Add(new TherapyPlaceType(name, iconType, id));
			}
				
			return new GetTherapyPlacesTypeListResponse(therapyPlaceTypeList);
		}
	}
}