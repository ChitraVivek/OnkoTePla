using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid
{
	internal class PrintAppointmentGridViewModelSampleData : IPrintAppointmentGridViewModel
	{
		public PrintAppointmentGridViewModelSampleData()
		{
			TimeGridViewModel = new TimeGridViewModelSampleData();
			TherapyPlaceRowViewModels = new ObservableCollection<IPrintTherapyPlaceRowViewModel>
			{
				new PrintTherapyPlaceRowViewModelSampleData(),
				new PrintTherapyPlaceRowViewModelSampleData()
			};
		}

		public ObservableCollection<IPrintTherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; }
		public ITimeGridViewModel TimeGridViewModel { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}