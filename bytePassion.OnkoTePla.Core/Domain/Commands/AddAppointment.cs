﻿using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Core.CommandSystem;
using bytePassion.OnkoTePla.Core.Domain.AppointmentLogic;
using bytePassion.OnkoTePla.Core.Eventsystem;
using System;


namespace bytePassion.OnkoTePla.Core.Domain.Commands
{
    public class AddAppointment : DomainCommand
	{
		public AddAppointment(AggregateIdentifier aggregateId, uint aggregateVersion, 
							  Guid userId, ActionTag actionTag,
							  Guid patientId, string description, 
							  Time startTime, Time endTime, 
							  Guid therapyPlaceId, Guid? appointmentId = null)
			: base(userId, patientId, actionTag)
		{
			var newAppointmentId = appointmentId ?? Guid.NewGuid();

			CreateAppointmentData = new CreateAppointmentData(patientId, description, 
															  startTime, endTime, aggregateId.Date, 
															  therapyPlaceId, newAppointmentId);
			AggregateId = aggregateId;
			AggregateVersion = aggregateVersion;
		}

		public CreateAppointmentData CreateAppointmentData { get; }
		public AggregateIdentifier   AggregateId           { get; }
		public uint                  AggregateVersion      { get; }
	}
}