using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
           .Build();
// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
