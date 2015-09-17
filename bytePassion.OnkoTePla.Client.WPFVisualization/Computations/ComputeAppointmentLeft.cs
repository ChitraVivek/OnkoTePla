﻿using System.Globalization;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfUtils.ConverterBase;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.Computations
{
	public class ComputeAppointmentPixelLeft : GenericFourToOneValueConverter<Time, double, Time, Time, double>
	{
		protected override double Convert(Time staringTime, double gridWidth, Time timeSlotStart, Time timeSlotEnd, CultureInfo culture)
		{
			var lengthOfOneHour = gridWidth / (Time.GetDurationBetween(timeSlotEnd, timeSlotStart).Seconds / 3600.0);
			var durationFromDayBeginToAppointmentStart = Time.GetDurationBetween(staringTime, timeSlotStart);

			return  lengthOfOneHour * (durationFromDayBeginToAppointmentStart.Seconds / 3600.0);			
		}
	}
}