﻿using System.Globalization;
using System.Windows;
using bytePassion.Lib.WpfUtils.ConverterBase;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.Computations
{
	public class PatientToCollapedVisibilityConverter : GenericParameterizedValueConverter<Patient, Visibility, bool>
	{
		protected override Visibility Convert(Patient value, bool invert, CultureInfo culture)
		{
			if (invert)
				return value != null ? Visibility.Collapsed : Visibility.Visible;
			else			
				return value == null ? Visibility.Collapsed : Visibility.Visible;
			
		}		
	}
}