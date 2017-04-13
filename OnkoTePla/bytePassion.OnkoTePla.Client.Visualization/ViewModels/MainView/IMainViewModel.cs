using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView
{
	internal interface IMainViewModel : IViewModel,
							            IViewModelMessageHandler<ShowPage>

	{
		int SelectedPage { get; }			

		IOverviewPageViewModel OverviewPageViewModel { get; }
		ISearchPageViewModel   SearchPageViewModel   { get; }
		IOptionsPageViewModel  OptionsPageViewModel  { get; }								
	}
}
