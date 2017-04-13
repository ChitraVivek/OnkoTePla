using System.ComponentModel;
using System.Windows;
using bytePassion.OnkoTePla.Resources.UserNotificationService;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.MainWindow;
using MahApps.Metro.Controls.Dialogs;

namespace bytePassion.OnkoTePla.Server.Visualization
{
	public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		private async void MainWindow_OnClosing (object sender, CancelEventArgs e)
		{
			var viewModel = DataContext as IMainWindowViewModel;

			if (viewModel.CheckWindowClosing)
			{
				e.Cancel = true;
			}

			if (viewModel.CheckWindowClosing)
			{
				var dialog = new UserDialogBox("Programm beenden.", "Wollen Sie das Programm wirklich beenden?",
				MessageBoxButton.OKCancel);

				var result = await dialog.ShowMahAppsDialog();

				if (result == MessageDialogResult.Affirmative)
				{
					viewModel.CloseWindow.Execute(null);
				}
			}
		}
	}
}
