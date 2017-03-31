using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
