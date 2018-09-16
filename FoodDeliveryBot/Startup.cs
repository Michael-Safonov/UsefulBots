﻿using System;
using System.IO;
using FoodDeliveryBot.Middleware;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace FoodDeliveryBot
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.CreateLogger();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddBot<EchoBot>(options =>
			{
				options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

				// The CatchExceptionMiddleware provides a top-level exception handler for your bot. 
				// Any exceptions thrown by other Middleware, or by your OnTurn method, will be 
				// caught here. To facillitate debugging, the exception is sent out, via Trace, 
				// to the emulator. Trace activities are NOT displayed to users, so in addition
				// an "Ooops" message is sent. 
				options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
				{
					Log.Error(exception, "Exception");

					await context.TraceActivity("EchoBot Exception", exception);
					await context.SendActivity("Sorry, it looks like something went wrong!");

				}));

				// The Memory Storage used here is for local bot debugging only. When the bot
				// is restarted, anything stored in memory will be gone. 
				IStorage dataStore = new MemoryStorage();

				// The File data store, shown here, is suitable for bots that run on 
				// a single machine and need durable state across application restarts.                 
				// IStorage dataStore = new FileStorage(System.IO.Path.GetTempPath());

				// For production bots use the Azure Table Store, Azure Blob, or 
				// Azure CosmosDB storage provides, as seen below. To include any of 
				// the Azure based storage providers, add the Microsoft.Bot.Builder.Azure 
				// Nuget package to your solution. That package is found at:
				//      https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/

				// IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureTableStorage("AzureTablesConnectionString", "TableName");
				// IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

				options.Middleware.Add(new ConversationState<ConversationInfo>(dataStore));
				options.Middleware.Add(new UserState<SessionInfo>(dataStore));
			});

			services.AddDirectoryBrowser();
			services.AddMvc();

			services.AddTransient<DeliveryServiceRepository>(provider => new DeliveryServiceRepository("DeliveryServices"));
			services.AddTransient<OrderSessionRepository>(provider => new OrderSessionRepository("OrderSessions"));
			services.AddTransient<UserOrderRepository>(provider => new UserOrderRepository("UserOrders"));

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDefaultFiles()
				.UseStaticFiles();

			app.UseMiddleware<ExceptionHandlingMiddleware>();

			app.UseFileServer(new FileServerOptions
			{
				FileProvider = new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), "Logs")),
				RequestPath = "/Logs",
				EnableDirectoryBrowsing = true
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseBotFramework();
		}
	}
}