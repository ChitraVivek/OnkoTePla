using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.OnkoTePla.Contracts.Config;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.EditDescriptionViewModel
{
	internal class EditDescriptionViewModelSampleData : IEditDescriptionViewModel
	{
		public EditDescriptionViewModelSampleData()
		{
			Description = "sampleText";

			AllAvailablesLabels = new ObservableCollection<Label>
			{
				new Label("label1", Colors.Red,        Guid.NewGuid()),
				new Label("label2", Colors.Chartreuse, Guid.NewGuid()),
				new Label("label3", Colors.DarkBlue,   Guid.NewGuid()),
				new Label("label4", Colors.Gold,       Guid.NewGuid())
			};

			SelectedLabel = AllAvailablesLabels[1];
		}

		public ICommand Cancel => null;
		public ICommand Accept => null;

		public string Description { get; set; }

		public ObservableCollection<Label> AllAvailablesLabels { get; }
		public Label SelectedLabel { get; set; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;			
	}
}