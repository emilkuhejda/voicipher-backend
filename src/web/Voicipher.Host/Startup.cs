using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Voicipher.Business.BackgroundServices;
using Voicipher.Business.Polling;
using Voicipher.Business.Services;
using Voicipher.DataAccess;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Settings;
using Voicipher.Host.Configuration;
using Voicipher.Host.Extensions;
using Voicipher.Host.Filters;
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
            var appSettingsSection = Configuration.GetSection("ApplicationSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            // Enable CORS
            services.AddCors(options =>
            {
                options.AddPolicy(Constants.CorsPolicy,
                    builder => builder
                        .WithOrigins(appSettings.AllowedHosts)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            services.Configure<AppSettings>(appSettingsSection);

            // Database connection
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(appSettings.ConnectionString, providerOptions => providerOptions.CommandTimeout(300)));

            // SignalR
            services.AddSignalR();

            services.AddControllers();
            services.AddApiVersioning();

            // Swagger
            services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Voicipher API",
                    Version = "v1"
                });

                configuration.EnableAnnotations();
                configuration.CustomSchemaIds(tpye =>
                {
                    const string ending = "OutputModel";
                    var returnedValue = tpye.Name;
                    if (returnedValue.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
                        returnedValue = returnedValue.Replace(ending, string.Empty, StringComparison.OrdinalIgnoreCase);

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
            services.AddMvcCore(options =>
            {
                options.Filters.AddService<ApiExceptionFilter>();
            });

            // Hosted services
            services.AddHostedService<MailService>();
            services.AddHostedService<AudioFileProcessingService>();
            services.AddHostedService<RestoreRecognitionStateService>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }

        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }

        public void ConfigureDevelopmentContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterType<FakeSpeechRecognitionService>().As<ISpeechRecognitionService>();
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

            // Clean temporary data
            app.CleanTemporaryData();

            // Enable CORS
            app.UseCors(Constants.CorsPolicy);

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Voicipher API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/api/message-hub");
            });
        }
    }
}
