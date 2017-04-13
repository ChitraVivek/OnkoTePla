using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Contracts.Domain;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid
{
	internal class AppointmentGridViewModelSampleData : IAppointmentGridViewModel
	{
		public AppointmentGridViewModelSampleData()
		{			
			TherapyPlaceRowViewModels = new ObservableCollection<ITherapyPlaceRowViewModel>
			{
				new TherapyPlaceRowViewModelSampleData(),
				new TherapyPlaceRowViewModelSampleData(),
				new TherapyPlaceRowViewModelSampleData(),
			};

			TimeGridViewModel = new TimeGridViewModelSampleData();	
			Identifier = new AggregateIdentifier(Date.Dummy, new Guid());
			PracticeIsClosedAtThisDay = true;
			IsActive = true;
		}
		
		public ObservableCollection<ITherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; }

		public ITimeGridViewModel TimeGridViewModel { get; }

		public bool PracticeIsClosedAtThisDay { get; }
		public bool IsActive { get; }

		public AggregateIdentifier Identifier { get; }

		public void Dispose() {}

		public void Process(Activate message) {}
		public void Process(Deactivate message) {}		

		public event PropertyChangedEventHandler PropertyChanged;		
	}
}
