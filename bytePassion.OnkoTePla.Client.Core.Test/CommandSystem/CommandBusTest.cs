﻿using System;
using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Client.Core.CommandSystem;
using bytePassion.OnkoTePla.Client.Core.CommandSystem.DomainCommands;
using bytePassion.OnkoTePla.Client.Core.Domain;
using Xunit;

namespace bytePassion.OnkoTePla.Client.Core.Test.CommandSystem
{
	public class CommandBusTest
	{		

		private class TestCommandHandler : IDomainCommandHandler<AddAppointment>
		{
			public TestCommandHandler()
			{
				CommandExecuted = false;
			}

			public void Execute(AddAppointment command)
			{
				CommandExecuted = true;
			}

			public bool CommandExecuted { private set; get; }
		}	

		[Fact]
		public void CommandRegistrationAndExecutionTest()
		{
			ICommandBus commandBus = new CommandBus();
			var testCommandHandler = new TestCommandHandler();

			Assert.False(testCommandHandler.CommandExecuted);

			commandBus.RegisterCommandHandler(testCommandHandler);
			commandBus.Send(new AddAppointment(new AggregateIdentifier(Date.Dummy, new Guid()), -1, new Guid(), null, null, Date.Dummy, Time.Dummy, Time.Dummy, null, null));

			Assert.True(testCommandHandler.CommandExecuted);
		}
	}
}
