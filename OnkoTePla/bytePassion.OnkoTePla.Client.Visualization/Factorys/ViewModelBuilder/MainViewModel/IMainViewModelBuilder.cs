using System;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.MainViewModel
{
	internal interface IMainViewModelBuilder
	{
		IMainViewModel Build(Action<string> errorCallback, Size initialSize = null);
		void DisposeViewModel(IMainViewModel viewModelToDispose);

		Size GetCurrentGridSize();
	}
}