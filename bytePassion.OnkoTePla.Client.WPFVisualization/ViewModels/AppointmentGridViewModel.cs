﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.Lib.Commands;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Core.Domain;
using bytePassion.OnkoTePla.Client.Core.Readmodels;
using bytePassion.OnkoTePla.Client.Core.Repositories.Config;
using bytePassion.OnkoTePla.Client.Core.Repositories.Readmodel;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Helper;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Interfaces;


namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels
{
	public class AppointmentGridViewModel : IAppointmentGridViewModel
	{
		private enum GridViewDivision { QuarterHours, HalfHours, Hours, TwoHours}

		private const double ThresholdQuarterHoursToHalfHours = 1400;
		private const double ThresholdHalfHoursToHours        = 1000;
		private const double ThresholdHoursToTwoHours         =  600;

		// DataAccess //////////////////////////////////////////////////////////////////////////////////////

		private readonly IReadModelRepository         readModelRepository;
		private readonly IConfigurationReadRepository configuration;


		// GridDrawing /////////////////////////////////////////////////////////////////////////////////////

		private double currentGridWidth;
		private double currentGridHeight;

		private GridViewDivision currentGridViewDivision;

		private Time startTime;
		private Time endTime;

		private readonly ObservableCollection<TimeSlotLabel> timeSlotLabels;
		private readonly ObservableCollection<TimeSlotLine>  timeSlotLines;


		// AppointmentData /////////////////////////////////////////////////////////////////////////////////

		private AppointmentsOfADayReadModel appointmentReadModel;
		
		private readonly ObservableCollection<ITherapyPlaceRowViewModel> therapyPlaceRows;
		private readonly IDictionary<Guid, ObservableCollection<IAppointmentViewModel>> appointmentsOnGrid;		


		// Commands ////////////////////////////////////////////////////////////////////////////////////////

		private readonly ParameterrizedCommand<AggregateIdentifier> loadReamodelCommand; 



		public AppointmentGridViewModel(IReadModelRepository readModelRepository, 
										IConfigurationReadRepository configuration)
		{
			this.readModelRepository = readModelRepository;
			this.configuration = configuration;

			startTime = new Time(7, 0);
			endTime   = new Time(16, 0);
			timeSlotLabels = new ObservableCollection<TimeSlotLabel>();
			timeSlotLines  = new ObservableCollection<TimeSlotLine>();

			appointmentReadModel = null;			
			therapyPlaceRows = new ObservableCollection<ITherapyPlaceRowViewModel>();
			appointmentsOnGrid = new ConcurrentDictionary<Guid, ObservableCollection<IAppointmentViewModel>>();			
			
			loadReamodelCommand = new ParameterrizedCommand<AggregateIdentifier>(LoadReadModelFromId);

			CreateGridDrawing(GridViewDivision.TwoHours);
		}

		private void OnAppointmentChanged(object sender, AppointmentChangedEventArgs appointmentChangedEventArgs)
		{
			switch (appointmentChangedEventArgs.ChangeAction)
			{
				case ChangeAction.Added:
				{
					var appointment = appointmentChangedEventArgs.Appointment;
					appointmentsOnGrid[appointment.TherapyPlace.Id].Add(new AppointmentViewModel(appointment, therapyPlaceRows.First(row => row.TherapyPlaceId == appointment.TherapyPlace.Id)));
					break;
				}
			}
		}

		private void LoadReadModelFromId(AggregateIdentifier id)
		{
			if (appointmentReadModel != null)
				appointmentReadModel.AppointmentChanged -= OnAppointmentChanged;

			appointmentReadModel = readModelRepository.GetAppointmentsOfADayReadModel(id);
			appointmentReadModel.AppointmentChanged += OnAppointmentChanged;

			var updatedId = appointmentReadModel.Identifier;

			var medicalPractice = configuration.GetMedicalPracticeByIdAndVersion(updatedId.MedicalPracticeId,
																				 updatedId.PracticeVersion);

			if (!medicalPractice.HoursOfOpening.IsOpen(updatedId.Date))
				throw new ArgumentException();

			startTime = medicalPractice.HoursOfOpening.GetOpeningTime(updatedId.Date);
			endTime   = medicalPractice.HoursOfOpening.GetClosingTime(updatedId.Date);

			CreateGridDrawing(GetDevisionForWidth(CurrentGridWidth));

			therapyPlaceRows.Clear();

			foreach (var room in medicalPractice.Rooms)  
				foreach (var therapyPlace in room.TherapyPlaces)
				{
					var appointmentListForTherapyPlace = new ObservableCollection<IAppointmentViewModel>();
					appointmentsOnGrid.Add(therapyPlace.Id, appointmentListForTherapyPlace);
					therapyPlaceRows.Add(new TherapyPlaceRowViewModel(appointmentListForTherapyPlace, therapyPlace,room.DisplayedColor, startTime, endTime));
				}

			foreach (var appointment in appointmentReadModel.Appointments)
				appointmentsOnGrid[appointment.TherapyPlace.Id].Add(new AppointmentViewModel(appointment, therapyPlaceRows.First(row => row.TherapyPlaceId == appointment.TherapyPlace.Id)));								
		}

		public ICommand LoadReadModel
		{
			get { return loadReamodelCommand; }
		}

		public ObservableCollection<TimeSlotLabel>             TimeSlotLabels   { get { return timeSlotLabels;   }}
		public ObservableCollection<TimeSlotLine>              TimeSlotLines    { get { return timeSlotLines;    }}
		public ObservableCollection<ITherapyPlaceRowViewModel> TherapyPlaceRows { get { return therapyPlaceRows; }}

		public double CurrentGridWidth
		{
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref currentGridWidth, value);
				RecomputeGrid();
			}
			get { return currentGridWidth; }
		}

		public double CurrentGridHeight
		{
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref currentGridHeight, value);
				RecomputeGrid();
			}
			get { return currentGridHeight; }
		}

		private void RecomputeGrid()
		{
			if (CurrentGridWidth < 200)
				return;

			var gridViewDivision    = GetDevisionForWidth(CurrentGridWidth);			

			if (gridViewDivision == currentGridViewDivision)
				UpdateGridDrawing(gridViewDivision);
			else			
				CreateGridDrawing(gridViewDivision);						
		}

		private void CreateGridDrawing(GridViewDivision gridViewDivision)
		{
			currentGridViewDivision = gridViewDivision;

			var duration = Time.GetDurationBetween(endTime, startTime);

			var slotLengthInSeconds = GetSlotLengthInSeconds(gridViewDivision);

			var timeSlotCount = duration.Seconds / slotLengthInSeconds;
			var timeSlotWidth = CurrentGridWidth / timeSlotCount;

			timeSlotLabels.Clear();
			timeSlotLines.Clear();

			for (uint slot = 0; slot < timeSlotCount + 1; slot++)
			{

				var timeCaption = new Time(startTime + new Duration(slot*slotLengthInSeconds)).ToString()
																							  .Substring(0, 5);

				timeSlotLabels.Add(new TimeSlotLabel(timeCaption)
				{
					XCoord = slot * timeSlotWidth,
					YCoord = 30
				});

				timeSlotLines.Add(new TimeSlotLine()
				{
					XCoord = slot * timeSlotWidth,
					YCoordTop = 60,
					YCoordBottom = CurrentGridHeight
				});
			}			
		}

		private void UpdateGridDrawing(GridViewDivision gridViewDivision)
		{
			var duration            = Time.GetDurationBetween(endTime, startTime);
			var slotLengthInSeconds = GetSlotLengthInSeconds(gridViewDivision);
			var timeSlotCount       = duration.Seconds / slotLengthInSeconds;
			var timeSlotWidth       = CurrentGridWidth / timeSlotCount;

			for (int slot = 0; slot < timeSlotCount + 1; slot++)
			{
				var xCoord = slot*timeSlotWidth;

				timeSlotLabels[slot].XCoord = xCoord;
				timeSlotLines[slot].XCoord  = xCoord;
				timeSlotLines[slot].YCoordBottom = CurrentGridHeight;
			}
		}

		private GridViewDivision GetDevisionForWidth(double width)
		{
			if (width < ThresholdHoursToTwoHours)         return GridViewDivision.TwoHours;
			if (width < ThresholdHalfHoursToHours)        return GridViewDivision.Hours;
			if (width < ThresholdQuarterHoursToHalfHours) return GridViewDivision.HalfHours;

			return GridViewDivision.QuarterHours;
		}

		private uint GetSlotLengthInSeconds(GridViewDivision gridViewDivision)
		{
			switch (gridViewDivision)
			{
				case GridViewDivision.QuarterHours: return  900;
				case GridViewDivision.HalfHours:    return 1800;
				case GridViewDivision.Hours:        return 3600;				
				case GridViewDivision.TwoHours:     return 7200;
			}
			throw new ArgumentException();
		}	

		public event PropertyChangedEventHandler PropertyChanged;

		///////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                           ///////////
		/////////                                  TestArea                                 ///////////
		/////////                                                                           ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////

		public void TestLoad()
		{
			var identifier = new AggregateIdentifier(new Date(3, 7, 2015), configuration.GetMedicalPracticeByName("examplePractice1").Id);
			LoadReadModel.Execute(identifier);
		}
	}
}
