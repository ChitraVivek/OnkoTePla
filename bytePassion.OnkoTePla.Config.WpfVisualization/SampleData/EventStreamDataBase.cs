﻿
using System;
using System.Collections.Generic;
using System.Linq;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Core.CommandSystem.Bus;
using bytePassion.OnkoTePla.Client.Core.Domain;
using bytePassion.OnkoTePla.Client.Core.Domain.CommandHandler;
using bytePassion.OnkoTePla.Client.Core.Domain.Commands;
using bytePassion.OnkoTePla.Client.Core.Eventsystem.Bus;
using bytePassion.OnkoTePla.Client.Core.Readmodels;
using bytePassion.OnkoTePla.Client.Core.Repositories;
using bytePassion.OnkoTePla.Client.Core.Repositories.Aggregate;
using bytePassion.OnkoTePla.Client.Core.Repositories.Config;
using bytePassion.OnkoTePla.Client.Core.Repositories.EventStore;
using bytePassion.OnkoTePla.Client.Core.Repositories.Patients;
using bytePassion.OnkoTePla.Client.Core.Repositories.Readmodel;
using bytePassion.OnkoTePla.Client.Resources;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;


namespace bytePassion.OnkoTePla.Config.WpfVisualization.SampleData
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
			Duration currentTry = RandomTimeInterval(4);

			while (startTime + currentTry > closingTime)
			{
				if (tryCount++ > 100)
				{
					currentTry = new Duration(60 * 30);
					break;
				}

				currentTry = RandomTimeInterval(4);
			}

			return currentTry;
		}

		private static AddAppointment CreateAppointmentData (AppointmentsOfADayReadModel readModel, IReadOnlyList<Patient> patients, 
															 Time startTime, Time closingTime, Guid therapyPlaceId, Guid userId)
		{
			return new AddAppointment(readModel.Identifier, 
									  readModel.AggregateVersion,
									  userId,
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
					commandBus.Send(nextCommand);

					currrentTime += Time.GetDurationBetween(nextCommand.CreateAppointmentData.StartTime,nextCommand.CreateAppointmentData.EndTime);
					currrentTime += RandomTimeInterval(0);
				}
			}
		}


		public static void GenerateExampleEventStream()
		{			
			IPersistenceService<IEnumerable<Patient>> patientPersistenceService = new JSonPatientDataStore(GlobalConstants.PatientJsonPersistenceFile);
			IPatientReadRepository patientRepository = new PatientRepository(patientPersistenceService);
			patientRepository.LoadRepository();

			IPersistenceService<Configuration> configPersistenceService = new XmlConfigurationDataStore(GlobalConstants.ConfigPersistenceFile);
			IConfigurationReadRepository configReadRepository = new ConfigurationRepository(configPersistenceService);
			configReadRepository.LoadRepository();

			IPersistenceService<IEnumerable<EventStream<AggregateIdentifier>>> eventStorePersistenceService = new XmlEventStreamDataStore(GlobalConstants.EventHistoryPersistenceFile);
			IEventStore eventStore = new EventStore(eventStorePersistenceService, configReadRepository);

			IEventBus   eventBus   = new EventBus();
			ICommandBus commandBus = new CommandBus();

			IAggregateRepository aggregateRepository = new AggregateRepository(eventBus, eventStore, patientRepository, configReadRepository);
			IReadModelRepository readModelRepository = new ReadModelRepository(eventBus, eventStore, patientRepository, configReadRepository);

			commandBus.RegisterCommandHandler(new AddAppointmentCommandHandler(aggregateRepository));


			//////////////////////////////

			var medicalPratice = configReadRepository.GetMedicalPracticeByName("examplePractice1");				
			var user = configReadRepository.GetUserByName("exampleUser1");

			var startCreation = new Date(1,  6, 2015);
			var endCreation   = new Date(1, 12, 2015);

			for (var date = startCreation; date < endCreation; date = date.DayAfter())
			{
				var aggregateID = new AggregateIdentifier(date, medicalPratice.Id);
				var readmodel = readModelRepository.GetAppointmentsOfADayReadModel(aggregateID);
				GenerateAppointments(commandBus, readmodel, 
									 medicalPratice, user, 
									 patientRepository.GetAllPatients().ToList(),
									 date);
				readmodel.Dispose();
			}			

			eventStore.PersistRepository();			
		} 
	}
}
