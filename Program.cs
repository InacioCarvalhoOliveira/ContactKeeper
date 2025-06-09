using ContactKeeper.Services.Interfaces;
using ContactKeeper.Services.Repositories;using ContactKeeper.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Any;
using ContactKeeper.Models;
using ContactKeeper;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ContactKeeper.Microservices;
using Prometheus;
using ContactKeeper.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ContactKeeper API",
            Version = "v1",
            Description = "API para Cadastro e consulta de Contatos telefônicos",
            Contact = new OpenApiContact
            {
                Name = "Inacio Carvalho de Oliveira",
                Url = new Uri("http://localhost:5059/api-docs/index.html"),
                Extensions = { { "LinkedIn", new OpenApiString("https://www.linkedin.com/in/inacio-carvalho-oliveira") } },
            }
            
        });   
        c.EnableAnnotations();   
    });        
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


builder.Services.AddScoped<IuserRepository, UserRepository>();
builder.Services.AddScoped<IUserContactRepository, UserContactRepository>();
builder.Services.AddScoped<IunitOfWork, UnitOfWork>();
builder.Services.AddScoped<HalfOpenCircuit>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddSingleton<DbSession>();
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<DataContext>(options =>
{
    var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

    if (builder.Environment.IsDevelopment())
    {
        // Use the development connection string
        options.UseSqlServer(databaseSettings?.DevelopmentConnection);
    }
    else
    {
        // Use the production connection string
        options.UseSqlServer(databaseSettings?.ProductionConnection);
    }
});

var app = builder.Build();

Console.WriteLine($"Current Environment: {builder.Environment.EnvironmentName}");

if (app.Environment.IsDevelopment())
{
     app.UseDeveloperExceptionPage();
}
else{
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDDOC API DOCUMENTATION";
        c.SpecUrl = "/swagger/v1/swagger.json";

    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactKeeper API v1");
    c.RoutePrefix = string.Empty;

});

app.UseMetricServer();
app.UseHttpMetrics();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();

app.MapControllers();
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseReDoc();
app.Run();

