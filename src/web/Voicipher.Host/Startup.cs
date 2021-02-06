using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Voicipher.DataAccess;
using Voicipher.Domain.Settings;
using Voicipher.Host.Configuration;
using Voicipher.Host.Extensions;
using Voicipher.Host.Filters;
using Voicipher.Host.Security;
using Voicipher.Host.Security.Extensions;
using Voicipher.Host.Utils;

namespace Voicipher.Host
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
            services.AddCors(options =>
            {
                options.AddPolicy(Constants.CorsPolicy,
                    builder => builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            var appSettingsSection = Configuration.GetSection("ApplicationSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.Configure<AppSettings>(appSettingsSection);

            // Database connection
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(appSettings.ConnectionString, providerOptions => providerOptions.CommandTimeout(60)));

            services.AddControllers();
            services.AddApiVersioning();

            // Swagger
            services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Voicipher API",
                    Version = "v2"
                });

                configuration.EnableAnnotations();
                configuration.CustomSchemaIds(tpye =>
                {
                    const string ending = "OutputModel";
                    var returnedValue = tpye.Name;
                    if (returnedValue.EndsWith(ending, StringComparison.Ordinal))
                        returnedValue = returnedValue.Replace(ending, string.Empty, StringComparison.Ordinal);

                    return returnedValue;
                });

                configuration.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                configuration.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            services.AddVoicipherAuthorization(appSettings);
            services.AddAzureAdAuthorization(appSettings);
            services.AddMvc(options =>
            {
                options.Filters.AddService<ApiExceptionFilter>();
            }).AddFilterProvider(_ =>
            {
                var azureAdAuthorizeFilter = new AuthorizeFilter(new[]
                    {new AuthorizeData {AuthenticationSchemes = Constants.AzureAdScheme}});
                var rewriteMeAuthorizeFilter = new AuthorizeFilter(new[]
                    {new AuthorizeData {AuthenticationSchemes = Constants.VoicipherScheme}});

                var filterProviderOptions = new[]
                {
                    new FilterProviderOption
                    {
                        RoutePrefix = "api/b2c",
                        Filter = azureAdAuthorizeFilter
                    },
                    new FilterProviderOption
                    {
                        RoutePrefix = "api",
                        Filter = rewriteMeAuthorizeFilter
                    }
                };

                return new AuthenticationFilterProvider(filterProviderOptions);
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Migrate database
            app.MigrateDatabase();

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v2/swagger.json", "Voicipher API V2"); });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
