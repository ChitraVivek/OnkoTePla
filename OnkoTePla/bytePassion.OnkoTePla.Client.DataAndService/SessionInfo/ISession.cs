﻿using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Client.DataAndService.Domain.UndoRedo.UserActions;
using bytePassion.OnkoTePla.Client.DataAndService.Workflow;
using bytePassion.OnkoTePla.Contracts.Config;

namespace bytePassion.OnkoTePla.Client.DataAndService.SessionInfo
{
	public interface ISession
	{
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                 applicationState                                  ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////

		event Action<ApplicationState> ApplicationStateChanged;		
		ApplicationState CurrentApplicationState { get; }



		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                       user                                        ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////		
		 
		ClientUserData LoggedInUser { get; }
		void RequestUserList(Action<IReadOnlyList<ClientUserData>> dataReceivedCallback,
							 Action<string> errorCallback); 
		
		void TryLogin(ClientUserData user, string password, Action<string> errorCallback);
		void Logout(Action logoutSuccessful, Action<string> errorCallback);


		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                 serverConnection                                  ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////

		void TryConnect     (Address serverAddress, Address clientAddress, Action<string> errorCallback);
		void TryDebugConnect(Address serverAddress, Address clientAddress, Action<string> errorCallback);
		void TryDisconnect  (Action dissconnectionSuccessful, Action<string> errorCallback);



		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                     locking                                       ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////		

		void TryToGetLock(Action<bool> resultCallback,   Guid medicalPracticeId, Date day, Action<string> errorCallback);
		void ReleaseLock (Action actionCompleteCallback, Guid medicalPracticeId, Date day, Action<string> errorCallback);


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/////////                                                                                   ///////////
		/////////                                    undo / redo                                    ///////////
		/////////                                                                                   ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////

		event Action<bool> UndoPossibleChanged;
		event Action<bool> RedoPossibleChanged;

		bool UndoPossible();
		void Undo(Action<bool> operationResultCallback, Action<string> errorCallback);
		string GetCurrentUndoActionMsg();		
		
		bool RedoPossible();
		void Redo(Action<bool> operationResultCallback, Action<string> errorCallback);
		string GetCurrentRedoActionMsg ();

		void ReportUserAction(IUserAction newUserAction);
		void ResetUndoRedoHistory();
	}
}
