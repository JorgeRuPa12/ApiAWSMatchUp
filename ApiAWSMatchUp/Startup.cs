using ApiAWSMatchUp.Helpers;
using ApiAWSMatchUp.Data;
using ApiAWSMatchUp.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using ApiAWSMatchUp.Models;

namespace ApiAWSMatchUp;

public class Startup
{
    private HelperActionServicesOAuth helper;
    private Secrets keys;
    public Startup(IConfiguration configuration)
    {
        string secretJson = HelperSecretManager.GetSecretAsync().GetAwaiter().GetResult(); 
        keys = JsonConvert.DeserializeObject<Secrets>(secretJson);
        string secretKey = keys.Secretkey;
        Configuration = configuration;
        helper = new HelperActionServicesOAuth(configuration, secretKey);
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(keys);
        services.AddTransient<HelperSecretManager>();
        services.AddSingleton<HelperActionServicesOAuth>(helper);
        services.AddAuthentication(helper.GetAuthenticateSchema()).AddJwtBearer(helper.GetJwtBearerOptions());


        // Add services to the container.
        //string connectionString = keys.MySql;
        string connectionString = "server=matchupmysqljrp.cw9ksg8u4fgi.us-east-1.rds.amazonaws.com;port=3306;user id=adminsql;password=Admin123;database=matchup";

        services.AddControllers();
        services.AddTransient<RepositoryMatchUp>();
        services.AddDbContext<MatchUpContext>(options => options.UseMySQL(connectionString));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", x => x.AllowAnyOrigin());
        });
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Api AWS MatchUp", Version = "v1" });
        });
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(options => options.AllowAnyOrigin());

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(url: "swagger/v1/swagger.json"
            , "ApiAWSMatchUp");
            options.RoutePrefix = "";
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}