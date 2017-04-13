using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Config;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.EventStore;
using bytePassion.OnkoTePla.Server.DataAndService.Repositories.Patients;
using Duration = bytePassion.Lib.TimeLib.Duration;

namespace bytePassion.OnkoTePla.Server.Visualization.SampleDataGenerators
{
	public class AppointmentGenerator
	{
		private readonly Random rand;
		
		private readonly IConfigurationRepository configurationRepository;
		private readonly IPatientRepository patientRepository;
		private readonly IEventStore eventStore;

		public AppointmentGenerator(IConfigurationRepository configurationRepository,
									IPatientRepository patientRepository,
									IEventStore eventStore)
		{
			this.configurationRepository = configurationRepository;
			this.patientRepository = patientRepository;
			this.eventStore = eventStore;

			rand = new Random();
		}

		public void NewAppointments(Guid medicalPracticeId, Date day)
		{
			var patients = patientRepository.GetAllPatients().ToList();
			var user = configurationRepository.GetAllUsers().First();
			var eventstream = eventStore.GetEventStreamForADay(new AggregateIdentifier(day, medicalPracticeId));

			if (eventstream.EventCount > 0)
			{
				MessageBox.Show("cannot create appointments - there are already appointments");
				return;
			}

			var medicalPractice = configurationRepository.GetMedicalPracticeByIdAndVersion(medicalPracticeId,
																						   eventstream.Id.PracticeVersion);			
			
			if (!medicalPractice.HoursOfOpening.IsOpen(day))
			{
				MessageBox.Show("cannot create appointments - practice is not open");
				return;
			}

			var identifier = new AggregateIdentifier(day, medicalPractice.Id, medicalPractice.Version);

			var openingTime = medicalPractice.HoursOfOpening.GetOpeningTime(day);
			var closingTime = medicalPractice.HoursOfOpening.GetClosingTime(day);

			var eventList = new List<DomainEvent>();

			uint aggregateVersion = 0;						

			foreach (var therapyPlace in medicalPractice.GetAllTherapyPlaces())
			{
				var currrentTime = openingTime;
				currrentTime += RandomTimeInterval(0);

				while (currrentTime + new Duration(60*60) < closingTime)
				{					
					var newEvent = CreateAppointmentData(patients, user, identifier, aggregateVersion++,
														 currrentTime, closingTime, therapyPlace.Id);

					eventList.Add(newEvent);
					
					currrentTime += new Duration(newEvent.StartTime, newEvent.EndTime);
					currrentTime += RandomTimeInterval(0);
				}
			}

			eventStore.AddEvents(eventList);
		}

		private AppointmentAdded CreateAppointmentData (IReadOnlyList<Patient> patients, 
														User user,
														AggregateIdentifier identifier,
														uint aggregateVersion,
														Time startTime, Time closingTime, 
														Guid therapyPlaceId)
		{
			return new AppointmentAdded(identifier, 
										aggregateVersion, 
										user.Id, 
										TimeTools.GetCurrentTimeStamp(), 
										ActionTag.RegularAction,
										patients[rand.Next(0, patients.Count-1)].Id,
										"automated generated appointment",
										startTime,
										startTime + GetAppointmentDuration(startTime, closingTime),																  
										therapyPlaceId,
										Guid.Empty,
										Guid.NewGuid());			
		}

		private Duration RandomTimeInterval (int minimum)
		{
			return new Duration((uint)(60 * 15 * rand.Next(minimum, 12)));
		}

		private Duration GetAppointmentDuration (Time startTime, Time closingTime)
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
	}
}
