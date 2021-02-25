using Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;

namespace Bot.Services
{
	public class StateService
	{
		// State
		public UserState UserState { get; }
		public ConversationState ConversationState { get; set; }

		// IDs
		public static string UserProfileId { get; } = $"{nameof(StateService)}.UserProfile";
		public static string ConversationDataId { get; } = $"{nameof(ConversationData)}.ConversationData";
		public static string DialogStateId { get; } = $"{nameof(StateService)}.DialogState";

		// Accessors
		public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
		public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
		public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }


		public StateService(UserState userState, ConversationState conversationState)
		{
			UserState = userState ?? throw new ArgumentNullException($"{nameof(userState)} in State Service Class");
			ConversationState = conversationState ?? throw new ArgumentNullException($"{nameof(conversationState)} in State Service Class");

			InitializeAccessors();
		}

		public void InitializeAccessors()
		{
			UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
			ConversationDataAccessor = UserState.CreateProperty<ConversationData>(ConversationDataId);
			DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
		}
	}
}
