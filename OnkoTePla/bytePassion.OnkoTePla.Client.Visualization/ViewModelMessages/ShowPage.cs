using bytePassion.Lib.Communication.ViewModel.Messages;
using bytePassion.OnkoTePla.Client.Visualization.Enums;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModelMessages
{
	internal class ShowPage : ViewModelMessage
	{		
		public ShowPage(MainPage page)
		{
			Page = page;
		}

		public MainPage Page { get; }
	}
}
