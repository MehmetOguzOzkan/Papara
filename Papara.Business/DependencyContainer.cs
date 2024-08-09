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

namespace Papara.Business
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));

            // JWT Configuration
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

            // Memory Cache
            services.AddMemoryCache();

            // SessionContext
            services.AddScoped<ISessionContext>(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>();
                var sessionContext = new SessionContext();
                sessionContext.Session = JwtManager.GetSession(context.HttpContext);
                sessionContext.HttpContext = context.HttpContext;
                return sessionContext;
            });

            // Redis
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

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // ITokenService
            services.AddScoped<ITokenService, TokenService>();

            // FluentValidation
            //services.AddFluentValidationAutoValidation()
            //    .AddFluentValidationClientsideAdapters()
            //    .AddValidatorsFromAssemblyContaining<PaymentRequestValidator>();

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

            return services;
        }
    }
}
