using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.TherapyPlaceTypeRepository;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Contracts.Infrastructure;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.TherapyPlaceRowView
{
	internal class TherapyPlaceRowViewModel : ViewModel,
											  ITherapyPlaceRowViewModel
	{
		private readonly IViewModelCommunication viewModelCommunication;
		private readonly IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository;
		private readonly TherapyPlace therapyPlace;
		private readonly ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable;
		

		private double gridWidth;
		private bool isVisible;
		private AppointmentModifications appointmentModifications;
		private ImageSource placeTypeIcon;

		public TherapyPlaceRowViewModel(IViewModelCommunication viewModelCommunication,
										IClientTherapyPlaceTypeRepository therapyPlaceTypeRepository, 									    
										TherapyPlace therapyPlace, 
										Color roomDisplayColor,										
										TherapyPlaceRowIdentifier identifier,
										AdornerControl adornerControl,
										Time timeSlotBegin,
										Time timeSlotEnd,
										ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable,
										Width initialGridWidth,
										Action<string> errorCallback)
		{
			this.viewModelCommunication = viewModelCommunication;
			this.therapyPlaceTypeRepository = therapyPlaceTypeRepository;
			this.therapyPlace = therapyPlace;
			this.appointmentModificationsVariable = appointmentModificationsVariable;						
			
			therapyPlaceTypeRepository.UpdatedTherapyPlaceTypeAvailable += OnUpdatedTherapyPlaceTypeAvailable;

			IsVisible		      = true;
			RoomColor             = roomDisplayColor;		
			Identifier            = identifier;			
			TherapyPlaceName      = therapyPlace.Name;
			AppointmentViewModels = new ObservableCollection<IAppointmentViewModel>();	
			
			viewModelCommunication.RegisterViewModelAtCollection<ITherapyPlaceRowViewModel, TherapyPlaceRowIdentifier>(
				Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
				this	
			);

			appointmentModificationsVariable.StateChanged += OnAppointmentModificationsChanged;
			OnAppointmentModificationsChanged(appointmentModificationsVariable.Value);

			TimeSlotBegin = timeSlotBegin;
			TimeSlotEnd   = timeSlotEnd;

			AdornerControl = adornerControl;

			GridWidth = initialGridWidth;

			therapyPlaceTypeRepository.RequestTherapyPlaceTypes(
				placeType =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						PlaceTypeIcon = GetIconForTherapyPlaceType(placeType.IconType);
					});					
				},
				therapyPlace.TypeId,
				errorCallback
			);
		}

		private void OnUpdatedTherapyPlaceTypeAvailable(TherapyPlaceType updatedTherapyPlaceType)
		{
			if (updatedTherapyPlaceType.Id == therapyPlace.TypeId)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					PlaceTypeIcon = GetIconForTherapyPlaceType(updatedTherapyPlaceType.IconType);
				});
			}
		}

		private void OnAppointmentModificationsChanged(AppointmentModifications newAppointmentModifications)
		{
			AppointmentModifications = newAppointmentModifications;
		}

		public TherapyPlaceRowIdentifier Identifier { get; }

		public ObservableCollection<IAppointmentViewModel> AppointmentViewModels { get; }		
		
		public Color  RoomColor        { get; }		
		public string TherapyPlaceName { get; }

		public ImageSource PlaceTypeIcon
		{
			get { return placeTypeIcon; }
			private set { PropertyChanged.ChangeAndNotify(this, ref placeTypeIcon, value); }
		}

		public Time TimeSlotBegin { get; }
		public Time TimeSlotEnd   { get; }

		public double GridWidth
		{
			get { return gridWidth; }
			private set { PropertyChanged.ChangeAndNotify(this, ref gridWidth, value); }
		}

		public AdornerControl AdornerControl { get; }

		public AppointmentModifications AppointmentModifications
		{
			get { return appointmentModifications; }
			private set { PropertyChanged.ChangeAndNotify(this, ref appointmentModifications, value); }
		}		

		public bool IsVisible
		{
			get { return isVisible; }
			private set { PropertyChanged.ChangeAndNotify(this, ref isVisible, value); }
		}

		public void Process (NewSizeAvailable message)
		{
			GridWidth = message.NewSize.Width;
		}

		public void Process(AddAppointmentToTherapyPlaceRow message)
		{
			AppointmentViewModels.Add(message.AppointmentViewModelToAdd);
		}

		public void Process(RemoveAppointmentFromTherapyPlaceRow message)
		{
			AppointmentViewModels.Remove(message.AppointmentViewModelToRemove);
		}

		public void Process (SetVisibility message)
		{
			IsVisible = message.Visible;
		}

		private static ImageSource GetIconForTherapyPlaceType(TherapyPlaceTypeIcon iconType)
		{
			const string basePath = "pack://application:,,,/bytePassion.OnkoTePla.Resources;component/Icons/TherapyPlaceType/";

			switch (iconType)
			{
				case TherapyPlaceTypeIcon.BedType1:   return ImageLoader.LoadImage(new Uri(basePath + "bed01.png"));
				case TherapyPlaceTypeIcon.BedType2:   return ImageLoader.LoadImage(new Uri(basePath + "bed02.png"));
				case TherapyPlaceTypeIcon.BedType3:   return ImageLoader.LoadImage(new Uri(basePath + "bed03.png"));
				case TherapyPlaceTypeIcon.BedType4:   return ImageLoader.LoadImage(new Uri(basePath + "bed04.png"));
				case TherapyPlaceTypeIcon.BedType5:   return ImageLoader.LoadImage(new Uri(basePath + "bed05.png"));
				case TherapyPlaceTypeIcon.ChairType1: return ImageLoader.LoadImage(new Uri(basePath + "chair01.png"));
				case TherapyPlaceTypeIcon.ChairType2: return ImageLoader.LoadImage(new Uri(basePath + "chair02.png"));
				case TherapyPlaceTypeIcon.ChairType3: return ImageLoader.LoadImage(new Uri(basePath + "chair03.png"));
				case TherapyPlaceTypeIcon.ChairType4: return ImageLoader.LoadImage(new Uri(basePath + "chair04.png"));
				case TherapyPlaceTypeIcon.ChairType5: return ImageLoader.LoadImage(new Uri(basePath + "chair05.png"));
				case TherapyPlaceTypeIcon.None:       return ImageLoader.LoadImage(new Uri(basePath + "none.png"));

				default:
					throw new ArgumentException();
			}			
		}

        protected override void CleanUp()
		{
			viewModelCommunication.DeregisterViewModelAtCollection<TherapyPlaceRowViewModel, TherapyPlaceRowIdentifier>(
				Constants.ViewModelCollections.TherapyPlaceRowViewModelCollection,
				this
			);

			appointmentModificationsVariable.StateChanged -= OnAppointmentModificationsChanged;
			therapyPlaceTypeRepository.UpdatedTherapyPlaceTypeAvailable -= OnUpdatedTherapyPlaceTypeAvailable;
		}		

		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}