namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentViewDragAdorner
{
	public interface IAppointmentViewDragAdornerViewModel : IViewModel
	{
		bool DropPossible { get; set; }		
		string Content { get; }
	}
}