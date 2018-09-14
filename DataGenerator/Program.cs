using System.Collections.Generic;
using Bogus;
using FoodDeliveryBot.Models;

namespace DataGenerator
{
    class Program
    {
        private static readonly string[] Food = { "Шава", "Пицца", "Роллы", "Суши", "Pepsi", "Сок", "Хот-Дог", "Пельмени" };

        static void Main(string[] args)
        {
            var deliveryServices = GetDeliveryServices(5);


        }

        private static IEnumerable<DeliveryService> GetDeliveryServices(int count, int startId = 0, int productsFrom = 3, int productsTo = 10)
        {
            var currentId = startId;

            return new Faker<DeliveryService>()
                .RuleFor(d => d.Name, f => f.Company.CompanyName())
                .RuleFor(d => d.Id, f => currentId++)
                    .RuleFor(u => u.Range,
                        f => new Faker<Product>()
                            .RuleFor(p => p.Name, f1 => f1.PickRandom(Food))
                            .RuleFor(p => p.Price, f1 => decimal.Parse(f1.Commerce.Price(70, 1000)))
                            .Generate(f.Random.Number(productsFrom, productsTo))).Generate(count);
        }
    }
}
