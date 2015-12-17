﻿using bytePassion.Lib.Communication.MessageBus;
using bytePassion.Lib.Communication.MessageBus.HandlerCollection;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.Client.Resources;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Core.CommandSystem;
using bytePassion.OnkoTePla.Core.Domain;
using bytePassion.OnkoTePla.Core.Domain.CommandHandler;
using bytePassion.OnkoTePla.Core.Domain.Commands;
using bytePassion.OnkoTePla.Core.Eventsystem;
using bytePassion.OnkoTePla.Core.Readmodels;
using bytePassion.OnkoTePla.Core.Repositories.Aggregate;
using bytePassion.OnkoTePla.Core.Repositories.Config;
using bytePassion.OnkoTePla.Core.Repositories.EventStore;
using bytePassion.OnkoTePla.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Core.Repositories.Readmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Duration = bytePassion.Lib.TimeLib.Duration;


namespace bytePassion.OnkoTePla.SampleDataCreation.SampleData
{
    public static class EventStreamDataBase
	{

		private static readonly Random Rand = new Random();


		private static Duration RandomTimeInterval(int minimum)
		{
			return new Duration((uint)(60 * 15 * Rand.Next(minimum, 12)));
		}

		private static Duration GetAppointmentDuration(Time startTime, Time closingTime)
		{
			int tryCount = 0;
			var currentIntervalTry = RandomTimeInterval(4);

			while (startTime + currentIntervalTry > closingTime)
			{
				if (tryCount++ > 100)
				{
					currentIntervalTry = new Duration(60 * 30);
					break;
				}

				currentIntervalTry = RandomTimeInterval(4);
			}

			return currentIntervalTry;
		}

		private static AddAppointment CreateAppointmentData (AppointmentsOfADayReadModel readModel, IReadOnlyList<Patient> patients, 
															 Time startTime, Time closingTime, Guid therapyPlaceId, Guid userId)
		{
			return new AddAppointment(readModel.Identifier, 
									  readModel.AggregateVersion,
									  userId,
									  ActionTag.RegularAction,
									  patients[Rand.Next(0, patients.Count-1)].Id,
									  "automated generated appointment",
									  startTime,
									  startTime + GetAppointmentDuration(startTime, closingTime),
									  therapyPlaceId);			
		}

		private static void GenerateAppointments (ICommandBus commandBus, AppointmentsOfADayReadModel readModel, 
												  MedicalPractice medicalPractice, User user, 
												  IReadOnlyList<Patient> patients, Date date)
		{
			if (!medicalPractice.HoursOfOpening.IsOpen(date)) return;

			var openingTime = medicalPractice.HoursOfOpening.GetOpeningTime(date);
			var closingTime = medicalPractice.HoursOfOpening.GetClosingTime(date);

			foreach (var therapyPlace in medicalPractice.GetAllTherapyPlaces())
			{
				var currrentTime = openingTime;
				currrentTime += RandomTimeInterval(0);

				while (currrentTime + new Duration(60*60) < closingTime)
				{
					var nextCommand = CreateAppointmentData(readModel, patients, currrentTime, closingTime, therapyPlace.Id, user.Id);
					commandBus.SendCommand(nextCommand);

					currrentTime += new Duration(nextCommand.CreateAppointmentData.StartTime,nextCommand.CreateAppointmentData.EndTime);
					currrentTime += RandomTimeInterval(0);
				}
			}
		}


		public static void TestLoad()
		{
			IPersistenceService<Configuration> configPersistenceService = new JsonConfigurationDataStore(GlobalConstants.ConfigJsonPersistenceFile);
			IConfigurationReadRepository configReadRepository = new ConfigurationRepository(configPersistenceService);
			configReadRepository.LoadRepository();

			IPersistenceService<IEnumerable<EventStream<AggregateIdentifier>>> eventStorePersistenceService = new JsonEventStreamDataStore(GlobalConstants.EventHistoryJsonPersistenceFile);
			IEventStore eventStore = new EventStore(eventStorePersistenceService, configReadRepository);

			eventStore.LoadRepository();
		}


		public static void GenerateExampleEventStream()
		{			
			IPersistenceService<IEnumerable<Patient>> patientPersistenceService = new JSonPatientDataStore(GlobalConstants.PatientJsonPersistenceFile);
			IPatientReadRepository patientRepository = new PatientRepository(patientPersistenceService);
			patientRepository.LoadRepository();

			
			IPersistenceService<Configuration> configPersistenceService = new JsonConfigurationDataStore(GlobalConstants.ConfigJsonPersistenceFile);
			IConfigurationReadRepository configReadRepository = new ConfigurationRepository(configPersistenceService);
			configReadRepository.LoadRepository();

			IPersistenceService<IEnumerable<EventStream<AggregateIdentifier>>> eventStorePersistenceService = new JsonEventStreamDataStore(GlobalConstants.EventHistoryJsonPersistenceFile);			
			IEventStore eventStore = new EventStore(eventStorePersistenceService, configReadRepository);

			IHandlerCollection<DomainEvent> eventHandlerCollection = new MultiHandlerCollection<DomainEvent>();
			IMessageBus<DomainEvent> eventMessageBus = new LocalMessageBus<DomainEvent>(eventHandlerCollection);


			IHandlerCollection<DomainCommand> commandHandlerCollection = new SingleHandlerCollection<DomainCommand>();
			IMessageBus<DomainCommand> commandMessageBus = new LocalMessageBus<DomainCommand>(commandHandlerCollection);


			IEventBus   eventBus   = new EventBus(eventMessageBus);
			ICommandBus commandBus = new CommandBus(commandMessageBus);

			IAggregateRepository aggregateRepository = new AggregateRepository(eventBus, eventStore, patientRepository, configReadRepository);
			IReadModelRepository readModelRepository = new ReadModelRepository(eventBus, eventStore, patientRepository, configReadRepository);

			commandBus.RegisterCommandHandler(new AddAppointmentCommandHandler(aggregateRepository));


			//////////////////////////////

			var medicalPratice = configReadRepository.GetMedicalPracticeByName("examplePractice1");				
			var user = configReadRepository.GetUserByName("exampleUser1");

			var startCreation = new Date(1, 10, 2015);
			var endCreation   = new Date(3, 10, 2016);

			for (var date = startCreation; date < endCreation; date = date.DayAfter())
			{
				var aggregateId = new AggregateIdentifier(date, medicalPratice.Id);
				var readmodel = readModelRepository.GetAppointmentsOfADayReadModel(aggregateId);
				GenerateAppointments(commandBus, readmodel, 
									 medicalPratice, user, 
									 patientRepository.GetAllPatients().ToList(),
									 date);
				readmodel.Dispose();
			}			

			eventStore.PersistRepository();

			MessageBox.Show("fertig");
		} 
	}
}