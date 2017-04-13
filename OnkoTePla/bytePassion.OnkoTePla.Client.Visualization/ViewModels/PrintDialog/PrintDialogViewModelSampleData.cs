using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintDialog
{
	internal class PrintDialogViewModelSampleData : IPrintDialogViewModel
	{
		public PrintDialogViewModelSampleData()
		{
			AppointmentGrid = new PrintAppointmentGridViewModelSampleData();
		}

		public ICommand Cancel => null;
		public ICommand Print  => null;

		public IPrintAppointmentGridViewModel AppointmentGrid { get; }
		public Size CurrentGridSize { set {} }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}