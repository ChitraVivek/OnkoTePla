using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.Lib.Types.SemanticTypes;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class NewSizeAvailable : ViewModelMessage
	{
		public NewSizeAvailable(Size newSize)
		{
			NewSize = newSize;
		}

		public Size NewSize { get; }
	}
}
