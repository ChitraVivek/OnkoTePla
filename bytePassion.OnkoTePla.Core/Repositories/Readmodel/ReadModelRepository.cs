﻿using bytePassion.OnkoTePla.Core.Domain;
using bytePassion.OnkoTePla.Core.Eventsystem;
using bytePassion.OnkoTePla.Core.Readmodels;
using bytePassion.OnkoTePla.Core.Repositories.Config;
using bytePassion.OnkoTePla.Core.Repositories.EventStore;
using bytePassion.OnkoTePla.Core.Repositories.Patients;
using System;


namespace bytePassion.OnkoTePla.Core.Repositories.Readmodel
{
    public class ReadModelRepository : IReadModelRepository
	{
		private readonly IEventBus eventBus;
		private readonly IEventStore eventStore;
		private readonly IConfigurationReadRepository config;
		private readonly IPatientReadRepository patientsRepository;

		public ReadModelRepository (IEventBus eventBus, IEventStore eventStore,
 								   IPatientReadRepository patientsRepository,
								   IConfigurationReadRepository config)
		{
			this.eventStore = eventStore;
			this.config = config;
			this.patientsRepository = patientsRepository;
			this.eventBus = eventBus;
		}

		public FixedAppointmentSet GetAppointmentSetOfADay (AggregateIdentifier id, uint eventStreamVersionLimit)
		{
			var eventStream = eventStore.GetEventStreamForADay(id);
			return new FixedAppointmentSet(config, patientsRepository, eventStream, eventStreamVersionLimit);
		}

		public AppointmentsOfADayReadModel GetAppointmentsOfADayReadModel(AggregateIdentifier id)
		{
			var eventStream = eventStore.GetEventStreamForADay(id);
			var readmodel = new AppointmentsOfADayReadModel(eventBus, config, patientsRepository, eventStream.Id);
			readmodel.LoadFromEventStream(eventStream);

			return readmodel;
		}

		public AppointmentsOfAPatientReadModel GetAppointmentsOfAPatientReadModel(Guid patientId)
		{
			var readModel = new AppointmentsOfAPatientReadModel(patientId, eventBus, config, patientsRepository);
			readModel.LoadFromEventStream(eventStore.GetEventStreamForAPatient(patientId));

			return readModel;
		}
	}
}