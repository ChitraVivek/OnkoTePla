using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.OnkoTePla.Utils;
using Microsoft.Win32;

namespace bytePassion.OnkoTePla.CommonUiElements.DebugOutput.ViewModel
{
	public class DebugOutputWindowViewModel : Lib.WpfLib.ViewModelBase.ViewModel, IDebugOutputWindowViewModel
	{
		private readonly IOnkoTePlaDebugListener listener;
		private readonly ObservableCollection<string> debugMessages;
		private readonly CollectionViewSource dmSource;
		private string filter;
		private bool alwaysOnTop;
		private bool scrollDown;

		public DebugOutputWindowViewModel(IOnkoTePlaDebugListener listener)
		{
			this.listener = listener;

			listener.OnNewDebugMessage += OnNewDebugMessage;

			debugMessages = new ObservableCollection<string>();

			dmSource = new CollectionViewSource
			{
				Source = debugMessages
			};
			Output = dmSource.View;
			dmSource.Filter += OnFilter;
			Filter = "";

			ClearFilter = new Command(DoClearFilter);
			DumpOutput  = new Command(DoDumpOutput);
			ClearOutput = new Command(DoClearOutput);

			AlwaysOnTop = false;
			ScrollDown = true;
		}

		private void DoClearOutput()
		{
			debugMessages.Clear();
			dmSource.View.Refresh();
		}

		private void DoDumpOutput()
		{
			var dialog = new SaveFileDialog()
			{
				Filter = "textFiles |*.txt",
				AddExtension = true,
				CheckFileExists = false,
				OverwritePrompt = true,
				ValidateNames = true,
				CheckPathExists = true,
				CreatePrompt = false,
				Title = "Debug speichern"
			};

			var result = dialog.ShowDialog();

			if (result.HasValue)
			{
				if (result.Value)
				{
					SaveDebugToFile(dialog.FileName);
				}
			}
		}

		private void SaveDebugToFile(string filename)
		{
			var sb = new StringBuilder();

			for (int index = 0; index < debugMessages.Count; index++)
			{
				var debugMessage = debugMessages[index];
				sb.Append(debugMessage);

				if (index != debugMessages.Count - 1)
					sb.Append(Environment.NewLine);
			}

			File.WriteAllText(filename, sb.ToString());
		}

		private void DoClearFilter()
		{
			Filter = string.Empty;
		}

		private void OnFilter (object sender, FilterEventArgs e)
		{
			var stringToCheck = e.Item as string;
			var finalFilter = Filter.Trim();
			e.Accepted = string.IsNullOrWhiteSpace(finalFilter) || stringToCheck.Contains(finalFilter);			
		}

		private void OnNewDebugMessage(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					debugMessages.Add(s);
					dmSource.View.Refresh();
				});
			}					
		}

		public ICommand DumpOutput  { get; }
		public ICommand ClearFilter { get; }
		public ICommand ClearOutput { get; }

		public bool AlwaysOnTop
		{
			get { return alwaysOnTop; }
			set { PropertyChanged.ChangeAndNotify(this, ref alwaysOnTop, value); }
		}

		public bool ScrollDown
		{
			get { return scrollDown; }
			set { PropertyChanged.ChangeAndNotify(this, ref scrollDown, value); }
		}

		public string Filter
		{
			get { return filter; }
			set
			{
				PropertyChanged.ChangeAndNotify(this, ref filter, value);
				dmSource.View.Refresh();
			}
		}

		public ICollectionView Output { get; }

		protected override void CleanUp()
		{
			listener.OnNewDebugMessage -= OnNewDebugMessage;
			dmSource.Filter -= OnFilter;
		}

		public override event PropertyChangedEventHandler PropertyChanged;
		
	}
}
