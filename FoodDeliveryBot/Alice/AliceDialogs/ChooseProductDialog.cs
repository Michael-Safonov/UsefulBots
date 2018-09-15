namespace FoodDeliveryBot.Alice.AliceDialogs
{
	/// <summary>
	/// Диалог выбора продукта.
	/// </summary>
	public class ChooseProductDialog : AbstractAliceDialog
	{
		public override DialogType DialogType() => Alice.DialogType.ChooseProducts;

		public override string Title => "Выберите продукт:";

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			if (pressedButton.Payload.Type != ButtonType.ClickOnProduct)
			{
				// ошибка, вернуть в начало
				return new InitialDialog();
			}

			var productId = pressedButton.Payload.Data.Id;

			return new PrintDialog
			{
				Text = $"Вы выбрали продукт с Id: {productId}"
			};
		}
	}
}
