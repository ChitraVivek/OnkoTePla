﻿using System;
using System.Globalization;
using System.Windows.Media;
using bytePassion.Lib.WpfUtils.ConverterBase;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AddAppointmentDialog.Helper;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.Converter
{
	public class CreationStateToSolidColorBrushConverter : GenericValueConverter<AppointmentCreationState, SolidColorBrush>
	{
		protected override SolidColorBrush Convert(AppointmentCreationState value, CultureInfo culture)
		{
			switch (value)
			{
				case AppointmentCreationState.NoSpaceAvailable:
				case AppointmentCreationState.NoPatientSelected:              return new SolidColorBrush(Colors.Red); 
				case AppointmentCreationState.PatientAndDespriptionAvailable: return new SolidColorBrush(Colors.LawnGreen); 
				case AppointmentCreationState.PatientSelected:                return new SolidColorBrush(Colors.Orange); 
			}

			throw new Exception("inner exception");
		}
	}
}
