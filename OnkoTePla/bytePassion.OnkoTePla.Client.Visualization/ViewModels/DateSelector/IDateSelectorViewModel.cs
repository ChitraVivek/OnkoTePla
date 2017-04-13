using System.Windows.Input;
using bytePassion.Lib.TimeLib;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector
{
	public interface IDateSelectorViewModel : IViewModel
	{
		Date SelectedDate { get; set; }

		ICommand SelectToday { get; }
	}
}
