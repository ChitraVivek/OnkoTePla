using System.Windows;
using System.Windows.Documents;
using bytePassion.Lib.WpfLib.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentViewDragAdorner;
using bytePassion.OnkoTePla.Client.Visualization.Views;

namespace bytePassion.OnkoTePla.Client.Visualization.Adorner
{
	internal class AdornerControl 
	{				
		private UIElementAdorner appointmentAdorner;
		private IAppointmentViewDragAdornerViewModel adornerViewModel;

		private double currentAdornerWidth;

		internal void CreateAdorner(string content, double width)
		{
			if (appointmentAdorner == null && ReferenceElement != null)
			{
				currentAdornerWidth = width;
				adornerViewModel = new AppointmentViewDragAdornerViewModel(content);

				var adornerView = new AppointmentViewDragAdorner
				                  {
					                  DataContext = adornerViewModel,
					                  Height = 30,
					                  Width = width
				                  };

				appointmentAdorner = new UIElementAdorner(ReferenceElement,
				                                          adornerView,
				                                          AdornerLayer.GetAdornerLayer(ReferenceElement));
			}
		}

		internal UIElement ReferenceElement { get; set; }

		internal void NewMousePositionForAdorner (Point newMousePosition)
		{
			appointmentAdorner.UpdatePosition(newMousePosition.X-(currentAdornerWidth / 2), newMousePosition.Y - 15);
		}

		internal void ShowAdornerLikeDropIsPossible ()
		{
			adornerViewModel.DropPossible = true;
		}

		internal void ShowAdornerLikeDropIsNotPossible ()
		{
			adornerViewModel.DropPossible = false;
		}

		internal void DisposeAdorner ()
		{
			appointmentAdorner?.Destroy();
			appointmentAdorner = null;
		}         		
	}
}
