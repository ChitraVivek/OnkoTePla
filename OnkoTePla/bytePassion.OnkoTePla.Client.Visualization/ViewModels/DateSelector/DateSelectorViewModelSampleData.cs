using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.TimeLib;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector
{
	public class DateSelectorViewModelSampleData : IDateSelectorViewModel
	{
		public DateSelectorViewModelSampleData()
		{
			SelectedDate = TimeTools.Today().DayBefore().DayBefore();
		}

		public Date     SelectedDate { get; set; }
	    public ICommand SelectToday  { get; } = null;	    
		
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
