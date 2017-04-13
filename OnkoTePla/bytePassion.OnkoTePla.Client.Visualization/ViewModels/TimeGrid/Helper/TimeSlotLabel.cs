using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid.Helper
{
	public class TimeSlotLabel : INotifyPropertyChanged
	{
		private double xCoord;
		private double yCoord;

		public TimeSlotLabel(string caption)
		{
			Caption = caption;
		}

		public double XCoord
		{
			get { return xCoord; }
			set { PropertyChanged.ChangeAndNotify(this, ref xCoord, value); }
		}

		public double YCoord
		{
			get { return yCoord; }
			set { PropertyChanged.ChangeAndNotify(this, ref yCoord, value); }
		}

		public string Caption { get; }

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}