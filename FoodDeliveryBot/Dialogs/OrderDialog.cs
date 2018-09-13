using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Bot.Builder.Prompts.Choices;
using FoodDeliveryBot.Models;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;

namespace FoodDeliveryBot.Dialogs
{
	public class OrderDialog : DialogContainer
	{
		public const string Id = "order";

		private const string OrderKey = nameof(OrderDialog);
		public static List<StartChoice> OrderMenu { get; } = new List<StartChoice>
		{
			new StartChoice { Description = "Новый заказ", DialogName = "newOrder" },
			new StartChoice { Description = "Существующий заказ", DialogName = "existOrder" },
		};
		public static List<string> MainMenu
		{
			get
			{
				return OrderMenu.Select(x => x.Description).ToList();
			}
		}
		public static OrderDialog Instance { get; } = new OrderDialog();
		private OrderDialog() : base(Id)
		{
			//todo: Добавить диалог выбора магазина. Подгружать из БД			
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				async (dc, args, next) =>
				{
					await dc.Prompt("choicePrompt", "Доступные команды", new ChoicePromptOptions
					{
						Choices = ChoiceFactory.ToChoices(MainMenu),
						RetryPromptActivity = MessageFactory.SuggestedActions(MainMenu, "Пожалуйста, выберите команду") as Activity,
					});
				},
				async (dc, args, next) =>
				{
					var choice = (FoundChoice)args["Value"];
					if (OrderMenu[choice.Index].DialogName == "newOrder")
					{
						var userState = UserState<UserInfo>.Get(dc.Context);
						var newOrder = new OrderInfo {
							OrderId = System.Guid.NewGuid().ToString()
						};
						userState.Order = newOrder;
						
						await dc.Begin(DeliveryServiceDialog.Id);
				
					}
					else
					{
						await dc.Begin("existOrder", dc.ActiveDialog.State);
					}
				},
				async (dc, args, next) =>
				{
					await dc.Replace(Id);
				},
			});
			this.Dialogs.Add("existOrder", new WaterfallStep[]
			{
				async (dc, args, next) =>
				{
					await dc.Prompt("textPrompt", "Введите идентификатор существующего заказа:");
				},
				async (dc, args, next) => {
					var orderId = args["Value"] as string;
					//todo: check if entered order exist in db. 
					dc.ActiveDialog.State[OrderKey] = new OrderInfo();
					var orderInfo = dc.ActiveDialog.State[OrderKey];
					 ((OrderInfo)orderInfo).OrderId = orderId;
					await dc.Prompt("textPrompt", "Введите идентификатор нотификации:");
				},
				async (dc, args, next) => {
					var notificationId = args["Value"] as string;
					var orderInfo = dc.ActiveDialog.State[OrderKey];
					((OrderInfo)orderInfo).NotificationId = notificationId;
					 dc.ActiveDialog.State[OrderKey] = (OrderInfo)orderInfo;
					var userState = UserState<UserInfo>.Get(dc.Context);
					userState.Order = (OrderInfo)orderInfo;
				}
			});
			this.Dialogs.Add(DeliveryServiceDialog.Id, DeliveryServiceDialog.Instance);
			this.Dialogs.Add("textPrompt", new TextPrompt());
			this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}
	}
	public class StartChoice
	{
		public string DialogName { get; set; }
		public string Description { get; set; }
	}
}