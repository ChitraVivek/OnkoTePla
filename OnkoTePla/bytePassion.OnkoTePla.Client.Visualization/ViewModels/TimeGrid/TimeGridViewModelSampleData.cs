using System;
using System.Collections.ObjectModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid.Helper;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid
{
	internal class TimeGridViewModelSampleData : ITimeGridViewModel
	{
		public TimeGridViewModelSampleData()
		{
			TimeSlotLabels = new ObservableCollection<TimeSlotLabel>
			{
				new TimeSlotLabel("8:00") { XCoord = 200, YCoord = 10 },
				new TimeSlotLabel("7:00") { XCoord = 100, YCoord = 10 }
			};

			TimeSlotLines = new ObservableCollection<TimeSlotLine>
			{
				new TimeSlotLine {XCoord =  100, YCoordTop = 40, YCoordBottom = 400},
				new TimeSlotLine {XCoord =  200, YCoordTop = 40, YCoordBottom = 400}
			};

			Identifier = new AggregateIdentifier(Date.Dummy, new Guid());
		}

		public ObservableCollection<TimeSlotLabel> TimeSlotLabels { get; }
		public ObservableCollection<TimeSlotLine>  TimeSlotLines  { get; }

		public AggregateIdentifier Identifier { get; }
		
		public void Process(NewSizeAvailable message) {}
		public void Process(Dispose message) {}

		public void Dispose () {}
	}
}
