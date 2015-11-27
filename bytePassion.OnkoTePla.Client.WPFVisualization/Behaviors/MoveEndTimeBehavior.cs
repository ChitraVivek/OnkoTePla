﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.AppointmentView.Helper;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.Behaviors
{
    public class MoveEndTimeBehavior : Behavior<FrameworkElement>
	{

        public static readonly DependencyProperty AppointmentModificationsProperty =
            DependencyProperty.Register(nameof(AppointmentModifications),
                                        typeof(AppointmentModifications),
                                        typeof(MoveEndTimeBehavior));

        public AppointmentModifications AppointmentModifications
        {
            get { return (AppointmentModifications)GetValue(AppointmentModificationsProperty); }
            set { SetValue(AppointmentModificationsProperty, value); }
        }

        protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.PreviewMouseLeftButtonDown += OnAssociatedObjectMouseLeftButtonDown;
			AssociatedObject.MouseLeftButtonUp          += OnAssociatedObjectMouseLeftButtonUp;
			AssociatedObject.MouseMove                  += OnAssociatedObjectMouseMove;

			container = Application.Current.MainWindow;
			mouseIsDown = false;
		}

		protected override void OnDetaching ()
		{
			base.OnDetaching();
			AssociatedObject.PreviewMouseLeftButtonDown -= OnAssociatedObjectMouseLeftButtonDown;
			AssociatedObject.MouseLeftButtonUp          -= OnAssociatedObjectMouseLeftButtonUp;
			AssociatedObject.MouseMove                  -= OnAssociatedObjectMouseMove;
		}

		private FrameworkElement container;
		
		private bool  mouseIsDown;		
		private Point referencePoint;		

		private void OnAssociatedObjectMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
			if (mouseIsDown)
			{
				var displacement = mouseEventArgs.GetPosition(container) - referencePoint;

                AppointmentModifications.SetNewEndTimeDelta(displacement.X);				
			}
		}

		private void OnAssociatedObjectMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			EndDrag();
		}

		private void OnAssociatedObjectMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{			
			InitDrag(mouseButtonEventArgs.GetPosition(container));

			mouseButtonEventArgs.Handled = true;
		}

		private void InitDrag (Point startinPoint)
		{
			mouseIsDown = true;
			referencePoint = startinPoint;			
			AssociatedObject.CaptureMouse();
		}

		private void EndDrag ()
		{
            AppointmentModifications.FixEndTimeDelta();
			mouseIsDown = false;
			Mouse.Capture(null);
		}
	}
}
