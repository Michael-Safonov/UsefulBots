using System.Collections.Generic;
using System.Linq;
using Serilog;

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
					Type = ButtonType.ClickOnDelivery,
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

		private string title;

		public override string Title
		{
			get { return title ?? "Выберите вариант";  }
			set { title = value; }
		}
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
				DeliveryId = deliveryId.Value
			};

			return dialog;
		}

		public override DialogType DialogType() => Alice.DialogType.ChooseDelivery;

		public override string Title => "Выберите метод доставки";
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

	/// <summary>
	/// Диалог для печати чего-нибудь.
	/// </summary>
	public class PrintDialog : AbstractAliceDialog
	{
		public override bool NoAnswer => true;

		public string Text { get; set; }

		public override DialogType DialogType() => Alice.DialogType.Print;

		public override string Title => Text;

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			Log.Debug("PrintDialog");
			Log.Debug($"Title:{Title}");
			// todo: м.б. настроить возвращение в любое место
			return new InitialDialog();
		}
	}
}
