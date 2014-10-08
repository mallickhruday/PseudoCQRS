﻿using System.Web.Mvc;

namespace PseudoCQRS.Controllers
{
	[DbSessionManager]
	public abstract class BaseCommandController<TViewModel, TCommand> : Controller
	{
		private readonly ICommandExecutor _commandExecutor;

		protected BaseCommandController( ICommandExecutor commandExecutor )
		{
			_commandExecutor = commandExecutor;
		}

		public abstract ActionResult OnSuccessfulExecution( TViewModel viewModel, CommandResult commandResult );
		public abstract ActionResult OnFailureExecution( TViewModel viewModel, CommandResult commandResult );

		protected virtual CommandResult ExecuteCommand( TViewModel viewModel )
		{
			var command = ConvertViewModelToCommand( viewModel );
			var result = ExecuteCommand( command );
			return result;
		}

		private CommandResult ExecuteCommand( TCommand command )
		{
			return _commandExecutor.ExecuteCommand( command );
		}

		protected virtual TCommand ConvertViewModelToCommand( TViewModel viewModel )
		{
			return Mapper.Map<TViewModel, TCommand>( viewModel );
		}

		protected ActionResult ExecuteCommandAndGetActionResult( TViewModel viewModel )
		{
			var commandResult = ExecuteCommand( viewModel );
			return commandResult.ContainsError ? OnFailureExecution( viewModel, commandResult ) : OnSuccessfulExecution( viewModel, commandResult );
		}
	}
}