using System;
using System.Collections.Generic;
using System.Windows;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Client.DataAndService.Connection;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.UndoRedo;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.UndoRedo.UserActions;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Contracts.Config;


namespace bytePassion.OnkoTePla.Client.DataAndService.SessionInfo
{
	public class Session : ISession
	{
	    private readonly IConnectionService connectionService;
	    private readonly IClientWorkflow clientWorkflow;		
		

		private ApplicationState currentApplicationState;		    

		public Session(IConnectionService connectionService, 
					   IClientWorkflow clientWorkflow)
	    {
		    this.connectionService = connectionService;
		    this.clientWorkflow = clientWorkflow;			

			CurrentApplicationState = clientWorkflow.CurrentState;

			clientWorkflow.StateChanged += OnApplicationStateChanged;
			connectionService.ConnectionEventInvoked += OnConnectionServiceEventInvoked;
		}
		

		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                 applicationState                                  ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////

		public event Action<ApplicationState> ApplicationStateChanged;

		public ApplicationState CurrentApplicationState
		{
			get { return currentApplicationState; }
			private set
			{
				if (currentApplicationState != value)
				{
					currentApplicationState = value;					
					ApplicationStateChanged?.Invoke(CurrentApplicationState);
				}
			}		
		}
		
		private void OnApplicationStateChanged (ApplicationState newApplicationState)
		{
			CurrentApplicationState = newApplicationState;
		}

		private void ApplyWorkflowEvent (WorkflowEvent workflowEvent)
		{
			clientWorkflow.ApplyEvent(workflowEvent);
		}


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                     userInfo                                      ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////


		public ClientUserData LoggedInUser { get; private set; }

		public void RequestUserList(Action<IReadOnlyList<ClientUserData>> dataReceivedCallback, 
									Action<string> errorCallback)
		{
			connectionService.RequestUserList(dataReceivedCallback, errorCallback);
		}
		
		public void TryLogin(ClientUserData user, string password, Action<string> errorCallback)
		{
			connectionService.TryLogin(
				() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						LoggedInUser = user;
						ApplyWorkflowEvent(WorkflowEvent.LoggedIn);

						undoRedoService = new UndoRedoService(50);
						undoRedoService.RedoPossibleChanged += OnUndoRedoServiceRedoPossibleChanged;
						undoRedoService.UndoPossibleChanged += OnUndoRedoServiceUndoPossibleChanged;						
					});
				},
				user,
				password,
				errorCallback	
			);			
		}

		

		public void Logout(Action logoutSuccessful, Action<string> errorCallback)
		{
			connectionService.TryLogout(
				() =>
				{
					Application.Current?.Dispatcher.Invoke(() =>
					{
						undoRedoService.RedoPossibleChanged -= OnUndoRedoServiceRedoPossibleChanged;
						undoRedoService.UndoPossibleChanged -= OnUndoRedoServiceUndoPossibleChanged;
						undoRedoService = null;
						
						LoggedInUser = null;
						ApplyWorkflowEvent(WorkflowEvent.LoggedOut);

						logoutSuccessful?.Invoke();
					});
				},
				LoggedInUser,
				errorCallback
			);

			
		}


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                 serverConnection                                  ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////


		private void OnConnectionServiceEventInvoked (ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
				case ConnectionEvent.ConnectionEstablished:  ApplyWorkflowEvent(WorkflowEvent.ConnectionEstablished);  break;
				case ConnectionEvent.Disconnected:           ApplyWorkflowEvent(WorkflowEvent.Disconnected);           break;
				case ConnectionEvent.ConAttemptUnsuccessful: ApplyWorkflowEvent(WorkflowEvent.ConAttemptUnsuccessful); break;
				case ConnectionEvent.ConnectionLost:         ApplyWorkflowEvent(WorkflowEvent.ConnectionLost);         break;
				case ConnectionEvent.StartedTryConnect:      ApplyWorkflowEvent(WorkflowEvent.StartedTryConnect);      break;
				case ConnectionEvent.StartedTryDisconnect:   ApplyWorkflowEvent(WorkflowEvent.StartedTryDisconnect);   break;
			}
		}		

		public void TryConnect (Address serverAddress, Address clientAddress, Action<string> errorCallback)
		{
			connectionService.TryConnect(serverAddress, clientAddress, errorCallback);
		}

		public void TryDebugConnect(Address serverAddress, Address clientAddress, Action<string> errorCallback)
		{
			connectionService.TryDebugConnect(serverAddress, clientAddress, errorCallback);
		}

		public void TryDisconnect(Action dissconnectionSuccessful, Action<string> errorCallback)
		{
			connectionService.TryDisconnect(dissconnectionSuccessful, errorCallback);
		}

		public void TryToGetLock(Action<bool> resultCallback, Guid medicalPracticeId, Date day, Action<string> errorCallback)
		{
			connectionService.TryToGetLock(resultCallback, medicalPracticeId, day, errorCallback);
		}

		public void ReleaseLock(Action actionCompleteCallback, Guid medicalPracticeId, Date day, Action<string> errorCallback)
		{
			connectionService.ReleaseLock(actionCompleteCallback, medicalPracticeId, day, errorCallback);
		}


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                    undo / redo                                    ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		 		
		private IUndoRedoService undoRedoService;

		public event Action<bool> UndoPossibleChanged;
		public event Action<bool> RedoPossibleChanged;

		private void OnUndoRedoServiceUndoPossibleChanged (bool b) { UndoPossibleChanged?.Invoke(b); }		
		private void OnUndoRedoServiceRedoPossibleChanged (bool b) { RedoPossibleChanged?.Invoke(b); }
		
		public bool UndoPossible () => undoRedoService != null && undoRedoService.UndoPossible;		
		public bool RedoPossible () => undoRedoService != null && undoRedoService.RedoPossible;
		
		public void Undo (Action<bool> operationResultCallback, Action<string> errorCallback) { undoRedoService?.Undo(operationResultCallback, errorCallback); }
		public void Redo (Action<bool> operationResultCallback, Action<string> errorCallback) { undoRedoService?.Redo(operationResultCallback, errorCallback); }
		
		public string GetCurrentUndoActionMsg()
		{
			if (undoRedoService == null)
				throw new InvalidOperationException("the undoRedoService is not initiated");

			return undoRedoService.GetCurrentUndoActionMsg();
		}				

		public string GetCurrentRedoActionMsg()
		{
			if (undoRedoService == null)
				throw new InvalidOperationException("the undoRedoService is not initiated");

			return undoRedoService.GetCurrentRedoActionMsg();
		}
	
		public void ReportUserAction(IUserAction newUserAction)
		{
			undoRedoService.ReportUserAction(newUserAction);
		}

		public void ResetUndoRedoHistory()
		{
			undoRedoService.ResetHistory();
		}
	}
}
