using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodDeliveryBot.Dialogs;
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
				// example of using DB
				//var db = DbManager.Instance;
				// var states = db.GetCollection<ChatMessage>(nameof(ChatMessage));
				//states.Insert(newMessage);
				//var messages = db.GetCollection<ChatMessage>(nameof(ChatMessage)).FindAll();

				var sessionInfo = UserState<SessionInfo>.Get(context);
				var conversationInfo = ConversationState<ConversationInfo>.Get(context);

				// Establish dialog state from the conversation state.
				var dc = _dialogs.CreateContext(context, conversationInfo);

				// Continue any current dialog.

				await dc.Continue();

				// Every turn sends a response, so if no response was sent,
				// then there no dialog is currently active.
				if (!context.Responded)
				{
					//Если еще нет информации о заказе, надо ее получить
					//if (string.IsNullOrWhiteSpace(userInfo?.Order?.OrderId))
					if (sessionInfo.OrderSession?.OrderSessionId == null)
					{
						await dc.Begin(OrderSessionDialog.Id);
					}
					// Otherwise, start our bot's main dialog.
					else
					{
						await dc.Begin(MainMenuDialogId);
					}
				}
			}
		}

		private static DialogSet ComposeMainDialog()
		{
			var dialogs = new DialogSet();
			var userOrderActions = new List<string> { "Выбор продуктов", "Статистика", "Отменить заказ", "Завершить заказ" };

			dialogs.Add(MainMenuDialogId, new WaterfallStep[]
			{				
				async (dc, args, next) =>
				{
					// await dc.Prompt("textPrompt", $"Доставка из {orderSession.DeliveryService.Name}\nКод заказа: {orderSession.Pincode}");
					await dc.Prompt("choicePrompt", "Выберите действие:", new ChoicePromptOptions
					{
						Choices = ChoiceFactory.ToChoices(userOrderActions),
						RetryPromptActivity = MessageFactory.SuggestedActions(userOrderActions, "Пожалуйста, выберите действие") as Activity,
					});
				},
				async (dc, args, next) =>
				{
					var choice = (FoundChoice)args["Value"];
					if (choice.Value == "Выбор продуктов")
					{
						await dc.Begin(ProductsDialog.Id);
					}
					else if (choice.Value == "Статистика")
					{
						//реализовать вывод статистики
						await next();
					}
					else if (choice.Value == "Отменить заказ")
					{
						var sessionInfo = UserState<SessionInfo>.Get(dc.Context);
						sessionInfo.OrderSession = null;
					}
					else if (choice.Value == "Завершить заказ")
					{
						await dc.Begin(EndOrderSessionDialog.Id);
					}
					else
					{
						await dc.Context.SendActivity("Не понимаю.");
						await next();
					}
				},
				async (dc, args, next) =>
				{
					 //Show the main menu again.
					if (args == null)
					{
					}
					else
					{
						await dc.Replace(MainMenuDialogId);
					}
					 
				}
			});

			dialogs.Add(OrderDialog.Id, OrderDialog.Instance);
			dialogs.Add(OrderSessionDialog.Id, OrderSessionDialog.Instance);
			dialogs.Add(ProductsDialog.Id, ProductsDialog.Instance);
			dialogs.Add(EndOrderSessionDialog.Id, EndOrderSessionDialog.Instance);
			dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
			return dialogs;
		}

		private class ChatMessage
		{
			public int Id { get; set; }
			public DateTime DateTime { get; set; }
			public string Text { get; set; }
		}
	}
}