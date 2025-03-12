using backend.Database;
using static Backend.DTOs.AuthDto;

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
        services.AddCors(options =>
        {
            
            options.AddPolicy(name: "AllowAllOrigins",
                              builder =>
                              {
                                  builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                              });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}