using InsightRESTAPI.Common.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsightRESTAPI.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using InsightRESTAPI.SwaggerConfig;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.IO;
using InsightRESTAPI.Services.Admin;
using InsightRESTAPI.Services;

namespace InsightRESTAPI
{
    public class Startup
    {
        public const string defaultCorsPolicy = "defaultCorsPolicy";
        public const string allowAnyCorsPolicy = "allowAnyCorsPolicy";
        public IConfiguration Configuration { get; }
        IWebHostEnvironment _environment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            // CORS policy required for accessing API from browser (e.g., from our flutter app)
            var CORSAllowedOrigins = Configuration.GetSection("CORSAllowedOrigins").Get<string[]>() ?? new string[] { };
            services.AddCors(options =>
            {
                options.AddPolicy(name: defaultCorsPolicy, builder =>
                {
                    builder.WithOrigins(CORSAllowedOrigins).AllowAnyMethod().AllowAnyHeader();
                });

                options.AddPolicy(name: allowAnyCorsPolicy, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddDbContext<ApplicationDbContext>(
                    option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //var mySettings = new GlobalConfiguration();
            services.AddConfiguration<ConnectionStringConfig>(Configuration, "ConnectionStrings");
            services.AddConfiguration<JWTSettingsConfig>(Configuration, "JWTSettings");

            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 6;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            #region Authentication & Authorization
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("JWTSettings")["Secret"]);
            #endregion
            #region Authentication Module
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("X-Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            #region Authorization Policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });
            #endregion

            services.AddMvcCore().AddNewtonsoftJson();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddMvc().AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            // Register the Swagger generator, defining 1 or more Swagger documents
            #region Swagger Region
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<SwaggerExcludeFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "REST API",
                    Version = "v1"
                });
                //c.DocumentFilter<RemoveSchemasFilter>();
                c.ExampleFilters();
                // Set the comments path for the Swagger JSON and UI.

                c.OperationFilter<SwaggerAuthorizationOperationFilter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Enter the request header in the following box to add Jwt To grant authorization Token: Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //c.SchemaFilter<SwaggerExcludeSchemaFilter>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                    });
                c.EnableAnnotations();
            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            #endregion

            #region Custom Service Registration
            services.AddScoped<JWTServices>();
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<INoteServices, NoteServices>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            //app.UseMiddleware<GCMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBlockingDetection();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelExpandDepth(2);
                c.DefaultModelsExpandDepth(-1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                //c.MaxDisplayedTags(5);
                c.ShowExtensions();
                c.EnableValidator();
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AP REST API V1");
                c.RoutePrefix = "documentation/ui/index.html";
            });
            SeedDB.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider, userManager, roleManager, context).Wait();
            app.UseRouting();
            app.UseCors(env.IsDevelopment() ? allowAnyCorsPolicy : defaultCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class ConfigurationExtension
    {
        public static void AddConfiguration<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationTag = null)
            where T : class
        {
            if (string.IsNullOrEmpty(configurationTag))
            {
                configurationTag = typeof(T).Name;
            }

            var instance = Activator.CreateInstance<T>();
            new ConfigureFromConfigurationOptions<T>(configuration.GetSection(configurationTag)).Configure(instance);
            services.AddSingleton(instance);
        }
    }
}
