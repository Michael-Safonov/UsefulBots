using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class UserOrder
	{
		public Guid UserId { get; set; }

        public Guid SessionId { get; set; }

		public List<Product> Products { get; set; }
	}
}
