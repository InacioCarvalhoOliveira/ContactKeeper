using CollabNet.API.Interfaces;
using ContactKeeper;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IuserRepository, UserRepository>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<DbSession>();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDDOC API DOCUMENTATION";
        c.SpecUrl = "/swagger/v1/swagger.json";

    });
}

app.UseHttpsRedirection();
app.UseReDoc();
app.Run();

