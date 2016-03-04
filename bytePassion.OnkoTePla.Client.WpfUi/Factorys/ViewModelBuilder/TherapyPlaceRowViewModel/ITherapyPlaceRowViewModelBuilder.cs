﻿using System;
using System.Collections.Generic;
using bytePassion.OnkoTePla.Client.WpfUi.ViewModels.TherapyPlaceRowView;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Infrastructure;


namespace bytePassion.OnkoTePla.Client.WpfUi.Factorys.ViewModelBuilder.TherapyPlaceRowViewModel
{
	internal interface ITherapyPlaceRowViewModelBuilder
	{		
		void RequestBuild(Action<IEnumerable<ITherapyPlaceRowViewModel>> viewModelsAvailable,
						  AggregateIdentifier identifier, IEnumerable<Room> rooms,
						  Action<string> errorCallback);
	}
}