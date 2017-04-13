using bytePassion.Lib.Communication.ViewModel.Messages;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class SetVisibility : ViewModelMessage
	{
		public SetVisibility(bool visible)
		{
			Visible = visible;
		}

		public bool Visible { get; }
	}
}
