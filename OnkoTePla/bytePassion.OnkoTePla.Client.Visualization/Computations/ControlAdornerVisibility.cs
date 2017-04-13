using System.Globalization;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.Computations
{
	internal class ControlAdornerVisibility : GenericTwoToOneValueConverter<OperatingMode, bool, bool>
	{
		protected override bool Convert(OperatingMode operatingMode, bool showDisabledOverlay, CultureInfo culture)
		{
			if (showDisabledOverlay)
				return false;

			return operatingMode == OperatingMode.Edit;
		}
	}
}
