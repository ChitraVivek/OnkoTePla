using System;
using System.Collections.Generic;

namespace bytePassion.OnkoTePla.Utils
{
	public interface IOnkoTePlaDebugListener
	{
		event Action<string> OnNewDebugMessage;

		IList<string> GetInitialList();
	}
}