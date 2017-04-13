using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.ConverterBase;

namespace bytePassion.OnkoTePla.Server.Visualization.Computations
{
	internal class ConnectionStatusBackgroundColor : GenericValueConverter<bool, SolidColorBrush>
	{
		protected override SolidColorBrush Convert(bool value, CultureInfo culture)
		{
			return value 
				? new SolidColorBrush(Colors.LawnGreen)
				: new SolidColorBrush(Colors.Red);
		}
	}
}
