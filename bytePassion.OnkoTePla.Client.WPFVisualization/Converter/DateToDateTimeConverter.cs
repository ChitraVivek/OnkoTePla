﻿using System;
using System.Globalization;
using bytePassion.Lib.GenericValueConverter;
using bytePassion.Lib.TimeLib;


namespace bytePassion.OnkoTePla.Client.WPFVisualization.Converter
{
	public class DateToDateTimeConverter : GenericValueConverter<Date, DateTime>
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