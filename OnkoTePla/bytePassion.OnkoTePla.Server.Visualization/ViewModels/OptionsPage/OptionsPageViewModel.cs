using System.ComponentModel;
using bytePassion.Lib.WpfLib.ViewModelBase;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.OptionsPage
{
	internal class OptionsPageViewModel : ViewModel, 
                                          IOptionsPageViewModel
    {
        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
