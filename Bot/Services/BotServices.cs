using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace Bot.Services
{
	public class BotServices
	{
		public LuisRecognizer Dispatch { get; private set; }

		public BotServices(IConfiguration configuration)
		{
			var luisApplication = new LuisApplication(configuration["Luis:AppId"], configuration["Luis:APIKey"], configuration["Luis:Url"]);

			var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
			{
				PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
				{
					IncludeAllIntents = true,
					IncludeInstanceData = true
				}
			};

			Dispatch = new LuisRecognizer(recognizerOptions);
		}
	}
}
