using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Server.Visualization.Enums;

namespace bytePassion.OnkoTePla.Server.Visualization.Computations
{
	internal class ButtonBackgroundColor : GenericParameterizedValueConverter<MainPage, SolidColorBrush, MainPage>
	{
		protected override SolidColorBrush Convert(MainPage value, MainPage parameter, CultureInfo culture)
		{
			return value == parameter 
								? new SolidColorBrush(Colors.Aqua) 
								: new SolidColorBrush(Colors.White) ;
		}
	}
}
