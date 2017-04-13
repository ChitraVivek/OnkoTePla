using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.Readmodels;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.Readmodels.Notification;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.Visualization.Enums;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage.Helper;
using bytePassion.OnkoTePla.CommonUiElements.PatientSelector.ViewModel;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage
{
	internal class SearchPageViewModel : ViewModel, 
                                         ISearchPageViewModel
    {
		private class AppointmentSorter : IComparer<DisplayAppointmentData>
		{
			public int Compare (DisplayAppointmentData a1, DisplayAppointmentData a2)
			{
				return a1.Day == a2.Day
					? a1.AppointmentRawData.StartTime.CompareTo(a2.AppointmentRawData.StartTime)
					: a1.Day.CompareTo(a2.Day);
			}
		}

		private const string NoPatientSelected = "- noch kein Patient ausgewählt -";
		
		private readonly ISharedState<Date> selectedDateVariable;		
        private readonly ISharedStateReadOnly<Patient> selectedPatientVariable;        
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly ICommandService commandService;		
		private readonly IClientReadModelRepository readModelRepository;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;

		private readonly Action<string> errorCallBack;

		private AppointmentsOfAPatientReadModel currentReadModel;
		private string selectedPatient;
		private bool showPreviousAppointments;
		private bool noAppointmentsAvailable;

		public SearchPageViewModel(IPatientSelectorViewModel patientSelectorViewModel,
								   ISharedStateReadOnly<Patient> selectedPatientVariable,
								   ISharedState<Date> selectedDateVariable,								                                     
								   IViewModelCommunication viewModelCommunication,
								   ICommandService commandService,
								   IClientReadModelRepository readModelRepository,	
								   IClientMedicalPracticeRepository medicalPracticeRepository,							   
								   Action<string> errorCallBack)
		{
		    this.selectedPatientVariable = selectedPatientVariable;		    
			this.viewModelCommunication = viewModelCommunication;
			this.commandService = commandService;			
			this.readModelRepository = readModelRepository;
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.errorCallBack = errorCallBack;			
			this.selectedDateVariable = selectedDateVariable;

			NoAppointmentsAvailable = false;

			selectedPatientVariable.StateChanged += OnSelectedPatientVariableChanged;

			PatientSelectorViewModel = patientSelectorViewModel;

			DeleteAppointment = new ParameterrizedCommand<DisplayAppointmentData>(DoDeleteAppointment);
			ModifyAppointment = new ParameterrizedCommand<DisplayAppointmentData>(DoModifyAppointment);
						
			SelectedPatient = NoPatientSelected;

			DisplayedAppointments = new ObservableCollection<DisplayAppointmentData>();
		}

		private void DoModifyAppointment(DisplayAppointmentData appointment)
		{
			viewModelCommunication.Send(new ShowPage(MainPage.Overview));

			viewModelCommunication.Send(new AsureDayIsLoaded(appointment.AppointmentRawData.MedicalPracticeId, 
															 appointment.Day, 
															 () =>
															 {
																 selectedDateVariable.Value = appointment.Day;

																 viewModelCommunication.SendTo(
																	Constants.ViewModelCollections.AppointmentViewModelCollection,
																	appointment.AppointmentRawData.Id,
																	new SwitchToEditMode()	
																 );															  
															 }));					
		}

		private async void DoDeleteAppointment(DisplayAppointmentData appointment)
		{
			var dialog = new UserDialogBox("", "Wollen Sie den Termin wirklich löschen?",
										   MessageBoxButton.OKCancel);
			var result = await dialog.ShowMahAppsDialog();

			if (result == MessageDialogResult.Affirmative)
			{
				medicalPracticeRepository.RequestPraticeVersion(
					practiceVersion =>
					{
						commandService.TryDeleteAppointment(
							operationSuccessful =>
							{
								if (!operationSuccessful)
									viewModelCommunication.Send(new ShowNotification("Termin kann nicht gelöscht werden", 5));
							},
							new AggregateIdentifier(appointment.Day,
													appointment.AppointmentRawData.MedicalPracticeId,
													practiceVersion),
							appointment.AppointmentRawData.PatientId,
							appointment.AppointmentRawData.Id,
							appointment.Description,
							appointment.AppointmentRawData.StartTime,
							appointment.AppointmentRawData.EndTime,
							appointment.AppointmentRawData.TherapyPlaceId,
							appointment.AppointmentRawData.LabelId,
							ActionTag.RegularAction,
							errorCallBack);
					},
					appointment.AppointmentRawData.MedicalPracticeId,
					appointment.Day,
					errorCallBack
				);				
			}
		}

		private void OnSelectedPatientVariableChanged(Patient patient)
		{
			if (currentReadModel != null)
			{
				currentReadModel.AppointmentChanged -= OnCurrentReadModelAppointmentsChanged;
				DisplayedAppointments.Clear();
			}

			ShowPreviousAppointments = false;

			if (patient != null)
			{
				SelectedPatient = patient.Name;

				readModelRepository.RequestAppointmentsOfAPatientReadModel(
					patientReadModel =>
					{						
						currentReadModel = patientReadModel;						
						currentReadModel.Appointments.Where(appointment => appointment.Day >= TimeTools.Today())
													 .Do(AddAppointment);

						currentReadModel.AppointmentChanged += OnCurrentReadModelAppointmentsChanged;
					},
					patient.Id,
					errorCallBack						
				);				
			}
			else
			{
				SelectedPatient = NoPatientSelected;
			}
		}

		private void AddAppointment(AppointmentTransferData newAppointmentData)
		{
			medicalPracticeRepository.RequestMedicalPractice(
				practice =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						DisplayedAppointments.Add(new DisplayAppointmentData(newAppointmentData, practice.Name));
						DisplayedAppointments.Sort(new AppointmentSorter());

						CheckDisplayedAppointmentCount();												
					});
				},
				newAppointmentData.MedicalPracticeId,
				newAppointmentData.Day,
				errorCallBack
			);
		}

		private void OnCurrentReadModelAppointmentsChanged(object sender, RawAppointmentChangedEventArgs appointmentChangedEventArgs)
		{
			if (!ShowPreviousAppointments && 
				appointmentChangedEventArgs.Appointment.Day < TimeTools.Today())
			{
				return;
			}

			switch (appointmentChangedEventArgs.ChangeAction)
			{
				case ChangeAction.Added:
				{	
					AddAppointment(appointmentChangedEventArgs.Appointment);
					break;
				}
				case ChangeAction.Deleted:
				{
					var appointmentToRemove = DisplayedAppointments.First(app => app.AppointmentRawData.Id == appointmentChangedEventArgs.Appointment.Id);
					DisplayedAppointments.Remove(appointmentToRemove);
					DisplayedAppointments.Sort(new AppointmentSorter());
					break;
				}
				case ChangeAction.Modified:
				{
					var oldAppointment = DisplayedAppointments.First(appointment => appointment.AppointmentRawData.Id == appointmentChangedEventArgs.Appointment.Id);
					DisplayedAppointments.Remove(oldAppointment);
					AddAppointment(appointmentChangedEventArgs.Appointment);
					break;
				}
			}						
		}

		private void CheckDisplayedAppointmentCount()
		{
			if (DisplayedAppointments != null)
			{
				NoAppointmentsAvailable = DisplayedAppointments.Count == 0;
			}
		}
		
		public ICommand DeleteAppointment { get; }
		public ICommand ModifyAppointment { get; }		

		public IPatientSelectorViewModel PatientSelectorViewModel { get; }

		public bool ShowPreviousAppointments
		{
			get { return showPreviousAppointments; }
			set
			{
				if (showPreviousAppointments == value)
					return;

				if (value)
				{
					currentReadModel.Appointments.Where(appointment => DisplayedAppointments.All(app => app.AppointmentRawData.Id != appointment.Id))
												 .Do(AddAppointment);

					DisplayedAppointments.Sort(new AppointmentSorter());
				}
				else
				{
					DisplayedAppointments.Where(appointment => appointment.Day < TimeTools.Today())
										 .ToList()
										 .Do(appointment => DisplayedAppointments.Remove(appointment));
				}

				CheckDisplayedAppointmentCount();

				PropertyChanged.ChangeAndNotify(this, ref showPreviousAppointments, value);
			}
		}

		public bool NoAppointmentsAvailable
		{
			get { return noAppointmentsAvailable; }
			private set { PropertyChanged.ChangeAndNotify(this, ref noAppointmentsAvailable, value); }
		}

		public string SelectedPatient
		{
			get { return selectedPatient; }
			private set { PropertyChanged.ChangeAndNotify(this, ref selectedPatient, value); }
		}

		public ObservableCollection<DisplayAppointmentData> DisplayedAppointments { get; }
		
	    protected override void CleanUp()
	    {
            selectedPatientVariable.StateChanged -= OnSelectedPatientVariableChanged;
        }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}