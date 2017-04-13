using System.Windows;
using System.Windows.Media;

namespace bytePassion.OnkoTePla.Client.Visualization.Global
{
	internal static class Constants
	{
		internal static class LayoutColors
	    {
			public static readonly Color ConnectionServiceColorWhileConnection  = Colors.Yellow;
			public static readonly Color ConnectionServiceColorWhenConnected    = Colors.LawnGreen;
			public static readonly Color ConnectionServiceColorWhenDisconnected = Colors.Red;

			public static readonly Color AppointmentCreateStateImpossible             = Colors.Red;
			public static readonly Color AppointmentCreateStatePossibleButNotComplete = Colors.Orange;
			public static readonly Color AppointmentCreateStatePossible               = Color.FromRgb(0, 174, 0);
		}

		internal static class Layout
		{
			public const double NotificationServiceWidth = 700.0;

			public static readonly GridLength AppointmentGridLeftColumsSize = new GridLength(150.0, GridUnitType.Pixel);
			public static readonly GridLength AppointmentGridTopRowSize     = new GridLength( 50.0, GridUnitType.Pixel);

			// Thresholds to add or remove TimeGridLines

			public const double ThresholdGridWidthQuarterHoursToHalfHours = 1400;
			public const double ThresholdGridWidthHalfHoursToHours        = 1000;
			public const double ThresholdGridWidthHoursToTwoHours         =  600;
		}		
		
		internal static class ViewModelCollections
		{
			public const string TherapyPlaceRowViewModelCollection = "TherpyPlaceRowCollection";
			public const string AppointmentGridViewModelCollection = "AppointmentGridCollection";
			public const string TimeGridViewModelCollection        = "TimeGridCollection";
			public const string AppointmentViewModelCollection     = "AppointmentCollection";
		}				
	}
}
