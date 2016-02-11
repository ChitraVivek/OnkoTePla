﻿using System;
using bytePassion.OnkoTePla.Core.Readmodels;


namespace bytePassion.OnkoTePla.Core.Domain.AppointmentLogic
{
	public interface INotifyAppointmentChanged
	{
		event EventHandler<AppointmentChangedEventArgs> AppointmentChanged;
	}
}