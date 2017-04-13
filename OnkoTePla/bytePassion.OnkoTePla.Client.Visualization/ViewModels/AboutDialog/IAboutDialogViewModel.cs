using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AboutDialog
{
	internal interface IAboutDialogViewModel : IViewModel
    {
        ICommand CloseDialog { get; } 
        string VersionNumber { get; }
    }
}