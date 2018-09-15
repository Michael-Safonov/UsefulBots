using System.Linq;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	/// <summary>
	/// Диалог, который выходит после выбора доставки или ввода Id существующего диалога.
	/// </summary>
	public class ChooseActionOnOrderDialog : AbstractAliceDialog
	{
		public string OrderCode { get; set; }

		/// <summary>
		/// Выбранный магазин.
		/// </summary>
		public int DeliveryId { get; set; }

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			var deliveryProducts = AliceData.DeliveriesAndProducts[DeliveryId];

			AbstractAliceDialog nextDialog = null;
			switch (pressedButton.Payload.Type)
			{
				case ButtonType.ChooseProduct:
					{
						var buttons = deliveryProducts.Select(p => new AliceButton
						{
							Title = p.Name,
							Payload = new AliceButtonPayloadModel
							{
								Type = ButtonType.ClickOnProduct,
								Data = new IdNameModel
								{
									Id = p.Id,
									Name = p.Name
								}
							}
						}).ToArray();
						nextDialog = new ChooseProductDialog
						{
							Buttons = buttons,
							OrderCode = OrderCode,
						};

						break;
					}
				case ButtonType.SeeMyOrder:
					{
						nextDialog = new SeeMyOrderDialog
						{
							DeliveryId = DeliveryId,
							OrderCode = OrderCode,
						};
						break;
					}
				case ButtonType.EndMyOrder:
					{
						// todo: implement
						nextDialog = new EndMyOrderDialog
						{
							OrderCode = OrderCode
						};

						break;
					}
				default:
					{
						return new InitialDialog();
					}
			}

			return nextDialog;
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseActionOnOrder;

		public override string Title => $"Вы выбрали доставку Id={DeliveryId}. Что дальше?";

		public override AliceButton[] Buttons => new[]
		{
			new AliceButton
			{
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.ChooseProduct
				},
				DialogType = DialogType(),
				Title = "Выберите продукты"
			},
			new AliceButton
			{
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.SeeMyOrder
				},
				DialogType = DialogType(),
				Title = "Посмотреть мой заказ"
			},
			new AliceButton
			{
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.EndMyOrder
				},
				DialogType = DialogType(),
				Title = "Завершить мой заказ"
			}
		};
	}
}
