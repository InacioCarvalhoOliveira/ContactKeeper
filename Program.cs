using ContactKeeper.Interfaces;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Any;
using ContactKeeper.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

builder.Services.AddScoped<IuserRepository, UserRepository>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<IunitOfWork, UnitOfWork>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<DbSession>();
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

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => 
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactKeeper API v1");
c.RoutePrefix = string.Empty;

});

    

app.MapControllers();
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseReDoc();
app.Run();

