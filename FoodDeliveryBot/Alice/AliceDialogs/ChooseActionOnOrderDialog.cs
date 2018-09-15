using System.Collections.Generic;
using System.Linq;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	/// <summary>
	/// Диалог, который выходит после выбора доставки или ввода Id существующего диалога.
	/// </summary>
	public class ChooseActionOnOrderDialog : AbstractAliceDialog
	{
		/// <summary>
		/// Выбранный магазин.
		/// </summary>
		public int DeliveryId { get; set; }

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			// TODO: из базы
			var deliveriesAndProducts = new Dictionary<int, IdNameModel[]>
			{
				{ 1, new [] {
					new IdNameModel { Id = 1, Name = "Кинг шавуха стандарт" },
					new IdNameModel { Id = 2, Name = "Кинг шавуха большая" }
					}},

				{ 2, new [] {
					new IdNameModel { Id = 3, Name = "Дёнер шавуха стандарт" },
					new IdNameModel { Id = 4, Name = "Дёнер шавуха большая" }
					}}
			};

			var deliveryProducts = deliveriesAndProducts[DeliveryId];

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
							Buttons = buttons
						};

						break;
					}
				case ButtonType.CancelMyOrder:
					{
						// TODO: отмена заказа, вернуться в начало
						nextDialog = new InitialDialog();
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
					Type = ButtonType.CancelMyOrder
				},
				DialogType = DialogType(),
				Title = "Отменить мой заказ"
			}
		};
	}
}
