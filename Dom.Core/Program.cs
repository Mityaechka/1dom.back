using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Taxiverk.Domain.Mongo;
using Taxiverk.Infrastructure.CacheService;
using Taxiverk.Infrastructure.CacheWrapper;
using Taxiverk.Infrastructure.Http;
using Taxiverk.Infrastructure.Jwt;
using Taxiverk.Infrastructure.MinioService;
using Taxiverk.Infrastructure.Random;
using Taxiverk.Infrastructure.Sql;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

logger.Debug("init app");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    
    var services = builder.Services;
    services.AddMvcCore();

    services.AddDbContext<SqlContext>(options =>
        options.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("Npgsql"),
            x => x.MigrationsAssembly("Dom.Core")));

    services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Path));
    services.Configure<RedisCacheService.Options>(builder.Configuration.GetSection(RedisCacheService.Options.Path));
    services.Configure<MongoContext.Options>(builder.Configuration.GetSection(MongoContext.Options.Path));
    services.Configure<MinioFactory.Options>(builder.Configuration.GetSection(MinioFactory.Options.Path));

    services.AddTransient<ICacheService, RedisCacheService>();
    services.AddSingleton<ICacheWrapper, CacheWrapper>();
    
    services.AddTransient<MongoContext>();  
    
    services.AddTransient<MinioFactory>();
    services.AddTransient<MinioObjectFileStrategy>(sp => MinioObjectFileStrategies.TimestampStrategy);
    services.AddTransient(typeof(MinioBucketProvider<>));
    services.AddSingleton<TestMinioBucket>();

    services.AddSingleton<JwtService>();
    
    services.AddSingleton<Http>();
    
    services.AddScoped<IRandomService, MockRandomService>();
    
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
        c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Scheme = "bearer"
        });
        c.OperationFilter<AuthenticationRequirementsOperationFilter>();
    });

    services.AddControllers().AddNewtonsoftJson();

    var app = builder.Build();
    
    using (var scope = app.Services.CreateScope())
    {
        var sqlContext = scope.ServiceProvider.GetService<SqlContext>();
        sqlContext.Database.Migrate();
    }
    
    app.UseHttpLogging();
    
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
    app.UseHsts();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseMiddleware<JwtMiddleware>();
    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

    
    app.Run("http://localhost:9104");
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
