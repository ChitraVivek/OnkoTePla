using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage
{
	internal class LabelPageViewModelSampleData : ILabelPageViewModel
	{
		public LabelPageViewModelSampleData()
		{
			AllColors = new ObservableCollection<ColorDisplayData>
			{
				new ColorDisplayData(Colors.Aqua),
				new ColorDisplayData(Colors.Blue),
				new ColorDisplayData(Colors.Fuchsia)
			};

			Labels = new ObservableCollection<Label>
			{
				new Label("label1", Colors.Aquamarine, Guid.NewGuid()),
				new Label("label2", Colors.Fuchsia,    Guid.NewGuid()),
				new Label("label3", Colors.HotPink,    Guid.NewGuid()),
				new Label("label4", Colors.SandyBrown, Guid.NewGuid()),
			};
			
			SelectedLabel = Labels[1];

			Name = SelectedLabel.Name;
			LabelColor = AllColors[2];

			ShowModificationView = true;
		}

		public ICommand AddLabel       => null;
		public ICommand SaveChanges    => null;
		public ICommand DiscardChanges => null;

		public ObservableCollection<Label> Labels { get; }
		public Label SelectedLabel { get; set; }

		public bool ShowModificationView { get; }
		public string Name { get; set; }
		public ColorDisplayData LabelColor { get; set; }

		public ObservableCollection<ColorDisplayData> AllColors { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;
	}
}