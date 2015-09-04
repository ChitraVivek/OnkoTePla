﻿using System.Collections.ObjectModel;
using System.Windows.Media;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.OnkoTePla.Client.WPFVisualization.Global;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AppointmentView;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Base;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.TherapyPlaceRowView.Helper;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.TherapyPlaceRowView.Messages;
using bytePassion.OnkoTePla.Contracts.Infrastructure;


namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.TherapyPlaceRowView
{
	public class TherapyPlaceRowViewModel : ITherapyPlaceRowViewModel,
											IViewModelMessageHandler<AddAppointmentToTherapyPlaceRow>,
											IViewModelMessageHandler<RemoveAppointmentFromTherapyPlaceRow>
	{		

		public TherapyPlaceRowViewModel(ViewModelCommunication<ViewModelMessage> viewModelCommunication,
										TherapyPlace therapyPlace, Color roomDisplayColor,										
										TherapyPlaceRowIdentifier identifier)
		{
			
			RoomColor        = roomDisplayColor;		
			Identifier       = identifier;			
			TherapyPlaceName = therapyPlace.Name;

			AppointmentViewModels = new ObservableCollection<IAppointmentViewModel>();	
			
			viewModelCommunication.RegisterViewModelAtCollection<TherapyPlaceRowViewModel, TherapyPlaceRowIdentifier>(
				Constants.TherapyPlaceRowViewModelCollection,
				this	
			);
		}

		public TherapyPlaceRowIdentifier Identifier { get; }

		public ObservableCollection<IAppointmentViewModel> AppointmentViewModels { get; }		
		
		public Color  RoomColor        { get; }		
		public string TherapyPlaceName { get; }
	

		public void Process(AddAppointmentToTherapyPlaceRow message)
		{
			AppointmentViewModels.Add(message.AppointmentViewModelToAdd);
		}

		public void Process(RemoveAppointmentFromTherapyPlaceRow message)
		{
			AppointmentViewModels.Remove(message.AppointmentViewModelToRemove);
		}
	}
}