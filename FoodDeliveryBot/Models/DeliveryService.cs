using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class DeliveryService
	{
		public int Id { get; set; }
		public string Name { get; set; }
        public string Phone { get; set; } = "+79375116095";
	    public List<Product> Range { get; set; }

        //todo: add work time and additional information for delivery service
    }
}
