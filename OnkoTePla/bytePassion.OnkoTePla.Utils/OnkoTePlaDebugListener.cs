using System;
using System.Diagnostics;

namespace bytePassion.OnkoTePla.Utils
{

	public class OnkoTePlaDebugListener : TraceListener, IOnkoTePlaDebugListener
	{
		public override void Write(string message)
		{
			OnNewDebugMessage?.Invoke(message);
		}

		public override void WriteLine(string message)
		{
			OnNewDebugMessage?.Invoke(message);
		}

		public event Action<string> OnNewDebugMessage;
	}
}
