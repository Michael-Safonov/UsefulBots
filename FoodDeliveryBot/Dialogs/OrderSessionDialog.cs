using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Utils;

namespace FoodDeliveryBot.Dialogs
{
	public class OrderSessionDialog : DialogContainer
	{
		private class StartMenuItem
		{
			public string DialogName { get; set; }
			public string Description { get; set; }
		}

		private readonly List<StartMenuItem> startMenuList = new List<StartMenuItem>()
		{
			new StartMenuItem() { Description = "Новый заказ", DialogName = "newOrder" },
			new StartMenuItem() { Description = "Существующий заказ", DialogName = "existOrder" },			
		};

		private readonly OrderSessionRepository orderSessionRepository;

		public const string Id = "orderSession";

		public static OrderSessionDialog Instance { get; } = new OrderSessionDialog(new OrderSessionRepository("OrderSessions"));

		private OrderSessionDialog(OrderSessionRepository orderSessionRepository) : base(Id)
		{
			this.orderSessionRepository = orderSessionRepository;
			InitOrderSessionDialog();
		}

		private void InitOrderSessionDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				GetOrderType,
				BeginNewOrExistDialog,
			});

			this.Dialogs.Add(JoinOrderDialog.Id, JoinOrderDialog.Instance);
			this.Dialogs.Add(DeliveryServiceDialog.Id, DeliveryServiceDialog.Instance);			
			this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}

		private async Task GetOrderType (DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var choiceList = this.startMenuList.Select(i => i.Description).ToList();
			await dc.Prompt("choicePrompt", "Доступные команды", new ChoicePromptOptions
			{	
				Choices = ChoiceFactory.ToChoices(choiceList),
				RetryPromptActivity = MessageFactory.SuggestedActions(choiceList, "Пожалуйста, выберите команду") as Activity,
			});
		}

		private async Task BeginNewOrExistDialog(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var choice = (FoundChoice)args["Value"];

			if (startMenuList[choice.Index].DialogName == "newOrder")
			{
				//Стартуем новый заказ
				OrderSession newOrder = new OrderSession()
				{
					OrderSessionId = Guid.NewGuid(),
					Pincode = PinCodeGenerator.GetPinCode(),
					OwnerUserId = dc.Context.Activity.From.Id ?? throw new Exception("Не нашел UserId")
				};

				await orderSessionRepository.Insert(newOrder);

				var sessionInfo = UserState<SessionInfo>.Get(dc.Context);
				sessionInfo.OrderSession = newOrder;

			    await dc.Context.SendActivity($"Пин код: {sessionInfo.OrderSession.Pincode}");

                await dc.Begin(DeliveryServiceDialog.Id);
			}
			else
			{
				await dc.Begin(JoinOrderDialog.Id);
			}
		}
	}
}
