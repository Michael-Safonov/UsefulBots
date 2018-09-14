using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Dialogs
{
	public class JoinOrderDialog : DialogContainer
	{
		//todo: убрать когда сделаем взаимодействие с базой
		private const bool orderSessionExistFlag = true;

		public const string Id = "joinOrder";

		public static JoinOrderDialog Instance { get; } = new JoinOrderDialog();

		private JoinOrderDialog() : base(Id)
		{
			InitJoinOrderDialog();
		}

		private void InitJoinOrderDialog()
		{
			this.Dialogs.Add("existOrder", new WaterfallStep[]
			{
				GetOrderPincodeStep,
				CheckOrderPincodeStep
			});
			this.Dialogs.Add("textPrompt", new TextPrompt());
		}

		private async Task GetOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			await dc.Prompt("textPrompt", "Введите ключ существующего заказа:");
		}

		private async Task CheckOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var pincode = args["Value"] as string;
			//todo: Найти заказ в БД по пин-коду
			if (orderSessionExistFlag)
			{
				await dc.Begin(ProductsDialog.Id);
			}
			else
			{
				await dc.Prompt("textPrompt", "Заказа с данным ключом не существует");
			}
		}
	}
}
