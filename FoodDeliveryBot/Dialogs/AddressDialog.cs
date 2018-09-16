using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Dialogs
{
	public class AddressDialog : DialogContainer
	{
		private readonly DeliveryServiceRepository deliveryServiceRepository;
		private readonly OrderSessionRepository orderSessionRepository;

		public const string Id = "getAddress";

		public static AddressDialog Instance
		{
			get
			{
				return new AddressDialog(
						new DeliveryServiceRepository("DeliveryServices"),
						new OrderSessionRepository("OrderSessions")
					);
			}
		}

		private AddressDialog(DeliveryServiceRepository deliveryServiceRepository, OrderSessionRepository orderSessionRepository) : base(Id)
		{
			this.deliveryServiceRepository = deliveryServiceRepository;
			this.orderSessionRepository = orderSessionRepository;
			InitDeliveryServiceDialog();
		}

		private void InitDeliveryServiceDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				GetAddressStep,
				SetAddressStep
			});

            this.Dialogs.Add(OrderSessionDialog.Id, OrderSessionDialog.Instance);
			this.Dialogs.Add("textPrompt", new TextPrompt());
		}

		private async Task GetAddressStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			//todo:сделать прием гео данных
			await dc.Prompt("textPrompt", "Введите адрес доставки заказа:");
		}

		private async Task SetAddressStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var address = args["Value"] as string;
			//todo:отправить смс по телефону который взять из DeliveryService
			await dc.Context.SendActivity($"Заказ отправлен.\nЗаказ будет доставлен по адресу:\n{address}");

			UserState<SessionInfo>.Get(dc.Context).OrderSession = null;
			dc.ActiveDialog.State.Clear();
            await dc.Replace(OrderSessionDialog.Id);
		}
	}
}
