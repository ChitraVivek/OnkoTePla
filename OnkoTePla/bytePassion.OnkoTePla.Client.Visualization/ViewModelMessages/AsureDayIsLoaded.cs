using System;
using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.Lib.TimeLib;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class AsureDayIsLoaded : ViewModelMessage
	{
	    public AsureDayIsLoaded(Guid medicalPracticeId, Date day, Action dayIsLoaded)
	    {
		    MedicalPracticeId = medicalPracticeId;
		    Day = day;
		    DayIsLoaded = dayIsLoaded;
	    }

	    public Guid   MedicalPracticeId { get; }
		public Date   Day               { get; }
		public Action DayIsLoaded       { get; }
	}
}
