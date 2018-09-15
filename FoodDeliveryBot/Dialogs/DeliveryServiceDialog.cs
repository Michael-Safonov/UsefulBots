using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;

namespace FoodDeliveryBot.Dialogs
{
	public class DeliveryServiceDialog : DialogContainer
	{
		private readonly DeliveryServiceRepository deliveryServiceRepository;
		private readonly OrderSessionRepository orderSessionRepository;

		public const string Id = "choiceDeliveryService";

		public static DeliveryServiceDialog Instance
		{
			get
			{
				return new DeliveryServiceDialog(
						new DeliveryServiceRepository("DeliveryServices"),
						new OrderSessionRepository("OrderSessions")
					);
			}
		}		

		private DeliveryServiceDialog(DeliveryServiceRepository deliveryServiceRepository, OrderSessionRepository orderSessionRepository): base(Id)
		{
			this.deliveryServiceRepository = deliveryServiceRepository;
			this.orderSessionRepository = orderSessionRepository;
			InitDeliveryServiceDialog();
		}

		private void InitDeliveryServiceDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				ChoiceDeliveryServiceStep,
				SetDeliveryServiceStep
			});

			this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}

		private async Task ChoiceDeliveryServiceStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			//todo: сделать полем класса?
			var deliveryServices = await this.deliveryServiceRepository.GetAll();

			var choiceList = deliveryServices.Select(ds => ds.Name).ToList();
			await dc.Prompt("choicePrompt", "Откуда закажем?", new ChoicePromptOptions
			{
				Choices = ChoiceFactory.ToChoices(choiceList),
				RetryPromptActivity = MessageFactory.SuggestedActions(choiceList, "Пожалуйста, выберите команду") as Activity,
			});
		}

		private async Task SetDeliveryServiceStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var choice = (FoundChoice)args["Value"];

			//todo: тягали все сервисы в методе выше
			// Получаем из БД нужный сервис доставки
			var deliveryService = await this.deliveryServiceRepository.GetByName(choice.Value);
			
			var sessionInfo = UserState<SessionInfo>.Get(dc.Context);
			sessionInfo.OrderSession.DeliveryService = deliveryService;

			//сохраняем обновленный OrderSession в БД
			await this.orderSessionRepository.Update(sessionInfo.OrderSession);
		}
	}
}
