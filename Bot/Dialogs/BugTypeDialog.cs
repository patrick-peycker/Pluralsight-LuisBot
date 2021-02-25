using Bot.Helpers;
using Bot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
	public class BugTypeDialog : ComponentDialog
	{
		private readonly StateService stateService;
		private readonly BotServices botServices;

		public BugTypeDialog(string dialogId, StateService StateService, BotServices BotServices) : base(dialogId)
		{
			stateService = StateService ?? throw new ArgumentNullException(nameof(stateService));
			botServices = BotServices ?? throw new ArgumentNullException(nameof(botServices));

			InitializeWaterfallDialog();
		}

		private void InitializeWaterfallDialog()
		{
			// Create Waterfall Steps
			var waterfallSteps = new WaterfallStep[]
			{
					 InitialStepAsync,
					 FinalStepAsync
			};

			// Add Named Dialogs
			AddDialog(new WaterfallDialog($"{nameof(BugTypeDialog)}.mainFlow", waterfallSteps));

			// Set the starting Dialog
			InitialDialogId = $"{nameof(BugTypeDialog)}.mainFlow";
		}

		private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var result = await botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
			var token = result.Entities.FindTokens("BugType").FirstOrDefault();
			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			var value = rgx.Replace(token.ToString(), "").Trim();

			if (Common.BugTypes.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase)))
			{
				await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Yes! {0} is a Bug Type!", value)), cancellationToken);
			}

			else
			{
				await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("No {0} is not a Bug Type.", value)), cancellationToken);
			}

			return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}
