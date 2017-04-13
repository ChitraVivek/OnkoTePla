using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Client.Visualization.Behaviors
{
	internal class AppointmentAdornerPlacementBehavior : Behavior<StackPanel>
    {
        public static readonly DependencyProperty ReferenceElementProperty = 
			DependencyProperty.Register(nameof(ReferenceElement), 
										typeof (UIElement), 
										typeof (AppointmentAdornerPlacementBehavior));        

        public UIElement ReferenceElement
        {
            get { return (UIElement) GetValue(ReferenceElementProperty); }
            set { SetValue(ReferenceElementProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.IsVisibleChanged += OnLayoutUpdate;           
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= OnLayoutUpdate;
        }

        private void OnLayoutUpdate(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
	        AssociatedObject.Margin = ReferenceElement.IsUserVisible() 
											? new Thickness(0, -25, 0,  30)
											: new Thickness(0,  30, 0, -25);
        }
    }
}