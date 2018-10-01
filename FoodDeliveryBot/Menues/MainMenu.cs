using System.Collections.Generic;

namespace FoodDeliveryBot.Menues
{
    public static class MainMenu
    {
        public const string ChooseProducts = "Выбор продуктов";
        public const string Statistics = "Статистика";
        public const string CancelOrder = "Отменить заказ";
        public const string FinishOrder = "Завершить заказ";

        public static List<string> MenuList = new List<string> {ChooseProducts, Statistics, CancelOrder, FinishOrder};

    }
}