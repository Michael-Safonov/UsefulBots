using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Menues;
using FoodDeliveryBot.Models;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;

namespace FoodDeliveryBot.Dialogs
{
    public class MainMenuDialog : DialogContainer
    {
        private readonly EndOrderSessionDialog _endOrderSessionDialog;
        private readonly ProductsDialog _productsDialog;

        public const string Id = "mainMenuDialog";

        public MainMenuDialog(ProductsDialog productsDialog, EndOrderSessionDialog endOrderSessionDialog) : base(Id)
        {
            _productsDialog = productsDialog;
            _endOrderSessionDialog = endOrderSessionDialog;

            InitDialog();
        }

        private void InitDialog()
        {
            this.Dialogs.Add(Id, new WaterfallStep[]
            {
                ShowMainMenu,
                MainMenuProcessing,
                GoToFirstStep,
            });

            this.Dialogs.Add(ProductsDialog.Id, _productsDialog);
            this.Dialogs.Add(EndOrderSessionDialog.Id, _endOrderSessionDialog);
            this.Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English));
            this.Dialogs.Add("textPrompt", new TextPrompt());
        }
        private static async Task MainMenuProcessing(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var choice = (FoundChoice)args["Value"];

            switch (choice.Value)
            {
                case MainMenu.ChooseProducts:
                    await dc.Begin(ProductsDialog.Id);
                    break;
                case MainMenu.Statistics:
                    //реализовать вывод статистики
                    await next();
                    break;
                case MainMenu.CancelOrder:
                    var sessionInfo = UserState<SessionInfo>.Get(dc.Context);
                    sessionInfo.OrderSession = null;
                    await dc.End();
                    break;
                case MainMenu.FinishOrder:
                    await dc.Begin(EndOrderSessionDialog.Id);
                    break;
                default:
                    await dc.Context.SendActivity("Не понимаю.");
                    await next();
                    break;
            }
        }

        private static async Task ShowMainMenu(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            await dc.Prompt("choicePrompt", "Выберите действие:", new ChoicePromptOptions
            {
                Choices = ChoiceFactory.ToChoices(MainMenu.MenuList),
                RetryPromptActivity = MessageFactory.SuggestedActions(MainMenu.MenuList, "Пожалуйста, выберите действие") as Activity,
            });
        }
        private static async Task GoToFirstStep(DialogContext dc, IDictionary<string, object> args, SkipStepFunction next)
        {
            var sessionInfo = UserState<SessionInfo>.Get(dc.Context);

            if (sessionInfo.OrderSession != null)
            {
                await dc.Replace(Id);
            }
            else
            {
                await dc.End();
            }
        }
    }
}
