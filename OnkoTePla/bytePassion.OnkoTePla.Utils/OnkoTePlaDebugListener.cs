using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bytePassion.OnkoTePla.Utils
{

	public class OnkoTePlaDebugListener : TraceListener, IOnkoTePlaDebugListener
	{
		private bool initialGrab;
		private readonly IList<string> initialList;

		public OnkoTePlaDebugListener()
		{
			initialGrab = false;
			initialList = new List<string>();
		}
		
		public override void Write(string message)
		{
			if (initialGrab)
				OnNewDebugMessage?.Invoke(message);
			else
				initialList.Add(message);
		}

		public override void WriteLine(string message)
		{
			if (initialGrab)
				OnNewDebugMessage?.Invoke(message);
			else
				initialList.Add(message);
		}

		public event Action<string> OnNewDebugMessage;

		public IList<string> GetInitialList()
		{
			initialGrab = true;
			return initialList;
		}
	}
}
