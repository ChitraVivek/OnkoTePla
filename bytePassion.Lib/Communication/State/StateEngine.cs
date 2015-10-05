﻿using System.Collections.Generic;


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

		public void RegisterState<T>(string stateIdentifier, IGlobalState<T> newGlobalState)
		{
			states.Add(stateIdentifier, newGlobalState);
		}

		public void RegisterStateReadOnly<T>(string stateIdentifier, T value)
		{
			states.Add(stateIdentifier, new GlobalReadOnlyState<T>(value));
		}

		public IGlobalState<T> GetState<T> (string stateIdentifier)
		{
			return (IGlobalState<T>)states[stateIdentifier];
		}
	}	
}
