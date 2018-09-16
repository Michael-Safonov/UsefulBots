using System.Linq;
using System.Text;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class SeeMyOrderDialog : AbstractAliceDialog
	{
		public SeeMyOrderDialog(string orderCode, int deliveryId)
		{
			OrderCode = orderCode;
			DeliveryId = deliveryId;

			var order = AlicePersistence.UserOrders[OrderCode];
			if (order.Products.Any())
			{
				var sb = new StringBuilder("Ваш заказ:");
				foreach (var product in order.Products)
				{
					sb.AppendLine(product.Name);
				}

				this.Title = sb.ToString();
			}
		}

		public override bool NoAnswer => true;

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			return new ChooseActionOnOrderDialog
			{
				DeliveryId = DeliveryId,
				OrderCode = OrderCode,
			};
		}

		public override DialogType DialogType() => Alice.DialogType.SeeMyOrder;

		public string OrderCode { get; set; }

		/// <summary>
		/// Выбранный магазин.
		/// </summary>
		public int DeliveryId { get; set; }
	}
}
