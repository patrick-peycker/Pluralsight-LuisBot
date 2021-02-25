using Bot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
	public class MainDialog : ComponentDialog
	{
		private readonly StateService StateService;
		private readonly BotServices BotServices;

		public MainDialog(StateService StateService, BotServices BotServices) : base(nameof(MainDialog))
		{
			this.StateService = StateService ?? throw new ArgumentNullException(nameof(StateService));
			this.BotServices = BotServices ?? throw new ArgumentNullException(nameof(BotServices));

			InitializeWaterfallDialog();
		}

		private void InitializeWaterfallDialog()
		{
			/**
             * We are going to use a waterfall dialog to frame-up our conversation.
             * The great thing about the waterfall dialog is that it gives us a good
             * back and forth template to utilize for our conversation.
             * 
             * The first thing that we are going to do set-up our waterfall dialog
             * is to establish all the steps by creating an instance of the waterfall 
             * steps below. In the list are the detailed methods that are going to be
             * called and on what order in the waterfall flow. The order is very important
             * here since this will be called in the order based on the list.
             */
			var waterfallSteps = new WaterfallStep[]
			{
				InitialStepAsync,
				FinalStepAsync
			};

			// Add Named Dialogs
			AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", StateService));
			AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", StateService));
			AddDialog(new BugTypeDialog($"{nameof(MainDialog)}.bugType", StateService, BotServices));

			AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

			// Set the starting dialog
			InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
		}

		private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			/**
             * First, we use the dispatch model to determine which 
             * cognitive service (LUIS or QnA) to use
             */
			var recognizerResult = await BotServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
			/**
             * top intent tell us which cognitive service to use
             */
			var topIntent = recognizerResult.GetTopScoringIntent();

			switch (topIntent.intent)
			{
				case "GreetingIntent":
					return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
				case "NewBugReportIntent":
					return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
				case "QueryBugTypeIntent":
					return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugType", null, cancellationToken);
				default:
					await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I'm sorry I don't know what you mean."), cancellationToken);
					break;
			}

			return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}
