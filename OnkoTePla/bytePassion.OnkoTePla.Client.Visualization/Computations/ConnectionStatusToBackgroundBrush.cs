using System;
using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Client.Visualization.Enums;
using bytePassion.OnkoTePla.Client.Visualization.Global;

namespace bytePassion.OnkoTePla.Client.Visualization.Computations
{
	internal class ConnectionStatusToBackgroundBrush : GenericValueConverter<ConnectionStatus, SolidColorBrush>
	{
		protected override SolidColorBrush Convert(ConnectionStatus connectionStatus, CultureInfo culture)
		{
			switch (connectionStatus)
			{				
				case ConnectionStatus.TryToConnect: 
				case ConnectionStatus.TryToDisconnect: return new SolidColorBrush(Constants.LayoutColors.ConnectionServiceColorWhileConnection);	
				case ConnectionStatus.Connected:       return new SolidColorBrush(Constants.LayoutColors.ConnectionServiceColorWhenConnected); 
				case ConnectionStatus.Disconnected:    return new SolidColorBrush(Constants.LayoutColors.ConnectionServiceColorWhenDisconnected);
			}

			throw new ArgumentException();
		}
	}
}
