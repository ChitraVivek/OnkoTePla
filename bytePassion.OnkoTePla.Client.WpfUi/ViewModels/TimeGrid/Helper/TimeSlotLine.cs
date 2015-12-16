﻿using bytePassion.Lib.FrameworkExtensions;
using System.ComponentModel;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TimeGrid.Helper
{
    public class TimeSlotLine : INotifyPropertyChanged
	{
		private double xCoord;
		private double yCoordTop;
		private double yCoordBottom;

		public double XCoord
		{
			get { return xCoord; }
			set { PropertyChanged.ChangeAndNotify(this, ref xCoord, value); }
		}

		public double YCoordTop
		{
			get { return yCoordTop; }
			set { PropertyChanged.ChangeAndNotify(this, ref yCoordTop, value); }
		}

		public double YCoordBottom
		{
			get { return yCoordBottom; }
			set { PropertyChanged.ChangeAndNotify(this, ref yCoordBottom, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}