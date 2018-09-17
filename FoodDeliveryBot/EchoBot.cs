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
	public class EchoBot : IBot
	{
		private const string MainMenuDialogId = "mainMenu";
		private DialogSet _dialogs { get; } = ComposeMainDialog();


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
				
				await dc.Begin(MainMenuDialogId);
			}
		}

		private static DialogSet ComposeMainDialog()
		{
			var dialogs = new DialogSet();

			dialogs.Add(MainMenuDialogId, new WaterfallStep[]
			{		
				BeginOrderSessionDialog,		
				ShowMainMenu,
				MainMenuProcessing,
				GoToFirstStep
			});

			dialogs.Add(OrderSessionDialog.Id, OrderSessionDialog.Instance);
			dialogs.Add(ProductsDialog.Id, ProductsDialog.Instance);
			dialogs.Add(EndOrderSessionDialog.Id, EndOrderSessionDialog.Instance);
			dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
			return dialogs;
		}

        private static async Task GoToFirstStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Replace(MainMenuDialogId);
        }

        private static async Task MainMenuProcessing(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var choice = (FoundChoice)args["Value"];

			switch (choice.Value)
			{
				case MainMenu.ChooseProducts: 
					await dc.Begin(ProductsDialog.Id);
					break;
				case MainMenu.Statistics:
					//реализовать вывод статистики
					await next();
					break;
				case MainMenu.CancelOrder:
					var sessionInfo = UserState<SessionInfo>.Get(dc.Context);
					sessionInfo.OrderSession = null;
					await dc.End();
					break;
				case MainMenu.FinishOrder:
					await dc.Begin(EndOrderSessionDialog.Id);
					break;
				default:
					await dc.Context.SendActivity("Не понимаю.");
					await next();
					break;
			}
        }

        private static async Task ShowMainMenu(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Prompt("choicePrompt", "Выберите действие:", new ChoicePromptOptions
					{
						Choices = ChoiceFactory.ToChoices(MainMenu.MenuList),
						RetryPromptActivity = MessageFactory.SuggestedActions(MainMenu.MenuList, "Пожалуйста, выберите действие") as Activity,
					});
        }

        private static async Task BeginOrderSessionDialog(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Begin(OrderSessionDialog.Id);
        }
	}
}