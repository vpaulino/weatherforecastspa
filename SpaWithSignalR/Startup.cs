using Application.Abstractions;
using Application.Abstractions.Apis;
using Application.Frontend;
using Application.Frontend.Adapters;
using Application.Frontend.Controllers;
using Application.Frontend.Services;
using Messaging.Abstractions;
using Messaging.Azure.ServiceBus.Subscriber;
using Messaging.Azure.Storage.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

namespace SpaWithSignalR
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

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<ISubscriber<string>, TopicSubscriber>((serviceProvider) => 
            {
                var logger = serviceProvider.GetRequiredService<ILogger<TopicSubscriber>>();
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>();
                return new TopicSubscriber(logger, connectionStrings.Value.TopicSubscriberConnectionSettings);
            });
            
            services.AddSingleton((serviceProvider)=> 
            {
                var hubContext = serviceProvider.GetRequiredService<IHubContext<BroadcastHub, IHubClient>>();
                var subscriber = serviceProvider.GetRequiredService<ISubscriber<string>>();
                var repository = serviceProvider.GetRequiredService<IRecomendationsRepository>();
                var logger = serviceProvider.GetRequiredService<ILogger<RecomendationsTopicAdapter>>();
                var topicAdapter = new RecomendationsTopicAdapter(subscriber, hubContext, repository, logger);
                topicAdapter.SetupAsync().GetAwaiter().GetResult();
                return topicAdapter;
            });
            
            services.AddSingleton<IPublisher<string>, QueuePublisher>((serviceProvider)=> 
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>();
                return new QueuePublisher(new MessageOptions("WeatherForecastApi", 1, TimeSpan.FromMinutes(1)), connectionStrings.Value.QueuePublisherConnectionSettings, Microsoft.WindowsAzure.Storage.LogLevel.Informational);
            });

            services.AddSingleton<IRecomendationsRepository, RecomendationsRepository>();
            services.AddSingleton<IForecastsRepository, ForecastsRepository>();
            services.AddSingleton<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IForecastWriteService, ForecastService>((serviceProvider)=> 
            {
                serviceProvider.GetRequiredService<ILocationsRepository>();
                var serv = new ForecastService(serviceProvider.GetRequiredService<ILocationsRepository>(), serviceProvider.GetRequiredService<IForecastsRepository>(), serviceProvider.GetRequiredService<IPublisher<string>>());
                serv.Startup().GetAwaiter().GetResult();
                return serv;
            });
            services.AddScoped<IForecastReadService, ForecastService>();
            services.AddSignalR();

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Certification API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpaWithSignalR API V1");
                c.RoutePrefix = "docs";
            });


            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<BroadcastHub>("/notify");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.StartupTimeout = new TimeSpan(0, 1, 0);
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });


            eventsAdapter = app.ApplicationServices.GetRequiredService<RecomendationsTopicAdapter>();

        }
        RecomendationsTopicAdapter eventsAdapter;
    }
}
