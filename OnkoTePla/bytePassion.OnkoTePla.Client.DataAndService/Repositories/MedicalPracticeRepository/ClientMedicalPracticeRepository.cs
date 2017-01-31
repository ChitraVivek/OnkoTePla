using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.DataAndService.Connection;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository
{
	public class ClientMedicalPracticeRepository : IClientMedicalPracticeRepository
	{
		private readonly IConnectionService connectionService;

		private readonly IList<ClientMedicalPracticeData> cachedMedicalPractices;
		private readonly IDictionary<Guid, IDictionary<Date, uint>> cachedPracticeVersionInfo; 
		
		public ClientMedicalPracticeRepository(IConnectionService connectionService)
		{
			this.connectionService = connectionService;

			cachedMedicalPractices = new List<ClientMedicalPracticeData>();
			cachedPracticeVersionInfo = new Dictionary<Guid, IDictionary<Date, uint>>();
		}

		public void RequestMedicalPractice(Action<ClientMedicalPracticeData> practiceAvailableCallback, Guid practiceId, Action<string> errorCallback)
		{
			var medicalPractice = cachedMedicalPractices.FirstOrDefault(practice => practice.Id == practiceId);

			if (medicalPractice == null)
			{
				connectionService.RequestMedicalPractice(
					practice => practiceAvailableCallback(practice),
					practiceId, 
					uint.MaxValue,
					errorCallback
				);
			}
			else
			{
				practiceAvailableCallback(medicalPractice);
			} 						
		}

		public void RequestMedicalPractice(Action<ClientMedicalPracticeData> practiceAvailableCallback, 
										   Guid practiceId, uint version, Action<string> errorCallback)
		{
			if (cachedMedicalPractices.Any(practice => practice.Id == practiceId && practice.Version == version))
			{
				practiceAvailableCallback(cachedMedicalPractices.First(practice => practice.Id == practiceId && practice.Version == version));
				return;
			}

			connectionService.RequestMedicalPractice(
				medicalPractice =>
				{					
					cachedMedicalPractices.Add(medicalPractice);					
					practiceAvailableCallback(medicalPractice);
				},
				practiceId, version,
				errorCallback
			);
		}

		
		public void RequestMedicalPractice(Action<ClientMedicalPracticeData> practiceAvailableCallback, 
										   Guid practiceId, Date day, Action<string> errorCallback)
		{

			if (!cachedPracticeVersionInfo.ContainsKey(practiceId))
				cachedPracticeVersionInfo.Add(practiceId, new Dictionary<Date, uint>());

			var subDictionary = cachedPracticeVersionInfo[practiceId];

			if (subDictionary.ContainsKey(day))
			{				
				if (cachedMedicalPractices.Any(practice => practice.Id == practiceId && practice.Version == subDictionary[day]))
				{
					practiceAvailableCallback(cachedMedicalPractices.First(practice => practice.Id == practiceId && practice.Version == subDictionary[day]));
					return;
				}

				connectionService.RequestMedicalPractice(
					medicalPractice =>
					{
						cachedMedicalPractices.Add(medicalPractice);
						practiceAvailableCallback(medicalPractice);
					},
					practiceId, subDictionary[day],
					errorCallback
				);
			}
			else
			{
				connectionService.RequestPracticeVersionInfo(
					practiceVersion =>
					{
						if (!subDictionary.ContainsKey(day))
							subDictionary.Add(day, practiceVersion);

						if (cachedMedicalPractices.Any(practice => practice.Id == practiceId && practice.Version == practiceVersion))
						{
							practiceAvailableCallback(cachedMedicalPractices.First(practice => practice.Id == practiceId && practice.Version == practiceVersion));
							return;
						}

						connectionService.RequestMedicalPractice(
							medicalPractice =>
							{
								cachedMedicalPractices.Add(medicalPractice);
								practiceAvailableCallback(medicalPractice);
							},
							practiceId, practiceVersion,
							errorCallback
						);
					},
					practiceId,
					day,
					errorCallback
				);
			}


		}

		public void RequestPraticeVersion(Action<uint> practiceVersionAvailableCallback, 
										  Guid practiceId, Date day, Action<string> errorCallback)
		{
			if (!cachedPracticeVersionInfo.ContainsKey(practiceId))
				cachedPracticeVersionInfo.Add(practiceId, new Dictionary<Date, uint>());

			var subDictionary = cachedPracticeVersionInfo[practiceId];

			if (subDictionary.ContainsKey(day))
			{
				practiceVersionAvailableCallback(subDictionary[day]);				
			}
			else
			{
				connectionService.RequestPracticeVersionInfo(
					practiceVersion =>
					{
						if (!subDictionary.ContainsKey(day))
							subDictionary.Add(day, practiceVersion);

						practiceVersionAvailableCallback(practiceVersion);
					},
					practiceId,
					day,
					errorCallback	
				);
			}
		}
	}
}