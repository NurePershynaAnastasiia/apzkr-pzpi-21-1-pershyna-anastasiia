using GreenGuard.BuildInjections;
using GreenGuard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GreenGuardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GreenGuardDatabase")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:7042", "http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("CultureInvariant");

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwtAuthorization();

builder.Services.AddLogging();
builder.Services.AddSetSecurity(builder.Configuration);

builder.Services.AddServices();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
});

var app = builder.Build();


app.UseCors("AllowSpecificOrigin");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenGuard V1");
        c.RoutePrefix = "";
        c.OAuthClientId("swagger");
        c.OAuthAppName("Swagger UI");
    });
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


/*
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Worker}/{action=Login}/{id?}");
*/


app.Run();