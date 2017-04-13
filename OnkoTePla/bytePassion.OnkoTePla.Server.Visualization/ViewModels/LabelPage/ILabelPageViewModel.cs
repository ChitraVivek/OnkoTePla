using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.LabelPage
{
	internal interface ILabelPageViewModel : IViewModel
	{
		ICommand AddLabel       { get; }
		ICommand SaveChanges    { get; }
		ICommand DiscardChanges { get; }

		ObservableCollection<Label> Labels { get; }
		
		Label SelectedLabel { get; set; }

		bool ShowModificationView { get; }

		string           Name       { get; set; }
		ColorDisplayData LabelColor { get; set; }
		 
		ObservableCollection<ColorDisplayData> AllColors { get; }
	}
}