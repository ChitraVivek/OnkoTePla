﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using bytePassion.OnkoTePla.Utils;

namespace bytePassion.OnkoTePla.CommonUiElements.DebugOutput.ViewModel
{
	public class DebugOutputWindowViewModel : Lib.WpfLib.ViewModelBase.ViewModel, IDebugOutputWindowViewModel
	{
		private readonly IOnkoTePlaDebugListener listener;
		private readonly ObservableCollection<string> debugMessages;
		private readonly CollectionViewSource dmSource;

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

//			DumpOutput =
		}

		private void OnFilter (object sender, FilterEventArgs e)
		{
			var stringToCheck = e.Item as string;

			
			e.Accepted = string.IsNullOrWhiteSpace(Filter) || stringToCheck.Contains(Filter);			
		}

		private void OnNewDebugMessage(string s)
		{
			debugMessages.Add(s);
		}

		public ICommand DumpOutput { get; }
		public ICommand ClearFilter { get; }
		public bool AlwaysOnTop { get; set; }
		public bool ScrollDown { get; set; }
		public string Filter { get; set; }
		public ICollectionView Output { get; }

		protected override void CleanUp()
		{
			listener.OnNewDebugMessage -= OnNewDebugMessage;
			dmSource.Filter -= OnFilter;
		}

		public override event PropertyChangedEventHandler PropertyChanged;
		
	}
}
