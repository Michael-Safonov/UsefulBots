using System.Linq;

namespace FoodDeliveryBot.Alice
{
	public class InitialDialog : AbstractAliceDialog
	{
		public override DialogType DialogType() => Alice.DialogType.Initial;

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			AbstractAliceDialog nextDialog = null;
			switch (pressedButton.Payload.Type)
			{
				case ButtonType.NewOrder:
					{
						nextDialog = GetNewOrderDialog();
						break;
					}
				case ButtonType.ExistingOrder:
					{
						nextDialog = GetNewOrderDialog();
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

		private AbstractAliceDialog GetNewOrderDialog()
		{
			// todo: use DeliveryService and map to IdNameModel
			var deliveries = new[]
			{
				new IdNameModel { Id = 1, Name = "Шаурма Кинг" },
				new IdNameModel { Id = 2, Name = "Дёнер" },
			};

			var buttons = deliveries.Select(d => new AliceButton
			{
				Title = d.Name,
				Payload = new AliceButtonPayloadModel
				{
					Type = ButtonType.ClickOnProduct,
					Data = d
				}
			}).ToArray();

			return new ChooseDeliveryDialog
			{
				Buttons = buttons
			};
		}

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

		public override string Title() => "Выберите вариант";
	}

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
				DeliveryId = deliveryId ?? -1
			};

			return dialog;
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseDelivery;

		public override string Title() => "Выберите метод доставки";
	}

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
			switch (pressedButton)
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseActionOnOrder;

		public override string Title() => $"Вы выбрали доставку Id={DeliveryId}. Что дальше?";

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
