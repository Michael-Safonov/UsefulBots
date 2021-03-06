﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FoodDeliveryBot.DataStubs;
using FoodDeliveryBot.Db;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.AspNetCore.Mvc;

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
            await DbManager.DropCollection("DeliveryServices");

            var deliveryServices = GetDeliveryServices(count);

            foreach (var service in deliveryServices)
            {
                await deliveryServiceRepository.Insert(service);
            }

            return "Complete!";
        }

		[HttpGet("stubs")]
		public async Task<string> DropAndFillWithStubs()
		{
			// дропаем базу
			await DbManager.DropCollection("DeliveryServices");

			foreach (var service in DataStub.Deliveries)
			{
				var deliveryService = new DeliveryService
				{
					Name = service.Name,
					Products = service.Products?.Select(p => new Product
					{
						Name = p.Name,
						Desciption = p.Description,
						Price = p.Price
					}).ToList()
				};

				await deliveryServiceRepository.Insert(deliveryService);
			}

			return "Complete!";
		}

		[HttpGet("dropordersessions")]
		public async Task<string> DropOrders()
		{
			// дропаем базу
			await DbManager.DropCollection("OrderSessions");

			return "Complete!";
		}

		private static IEnumerable<DeliveryService> GetDeliveryServices(int count, int startId = 1, int productsFrom = 3, int productsTo = 10)
        {
            var currentId = startId;

            return new Faker<DeliveryService>()
                .RuleFor(d => d.Name, f => f.Company.CompanyName())
                .RuleFor(d => d.Id, f => currentId++)
                .RuleFor(u => u.Products,
                    f => new Faker<Product>()
                        .RuleFor(p => p.Name, f1 => f1.PickRandom(Food))
                        .RuleFor(p => p.Price, f1 => decimal.Parse(f1.Commerce.Price(70, 1000)))
                        .Generate(f.Random.Number(productsFrom, productsTo))).Generate(count);
        }
    }
}
