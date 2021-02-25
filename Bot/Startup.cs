// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.11.1

using Bot.Bots;
using Bot.Dialogs;
using Bot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson();

			// Create the Bot Framework Adapter with error handling enabled.
			services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

			// Configure BotServices
			services.AddSingleton<BotServices>();

			// Configure State
			ConfigureState(services);

			// Configure Dialogs;
			ConfigureDialogs(services);

			// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
			services.AddTransient<IBot, EchoBot<MainDialog>>();
		}

		public void ConfigureDialogs(IServiceCollection services)
		{
			services.AddSingleton<MainDialog>();
		}

		public void ConfigureState(IServiceCollection services)
		{
			// Create the storage we'll be using for User and Conversation state.
			services.AddSingleton<IStorage>(new BlobsStorage(Configuration["ConnectionString:Account"], Configuration["ConnectionString:Container"]));

			// Create the User State.
			services.AddSingleton<UserState>();

			// Create the Conversation State.
			services.AddSingleton<ConversationState>();

			// Create an instance of the State Service
			services.AddSingleton<StateService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDefaultFiles()
				 .UseStaticFiles()
				 .UseWebSockets()
				 .UseRouting()
				 .UseAuthorization()
				 .UseEndpoints(endpoints =>
				 {
					 endpoints.MapControllers();
				 });

			// app.UseHttpsRedirection();
		}
	}
}
