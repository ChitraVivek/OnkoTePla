using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
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
			Filter = "blubb";

			var testOutput = new CollectionViewSource
			{
				Source = new List<string>
				{
					"test1",
					"test2",
					"test3",
					"test4"
				}
			};

			Output = testOutput.View;
		}

		public ICommand DumpOutput  => null;
		public ICommand ClearFilter => null;
		public ICommand ClearOutput => null;

		public bool AlwaysOnTop { get; set; }
		public bool ScrollDown  { get; set; }
		public string Filter    { get; set; }

		public ICollectionView Output { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;				
	}
}