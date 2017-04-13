using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.WpfLib.Commands;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Resources;
using bytePassion.OnkoTePla.Server.DataAndService.Connection;
using bytePassion.OnkoTePla.Server.DataAndService.Data;
using bytePassion.OnkoTePla.Server.Visualization.Enums;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage
{
	public class LabelPageViewModel : ViewModel, ILabelPageViewModel
	{
		private readonly IDataCenter dataCenter;
		private readonly ISharedStateReadOnly<MainPage> selectedPageVariable;
		private readonly IConnectionService connectionService;
		private bool showModificationView;
		private Label selectedLabel;
		private string name;
		private ColorDisplayData labelColor;

		public LabelPageViewModel(IDataCenter dataCenter,
								  ISharedStateReadOnly<MainPage> selectedPageVariable,
								  IConnectionService connectionService)
		{
			this.dataCenter = dataCenter;
			this.selectedPageVariable = selectedPageVariable;
			this.connectionService = connectionService;

			AddLabel       = new Command(DoAddLabel);
			SaveChanges    = new Command(DoSaveChanges);
			DiscardChanges = new Command(DoDiscardChanges);

			Labels = new ObservableCollection<Label>();
			ShowModificationView = false;

			selectedPageVariable.StateChanged += OnSelectedPageChanged;

			AllColors = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)
									  .Select(p => (Color)p.GetValue(null, null))
									  .Select(color => new ColorDisplayData(color))
									  .ToObservableCollection();
		}

		private void OnSelectedPageChanged(MainPage mainPage)
		{
			if (mainPage == MainPage.Labels)
			{
				Labels.Clear();

				dataCenter.GetAllLabels().Do(Labels.Add);
				SelectedLabel = null;
			}
		}

		private void DoDiscardChanges()
		{
			SelectedLabel = null;
			ShowModificationView = false;
		}

		private void DoSaveChanges()
		{
			if (!NameChecker.CheckName(Name))
			{
				NameChecker.ShowCharacterError(Name);
				return;
			}

			var newLabel = SelectedLabel.SetNewName(Name)
								        .SetNewColor(LabelColor.Color);

			dataCenter.UpdateLabel(newLabel);
			connectionService.SendLabelUpdatedNotification(newLabel);

			Labels.Remove(SelectedLabel);
			Labels.Add(newLabel);

			SelectedLabel = null;
			ShowModificationView = false;
		}

		private void DoAddLabel()
		{
			var newLabel = LabelCreateAndUpdateLogic.Create();
			dataCenter.AddNewLabel(newLabel);
			connectionService.SendLabelAddedNotification(newLabel);
			Labels.Add(newLabel);

			SelectedLabel = newLabel;
		}

		public ICommand AddLabel       { get; }
		public ICommand SaveChanges    { get; }
		public ICommand DiscardChanges { get; }

		public ObservableCollection<Label> Labels { get; }

		public Label SelectedLabel
		{
			get { return selectedLabel; }
			set
			{
				var olsSelectedLabel = selectedLabel;

				PropertyChanged.ChangeAndNotify(this, ref selectedLabel, value);

				ShowModificationView = selectedLabel != null;

				if (selectedLabel != null && selectedLabel != olsSelectedLabel)
				{
					Name     = selectedLabel.Name;
					LabelColor = AllColors.First(colorDD => colorDD.Color == selectedLabel.Color);
				}
			}
		}

		public bool ShowModificationView
		{
			get { return showModificationView; }
			private set { PropertyChanged.ChangeAndNotify(this, ref showModificationView, value); }
		}

		public string Name
		{
			get { return name; }
			set { PropertyChanged.ChangeAndNotify(this, ref name, value); }
		}

		public ColorDisplayData LabelColor
		{
			get { return labelColor; }
			set { PropertyChanged.ChangeAndNotify(this, ref labelColor, value); }
		}

		public ObservableCollection<ColorDisplayData> AllColors { get; }

		protected override void CleanUp()
		{
			selectedPageVariable.StateChanged -= OnSelectedPageChanged;
		}
		public override event PropertyChangedEventHandler PropertyChanged;
	}
}
