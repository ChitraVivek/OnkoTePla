﻿using System.Collections.ObjectModel;
using bytePassion.Lib.WpfLib.ViewModelBase;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.OverviewPage
{
	internal interface IOverviewPageViewModel : IViewModel
    {   
		string ConnectionStatusText { get; }
		bool   IsConnectionActive   { get; }

		ObservableCollection<string> CurrentlyLoggedInUser { get; } 
    }
}