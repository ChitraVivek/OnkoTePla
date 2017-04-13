using System;
using System.Windows;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.MedicalPracticeRepository;
using bytePassion.OnkoTePla.Client.DataAndService.Repositories.ReadModelRepository;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintDialog;
using bytePassion.OnkoTePla.Client.Visualization.Views;
using bytePassion.OnkoTePla.Contracts.Domain;

namespace bytePassion.OnkoTePla.Client.Visualization.Factorys.WindowBuilder
{
	internal class PrintDialogWindowBuilder
	{
		private readonly IClientMedicalPracticeRepository medicalPracticeRepository;
		private readonly IClientReadModelRepository readModelRepository;
		private readonly Action<string> errorCallback;

		public PrintDialogWindowBuilder(IClientMedicalPracticeRepository medicalPracticeRepository, 
										IClientReadModelRepository readModelRepository, 
										Action<string> errorCallback)
		{
			this.medicalPracticeRepository = medicalPracticeRepository;
			this.readModelRepository = readModelRepository;
			this.errorCallback = errorCallback;
		}

		public PrintDialog BuildWindow(AggregateIdentifier identifier)
		{           
			return new PrintDialog
			       {
						Owner = Application.Current.MainWindow,
						DataContext = new PrintDialogViewModel(identifier, medicalPracticeRepository, readModelRepository, errorCallback)
			       };
		}

		public void DisposeWindow(PrintDialog buildedWindow)
		{
			throw new NotImplementedException();
		}		
	}
}
