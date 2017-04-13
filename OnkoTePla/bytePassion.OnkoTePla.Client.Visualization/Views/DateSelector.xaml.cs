using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace bytePassion.OnkoTePla.Client.Visualization.Views
{
	public partial class DateSelector
	{
		public DateSelector ()
		{
			InitializeComponent();
		}

        private void UIElement_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }
	}
}
