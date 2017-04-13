using System.ComponentModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.AboutPage
{
	internal class AboutPageViewModelSampleData : IAboutPageViewModel
    {
	    public AboutPageViewModelSampleData()
	    {
		    VersionNumber = "0.1.0.0";
	    }

		public string VersionNumber { get; }

		public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;	    
    }
}