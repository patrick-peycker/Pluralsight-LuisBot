// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.11.1

using Bot.Helpers;
using Bot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Bots
{
	public class EchoBot<T> : ActivityHandler where T : Dialog
	{
		protected readonly Dialog dialog;
		protected readonly StateService stateService;
		protected readonly ILogger logger;

		public EchoBot(T dialog, StateService stateService, ILogger<EchoBot<T>> logger)
		{
			this.dialog = dialog ?? throw new ArgumentNullException($"{nameof(dialog)} in Dialog Bot");
			this.stateService = stateService ?? throw new ArgumentNullException($"{nameof(stateService)} in Dialog Bot");
			this.logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} in Dialog Bot");
		}

		/*
		* This is another hook of the base class of the ActivityHandler.
		* This is called anytime a bot gets any activity at all regardless of the message type.
		* So, if the bot receives a message, this OnTurnAsync method and OnMessageActivityAsync will be called.
		*/

		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn
			await stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
			await stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			logger.LogInformation("Running dialog with MessageActivity.");

			// Run the dialog with the new message Activity.
			await dialog.Run(turnContext, stateService.DialogStateAccessor, cancellationToken);
		}
	}
}
