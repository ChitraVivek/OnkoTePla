using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.BackupPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.ConnectionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.HoursOfOpeningPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.LicencePage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OptionsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.OverviewPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.PatientsPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.MainWindow
{
	internal interface IMainWindowViewModel : IViewModel
    {
        ICommand SwitchToPage { get; }
        MainPage SelectedPage { get; }

		bool CheckWindowClosing { get; }
		ICommand CloseWindow { get; }

		IOverviewPageViewModel          OverviewPageViewModel          { get; }
        IConnectionsPageViewModel       ConnectionsPageViewModel       { get; }
        IUserPageViewModel              UserPageViewModel              { get; }
        ILicencePageViewModel           LicencePageViewModel           { get; }
        IInfrastructurePageViewModel    InfrastructurePageViewModel    { get; }
		IHoursOfOpeningPageViewModel    HoursOfOpeningPageViewModel    { get; }
		ITherapyPlaceTypesPageViewModel TherapyPlaceTypesPageViewModel { get; }
		ILabelPageViewModel				LabelPageViewModel			   { get; }
		IPatientsPageViewModel			PatientsPageViewModel		   { get; }
		IBackupPageViewModel			BackupPageViewModel			   { get; }
		IOptionsPageViewModel           OptionsPageViewModel           { get; }
        IAboutPageViewModel             AboutPageViewModel             { get; }
    }
}