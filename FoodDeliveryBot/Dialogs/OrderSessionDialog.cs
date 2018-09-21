using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using FoodDeliveryBot.Utils;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;

namespace FoodDeliveryBot.Dialogs
{
    public class OrderSessionDialog : DialogContainer
	{
        private readonly DeliveryServiceDialog _deliveryServiceDialog;
        private readonly MainMenuDialog _mainMenuDialog;
        private readonly JoinOrderDialog _joinOrderDialog;
        private readonly List<StartMenuItem> startMenuList = new List<StartMenuItem>
		{
			new StartMenuItem { Description = "Новый заказ", DialogName = "newOrder" },
			new StartMenuItem { Description = "Существующий заказ", DialogName = "existOrder" },
		};

		private readonly OrderSessionRepository _orderSessionRepository;

		public const string Id = "orderSession";

		public OrderSessionDialog(
            OrderSessionRepository orderSessionRepository,
            JoinOrderDialog joinOrderDialog,
            MainMenuDialog mainMenuDialog,
            DeliveryServiceDialog deliveryServiceDialog) : base(Id)
		{
			_orderSessionRepository = orderSessionRepository;
            _joinOrderDialog = joinOrderDialog;
            _mainMenuDialog = mainMenuDialog;
            _deliveryServiceDialog = deliveryServiceDialog;

            InitOrderSessionDialog();
        }

		private void InitOrderSessionDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				GetOrderType,
				BeginNewOrExistDialog,
                BeginMainMenuDialog,
                EndDialog
            });

			this.Dialogs.Add(JoinOrderDialog.Id, _joinOrderDialog);
            this.Dialogs.Add(MainMenuDialog.Id, _mainMenuDialog);
            this.Dialogs.Add(DeliveryServiceDialog.Id, _deliveryServiceDialog);
            this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}

        private async Task BeginMainMenuDialog(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Begin(MainMenuDialog.Id);
        }

        private async Task GetOrderType(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var choiceList = startMenuList.Select(i => i.Description).ToList();
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
				var newOrder = new OrderSession()
				{
					OrderSessionId = Guid.NewGuid(),
					Pincode = PinCodeGenerator.GetPinCode(),
					OwnerUserId = dc.Context.Activity.From.Id ?? throw new Exception("Не нашел UserId")
				};

                await _orderSessionRepository.Insert(newOrder);

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

        private async Task EndDialog(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.End();
        }
        private class StartMenuItem
        {
            public string DialogName { get; set; }
            public string Description { get; set; }
        }
    }
}
