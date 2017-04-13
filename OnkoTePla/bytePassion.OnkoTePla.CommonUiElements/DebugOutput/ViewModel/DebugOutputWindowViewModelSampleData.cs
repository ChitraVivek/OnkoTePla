using System.ComponentModel;
using System.Windows.Input;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.CommonUiElements.DebugOutput.ViewModel
{
	internal class DebugOutputWindowViewModelSampleData : IDebugOutputWindowViewModel
	{
		public DebugOutputWindowViewModelSampleData()
		{
			AlwaysOnTop = true;
			ScrollDown = false;
		}

		public ICommand DumpOutput  => null;
		public ICommand ClearFilter => null;

		public bool AlwaysOnTop { get; set; }
		public bool ScrollDown { get; set; }
		public string Filter { get; set; }
		public ICollectionView Output { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;				
	}
}