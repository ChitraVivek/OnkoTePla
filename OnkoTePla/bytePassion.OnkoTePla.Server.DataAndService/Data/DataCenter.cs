using System;
using System.Collections.Generic;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Config;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.EventStore;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;
using bytePassion.OnkoTePla.Utils;

namespace bytePassion.OnkoTePla.Server.DataAndService.Data
{
	public class DataCenter : IDataCenter
	{
		private readonly IConfigurationRepository configRepo;		
		private readonly IPatientRepository patientRepository;		
		private readonly IEventStore eventStore;		

		public DataCenter(IConfigurationRepository configRepo,						 
						  IPatientRepository patientRepository,						 
						  IEventStore eventStore)
		{
			this.configRepo = configRepo;			
			this.patientRepository = patientRepository;			
			this.eventStore = eventStore;			

			practiceVersionCache = new Dictionary<Guid, IDictionary<Date, uint>>();
		}

		public IReadOnlyList<Address> GetAllAvailableAddresses ()
		{
			return IpAddressCatcher.GetAllAvailableLocalIpAddresses();
		}	

		#region patients

		public IEnumerable<Patient> GetAllPatients()
		{
			return patientRepository.GetAllPatients();
		}

		public bool AddPatient(Patient newPatient)
		{
			return patientRepository.AddPatient(newPatient);
		}

		#endregion

		#region users

		public IEnumerable<User> GetAllUsers()
		{
			lock (this)
			{
				return configRepo.GetAllUsers();
			}			
		}

		public void AddNewUser(User newUser)
		{
			lock (this)
			{
				configRepo.AddUser(newUser);
			}
		}

		public void UpdateUser(User updatedUser)
		{
			lock (this)
			{
				configRepo.UpdateUser(updatedUser);
			}
		}

		public User GetUser(Guid id)
		{
			lock (this)
			{
				return configRepo.GetUserById(id);
			}
		}

		#endregion

		#region labels

		public IEnumerable<Label> GetAllLabels()
		{
			lock (this)
			{
				return configRepo.GetAllLabels();
			}
		}

		public void AddNewLabel(Label newLabel)
		{
			lock (this)
			{
				configRepo.AddLabel(newLabel);
			}
		}

		public void UpdateLabel(Label updatedLabel)
		{
			lock (this)
			{
				configRepo.UpdateLabel(updatedLabel);
			}
		}

		public Label GetLabel(Guid id)
		{
			lock (this)
			{
				return configRepo.GetLabelById(id);
			}
		}

		#endregion

		#region therapyPlaceTypes

		public IEnumerable<TherapyPlaceType> GetAllTherapyPlaceTypes()
		{
			lock (this)
			{
				return configRepo.GetAllTherapyPlaceTypes();
			}
		}

		public IEnumerable<TherapyPlaceType> GetAllTherapyPlaceTypesPlusDummy()
		{
			lock (this)
			{
				return configRepo.GetAllTherapyPlaceTypes()
								 .Append(TherapyPlaceType.NoType);
			}
		}

		public void AddNewTherapyPlaceType(TherapyPlaceType newTherapyPlaceType)
		{
			lock (this)
			{
				configRepo.AddTherapyPlaceType(newTherapyPlaceType);
			}
		}

		public void UpdateTherapyPlaceType(TherapyPlaceType updatedTherapyPlaceType)
		{
			lock (this)
			{
				configRepo.UpdateTherapyPlaceType(updatedTherapyPlaceType);
			}
		}

		public TherapyPlaceType GetTherapyPlaceType(Guid id)
		{
			lock (this)
			{
				return configRepo.GetTherapyPlaceTypeById(id);
			}
		}

		#endregion

		#region medicalPractices

		public IEnumerable<MedicalPractice> GetAllMedicalPractices ()
		{
			lock (this)
			{
				return configRepo.GetAllMedicalPractices();
			}
		}

		public void AddNewMedicalPractice (MedicalPractice newMedicalPractice)
		{
			lock (this)
			{
				configRepo.AddMedicalPractice(newMedicalPractice);
			}
		}

		public void UpdateMedicalPractice (MedicalPractice updatedMedicalPractice)
		{
			lock (this)
			{
				configRepo.RemoveMedicalPractice(updatedMedicalPractice.Id);
				configRepo.AddMedicalPractice(updatedMedicalPractice);
			}
		}

		public void RemoveMedicalPractice(MedicalPractice medicalPracticeToRemove)
		{
			lock (this)
			{
				configRepo.RemoveMedicalPractice(medicalPracticeToRemove.Id);
			}
		}

		public MedicalPractice GetMedicalPractice(Guid id)
		{
			lock (this)
			{
				return configRepo.GetMedicalPracticeById(id);
			}
		}

		public MedicalPractice GetMedicalPractice(Guid id, uint version)
		{
			lock (this)
			{
				return configRepo.GetMedicalPracticeByIdAndVersion(id, version);
			}
		}

		private readonly IDictionary<Guid, IDictionary<Date, uint>> practiceVersionCache;
		
		public uint GetMedicalPracticeVersion(Guid id, Date date)
		{
			if (!practiceVersionCache.ContainsKey(id))
				practiceVersionCache.Add(id, new Dictionary<Date, uint>());

			var innerCache = practiceVersionCache[id];

			if (!innerCache.ContainsKey(date))
			{
				var eventStream = eventStore.GetEventStreamForADay(new AggregateIdentifier(date, id));
				var practiceVersion = eventStream.Id.PracticeVersion;

				innerCache.Add(date, practiceVersion);				
			}

			return innerCache[date];
		}

		#endregion

		#region eventstore

		public bool AddEvents (IEnumerable<DomainEvent> newEvents)
		{
			lock (eventStore)
			{
				return eventStore.AddEvents(newEvents);
			}
		}		

		public EventStream<Guid> GetEventStreamForAPatient(Guid patientId)
		{
			lock (eventStore)
			{
				return eventStore.GetEventStreamForAPatient(patientId);
			}
		}

		public EventStream<AggregateIdentifier> GetEventStreamForADay(AggregateIdentifier id)
		{
			lock (eventStore)
			{
				return eventStore.GetEventStreamForADay(id);
			}
		}

		#endregion
	}
}
