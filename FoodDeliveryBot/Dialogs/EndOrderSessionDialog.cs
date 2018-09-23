using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace FoodDeliveryBot.Dialogs
{
    public class EndOrderSessionDialog : DialogContainer
	{
        private readonly AddressDialog _addressDialog;

		private readonly OrderSessionRepository _orderSessionRepository;
		private readonly UserOrderRepository _userOrderRepository;

		public const string Id = "endOrderSession";

		public EndOrderSessionDialog(OrderSessionRepository orderSessionRepository, UserOrderRepository userOrderRepository, AddressDialog addressDialog) : base(Id)
		{
            _addressDialog = addressDialog;

            _orderSessionRepository = orderSessionRepository;
			_userOrderRepository = userOrderRepository;

			InitEndOrderSessionDialog();
		}

		private void InitEndOrderSessionDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				CheckOrderOwnerStep,

			});

			this.Dialogs.Add(AddressDialog.Id, _addressDialog);
			this.Dialogs.Add("textPrompt", new TextPrompt());
		}

		private async Task CheckOrderOwnerStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var orderSession = UserState<SessionInfo>.Get(dc.Context).OrderSession;
			var userId = dc.Context.Activity.From.Id ?? throw new Exception("Не нашел UserId");

			if (orderSession.OwnerUserId == userId)
			{
				// Сохраняем завершенный заказ
				orderSession.IsCompleted = true;
				await _orderSessionRepository.Upsert(orderSession);

				// Формируем чек
				var attachment = await GetReceiptCardSession(orderSession);
				await dc.Context.SendActivity(MessageFactory.Attachment(attachment));

				await dc.Begin(AddressDialog.Id);
			}
			else
			{
				await dc.Prompt("textPrompt", "Вы не являетесь администратором заказа");
				await dc.End();
			}
		}

		private async Task<Attachment> GetReceiptCardSession(OrderSession orderSession)
		{
			var userOrders = (await _userOrderRepository.GetBySessionId(orderSession.OrderSessionId)).ToList();
			var summaryOrder = userOrders.SelectMany(uo => uo.Products).Sum(p => p.Price);
			var receipt = new ReceiptCard
			{
				Title = "Общий заказ",
				Facts = new List<Fact> { new Fact(key: "Ключ заказа", value: orderSession.Pincode) },
				Items = userOrders.Select(uo => new ReceiptItem
				{
					Title = uo.UserName,
					Price = $"{uo.Products.Sum(p => p.Price):0.00}₽",
				}).ToList(),
				Total = summaryOrder.ToString("0.00"),
			}.ToAttachment();

			return receipt;
		}
	}
}
