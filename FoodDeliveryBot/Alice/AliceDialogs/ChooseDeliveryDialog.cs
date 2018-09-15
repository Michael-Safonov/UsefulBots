namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class ChooseDeliveryDialog : AbstractAliceDialog
	{
		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			var deliveryId = pressedButton?.Payload?.Data?.Id;
			if (deliveryId == null)
			{
				// TODO: неправильный ввод, показать диалог доставки ещё раз
				return new InitialDialog();
			}

			var dialog = new ChooseActionOnOrderDialog
			{
				DeliveryId = deliveryId.Value
			};

			return dialog;
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseDelivery;

		public override string Title => "Выберите метод доставки";
	}
}
