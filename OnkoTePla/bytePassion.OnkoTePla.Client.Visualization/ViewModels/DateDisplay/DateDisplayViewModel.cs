using System.ComponentModel;
using System.Globalization;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateDisplay
{
	public class DateDisplayViewModel : ViewModel, 
                                        IDateDisplayViewModel
	{
	    private readonly ISharedStateReadOnly<Date> selectedDateVariable; 

		private string date;

		public DateDisplayViewModel(ISharedStateReadOnly<Date> selectedDateVariable)
		{
		    this.selectedDateVariable = selectedDateVariable;		    
			selectedDateVariable.StateChanged += OnSelectedDateVariableChanged;

			OnSelectedDateVariableChanged(selectedDateVariable.Value);
		}

		private void OnSelectedDateVariableChanged(Date newDate)
		{
            // TODO: getCulture from settings
			Date = newDate.GetDisplayString(new CultureInfo("de-DE"));
		}

        public string Date
		{
			get { return date; }
			private set { PropertyChanged.ChangeAndNotify(this, ref date, value); }
		}
		
	    protected override void CleanUp()
	    {
            selectedDateVariable.StateChanged -= OnSelectedDateVariableChanged;
        }

        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
