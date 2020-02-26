using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Storage;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProfileImageService.Bot.Bots;
using ProfileImageService.Components.AzureStorage;
using ProfileImageService.Components.FaceApi;
using ProfileImageService.Components.PhotoProcessor;
using ProfileImageService.Components.RemoveBg;
using ProfileImageService.Settings;

namespace ProfileImageService.Bot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient.
            services.AddTransient<IBot, ProfilePictureBot>();

            services.Configure<ProfileImageServiceSettings>(_configuration);
            services.AddSingleton(r => r.GetRequiredService<IOptions<ProfileImageServiceSettings>>().Value);

            services.AddHttpClient<RemoveBgClient>();
            services.AddHttpClient<FaceApiClient>();

            services.AddTransient<PhotoProcessorService>();

            services.AddSingleton<AzureBlobStorageService>();
            services.AddSingleton(CloudStorageAccount.Parse(_configuration["BlobStorageConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("api/messages", async context => {
                   var adapter = context.RequestServices.GetService<IBotFrameworkHttpAdapter>();
                   var bot = context.RequestServices.GetService<IBot>();

                    // Delegate the processing of the HTTP POST to the adapter.
                    // The adapter will invoke the bot.
                    await adapter.ProcessAsync(context.Request, context.Response, bot);
                });
            });
        }
    }
}
