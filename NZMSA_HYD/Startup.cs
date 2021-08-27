using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NZMSA_HYD.Data;
using NZMSA_HYD.GraphQL;
using NZMSA_HYD.GraphQL.Mutations.Days;
using NZMSA_HYD.GraphQL.Mutations.Events;
using NZMSA_HYD.GraphQL.Mutations.Users;
using NZMSA_HYD.GraphQL.Mutations.BlobSaS;
using System;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Microsoft.Extensions.Azure;

namespace NZMSA_HYD
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Fixes CORS issue 
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            // Sets up SecretClient to retrieve secret keys from Azure Key Vault
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            var client = new SecretClient(new Uri(Configuration["KeyVault:URI"]), new DefaultAzureCredential(), options);

            KeyVaultSecret azureSQL = client.GetSecret(Configuration["KeyVault:SQLConnectionString"]);
            string azureSQLConnectionString = azureSQL.Value;
            
            // Registers DBContext
            services.AddPooledDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(azureSQLConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        // Fix transient error issues due to database availability 
                        sqlOptions.EnableRetryOnFailure();
                    });
            });

            // Add authentication
            // Generates a key from our own secret key
            KeyVaultSecret JWTSecret = client.GetSecret(Configuration["KeyVault:JWTSecret"]);
            string JWTSecretStr = JWTSecret.Value;

            var key = Encoding.UTF8.GetBytes(JWTSecretStr);
            var signingKey = new SymmetricSecurityKey(key);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidIssuer = "HYD",
                                ValidAudience = "NZMSA-2021",
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = signingKey
                            };
                    });

            services.AddAuthorization();

            services
                   .AddGraphQLServer()
                   .AddAuthorization()
                   .AddQueryType(d => d.Name("Query"))
                   .AddTypeExtension<UserQueries>()
                   .AddTypeExtension<DayQueries>()
                   .AddTypeExtension<BlobSaSQueries>()
                   .AddType<UserType>()
                   .AddType<DayType>()
                   .AddType<EventType>()
                   .AddMutationType(d => d.Name("Mutation"))
                   .AddTypeExtension<UserMutation>()
                   .AddTypeExtension<EventMutation>()
                   .AddProjections();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();


            // Register GraphQl endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });

            // Voyager - GQL visualizer
            app.UseGraphQLVoyager(new VoyagerOptions()
            {
                GraphQLEndPoint = "/graphql",
            }, "/graphql-voyager");

            
        }

        
    }
}
