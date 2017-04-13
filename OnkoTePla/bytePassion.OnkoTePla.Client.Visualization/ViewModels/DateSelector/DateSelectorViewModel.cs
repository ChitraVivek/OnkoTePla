using System.ComponentModel;
using System.Windows.Input;
using bytePassion.Lib.Communication.State;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.WpfLib.Commands;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector
{
	public class DateSelectorViewModel : ViewModel, 
                                         IDateSelectorViewModel
	{		
		private readonly ISharedState<Date> selectedDateVariable;

		private Date selectedDate;

		public DateSelectorViewModel(ISharedState<Date> selectedDateVariable)
		{
		    this.selectedDateVariable = selectedDateVariable;

            SelectToday = new Command(() => SelectedDate = TimeTools.Today());

            selectedDateVariable.StateChanged += OnSelectedDateChanged;

			SelectedDate = selectedDateVariable.Value;			
		}

		private void OnSelectedDateChanged(Date date)
		{
			selectedDate = date;
			PropertyChanged.Notify(this, nameof(SelectedDate));
		}

		public Date SelectedDate
		{
			get { return selectedDate; }
			set
			{
				if (value != selectedDate)
				{					
					selectedDateVariable.Value = value;
					PropertyChanged.ChangeAndNotify(this, ref selectedDate, value);					
				}
			}
		}

		public ICommand SelectToday { get; }

        protected override void CleanUp()
        {
            selectedDateVariable.StateChanged -= OnSelectedDateChanged;
        }        
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
