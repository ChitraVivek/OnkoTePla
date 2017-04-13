using System.Windows;
using bytePassion.Lib.Utils;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AboutDialog;
using bytePassion.OnkoTePla.Client.Visualization.Views;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder
{
	internal class AboutDialogWindowBuilder : IWindowBuilder<AboutDialog>
    {
        
		public AboutDialog BuildWindow()
        {
            var aboutDialogViewModel = new AboutDialogViewModel();

            return new AboutDialog
                   {
                       Owner = Application.Current.MainWindow,
                       DataContext = aboutDialogViewModel
                   };
        }

        public void DisposeWindow(AboutDialog buildedWindow)
        {
			// Nothing to do
		}
    }
}
