﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Text;
using WeatherAttack.Application.Command.Spell;
using WeatherAttack.Application.Command.Spell.Handlers;
using WeatherAttack.Application.Command.User;
using WeatherAttack.Application.Command.User.Handlers;
using WeatherAttack.Application.Mapper.Spell;
using WeatherAttack.Application.Mapper.SpellRule;
using WeatherAttack.Application.Mapper.User;
using WeatherAttack.Contracts.Command;
using WeatherAttack.Contracts.Dtos.Spell.Request;
using WeatherAttack.Contracts.Dtos.SpellRule.Request;
using WeatherAttack.Contracts.Dtos.User.Request;
using WeatherAttack.Contracts.Dtos.User.Response;
using WeatherAttack.Contracts.interfaces;
using WeatherAttack.Contracts.Interfaces;
using WeatherAttack.Contracts.Mapper;
using WeatherAttack.Domain.Contracts;
using WeatherAttack.Domain.Entities;
using WeatherAttack.Infra;
using WeatherAttack.Infra.Repositories;
using WeatherAttack.Security.Commands;
using WeatherAttack.Security.Commands.Handlers;
using WeatherAttack.Security.Entities;
using WeatherAttack.Security.Services;

namespace Weatherattack.WebApi
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["SecuritySettings:SigningKey"])
                        )
                    };
                });

            services.Configure<SecuritySettings>(options => Configuration.GetSection("SecuritySettings").Bind(options));            

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "WeatherAttackAPI", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } } 
                });
            });

            ConfigureDatabase(services);
            ConfigureRepositories(services);
            ConfigureMappers(services);
            ConfigureCommonServices(services);
            ConfigureActionHandlers(services);

            services.AddCors();
        }

        public void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISpellRepository, SpellRepository>();
        }

        public void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("WeatherAttack");
            services.AddDbContext<WeatherAttackContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DbContext, WeatherAttackContext>(options => options.UseSqlServer(connectionString));
        }

        public void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        public void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IMapper<User, UserRequestDto, UserResponseDto>, UserEntityMapper>();
            services.AddTransient<IMapper<SpellRule, SpellRuleRequestDto, SpellRuleRequestDto>, SpellRuleEntityMapper>();
            services.AddTransient<IMapper<Spell, SpellRequestDto, SpellRequestDto>, SpellEntityMapper>();
        }

        public void ConfigureActionHandlers(IServiceCollection services)
        {
            services.AddTransient<IActionHandler<LoginCommand>, LoginActionHandler>();

            services.AddTransient<IActionHandler<AddUserCommand>, AddUserActionHandler>();
            services.AddTransient<IActionHandler<GetAllUsersCommand>, GetAllUsersActionHandler>();
            services.AddTransient<IActionHandler<GetUserCommand>, GetUserActionHandler>();
            services.AddTransient<IActionHandler<DeleteUserCommand>, DeleteUserActionHandler>();

            services.AddTransient<IActionHandler<GetSpellCommand>, GetSpellActionHandler>();
            services.AddTransient<IActionHandler<GetAllSpellsCommand>, GetAllSpellsActionHandler>();
            services.AddTransient<IActionHandler<AddSpellCommand>, AddSpellActionHandler>();
            services.AddTransient<IActionHandler<DeleteSpellCommand>, DeleteSpellActionHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseCors(builder => builder.WithOrigins("http://localhost:4500").AllowAnyMethod().AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherAttack API");
            });

            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
