using System;
using System.Linq;
using System.Windows;
using bytePassion.OnkoTePla.Client.DataAndService.SessionInfo;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Client.Visualization;

namespace bytePassion.OnkoTePla.Client.Application
{
	internal class FatalErrorHandler
	{
		private readonly ISession session;

		public FatalErrorHandler(ISession session)
		{
			this.session = session;
		}

		public void HandleFatalError(string errorMsg)
		{
			MessageBox.Show($"A fatal and very unexpected error Occured:\n{errorMsg}");

			switch (session.CurrentApplicationState)
			{
				case ApplicationState.LoggedIn:
				{
					session.Logout(
						() =>
						{
							session.TryDisconnect(() => { },
								_ => KillWindow()
							);
						},
						_ => KillWindow()
					);
					break;
				}

				case ApplicationState.ConnectedButNotLoggedIn:
				{
					session.TryDisconnect(
						() => {	},
						_ => KillWindow()
					);
					break;
				}

				default:
				{					
					KillWindow();
					break;
				}
			}
		}

		private static void KillWindow ()
		{
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				var windows = System.Windows.Application.Current.Windows
											 .OfType<MainWindow>()
											 .ToList();

				if (windows.Count == 1)
					windows[0].Close();
				else
					throw new Exception("inner error");
			});
		}
	}
}
