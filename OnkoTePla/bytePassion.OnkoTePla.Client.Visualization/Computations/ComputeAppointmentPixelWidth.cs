using System.Globalization;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.ConverterBase;

namespace bytePassion.OnkoTePla.Client.Visualization.Computations
{
	internal class ComputeAppointmentPixelWidth : GenericFiveToOneValueConverter<Time, Time, double, Time, Time, double>
	{
		protected override double Convert(Time startingTime, Time endingTime, double gridWidth, Time timeSlotStart, Time timeSlotEnd, CultureInfo culture)
		{
			var lengthOfOneHour = gridWidth / (new Duration(timeSlotEnd, timeSlotStart).Seconds / 3600.0);															
			var durationOfAppointment = new Duration(startingTime, endingTime);

			return lengthOfOneHour * (durationOfAppointment.Seconds / 3600.0);
		}
	}
}