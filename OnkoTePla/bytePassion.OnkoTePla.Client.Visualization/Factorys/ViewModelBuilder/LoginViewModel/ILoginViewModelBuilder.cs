using bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.LoginViewModel
{
	internal interface ILoginViewModelBuilder
    {
        ILoginViewModel Build();
        void DisposeViewModel(ILoginViewModel viewModelToDispose);
    }
}