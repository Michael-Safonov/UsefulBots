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

namespace FoodDeliveryBot.Dialogs
{
	public class DeliveryServiceDialog : DialogContainer
	{
		private readonly List<DeliveryService> deliveryServicesList = new List<DeliveryService>()
		{
			new DeliveryService() { Id = 1, Name = "ШаурмаKing" },
			new DeliveryService() { Id = 2, Name= "Самурай" },
			new DeliveryService() { Id = 3, Name= "Автосуши" }
		};

		public const string Id = "choiceDeliveryService";

		public static DeliveryServiceDialog Instance { get; } = new DeliveryServiceDialog();

		private DeliveryServiceDialog() : base(Id)
		{
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
			var choiceList = this.deliveryServicesList.Select(ds => ds.Name).ToList();
			await dc.Prompt("choicePrompt", "Откуда закажем?", new ChoicePromptOptions
			{
				Choices = ChoiceFactory.ToChoices(choiceList),
				RetryPromptActivity = MessageFactory.SuggestedActions(choiceList, "Пожалуйста, выберите команду") as Activity,
			});
		}

		private async Task SetDeliveryServiceStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var choice = (FoundChoice)args["Value"];

			var deliveryService = deliveryServicesList.SingleOrDefault(ds => ds.Name == choice.Value);
			var userState = UserState<UserInfo>.Get(dc.Context);
			userState.OrderDeliveryService = deliveryService;
		}
	}
}
