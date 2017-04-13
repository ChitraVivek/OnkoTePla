using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintTherapyPlaceRow;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid
{
	internal class PrintAppointmentGridViewModel : ViewModel, IPrintAppointmentGridViewModel
	{
		public PrintAppointmentGridViewModel(AggregateIdentifier identifier, 
											 ClientMedicalPracticeData medicalPractice, 
											 IEnumerable<Appointment> appointments, 
											 Size initalSize)
		{
			TimeGridViewModel = new PrintTimeGridViewModel(identifier, medicalPractice, initalSize);

			TherapyPlaceRowViewModels = medicalPractice.Rooms
													   .SelectMany(room => room.TherapyPlaces)
													   .Select(therapyPlaceRow => new PrintTherapyPlaceRowViewModel(appointments.Where(appointment => appointment.TherapyPlace.Id == therapyPlaceRow.Id)
																																.Select(appointment => new PrintAppointmentViewModel(appointment.StartTime,
																																													 appointment.EndTime,
																																													 $"{appointment.Patient.Name} (*{appointment.Patient.Birthday.Year})")),
																												    medicalPractice.HoursOfOpening.GetOpeningTime(identifier.Date),
																													medicalPractice.HoursOfOpening.GetClosingTime(identifier.Date),
																													therapyPlaceRow.Name))
													   .Cast<IPrintTherapyPlaceRowViewModel>()
													   .ToObservableCollection();			
		}
		
		public ObservableCollection<IPrintTherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; }
		public ITimeGridViewModel TimeGridViewModel { get; }

		protected override void CleanUp() {	}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
