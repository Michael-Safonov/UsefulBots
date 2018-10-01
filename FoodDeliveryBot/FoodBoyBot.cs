using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodDeliveryBot.Dialogs;
using FoodDeliveryBot.Menues;
using FoodDeliveryBot.Models;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;

namespace FoodDeliveryBot
{
    public class FoodBoyBot : IBot
	{
        private readonly OrderSessionDialog _orderDialog;

        private const string StartDialogId = "mainMenu";
		private DialogSet _dialogs { get; }

        public FoodBoyBot(OrderSessionDialog orderDialog)
        {
            _orderDialog = orderDialog ?? throw new ArgumentNullException(nameof(orderDialog));
            _dialogs = ComposeMainDialog();
        }

		public async Task OnTurn(ITurnContext context)
		{
			if (context.Activity.Type is ActivityTypes.Message)
			{
				var sessionInfo = UserState<SessionInfo>.Get(context);
				var conversationInfo = ConversationState<ConversationInfo>.Get(context);

				// Establish dialog state from the conversation state.
				var dc = _dialogs.CreateContext(context, conversationInfo);

				// Continue any current dialog.

				await dc.Continue();

				if (dc.Context.Responded)
				{
					return;
				}

				await dc.Begin(StartDialogId);
			}
		}

		private DialogSet ComposeMainDialog()
		{
			var dialogs = new DialogSet();

			dialogs.Add(StartDialogId, new WaterfallStep[]
			{
				BeginOrderSessionDialog,
                GoToFirstStep
            });

			dialogs.Add(OrderSessionDialog.Id, _orderDialog);
			return dialogs;
		}

        private static async Task BeginOrderSessionDialog(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Begin(OrderSessionDialog.Id);
        }

        private static async Task GoToFirstStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Replace(StartDialogId);
        }
    }
}