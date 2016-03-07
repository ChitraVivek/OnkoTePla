﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.Readmodels.Notification;
using bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.AppointmentViewModel;
using bytePassion.OnkoTePla.Client.WpfUi.Global;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModelMessages;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Contracts.Appointments;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using AppointmentsOfADayReadModel = bytePassion.OnkoTePla.Client.DataAndService.Domain.Readmodels.AppointmentsOfADayReadModel;
using Size = bytePassion.Lib.Types.SemanticTypes.Size;

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AppointmentGrid
{
	internal class AppointmentGridViewModel : ViewModel,
											  IAppointmentGridViewModel											
	{
		private bool isActive;

		private readonly ClientMedicalPracticeData medicalPractice;		
		private readonly IViewModelCommunication viewModelCommunication;		
		private readonly ISharedStateReadOnly<Size> gridSizeVariable;
		private readonly ISharedStateReadOnly<Guid?> roomFilterVariable;
		private readonly ISharedStateReadOnly<Guid> displayedMedicalPracticeVariable;
		private readonly IAppointmentViewModelBuilder appointmentViewModelBuilder;
		private readonly Action<string> errorCallback;
		  
		private readonly AppointmentsOfADayReadModel readModel;
		private ITimeGridViewModel timeGridViewModel;
		private ObservableCollection<ITherapyPlaceRowViewModel> therapyPlaceRowViewModels;
		 
		public AppointmentGridViewModel(AggregateIdentifier identifier, 
									    ClientMedicalPracticeData medicalPractice, 																				
										IViewModelCommunication viewModelCommunication,
                                        ISharedStateReadOnly<Size> gridSizeVariable,
										ISharedStateReadOnly<Guid?> roomFilterVariable,
										ISharedStateReadOnly<Guid> displayedMedicalPracticeVariable,
										IAppointmentViewModelBuilder appointmentViewModelBuilder,										
										AppointmentsOfADayReadModel readModel,
										IEnumerable<ITherapyPlaceRowViewModel> therapyPlaceRowViewModels,
										Action<string> errorCallback)
		{
			this.medicalPractice = medicalPractice;			
			this.viewModelCommunication = viewModelCommunication;
		    this.gridSizeVariable = gridSizeVariable;
		    this.roomFilterVariable = roomFilterVariable;
			this.displayedMedicalPracticeVariable = displayedMedicalPracticeVariable;
			this.appointmentViewModelBuilder = appointmentViewModelBuilder;
			this.readModel = readModel;
			this.errorCallback = errorCallback;

			// Initial appointment-Creation is done at the AppointmentGridBuilder

	        IsActive = false;
			PracticeIsClosedAtThisDay = false;

			gridSizeVariable.StateChanged   += OnGridSizeChanged;			
			roomFilterVariable.StateChanged += OnGlobalRoomFilterVariableChanged;

			viewModelCommunication.RegisterViewModelAtCollection<IAppointmentGridViewModel, AggregateIdentifier>(
				Constants.ViewModelCollections.AppointmentGridViewModelCollection,
				this					
			);

			Identifier = identifier;

			readModel.AppointmentChanged += OnReadModelAppointmentChanged;

			TimeGridViewModel = new TimeGridViewModel(Identifier, viewModelCommunication,
													  medicalPractice, gridSizeVariable.Value);

			TherapyPlaceRowViewModels = new ObservableCollection<ITherapyPlaceRowViewModel>(therapyPlaceRowViewModels);
			

			OnGlobalRoomFilterVariableChanged(roomFilterVariable.Value);
		}

		private void OnGlobalRoomFilterVariableChanged(Guid? newRoomFilter)
		{
			if (IsActive)
			{
				if (displayedMedicalPracticeVariable.Value == medicalPractice.Id)
				{
					if (newRoomFilter == null)
					{
						TherapyPlaceRowViewModels.Do(viewModel =>
						{
							viewModelCommunication.SendTo(
								Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
								viewModel.Identifier,
								new SetVisibility(true)
								);
						});
					}
					else
					{
						TherapyPlaceRowViewModels.Do(viewModel =>
						{
							viewModelCommunication.SendTo(
								Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
								viewModel.Identifier,
								new SetVisibility(false)
								);
						});

						medicalPractice.GetRoomById(newRoomFilter.Value)
									   .TherapyPlaces
									   .Select(therapyPlace => therapyPlace.Id)								   
									   .Do(id =>
										   {
								   				viewModelCommunication.SendTo(
								   					Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
								   					new TherapyPlaceRowIdentifier(Identifier, id),
								   					new SetVisibility(true)
								   				);
										   });
					}
				}
			}
		}

		private void OnReadModelAppointmentChanged(object sender, AppointmentChangedEventArgs appointmentChangedEventArgs)
		{
			switch (appointmentChangedEventArgs.ChangeAction)
			{
				case ChangeAction.Added:   { AddAppointment(appointmentChangedEventArgs.Appointment);    break; }
				case ChangeAction.Deleted: { RemoveAppointment(appointmentChangedEventArgs.Appointment); break; }
				case ChangeAction.Modified:
				{					
					RemoveAppointment(appointmentChangedEventArgs.Appointment);
					AddAppointment(appointmentChangedEventArgs.Appointment);
					break;
				}
			}
		}

		private void AddAppointment(Appointment newAppointment)
		{
			appointmentViewModelBuilder.Build(newAppointment, Identifier, errorCallback);			
		}
		
		private void RemoveAppointment(Appointment appointmentToRemove)
		{
			viewModelCommunication.SendTo( 
				Constants.ViewModelCollections.AppointmentViewModelCollection, 
				appointmentToRemove.Id, 
				new Dispose()
			);
		}

		private void OnGridSizeChanged(Size newGridSize)
		{
			if (IsActive)
			{
				viewModelCommunication.SendTo(
					Constants.ViewModelCollections.TimeGridViewModelCollection,
					Identifier,
					new NewSizeAvailable(newGridSize)	
				);

				foreach (var therapyPlaceRowIdentifier in TherapyPlaceRowViewModels.Select(viewModel => viewModel.Identifier))
				{ 
					viewModelCommunication.SendTo(
						Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
						therapyPlaceRowIdentifier,
						new NewSizeAvailable(newGridSize)
					);
				}							
			}
		}

		public AggregateIdentifier Identifier { get; }

		public ObservableCollection<ITherapyPlaceRowViewModel> TherapyPlaceRowViewModels
		{
			get { return therapyPlaceRowViewModels; }
			private set { PropertyChanged.ChangeAndNotify(this, ref therapyPlaceRowViewModels, value); }
		}

		public ITimeGridViewModel TimeGridViewModel
		{
			get { return timeGridViewModel; }
			private set { PropertyChanged.ChangeAndNotify(this, ref timeGridViewModel, value); }
		}

		public bool PracticeIsClosedAtThisDay { get; }

		public bool IsActive
		{
			get { return isActive; }
			private set
			{
				PropertyChanged.ChangeAndNotify(this, ref isActive, value);
			}
		}


		public void Process(Activate message)
		{
			IsActive = true;
			OnGridSizeChanged(gridSizeVariable.Value);
			OnGlobalRoomFilterVariableChanged(roomFilterVariable.Value);
		}

		public void Process(Deactivate message)
		{
			IsActive = false;
		}        
		
        protected override void CleanUp()
        {
            gridSizeVariable.StateChanged -= OnGridSizeChanged;
            roomFilterVariable.StateChanged -= OnGlobalRoomFilterVariableChanged;
            readModel.AppointmentChanged -= OnReadModelAppointmentChanged;

            viewModelCommunication.DeregisterViewModelAtCollection<IAppointmentGridViewModel, AggregateIdentifier>(
                Constants.ViewModelCollections.AppointmentGridViewModelCollection,
                this
            );

            viewModelCommunication.SendTo(
                Constants.ViewModelCollections.TimeGridViewModelCollection,
                Identifier,
                new Dispose()
            );

            readModel.Appointments
                     .Do(RemoveAppointment);

            readModel.Dispose();

            TherapyPlaceRowViewModels.Do(viewModel => viewModel.Dispose());
        }

        public override event PropertyChangedEventHandler PropertyChanged;		
	}
}