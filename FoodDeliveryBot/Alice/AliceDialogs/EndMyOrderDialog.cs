using System.Text;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class EndMyOrderDialog : AbstractAliceDialog
	{
		public override bool NoAnswer => true;

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			var order = AlicePersistence.UserOrders[OrderCode];
			var sb = new StringBuilder("Ваш заказ закрыт:");

			foreach (var product in order.Products)
			{
				sb.AppendLine(product.Name);
			}

			this.Title = sb.ToString();

			AlicePersistence.UserOrders[OrderCode].IsCompleted = true;

			return new InitialDialog();
		}

		public override DialogType DialogType() => Alice.DialogType.SeeMyOrder;

		public string OrderCode { get; set; }
	}
}
