using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Recognizers.Text;

namespace FoodDeliveryBot.Dialogs
{
    public class ProductsDialog : DialogContainer
	{
		public const string Id = "productsDialog";
		public const string OrderedProductsList = "orderedProductsList";

	    private readonly UserOrderRepository _userOrderRepository;
        private readonly List<string> _actions = new List<string>
        {
            "Завершить",
            "Отменить",
        };

		public ProductsDialog(UserOrderRepository userOrderRepository) : base(Id)
		{
		    _userOrderRepository = userOrderRepository;
            InitDialog();
		}

        private void InitDialog()
        {

            this.Dialogs.Add(Id, new WaterfallStep[]
            {
                ChooseProductMenu,
                WorkingWithUserCart,
            });
            this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
        }

        private async Task WorkingWithUserCart(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var productList = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Products;

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
                        UserName = dc.Context.Activity.From.Name ?? throw new Exception("Не нашел UserName. Возможно вы используете старую версию Skype"),
                        SessionId = sessioninfo.OrderSession.OrderSessionId,
                        Products = cart,
                    };

                    await _userOrderRepository.Insert(userOrder);

                    await dc.Context.SendActivity("Заказ завершен. Спасибо!");

                    if (dc.Context.Activity.From.Id == sessioninfo.OrderSession.OwnerUserId)
                        await dc.End();
                    else
                    {
                        sessioninfo.UserOrder = null;
                        sessioninfo.OrderSession = null;

                        await dc.End();
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
                var product = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Products[choice.Index];

                cart.Add(product);
                var total = cart.Sum(x => x.Price);
                //берем инфу по оредру
                //
                await dc.Context.SendActivity($"Добавлена {product.Name} ({product.Price:0.00}₽)." +
                    Environment.NewLine + Environment.NewLine +
                    $"Текущий заказ на сумму {total:0.00}₽.");
                await dc.Replace(Id, dc.ActiveDialog.State);
            }
        }

        private async Task ChooseProductMenu(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var productList = UserState<SessionInfo>.Get(dc.Context).OrderSession.DeliveryService.Products;

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