using Cundi.XAF.ApiKey.Api.Extensions;
using Cundi.XAF.SyncReceiver.Extensions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using DevExpress.ExpressApp.WebApi.Services;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Receiver.WebApi.JWT;
using System.Text;

namespace Receiver.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationTokenProvider, JwtTokenProviderService>();

        services.AddXafWebApi(builder =>
        {
            builder.AddXpoServices();

            builder.ConfigureOptions(options =>
            {
                // Expose SyncedTriggerDemo via OData (read-only access for viewing synced data)
                options.BusinessObject<Receiver.Module.BusinessObjects.SyncedTriggerDemo>();
            });

            builder.Modules
                .AddValidation()
                .Add<Receiver.Module.ReceiverModule>()
                .Add<Cundi.XAF.ApiKey.Api.ApiKeyApiModule>()
                .Add<Cundi.XAF.SyncReceiver.Api.SyncReceiverApiModule>(); // Add SyncReceiver API module

            builder.ObjectSpaceProviders
                .AddSecuredXpo((serviceProvider, options) =>
                {
                    string? connectionString = Configuration.GetConnectionString("ConnectionString");
                    ArgumentNullException.ThrowIfNull(connectionString);
                    options.ConnectionString = connectionString;
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
                .AddNonPersistent();

            builder.Security
                .UseIntegratedMode(options =>
                {
                    options.Lockout.Enabled = true;
                    options.RoleType = typeof(PermissionPolicyRole);
                    options.UserType = typeof(Receiver.Module.BusinessObjects.ApplicationUser);
                    options.UseXpoPermissionsCaching();
                    options.Events.OnSecurityStrategyCreated += securityStrategy =>
                    {
                        ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.CacheOnFirstAccess;
                    };
                })
                .AddPasswordAuthentication(options =>
                {
                    options.IsSupportChangePassword = true;
                });

            builder.AddBuildStep(application =>
            {
                application.ApplicationName = "SetupApplication.Receiver";
                application.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached && application.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
                {
                    application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
                    application.DatabaseVersionMismatch += (s, e) =>
                    {
                        e.Updater.Update();
                        e.Handled = true;
                    };
                }
#endif
            });
        }, Configuration);

        // Register SyncReceiver services (SyncTypeMappings and SyncService)
        // Type mappings are configured via the SyncTypeMappingConfig entity in the XAF UI
        services.AddSyncReceiver();

        services
            .AddControllers()
            .AddOData((options, serviceProvider) =>
            {
                options
                    .AddRouteComponents("api/odata", new EdmModelBuilder(serviceProvider).GetEdmModel())
                    .EnableQueryFeatures(100);
            });

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"]!)),
                    AuthenticationType = JwtBearerDefaults.AuthenticationScheme
                };
            })
            .AddApiKey();

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme,
                Cundi.XAF.ApiKey.Api.Authentication.ApiKeyAuthenticationHandler.SchemeName)
                    .RequireAuthenticatedUser()
                    .RequireXafAuthentication()
                    .Build();
        });

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Receiver API",
                Version = "v1",
                Description = @"Receiver WebApi for receiving sync data from Cundi.XAF.Triggers webhooks."
            });
            c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Name = "Bearer",
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "X-API-Key",
                In = ParameterLocation.Header,
                Description = "API Key authentication. Enter your API Key in the format: cak_xxxxx"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme() {
                        Reference = new OpenApiReference() {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "JWT"
                        }
                    },
                    new string[0]
                },
                {
                    new OpenApiSecurityScheme() {
                        Reference = new OpenApiReference() {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new string[0]
                },
            });
        });

        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Receiver WebApi v1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapXafEndpoints();
        });
    }
}
