using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Dialogs
{
	public class EndOrderSessionDialog : DialogContainer
	{
		private readonly OrderSessionRepository orderSessionRepository;
		private readonly UserOrderRepository userOrderRepository;

		public const string Id = "endOrderSession";

		public static EndOrderSessionDialog Instance
		{
			get
			{
				return new EndOrderSessionDialog(new OrderSessionRepository("OrderSessions"), new UserOrderRepository("UserOrders"));
			}
		}

		private EndOrderSessionDialog(OrderSessionRepository orderSessionRepository, UserOrderRepository userOrderRepository) : base(Id)
		{
			this.orderSessionRepository = orderSessionRepository;
			this.userOrderRepository = userOrderRepository;
			InitEndOrderSessionDialog();
		}

		private void InitEndOrderSessionDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				CheckOrderOwnerStep,
				
			});
			
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
				await this.orderSessionRepository.Upsert(orderSession);

				// Формируем чек
				var attachment = await GetReceiptCardSession(orderSession);
				await dc.Context.SendActivity(MessageFactory.Attachment(attachment));

				UserState<SessionInfo>.Get(dc.Context).OrderSession = null;
				await dc.End();
			}
			else
			{
				await dc.Prompt("textPrompt", "Вы не являетесь администратором заказа");
			}
		}

		private async Task<Attachment> GetReceiptCardSession(OrderSession orderSession)
		{
			//todo:собрать статистику сессии и красиво показать
			var userOrders = (await this.userOrderRepository.GetBySessionId(orderSession.OrderSessionId)).ToList();
			var summaryOrder = userOrders.SelectMany(uo => uo.Products).Sum(p => p.Price);
			var receipt = new ReceiptCard
			{
				Title = "Общий заказ",
				Facts = new List<Fact> { new Fact(key: "Order Id", value: orderSession.OrderSessionId.ToString()) },
				Items = userOrders.Select(uo => new ReceiptItem
				{
					Title = uo.UserId,
					Price = uo.Products.Sum(p => p.Price).ToString("0.00")
				}).ToList(),
				Total = summaryOrder.ToString("0.00"),
			}.ToAttachment();

			return receipt;
		}
	}
}
