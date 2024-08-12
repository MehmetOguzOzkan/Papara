using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.DTOs.Payment;
using Papara.Business.Mapping;
using Papara.Business.Session;
using Papara.Business.Token;
using Papara.Business.Validation;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Coupon;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.DTOs.Product;
using Papara.Business.DTOs.ProductCategory;
using Papara.Business.DTOs.User;
using Papara.Business.Job;
using Papara.Business.Message;
using Papara.Business.Notification;
using Hangfire;

namespace Papara.Business
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MediatR handlers from the specified assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));

            // Configure JWT authentication
            var jwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfig>();
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtConfig>>().Value);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });

            // Enable in-memory caching
            services.AddMemoryCache();

            // Configure SessionContext with the current HTTP context
            services.AddScoped<ISessionContext>(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>();
                var sessionContext = new SessionContext();
                sessionContext.Session = JwtManager.GetSession(context.HttpContext);
                sessionContext.HttpContext = context.HttpContext;
                return sessionContext;
            });

            // Configure Redis connection
            var redisConfig = new ConfigurationOptions
            {
                DefaultDatabase = 0,
                EndPoints = { { configuration["Redis:Host"], Convert.ToInt32(configuration["Redis:Port"]) } },
                AllowAdmin = true,
                Ssl = false,
                AbortOnConnectFail = false,
                ConnectTimeout = 30000,
                SyncTimeout = 30000

            };
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.ConfigurationOptions = redisConfig;
                opt.InstanceName = configuration["Redis:InstanceName"];
            });

            // Register AutoMapper profiles
            services.AddAutoMapper(typeof(MappingProfile));

            // Register TokenService for token management
            services.AddScoped<ITokenService, TokenService>();

            // Register FluentValidation validators
            services.AddTransient<IValidator<CategoryRequest>, CategoryRequestValidator>();
            services.AddTransient<IValidator<CouponRequest>, CouponRequestValidator>();
            services.AddTransient<IValidator<OrderRequest>, OrderRequestValidator>();
            services.AddTransient<IValidator<OrderDetailRequest>, OrderDetailRequestValidator>();
            services.AddTransient<IValidator<ProductRequest>, ProductRequestValidator>();
            services.AddTransient<IValidator<ProductCategoryRequest>, ProductCategoryRequestValidator>();
            services.AddTransient<IValidator<AuthorizationRequest>, AuthorizationRequestValidator>();
            services.AddTransient<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
            services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
            services.AddTransient<IValidator<PaymentRequest>, PaymentRequestValidator>();
            services.AddTransient<IValidator<UserRequest>, UserRequestValidator>();
            services.AddTransient<IValidator<PaymentRequestWithoutCard>, PaymentRequestWithoutCardValidator>();
            services.AddTransient<IValidator<MoneyTransferRequest>, MoneyTransferRequestValidator>();

            // Register notification services
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<EmailProcessorJob>();
            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.AddSingleton<INotificationService, NotificationService>();

            return services;
        }
    }
}
