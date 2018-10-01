using System.Collections.Generic;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodDeliveryBot.Dialogs
{
    public class JoinOrderDialog : DialogContainer
	{
        private readonly ProductsDialog _productsDialog;
        private readonly OrderSessionRepository _orderSessionRepository;

		public const string Id = "joinOrder";

		public JoinOrderDialog(OrderSessionRepository orderSessionRepository, ProductsDialog productsDialog) : base(Id)
		{
			_orderSessionRepository = orderSessionRepository;
            _productsDialog = productsDialog;

            InitJoinOrderDialog();
        }

		private void InitJoinOrderDialog()
		{
			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				GetOrderPincodeStep,
				CheckOrderPincodeStep
			});
			this.Dialogs.Add(ProductsDialog.Id, _productsDialog);
			this.Dialogs.Add("textPrompt", new TextPrompt());
		}

		private async Task GetOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			await dc.Prompt("textPrompt", "Введите ключ существующего заказа:");
		}

		private async Task CheckOrderPincodeStep(DialogContext dc, IDictionary<string, object> args = null, SkipStepFunction next = null)
		{
			var pincode = args["Value"] as string;
            var orderSession = await _orderSessionRepository.GetByPinCode(pincode);

		    if (orderSession != null && orderSession.IsCompleted)
		    {
		        await dc.Prompt("textPrompt", "Извините. Заказ уже завершен :(");
            }
		    else if (orderSession != null && !orderSession.IsCompleted)
			{
				UserState<SessionInfo>.Get(dc.Context).OrderSession = orderSession;
                await dc.End();
			}
			else
			{
				await dc.Prompt("textPrompt", "Заказа с данным ключом не существует");
			}
		}
	}
}
