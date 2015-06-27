﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using bytePassion.OnkoTePla.Client.Core.Readmodels;
using bytePassion.OnkoTePla.Contracts.Appointments;


namespace bytePassion.OnkoTePla.Client.Core.Domain.AppointmentLogic
{
	public class ObservableAppointmentCollection : INotifyAppointmentChanged, 
												   INotifyCollectionChanged
	{
		public event EventHandler<AppointmentChangedEventArgs> AppointmentChanged;
		public event NotifyCollectionChangedEventHandler       CollectionChanged;

		private readonly IList<Appointment> appointments;

		public ObservableAppointmentCollection()
		{
			appointments = new List<Appointment>();
		}

		public void AddAppointment(Appointment newAppointment)
		{
			appointments.Add(newAppointment);

			var appointmentHandler = AppointmentChanged;
			if (appointmentHandler != null)
				appointmentHandler(this, new AppointmentChangedEventArgs(newAppointment, ChangeAction.Added));

			var collectionHandler = CollectionChanged;
			if (collectionHandler != null)
				collectionHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
		}

		public IEnumerable<Appointment> Appointments
		{
			get { return appointments.ToList(); }
		}

		public Appointment GetAppointmentById(Guid appointmentId)
		{
			return appointments.FirstOrDefault(appointment => appointment.Id == appointmentId);
		}		
	}
}