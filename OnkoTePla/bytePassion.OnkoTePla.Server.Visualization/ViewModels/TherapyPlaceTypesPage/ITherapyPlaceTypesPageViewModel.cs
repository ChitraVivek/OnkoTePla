using System.Collections.ObjectModel;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.ViewModelBase;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage.Helper;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.TherapyPlaceTypesPage
{
	public interface ITherapyPlaceTypesPageViewModel : IViewModel
	{
		ICommand AddTherapyPlaceType { get; }
		ICommand SaveChanges         { get; }
		ICommand DiscardChanges      { get; }

		ObservableCollection<TherapyPlaceType> TherapyPlaceTypes { get; }

		TherapyPlaceType SelectedTherapyPlaceType { get; set; }
		
		bool ShowModificationView { get; }

		string          Name     { get; set; }
		IconDisplayData IconType { get; set; }

		ObservableCollection<IconDisplayData> AllIcons { get; }
	}
}