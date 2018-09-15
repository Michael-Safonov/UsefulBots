using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Dialogs
{
	public class JoinOrderDialog : DialogContainer
	{
		private readonly OrderSessionRepository orderSessionRepository;

		public const string Id = "joinOrder";

		public static JoinOrderDialog Instance { get; } = new JoinOrderDialog(new OrderSessionRepository("OrderSessions"));

		private JoinOrderDialog(OrderSessionRepository orderSessionRepository) : base(Id)
		{
			this.orderSessionRepository = orderSessionRepository;
			InitJoinOrderDialog();
		}

		private void InitJoinOrderDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				GetOrderPincodeStep,
				CheckOrderPincodeStep
			});
			this.Dialogs.Add(ProductsDialog.Id, ProductsDialog.Instance);
			this.Dialogs.Add("textPrompt", new TextPrompt());
		}

		private async Task GetOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			await dc.Prompt("textPrompt", "Введите ключ существующего заказа:");
		}

		private async Task CheckOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var pincode = args["Value"] as string;
            var orderSession = await this.orderSessionRepository.GetByPinCode(pincode);

            if (orderSession != null)
			{
				UserState<SessionInfo>.Get(dc.Context).OrderSession = orderSession;
				await dc.Begin(ProductsDialog.Id);
			}
			else
			{
				await dc.Prompt("textPrompt", "Заказа с данным ключом не существует");
			}
		}
	}
}
