using System.Net;
using System.Text;
using BasicMessageStore.Configuration;
using BasicMessageStore.Models;
using BasicMessageStore.Models.Messages;
using BasicMessageStore.Models.Users;
using BasicMessageStore.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using IConfigurationProvider = BasicMessageStore.Configuration.IConfigurationProvider;

namespace BasicMessageStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IConfigurationProvider ConfigurationProvider { get; set; }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationProvider.TokenSecret));
            
            // Validate issuer, audience, key and expiry
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,  
                ValidateIssuer = true,
                ValidIssuer = ConfigurationProvider.TokenIssuer,
                ValidateAudience = true,
                ValidAudience = ConfigurationProvider.TokenAudience,
                ValidateLifetime = true
            };

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }
        
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(typeof(ClientProviderFilterAsync));
                options.Filters.Add(typeof(ExceptionFilter));
            });

            ConfigurationProvider = new AppSettingConfigurationProvider(Configuration);
            services.AddDbContext<MessageStoreContext>(options => options.UseInMemoryDatabase(ConfigurationProvider.ConnectionString));
            services.AddSingleton(provider => ConfigurationProvider);
            services.AddTransient<ClientProviderFilterAsync>();
            services.AddTransient<ExceptionFilter>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            // Client is request specific
            services.AddScoped<IClientProvider, ClientProvider>();
            services.AddSingleton<ITokenProvider, JWTTokenProvider>();

            ConfigureAuthentication(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}