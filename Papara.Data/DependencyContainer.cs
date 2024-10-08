﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Papara.Data.Context;
using Papara.Data.UnitOfWork;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Papara.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace Papara.Data
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Determine the database provider and configure the DbContext accordingly
            if (configuration.GetConnectionString("DatabaseProvider") == "MsSql")
            {
                services.AddDbContext<PaparaDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("MsSqlConnection"),
                             sqlOptions => sqlOptions.MigrationsAssembly("Papara.Data")),
                    ServiceLifetime.Transient);
            }
            else if (configuration.GetConnectionString("DatabaseProvider") == "PostgreSql")
            {
                services.AddDbContext<PaparaDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"),
                                      npgsqlOptions => npgsqlOptions.MigrationsAssembly("Papara.Data")),
                    ServiceLifetime.Transient);
            }

            // Configure Identity with custom options and integrate with the DbContext
            services.AddIdentity<User, IdentityRole<Guid>>().
                AddEntityFrameworkStores<PaparaDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Set password requirements
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 1;
            });

            // Register UnitOfWork for managing transactions and repositories
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
