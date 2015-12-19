﻿using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Core.CommandSystem;
using bytePassion.OnkoTePla.Core.Domain;
using bytePassion.OnkoTePla.Core.Readmodels;
using bytePassion.OnkoTePla.Core.Repositories.Config;
using bytePassion.OnkoTePla.Core.Repositories.EventStore;
using bytePassion.OnkoTePla.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Core.Repositories.Readmodel;

namespace bytePassion.OnkoTePla.Client.DataAndService.Data
{

	public class DataCenter : IDataCenter
	{
        private readonly IConfigurationReadRepository configuration;
        private readonly IPatientReadRepository patientRepository;
        private readonly IReadModelRepository readModelRepository;
		private readonly ICommandBus commandBus;
		private readonly IEventStore eventStore;

		internal DataCenter(IConfigurationReadRepository configuration, 
						    IPatientReadRepository patientRepository, 
						    IReadModelRepository readModelRepository,
							ICommandBus commandBus,
							IEventStore eventStore)
        {
            this.configuration = configuration;
            this.patientRepository = patientRepository;
            this.readModelRepository = readModelRepository;
			this.commandBus = commandBus;
			this.eventStore = eventStore;

			dataCache = new Dictionary<Guid, IDictionary<Date, MedicalPractice>>();
        }

        private readonly IDictionary<Guid, IDictionary<Date, MedicalPractice>> dataCache;
        

        public AppointmentsOfADayReadModel GetAppointmentsOfADayReadModel(AggregateIdentifier identifier)
        {
            return readModelRepository.GetAppointmentsOfADayReadModel(identifier);
        }

        public AppointmentsOfAPatientReadModel GetAppointmentsOfAPatientReadModel(Guid patientId)
        {
            return readModelRepository.GetAppointmentsOfAPatientReadModel(patientId);
        }

        public MedicalPractice GetMedicalPracticeByIdAndDate(Guid medicalPracticeId, Date date)
		{			
			if (!dataCache.ContainsKey(medicalPracticeId))
				dataCache.Add(medicalPracticeId, new Dictionary<Date, MedicalPractice>());

			var innerCache = dataCache[medicalPracticeId];

			if (!innerCache.ContainsKey(date))
			{
				var readModel = readModelRepository.GetAppointmentsOfADayReadModel(new AggregateIdentifier(date, medicalPracticeId));
				var medicalPractice = configuration.GetMedicalPracticeByIdAndVersion(medicalPracticeId, readModel.Identifier.PracticeVersion);

				innerCache.Add(date, medicalPractice);

				readModel.Dispose();
			}
			
            return innerCache[date];
		}

        public MedicalPractice GetMedicalPracticeByIdAndVersion(Guid medicalPracticeId, uint version = 0)
        {
            return configuration.GetMedicalPracticeByIdAndVersion(medicalPracticeId, version);
        }

        public MedicalPractice GetMedicalPracticeById(Guid medicalPracticeId)
        {
            return configuration.GetMedicalPracticeById(medicalPracticeId);
        }

        public IEnumerable<MedicalPractice> GetAllMedicalPractices()
        {
            return configuration.GetAllMedicalPractices();
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return patientRepository.GetAllPatients();
        }

		public void SendCommand(DomainCommand command)
		{
			commandBus.SendCommand(command);
		}

		public void PersistEventstore()
		{
			eventStore.PersistRepository();
		}
	}
}
