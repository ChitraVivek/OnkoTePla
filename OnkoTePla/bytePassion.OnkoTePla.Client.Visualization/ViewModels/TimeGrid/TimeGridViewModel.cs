using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.GeometryLib.Utils;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid.Helper;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using Duration = bytePassion.Lib.TimeLib.Duration;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid
{
	internal class TimeGridViewModel : ViewModel,
									   ITimeGridViewModel									 
	{		
		private enum GridViewDivisionState
		{
			QuarterHours,
			HalfHours,
			Hours,
			TwoHours
		}

		private Size gridSize;

		private GridViewDivisionState gridViewDivisionState;

		private readonly Time timeSlotStart;
		private readonly Time timeSlotEnd;

		private readonly IViewModelCommunication viewModelCommunication;

		public TimeGridViewModel(AggregateIdentifier identifier,
								 IViewModelCommunication viewModelCommunication,
                                 ClientMedicalPracticeData medicalPractice,
								 Size initalSize)
		{
			this.viewModelCommunication = viewModelCommunication;

			viewModelCommunication.RegisterViewModelAtCollection<ITimeGridViewModel, AggregateIdentifier>(
				Constants.ViewModelCollections.TimeGridViewModelCollection,
				this
			);

			Identifier = identifier;			
			
			timeSlotStart = medicalPractice.HoursOfOpening.GetOpeningTime(identifier.Date);
			timeSlotEnd   = medicalPractice.HoursOfOpening.GetClosingTime(identifier.Date);

			TimeSlotLines  = new ObservableCollection<TimeSlotLine>();
			TimeSlotLabels = new ObservableCollection<TimeSlotLabel>();

			SetnewSize(initalSize);						
		}

		public void Process (NewSizeAvailable message)
		{
			SetnewSize(message.NewSize);
		}

		public void Process (Dispose message)
		{
			Dispose();
		}

		public ObservableCollection<TimeSlotLabel> TimeSlotLabels { get; }
		public ObservableCollection<TimeSlotLine>  TimeSlotLines  { get; }

		public AggregateIdentifier Identifier { get; }
        
		private void SetnewSize (Size newGridSize)
		{
			gridSize = newGridSize;
			RecomputeGrid();
		}

		private void RecomputeGrid ()
		{
			if (gridSize.Width < 200)
				return;

			var newGridViewDivision  = GetDevisionForWidth(gridSize.Width);
			var slotLengthInSeconds  = GetSlotLengthInSeconds(newGridViewDivision);
			var timeSlotDuration     = new Duration(timeSlotEnd, timeSlotStart);
			var excactTimeSlotCount  = (double)timeSlotDuration.Seconds / slotLengthInSeconds;
			int roundedTimeSlotCount = (int)Math.Floor(excactTimeSlotCount);
			var timeSlotWidth        = gridSize.Width / excactTimeSlotCount;

			if (newGridViewDivision != gridViewDivisionState || TimeSlotLabels.Count == 0)
			{
				gridViewDivisionState = newGridViewDivision;
				CreateGridDrawing(roundedTimeSlotCount, excactTimeSlotCount,
								  slotLengthInSeconds, timeSlotWidth);
			}
			else
			{
				UpdateGridDrawing(roundedTimeSlotCount, excactTimeSlotCount, timeSlotWidth);
			}
		}

		private void CreateGridDrawing (int roundedTimeSlotCount, double excactTimeSlotCount,
										uint slotLengthInSeconds, double timeSlotWidth)
		{
			TimeSlotLabels.Clear();
			TimeSlotLines.Clear();

			for (uint slot = 0; slot < roundedTimeSlotCount + 1; slot++)
			{

				var timeCaption = new Time(timeSlotStart + new Duration(slot*slotLengthInSeconds)).ToString()
																								  .Substring(0, 5);

				TimeSlotLabels.Add(new TimeSlotLabel(timeCaption)
				{
					XCoord = slot * timeSlotWidth,
					YCoord = 30
				});

				TimeSlotLines.Add(new TimeSlotLine()
				{
					XCoord = slot * timeSlotWidth,
					YCoordTop = 60,
					YCoordBottom = gridSize.Height
				});
			}

			if (!GeometryLibUtils.DoubleEquals(excactTimeSlotCount, roundedTimeSlotCount))
			{
				var timeCaption = timeSlotEnd.ToString()
											 .Substring(0, 5);

				TimeSlotLabels.Add(new TimeSlotLabel(timeCaption)
				{
					XCoord = gridSize.Width,
					YCoord = 30
				});

				TimeSlotLines.Add(new TimeSlotLine()
				{
					XCoord = gridSize.Width,
					YCoordTop = 60,
					YCoordBottom = gridSize.Height
				});
			}
		}

		private void UpdateGridDrawing (int roundedTimeSlotCount, double excactTimeSlotCount, double timeSlotWidth)
		{
			for (int slot = 0; slot < roundedTimeSlotCount + 1; slot++)
			{
				var xCoord = slot*timeSlotWidth;

				TimeSlotLabels[slot].XCoord       = xCoord;
				TimeSlotLines [slot].XCoord       = xCoord;
				TimeSlotLines [slot].YCoordBottom = gridSize.Height;
			}

			if (!GeometryLibUtils.DoubleEquals(excactTimeSlotCount, roundedTimeSlotCount))
			{
				TimeSlotLabels[roundedTimeSlotCount + 1].XCoord       = gridSize.Width;
				TimeSlotLines [roundedTimeSlotCount + 1].XCoord       = gridSize.Width;
				TimeSlotLines [roundedTimeSlotCount + 1].YCoordBottom = gridSize.Height;
			}
		}

		private static GridViewDivisionState GetDevisionForWidth (Width width)
		{ 
			if (width < Constants.Layout.ThresholdGridWidthHoursToTwoHours)         return GridViewDivisionState.TwoHours;
			if (width < Constants.Layout.ThresholdGridWidthHalfHoursToHours)        return GridViewDivisionState.Hours;
			if (width < Constants.Layout.ThresholdGridWidthQuarterHoursToHalfHours) return GridViewDivisionState.HalfHours;

			return GridViewDivisionState.QuarterHours;
		}

		private static uint GetSlotLengthInSeconds (GridViewDivisionState gridViewDivisionState)
		{
			switch (gridViewDivisionState)
			{
				case GridViewDivisionState.QuarterHours: return 900;
				case GridViewDivisionState.HalfHours:    return 1800;
				case GridViewDivisionState.Hours:        return 3600;
				case GridViewDivisionState.TwoHours:     return 7200;
			}
			throw new ArgumentException();
		}

        protected override void CleanUp()
        {
            viewModelCommunication.DeregisterViewModelAtCollection<TimeGridViewModel, AggregateIdentifier>(
                Constants.ViewModelCollections.TimeGridViewModelCollection,
                this
            );
        }

        public override event PropertyChangedEventHandler PropertyChanged;
	}
}
