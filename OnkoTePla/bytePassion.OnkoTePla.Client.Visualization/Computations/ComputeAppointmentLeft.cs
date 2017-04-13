using System.Globalization;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.ConverterBase;

namespace bytePassion.OnkoTePla.Client.Visualization.Computations
{
	internal class ComputeAppointmentPixelLeft : GenericFourToOneValueConverter<Time, double, Time, Time, double>
	{
		protected override double Convert(Time staringTime, double gridWidth, Time timeSlotStart, Time timeSlotEnd, CultureInfo culture)
		{
			var lengthOfOneHour = gridWidth / (new Duration(timeSlotEnd, timeSlotStart).Seconds / 3600.0);
			var durationFromDayBeginToAppointmentStart = new Duration(staringTime, timeSlotStart);

			return  lengthOfOneHour * (durationFromDayBeginToAppointmentStart.Seconds / 3600.0);			
		}
	}
}
