using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FoodDeliveryBot.Db;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;

namespace FoodDeliveryBot.Controllers
{
    [Produces("application/json")]
    [Route("api/FakeData")]
    public class FakeDataController : Controller
    {
        private readonly DeliveryServiceRepository deliveryServiceRepository;

        private static readonly string[] Food = { "Шава", "Пицца", "Роллы", "Суши", "Pepsi", "Сок", "Хот-Дог", "Пельмени" };

        public FakeDataController(DeliveryServiceRepository deliveryServiceRepository)
        {
            this.deliveryServiceRepository = deliveryServiceRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<DeliveryService>> GetData()
        {
            return await this.deliveryServiceRepository.GetAll();
        }

        // GET: api/FakeData
        [HttpGet("generate/{count}")]
        public async Task<string> RegenerageData(int count)
        {
            DbManager.DropCollection("DeliveryServices");

            var deliveryServices = GetDeliveryServices(count);

            foreach (var service in deliveryServices)
            {
                await deliveryServiceRepository.Insert(service);
            }

            return "Complete!";
        }

        private static IEnumerable<DeliveryService> GetDeliveryServices(int count, int startId = 1, int productsFrom = 3, int productsTo = 10)
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
