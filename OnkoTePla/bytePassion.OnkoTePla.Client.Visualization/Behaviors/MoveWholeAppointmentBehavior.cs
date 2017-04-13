using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using bytePassion.Lib.Communication.ViewModel;
using bytePassion.OnkoTePla.Client.Visualization.Adorner;
using bytePassion.OnkoTePla.Client.Visualization.Global;
using bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.AppointmentView.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.Behaviors
{
	internal class MoveWholeAppointmentBehavior : Behavior<FrameworkElement>
	{
        public static readonly DependencyProperty AppointmentModificationsProperty =
            DependencyProperty.Register(nameof(AppointmentModifications),
                                        typeof(AppointmentModifications),
                                        typeof(MoveWholeAppointmentBehavior));

	    public static readonly DependencyProperty AdornerControlProperty = 
            DependencyProperty.Register(nameof(AdornerControl), 
                                        typeof (AdornerControl), 
                                        typeof (MoveWholeAppointmentBehavior));

        public static readonly DependencyProperty ViewModelCommunicationProperty = 
            DependencyProperty.Register(nameof(ViewModelCommunication), 
                                        typeof (IViewModelCommunication), 
                                        typeof (MoveWholeAppointmentBehavior));

        public IViewModelCommunication ViewModelCommunication
        {
            get { return (IViewModelCommunication) GetValue(ViewModelCommunicationProperty); }
            set { SetValue(ViewModelCommunicationProperty, value); }
        }

	    public AdornerControl AdornerControl
	    {
	        get { return (AdornerControl) GetValue(AdornerControlProperty); }
	        set { SetValue(AdornerControlProperty, value); }
	    }

        public AppointmentModifications AppointmentModifications
        {
            get { return (AppointmentModifications)GetValue(AppointmentModificationsProperty); }
            set { SetValue(AppointmentModificationsProperty, value); }
        }

        protected override void OnAttached ()
		{
			AssociatedObject.PreviewMouseLeftButtonDown += OnAssociatedObjectMouseLeftButtonDown;
			AssociatedObject.MouseLeftButtonUp          += OnAssociatedObjectMouseLeftButtonUp;
			AssociatedObject.MouseMove                  += OnAssociatedObjectMouseMove;
			AssociatedObject.MouseLeave                 += AssociatedObjectOnMouseLeave;
			AssociatedObject.PreviewQueryContinueDrag   += OnQueryContinueDrag;
			AssociatedObject.GiveFeedback               += OnGiveFeedback;

			container = Application.Current.MainWindow;
			mouseIsDown = false;
		}

		protected override void OnDetaching ()
		{
			AssociatedObject.PreviewMouseLeftButtonDown -= OnAssociatedObjectMouseLeftButtonDown;
			AssociatedObject.MouseLeftButtonUp          -= OnAssociatedObjectMouseLeftButtonUp;
			AssociatedObject.MouseMove                  -= OnAssociatedObjectMouseMove;
			AssociatedObject.MouseLeave                 -= AssociatedObjectOnMouseLeave;
			AssociatedObject.PreviewQueryContinueDrag   -= OnQueryContinueDrag;
			AssociatedObject.GiveFeedback               -= OnGiveFeedback;
		}

		private void AssociatedObjectOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
		{
			if (mouseIsDown)
			{
				EndDrag();				

				ViewModelCommunication.SendTo(
					Constants.ViewModelCollections.AppointmentViewModelCollection,
                    AppointmentModifications.OriginalAppointment.Id, 
					new ShowDisabledOverlay()
				);				

                AdornerControl.CreateAdorner(AppointmentModifications.OriginalAppointment.Patient.Name,
                                             AppointmentModifications.CurrentAppointmentPixelWidth);
				

				DragDrop.DoDragDrop((DependencyObject)sender,
                                    AppointmentModifications.OriginalAppointment,
				                    DragDropEffects.Link);

                AdornerControl.DisposeAdorner();

				ViewModelCommunication.SendTo(
					Constants.ViewModelCollections.AppointmentViewModelCollection,
                    AppointmentModifications.OriginalAppointment.Id,
					new HideDisabledOverlay()
				);
			}
		}

		private static void OnQueryContinueDrag (object sender, QueryContinueDragEventArgs e)
		{
			if (e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}			
		}		

		private static void OnGiveFeedback(object sender, GiveFeedbackEventArgs giveFeedbackEventArgs)
		{
			Mouse.SetCursor(Cursors.None);
			giveFeedbackEventArgs.UseDefaultCursors = false;
			giveFeedbackEventArgs.Handled = true;
		}

		private FrameworkElement container;

		private bool  mouseIsDown;
		private Point referencePoint;		

	    private void OnAssociatedObjectMouseMove (object sender, MouseEventArgs mouseEventArgs)
		{
			if (mouseIsDown)
			{
			    var mousePos = mouseEventArgs.GetPosition(container);
                var displacement =  mousePos- referencePoint;
                AppointmentModifications.SetNewTimeShiftDelta(displacement.X);
			}
		}

		private void OnAssociatedObjectMouseLeftButtonUp (object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			if (mouseIsDown)
				EndDrag();
		}

		private void OnAssociatedObjectMouseLeftButtonDown (object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			InitDrag(mouseButtonEventArgs.GetPosition(container));

			mouseButtonEventArgs.Handled = true;
		}

		private void InitDrag (Point startinPoint)
		{						
			if (AppointmentModifications != null)
			{
				mouseIsDown = true;
				referencePoint = startinPoint;				
			}
		}

		private void EndDrag ()
		{
            AppointmentModifications.FixTimeShiftDelta();
			mouseIsDown = false;
		}
	}
}
