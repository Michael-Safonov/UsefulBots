using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Bot.Builder.Prompts.Choices;
using WelcomeBot.Models;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System;

namespace WelcomeBot.Dialogs
{
    public class ProductsDialog : DialogContainer
    {
        public const string Id = "productsDialog";

        public const string OrderedProductsList = "orderedProductsList";

        public static ProductsDialog Instance { get; } = new ProductsDialog();

        private readonly List<Product> Products = new List<Product>() {
            //todo: Получать продукты из бд. По Id заказа определять продавашку и тут подгрузить его продукты.
            new Product { Name = "Classic", Desciption = "Classic shavuha", Price = 120m},
            new Product { Name = "Non classic", Desciption = "Non classic shavuha", Price = 150m},
            //todo: вынести в константы
            new Product { Name = "Завершить"},
            new Product { Name = "Отменить"}
        };

        private List<string> MainMenu {
            get
            {
                return Products.Select(x => x.Name).ToList();
            }
        }

        private ProductsDialog() : base(Id)
        {
            this.Dialogs.Add(Id, new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
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
                        Choices = ChoiceFactory.ToChoices(MainMenu)
                    });
                },
                async (dc, args, next) =>
                {
                    var choice = (FoundChoice)args["Value"];
                    var option = Products[choice.Index];

                    var cart = (OrderedProducts)dc.ActiveDialog.State[OrderedProductsList];

                    if (option.Name == "Завершить")
                    {
                        if (cart.Count > 0)
                        {
                            var userState = UserState<UserInfo>.Get(dc.Context);
                            userState.OrderedProducts = cart;


                            //todo: Просто для примера, потом убрать все в статс
                            var total = cart.Sum(x => x.Price);
                            var message = cart.Select(x => $"{x.Name} - {x.Price}");
                            await dc.Context.SendActivity($"Order ID {userState.Order.OrderId}{Environment.NewLine}" +
                                $"{string.Join(Environment.NewLine, message.ToArray())}{Environment.NewLine}{total}");

                            await dc.End();
                        }
                        else
                        {
                            await dc.Context.SendActivity(
                                "Вы не выбрали ни одного продукта. Выбирайте блеать!");
                            await dc.Replace(Id);
                        }
                    }
                    else if (option.Name == "Отменить")
                    {
                        await dc.Context.SendActivity("Вы отменили заказ");

                        dc.ActiveDialog.State.Clear();
                        await dc.End(new Dictionary<string, object>());
                    }
                    else
                    {
                        cart.Add(option);

                        var total = cart.Sum(x => x.Price);
                        await dc.Context.SendActivity($"Добавлена {option.Name} (${option.Price:0.00})." +
                            Environment.NewLine + Environment.NewLine +
                            $"Еекущий заказ на сумму ${total:0.00}.");

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