using bytePassion.OnkoTePla.Client.DataAndService.Repositories.LocalSettings;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.LoginView;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.ViewModelBuilder.LoginViewModel
{
	internal class LoginViewModelBuilder : ILoginViewModelBuilder
    {
	    private readonly ISession session;
		private readonly ILocalSettingsRepository localSettingsRepository;		

		public LoginViewModelBuilder(ISession session,
									 ILocalSettingsRepository localSettingsRepository)
		{
			this.session = session;
			this.localSettingsRepository = localSettingsRepository;			
		}
		
		public ILoginViewModel Build()
        {
            return new ViewModels.LoginView.LoginViewModel(session, localSettingsRepository);
        }

        public void DisposeViewModel(ILoginViewModel viewModelToDispose)
        {
            viewModelToDispose.Dispose();
        }
    }

}
