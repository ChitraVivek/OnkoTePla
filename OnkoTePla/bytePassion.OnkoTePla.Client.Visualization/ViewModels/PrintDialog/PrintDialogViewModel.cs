using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid;
using bytePassion.OnkoTePla.Contracts.Domain;
using Size = bytePassion.Lib.Types.SemanticTypes.Size;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintDialog
{
	internal class PrintDialogViewModel : ViewModel, IPrintDialogViewModel
	{
		private IPrintAppointmentGridViewModel appointmentGrid;
		private Size currentGridSize;

		public PrintDialogViewModel(AggregateIdentifier identifier,
									IClientMedicalPracticeRepository medicalPracticeRepository,
									IClientReadModelRepository readModelRepository,
									Action<string> errorCallback)
		{
			Cancel = new Command(CloseWindow);
			Print = new ParameterrizedCommand<FrameworkElement>(DoPrint);

			medicalPracticeRepository.RequestMedicalPractice(
				practice =>
				{					
					readModelRepository.RequestAppointmentSetOfADay(
						appointmentSet =>
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								var initialGridSize = CurrentGridSize ?? new Size(new Width(850), new Height(600));
								
								AppointmentGrid = new PrintAppointmentGridViewModel(identifier, practice, appointmentSet.Appointments, initialGridSize);
							});
						},
						identifier,
						errorCallback							
					);
				},
				identifier.MedicalPracticeId,
				identifier.Date,
				errorCallback	
			);
		}

		public ICommand Cancel { get; }
		public ICommand Print  { get; }

		public IPrintAppointmentGridViewModel AppointmentGrid
		{
			get { return appointmentGrid; }
			private set { PropertyChanged.ChangeAndNotify(this, ref appointmentGrid, value); }
		}

		public Size CurrentGridSize
		{
			private get { return currentGridSize; }
			set
			{
				currentGridSize = value;

				if (AppointmentGrid != null && value != null)
				{
					AppointmentGrid.TimeGridViewModel.Process(new NewSizeAvailable(CurrentGridSize));
					foreach (var printTherapyPlaceRowViewModel in AppointmentGrid.TherapyPlaceRowViewModels)
					{
						printTherapyPlaceRowViewModel.GridWidth = CurrentGridSize.Width;
					}
				}					
			}
		}

		private static void DoPrint(FrameworkElement grid)
		{
			var pd = new System.Windows.Controls.PrintDialog();
			if (pd.ShowDialog() == true)
			{
				pd.PrintVisual(grid, "appointments to print");
			}
		}

		private static void CloseWindow ()
		{
			var windows = Application.Current.Windows
											 .OfType<Views.PrintDialog>()
											 .ToList();

			if (windows.Count == 1)
				windows[0].Close();
			else
				throw new Exception("inner error");
		}

		protected override void CleanUp() {	}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
