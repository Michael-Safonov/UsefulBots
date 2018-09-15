using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace FoodDeliveryBot.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next.Invoke(httpContext);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Exception");
				throw;
			}
		}
	}
}
