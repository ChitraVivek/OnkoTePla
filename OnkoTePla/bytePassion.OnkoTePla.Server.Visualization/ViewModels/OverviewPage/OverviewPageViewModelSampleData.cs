using System.Collections.ObjectModel;
using System.ComponentModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.OverviewPage
{
	internal class OverviewPageViewModelSampleData : IOverviewPageViewModel
    {
		public OverviewPageViewModelSampleData()
		{
			ConnectionStatusText  = "inactive";			
			IsConnectionActive = false;

			CurrentlyLoggedInUser = new ObservableCollection<string>
			{
				"user1",
				"user2",
				"user3"
			};
		}

		public string ConnectionStatusText { get; }
		public bool   IsConnectionActive   { get; }

		public ObservableCollection<string> CurrentlyLoggedInUser { get; }

		public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;		
    }
}