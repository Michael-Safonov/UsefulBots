using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Bot.Builder.Prompts.Choices;
using FoodDeliveryBot.Models;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Adapters;

namespace FoodDeliveryBot.Dialogs
{
	public class ProductsDialog : DialogContainer
	{
		public const string Id = "productsDialog";
		public const string OrderedProductsList = "orderedProductsList";
		public static ProductsDialog Instance { get; } = new ProductsDialog(new UserOrderRepository("UserOrders"));


	    private readonly UserOrderRepository userOrderRepository;
        private readonly List<string> _actions = new List<string>
        {
            "Завершить",
            "Отменить",
        };

		private ProductsDialog(UserOrderRepository userOrderRepository) : base(Id)
		{
		    this.userOrderRepository = userOrderRepository;

			this.Dialogs.Add(Id, new WaterfallStep[]
			{
				async (dc, args, next) =>
				{
				    var productList = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Range;

					if (args is null || !args.ContainsKey(OrderedProductsList))
					{
						dc.ActiveDialog.State[OrderedProductsList] = new OrderedProducts();
					}
					else
					{
						dc.ActiveDialog.State = new Dictionary<string, object>(args);
					}
					//todo: переделать на Hero Card
					await dc.Prompt("choicePrompt", "Выбор продукта", new ChoicePromptOptions
					{
						Choices = ChoiceFactory.ToChoices(GetMainMenuWithPrice(productList))
					});
				},
				async (dc, args, next) =>
				{
				    var productList = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Range;

                    var choice = (FoundChoice)args["Value"];
					var option = GetMainMenu(productList)[choice.Index];

                    var cart = (OrderedProducts)dc.ActiveDialog.State[OrderedProductsList];

                    if (option == "Завершить")
					{
						if (cart.Count > 0)
						{
						    var sessioninfo = UserState<SessionInfo>.Get(dc.Context);
                            
                            var userOrder = new UserOrder
                            {
                                UserId = dc.Context.Activity.From.Id ?? throw new Exception("Не нашел UserId"),
								UserName = dc.Context.Activity.From.Name ?? throw new Exception("Не нашел UserName"),
								SessionId = sessioninfo.OrderSession.OrderSessionId,
                                Products = cart,
                            };

						    // sessioninfo.UserOrder = userOrder;

						    await this.userOrderRepository.Insert(userOrder);

                            await dc.Context.SendActivity("Заказ завершен. Спасибо!");							
							
							if (dc.Context.Activity.From.Id == sessioninfo.OrderSession.OwnerUserId)
								await dc.End(new Dictionary<string, object>());
							else
							{
								//await dc.Context.SendActivity("Вы отменили заказ");
								sessioninfo.UserOrder = null;
								sessioninfo.OrderSession = null;
								dc.ActiveDialog.State.Clear();
								await dc.End(null);
							}
						}
                        else
						{
							await dc.Context.SendActivity("Вы не выбрали ни одного продукта.");
							await dc.Replace(Id);
						}
					}
					else if (option == "Отменить")
					{
						await dc.Context.SendActivity("Вы отменили заказ");
						dc.ActiveDialog.State.Clear();
						await dc.End(new Dictionary<string, object>());
					}
					else
                    {
                        var product = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Range[choice.Index];

						cart.Add(product);
						 var total = cart.Sum(x => x.Price);
						//берем инфу по оредру
						//
						await dc.Context.SendActivity($"Добавлена {product.Name} ({product.Price:0.00}₽)." +
							Environment.NewLine + Environment.NewLine +
							$"Текущий заказ на сумму {total:0.00}₽.");
						await dc.Replace(Id, dc.ActiveDialog.State);
					}
				},
				//async (dc, args, next) =>
				//{
				//	await dc.Replace(Id);
				//},
			});
			this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}

	    private List<string> GetMainMenu(List<Product> products)
	    {
	        var menuItems = products.Select(x => x.Name).ToList();

	        menuItems.AddRange(this._actions);

	        return menuItems;
        }

	    private List<string> GetMainMenuWithPrice(List<Product> products)
	    {
	        var menuItems = products.Select(x => $"{x.Name} ({x.Price}₽)").ToList();

	        menuItems.AddRange(this._actions);

	        return menuItems;
	    }

        private class OrderedProducts : List<Product> { }
	}
}