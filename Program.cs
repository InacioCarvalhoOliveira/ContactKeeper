using ContactKeeper;
using ContactKeeper.Services.Repositories;
using ContactKeeper.Services.Interfaces;
using ContactKeeper.Data;
using ContactKeeper.Microservices;
using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Prometheus;
using RabbitMQ.Client;

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

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
     c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddScoped<IunitOfWork, UnitOfWork>();
builder.Services.AddScoped<IuserRepository, UserRepository>();
builder.Services.AddScoped<IUserContactRepository, UserContactRepository>();
//builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddSingleton<HalfOpenCircuit>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddSingleton<DbSession>();
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConnection>(provider =>
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    return (IConnection)factory.CreateConnectionAsync();
});

builder.Services.AddDbContext<DataContext>(options =>
{
    var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

    if (builder.Environment.IsDevelopment())
    {
        // Use the development connection string
        options.UseSqlServer(databaseSettings?.ConnectionString);
    }
    else
    {
        // Use the production connection string
        options.UseSqlServer(databaseSettings?.Production);
    }
});


var key = System.Text.Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
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

app.UseCors("AllowAll");
app.UseSwagger();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMetricServer();
app.UseHttpMetrics();
app.UseReDoc();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    try
    {
        if (app.Environment.IsDevelopment())
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex}");
        throw;
    }
}

app.Run();
