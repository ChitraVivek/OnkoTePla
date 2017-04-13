using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.EditDescriptionViewModel
{
	internal interface IEditDescriptionViewModel : IViewModel
	{
		ICommand Cancel { get; }
		ICommand Accept { get; }

		string Description { get; set; }

		ObservableCollection<Label> AllAvailablesLabels { get; }
		Label SelectedLabel { get; set; }
	}
}