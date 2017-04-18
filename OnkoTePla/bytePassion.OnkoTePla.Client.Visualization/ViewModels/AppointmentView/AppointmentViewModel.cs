using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Utils;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.CommandSrv;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.AppointmentModification;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.Views;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView
{
	internal class AppointmentViewModel : ViewModel, IAppointmentViewModel										
	{		
		private readonly Appointment appointment;		
		private readonly TherapyPlaceRowIdentifier initialLocalisation;

		private readonly ISharedState<AppointmentModifications> appointmentModificationsVariable;
	    private readonly ISharedState<Date> selectedDateVariable;		

		private TherapyPlaceRowIdentifier currentLocation;
		private OperatingMode operatingMode;
		
		private Time beginTime;
		private Time endTime;
		
		private bool showDisabledOverlay;
		private AppointmentModifications currentAppointmentModifications;
        private string description;
		private Color labelColor;
		private string labelName;

		public AppointmentViewModel(Appointment appointment,
									ICommandService commandService,
									IViewModelCommunication viewModelCommunication,																
									TherapyPlaceRowIdentifier initialLocalisation, 
                                    ISharedState<AppointmentModifications> appointmentModificationsVariable,
                                    ISharedState<Date> selectedDateVariable,									
                                    IAppointmentModificationsBuilder appointmentModificationsBuilder,
                                    IWindowBuilder<EditDescription> editDescriptionWindowBuilder,
                                    AdornerControl adornerControl,
									Action<string> errorCallback)
		{ 						
			this.appointment = appointment;	        
	        this.initialLocalisation = initialLocalisation;
		    this.appointmentModificationsVariable = appointmentModificationsVariable;
		    this.selectedDateVariable = selectedDateVariable;	        
	        ViewModelCommunication = viewModelCommunication;
			AdornerControl = adornerControl;					

			viewModelCommunication.RegisterViewModelAtCollection<IAppointmentViewModel, Guid>(
				Constants.ViewModelCollections.AppointmentViewModelCollection,
				this	
			);

			SwitchToEditMode = new ParameterrizedCommand<bool>(
				isInitalAdjusting =>
				{
					if (appointmentModificationsVariable.Value == null)
					{
					    CurrentAppointmentModifications = appointmentModificationsBuilder.Build(appointment,
					                                                                            initialLocalisation.PlaceAndDate.MedicalPracticeId,
					                                                                            isInitalAdjusting,
																								errorCallback); 
						
						CurrentAppointmentModifications.PropertyChanged += OnAppointmentModificationsPropertyChanged;
						appointmentModificationsVariable.Value = CurrentAppointmentModifications;
						OperatingMode = OperatingMode.Edit;
						appointmentModificationsVariable.StateChanged += OnCurrentModifiedAppointmentChanged;
					}
				}
			);

			DeleteAppointment = new Command(
				async() =>
				{
					var dialog = new UserDialogBox("", "Wollen Sie den Termin wirklich löschen?", 
												   MessageBoxButton.OKCancel);
					var result = await dialog.ShowMahAppsDialog();

				    if (result == MessageDialogResult.Affirmative)
				    {
					    if (appointmentModificationsVariable.Value.IsInitialAdjustment)
					    {
						    viewModelCommunication.SendTo(											//
							    Constants.ViewModelCollections.AppointmentViewModelCollection,		// do nothing but
							    appointmentModificationsVariable.Value.OriginalAppointment.Id,		// deleting the temporarly
							    new Dispose()														// created Appointment
							);																		//
					    }
					    else
					    {
							commandService.TryDeleteAppointment(
								operationSuccessful =>
								{
									Application.Current.Dispatcher.Invoke(() =>
									{
										if (!operationSuccessful)
										{
											Process(new RestoreOriginalValues());
											viewModelCommunication.Send(new ShowNotification("löschen des Termins fehlgeschlagen; bearbeitung wurde zurückgesetzt", 5));
										}
									});
								},
								currentLocation.PlaceAndDate,
								appointment.Patient.Id,
								appointment.Id,
								appointment.Description,
								appointment.StartTime,
								appointment.EndTime,
								appointment.TherapyPlace.Id,
								appointment.Label.Id,
								ActionTag.RegularAction,
								errorCallback
							);
						}

					    appointmentModificationsVariable.Value = null;						
					}					
				}
			);	

            EditDescription = new Command( 
				() =>
				{
				    viewModelCommunication.Send(new ShowDisabledOverlay());

				    var dialog = editDescriptionWindowBuilder.BuildWindow();
				    dialog.ShowDialog();

				    viewModelCommunication.Send(new HideDisabledOverlay());
				}
			);

			ConfirmChanges = new Command(() => viewModelCommunication.Send(new ConfirmChanges()));
			RejectChanges  = new Command(() => viewModelCommunication.Send(new RejectChanges()));

			BeginTime = appointment.StartTime;
			EndTime = appointment.EndTime;
            Description = appointment.Description;
			LabelColor = appointment.Label.Color;
			LabelName = appointment.Label.Name;

			ShowDisabledOverlay = false;
			
			SetNewLocation(initialLocalisation, true);		
		}

		private void OnAppointmentModificationsPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var appointmentModifications = (AppointmentModifications)sender;
			switch (propertyChangedEventArgs.PropertyName)
			{
				case nameof(AppointmentModifications.BeginTime):
				{
					BeginTime = appointmentModifications.BeginTime;
					break;
				}
				case nameof(AppointmentModifications.EndTime):
				{
					EndTime = appointmentModifications.EndTime;
					break;
				}				
				case nameof(AppointmentModifications.CurrentLocation):
				{
					SetNewLocation(appointmentModifications.CurrentLocation, false);
					break;
				}
                case nameof(AppointmentModifications.Description):
				{
					Description = appointmentModifications.Description;
			        break;
			    }
				case nameof(AppointmentModifications.Label):
				{
					LabelColor = appointmentModifications.Label.Color;
					LabelName = appointmentModifications.Label.Name;
					break;
				}
			}
		}

		private void OnCurrentModifiedAppointmentChanged(AppointmentModifications newModifiedAppointment)
		{
			if (newModifiedAppointment == null || appointment != newModifiedAppointment.OriginalAppointment)
			{
				OperatingMode = OperatingMode.View;
				CurrentAppointmentModifications.PropertyChanged -= OnAppointmentModificationsPropertyChanged;
				appointmentModificationsVariable.StateChanged -= OnCurrentModifiedAppointmentChanged;
			}
		}


		private void SetNewLocation(TherapyPlaceRowIdentifier therapyPlaceRowIdentifier, bool isInitialLocation)
		{
			if (!isInitialLocation)
			{
				ViewModelCommunication.SendTo(
					Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
					currentLocation,
					new RemoveAppointmentFromTherapyPlaceRow(this)
				);
			}

			currentLocation = therapyPlaceRowIdentifier;

			ViewModelCommunication.SendTo(
				Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
				therapyPlaceRowIdentifier,
				new AddAppointmentToTherapyPlaceRow(this)	
			);				
		}


		public Guid Identifier => appointment.Id;
		
		public ICommand DeleteAppointment { get; }
		public ICommand SwitchToEditMode  { get; }
        public ICommand EditDescription   { get; }
		public ICommand ConfirmChanges    { get; }
		public ICommand RejectChanges     { get; }

		public Time BeginTime
		{
			get { return beginTime; }
			private set { PropertyChanged.ChangeAndNotify(this, ref beginTime, value); }
		}

		public Time EndTime
		{
			get { return endTime; }
			private set { PropertyChanged.ChangeAndNotify(this, ref endTime, value); }
		}

		public Color LabelColor
		{
			get { return labelColor; }
			private set { PropertyChanged.ChangeAndNotify(this, ref labelColor, value); }
		}

		public string LabelName
		{
			get { return labelName; }
			private set { PropertyChanged.ChangeAndNotify(this, ref labelName, value); }
		}

		public string ToolTipNameWithBirthday => $"{appointment.Patient.Name} ({appointment.Patient.Birthday})";
		public string PatientDisplayName      => $"{appointment.Patient.Name} (*{appointment.Patient.Birthday.Year})";
        public string TimeSpan                => $"{appointment.StartTime.ToString().Substring(0, 5)} - {appointment.EndTime.ToString().Substring(0, 5)}";
		public string AppointmentDate         => appointment.Day.ToString();		
		public string Room                    => appointment.TherapyPlace.Name;		

		public string Description
        {
            get { return description; }
            set
            {
                PropertyChanged.ChangeAndNotify(this, ref description, value);
                    
            }
        }

        public AppointmentModifications CurrentAppointmentModifications												// TODO: evtl noch benachrichtigung von der Variable
		{
			get { return currentAppointmentModifications; }
			private set { PropertyChanged.ChangeAndNotify(this, ref currentAppointmentModifications, value); }
		}

		public AdornerControl AdornerControl { get; }


		public OperatingMode OperatingMode
		{
			get { return operatingMode; }
			private set { PropertyChanged.ChangeAndNotify(this, ref operatingMode, value); }
		}

		public bool ShowDisabledOverlay
		{
			get { return showDisabledOverlay; }
			private set { PropertyChanged.ChangeAndNotify(this, ref showDisabledOverlay, value); }
		}

		#region process messages

		public void Process (Dispose message)
		{
			Dispose();
		}
		
		public void Process (RestoreOriginalValues message)
		{
			BeginTime   = appointment.StartTime;
			EndTime     = appointment.EndTime;
			Description = appointment.Description;
			LabelColor  = appointment.Label.Color;
			LabelName   = appointment.Label.Name;

			if (initialLocalisation != currentLocation)			
				SetNewLocation(initialLocalisation, false);
			
            selectedDateVariable.Value = appointment.Day;
		}

		public void Process (ShowDisabledOverlay message)
		{
			ShowDisabledOverlay = true;
		}

		public void Process (HideDisabledOverlay message)
		{
			ShowDisabledOverlay = false;
		}

		public void Process (SwitchToEditMode message)
		{
			SwitchToEditMode.Execute(false);
		}

		#endregion

		protected override void CleanUp()
		{
			ViewModelCommunication.DeregisterViewModelAtCollection<AppointmentViewModel, Guid>(
				Constants.ViewModelCollections.AppointmentViewModelCollection,
				this
			);

			ViewModelCommunication.SendTo(
				Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
				currentLocation,
				new RemoveAppointmentFromTherapyPlaceRow(this)
			);
		}

		public IViewModelCommunication ViewModelCommunication { get; }
        
		public override event PropertyChangedEventHandler PropertyChanged;		
	}	
}
