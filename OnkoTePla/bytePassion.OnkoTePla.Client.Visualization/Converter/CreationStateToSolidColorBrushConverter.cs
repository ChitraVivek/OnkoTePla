using System;
using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.ConverterBase;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AddAppointmentDialog.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.Converter
{
	internal class CreationStateToSolidColorBrushConverter : GenericValueConverter<AppointmentCreationState, SolidColorBrush>
	{
		protected override SolidColorBrush Convert(AppointmentCreationState value, CultureInfo culture)
		{
			switch (value)
			{
				case AppointmentCreationState.NoSpaceAvailable:
				case AppointmentCreationState.NoPatientSelected:              return new SolidColorBrush(Constants.LayoutColors.AppointmentCreateStateImpossible); 
				case AppointmentCreationState.PatientAndDespriptionAvailable: return new SolidColorBrush(Constants.LayoutColors.AppointmentCreateStatePossible); 
				case AppointmentCreationState.PatientSelected:                return new SolidColorBrush(Constants.LayoutColors.AppointmentCreateStatePossibleButNotComplete); 
			}

			throw new Exception("inner exception");
		}
	}
}
