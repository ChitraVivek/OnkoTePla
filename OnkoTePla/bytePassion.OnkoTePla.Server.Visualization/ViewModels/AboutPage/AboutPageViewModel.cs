using System.ComponentModel;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Resources;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage
{
	internal class AboutPageViewModel : ViewModel, IAboutPageViewModel
    {
		public AboutPageViewModel()
		{
			VersionNumber = ApplicationInfo.ServerVersion;
		}

		public string VersionNumber { get; }

		protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;		
    }
}
