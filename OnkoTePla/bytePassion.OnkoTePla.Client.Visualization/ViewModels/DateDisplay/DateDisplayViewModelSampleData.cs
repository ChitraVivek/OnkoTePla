using System.ComponentModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateDisplay
{
	public class DateDisplayViewModelSampleData : IDateDisplayViewModel
    {
	    public DateDisplayViewModelSampleData()
	    {
		    Date = "30.05.2015";
	    }

	    public string Date { get; }
	    
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
