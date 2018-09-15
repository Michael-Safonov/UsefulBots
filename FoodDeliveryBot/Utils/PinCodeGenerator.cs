using System;

namespace FoodDeliveryBot.Utils
{
    public static class PinCodeGenerator
    {
        public static string GetPinCode()
        {
            var random = new Random();

            return random.Next(0, 9999).ToString("0000");
        }
    }
}