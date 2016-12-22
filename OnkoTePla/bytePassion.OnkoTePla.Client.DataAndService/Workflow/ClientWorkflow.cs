using System.Collections.Generic;
using bytePassion.Lib.Utils.Workflow;
using Event = bytePassion.OnkoTePla.Client.DataAndService.Workflow.WorkflowEvent;
using State = bytePassion.OnkoTePla.Client.DataAndService.Workflow.ApplicationState;
using Transition = bytePassion.Lib.Utils.Workflow.StateTransition<bytePassion.OnkoTePla.Client.DataAndService.Workflow.ApplicationState,
																  bytePassion.OnkoTePla.Client.DataAndService.Workflow.WorkflowEvent>;

namespace bytePassion.OnkoTePla.Client.DataAndService.Workflow
{
	public class ClientWorkflow : WorkflowEngine<State, Event>, 
                                  IClientWorkflow
    {
        private static readonly IReadOnlyList<StateTransition<State, Event>> Transitions 
			= new List<StateTransition<State, Event>>
        {
            new Transition(State.DisconnectedFromServer,  Event.StartedTryConnect,      State.TryingToConnect),
			new Transition(State.TryingToConnect,         Event.ConAttemptUnsuccessful, State.DisconnectedFromServer),
			new Transition(State.TryingToConnect,         Event.ConnectionEstablished,  State.ConnectedButNotLoggedIn),
			new Transition(State.TryingToConnect,         Event.StartedTryDisconnect,   State.TryingToDisconnect),
			new Transition(State.ConnectedButNotLoggedIn, Event.StartedTryDisconnect,   State.TryingToDisconnect),
			new Transition(State.TryingToDisconnect,      Event.Disconnected,           State.DisconnectedFromServer),
			new Transition(State.TryingToDisconnect,      Event.ConAttemptUnsuccessful, State.DisconnectedFromServer),			
			new Transition(State.TryingToConnect,         Event.ConnectionLost,         State.DisconnectedFromServer),
			new Transition(State.ConnectedButNotLoggedIn, Event.ConnectionLost,         State.DisconnectedFromServer),
			new Transition(State.ConnectedButNotLoggedIn, Event.LoggedIn,               State.LoggedIn),
			new Transition(State.LoggedIn,                Event.LoggedOut,              State.ConnectedButNotLoggedIn),
			new Transition(State.LoggedIn,                Event.ConnectionLost,         State.DisconnectedFromServer),
		};
        
        public ClientWorkflow()
            : base(Transitions, State.DisconnectedFromServer)
        {            
        }
    }
}
