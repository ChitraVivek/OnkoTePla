﻿using System;
using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Client.WpfUi.Enums;
using bytePassion.OnkoTePla.Client.WpfUi.Global;

namespace bytePassion.OnkoTePla.Client.WpfUi.Computations
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
