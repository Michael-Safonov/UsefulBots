namespace FoodDeliveryBot.DataStubs
{
	public static class DataStub
	{
		public static DeliveryStub[] Deliveries = new[]
		{
			new DeliveryStub
			{
				Name = "Олива Пицца",
				Url = "https://oliva13.ru/",
				Products = new []
				{
					new ProductStub
					{
						Name = "Пепперони",
						Description = "Томатный соус, колбаска пепперони, сыр моцарелла",
						Price = 550
					},
					new ProductStub
					{
						Name = "Вегетарианская",
						Description = "Томатный соус, сыр моцарелла, шампиньоны, томаты, перец болгарский, красный лук, маслины",
						Price = 500
					},
					new ProductStub
					{
						Name = "Пирог вишневый",
						Description = "Сгущенное молоко, сыр маскарпоне, вишня.",
						Price = 250
					}
				}
			},
			new DeliveryStub
			{
				Name = "Самурай",
				Url = "http://samurai-saransk.ru/",
				Products = new []
				{
					new ProductStub
					{
						Name = "Филадельфия",
						Description = "Лосось, сыр, огурец",
						Price = 199
					},
					new ProductStub
					{
						Name = "Вулкан",
						Description = "Лосось (жареный), лук зелёный, майонез, унаги соус, кунжут",
						Price = 150
					},
					new ProductStub
					{
						Name = "Мозаика №3",
						Description = "Роллы: Сяке унаги, Ика, Крит Суши: Угорь 1шт., лосось 1шт., креветка 1шт.",
						Price = 760
					}
				}
			},
			new DeliveryStub
			{
				Name = "ШаурмаKing",
				Url = "https://vk.com/shaurma_king13",
				Products = new []
				{
					new ProductStub
					{
						Name = "Шаурма",
						Description = "Курица, огурец, помидоры, капуста, соус кефирный, лаваш",
						Price = 130
					},
					new ProductStub
					{
						Name = "Данар",
						Description = "Курица, огурец, помидоры, капуста, соус кефирный, булка",
						Price = 150
					},
					new ProductStub
					{
						Name = "Бастелла",
						Description = "Курица, лук, чеснок, тесто",
						Price = 65
					}
				}
			},
			new DeliveryStub
			{
				Name = "Рука Грека",
				Url = "https://vk.com/clubgiross",
				Products = new []
				{
					new ProductStub
					{
						Name = "Гирос",
						Description = "Курица, лук, морковь, картофель фри, помидоры, огурец",
						Price = 170
					},
					new ProductStub
					{
						Name = "Греческий салат",
						Description = "Огурцы, помидоры, болгарский перец, листья салата, сыр фета, оливковое масло, бальзамический уксус",
						Price = 160
					},
					new ProductStub
					{
						Name = "Гриль-ролл",
						Description = "Курица, пшеничная тортилья, греческий соус, огурцы, помидоры",
						Price = 120
					}
				}
			},
			new DeliveryStub
			{
				Name = "Додо Пицца",
				Url = "https://dodopizza.ru/",
				Products = new []
				{
					new ProductStub
					{
						Name = "Двойная пепперони",
						Description = "Пикантная пепперони, томатный соус и моцарелла",
						Price = 385
					},
					new ProductStub
					{
						Name = "Додо",
						Description = "ветчина, говядина (фарш), пикантная пепперони, томатный соус, шампиньоны, сладкий перец, лук красный, моцарелла и маслины",
						Price = 385
					},
					new ProductStub
					{
						Name = "Морс клюквенный",
						Description = "Курица, пшеничная тортилья, греческий соус, огурцы, помидоры",
						Price = 95
					}
				}
			},
			new DeliveryStub
			{
				Name = "Дёнер Кебаб",
				Url = "https://vk.com/dener_kebab13",
				Products = new []
				{
					new ProductStub
					{
						Name = "Дёнер с Курицей",
						Description = string.Empty,
						Price = 120
					},
					new ProductStub
					{
						Name = "Ланч Бокс со Свининой",
						Description = "",
						Price = 190
					},
					new ProductStub
					{
						Name = "Датский хот-дог",
						Description = "",
						Price = 120
					}
				}
			},
		};
	}
}
