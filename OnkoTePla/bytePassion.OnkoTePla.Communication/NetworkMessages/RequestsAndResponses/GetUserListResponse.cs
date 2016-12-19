using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class GetUserListResponse : NetworkMessageBase
	{
		public GetUserListResponse(IReadOnlyList<ClientUserData> availableUsers) 
			: base(NetworkMessageType.GetUserListResponse)
		{
			AvailableUsers = availableUsers;
		}

		public IReadOnlyList<ClientUserData> AvailableUsers { get; } 

		public override string AsString()
		{
			if (AvailableUsers.Count == 0)
				return "";

			var msgBuilder = new StringBuilder();

			foreach (var user in AvailableUsers)
			{
				msgBuilder.Append(user.Name); msgBuilder.Append(";");
				msgBuilder.Append(user.Id);   msgBuilder.Append(";");

				foreach (var practiceId in user.ListOfAccessablePractices)
				{
					msgBuilder.Append(practiceId);
					msgBuilder.Append("#");
				}

				if (user.ListOfAccessablePractices.Count > 0)
					msgBuilder.Remove(msgBuilder.Length - 1, 1);

				msgBuilder.Append("|");
			}

			if (AvailableUsers.Count > 0)
				msgBuilder.Remove(msgBuilder.Length - 1, 1);

			return msgBuilder.ToString();
		}

		public static GetUserListResponse Parse(string s)
		{
			if (string.IsNullOrWhiteSpace(s))
				return new GetUserListResponse(new List<ClientUserData>());

			var userList = new List<ClientUserData>();

			var userData = s.Split('|')
							.Select(element => element.Trim())							
							.Select(userInfo => userInfo.Split(';').ToList());

			foreach (var userParts in userData)
			{
				var userName = userParts[0];
				var userId = Guid.Parse(userParts[1]);
				var practiceListParts = userParts[2].Split('#')
													.Select(Guid.Parse)
													.ToList();

				
				userList.Add(new ClientUserData(userName, userId, practiceListParts));
				
			}
						
			return new GetUserListResponse(userList);
		}
	}
}
