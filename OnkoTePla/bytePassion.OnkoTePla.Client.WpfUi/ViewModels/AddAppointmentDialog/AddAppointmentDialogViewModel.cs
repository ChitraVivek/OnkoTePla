using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.Commands.Updater;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LabelRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.AppointmentViewModel;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddAppointmentDialog.Helper;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Resources;
using Duration = bytePassion.Lib.TimeLib.Duration;


namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AddAppointmentDialog
{
	internal class AddAppointmentDialogViewModel : ViewModel, IAddAppointmentDialogViewModel
	{				
		private readonly IClientReadModelRepository readModelRepository;
		private readonly Date creationDate;		
	   
	    private readonly IAppointmentViewModelBuilder appointmentViewModelBuilder;
		private readonly Action<string> errorCallback;
		private readonly ISharedStateReadOnly<Patient> selectedPatientVariable;      
		  
        private IDictionary<TherapyPlaceRowIdentifier, IEnumerable<TimeSlot>> allAvailableTimeSlots;

		private Patient selectedPatient;

		private ClientMedicalPracticeData medicalPractice;
		private byte durationMinutes;
		private byte durationHours;
		private AppointmentCreationState creationState;
		private string description;
		
		private Tuple<TherapyPlaceRowIdentifier, TimeSlot> firstFittingTimeSlot;
		private Label selectedLabel;

		public AddAppointmentDialogViewModel(IClientMedicalPracticeRepository medicalPracticeRepository,
											 IClientReadModelRepository readModelRepository,
											 IClientLabelRepository labelRepository,
                                             IPatientSelectorViewModel patientSelectorViewModel,											 											 
                                             ISharedStateReadOnly<Patient> selectedPatientVariable,
                                             Date creationDate,   
											 Guid medicalPracticeId,                                          
											 IAppointmentViewModelBuilder appointmentViewModelBuilder,
											 Action<string> errorCallback)
		{						
			this.readModelRepository = readModelRepository;
			this.creationDate = creationDate;			
			this.appointmentViewModelBuilder = appointmentViewModelBuilder;
			this.errorCallback = errorCallback;
			this.selectedPatientVariable = selectedPatientVariable;

			ComputeTimeSlots(medicalPracticeRepository, medicalPracticeId);

			selectedPatientVariable.StateChanged += OnSelectedPatientVariableChanged;

			PatientSelectorViewModel = patientSelectorViewModel;

			CloseDialog = new Command(CloseWindow);

			CreateAppointment = new Command(DoCreateAppointment,
											() => CreationState != AppointmentCreationState.NoPatientSelected && 
												  CreationState != AppointmentCreationState.NoSpaceAvailable,
											new PropertyChangedCommandUpdater(this, nameof(CreationState)));

			HourPlusOne = new Command(DoHourPlusOne,
									  CanHourPlusOne,
									  new PropertyChangedCommandUpdater(this, nameof(DurationHours)));

			HourMinusOne = new Command(DoHourMinusOne,
									   CanHourMinusOne,
									   new PropertyChangedCommandUpdater(this, nameof(DurationHours), nameof(DurationMinutes)));

			MinutePlusFifteen = new Command(DoMinutePlusFifteen,
											CanMinutePlusFifteen,
											new PropertyChangedCommandUpdater(this, nameof(DurationHours), nameof(DurationMinutes)));

			MinuteMinusFifteen = new Command(DoMinuteMinusFifteen,
											 CanMinuteMinusFifteen,
											 new PropertyChangedCommandUpdater(this, nameof(DurationHours), nameof(DurationMinutes)));

			SelectedPatient = Patient.Dummy;
			Description = string.Empty;

			DurationMinutes = 0;
			DurationHours = 2;

			CreationState = AppointmentCreationState.NoPatientSelected;

			AllAvailablesLabels = new ObservableCollection<Label>();

			labelRepository.RequestAllLabels(
				labelList =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						labelList.Do(AllAvailablesLabels.Add);
						SelectedLabel = labelList.First();
					});					
				},	
				errorCallback
			);
		}

		private void DetermineCreationState()
		{
			if (medicalPractice != null) 
				firstFittingTimeSlot = ComputeFirstFittingSlot();

			if (firstFittingTimeSlot == null)
			{
				CreationState = AppointmentCreationState.NoSpaceAvailable;
			}
			else if (SelectedPatient != Patient.Dummy)
			{
				CreationState = string.IsNullOrWhiteSpace(Description) 
									? AppointmentCreationState.PatientSelected 
									: AppointmentCreationState.PatientAndDespriptionAvailable;
			}
			else
				CreationState = AppointmentCreationState.NoPatientSelected;

		}		

		private bool CanHourPlusOne()
		{
			return DurationHours < 7;
		}

		private bool CanHourMinusOne()
		{
			if (DurationHours > 1)
				return true;

			if (DurationHours == 1)
			{
				return DurationMinutes > 0;
			}

			return false;
		}

		private bool CanMinuteMinusFifteen()
		{
			if (DurationMinutes > 15)
				return true;

			return DurationHours > 0;
		}

		private bool CanMinutePlusFifteen()
		{
			if (DurationMinutes < 45)
				return true;

			return CanHourPlusOne();
		}


		private void OnSelectedPatientVariableChanged(Patient patient)
		{
			SelectedPatient = patient ?? Patient.Dummy;
		}

		private static void CloseWindow()
		{
			var windows = Application.Current.Windows
											 .OfType<Views.AddAppointmentDialog>()
											 .ToList();

			if (windows.Count == 1)
				windows[0].Close();
			else
				throw new Exception("inner error");
		}

		private void DoCreateAppointment()
		{
			var therapyPlace = medicalPractice.GetTherapyPlaceById(firstFittingTimeSlot.Item1.TherapyPlaceId);

			var duration = new Duration((uint) (DurationHours * 3600 + DurationMinutes * 60));

			var newAppointmentViewModel = appointmentViewModelBuilder.Build(new Appointment(SelectedPatient,
																						    Description,
																						    therapyPlace,
																						    creationDate,
																						    firstFittingTimeSlot.Item2.Begin,
																						    firstFittingTimeSlot.Item2.Begin + duration,
																						    Guid.NewGuid(),
																							selectedLabel),
											                                new AggregateIdentifier(creationDate, medicalPractice.Id, medicalPractice.Version), 
																			errorCallback);
		
			newAppointmentViewModel.SwitchToEditMode.Execute(true);
			CloseWindow();
		}

		private void DoHourPlusOne()
		{
			DurationHours += 1;
		}

		private void DoHourMinusOne()
		{
			DurationHours -= 1;
		}

		private void DoMinutePlusFifteen()
		{
			var newMinutes = (byte) (DurationMinutes + 15);

			if (newMinutes == 60)
			{
				DurationMinutes = 0;
				DoHourPlusOne();
			}
			else
			{
				DurationMinutes = newMinutes;
			}
		}

		private void DoMinuteMinusFifteen()
		{
			var newMinuts = DurationMinutes - 15;

			if (newMinuts < 0)
			{
				DurationMinutes = 45;
				DoHourMinusOne();
			}
			else
			{
				DurationMinutes = (byte) newMinuts;
			}
		}

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public ICommand CloseDialog       { get; }
		public ICommand CreateAppointment { get; }
		 
		public ICommand HourPlusOne        { get; }
		public ICommand HourMinusOne       { get; }
		public ICommand MinutePlusFifteen  { get; }
		public ICommand MinuteMinusFifteen { get; }

		public byte DurationHours
		{
			get { return durationHours; }
			private set
			{
				PropertyChanged.ChangeAndNotify(this, ref durationHours, value);
				DetermineCreationState();
			}
		}

		public byte DurationMinutes
		{
			get { return durationMinutes; }
			private set
			{
				PropertyChanged.ChangeAndNotify(this, ref durationMinutes, value);
				DetermineCreationState();
			}
		}

		public ObservableCollection<Label> AllAvailablesLabels { get; }

		public Label SelectedLabel
		{
			get { return selectedLabel; }
			set { PropertyChanged.ChangeAndNotify(this, ref selectedLabel, value); }
		}

		public AppointmentCreationState CreationState
		{
			get { return creationState; }
			private set { PropertyChanged.ChangeAndNotify(this, ref creationState, value); }
		}

		public Patient SelectedPatient
		{
			get { return selectedPatient; }
			private set
			{
				PropertyChanged.ChangeAndNotify(this, ref selectedPatient, value);
				DetermineCreationState();
			}
		}

		public string Description
		{
			set
			{
				description = value;

				if (!NameChecker.CheckName(description))
				{
					var descWithoutForbiddenChar = NameChecker.GetNameWithoutForbittenCharacters(description);
					PropertyChanged.ChangeAndNotify(this, ref description, descWithoutForbiddenChar);
				}

				DetermineCreationState();
			}
			get { return description; }
		}                 

		private void ComputeTimeSlots (IClientMedicalPracticeRepository medicalPracticeRepository, Guid medicalPracticeId)
		{
			medicalPracticeRepository.RequestMedicalPractice(
				loadedPractice =>
				{
					readModelRepository.RequestAppointmentSetOfADay(
						fixAppointmentSet =>
						{
							medicalPractice = loadedPractice;

							IDictionary<TherapyPlaceRowIdentifier, IEnumerable<TimeSlot>> allSlots =
								new Dictionary<TherapyPlaceRowIdentifier, IEnumerable<TimeSlot>>();

							IDictionary<TherapyPlace, IList<Appointment>> sortedAppointments =
								new Dictionary<TherapyPlace, IList<Appointment>>();

							foreach (var therapyPlace in medicalPractice.GetAllTherapyPlaces())
								sortedAppointments.Add(therapyPlace, new List<Appointment>());

							fixAppointmentSet.Appointments.Do(appointment => sortedAppointments[appointment.TherapyPlace].Add(appointment));

							var openingTime = medicalPractice.HoursOfOpening.GetOpeningTime(creationDate);
							var closingTime = medicalPractice.HoursOfOpening.GetClosingTime(creationDate);

							foreach (var therapyRowData in sortedAppointments)
							{
								var slots = ComputeSlots(openingTime, closingTime, therapyRowData.Value);

								allSlots.Add(
									new TherapyPlaceRowIdentifier(new AggregateIdentifier(creationDate, medicalPractice.Id), therapyRowData.Key.Id),
									slots);
							}

							allAvailableTimeSlots = allSlots;
						},
						new AggregateIdentifier(creationDate, loadedPractice.Id, loadedPractice.Version),						
						errorCallback
					);
				},
				medicalPracticeId,
				creationDate,
				errorCallback
			);
		}		

		private static IEnumerable<TimeSlot> ComputeSlots(Time openingTime, Time closingTime, IEnumerable<Appointment> appointments)
		{
			var sortedAppointments = appointments.ToList();
			sortedAppointments.Sort((appointment, appointment1) => appointment.StartTime.CompareTo(appointment1.StartTime));

			var startOfSlots = new List<Time>();
			var endOfSlots = new List<Time>();

			startOfSlots.Add(openingTime);

			foreach (var appointment in sortedAppointments)
			{
				endOfSlots.Add(appointment.StartTime);
				startOfSlots.Add(appointment.EndTime);
			}

			endOfSlots.Add(closingTime);

			var slots = new List<TimeSlot>();

			for (int i = 0; i < startOfSlots.Count; i++)
			{
				slots.Add(new TimeSlot(startOfSlots[i], endOfSlots[i]));
			}

			return slots;
		}

		private Tuple<TherapyPlaceRowIdentifier, TimeSlot> ComputeFirstFittingSlot ()
		{
			if (allAvailableTimeSlots == null)
				return null;

			var duration = new Duration((uint) (DurationHours * 3600 + DurationMinutes * 60));

			return (from timeSlotsOfARow in allAvailableTimeSlots
					from timeSlot in timeSlotsOfARow.Value

					let slotDuration = new Duration(timeSlot.Begin, timeSlot.End)
					where slotDuration >= duration

					select new Tuple<TherapyPlaceRowIdentifier, TimeSlot>(new TherapyPlaceRowIdentifier(new AggregateIdentifier(creationDate, medicalPractice.Id), 
																										timeSlotsOfARow.Key.TherapyPlaceId), 
																		  timeSlot)
					).FirstOrDefault();
		}

        protected override void CleanUp()
        {
            selectedPatientVariable.StateChanged -= OnSelectedPatientVariableChanged;

            ((Command)HourPlusOne).Dispose();
            ((Command)HourMinusOne).Dispose();
            ((Command)MinutePlusFifteen).Dispose();
            ((Command)MinuteMinusFifteen).Dispose();
        }

        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
