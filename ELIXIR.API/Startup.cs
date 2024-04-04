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
using ELIXIR.DATA.SERVICES;
using System.Text.Json.Serialization;
using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ORDERING_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY;
using static ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions.AddNewChecklistQuestions;

namespace ELIXIR.API;

public class Startup
{
    private readonly string _policyName = "CorsPolicy";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //     services.AddControllers()
        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        
        //    options.JsonSerializerOptions.MaxDepth = 32;
        //});

        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(AddNewChecklistQuestionCommand).Assembly));
        services.AddMediatR(x =>
            x.RegisterServicesFromAssemblies(typeof(GetAllChecklists.GetAllChecklistsQuery).Assembly));
        services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

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
        services.AddScoped(typeof(IUserService), typeof(UserService));
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IRawMaterialRepository, RawMaterialRepository>();
        services.AddScoped<IOrdering, OrderingRepository>();

        services.AddDbContext<StoreContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("LiveConnectionDevelopment")));

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

        //services.AddHttpsRedirection(options =>
        //{
        //    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
        //    options.HttpsPort = 82;
        /// sample
        //});

        services.AddApplicationServices();

        services.AddSwaggerDocumentation();

        services.AddCors(opt =>
        {
            opt.AddPolicy(name: _policyName, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllers();
        services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        System.IO.Directory.SetCurrentDirectory(env.ContentRootPath);

        app.UseStatusCodePagesWithReExecute("/errors/{0}");

        app.UseHsts();

        app.UseRouting();

        app.UseCors(_policyName);

        app.UseAuthorization();
        //app.ApplyMigrations();

        app.UserSwaggerDocumentation();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSwagger();
            endpoints.MapControllers();
            endpoints.MapHub<OrderHub>("moveorder");
        });
    }
}