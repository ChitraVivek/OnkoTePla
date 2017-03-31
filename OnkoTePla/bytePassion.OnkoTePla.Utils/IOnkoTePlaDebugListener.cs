using System;

namespace bytePassion.OnkoTePla.Utils
{
	public interface IOnkoTePlaDebugListener
	{
		event Action<string> OnNewDebugMessage;
	}
}