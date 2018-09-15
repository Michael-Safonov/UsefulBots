using System.Linq;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class ChooseDeliveryDialog : AbstractAliceDialog
	{
		public ChooseDeliveryDialog()
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

			Buttons = buttons;
		}

		public string OrderCode { get; set; }

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			var deliveryId = pressedButton?.Payload?.Data?.Id;
			if (!deliveryId.HasValue)
			{
				// TODO: неправильный ввод, показать диалог доставки ещё раз
				return new InitialDialog();
			}

			AlicePersistence.UserOrders[OrderCode].DeliveryId = deliveryId.Value;

			var dialog = new ChooseActionOnOrderDialog
			{
				DeliveryId = deliveryId.Value,
				OrderCode = OrderCode
			};

			return dialog;
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseDelivery;

		public override string Title => $"Код Вашего заказа: {OrderCode}\nВыберите метод доставки";
	}
}
