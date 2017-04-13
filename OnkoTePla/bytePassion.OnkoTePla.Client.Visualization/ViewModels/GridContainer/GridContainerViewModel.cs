using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.AppointmentGridViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid;
using bytePassion.OnkoTePla.Contracts.Domain;
using Size = bytePassion.Lib.Types.SemanticTypes.Size;


namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.GridContainer
{
	internal class GridContainerViewModel : ViewModel, 
                                            IGridContainerViewModel
	{
		private readonly IAppointmentGridViewModelBuilder appointmentGridViewModelBuilder;				
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly ISharedStateReadOnly<Date> selectedDateVariable;
		private readonly ISharedStateReadOnly<Guid> selectedMedicalPracticeIdVariable;
	    private readonly ISharedState<Size> appointmentGridSizeVariable;
		private readonly Action<string> errorCallback;


		private readonly IDictionary<AggregateIdentifier, IAppointmentGridViewModel> cachedAppointmentGridViewModels; 

		private int                  currentDisplayedAppointmentGridIndex;
		private AggregateIdentifier? currentDisplayedAppointmentGridIdentifier;
		
		public GridContainerViewModel(IViewModelCommunication viewModelCommunication,
									  IClientMedicalPracticeRepository medicalPracticeRepository,
                                      ISharedStateReadOnly<Date> selectedDateVariable, 
                                      ISharedStateReadOnly<Guid> selectedMedicalPracticeIdVariable,
									  ISharedState<Size> appointmentGridSizeVariable,									  
                                      IEnumerable<AggregateIdentifier> initialGridViewModelsToCache,									  
									  int maximumCashedGrids, /* TODO */
									  IAppointmentGridViewModelBuilder appointmentGridViewModelBuilder,
									  Action<string> errorCallback)
		{
			// TODO caching implementieren

			this.viewModelCommunication = viewModelCommunication;
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.selectedDateVariable = selectedDateVariable;
		    this.selectedMedicalPracticeIdVariable = selectedMedicalPracticeIdVariable;
		    this.appointmentGridSizeVariable = appointmentGridSizeVariable;		    
			this.appointmentGridViewModelBuilder = appointmentGridViewModelBuilder;
			this.errorCallback = errorCallback;

			LoadedAppointmentGrids          = new ObservableCollection<IAppointmentGridViewModel>();
			cachedAppointmentGridViewModels = new Dictionary<AggregateIdentifier, IAppointmentGridViewModel>();

			currentDisplayedAppointmentGridIdentifier = null;			
			
			foreach (var identifier in initialGridViewModelsToCache)
			{
				AddGridViewModel(identifier);
			}

			selectedDateVariable.StateChanged              += OnSelectedDateStateChanged;
			selectedMedicalPracticeIdVariable.StateChanged += OnDisplayedPracticeStateChanged;
			
			medicalPracticeRepository.RequestPraticeVersion(
				practiceVersion =>
				{
					var newIdentifier = new AggregateIdentifier(selectedDateVariable.Value, selectedMedicalPracticeIdVariable.Value, practiceVersion);
					TryToShowGridViewModel(newIdentifier);
				},
				selectedMedicalPracticeIdVariable.Value,
				selectedDateVariable.Value,
				errorCallback
			);
		}		

		public ObservableCollection<IAppointmentGridViewModel> LoadedAppointmentGrids { get; }		

		public int CurrentDisplayedAppointmentGridIndex
		{
			get { return currentDisplayedAppointmentGridIndex; }
			private set { PropertyChanged.ChangeAndNotify(this, ref currentDisplayedAppointmentGridIndex, value); }
		}

		public Size ReportedGridSize
		{
			set
			{
				if (value != null)
					appointmentGridSizeVariable.Value = value;
			}
		}

		private void AddGridViewModel(AggregateIdentifier identifier)
		{
			if (!cachedAppointmentGridViewModels.ContainsKey(identifier))
			{
				cachedAppointmentGridViewModels.Add(identifier, null);

				appointmentGridViewModelBuilder.RequestBuild(
					buildedViewModel =>
					{
						Application.Current.Dispatcher.Invoke(() =>
						{							
							cachedAppointmentGridViewModels[identifier] = buildedViewModel;
							LoadedAppointmentGrids.Add(buildedViewModel);							
								
							if (identifier.Date == selectedDateVariable.Value &&
								identifier.MedicalPracticeId == selectedMedicalPracticeIdVariable.Value)
							{
								DisplayCachedViewModel(identifier);
							}
						});						
					},
					identifier,
					errorCallback	
				);								
			}
		}

		private void RemoveGridViewModel(AggregateIdentifier identifier)
		{
			if (GridViewModelIsCached(identifier))
			{
				var gridViewModel = cachedAppointmentGridViewModels[identifier];
				cachedAppointmentGridViewModels.Remove(identifier);
				LoadedAppointmentGrids.Remove(gridViewModel);

				gridViewModel.Dispose();
			}
		}

		private bool GridViewModelIsCached(AggregateIdentifier identifier)
		{
			return cachedAppointmentGridViewModels.ContainsKey(identifier);
		}

		private int GetGridIndex(AggregateIdentifier identifier)
		{
			if (GridViewModelIsCached(identifier))
			{
				var gridViewModel = cachedAppointmentGridViewModels[identifier];
				return LoadedAppointmentGrids.IndexOf(gridViewModel);
			}

			throw new ArgumentException("viewModel is not cached");
		}				

		private void OnDisplayedPracticeStateChanged(Guid medicalPracticeId)
		{
			medicalPracticeRepository.RequestPraticeVersion(
				practiceVersion =>
				{
					var newIdentifier = new AggregateIdentifier(selectedDateVariable.Value, medicalPracticeId, practiceVersion);
					TryToShowGridViewModel(newIdentifier);
				},
				medicalPracticeId,
				selectedDateVariable.Value,
				errorCallback	
			);			
		}

		private void OnSelectedDateStateChanged (Date date)
		{
			medicalPracticeRepository.RequestPraticeVersion(
				practiceVersion =>
				{
					var newIdentifier = new AggregateIdentifier(date, selectedMedicalPracticeIdVariable.Value, practiceVersion);
					TryToShowGridViewModel(newIdentifier);
				},
				selectedMedicalPracticeIdVariable.Value,
				date,
				errorCallback
			);		
		}

		private void ActivateGridViewModel(AggregateIdentifier identifier)
		{
			viewModelCommunication.SendTo(
				Constants.ViewModelCollections.AppointmentGridViewModelCollection,
				identifier,
				new Activate()
			);
		}

		private void DeactivateGridViewModel (AggregateIdentifier identifier)
		{
			viewModelCommunication.SendTo(
				Constants.ViewModelCollections.AppointmentGridViewModelCollection,
				identifier,
				new Deactivate()
			);
		}		

		private void TryToShowGridViewModel(AggregateIdentifier identifier)
		{			
			if (currentDisplayedAppointmentGridIdentifier.HasValue)
				DeactivateGridViewModel(currentDisplayedAppointmentGridIdentifier.Value);

			if (!GridViewModelIsCached(identifier))
				AddGridViewModel(identifier);
			else			
				DisplayCachedViewModel(identifier);
						
		}

		private void DisplayCachedViewModel(AggregateIdentifier identifier)
		{
			CurrentDisplayedAppointmentGridIndex      = GetGridIndex(identifier);
			currentDisplayedAppointmentGridIdentifier = identifier;

			ActivateGridViewModel(identifier);
		}

		public void Process (AsureDayIsLoaded message)
		{			
			if (!cachedAppointmentGridViewModels.ContainsKey(new AggregateIdentifier(message.Day, message.MedicalPracticeId)))
			{
				cachedAppointmentGridViewModels.Add(new AggregateIdentifier(message.Day, message.MedicalPracticeId), null);

				medicalPracticeRepository.RequestPraticeVersion(
					practiceVersion =>
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							var identifier = new AggregateIdentifier(message.Day, message.MedicalPracticeId, practiceVersion);

							appointmentGridViewModelBuilder.RequestBuild(
								buildedViewModel =>
								{									
									cachedAppointmentGridViewModels[identifier] = buildedViewModel;
									LoadedAppointmentGrids.Add(buildedViewModel);
																			
									message.DayIsLoaded?.Invoke();
								},
								identifier,
								errorCallback
							);
						});
					},
					message.MedicalPracticeId,
					message.Day,
					errorCallback
				);										
			}
			else
			{
				message.DayIsLoaded?.Invoke();
			}
		}

		protected override void CleanUp()
	    {
	        selectedDateVariable.StateChanged              -= OnSelectedDateStateChanged;
			selectedMedicalPracticeIdVariable.StateChanged -= OnDisplayedPracticeStateChanged;
	    }
        public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
