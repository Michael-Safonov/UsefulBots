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
using Microsoft.Bot.Builder.Adapters;

namespace FoodDeliveryBot.Dialogs
{
	public class ProductsDialog : DialogContainer
	{
		public const string Id = "productsDialog";
		public const string OrderedProductsList = "orderedProductsList";
		public static ProductsDialog Instance { get; } = new ProductsDialog();

        private readonly List<string> Actions = new List<string>
        {
            "Завершить",
            "Отменить",
        };

		private ProductsDialog() : base(Id)
		{
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
						Choices = ChoiceFactory.ToChoices(GetMainMenu(productList))
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
							//var userState = UserState<UserInfo>.Get(dc.Context);
							//userState.OrderedProducts = cart;
							////todo: Просто для примера, потом убрать все в статс
							//var total = cart.Sum(x => x.Price);
							//var message = cart.Select(x => $"{x.Name} - {x.Price}");
							//await dc.Context.SendActivity($"Order ID {userState.Order.OrderId}{Environment.NewLine}" +
							//	$"{string.Join(Environment.NewLine, message.ToArray())}{Environment.NewLine}{total}");
							// await dc.End();
						}
						else
						{
							await dc.Context.SendActivity(
								"Вы не выбрали ни одного продукта.");
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
						await dc.Context.SendActivity($"Добавлена {product.Name} (${product.Price:0.00})." +
							Environment.NewLine + Environment.NewLine +
							$"Текущий заказ на сумму ${total:0.00}.");
						 await dc.Replace(Id, dc.ActiveDialog.State);
					}
				},
				async (dc, args, next) =>
				{
					await dc.Replace(Id);
				},
			});
			this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
		}

	    private List<string> GetMainMenu(List<Product> products)
	    {
	        var menuItems = products.Select(x => x.Name).ToList();

	        menuItems.AddRange(this.Actions);

	        return menuItems;
        }
        
        private class OrderedProducts : List<Product> { }
	}
}