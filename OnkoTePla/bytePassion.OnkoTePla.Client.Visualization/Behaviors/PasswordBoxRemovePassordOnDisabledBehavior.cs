using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace bytePassion.OnkoTePla.Client.Visualization.Behaviors
{
	internal class PasswordBoxRemovePassordOnDisabledBehavior : Behavior<PasswordBox>
	{
		protected override void OnAttached()
		{
			AssociatedObject.IsEnabledChanged += OnIsEnabledChanged;
		}
				
		protected override void OnDetaching()
		{
			AssociatedObject.IsEnabledChanged -= OnIsEnabledChanged;
		}

		private void OnIsEnabledChanged (object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var passordBox = (PasswordBox) sender;

			if (!passordBox.IsEnabled)
				passordBox.Password = "";
		}
	}
}
