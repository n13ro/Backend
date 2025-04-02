using Backend.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace Backend;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)  
    {
        services.AddDbContext<AppDbContext>();
        services.AddControllers();
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30); 
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title ="backend first big proj", Version = "v1" });
        });
        services.AddCors(options =>
        {

            options.AddPolicy(name: "AllowedHosts",
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                      });
        });
        
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseCors("AllowedHosts");
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}