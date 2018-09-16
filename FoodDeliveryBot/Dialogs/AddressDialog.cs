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
		private readonly UserOrderRepository userOrderRepository;

		public const string Id = "getAddress";

		public static AddressDialog Instance
		{
			get
			{
				return new AddressDialog(
						new DeliveryServiceRepository("DeliveryServices"),
						new OrderSessionRepository("OrderSessions"),
						new UserOrderRepository("UserOrders")
					);
			}
		}

		private AddressDialog(DeliveryServiceRepository deliveryServiceRepository,
			OrderSessionRepository orderSessionRepository,
			UserOrderRepository userOrderRepository) : base(Id)
		{
			this.deliveryServiceRepository = deliveryServiceRepository;
			this.orderSessionRepository = orderSessionRepository;
			this.userOrderRepository = userOrderRepository;
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

            var orderSession = UserState<SessionInfo>.Get(dc.Context).OrderSession;
			var userOrders = (await this.userOrderRepository.GetBySessionId(orderSession.OrderSessionId)).ToList();
			//var products = orderSession.DeliveryService.Range;

            var messageList = userOrders.SelectMany(uo => uo.Products).GroupBy(x => x.Name)
                                        .Select(x => $"{x.Key} - {x.Count()} шт. ({x.Sum(p => p.Price)})\n").ToList();

            messageList.Add($"Доставка по адресу: {address}");
            var message = string.Join(Environment.NewLine, messageList);

            await dc.Context.SendActivity(message);

            var smsSender = new SMS.SmsSender();
            smsSender.SendSms(orderSession.DeliveryService.Phone, message);
			UserState<SessionInfo>.Get(dc.Context).OrderSession = null;
			dc.ActiveDialog.State.Clear();
            await dc.Replace(OrderSessionDialog.Id);
		}
	}
}
