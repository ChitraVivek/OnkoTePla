using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow
{
	internal class PrintTherapyPlaceRowViewModelSampleData : IPrintTherapyPlaceRowViewModel
	{
		public PrintTherapyPlaceRowViewModelSampleData()
		{
			AppointmentViewModels = new ObservableCollection<IPrintAppointmentViewModel>
			{
				new PrintAppointmentViewModelSampleData(new Time( 8,30), new Time(10,0), "patient1"),
				new PrintAppointmentViewModelSampleData(new Time(10,30), new Time(14,0), "patient2")
			};

			TimeSlotBegin = new Time( 8,0);
			TimeSlotEnd   = new Time(16,0);

			GridWidth = 800;

			TherapyPlaceName = "-1-";
		}

		public ObservableCollection<IPrintAppointmentViewModel> AppointmentViewModels { get; }

		public string TherapyPlaceName { get; }

		public Time TimeSlotBegin { get; }
		public Time TimeSlotEnd { get; }
		public double GridWidth { get; set; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;			
	}
}