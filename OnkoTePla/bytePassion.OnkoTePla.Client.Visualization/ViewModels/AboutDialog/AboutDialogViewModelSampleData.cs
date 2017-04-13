using System.ComponentModel;
using System.Windows.Input;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AboutDialog
{
	internal class AboutDialogViewModelSampleData : IAboutDialogViewModel
    {
        public AboutDialogViewModelSampleData()
        {
            VersionNumber = "0.1.0.345";
        }

        public ICommand CloseDialog => null;
        public string VersionNumber { get; }

        public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}