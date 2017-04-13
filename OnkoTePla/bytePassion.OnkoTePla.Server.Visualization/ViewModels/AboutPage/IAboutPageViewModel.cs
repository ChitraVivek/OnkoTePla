using bytePassion.Lib.WpfLib.ViewModelBase;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage
{
	internal interface IAboutPageViewModel : IViewModel
    {
       string VersionNumber { get; } 
    }
}