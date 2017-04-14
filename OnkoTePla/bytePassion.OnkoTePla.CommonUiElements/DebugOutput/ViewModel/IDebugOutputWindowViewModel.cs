using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;

namespace bytePassion.OnkoTePla.CommonUiElements.DebugOutput.ViewModel
{
	public interface IDebugOutputWindowViewModel : IViewModel
	{
		ICommand DumpOutput  { get; }
		ICommand ClearFilter { get; }
		ICommand ClearOutput { get; }

		bool AlwaysOnTop { get; set; }
		bool ScrollDown { get; set; }
		string Filter { get; set; }

		ICollectionView Output { get; }
	}
}