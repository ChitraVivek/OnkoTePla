using System;
using System.Collections.Generic;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.DataAndService.Connection;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository
{
	public class ClientTherapyPlaceTypeRepository : DisposingObject, IClientTherapyPlaceTypeRepository
	{
		public event Action<TherapyPlaceType> UpdatedTherapyPlaceTypeAvailable;

		private readonly IConnectionService connectionService;
		private  IDictionary<Guid, TherapyPlaceType> cachedTherapyPlaceTypes; 


		public ClientTherapyPlaceTypeRepository (IConnectionService connectionService)
		{
			this.connectionService = connectionService;		
			
			connectionService.NewTherapyPlaceTypeAvailable     += OnNewTherapyPlaceTypeAvailable;
			connectionService.UpdatedTherapyPlaceTypeAvailable += OnUpdatedTherapyPlaceTypeAvailable;	
		}

		private void OnUpdatedTherapyPlaceTypeAvailable(TherapyPlaceType updatedTherapyPlaceType)
		{
			cachedTherapyPlaceTypes[updatedTherapyPlaceType.Id] = updatedTherapyPlaceType;
			UpdatedTherapyPlaceTypeAvailable?.Invoke(updatedTherapyPlaceType);
		}

		private void OnNewTherapyPlaceTypeAvailable(TherapyPlaceType newTherapyPlaceType)
		{
			cachedTherapyPlaceTypes.Add(newTherapyPlaceType.Id, newTherapyPlaceType);
		}		

		public void RequestTherapyPlaceTypes(Action<TherapyPlaceType> therapyPlacetypesAvailableCallback, Guid id, Action<string> errorCallback)
		{
			if (cachedTherapyPlaceTypes == null)
			{
				connectionService.RequestTherapyPlaceTypeList(
					therapyPlaceTypeList =>
					{
						cachedTherapyPlaceTypes = new Dictionary<Guid, TherapyPlaceType>();
						foreach (var therapyPlaceType in therapyPlaceTypeList)
						{
							if (!cachedTherapyPlaceTypes.ContainsKey(therapyPlaceType.Id))
								cachedTherapyPlaceTypes.Add(therapyPlaceType.Id, therapyPlaceType);
						}

						therapyPlacetypesAvailableCallback?.Invoke(GetTherapyPlaceType(id, errorCallback));
					},
					errorCallback
				);
			}
			else
			{
				therapyPlacetypesAvailableCallback?.Invoke(GetTherapyPlaceType(id, errorCallback));
			}
		}

		private TherapyPlaceType GetTherapyPlaceType (Guid id, Action<string> errorCallback)
		{
			if (cachedTherapyPlaceTypes.ContainsKey(id))
				return cachedTherapyPlaceTypes[id];

			errorCallback("therpyPlaceType not found");
			return null;
		}

		protected override void CleanUp()
		{
			connectionService.NewTherapyPlaceTypeAvailable     -= OnNewTherapyPlaceTypeAvailable;
			connectionService.UpdatedTherapyPlaceTypeAvailable -= OnUpdatedTherapyPlaceTypeAvailable;
		}
	}
}