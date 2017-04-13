using System.ComponentModel;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.SearchPage;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MainView
{
	internal class MainViewModel : ViewModel, 
                                   IMainViewModel
    {
	    private readonly IViewModelCommunication viewModelCommunication;
	    private readonly ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable;
	    private int selectedPage;        

        public MainViewModel(IViewModelCommunication viewModelCommunication,
							 IOverviewPageViewModel overviewPageViewModel,
						     ISearchPageViewModel searchPageViewModel,
						     IOptionsPageViewModel optionsPageViewModel,
							 ISharedStateReadOnly<AppointmentModifications> appointmentModificationsVariable)
        {
	        this.viewModelCommunication = viewModelCommunication;
	        this.appointmentModificationsVariable = appointmentModificationsVariable;
	        OverviewPageViewModel = overviewPageViewModel;
            SearchPageViewModel = searchPageViewModel;
            OptionsPageViewModel = optionsPageViewModel;

            SelectedPage = 0;
        }

        public int SelectedPage
        {
            get { return selectedPage; }
            private set { PropertyChanged.ChangeAndNotify(this, ref selectedPage, value); }
        }
				
        public IOverviewPageViewModel OverviewPageViewModel { get; }
        public ISearchPageViewModel   SearchPageViewModel   { get; }
        public IOptionsPageViewModel  OptionsPageViewModel  { get; }

		public void Process (ShowPage message)
		{
			if (appointmentModificationsVariable.Value == null)
			{
				SelectedPage = (int) message.Page;
			}
			else
			{
				viewModelCommunication.Send(new ShowNotification("Seite kann nicht gewechselt werden, da Termin noch in Bearbeitung", 10));
			}
		}		
	            
	    protected override void CleanUp() {	}
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}