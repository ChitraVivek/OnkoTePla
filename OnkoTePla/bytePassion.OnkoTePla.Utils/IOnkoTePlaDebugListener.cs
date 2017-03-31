using System;

namespace bytePassion.OnkoTePla.Utils
{
	/// <summary>
	///  !!!!!!!!!!!!! 1000 !!!!!!!!!!!!!
	/// </summary>
	public interface IOnkoTePlaDebugListener
	{
		event Action<string> OnNewDebugMessage;
	}
}