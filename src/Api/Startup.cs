using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository;
using Serilog;
using Sympli.SEO.Common.Interfaces;
using Sympli.SEO.Services;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowedOrigins = "_myAllowedOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowedOrigins,
                builder =>
                {
                    var allowedOrigins = Environment
                            .GetEnvironmentVariable("FRONTEND_URL")
                            .Split(';', StringSplitOptions.RemoveEmptyEntries);
                    builder.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod(); ;
                });
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SEO API", Version = "v1" });
            });

            services.AddHttpClient();

            var connectionString = Configuration.GetConnectionString("SEODatabase");
            services.AddDbContext<SearchResultsContext>(options =>
                options.UseSqlite(connectionString));

            services.AddScoped<ISearchResultsRepo, SearchResultsRepo>();
            services.AddScoped<ISearchService, SearchService>();

            services.AddScoped<ISearchResultsProvider, GoogleSearchResultsProvider>();
            services.AddScoped<ISearchResultsProvider, BingSearchResultProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                UpdateDatabase(app);
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(); 

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SEO API V1");
            });

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(MyAllowedOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SearchResultsContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
