using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow
{
	internal class PrintTherapyPlaceRowViewModel : ViewModel, IPrintTherapyPlaceRowViewModel 
	{
		private double gridWidth;

		public PrintTherapyPlaceRowViewModel(IEnumerable<IPrintAppointmentViewModel> appointments,  
											 Time timeSlotBegin, Time timeSlotEnd, 
											 string therapyPlaceName)
		{
			TimeSlotBegin = timeSlotBegin;
			TimeSlotEnd = timeSlotEnd;
			TherapyPlaceName = therapyPlaceName;

			AppointmentViewModels = appointments.ToObservableCollection();
		}
		
		public ObservableCollection<IPrintAppointmentViewModel> AppointmentViewModels { get; }

		public string TherapyPlaceName { get; }

		public Time TimeSlotBegin { get; }
		public Time TimeSlotEnd   { get; }

		public double GridWidth
		{
			get { return gridWidth; }
			set { PropertyChanged.ChangeAndNotify(this, ref gridWidth, value); }
		}

		protected override void CleanUp () { }
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
