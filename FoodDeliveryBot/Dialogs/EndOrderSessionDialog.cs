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
	public class EndOrderSessionDialog : DialogContainer
	{
		private readonly OrderSessionRepository orderSessionRepository;

		public const string Id = "endOrderSession";

		public static EndOrderSessionDialog Instance { get; } = new EndOrderSessionDialog(new OrderSessionRepository("OrderSessions"));

		private EndOrderSessionDialog(OrderSessionRepository orderSessionRepository) : base(Id)
		{
			this.orderSessionRepository = orderSessionRepository;
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
				//todo:собрать статистику сессии и красиво показать
				await dc.Prompt("textPrompt", $"Итого: {orderSession.DeliveryService.Name}");

				// Сохраняем завершенный заказ
				orderSession.IsCompleted = true;
				await this.orderSessionRepository.Upsert(orderSession);
				UserState<SessionInfo>.Get(dc.Context).OrderSession = null;
				await dc.End();
			}
			else
			{
				await dc.Prompt("textPrompt", "Вы не являетесь администратором заказа");
			}
		}
	}
}
