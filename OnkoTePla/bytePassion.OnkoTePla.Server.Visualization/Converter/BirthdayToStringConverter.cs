using System.Globalization;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.ConverterBase;

namespace bytePassion.OnkoTePla.Server.Visualization.Converter
{
	internal class BirthdayToStringConverter : GenericValueConverter<Date, string>
	{
		protected override string Convert(Date date, CultureInfo culture)
		{			
		    return date.GetDisplayString(CultureInfo.CurrentCulture);
		}		
	}
}
