using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ELIXIR.API.EXTENSIONS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.JWT.SERVICES;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ELIXIR.API.ERRORS;
using System.Net;
using Microsoft.OpenApi.Models;

namespace ELIXIR.API
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

          services.AddControllers();
          services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(jwtOptions =>
           {
               var key = Configuration.GetValue<string>("JwtConfig:Key");
               var keyBytes = Encoding.ASCII.GetBytes(key);

               jwtOptions.SaveToken = true;
               jwtOptions.TokenValidationParameters = new TokenValidationParameters
               {

                   IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                   ValidateLifetime = true,
                   ValidateAudience = false,
                   ValidateIssuer = false,
                   ClockSkew = TimeSpan.Zero
               };

           });
            services.AddTransient(typeof(IUserService), typeof(UserService));
      
            services.AddDbContext<StoreContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                                  .Where(e => e.Value.Errors.Count > 0)
                                  .SelectMany(x => x.Value.Errors)
                                  .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationError
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 5001;
            });
            
            services.AddApplicationServices();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //});

            services.AddSwaggerDocumentation();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            System.IO.Directory.SetCurrentDirectory(env.ContentRootPath);

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UserSwaggerDocumentation();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSwagger();
                endpoints.MapControllers();
            });
        }
    }
}
