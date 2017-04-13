using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TimeGrid;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentGrid
{
	internal class ClosedDayGridViewModel : ViewModel, 
										    IAppointmentGridViewModel
	{
		private bool isActive;
		
		private readonly IViewModelCommunication viewModelCommunication;		         
		private readonly ISharedStateReadOnly<Size> appointmentGridSizeVariable;
		
         
		public ClosedDayGridViewModel (AggregateIdentifier identifier,
									   ClientMedicalPracticeData medicalPractice,										
									   IViewModelCommunication viewModelCommunication,
									   ISharedStateReadOnly<Size> appointmentGridSizeVariable)
		{			
			this.viewModelCommunication = viewModelCommunication;
		    this.appointmentGridSizeVariable = appointmentGridSizeVariable;

			Identifier = identifier;
		    IsActive = false;

			viewModelCommunication.RegisterViewModelAtCollection<IAppointmentGridViewModel, AggregateIdentifier>(
				Constants.ViewModelCollections.AppointmentGridViewModelCollection,
				this
			);
			
			appointmentGridSizeVariable.StateChanged += OnGridSizeChanged;
			
			TimeGridViewModel = new TimeGridViewModel(Identifier, viewModelCommunication,
													  medicalPractice, appointmentGridSizeVariable.Value);

			TherapyPlaceRowViewModels = new ObservableCollection<ITherapyPlaceRowViewModel>();

			PracticeIsClosedAtThisDay = true;
		}				

		
		private void OnGridSizeChanged (Size newGridSize)
		{
			if (IsActive)
			{
				viewModelCommunication.SendTo(
					Constants.ViewModelCollections.TimeGridViewModelCollection,
					Identifier,
					new NewSizeAvailable(newGridSize)
				);			
			}
		}

		public AggregateIdentifier Identifier { get; }

		public ObservableCollection<ITherapyPlaceRowViewModel> TherapyPlaceRowViewModels { get; }

		public ITimeGridViewModel TimeGridViewModel { get; }

		public bool PracticeIsClosedAtThisDay { get; }

		public bool IsActive
		{
			get { return isActive; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isActive, value); }
		}


		public void Process (Activate message)
		{
			IsActive = true;
			OnGridSizeChanged(appointmentGridSizeVariable.Value);
		}

		public void Process (Deactivate message)
		{
			IsActive = false;
		}        			

        protected override void CleanUp()
        {
            appointmentGridSizeVariable.StateChanged -= OnGridSizeChanged;

            viewModelCommunication.DeregisterViewModelAtCollection<IAppointmentGridViewModel, AggregateIdentifier>(
                Constants.ViewModelCollections.AppointmentGridViewModelCollection,
                this
            );

            viewModelCommunication.SendTo(
                Constants.ViewModelCollections.TimeGridViewModelCollection,
                Identifier,
                new Dispose()
            );
        }

        public override event PropertyChangedEventHandler PropertyChanged;
	}
}