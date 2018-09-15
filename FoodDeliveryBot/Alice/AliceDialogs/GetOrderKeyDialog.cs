namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class GetOrderKeyDialog : AbstractAliceDialog
	{
		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return new InitialDialog();
			}

			if (!AlicePersistence.UserOrders.TryGetValue(command, out var order))
			{
				return new PrintDialog
				{
					Text = $"Заказ с кодом {command} не найден"
				};
			}

			if (order.IsCompleted)
			{
				return new PrintDialog
				{
					Text = $"Этот заказ уже закончен. Можете начать новый заказ."
				};
			}

			return new ChooseActionOnOrderDialog
			{
				OrderCode = command,
				DeliveryId = order.DeliveryId
			};
		}

		public override DialogType DialogType() => Alice.DialogType.GetOrderKey;

		public override string Title => "Введите код существующего заказа";
	}
}
