﻿using System.Collections;
using System.Collections.Generic;


namespace bytePassion.Lib.Communication.State
{
	public class StateEngine : IStateEngine
	{

		private readonly IDictionary<string, object> states;

		public StateEngine()
		{
			states = new Dictionary<string, object>();
		}	

		public void RegisterState<T> (string stateIdentifier, T initialValue = default (T))
		{
			states.Add(stateIdentifier, new GlobalState<T>());

			var state = GetState<T>(stateIdentifier);
			state.Value = initialValue;
		}

		public GlobalState<T> GetState<T> (string stateIdentifier)
		{
			return (GlobalState<T>)states[stateIdentifier];
		}
	}	
}