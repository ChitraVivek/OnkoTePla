using bytePassion.Lib.WpfLib.ViewModelBase;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage
{
	public interface IAboutPageViewModel : IViewModel
    {
       string VersionNumber { get; } 
    }
}