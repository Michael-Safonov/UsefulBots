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
            "Продолжить"
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

                    var products = productList.Select(x => new HeroCard {
                        Title = x.Name,
                        Images = new List<CardImage>() { new CardImage("https://tashirpizza.ru/images/products/49-319.jpg") },
                        Text = x.Name,
                        Buttons = new List<CardAction> { new CardAction { Title = $"{x.Name} ({x.Price})", Value = x.Name, Type = ActionTypes.PostBack } }
                    }.ToAttachment()).ToList();

                    await dc.Context.SendActivity(MessageFactory.Carousel(products));
                },
				async (dc, args, next) =>
				{
                    var session = UserState<SessionInfo>.Get(dc.Context).OrderSession;
                    var productList = session.DeliveryService.Range;

                    var option = ((Activity)args["Activity"]).Text;
					//var option = GetMainMenu(productList)[choice];

                    var cart = (OrderedProducts)dc.ActiveDialog.State[OrderedProductsList];

                    var product = session.DeliveryService.Range.First(x => x.Name == option);

					cart.Add(product);
					 var total = cart.Sum(x => x.Price);
					//берем инфу по оредру
					//
					await dc.Context.SendActivity($"Добавлен {product.Name} (${product.Price:0.00})." +
						Environment.NewLine + Environment.NewLine +
						$"Текущий заказ на сумму ${total:0.00}.");
				},
                async (dc, args, next) =>
                {
                    await dc.Prompt("choicePrompt", "Выбор продукта", new ChoicePromptOptions
                    {
                        Choices = ChoiceFactory.ToChoices(Actions)
                    });
                },
                async (dc, args, next) =>
                {
                    var choice = (FoundChoice)args["Value"];
                    var option = Actions[choice.Index];
                    var cart = (OrderedProducts)dc.ActiveDialog.State[OrderedProductsList];
                    var session = UserState<SessionInfo>.Get(dc.Context).OrderSession;

                    if (option == "Завершить")
                    {
                        if (cart.Count > 0)
                        {
                            var total = cart.Sum(x => x.Price);
                            var receipt = new ReceiptCard
                            {
                                Title = "Текущий заказ пользователя",
                                Facts = new List<Fact>{ new Fact (key: "Order Id", value: session.OrderSessionId.ToString())},
                                Items = cart.Select(x => new ReceiptItem
                                    {
                                        Title = x.Name,
                                        Price = x.Price.ToString("0.00")
                                    }).ToList(),
                                Total = total.ToString("0.00"),
                            }.ToAttachment();
							//todo: Просто для примера, потом убрать все в статс

                            await dc.Context.SendActivity(MessageFactory.Attachment(receipt));
                            await dc.End();
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
                    else if (option == "Продолжить")
                    {
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

        private class OrderedProducts : List<Product> { }
	}
}