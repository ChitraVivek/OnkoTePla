using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using bytePassion.Lib.TimeLib;

namespace bytePassion.OnkoTePla.Contracts.Infrastructure
{
	public static class ClientMedicalPracticeDataSerializer
	{
		public static ClientMedicalPracticeData Deserialize(string s)
		{
			var parts = s.Split('|');

			var practiceId = Guid.Parse(parts[0]);
			var practiceName = parts[1];
			var practiceVersion = uint.Parse(parts[2]);

			var allRoomData = parts[3];
			var allRoomParts = allRoomData.Split(':');

			var roomList = new List<Room>();

			foreach (var roomData in allRoomParts)			
			{				
				var roomParts = roomData.Split(';');

				var roomId = Guid.Parse(roomParts[0]);
				var roomName = roomParts[1];
				var roomDisplayColor = (Color)ColorConverter.ConvertFromString(roomParts[2]);

				var allTherapyPlaceData = roomParts[3];
				var allTherapyPlaceParts = allTherapyPlaceData.Split('#');

				var therapyPlaceList = new List<TherapyPlace>();

				foreach(var therapyPlaceData in allTherapyPlaceParts)
				{					
					var therapyPlaceParts = therapyPlaceData.Split('(');

					var therapyPlaceId = Guid.Parse(therapyPlaceParts[0]);
					var therapyPlaceName = therapyPlaceParts[1];
					var theralyPlaceTypeId = Guid.Parse(therapyPlaceParts[2]);

					therapyPlaceList.Add(new TherapyPlace(therapyPlaceId, theralyPlaceTypeId, therapyPlaceName));
				}

				roomList.Add(new Room(roomId, roomName, therapyPlaceList, roomDisplayColor));
			}

			var hoursOfOpeningData = parts.Last();
			var hoursOfOpeningParts = hoursOfOpeningData.Split(new[] {';'}, StringSplitOptions.None);

			var openingTimeMonday    = Time.Parse(hoursOfOpeningParts[0]);
			var openingTimeTuesday   = Time.Parse(hoursOfOpeningParts[1]);
			var openingTimeWednesday = Time.Parse(hoursOfOpeningParts[2]);
			var openingTimeThursday  = Time.Parse(hoursOfOpeningParts[3]);
			var openingTimeFriday    = Time.Parse(hoursOfOpeningParts[4]);
			var openingTimeSaturday  = Time.Parse(hoursOfOpeningParts[5]);
			var openingTimeSunday    = Time.Parse(hoursOfOpeningParts[6]);

			var closingTimeMonday    = Time.Parse(hoursOfOpeningParts[7]);
			var closingTimeTuesday   = Time.Parse(hoursOfOpeningParts[8]);
			var closingTimeWednesday = Time.Parse(hoursOfOpeningParts[9]);
			var closingTimeThursday  = Time.Parse(hoursOfOpeningParts[10]);
			var closingTimeFriday    = Time.Parse(hoursOfOpeningParts[11]);
			var closingTimeSaturday  = Time.Parse(hoursOfOpeningParts[12]);
			var closingTimeSunday    = Time.Parse(hoursOfOpeningParts[13]);

			var isOpenOnMonday    = bool.Parse(hoursOfOpeningParts[14]);
			var isOpenOnTuesday   = bool.Parse(hoursOfOpeningParts[15]);
			var isOpenOnWednesday = bool.Parse(hoursOfOpeningParts[16]);
			var isOpenOnThursday  = bool.Parse(hoursOfOpeningParts[17]);
			var isOpenOnFriday    = bool.Parse(hoursOfOpeningParts[18]);
			var isOpenOnSaturday  = bool.Parse(hoursOfOpeningParts[19]);
			var isOpenOnSunday    = bool.Parse(hoursOfOpeningParts[20]);

			var closedDaydata = hoursOfOpeningParts[21];
			var closedDays = closedDaydata.Split('(')
										  .Where(part => !string.IsNullOrWhiteSpace(part))
										  .Select(Date.Parse)
										  .ToList();
			 
			var openedDaydata = hoursOfOpeningParts[22];
			var openedDays = openedDaydata.Split('(')
										  .Where(part => !string.IsNullOrWhiteSpace(part))
										  .Select(Date.Parse)
										  .ToList();

			var hoursOfOpening = new HoursOfOpening(openingTimeMonday, openingTimeTuesday, openingTimeWednesday, openingTimeThursday, 
													openingTimeFriday, openingTimeSaturday, openingTimeSunday, 
													closingTimeMonday, closingTimeTuesday, closingTimeWednesday, closingTimeThursday, 
													closingTimeFriday, closingTimeSaturday, closingTimeSunday, 
													isOpenOnMonday, isOpenOnTuesday, isOpenOnWednesday, isOpenOnThursday, 
													isOpenOnFriday, isOpenOnSaturday, isOpenOnSunday, 
													openedDays, closedDays);

			return new ClientMedicalPracticeData(roomList, practiceName, practiceVersion, practiceId, hoursOfOpening);
		}

		public static string Serialize(ClientMedicalPracticeData practice)
		{
			var sb = new StringBuilder();

			sb.Append(practice.Id);      sb.Append("|");
			sb.Append(practice.Name);    sb.Append("|");
			sb.Append(practice.Version); sb.Append("|");
			
			foreach (var room in practice.Rooms)
			{
				sb.Append(room.Id);             sb.Append(";");
				sb.Append(room.Name);           sb.Append(";");
				sb.Append(room.DisplayedColor); sb.Append(";");				

				foreach (var therapyPlace in room.TherapyPlaces)
				{
					sb.Append(therapyPlace.Id);     sb.Append("(");
					sb.Append(therapyPlace.Name);   sb.Append("(");
					sb.Append(therapyPlace.TypeId); sb.Append("#");
				}

				if (room.TherapyPlaces.Any())
					sb.Remove(sb.Length - 1, 1);

				sb.Append(":");
			}

			if (practice.Rooms.Any())
				sb.Remove(sb.Length - 1, 1);			

			sb.Append("|");

			sb.Append(practice.HoursOfOpening.OpeningTimeMonday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeTuesday);   sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeWednesday); sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeThursday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeFriday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeSaturday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.OpeningTimeSunday);    sb.Append(";");

			sb.Append(practice.HoursOfOpening.ClosingTimeMonday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeTuesday);   sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeWednesday); sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeThursday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeFriday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeSaturday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.ClosingTimeSunday);    sb.Append(";");

			sb.Append(practice.HoursOfOpening.IsOpenOnMonday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnTuesday);   sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnWednesday); sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnThursday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnFriday);    sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnSaturday);  sb.Append(";");
			sb.Append(practice.HoursOfOpening.IsOpenOnSunday);    sb.Append(";");

			
			foreach (var day in practice.HoursOfOpening.AdditionalClosedDays)
			{				
				sb.Append(day);
				sb.Append("(");
			}

			if (practice.HoursOfOpening.AdditionalClosedDays.Count > 0)
				sb.Remove(sb.Length - 1, 1);

			sb.Append(";");			

			foreach (var day in practice.HoursOfOpening.AdditionalOpenedDays)
			{				
				sb.Append(day);
				sb.Append("(");
			}

			if (practice.HoursOfOpening.AdditionalClosedDays.Count > 0)
				sb.Remove(sb.Length - 1, 1);			

			return sb.ToString();
		}
	}
}
