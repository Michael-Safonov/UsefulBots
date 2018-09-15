using System.Linq;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class InitialDialog : AbstractAliceDialog
	{
		public override DialogType DialogType() => Alice.DialogType.Initial;

		public override AliceButton[] Buttons => new[]
		{
			new AliceButton
			{
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.NewOrder
				},
				DialogType = DialogType(),
				Title = "Новый заказ"
			},
			new AliceButton
			{
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.ExistingOrder
				},
				DialogType = DialogType(),
				Title = "Существующий заказ"
			}
		};

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			AbstractAliceDialog nextDialog = null;
			switch (pressedButton.Payload.Type)
			{
				case ButtonType.NewOrder:
					{
						var code = AliceHelpers.CreateOrderCode();
						AlicePersistence.UserOrders[code] = new Domain.AliceOrder();
						nextDialog = GetNewOrderDialog(code);
						break;
					}
				case ButtonType.ExistingOrder:
					{
						nextDialog = GetExistingOrderDialog();
						break;
					}
				default:
					{
						// ошибка, вернуть в начало
						nextDialog = new InitialDialog();
						break;
					}
			}

			return nextDialog;
		}

		private AbstractAliceDialog GetNewOrderDialog(string orderCode)
		{
			var buttons = AliceData.Deliveries.Select(d => new AliceButton
			{
				Title = d.Name,
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.ClickOnDelivery,
					Data = d
				}
			}).ToArray();

			return new ChooseDeliveryDialog
			{
				Buttons = buttons,
				OrderCode = orderCode,
			};
		}

		private AbstractAliceDialog GetExistingOrderDialog()
		{
			return new GetOrderKeyDialog();
		}

		private string title;

		public override string Title
		{
			get { return title ?? "Выберите вариант"; }
			set { title = value; }
		}
	}
}
