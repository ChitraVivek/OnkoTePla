using System;
using System.Globalization;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.ConverterBase;

namespace bytePassion.OnkoTePla.Client.Visualization.Converter
{
	internal class DateToDateTimeConverter : GenericValueConverter<Date, DateTime>
	{
		protected override DateTime Convert(Date value, CultureInfo culture)
		{
			return new DateTime(value.Year, value.Month, value.Day);
		}

		protected override Date ConvertBack(DateTime value, CultureInfo culture)
		{
			return new Date(value);
		}
	}
}
