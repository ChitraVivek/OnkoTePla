using System.Globalization;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Server.Visualization.Enums;

namespace bytePassion.OnkoTePla.Server.Visualization.Converter
{
	internal class MainPageToIntConverter : GenericValueConverter<MainPage, int>
    {
        protected override int Convert(MainPage page, CultureInfo culture)
        {
            return (int) page;
        }
    }
}
