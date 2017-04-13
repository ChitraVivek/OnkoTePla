using System.Globalization;
using System.Windows;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.Converter
{
	internal class OperatingModeToVisibilityConverter : GenericValueConverter<OperatingMode, Visibility>
	{
		protected override Visibility Convert(OperatingMode value, CultureInfo culture)
		{
			return value == OperatingMode.Edit ? Visibility.Visible : Visibility.Collapsed;
		}		
	}
}
