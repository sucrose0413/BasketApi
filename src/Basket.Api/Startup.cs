﻿using System;
using Basket.Api.Models;
using Basket.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Basket.Api
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
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("basket db"));
            services.AddScoped<ApiContext>();
            services.AddScoped<BasketRepository>();

            services.AddHealthChecks(checks =>
            {
                checks.AddCheck("healthcheck",
                            () => { return HealthCheckResult.Healthy("I am healthy!"); },
                            new TimeSpan(0, 0, 5)); // 5 second cache

                // If we had a real database, I would add in a healthcheck for the database here
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Basket API", Version = "v1" });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API V1");
            });

            // var context = app.ApplicationServices.GetService<ApiContext>();
            // AddTestData(context);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void AddTestData(ApiContext context)
        {
            // place holder incase I need to add any test data
            var dummyBasket = new BasketOfItems(4);
            dummyBasket.AddUpdateOrRemoveItem(6, 3);
            dummyBasket.AddUpdateOrRemoveItem(92891, 5);
            
            context.Baskets.Add(dummyBasket);
            context.SaveChanges();
        }
    }
}
