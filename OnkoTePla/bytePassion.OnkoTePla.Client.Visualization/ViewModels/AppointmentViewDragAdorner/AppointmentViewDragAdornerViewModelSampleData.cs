using System.ComponentModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentViewDragAdorner
{

	public class AppointmentViewDragAdornerViewModelSampleData : IAppointmentViewDragAdornerViewModel
	{
		public AppointmentViewDragAdornerViewModelSampleData()
		{
			DropPossible = true;
			Content = "John Doe";
		}

		public bool   DropPossible { get; set; }
		public string Content      { get; }
		
	    public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}