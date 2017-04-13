using System.Windows.Input;
using bytePassion.Lib.Types.SemanticTypes;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintAppointmentGrid;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.PrintDialog
{
	internal interface IPrintDialogViewModel : IViewModel
	{
		ICommand Cancel { get; }
		ICommand Print  { get; }

		IPrintAppointmentGridViewModel AppointmentGrid { get; }

		Size CurrentGridSize { set; }
	}
}