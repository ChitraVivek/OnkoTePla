using System.ComponentModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView
{
	internal class MainViewModelSampleData : IMainViewModel
	{
		public MainViewModelSampleData()
		{
			SelectedPage = 0;

			OverviewPageViewModel = new OverviewPageViewModelSampleData();
			SearchPageViewModel   = new SearchPageViewModelSampleData();
			OptionsPageViewModel  = new OptionsPageViewModelSampleData();							
		}

		public int SelectedPage { get; }

		public ICommand ShowPage { get; } = null;

		public IOverviewPageViewModel OverviewPageViewModel { get; }
		public ISearchPageViewModel   SearchPageViewModel   { get; }
		public IOptionsPageViewModel  OptionsPageViewModel  { get; }		

		public void Process (ShowPage message) { }
		
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
