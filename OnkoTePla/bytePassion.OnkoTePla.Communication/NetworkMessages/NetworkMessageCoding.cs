using System;
using bytePassion.OnkoTePla.Communication.NetworkMessages.Notifications;
using bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses;

namespace bytePassion.OnkoTePla.Communication.NetworkMessages
{
	public static class NetworkMessageCoding
	{
		public static string Encode(NetworkMessageBase msg)
		{
			return msg.Type + "|" +  msg.AsString();
		}
		
		public static NetworkMessageBase Decode(string messageString)
		{
			if (string.IsNullOrWhiteSpace(messageString))
				return null;

			var type = (NetworkMessageType)Enum.Parse(typeof(NetworkMessageType), GetTypeFromMsg(messageString));
			var msg = GetMsgContent(messageString);

			switch (type)
			{
				case NetworkMessageType.ErrorResponse:                       return ErrorResponse.Parse(msg);	
																		     
				case NetworkMessageType.HeartbeatRequest:                    return HeartbeatRequest.Parse(msg);
				case NetworkMessageType.HeartbeatResponse:                   return HeartbeatResponse.Parse(msg);
																		     
				case NetworkMessageType.BeginConnectionRequest:              return BeginConnectionRequest.Parse(msg);
				case NetworkMessageType.BeginConnectionResponse:             return BeginConnectionResponse.Parse(msg);
				case NetworkMessageType.BeginDebugConnectionRequest:         return BeginDebugConnectionRequest.Parse(msg);
				case NetworkMessageType.BeginDebugConnectionResponse:        return BeginDebugConnectionResponse.Parse(msg);
				case NetworkMessageType.EndConnectionRequest:                return EndConnectionRequest.Parse(msg);
				case NetworkMessageType.EndConnectionResponse:               return EndConnectionResponse.Parse(msg);
				case NetworkMessageType.GetUserListRequest:                  return GetUserListRequest.Parse(msg);
				case NetworkMessageType.GetUserListResponse:                 return GetUserListResponse.Parse(msg);
				case NetworkMessageType.LoginRequest:                        return LoginRequest.Parse(msg);
				case NetworkMessageType.LoginResponse:                       return LoginResponse.Parse(msg);
				case NetworkMessageType.LogoutRequest:                       return LogoutRequest.Parse(msg);
				case NetworkMessageType.LogoutResponse:                      return LogoutResponse.Parse(msg);								
				case NetworkMessageType.GetAppointmentsOfADayRequest:        return GetAppointmentsOfADayRequest.Parse(msg);
				case NetworkMessageType.GetAppointmentsOfADayResponse:       return GetAppointmentsOfADayResponse.Parse(msg);
				case NetworkMessageType.GetAppointmentsOfAPatientRequest:    return GetAppointmentsOfAPatientRequest.Parse(msg);
				case NetworkMessageType.GetAppointmentsOfAPatientResponse:   return GetAppointmentsOfAPatientResponse.Parse(msg);
				case NetworkMessageType.GetPatientListRequest:               return GetPatientListRequest.Parse(msg);
				case NetworkMessageType.GetPatientListResponse:              return GetPatientListResponse.Parse(msg);
				case NetworkMessageType.GetMedicalPracticeRequest:           return GetMedicalPracticeRequest.Parse(msg);
				case NetworkMessageType.GetMedicalPracticeResponse:          return GetMedicalPracticeResponse.Parse(msg);
				case NetworkMessageType.GetTherapyPlacesTypeListRequest:     return GetTherapyPlacesTypeListRequest.Parse(msg);
				case NetworkMessageType.GetTherapyPlacesTypeListResponse:    return GetTherapyPlacesTypeListResponse.Parse(msg);
				case NetworkMessageType.GetPracticeVersionInfoRequest:       return GetPracticeVersionInfoRequest.Parse(msg);
				case NetworkMessageType.GetPracticeVersionInfoResponse:      return GetPracticeVersionInfoResponse.Parse(msg);
				case NetworkMessageType.TryToAddNewEventsRequest:			 return TryToAddNewEventsRequest.Parse(msg);
				case NetworkMessageType.TryToAddNewEventsResponse:			 return TryToAddNewEventsResponse.Parse(msg);
				case NetworkMessageType.TryToAddNewPatientRequest:           return TryToAddNewPatientRequest.Parse(msg);
				case NetworkMessageType.TryToAddNewPatientResponse:          return TryToAddNewPatientResponse.Parse(msg);
				case NetworkMessageType.GetLockRequest:						 return GetLockRequest.Parse(msg);
				case NetworkMessageType.GetLockResponse:					 return GetLockResponse.Parse(msg);
				case NetworkMessageType.ReleaseLockRequest:					 return ReleaseLockRequest.Parse(msg);
				case NetworkMessageType.ReleaseLockResponse:				 return ReleaseLockResponse.Parse(msg);
				case NetworkMessageType.GetLabelListRequest:				 return GetLabelListRequest.Parse(msg);
				case NetworkMessageType.GetLabelListResponse:				 return GetLabelListResponse.Parse(msg);
																		   
 				case NetworkMessageType.EventBusNotification:                return EventBusNotification.Parse(msg);
				case NetworkMessageType.PatientAddedNotification:            return PatientAddedNotification.Parse(msg);
				case NetworkMessageType.PatientUpdatedNotification:          return PatientUpdatedNotification.Parse(msg);
				case NetworkMessageType.TherapyPlaceTypeAddedNotification:   return TherapyPlaceTypeAddedNotification.Parse(msg);	
				case NetworkMessageType.TherapyPlaceTypeUpdatedNotification: return TherapyPlaceTypeUpdatedNotification.Parse(msg);										
				
				default:
					throw new ArgumentException();
			}
		}

		private static string GetTypeFromMsg(string messageString)
		{
			var index = messageString.IndexOf("|", StringComparison.Ordinal);

			if (index == -1)
				throw new ArgumentException("inner error @ message decoding");

			return messageString.Substring(0, index);
		}

		private static string GetMsgContent(string messageString)
		{
			var index = messageString.IndexOf("|", StringComparison.Ordinal);

			if (index == -1)
				throw new ArgumentException("inner error @ message decoding");

			return messageString.Substring(index + 1, messageString.Length - index - 1);
		}
	}
}
