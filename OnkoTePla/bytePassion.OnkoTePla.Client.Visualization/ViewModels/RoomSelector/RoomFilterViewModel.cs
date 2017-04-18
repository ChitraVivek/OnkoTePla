using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector.Helper;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector
{
	internal class RoomFilterViewModel : ViewModel,
                                         IRoomFilterViewModel
	{
		private readonly RoomSelectorData allRoomFilter = new RoomSelectorData("Alle Räume", null, Colors.White);


		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly ISharedState<Guid?>         roomFilterVariable;
		private readonly ISharedStateReadOnly<Date>  selectedDateVariable;
		private readonly ISharedStateReadOnly<Guid>  displayedMedicalPracticeVariable;
		private readonly ISharedState<AppointmentModifications> appointmentModificationsVariable;
		private readonly Action<string> errorCallback;

		private IList<Room> currentSelectableRoomFilters;
		private RoomSelectorData selectedRoomFilter;
		private ObservableCollection<RoomSelectorData> availableRoomFilters;

		private ClientMedicalPracticeData currentMedicalPractice;

		public RoomFilterViewModel(IClientMedicalPracticeRepository medicalPracticeRepository,
								   ISharedState<Guid?> roomFilterVariable, 
                                   ISharedStateReadOnly<Date> selectedDateVariable, 
                                   ISharedStateReadOnly<Guid> displayedMedicalPracticeVariable,
								   ISharedState<AppointmentModifications> appointmentModificationsVariable,
								   Action<string> errorCallback)
		{
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.roomFilterVariable = roomFilterVariable;
		    this.selectedDateVariable = selectedDateVariable;
		    this.displayedMedicalPracticeVariable = displayedMedicalPracticeVariable;
			this.appointmentModificationsVariable = appointmentModificationsVariable;
			this.errorCallback = errorCallback;

			roomFilterVariable.StateChanged += OnRoomFilterVariableChanged;
			displayedMedicalPracticeVariable.StateChanged += OnDisplayedPracticeVariableStateChanged;						
			selectedDateVariable.StateChanged += OnSelectedDateVariableChanged;

			SetRoomData(selectedDateVariable.Value, displayedMedicalPracticeVariable.Value);
		}

		private void OnRoomFilterVariableChanged(Guid? guid)
		{
			if (guid == null)
				SelectedRoomFilter = allRoomFilter;
			else
			{
				medicalPracticeRepository.RequestMedicalPractice(
					medicalPractice =>
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							var room = medicalPractice.GetRoomById(guid.Value);

							selectedRoomFilter = new RoomSelectorData(room.Name, room.Id, room.DisplayedColor);
							PropertyChanged.Notify(this, nameof(SelectedRoomFilter));
						});						
					},
					displayedMedicalPracticeVariable.Value, 
					selectedDateVariable.Value,
					errorCallback
				);								
			}
		}

		private void OnSelectedDateVariableChanged(Date date)
		{			
			SetRoomData(date, displayedMedicalPracticeVariable.Value);
		}
		
		private void OnDisplayedPracticeVariableStateChanged (Guid medicalPracticeId)
		{			
			SetRoomData(selectedDateVariable.Value, medicalPracticeId);
		}

		private void SetRoomData (Date date, Guid medicalPracticeId)
		{
			if (medicalPracticeId == Guid.Empty)
				return;

			medicalPracticeRepository.RequestMedicalPractice(
				medicalPractice =>
				{
					if (medicalPractice.Id == currentMedicalPractice?.Id &&
						medicalPractice.Version == currentMedicalPractice?.Version)
						return;

					Application.Current.Dispatcher.Invoke(() =>
					{
						currentMedicalPractice = medicalPractice;
						currentSelectableRoomFilters = medicalPractice.Rooms.ToList();

						AvailableRoomFilters = currentSelectableRoomFilters.Select(room => new RoomSelectorData(room.Name, room.Id, room.DisplayedColor))
																		   .Append(allRoomFilter)
																		   .ToObservableCollection();

						SelectedRoomFilter = AvailableRoomFilters.Last();
					});
				},
				medicalPracticeId,
				date,
				errorCallback
			);					
		}


		public ObservableCollection<RoomSelectorData> AvailableRoomFilters
		{
			get { return availableRoomFilters; }
			private set { PropertyChanged.ChangeAndNotify(this, ref availableRoomFilters, value);}
		}

		public RoomSelectorData SelectedRoomFilter
		{
			get { return selectedRoomFilter; }
			set
			{
				if (value == null) return; 

				if (value != selectedRoomFilter)
				{
					roomFilterVariable.Value = value.RoomId;
                    PropertyChanged.ChangeAndNotify(this, ref selectedRoomFilter, value);
                }				
			}
		}

	    public bool CheckSelectionValidity(RoomSelectorData data)
	    {
            if(appointmentModificationsVariable.Value == null || data == allRoomFilter)
            {
                return true;
            }

	        return data.RoomId != null && 
				   currentMedicalPractice.GetRoomById(data.RoomId.Value)
										 .TherapyPlaces
										 .Select(therapyPlace => therapyPlace.Id)
										 .Contains(appointmentModificationsVariable.Value.CurrentLocation.TherapyPlaceId);
	    }


        protected override void CleanUp()
        {
            roomFilterVariable.StateChanged               -= OnRoomFilterVariableChanged;
            displayedMedicalPracticeVariable.StateChanged -= OnDisplayedPracticeVariableStateChanged;
            selectedDateVariable.StateChanged             -= OnSelectedDateVariableChanged;
        }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
